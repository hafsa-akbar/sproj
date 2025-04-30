<script>
  import { JobCategoryMap } from '$lib/config/jobConfig';
  
  export let pastJobs = [];
  export let onAddPastJob;

  function handleAddPastJob() {
    onAddPastJob();
  }
</script>

<section>
  <div class="flex items-center justify-between">
    <div class="flex items-center space-x-2 text-[#767676] text-lg font-medium mb-6 mt-4">
      <img src="/images/time_logo.svg" alt="History" class="w-6 h-6" />
      <span>Work History</span>
    </div>
    <button
      class="px-4 py-1 bg-secondary text-white rounded-md text-sm hover:bg-secondary/90 transition-colors"
      onclick={handleAddPastJob}
    >Add Past Job</button>
  </div>
  {#if pastJobs.length === 0}
      <p class="text-left ml-10 text-sm text-tertiary">No Past Jobs to Show</p>
  {/if}
  <div class="max-h-52 overflow-y-auto pr-2 space-y-4">
    {#each pastJobs as past}
      <div class="group relative z-0 hover:z-10 border border-gray-200 hover:border-secondary rounded-2xl p-4 transform transition-all duration-200">
        <!-- header -->
        <div class="flex justify-between items-center">
          <div class="flex items-center gap-2">
            {#if past.jobGender === 3}
              <img src="/images/couple.png" alt="Couple" class="w-4 h-4" />
            {/if}
            {#if past.jobGender === 2}
              <img src="/images/female.png" alt="Female" class="w-4 h-4" />
            {/if}
            {#if past.jobGender === 1}
              <img src="/images/male.png" alt="Male" class="w-4 h-4" />
            {/if}
            <b class="text-heading-gray">{JobCategoryMap[past.jobCategory]}</b>
            <span
              class="px-2 py-0.5 text-xs rounded"
              class:bg-yellow-50={past.jobType === 2}
              class:text-yellow-600={past.jobType === 2}
              class:bg-green-50={past.jobType !== 2}
              class:text-green-600={past.jobType !== 2}
            >
              {past.jobType === 2 ? 'Full Time' : 'One Shot'}
            </span>
          </div>
          {#if past.isVerified}
            <img src="/images/verified.svg" alt="Verified" class="w-6 h-6" />
          {/if}
        </div>
  
        <!-- snippet -->
        <p class="text-sm text-tertiary mt-2 block group-hover:hidden">
          {past.description.split(' ').slice(0, 5).join(' ')}...
        </p>
  
        <!-- full details on hover -->
        <div class="hidden group-hover:block">
          <!-- full description -->
          <p class="text-sm text-tertiary mt-2">{past.description}</p>
  
          <!-- quote bubble -->
          {#if past.comments}
            <div class="flex justify-center mt-4">
              <div class="relative bg-gray-100 p-3 rounded-lg text-tertiary max-w-prose text-center">
                <span class="absolute -top-3 left-4">
                  <img src="/images/quotes.svg" alt="Quote" class="w-6 h-6" />
                </span>
                <p class="italic text-sm ml-4">{past.comments}</p>
              </div>
            </div>
          {/if}
  
          <!-- Hired by -->
          <p class="text-xs text-tertiary mt-2">Hired by {past.employerPhoneNumber}</p>
  
          <!-- rating -->
          {#if past.rating}
            <div class="flex gap-1 mt-2">
              {#each Array(5) as _, i}
                <img
                  src={i < past.rating ? '/images/filled_star.svg' : '/images/empty_star.svg'}
                  class="w-4 h-4"
                  alt="Star"
                />
              {/each}
            </div>
          {/if}
        </div>
      </div>
    {/each}
  </div>
</section> 