<script>
  import { createEventDispatcher } from 'svelte';
  import { verifyCnic } from '$lib/api';
  import { Button } from '../ui/button/index.js';

  const dispatch = createEventDispatcher();
  let selectedFile = null;
  let fileInput;
  let previewUrl = '';
  let isUploading = false;
  let uploadError = null;

  function handleFileUpload(event) {
    const file = event.target.files[0];
    if (file) {
      selectedFile = file;
      previewUrl = URL.createObjectURL(file);
    }
  }

  async function handleUpload() {
    if (!selectedFile) {
      uploadError = 'Please select an image first';
      return;
    }
    isUploading = true;
    uploadError = null;
    try {
      await verifyCnic(selectedFile);
      dispatch('success');
    } catch (error) {
      uploadError = 'Failed to upload image. Please try again.';
      console.error('Upload error:', error);
    } finally {
      isUploading = false;
    }
  }
</script>

<div class="fixed inset-0 bg-black/10 z-50"></div>
<div class="fixed inset-0 flex items-center justify-center p-4 z-50">
  <div
    in:fly={{ y: 20, duration: 200 }}
    class="relative bg-white rounded-2xl w-[90%] sm:w-110 p-6 shadow-2xl"
  >
    <button class="absolute top-3 right-3 text-gray-400 hover:text-secondary transition-colors" on:click={() => dispatch('close')} aria-label="Close">
      ×
    </button>

    <h2 class="text-xl font-semibold text-heading-gray mb-4 text-center">Verify Your Identity</h2>
    <p class="text-center text-gray-500 mb-4 text-sm">
      Ensure your ID is fully visible with no obstructions. Use a neutral background for best results.
    </p>

    <!-- Larger Preview Box -->
    <div class="flex items-center justify-center mb-6">
      <div class="w-90 h-40 bg-gray-100 rounded-lg overflow-hidden flex items-center justify-center">
        {#if previewUrl}
          <img src={previewUrl} alt="Selected ID" class="w-full h-full object-cover" />
        {:else}
          <img src="/images/verify_ID.svg" alt="ID Verification" class="w-20 h-20 opacity-40" />
        {/if}
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="space-y-4">
      <div class="flex gap-3">
        <Button
          variant="outline"
          on:click={() => fileInput.click()}
          class="flex-1 text-sm py-2"
        >
          Choose Image
        </Button>

        {#if selectedFile}
          <Button
            variant="secondary"
            on:click={handleUpload}
            disabled={isUploading}
            class="flex-1 text-sm py-2"
          >
            {#if isUploading}Uploading...{:else}Upload{/if}
          </Button>
        {/if}
      </div>

      <input
        type="file"
        accept="image/*"
        bind:this={fileInput}
        on:change={handleFileUpload}
        class="hidden"
      />

      {#if uploadError}
        <p class="text-sm text-red-500 text-center">{uploadError}</p>
      {/if}
    </div>

    <p class="mt-6 text-xs text-gray-400 text-center italic">
      * If you are signing up as a driver, please upload your driver's license.
    </p>
  </div>
</div>
