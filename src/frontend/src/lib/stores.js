import { writable } from 'svelte/store';
import { browser } from '$app/environment';

const initial = browser && localStorage.getItem('authUser')
  ? JSON.parse(localStorage.getItem('authUser'))
  : null;

export const authUser = writable(initial);

export function loginUser(user) {
  authUser.set(user);
  if (browser) localStorage.setItem('authUser', JSON.stringify(user));
}

export function logoutUser() {
  authUser.set(null);
  if (browser) localStorage.removeItem('authUser');
}
