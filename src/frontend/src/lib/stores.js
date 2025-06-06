import { writable } from 'svelte/store';
import { browser } from '$app/environment';

const getInitialUser = () => {
  if (!browser) return null;
  const stored = sessionStorage.getItem('authUser');
  return stored ? JSON.parse(stored) : null;
};

export const authUser = writable(getInitialUser());

authUser.subscribe((user) => {
  if (!browser) return;
  if (user) {
    sessionStorage.setItem('authUser', JSON.stringify(user));
  } else {
    sessionStorage.removeItem('authUser');
  }
});

export function loginUser(user) {
  authUser.set(user);
}

export function logoutUser() {
  authUser.set(null);
}

export const jobsStore = writable([]);