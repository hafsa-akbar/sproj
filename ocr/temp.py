from google.cloud import documentai_v1 as documentai

# run ❯ export GOOGLE_APPLICATION_CREDENTIALS="/Users/hafsaakbar/Desktop/sproj/ocr/documentAI_SA.json" on terminal before running code
# u can also look into setting up default creds here: https://cloud.google.com/docs/authentication/provide-credentials-adc#local-dev
# i need to figure out how to extract key-val pairs using the form parser api
# once I do, the idea is, we store info like cnic, drivers license, DOB (match agaisnt the user entered DOB), expiry (match against time.now)

def initialize_document_ai_client():
    return documentai.DocumentProcessorServiceClient()

def layout_to_text(layout, text):
    return "".join(
        text[int(segment.start_index) : int(segment.end_index)]
        for segment in layout.text_anchor.text_segments
    )

def get_key_value_pairs(result):
    text = result.document.text
    for page in result.document.pages:
        for field in page.form_fields:
            name = layout_to_text(field.field_name, text)
            value = layout_to_text(field.field_value, text)
            print(f"    * {repr(name.strip())}: {repr(value.strip())}")

def process_document(client, project_id, location, processor_id, image_path, key_value_pairs=False):
    name = client.processor_path(project_id, location, processor_id)

    with open(image_path, "rb") as image_file:
        document_content = image_file.read()

    document = {"content": document_content, "mime_type": "image/png"} 
    request = {"name": name, "raw_document": document}

    result = client.process_document(request=request)
    if key_value_pairs:
        get_key_value_pairs(result)

    return result.document

project_id = "590344950484"         
location = "us"            
form_parser = "3255425ad2e217bc"     
id_proofing_processor_id = "ecb806491cb77272"
image_path = 'license.png'            

client = initialize_document_ai_client()
print("Extracting Fields...")
ocr_document = process_document(client, project_id, location, form_parser, image_path, True)

print("Processing Identity Document Proofing...")
proofing_document = process_document(client, project_id, location, id_proofing_processor_id, image_path)
for e in proofing_document.entities:
    print(f'{e.type_}: {e.mention_text}')