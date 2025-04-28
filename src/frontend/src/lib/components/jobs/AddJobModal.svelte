<script>
  import { createEventDispatcher } from 'svelte';
  import { authUser } from '$lib/stores';
  import { toasts } from 'svelte-toasts';
  import { createJob } from '$lib/api';
  import {
    jobCategoryOptions,
    experienceLevelOptions,
    jobTypeOptions
  } from '$lib/config/jobConfig';
  import * as Select from '../ui/select/index.js';
  import { Input } from '../ui/input/index.js';
  import { Textarea } from '../ui/textarea/index.js';
  import { Label } from '../ui/label/index.js';
  import { Button } from '../ui/button/index.js';

  const dispatch = createEventDispatcher();

  const initialFormState = {
    wageRate: '',
    jobCategory: '',
    jobExperience: '',
    jobType: '',
    locale: '',
    description: '',
    postAsCouple: false,
    trialPeriod: ''
  };

  let formData = { ...initialFormState };
  let isSubmitting = false;

  $: user = $authUser;
  $: isPermanent = parseInt(formData.jobType) === 2;

  // derive selected option objects for Radix Select
  $: selectedCategory = formData.jobCategory
    ? {
        value: formData.jobCategory,
        label: jobCategoryOptions.find(o => String(o.value) === String(formData.jobCategory))?.label
      }
    : undefined;

  $: selectedExperience = formData.jobExperience
    ? {
        value: formData.jobExperience,
        label: experienceLevelOptions.find(o => String(o.value) === String(formData.jobExperience))?.label
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
        wageRate: parseInt(formData.wageRate),
        jobCategory: parseInt(formData.jobCategory),
        jobExperience: parseInt(formData.jobExperience),
        jobGender: parseInt(user?.gender),
        jobType: parseInt(formData.jobType),
        locale: formData.locale,
        description: formData.description,
        coupleName: user?.coupleName,
        trialPeriod: isPermanent ? parseInt(formData.trialPeriod) : undefined
      };

      await createJob(jobData);
      toasts.add({
        title: 'Success',
        description: 'Job posted successfully!',
        type: 'success',
        duration: 3000
      });
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
  class="fixed inset-0 z-50 flex items-center justify-center p-2 sm:p-4"
  style="background: rgba(0,0,0,0.1)"
>
  <!-- modal container -->
  <div
    class="relative
           bg-white
           rounded-3xl
           w-full max-w-xs sm:max-w-md
           flex flex-col
           max-h-[90vh]
           my-8 sm:my-4
           shadow-2xl border border-gray-100"
  >
    <!-- close button -->
    <button
      on:click={handleClose}
      aria-label="Close"
      class="absolute top-4 right-4
             text-gray-400 hover:text-secondary
             focus:outline-none focus:ring-2 focus:ring-secondary"
    >
      &times;
    </button>

    <!-- header -->
    <div class="px-4 sm:px-8 pt-8 pb-4 text-center">
      <h2 class="text-2xl font-bold">Add a Job</h2>
      <p class="text-sm text-gray-500">Fill in the details below</p>
    </div>

    <!-- scrollable form body -->
    <form
      on:submit|preventDefault={handleSubmit}
      class="flex-1
             overflow-y-auto
             px-4 sm:px-8
             space-y-4 sm:space-y-5
             pb-6"
    >
      <!-- Wage Rate -->
      <div>
        <Label for="wageRate">Wage Rate</Label>
        <Input
          id="wageRate"
          type="number"
          bind:value={formData.wageRate}
          placeholder="Enter wage rate"
          required
          class="placeholder-gray-400 focus:border-secondary focus:ring-secondary w-full"
        />
      </div>

      <!-- Category & Experience -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <Label for="jobCategory">Category</Label>
          <Select.Root
            selected={selectedCategory}
            onSelectedChange={opt => formData.jobCategory = opt?.value ?? ''}
          >
            <Select.Trigger aria-label="Category" class="w-full focus:border-secondary focus:ring-secondary">
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
          <Label for="jobExperience">Experience</Label>
          <Select.Root
            selected={selectedExperience}
            onSelectedChange={opt => formData.jobExperience = opt?.value ?? ''}
          >
            <Select.Trigger aria-label="Experience" class="w-full focus:border-secondary focus:ring-secondary">
              <Select.Value placeholder="Select experience" />
            </Select.Trigger>
            <Select.Content>
              {#each experienceLevelOptions as opt}
                <Select.Item value={opt.value} label={opt.label} />
              {/each}
            </Select.Content>
          </Select.Root>
        </div>
      </div>

      <!-- Type & Locale -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <Label for="jobType">Job Type</Label>
          <Select.Root
            selected={selectedType}
            onSelectedChange={opt => formData.jobType = opt?.value ?? ''}
          >
            <Select.Trigger aria-label="Job Type" class="w-full focus:border-secondary focus:ring-secondary">
              <Select.Value placeholder="Select type" />
            </Select.Trigger>
            <Select.Content>
              {#each jobTypeOptions as opt}
                <Select.Item value={opt.value} label={opt.label} />
              {/each}
            </Select.Content>
          </Select.Root>
        </div>
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
      </div>

      <!-- Trial Days (Permanent) -->
      {#if isPermanent}
        <div>
          <Label for="trialPeriod">Trial Days</Label>
          <Input
            id="trialPeriod"
            type="number"
            min="0"
            bind:value={formData.trialPeriod}
            placeholder="Enter trial period"
            required
            class="placeholder-gray-400 focus:border-secondary focus:ring-secondary w-full"
          />
        </div>
      {/if}

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

      <!-- Post as Couple -->
      {#if user?.coupleName}
        <div class="flex items-center justify-end gap-2">
          <span class="text-sm text-gray-600">Post as Couple</span>
          <label class="relative inline-block w-12 h-6">
            <input type="checkbox" bind:checked={formData.postAsCouple} class="sr-only peer" />
            <div class="absolute inset-0 bg-gray-200 rounded-full peer-checked:bg-secondary peer-focus:ring-2 peer-focus:ring-secondary transition"></div>
            <div class="absolute left-1 top-1 w-4 h-4 bg-white rounded-full shadow transform peer-checked:translate-x-6 transition"></div>
          </label>
        </div>
      {/if}

      <!-- Actions -->
      <div class="flex flex-col sm:flex-row justify-end space-y-2 sm:space-y-0 sm:space-x-3 pt-4">
        <Button variant="outline" on:click={handleClose} disabled={isSubmitting} class="w-full sm:w-auto">
          Cancel
        </Button>
        <Button variant="secondary" type="button" on:click={handleSubmit} disabled={isSubmitting} class="w-full sm:w-auto">
          {isSubmitting ? 'Posting...' : 'Post Job'}
        </Button>
      </div>
    </form>
  </div>
</div>
