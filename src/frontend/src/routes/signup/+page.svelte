<script>
  import SignupForm from '$lib/components/auth/SignupForm.svelte';
  import SmsVerification from '$lib/components/auth/SmsVerification.svelte';
  import AccountTypeSelection from '$lib/components/auth/AccountTypeSelection.svelte';
  import IdVerification from '$lib/components/auth/IdVerification.svelte';
  import ProgressBar from '$lib/components/auth/ProgressBar.svelte';
  import { loginUser } from '$lib/stores';
  import { writable } from 'svelte/store';
  import { fly } from 'svelte/transition';
  import { goto } from '$app/navigation';

  let currentStep = writable(1);
  let userData    = writable({});
  let accountType = writable(null); // 'hiring' or 'working'

  function goBack() {
    if ($currentStep > 1) currentStep.set($currentStep - 1);
  }

  function handleBasicInfoComplete(detail) {
    userData.set(detail);
    currentStep.set(2);
  }

  function handleAccountTypeSelect(event) {
    accountType.set(event.detail);
    userData.update(u => ({ ...u, accountType: event.detail }));
    currentStep.set(3);
  }
</script>

<main class="flex flex-col items-center justify-start min-h-screen split-bg p-12">
  <ProgressBar currentStep={$currentStep} />

  <div class="relative w-full max-w-md space-y-4">
    {#if $currentStep > 1 && $currentStep < 3}
      <button
        on:click={goBack}
        class="text-sm text-secondary underline mb-2 transition-transform transform hover:-translate-x-1"
      >
        ← Back
      </button>
    {/if}

    {#if $currentStep === 1}
      <div in:fly={{ x: -100, duration: 300, delay: 200 }} out:fly={{ x: -200, duration: 300 }}>
        <SignupForm
          initialData={$userData}
          on:complete={({ detail }) => handleBasicInfoComplete(detail)}
        />
      </div>

    {:else if $currentStep === 2}
      <div in:fly={{ x: 100, duration: 300, delay: 200 }} out:fly={{ x: 0, duration: 300 }}>
        <AccountTypeSelection on:select={handleAccountTypeSelect} />
      </div>

    {:else if $currentStep === 3}
      <div in:fly={{ x: 100, duration: 300, delay: 200 }} out:fly={{ x: 0, duration: 300 }}>
        <SmsVerification
          userData={$userData}
          on:complete={() => {
            if ($userData.accountType === 'hiring') {
              goto('/');
            } else {
              currentStep.set(4);
            }
          }}
        />
      </div>

    {:else if $currentStep === 4}
      <div in:fly={{ x: 100, duration: 300, delay: 200 }} out:fly={{ x: 0, duration: 300 }}>
        <IdVerification />
      </div>
    {/if}
  </div>
</main>
