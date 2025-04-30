<script>
  import { createEventDispatcher } from 'svelte';
  import { authUser } from '$lib/stores';
  import { toasts } from 'svelte-toasts';
  import { createPastJob } from '$lib/api';
  import { jobCategoryOptions, jobTypeOptions } from '$lib/config/jobConfig';
  import * as Select from '../ui/select/index.js';
  import { Input } from '../ui/input/index.js';
  import { Textarea } from '../ui/textarea/index.js';
  import { Label } from '../ui/label/index.js';
  import { Button } from '../ui/button/index.js';

  const dispatch = createEventDispatcher();

  const initialFormState = {
    jobCategory: '',
    jobType: '',
    locale: '',
    description: '',
    employerPhoneNumber: '',
    postAsCouple: false
  };

  let formData = { ...initialFormState };
  let isSubmitting = false;

  $: user = $authUser;

  // derive selected option objects
  $: selectedCategory = formData.jobCategory
    ? {
        value: formData.jobCategory,
        label: jobCategoryOptions.find(o => String(o.value) === String(formData.jobCategory))?.label
      }
    : undefined;

  $: selectedType = formData.jobType
    ? {
        value: formData.jobType,
        label: jobTypeOptions.find(o => String(o.value) === String(formData.jobType))?.label
      }
    : undefined;

  async function handleSubmit() {
    if (isSubmitting) return;
    isSubmitting = true;
    try {
      const jobData = {
        jobCategory: parseInt(formData.jobCategory),
        jobType: parseInt(formData.jobType),
        jobGender: formData.postAsCouple? 3: parseInt(user?.gender),
        locale: formData.locale,
        description: formData.description,
        employerPhoneNumber: formData.employerPhoneNumber
      };
      const newPast = await createPastJob(jobData);

      toasts.add({
        title: 'Success',
        description: 'Job posted successfully!',
        type: 'success',
        duration: 3000
      });
      dispatch('submit', newPast);
      dispatch('close');
    } catch (error) {
      toasts.add({
        title: 'Error',
        description: error.message || 'Failed to post job',
        type: 'error',
        duration: 3000
      });
    } finally {
      isSubmitting = false;
    }
  }

  function handleClose() {
    formData = { ...initialFormState };
    dispatch('close');
  }
</script>

<div
  class="fixed inset-0 z-50 flex items-center justify-center p-2 sm:p-4 bg-black/10"
  role="dialog"
  aria-modal="true"
>
  <div
    class="relative bg-white rounded-3xl w-full max-w-xs sm:max-w-md flex flex-col max-h-[90vh] shadow-2xl border border-gray-100"
  >
    <button
      on:click={handleClose}
      aria-label="Close"
      class="absolute top-4 right-4 text-gray-400 hover:text-secondary focus:outline-none focus:ring-2 focus:ring-secondary"
    >&times;</button>

    <div class="px-4 sm:px-8 pt-8 pb-4 text-center">
      <h2 class="text-2xl font-bold">Add a Past Job</h2>
      <p class="text-sm text-gray-500">Fill in the details below</p>
    </div>

    <form
      on:submit|preventDefault={handleSubmit}
      class="flex-1 overflow-y-auto px-4 sm:px-8 space-y-4 pb-6"
    >
      <!-- Category & Type -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <Label for="jobCategory">Category</Label>
          <Select.Root
            selected={selectedCategory}
            onSelectedChange={opt => formData.jobCategory = opt?.value ?? ''}
          >
            <Select.Trigger
              aria-label="Category"
              class="w-full focus:border-secondary focus:ring-secondary"
            >
              <Select.Value placeholder="Select category" />
            </Select.Trigger>
            <Select.Content>
              {#each jobCategoryOptions as opt}
                <Select.Item value={opt.value} label={opt.label} />
              {/each}
            </Select.Content>
          </Select.Root>
        </div>
        <div>
          <Label for="jobType">Job Type</Label>
          <Select.Root
            selected={selectedType}
            onSelectedChange={opt => formData.jobType = opt?.value ?? ''}
          >
            <Select.Trigger
              aria-label="Job Type"
              class="w-full focus:border-secondary focus:ring-secondary"
            >
              <Select.Value placeholder="Select type" />
            </Select.Trigger>
            <Select.Content>
              {#each jobTypeOptions as opt}
                <Select.Item value={opt.value} label={opt.label} />
              {/each}
            </Select.Content>
          </Select.Root>
        </div>
      </div>

      <!-- Locale -->
      <div>
        <Label for="locale">Locale</Label>
        <Input
          id="locale"
          type="text"
          bind:value={formData.locale}
          placeholder="Enter location"
          required
          class="placeholder-gray-400 focus:border-secondary focus:ring-secondary w-full"
        />
      </div>

      <!-- Description -->
      <div>
        <Label for="description">Description</Label>
        <Textarea
          id="description"
          bind:value={formData.description}
          placeholder="Describe the job in detail"
          required
          class="placeholder-gray-400 h-24 focus:border-secondary focus:ring-secondary w-full"
        />
      </div>

      <!-- Employer Phone -->
      <div>
        <Label for="employerPhoneNumber">Employer Phone</Label>
        <Input
          id="employerPhoneNumber"
          bind:value={formData.employerPhoneNumber}
          type="tel"
          placeholder="Enter phone number"
          required
          class="placeholder-gray-400 focus:border-secondary focus:ring-secondary w-full"
        />
      </div>

      {#if user?.coupleName}
        <div class="flex items-center justify-start gap-2">
          <span class="text-sm text-gray-600">Couple Job</span>
          <label class="relative inline-block w-12 h-6">
            <input type="checkbox" bind:checked={formData.postAsCouple} class="sr-only peer" />
            <div class="absolute inset-0 bg-gray-200 rounded-full peer-checked:bg-secondary peer-focus:ring-2 peer-focus:ring-secondary transition"></div>
            <div class="absolute left-1 top-1 w-4 h-4 bg-white rounded-full shadow transform peer-checked:translate-x-6 transition"></div>
          </label>
        </div>
      {/if}

      <!-- Actions -->
      <div class="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-3 pt-4">
        <Button
          variant="outline"
          on:click={handleClose}
          disabled={isSubmitting}
          class="w-full sm:w-auto"
        >
          Cancel
        </Button>
        <Button
          variant="secondary"
          type="submit"
          disabled={isSubmitting}
          class="w-full sm:w-auto"
        >
          {isSubmitting ? 'Posting...' : 'Post Job'}
        </Button>
      </div>
    </form>
  </div>
</div>
