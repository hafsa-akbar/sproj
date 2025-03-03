import { writable } from 'svelte/store';

export const authUser = writable(null);

export function loginUser(user) {
  authUser.set(user);
  localStorage.setItem('authUser', JSON.stringify(user));
}

export function logoutUser() {
  authUser.set(null);
  localStorage.removeItem('authUser');
}

if (typeof window !== 'undefined') {
  const storedUser = localStorage.getItem('authUser');
  if (storedUser) {
    authUser.set(JSON.parse(storedUser));
  }
}
