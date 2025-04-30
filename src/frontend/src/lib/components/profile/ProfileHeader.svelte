<script>
  import { authUser } from '$lib/stores';
  import { goto } from '$app/navigation';
  import CnicVerificationModal from './CnicVerificationModal.svelte';
  import { addCouple } from '$lib/api'; 
  import { toasts } from 'svelte-toasts';

  // custom action to detect clicks outside an element
  function clickOutside(node) {
    const handleClick = (e) => {
      if (!node.contains(e.target)) {
        showCoupleInput = false;
      }
    };
    document.addEventListener('click', handleClick, true);
    return {
      destroy() {
        document.removeEventListener('click', handleClick, true);
      }
    };
  }

  export let rating = 0;
  $: user = $authUser;

  $: avatarUrl = user?.fullName
    ? `https://avatar.iran.liara.run/public/${user.gender === 1 ? 'boy' : 'girl'}?username=${encodeURIComponent(user.fullName.replace(/\s+/g, ''))}`
    : null;

  let showCnicModal = false;
  let showCoupleInput = false;
  let spousePhone = '';
  let isAdding = false;

  function handleUpgrade() {
    showCnicModal = true;
  }

  function handleVerificationSuccess() {
    authUser.update(u => ({ ...u, role: 3 }));
    goto('/');
  }

  async function submitCouple() {
    if (isAdding) return;
    isAdding = true;
    try {
      const res = await addCouple({ phoneNumber: spousePhone });
      // update store with new couple name
      authUser.update(u => ({ ...u, coupleName: res.coupleName }));
      toasts.add({ description: 'Couple added successfully!', type: 'success' });
      showCoupleInput = false;
      spousePhone = '';
    } catch (err) {
      console.error(err);
      toasts.add({ description: err.message || 'Failed to add couple. Please try again.', type: 'error' });
    } finally {
      isAdding = false;
    }
  }
</script>

<div class="flex-shrink-0 w-full md:w-1/4 flex flex-col items-center space-y-4">
  {#if avatarUrl}
    <img
      src={avatarUrl}
      alt="Profile"
      class="h-32 w-32 rounded-full border border-gray-200"
    />
  {:else}
    <div class="h-32 w-32 rounded-full border border-gray-200 bg-gray-200 flex items-center justify-center">
      <span class="text-gray-400">Loading...</span>
    </div>
  {/if}

  <div class="text-center">
    <h2 class="font-semibold text-xl">{user?.fullName || 'Loading...'}</h2>
    {#if user?.role === 1}
      <div class="flex justify-center mt-1 space-x-1">
        {#each Array(5) as _, i}
          <img
            src={i < rating ? '/images/filled_star.svg' : '/images/empty_star.svg'}
            alt="Star"
            class="w-5 h-5"
          />
        {/each}
      </div>
    {/if}
    <div class="text-gray-600 mt-1">
      Based in <span class="font-semibold">{user?.address?.split(',')[1]?.trim() || '...'}</span>
    </div>
    <div class="text-gray-600">{user?.phoneNumber || '...'}</div>
  </div>

  <div class="border-t border-gray-200 w-full mt-4"></div>

  {#if user?.role === 3}
    <div class="w-full flex justify-center">
      {#if !user?.coupleName && !showCoupleInput}
        <button
          class="px-4 py-2 bg-yellow-100 text-yellow-800 rounded-md"
          on:click={() => showCoupleInput = true}
        >
          Add Couple User
        </button>
      {:else if showCoupleInput}
        <div use:clickOutside class="w-full px-4">
          <div class="flex flex-col sm:flex-row w-full space-y-2 sm:space-y-0 sm:space-x-2">
            <input
              type="tel"
              bind:value={spousePhone}
              placeholder="Spouse's phone"
              class="w-full sm:flex-1 p-2 border rounded-md focus:border-secondary focus:ring-secondary"
            />
            <button
              class="w-12 h-12 flex items-center justify-center bg-secondary text-white rounded-md disabled:opacity-50 hover:bg-secondary-dark"
              on:click={submitCouple}
              disabled={isAdding || !spousePhone}
              aria-label="Submit couple"
            >
              {#if isAdding}
                <span class="animate-spin">⏳</span>
              {:else}
                <svg xmlns="http://www.w3.org/2000/svg" class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              {/if}
            </button>
          </div>
        </div>
      {:else}
        <div class="text-gray-700">
          Couple with <span class="font-semibold">{user.coupleName}</span>
        </div>
      {/if}
    </div>
  {:else}
    <button
      class="px-4 py-2 bg-green-100 text-green-800 rounded-md"
      on:click={handleUpgrade}
    >
      Upgrade to Worker
    </button>
  {/if}
</div>

{#if showCnicModal}
  <CnicVerificationModal
    on:close={() => showCnicModal = false}
    on:success={handleVerificationSuccess}
  />
{/if}
