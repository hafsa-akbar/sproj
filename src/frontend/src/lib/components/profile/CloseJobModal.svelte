<script>
    import { createEventDispatcher, onMount } from 'svelte';
    import { authUser } from '$lib/stores';
    import { toasts } from 'svelte-toasts';
    import { createPastJob, closeJob } from '$lib/api';
    import { Input } from '../ui/input/index.js';
    import { Label } from '../ui/label/index.js';
    import { Button } from '../ui/button/index.js';
  
    // Props
    export let jobId;
    export let jobCategory;
    export let jobType;
    export let jobGender;
    export let description;
    export let locale;
  
    const dispatch = createEventDispatcher();
    let user;
    $: user = $authUser;
    let employerPhoneNumber = '';
    let isSubmitting = false;
  
    onMount(() => {
      // Prefill employer phone from logged-in user
      employerPhoneNumber = '';
    });
  
    async function handleSubmit() {
      if (isSubmitting) return;
      isSubmitting = true;
      try {
        // Create past job record
        const payload = {
          jobCategory,
          jobType,
          jobGender,
          locale,
          description,
          employerPhoneNumber
        };
        const newPastJob = await createPastJob(payload);
        // Then close the original job
        await closeJob(jobId);
  
        toasts.add({ 
          title: 'Success', 
          description: 'Job closed and added to work history!', 
          type: 'success',
          duration: 3000
        });
        dispatch('submit', newPastJob);
        dispatch('close');
      } catch (e) {
        console.error('Error closing job:', e);
        toasts.add({ 
          title: 'Error', 
          description: e.message || 'Failed to close job', 
          type: 'error',
          duration: 3000
        });
      } finally {
        isSubmitting = false;
      }
    }
  
    async function handleSkip() {
      if (isSubmitting) return;
      isSubmitting = true;
      try {
        await closeJob(jobId);
        toasts.add({ 
          title: 'Success', 
          description: 'Job closed successfully!', 
          type: 'success',
          duration: 3000
        });
        dispatch('success');
        dispatch('close');
      } catch (e) {
        console.error('Error skipping job:', e);
        toasts.add({ 
          title: 'Error', 
          description: e.message || 'Failed to close job', 
          type: 'error',
          duration: 3000
        });
      } finally {
        isSubmitting = false;
      }
    }
  
    function handleClose() {
      if (!isSubmitting) {
        dispatch('close');
      }
    }
  </script>
  
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/20">
    <div class="bg-white rounded-3xl w-full max-w-md max-h-[90vh] flex flex-col shadow-2xl border border-gray-100">
      <button 
        on:click={handleClose} 
        class="self-end p-4 text-gray-500 hover:text-secondary transition-colors"
        disabled={isSubmitting}
      >
        &times;
      </button>
      <div class="px-6 pb-4 space-y-2 text-center">
        <h2 class="text-2xl font-bold">Close Job</h2>
        <p class="text-sm text-gray-500">Great! You're done with this job. Please enter your employer's phone number so we can ask for a review.</p>
      </div>
      <div class="flex-1 overflow-y-auto px-6 space-y-4 pb-6">
        <div>
          <Label for="employerPhoneNumber">Employer Phone Number</Label>
          <Input
            id="employerPhoneNumber"
            type="tel"
            bind:value={employerPhoneNumber}
            class="w-full"
            placeholder="Enter employer's phone number"
            disabled={isSubmitting}
          />
        </div>
        <div class="flex justify-end space-x-4 pt-4">
          <Button 
            variant="outline" 
            on:click={handleSkip} 
            disabled={isSubmitting}
            class="hover:bg-gray-100 transition-colors"
          >
            {isSubmitting ? 'Processing...' : 'Skip and Delete'}
          </Button>
          <Button 
            variant="secondary" 
            on:click={handleSubmit} 
            disabled={isSubmitting}
            class="hover:bg-secondary/90 transition-colors"
          >
            {isSubmitting ? 'Processing...' : 'Submit'}
          </Button>
        </div>
      </div>
    </div>
  </div>
  