<script>
    import SignupForm from '$lib/components/auth/SignupForm.svelte';
    import SmsVerification from '$lib/components/auth/SmsVerification.svelte';
    import AccountTypeSelection from '$lib/components/auth/AccountTypeSelection.svelte';
    import IdVerification from '$lib/components/auth/IdVerification.svelte';
    import ProgressBar from '$lib/components/auth/ProgressBar.svelte';
    import { writable } from 'svelte/store';
    import { fly } from 'svelte/transition';
    import { loginUser } from '$lib/stores';
    import { goto } from '$app/navigation';

    let currentStep = writable(1);
    let userData = writable(null);

    function handleBasicInfoComplete(data) {
      userData.set(data);
      currentStep.set(2);
    }

    function handleVerificationComplete() {
      currentStep.set(3);
    }

    function handleAccountTypeSelect(event) {
      $userData = { ...$userData, accountType: event.detail };
      currentStep.set(4);
    }

    function handleIdVerificationSubmit(event) {
      $userData = { ...$userData, idVerification: event.detail };
      loginUser($userData);
      goto('/');
    }
</script>

<main class="flex flex-col items-center justify-start min-h-screen bg-gray-50 p-12 split-bg">
  <ProgressBar currentStep={$currentStep} />

  <div class="relative w-full max-w-md">
    {#if $currentStep === 1}
      <div
        in:fly={{ x: -100, duration: 300, delay: 300 }}
        out:fly={{ x: -200, duration: 300 }}
        class="w-full"
      >
        <SignupForm on:complete={({ detail }) => handleBasicInfoComplete(detail)} />
      </div>
    {:else if $currentStep === 2}
      <div
        in:fly={{ x: 100, duration: 300, delay: 300 }}
        out:fly={{ x: 0, duration: 300 }}
        class="w-full"
      >
        <SmsVerification
          phoneNumber={$userData?.phoneNumber}
          onVerificationComplete={handleVerificationComplete}
        />
      </div>
    {:else if $currentStep === 3}
      <div
        in:fly={{ x: 100, duration: 300, delay: 300 }}
        out:fly={{ x: 0, duration: 300 }}
        class="w-full"
      >
        <AccountTypeSelection on:select={handleAccountTypeSelect} />
      </div>
    {:else}
      <div
        in:fly={{ x: 100, duration: 300, delay: 300 }}
        out:fly={{ x: 0, duration: 300 }}
        class="w-full"
      >
        <IdVerification on:submit={handleIdVerificationSubmit} />
      </div>
    {/if}
  </div>
</main>