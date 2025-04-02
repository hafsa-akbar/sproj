<script>
  import { writable } from 'svelte/store';
  import { login } from '$lib/api';
  import { loginUser } from '$lib/stores';
  import { goto } from '$app/navigation';

  let phoneNumber = writable('');
  let password = writable('');
  let errorMessage = writable('');

  async function handleLogin() {
    errorMessage.set('');

    const user = { phoneNumber: $phoneNumber, password: $password };

    try {
      const response = await login(user);
      loginUser(response);
      goto('/');
    } catch (error) {
      errorMessage.set(error.message);
    }
  }
</script>

<div class="max-w-md mx-auto p-6 bg-white rounded-lg shadow-md">
  <h2 class="text-2xl font-bold mb-4">Login</h2>

  <form on:submit|preventDefault={handleLogin} class="space-y-4">
    <input type="text" bind:value={$phoneNumber} placeholder="Phone Number"
      class="w-full p-2 border border-gray-600 rounded-md" required />

    <input type="password" bind:value={$password} placeholder="Password"
      class="w-full p-2 border border-gray-600 rounded-md" required />

    <button type="submit" class="w-full bg-secondary text-white p-2 rounded-md hover:opacity-90">
      Login
    </button>

    {#if $errorMessage}
      <p class="text-red-500">{$errorMessage}</p>
    {/if}
  </form>

  <p class="mt-4 text-sm">
    Don't have an account? <a href="/signup" class="text-secondary">Sign up</a>
  </p>
</div>
