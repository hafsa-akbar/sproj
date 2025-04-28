<script>
  import { authUser, logoutUser } from '$lib/stores';
  import { goto } from '$app/navigation';
  import { toasts } from 'svelte-toasts';
  import AddJobModal from '$lib/components/jobs/AddJobModal.svelte';

  let showAddJob = false;

  function handleLogout() {
    logoutUser();
    goto('/');
  }

  function handleAddJob() {
    if ($authUser?.role === 3) {
      showAddJob = true;
    } else {
      toasts.add({ title: 'Error', description: 'Cannot post job', type: 'error' });
    }
  }
</script>

<nav class="flex flex-wrap md:flex-nowrap items-center justify-between p-4 bg-white shadow-md">
  <!-- logo -->
  <a href="/" class="flex-shrink-0 mb-2 md:mb-0">
    <img src="/images/nav_logo.svg" alt="Logo" class="h-8 md:h-10" />
  </a>

  <div class="flex-1"></div>

  <!-- actions -->
  <div class="flex space-x-2 md:space-x-4">
    {#if $authUser}
      <!-- Add Job button -->
      <button
        on:click={handleAddJob}
        class="px-4 md:px-6 py-1 md:py-1.5 border border-secondary text-secondary text-sm md:text-base rounded-full transition hover:bg-gray-100"
      >
        Add a Job
      </button>

      <!-- Logout button -->
      <button
        on:click={handleLogout}
        class="px-3 md:px-4 py-1 md:py-1.5 text-gray-500 text-sm md:text-base rounded transition hover:text-secondary"
      >
        Logout
      </button>
    {:else}
      <a
        href="/login"
        class="px-3 md:px-4 py-1 md:py-1.5 text-gray-700 text-sm md:text-base rounded transition hover:text-gray-900"
      >
        Login
      </a>
    {/if}
  </div>
</nav>

{#if showAddJob}
  <AddJobModal on:close={() => (showAddJob = false)} />
{/if}
