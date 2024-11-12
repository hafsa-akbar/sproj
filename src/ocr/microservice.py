import base64
import hashlib
import json

from dateutil import parser
from flask import Flask, request, jsonify
from google.cloud import documentai_v1 as documentai
import os
import re

app = Flask(__name__)

CACHE_FILE = "cache.json"

def load_cache():
    if os.path.exists(CACHE_FILE):
        with open(CACHE_FILE, "r") as file:
            return json.load(file)
    return {}

def save_cache(cache):
    with open(CACHE_FILE, "w") as file:
        json.dump(cache, file)

def get_cache_key(image_data):
    return hashlib.sha256(image_data).hexdigest()

def initialize_document_ai_client():
    return documentai.DocumentProcessorServiceClient()


def layout_to_text(layout, text):
    return "".join(
        text[int(segment.start_index): int(segment.end_index)]
        for segment in layout.text_anchor.text_segments
    ).strip(" .")


def get_key_value_pairs(result):
    text = result.document.text
    fields = {}
    for page in result.document.pages:
        for field in page.form_fields:
            name = layout_to_text(field.field_name, text)
            value = layout_to_text(field.field_value, text)
            fields[name.strip().lower()] = value.strip()
    return fields


def reformat_date(date_str):
    try:
        parsed_date = parser.parse(date_str, dayfirst=True)
        return parsed_date.strftime("%Y-%m-%d")
    except Exception as e:
        return None


def extract_identity_details(fields):
    details = {"birthdate": None, "expirydate": None, "licenseNumber": None, "cnic": None}

    for key, value in fields.items():
        if re.search(r"\b(dob|date of birth|birthdate)\b", key, re.IGNORECASE):
            details["birthdate"] = reformat_date(value)
        elif re.search(r"\bexpiry\b", key, re.IGNORECASE):
            details["expirydate"] = reformat_date(value)
        elif re.search(r"\b(cnic|national id|id number|identity)\b", key, re.IGNORECASE):
            details["cnic"] = value
        elif re.search(r"\blicense\b", key, re.IGNORECASE):
            details["licenseNumber"] = value

    return details


def process_document(client, project_id, location, ids, image):
    fp, proofing = ids

    def ocr(processor_id):
        name = client.processor_path(project_id, location, processor_id)
        document = {"content": image, "mime_type": "image/png"}
        request = {"name": name, "raw_document": document}
        return client.process_document(request=request)

    fp_result = ocr(fp)
    fields = get_key_value_pairs(fp_result)
    id_details = extract_identity_details(fields)

    id_proof_result = ocr(proofing)
    id_details['id_proofing'] = all(
        entity.mention_text == 'PASS' for entity in id_proof_result.document.entities
    )

    return id_details


@app.route('/check_identity', methods=['POST'])
def check_identity():
    image = request.files['image']
    if not image:
        return jsonify({"error": "No image provided"}), 400

    image_data = image.read()
    cache_key = get_cache_key(image_data)

    cache = load_cache()
    if cache_key in cache:
        return jsonify(cache[cache_key]), 200

    image_base64 = base64.b64encode(image_data).decode("utf-8")

    try:
        client = initialize_document_ai_client()
        project_id = "713882766306"
        location = "us"
        fp = "c92b67b5117b5320"
        id_proof = "c2cca91b6637391a"

        id_details = process_document(client, project_id, location, (fp, id_proof), image_base64)
        cache[cache_key] = id_details
        save_cache(cache)

        return jsonify(id_details), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    script_directory = os.path.dirname(os.path.abspath(__file__))
    os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = os.path.join(script_directory, "documentAI_SA.json")

    app.run(host='127.0.0.1', port=1811)
