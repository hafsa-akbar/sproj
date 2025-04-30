<script>
    import { createEventDispatcher } from 'svelte';
    import { verifyReview } from '$lib/api';
    import { JobCategoryMap, JobTypeMap } from '$lib/config/jobConfig';
  
    const dispatch = createEventDispatcher();
    export let job;
  
    let rating = 0;
    let comments = '';
    let isSubmitting = false;
    let error = null;
  
    async function handleSubmit() {
      if (rating === 0) {
        error = 'Please select a rating';
        return;
      }
  
      isSubmitting = true;
      error = null;
  
      try {
        await verifyReview(job.pastJobId, rating, comments);
        dispatch('success');
      } catch (e) {
        error = e.message;
      } finally {
        isSubmitting = false;
      }
    }
  </script>
  
  <div class="fixed inset-0 flex items-center justify-center bg-black/20 z-[999]">
    <button class="absolute inset-0 cursor-pointer" on:click={() => dispatch('close')} aria-label="Close modal"></button>
  
    <div class="relative bg-white rounded-3xl w-[90%] max-w-[630px] flex flex-col max-h-[80vh] font-[var(--font-poppins)] p-8">
      <button class="absolute top-4 right-4 text-[var(--color-tertiary)] text-xl hover:text-secondary transition-colors" on:click={() => dispatch('close')} aria-label="Close">×</button>
  
      <h2 class="text-2xl font-semibold text-heading-gray mb-4">Review Job</h2>
  
      <div class="space-y-4">
        <div>
          <h3 class="font-medium text-lg">{JobCategoryMap[job.jobCategory]}</h3>
          <p class="text-sm text-tertiary">{job.description}</p>
          <p class="text-sm text-tertiary mt-1">
            {job.workerNames.join(' & ')} • {JobTypeMap[job.jobType]} • {job.locale}
          </p>
        </div>
  
        <!-- Star Rating using static SVGs -->
        <div class="space-y-2">
          <span class="block text-sm font-medium text-gray-700">Rating</span>
          <div class="flex gap-2 mt-1">
            {#each Array(5) as _, i}
              <button
                type="button"
                on:click={() => rating = i + 1}
                aria-label={`Rate ${i + 1} stars`}
                class="focus:outline-none"
              >
                <img
                  src={rating > i ? '/images/filled_star.svg' : '/images/empty_star.svg'}
                  alt={`${i + 1} star`}
                  class="w-8 h-8"
                />
              </button>
            {/each}
          </div>
        </div>
  
        <div class="space-y-2">
          <label for="comments" class="block text-sm font-medium text-gray-700">Comments</label>
          <textarea
            id="comments"
            bind:value={comments}
            class="w-full h-32 p-2 border rounded-lg focus:border-secondary focus:ring-secondary"
            placeholder="Share your experience..."
          ></textarea>
        </div>
  
        {#if error}
          <p class="text-red-500 text-sm">{error}</p>
        {/if}
  
        <button
          class="w-full py-2 bg-[var(--color-secondary)] text-white rounded-lg hover:bg-secondary-dark transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          on:click={handleSubmit}
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Submitting...' : 'Submit Review'}
        </button>
      </div>
    </div>
  </div>
  