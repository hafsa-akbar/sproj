<script>
  import { writable } from 'svelte/store';
  import { toasts } from 'svelte-toasts';
  import { fly } from 'svelte/transition';
  import { startSmsVerification, verifySmsCode } from '$lib/api';

  export let onVerificationComplete;
  export let phoneNumber;

  let verificationCode = writable('');
  let isLoading = writable(false);
  let error = '';

  async function handleStartVerification() {
    try {
      isLoading.set(true);
      await startSmsVerification();
      
      toasts.add({
        title: 'Success',
        description: 'Verification code sent to your phone',
        duration: 3000,
        type: 'success',
        theme: 'dark'
      });
    } catch (err) {
      error = err.message;
      toasts.add({
        title: 'Error',
        description: 'Failed to send verification code',
        duration: 3000,
        type: 'error',
        theme: 'dark'
      });
    } finally {
      isLoading.set(false);
    }
  }

  async function handleVerifyCode() {
    if (!$verificationCode || $verificationCode.length !== 6) {
      error = 'Please enter a valid 6-digit code';
      return;
    }

    try {
      isLoading.set(true);
      await verifySmsCode($verificationCode);
      onVerificationComplete();
    } catch (err) {
      error = err.message;
      toasts.add({
        title: 'Error',
        description: 'Invalid verification code',
        duration: 3000,
        type: 'error',
        theme: 'dark'
      });
    } finally {
      isLoading.set(false);
    }
  }

  handleStartVerification();
</script>

<div class="w-full max-w-md mx-auto text-center mb-4">
  <h2 class="text-2xl font-bold">Verify your account</h2>
  <p class="text-gray-600 mb-8 text-center my-1">
    We've sent a verification code to {phoneNumber}
  </p>
</div>

<div
  in:fly={{ y: 20, duration: 300 }}
  class="w-full p-8 bg-white rounded-lg shadow-sm"
>

  <div class="space-y-6">
    <div>
      <input
        type="text"
        bind:value={$verificationCode}
        placeholder="Enter 6-digit code"
        maxlength="6"
        class="w-full p-3 border-2 border-gray-600 rounded-lg text-center text-xl focus:border-secondary focus:ring-1 focus:ring-secondary outline-none transition-colors { $verificationCode.length > 0 ? 'tracking-[0.5em]' : 'tracking-normal' }"
        required
      />
    </div>

    {#if error}
      <p class="text-red-500 text-sm">{error}</p>
    {/if}

    <button
      on:click={handleVerifyCode}
      disabled={$isLoading}
      class="w-full bg-secondary text-white p-3 rounded-lg hover:opacity-90 transition-colors disabled:opacity-50 font-medium"
    >
      {#if $isLoading}
        Verifying...
      {:else}
        Verify Code
      {/if}
    </button>

    <button
      on:click={handleStartVerification}
      disabled={$isLoading}
      class="w-full text-secondary p-2 hover:opacity-90 transition-colors font-medium"
    >
      Resend Code
    </button>
  </div>
</div> 