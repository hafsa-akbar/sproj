<script>
    import { createEventDispatcher } from 'svelte';
    import { authUser } from '$lib/stores';
    import { toasts } from 'svelte-toasts';
    import { createJob } from '$lib/api';
    import { 
      jobCategoryOptions, 
      experienceLevelOptions, 
      jobTypeOptions,
      getGenderLabel,
      GenderType 
    } from '$lib/config/jobConfig';

    const dispatch = createEventDispatcher();

    const initialFormState = {
      wageRate: '',
      jobCategory: '',
      jobExperience: '',
      jobType: '',
      locale: '',
      postAsCouple: false
    };

    let formData = { ...initialFormState };
    let isSubmitting = false;

    $: user = $authUser;
    $: defaultGender = user?.gender?.toString() || GenderType.MALE.toString();

    async function handleSubmit() {
      if (isSubmitting) return;
      isSubmitting = true;

      try {
        const jobData = {
          wageRate: parseInt(formData.wageRate),
          jobCategory: parseInt(formData.jobCategory),
          jobExperience: parseInt(formData.jobExperience),
          jobGender: formData.postAsCouple ? GenderType.COUPLE : parseInt(defaultGender),
          jobType: parseInt(formData.jobType),
          locale: formData.locale
        };

        await createJob(jobData);
        toasts.add({ title: 'Success', description: 'Job posted successfully!', type: 'success', duration: 3000 });
        dispatch('close');
      } catch (error) {
        toasts.add({ title: 'Error', description: error.message || 'Failed to post job', type: 'error', duration: 3000 });
      } finally {
        isSubmitting = false;
      }
    }

    function handleClose() {
      formData = { ...initialFormState };
      dispatch('close');
    }
</script>

<div class="modal-container">
  <div class="modal-content mx-4 sm:mx-auto max-h-[90vh] overflow-y-auto my-8">
    <button
      class="absolute top-4 left-4 text-gray-500 hover:text-gray-700"
      aria-label="Close form"
      on:click={handleClose}
    >
      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>

    <div class="form-header mt-8">
      <h2 class="text-2xl font-semibold mb-2">Add a Job</h2>
      <p class="text-gray-500 text-sm mb-6">Fill in the details below to create a new job posting</p>
    </div>

    <form on:submit|preventDefault={handleSubmit} class="space-y-6">
      <div class="form-group">
        <label for="wageRate">Wage Rate</label>
        <input 
          id="wageRate" 
          type="number" 
          bind:value={formData.wageRate} 
          placeholder="Enter wage rate" 
          class="placeholder-gray-400"
          required 
        />
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div class="form-group">
          <label for="jobCategory">Category</label>
          <select 
            id="jobCategory" 
            bind:value={formData.jobCategory} 
            class="placeholder-gray-400"
            required
          >
            <option value="" disabled class="text-gray-400">Select category</option>
            {#each jobCategoryOptions as opt}
              <option value={opt.value}>{opt.label}</option>
            {/each}
          </select>
        </div>

        <div class="form-group">
          <label for="jobExperience">Experience</label>
          <select 
            id="jobExperience" 
            bind:value={formData.jobExperience} 
            class="placeholder-gray-400"
            required
          >
            <option value="" disabled class="text-gray-400">Select experience</option>
            {#each experienceLevelOptions as opt}
              <option value={opt.value}>{opt.label}</option>
            {/each}
          </select>
        </div>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div class="form-group">
          <label for="jobType">Job Type</label>
          <select 
            id="jobType" 
            bind:value={formData.jobType} 
            class="placeholder-gray-400"
            required
          >
            <option value="" disabled class="text-gray-400">Select type</option>
            {#each jobTypeOptions as opt}
              <option value={opt.value}>{opt.label}</option>
            {/each}
          </select>
        </div>

        <div class="form-group">
          <label for="locale">Locale</label>
          <input 
            id="locale" 
            type="text" 
            bind:value={formData.locale} 
            placeholder="Enter location" 
            class="placeholder-gray-400"
            required 
          />
        </div>
      </div>

      <div class="form-group">
        <div class="flex flex-col sm:flex-row sm:items-center gap-4">
          <div class="flex-1">
            <label for="gender" class="block text-sm font-medium text-gray-700 mb-1">Gender</label>
            <input
              id="gender"
              type="text"
              value={getGenderLabel(parseInt(defaultGender))}
              disabled
              class="w-full sm:w-24 px-3 py-2 border border-gray-300 bg-gray-50 text-gray-600 rounded"
            />
          </div>

          {#if user?.coupleUserId}
            <div class="flex items-center justify-end gap-3 flex-1">
              <span class="text-sm text-gray-600 whitespace-nowrap">Post as Couple</span>
              <label class="relative inline-block w-[52px] h-[28px] flex-shrink-0">
                <input 
                  type="checkbox" 
                  bind:checked={formData.postAsCouple} 
                  class="sr-only peer" 
                />
                <div class="absolute inset-0 rounded-full bg-gray-200 cursor-pointer
                           peer-checked:bg-secondary transition-all duration-300 ease-in-out">
                </div>
                <div class="absolute left-[2px] top-[2px] w-6 h-6 bg-white rounded-full shadow
                           transition-all duration-300 ease-in-out peer-checked:translate-x-6">
                </div>
              </label>
            </div>
          {/if}
        </div>
      </div>

      <div class="flex justify-end space-x-3">
        <button 
          type="button" 
          class="px-4 py-2 text-gray-500 hover:text-gray-800 transition" 
          on:click={handleClose} 
          disabled={isSubmitting}
        >
          Cancel
        </button>
        <button 
          type="submit" 
          class="px-4 py-2 bg-secondary text-white rounded hover:brightness-110 transition" 
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Posting...' : 'Post Job'}
        </button>
      </div>
    </form>
  </div>
</div>

<style>
  .modal-container {
    position: fixed;
    inset: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 50;
    background-color: rgba(0, 0, 0, 0.5);
    padding: 1rem;
  }

  .modal-content {
    position: relative;
    background: white;
    border-radius: 0.75rem;
    width: 100%;
    max-width: 32rem;
    padding: 2rem;
    box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1),
                0 10px 10px -5px rgba(0,0,0,0.04);
    border: 1px solid #e5e7eb;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .form-group label,
  .form-group span {
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
  }

  input[type="text"],
  input[type="number"],
  select {
    width: 100%;
    padding: 0.5rem 0.75rem;
    border: 1px solid #e5e7eb;
    border-radius: 0.375rem;
    background-color: white;
    font-size: 0.875rem;
    transition: all 0.2s;
  }

  input:focus, select:focus {
    outline: none;
    border-color: var(--color-secondary);
    box-shadow: 0 0 0 2px rgba(var(--color-secondary-rgb), 0.1);
  }

  @media (max-width: 640px) {
    .modal-content {
      padding: 1.5rem;
      margin: 2rem 0.5rem;
    }
  }
</style>