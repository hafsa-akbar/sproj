<script>
  import { JobCategoryMap, JobExperienceMap } from '$lib/config/jobConfig';
  import { jobsStore } from '$lib/stores';
  
  export let onCloseJob;
  $: jobs = $jobsStore;
</script>

<section>
  <div class="flex items-center space-x-2 text-[#767676] text-lg font-medium mb-6">
    <img src="/images/job.png" alt="Current Jobs" class="w-6 h-6" />
    <span>Current Jobs</span>
  </div>
  {#if jobs.length === 0}
      <p class="text-left ml-10 text-sm text-tertiary">No Current Job Listings to Show</p>
  {/if}
  <div class="max-h-52 overflow-y-auto pr-2 space-y-4">
    {#each jobs as job}
      <div class="group relative z-0 hover:z-10 border border-gray-200 hover:border-secondary rounded-2xl p-4 transform transition-all duration-200">
        <!-- header -->
        <div class="flex justify-between items-center">
          <div class="flex items-center gap-2">
            {#if job.jobGender === 3}
              <img src="/images/couple.png" alt="Couple" class="w-4 h-4" />
            {/if}
            {#if job.jobGender === 2}
              <img src="/images/female.png" alt="Female" class="w-4 h-4" />
            {/if}
            {#if job.jobGender === 1}
              <img src="/images/male.png" alt="Male" class="w-4 h-4" />
            {/if}
            <b class="text-heading-gray">{JobCategoryMap[job.jobCategory]}</b>
            <span
              class="px-2 py-0.5 text-xs rounded"
              class:bg-yellow-50={job.jobType === 2}
              class:text-yellow-500={job.jobType === 2}
              class:bg-green-50={job.jobType !== 2}
              class:text-green-600={job.jobType !== 2}
            >
              {job.jobType === 2 ? 'Full Time' : 'One Shot'}
            </span>
          </div>
          <button
            class="text-red-600 text-sm hover:text-red-700 transition-colors"
            onclick={() => onCloseJob(job)}
          >Close Job</button>
        </div>
        <p class="text-sm text-tertiary mt-2 block group-hover:hidden">
          Looking for Work, <b>{JobExperienceMap[job.jobExperience]}</b> Expertise
        </p>
        <p class="text-sm text-tertiary mt-2 hidden group-hover:block">{job.description}</p>
      </div>
    {/each}
  </div>
</section> 