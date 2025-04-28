<script>
  import { createEventDispatcher } from 'svelte';
  import { computePosition, flip, shift, offset, arrow } from '@floating-ui/dom';
  import { goto } from '$app/navigation';
  import { verifyCnic } from '$lib/api';

  const dispatch = createEventDispatcher();

  let selectedIdType = 'NADRA CNIC';
  let dropdownOpen = false;

  // File upload state
  let selectedFile = null;
  let fileInput;
  let previewUrl = "";
  let dropdownButton;
  let dropdownMenu;
  let arrowElement;
  let isUploading = false;
  let uploadError = null;

  const idTypes = [
    { value: 'NADRA CNIC', label: 'NADRA CNIC' },
    { value: "Driver's License", label: "Driver's License" }
  ];

  // Toggle dropdown (if additional options are needed later)
  function toggleDropdown() {
    dropdownOpen = !dropdownOpen;
    if (dropdownOpen) {
      updatePosition();
    }
  }

  async function updatePosition() {
    const { x, y, placement, middlewareData } = await computePosition(
      dropdownButton,
      dropdownMenu,
      {
        placement: 'bottom-start',
        middleware: [
          offset(4),
          flip(),
          shift(),
          arrow({ element: arrowElement })
        ]
      }
    );

    Object.assign(dropdownMenu.style, {
      left: `${x}px`,
      top: `${y}px`
    });

    if (arrowElement) {
      const { x: arrowX, y: arrowY } = middlewareData.arrow;
      Object.assign(arrowElement.style, {
        left: `${arrowX}px`,
        top: `${arrowY}px`
      });
    }
  }

  function selectIdType(type) {
    selectedIdType = type;
    dropdownOpen = false;
  }

  // Handle file selection and create a preview
  function handleFileUpload(event) {
    const file = event.target.files[0];
    if (file) {
      selectedFile = file;
      previewUrl = URL.createObjectURL(file);
      dispatch('uploadSelected', { idType: selectedIdType, file });
    }
  }

  async function handleUpload() {
    if (!selectedFile) {
      uploadError = "Please select an image first";
      return;
    }

    isUploading = true;
    uploadError = null;

    try {
      await verifyCnic(selectedFile);
      // If upload is successful, redirect to home page
      goto('/');
    } catch (error) {
      uploadError = "Failed to upload image. Please try again.";
      console.error('Upload error:', error);
    } finally {
      isUploading = false;
    }
  }
</script>

<!-- Main Layout -->
<div class="mx-auto p-2 max-w-5xl space-y-4">
  <!-- Top Section: Headline & Bullet Points -->
  <div class="w-full space-y-3">
    <h2 class="text-4xl font-bold text-gray-800 text-center">Verify Your Identity</h2>
    <ul class="text-center text-md text-gray-500">
      <li>Ensure your ID is fully visible with no obstructions.</li>
      <li>Use a neutral background for best results.</li>
    </ul>
  </div>

  <!-- Image Section: Show preview if available -->
  <div class="w-full flex items-center justify-center">
    <div class="max-w-2xs">
      {#if previewUrl}
        <img src={previewUrl} alt="Selected ID" class="w-full h-auto object-contain" />
      {:else}
        <img src="/images/verify_ID.svg" alt="ID Verification" class="w-full h-auto object-contain" />
      {/if}
    </div>
  </div>

  <!-- Upload Control: Reduced width container -->
  <div class="mx-auto w-full max-w-xs flex flex-col items-center gap-3">
    <!-- ID Type Dropdown -->
    <div class="w-full relative">
      <button
        bind:this={dropdownButton}
        on:click={toggleDropdown}
        class="w-full px-4 py-3 bg-white border border-gray-300 rounded-full shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-700 flex items-center justify-between"
      >
        <span>{selectedIdType}</span>
        <svg
          class="w-5 h-5 transition-transform duration-200 {dropdownOpen ? 'transform rotate-180' : ''}"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
      </button>

      {#if dropdownOpen}
        <div
          bind:this={dropdownMenu}
          class="absolute z-10 w-full mt-2 bg-white rounded-lg shadow-lg border border-gray-200 py-1"
        >
          <div
            bind:this={arrowElement}
            class="absolute w-3 h-3 bg-white transform rotate-45 border-l border-t border-gray-200"
          >
          </div>
          {#each idTypes as type}
            <button
              class="w-full px-4 py-2 text-left hover:bg-gray-50 focus:outline-none focus:bg-gray-50 {selectedIdType === type.value ? 'text-blue-600 font-medium' : 'text-gray-700'}"
              on:click={() => selectIdType(type.value)}
            >
              {type.label}
            </button>
          {/each}
        </div>
      {/if}
    </div>

    <div class="w-full">
      <button
        on:click={() => fileInput.click()}
        class="w-full px-4 py-3 bg-secondary text-white rounded-full hover:opacity-90 shadow-md focus:outline-none focus:ring-2 focus:ring-secondary focus:ring-offset-2"
      >
        Choose Image
      </button>
      <input
        type="file"
        accept="image/*"
        bind:this={fileInput}
        on:change={handleFileUpload}
        class="hidden"
      />

      {#if selectedFile}
        <button
          on:click={handleUpload}
          disabled={isUploading}
          class="w-full px-4 py-3 my-2 bg-secondary text-white rounded-full hover:opacity-90 shadow-md focus:outline-none focus:ring-2 focus:ring-secondary focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isUploading ? 'Uploading...' : 'Upload Image'}
        </button>
      {/if}

      {#if uploadError}
        <p class="text-sm text-red-500 text-center">{uploadError}</p>
      {/if}
    </div>

    <p class="text-sm text-gray-500 text-center italic">
      * If you are signing up as a driver, please upload your driver's license
    </p>
  </div>
</div>
