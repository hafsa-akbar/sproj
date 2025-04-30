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
    credentials: 'include',
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

export async function getWorkerDetails() {
  const response = await fetch(`${BASE_URL}/worker-details`, {
    headers: { 'Content-Type': 'application/json' },
    credentials: "include"
  });
  if (!response.ok) {
    throw new Error('Failed to fetch worker details');
  }
  return response.json();
}

export async function closeJob(jobId) {
  const response = await fetch(`${BASE_URL}/jobs/${jobId}`, {
    method: 'DELETE',
    credentials: "include"
  });
  if (!response.ok) {
    throw new Error('Failed to close job');
  }
}

export async function createPastJob(jobData) {
  const response = await fetch(`${BASE_URL}/past-jobs`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: "include",
    body: JSON.stringify(jobData)
  });
  if (!response.ok) {
    throw new Error('Failed to create past job');
  }
  return response.json();
}

export async function getPendingReviews() {
  const response = await fetch(`${BASE_URL}/pending-reviews`, {
    headers: { 'Content-Type': 'application/json' },
    credentials: "include"
  });
  if (!response.ok) {
    throw new Error('Failed to fetch pending reviews');
  }
  return response.json();
}

export async function verifyReview(pastJobId, rating, comments) {
  const response = await fetch(`${BASE_URL}/verify-review`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: "include",
    body: JSON.stringify({ pastJobId, rating, comments })
  });
  if (!response.ok) {
    throw new Error('Failed to verify review');
  }
  return;
}

export async function addCouple(phoneNumber) {
  const response = await fetch(`${BASE_URL}/profile/add-couple`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(phoneNumber)
  });

  if (!response.ok) {
    throw new Error('Failed to add couple');
  }

  return response.json();
}