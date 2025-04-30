<script>
    import { createEventDispatcher } from 'svelte';
    import { JobCategoryMap, JobTypeMap } from '$lib/config/jobConfig';
  
    export let reviews = []; 
    const dispatch = createEventDispatcher();
  
    function handleClick(review) {
      dispatch('select', review);
    }
  </script>
  
  <section>
    <div class="flex items-center space-x-2 text-[#767676] text-lg font-medium mb-6 mt-4">
      <img src="/images/reviews.png" alt="Reviews" class="w-6 h-6" />
      <span>Pending Reviews</span>
    </div>
  
    <div class="max-h-64 overflow-y-auto space-y-4">
      {#each reviews as review (review.pastJobId)}
        <button
          type="button"
          class="group w-full bg-white border border-gray-200 rounded-xl p-4 shadow-sm
                 transition-all duration-200 hover:shadow-md hover:border-secondary text-left"
          on:click={() => handleClick(review)}
        >
          <!-- Always visible -->
          <h3 class="font-medium text-lg">
            {JobCategoryMap[review.jobCategory]}
          </h3>
  
          <!-- Hidden until hover -->
          <p class="text-sm text-tertiary mt-1 hidden group-hover:block">
            {review.description}
          </p>
  
          <!-- Also reveal these on hover -->
          <div class="text-sm text-tertiary mt-1">
            <p class="text-sm text-tertiary mt-1 font-medium">
                {review.workerNames.join(' & ')} • {JobTypeMap[review.jobType]} • {review.locale}
            </p>
          </div>
        </button>
      {/each}
    </div>
  </section>
  