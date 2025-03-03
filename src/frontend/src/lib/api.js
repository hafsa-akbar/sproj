const BASE_URL = 'http://localhost:5000/users';

export async function login(user) {
  const response = await fetch(`${BASE_URL}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(user),
  });

  const data = await response.json();
  if (!response.ok) {
    throw new Error(data.message || 'Login failed');
  }

  return data;
}

export async function register(user) {
  const response = await fetch(`${BASE_URL}/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(user),
  });

  const data = await response.json();
  if (!response.ok) {
    throw new Error(data.message || 'Signup failed');
  }

  return data;
}
