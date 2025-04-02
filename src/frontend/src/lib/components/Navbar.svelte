<script>
  import { authUser, logoutUser } from '$lib/stores';
  import { goto } from '$app/navigation';

  function handleLogout() {
    logoutUser();
    goto('/');
  }
</script>

<nav class="p-3 px-10 flex justify-between items-center sticky top-0 z-50 shadow-md">
  <div class="flex items-center">
    <a href="/">
      <img src="/images/nav_logo.svg" alt="Logo" class="h-10 w-auto mr-3" />
    </a>
  </div>

  {#if $authUser}
    <button on:click={handleLogout} class="hover:underline focus:outline-none">Logout</button>
  {:else}
    <a href="/login" class="hover:underline focus:outline-none">Login</a>
  {/if}
</nav>
