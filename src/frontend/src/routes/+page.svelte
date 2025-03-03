<script>
    import { authUser } from '$lib/stores';
  </script>
  
  <main class="p-6 text-center">
    {#if $authUser}
      <h1 class="text-3xl font-bold">Welcome, {$authUser.fullName}!</h1>
      <p class="text-gray-600">You are logged in with phone number: {$authUser.phoneNumber}</p>
    {:else}
      <h1 class="text-3xl font-bold">Welcome to Our App</h1>
      <p class="text-gray-600">Please <a href="/login" class="text-blue-500">Login</a> or <a href="/signup" class="text-blue-500">Sign Up</a></p>
    {/if}
  </main>
  