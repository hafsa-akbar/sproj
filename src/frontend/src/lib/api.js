const BASE_URL = '/api';

function parseErrorMessage(data, defaultMessage) {
  console.log(data);
  let errorMessage = data.message || defaultMessage;
  if (data.errors && Array.isArray(data.errors)) {
    errorMessage += ': ' + data.errors[0]['reason'];
  }
  return errorMessage;
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

  return { user: data };
}

export async function startSmsVerification() {
  const response = await fetch(`${BASE_URL}/verify/start-sms`, {
    method: 'POST',
    credentials: "include"
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
    headers: { 'Content-Type': 'application/json' },
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

  const response = await fetch(`${BASE_URL}/verify/cnic`, {
    method: 'POST',
    headers: { 'Content-Type': 'multipart/form-data; boundary=WebAppBoundary' },
    body: formData
  });

  if (!response.ok) {
    throw new Error('Upload failed');
  }

  return response;
}

export async function getJobs() {
  const res = await fetch(`${BASE_URL}/jobs`, {
    headers: { 'Content-Type': 'application/json' },
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
    headers: { 'Content-Type': 'application/json' },
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
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(jobData)
  });

  let text = await response.text();
  let data = text ? JSON.parse(text) : {};

  if (!response.ok) {
    throw new Error(parseErrorMessage(data, 'Failed to create job'));
  }

  return data;
}

export async function getJobDetails(jobId) {
  const response = await fetch(`${BASE_URL}/jobs/${jobId}`);
  if (!response.ok) {
    throw new Error('Failed to fetch job details');
  }
  return response.json();
}