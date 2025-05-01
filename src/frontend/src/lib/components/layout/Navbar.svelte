<script>
  import { onMount } from 'svelte';
  import { authUser, logoutUser, jobsStore } from '$lib/stores';
  import { goto } from '$app/navigation';
  import { toasts } from 'svelte-toasts';
  import AddJobModal from '$lib/components/jobs/AddJobModal.svelte';
  import * as Select from '$lib/components/ui/select/index.js';

  let showAddJob = false;
  let showProfileDropdown = false;
  $: user = $authUser;
  $: showAddJobButton = user?.role === 3;

  function handleLogout() {
    logoutUser();
    goto('/');
  }

  function handleOutsideClick(event) {
    if (!event.target.closest('.profile-dropdown')) {
      showProfileDropdown = false;
    }
  }

  onMount(() => {
    document.addEventListener('click', handleOutsideClick);
    return () => document.removeEventListener('click', handleOutsideClick);
  });
</script>

<nav class="flex flex-wrap md:flex-nowrap items-center justify-between p-4 bg-white shadow-md">
  <!-- logo -->
  <a href="/" class="flex-shrink-0 mb-2 md:mb-0">
    <img src="/images/nav_logo.svg" alt="Logo" class="h-8 md:h-10" />
  </a>

  <div class="flex-1"></div>

  <!-- actions -->
  <div class="flex space-x-2 md:space-x-4 items-center">
    {#if user}
      <!-- Add Job button -->
      {#if showAddJobButton}
      <button
        on:click={() => { showAddJob = true }}
        class="px-4 md:px-6 py-1 md:py-1.5 border border-secondary text-secondary text-sm md:text-base rounded-full transition hover:bg-gray-100"
      >
        Add a Job
        </button>
      {/if}

      <!-- Profile Dropdown -->
      <div class="relative profile-dropdown">
        <button
          class="rounded-full focus:outline-none focus:ring-2 focus:ring-secondary"
          on:click={() => showProfileDropdown = !showProfileDropdown}
        >
          <img src="/images/profile.png" alt="Profile" class="h-9 w-9 md:h-10 md:w-10 rounded-full border border-gray-200" />
        </button>
        {#if showProfileDropdown}
          <div class="absolute right-0 mt-2 w-40 bg-white border border-gray-200 rounded-2xl shadow-2xl z-50 overflow-hidden">
            <ul class="py-1">
              <li>
                <button
                  class="w-full text-left px-4 py-2 text-gray-700 hover:bg-[#92B285] transition text-poppins"
                  on:click={() => { goto('/profile'); showProfileDropdown = false; }}
                >
                  View Profile
                </button>
              </li>
              <li>
                <button
                  class="w-full text-left px-4 py-2 text-gray-700 hover:bg-[#92B285] transition text-poppins"
                  on:click={() => { handleLogout(); showProfileDropdown = false; }}
                >
                  Logout
                </button>
              </li>
            </ul>
          </div>
        {/if}
      </div>
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
  <AddJobModal
    on:submit={e => {
      const newJob = e.detail;
      newJob.workers = newJob.jobGender === 3
        ? [
            { fullName: $authUser.fullName, gender: $authUser.gender },
            { fullName: $authUser.coupleName, gender: $authUser.gender === 1 ? 2 : 1 }
          ]
        : [{ fullName: $authUser.fullName, gender: $authUser.gender }];
      jobsStore.update(js => [...js, newJob]);
      showAddJob = false;
    }}
    on:close={() => showAddJob = false}
  />
{/if}

<style></style>
