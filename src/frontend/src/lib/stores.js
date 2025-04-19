import { writable } from 'svelte/store';
import { browser } from '$app/environment';

const initial = browser && localStorage.getItem('authUser')
  ? JSON.parse(localStorage.getItem('authUser'))
  : null;

const initialSession = browser && localStorage.getItem('sessionId')
  ? localStorage.getItem('sessionId')
  : null;

export const authUser = writable(initial);
export const sessionId = writable(initialSession);

export function loginUser(user, sid) {
  authUser.set(user);
  if (sid) {
    sessionId.set(sid);
    if (browser) localStorage.setItem('sessionId', sid);
  }
  if (browser) localStorage.setItem('authUser', JSON.stringify(user));
}

export function logoutUser() {
  authUser.set(null);
  sessionId.set(null);
  if (browser) {
    localStorage.removeItem('authUser');
    localStorage.removeItem('sessionId');
  }
}
