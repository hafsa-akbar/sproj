import { sessionId } from './stores';

const BASE_URL = 'http://localhost:5000';

let currentSessionId;
sessionId.subscribe(value => {
  currentSessionId = value;
});

function getAuthHeaders() {
  const headers = {
    'Content-Type': 'application/json'
  };
  
  if (currentSessionId) {
    headers['Cookie'] = `session=${currentSessionId}`;
  }
  
  return headers;
}

function parseErrorMessage(data, defaultMessage) {
  console.log(data);
  let errorMessage = data.message || defaultMessage;
  if (data.errors && Array.isArray(data.errors)) {
    errorMessage += ': ' + data.errors[0]['reason'];
  }
  return errorMessage;
}

function getSessionIdFromCookie() {
  const match = document.cookie.match(/session=([^;]+)/);
  return match ? match[1] : null;
}

export async function login(user) {
  const response = await fetch(`${BASE_URL}/users/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(user)
  });

  let text = await response.text();
  let data = text ? JSON.parse(text) : {};

  if (!response.ok) {
    throw new Error('Login failed');
  }

  const sid = getSessionIdFromCookie();
  if (sid) {
    data.sessionId = sid;
  }

  return data;
}

export async function register(user) {
  const response = await fetch(`${BASE_URL}/users/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(user)
  });

  const data = await response.json();

  if (!response.ok) {
    throw new Error(parseErrorMessage(data, 'Signup failed'));
  }

  const sid = getSessionIdFromCookie();
  if (sid) {
    data.sessionId = sid;
  }

  return { user: data };
}

export async function startSmsVerification() {
  const response = await fetch(`${BASE_URL}/verify/start-sms`, {
    method: 'POST',
    headers: getAuthHeaders()
  });
  
  let text = await response.text();
  let data = text ? JSON.parse(text) : {};

  if (!response.ok) {
    throw new Error(parseErrorMessage(data, 'Failed to send code'));
  }
  
  return data;
}

export async function verifySmsCode(code) {
  const response = await fetch(`${BASE_URL}/verify/end-sms`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify({ code })
  });
  
  let text = await response.text();
  let data = text ? JSON.parse(text) : {};

  if (!response.ok) {
    throw new Error(parseErrorMessage(data, 'Failed to verify'));
  }

  return data;
}

export async function verifyCnic(file) {
  const formData = new FormData();
  formData.append('cnic', file);

  const headers = {};
  if (currentSessionId) {
    headers['Cookie'] = `session=${currentSessionId}`;
  }

  const response = await fetch(`${BASE_URL}/verify/cnic`, {
    method: 'POST',
    headers,
    body: formData
  });

  if (!response.ok) {
    throw new Error('Upload failed');
  }

  return response;
}

export async function getJobs() {
  const res = await fetch(`${BASE_URL}/jobs`, {
    headers: getAuthHeaders()
  });
  if (!res.ok) {
    const text = await res.text();
    const err = text ? JSON.parse(text) : {};
    throw new Error(parseErrorMessage(err, 'Failed to load jobs'));
  }
  return await res.json();
}

export async function getWorkerDetails(jobId) {
  const res = await fetch(`${BASE_URL}/jobs/${jobId}`, {
    headers: getAuthHeaders()
  });
  if (!res.ok) {
    const text = await res.text();
    const err = text ? JSON.parse(text) : {};
    throw new Error(parseErrorMessage(err, 'Failed to load worker details'));
  }
  return await res.json();
}

export async function createJob(jobData) {
  const response = await fetch(`${BASE_URL}/jobs`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(jobData)
  });

  let text = await response.text();
  let data = text ? JSON.parse(text) : {};

  if (!response.ok) {
    throw new Error(parseErrorMessage(data, 'Failed to create job'));
  }

  return data;
}