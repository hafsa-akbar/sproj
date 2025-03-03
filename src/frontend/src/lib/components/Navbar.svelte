<script>
    import { authUser, logoutUser } from '$lib/stores';
  </script>
  
  <nav class="bg-blue-500 text-white p-4 flex justify-between">
    <h1 class="text-xl font-bold">Daam pe Kaam</h1>
  
    {#if $authUser}
      <button on:click={logoutUser} class="hover:underline">Logout</button>
    {:else}
      <a href="/login" class="hover:underline">Login</a>
    {/if}
  </nav>
  