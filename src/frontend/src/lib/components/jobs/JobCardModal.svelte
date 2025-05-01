<script>
  import { onMount, onDestroy } from 'svelte';
  import { getJobDetails } from '$lib/api.js';
  import { GenderAvatarMap } from '$lib/config/jobConfig';
  import {
    JobCategoryMap,
    JobExperienceMap,
    ExperienceTooltip,
    TrialPeriodTooltip,
    JobTypeMap
  } from '$lib/config/jobConfig';

  export let jobId;
  export let closeModal;

  let job = null;
  let showAllPastJobs = false;
  let expTooltip = false;
  let trialTooltip = false;
  let whatsappLink = '';
  let showModal = false;

  function buildWhatsAppLink(number) {
    return `https://api.whatsapp.com/send?phone=${number.replace(/\D/g, '')}`;
  }

  function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
  }

  async function loadJobDetails() {
    job = await getJobDetails(jobId);
    whatsappLink = buildWhatsAppLink(job.workers[0]?.phoneNumber || '');
    showModal = true;
  }

  function handleGlobalKeydown(e) {
    if (e.key === 'Escape') closeModal();
  }

  onMount(() => {
    loadJobDetails();
    window.addEventListener('keydown', handleGlobalKeydown);
  });
  onDestroy(() => window.removeEventListener('keydown', handleGlobalKeydown));
</script>

{#if job}
<div class="fixed inset-0 flex items-center justify-center bg-black/20 z-[999]">
  <button class="absolute inset-0 cursor-pointer" on:click={closeModal} aria-label="Close modal"></button>

  <div class="relative bg-white rounded-3xl w-[90%] max-w-[630px] flex flex-col max-h-[80vh] font-[var(--font-poppins)]">
    <button class="absolute top-4 right-4 text-[var(--color-tertiary)] text-xl" on:click={closeModal} aria-label="Close">×</button>

    <div class="flex-1 flex flex-col overflow-y-auto">
      <div class="flex flex-col sm:flex-row items-center sm:items-start p-8 gap-6">
        <!-- Profile images -->
        <div class="flex items-center gap-2">
          {#each job.workers as worker}
            {#if worker?.fullName}
              <img
                src={`https://avatar.iran.liara.run/public/${worker.gender === 1 ? 'boy' : 'girl'}?username=${worker.fullName.split(' ').join('')}`}
                class="w-28 h-28 rounded-full object-cover border-4 border-white shadow"
                alt="Worker Profile"
              />
            {:else}
              <div class="w-28 h-28 rounded-full bg-gray-200 flex items-center justify-center border-4 border-white shadow">
                <span class="text-gray-400">Loading...</span>
              </div>
            {/if}
          {/each}
        </div>

        <!-- Basic Info -->
        <div class="flex-1 text-center sm:text-left">
          <h2 id="modal-title" class="text-2xl font-semibold text-heading-gray">
            {job.workers.map(w => w.fullName).join(' & ')}
          </h2>
          <hr class="border-t border-secondary my-2" />
          <p class="text-sm text-tertiary">
            Based in <span class="font-bold">{capitalizeFirstLetter(job.locale)}</span>
          </p>
          <p class="text-sm text-tertiary">
            {job.workers.map(w => w.phoneNumber).join(' & ')}
          </p>
        </div>
      </div>

      <!-- Current Job -->
      <div class="px-8">
        <div class="flex justify-between items-center">
          <h3 class="text-lg font-semibold text-heading-gray">
            {JobCategoryMap[job.jobCategory]}, {capitalizeFirstLetter(job.locale)}
          </h3>
          {#if job.jobType}
              <div class="border border-secondary rounded-full px-3 py-1.5 text-xs text-secondary">
                {JobTypeMap[job.jobType]}
              </div>
          {/if}
        </div>
        <p class="text-sm text-tertiary mt-1">{job.description}</p>

        <!-- Tags: Job Type, Experience, Trial -->
        <div class="flex items-center gap-4 mt-4 text-sm text-tertiary">
          <!-- Experience -->
          <div class="flex items-center gap-2 relative group">
            <img
              src="/images/info.png"
              alt="Experience Info"
              class="w-4 h-4 opacity-50 group-hover:opacity-100 transition-opacity duration-200"
              on:mouseenter={() => expTooltip = true}
              on:mouseleave={() => expTooltip = false}
            />
            {#if expTooltip}
              <div class="absolute top-full left-0 mt-1 p-2 text-xs bg-gray-700 text-white rounded shadow-lg">
                <p>{ExperienceTooltip}</p>
              </div>
            {/if}
            <span class="text-tertiary text-sm">Experience Level:</span>
            <span class="text-tertiary font-medium text-sm">{JobExperienceMap[job.jobExperience]}</span>
          </div>
          {#if job.trialPeriod}
            <div class="flex items-center gap-2 relative group">
              <img
                src="/images/info.png"
                alt="Experience Info"
                class="w-4 h-4 opacity-50 group-hover:opacity-100 transition-opacity duration-200"
                on:mouseenter={() => trialTooltip = true}
                on:mouseleave={() => trialTooltip = false}
              />
              {#if trialTooltip}
                <div class="absolute top-full left-0 mt-1 p-2 text-xs bg-gray-700 text-white rounded shadow-lg">
                  <p>{TrialPeriodTooltip}</p>
                </div>
              {/if}
              <span class="text-tertiary text-sm">Trial Period:</span>
              <span class="text-tertiary font-medium text-sm">{job.trialPeriod} days</span>
            </div>
          {/if}
        </div>

        <!-- Past Jobs -->
        <h3 class="text-lg font-semibold text-heading-gray mt-5 mb-2">Past Jobs</h3>

        <div class="flex flex-col gap-4 mb-4">
          {#each (showAllPastJobs ? job.pastJobs : job.pastJobs.slice(0,1)) as past}
            <div class="border border-secondary rounded-2xl p-4">
              <div class="flex justify-between items-center">
                <div class="flex items-center gap-1">
                {#if past.jobGender === 3}
                    <img src="/images/couple.png" alt="Couple Job" class="w-4 h-4 mr-1" />
                {/if}
                {#if past.jobGender === 2}
                    <img src="/images/female.png" alt="Female Worker" class="w-4 h-4 mr-1" />
                {/if}
                {#if past.jobGender === 1}
                    <img src="/images/male.png" alt="Male Worker" class="w-4 h-4 mr-1" />
                {/if}
                <b class="text-heading-gray">{JobCategoryMap[past.jobCategory]}</b>
                </div>
                <div class="flex items-center gap-1">
                  {#if past.isVerified}
                    <img src="/images/verified.svg" alt="Verified" class="w-6 h-6" />
                  {/if}
                </div>
              </div>

              <p class="text-sm text-tertiary mt-2">{past.description}</p>

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

              <p class="text-xs text-tertiary mt-2">Hired by {past.employerName}</p>

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
          {/each}
          {#if job.pastJobs.length > 1}
              <button class="self-center text-sky-400 text-sm" on:click={() => showAllPastJobs = !showAllPastJobs}>
                {showAllPastJobs ? 'See Less' : 'See More'}
              </button>
            {/if}
        </div>
      </div>
    </div>

    <!-- WhatsApp Button -->
    <a
      href={whatsappLink}
      target="_blank"
      rel="noopener"
      class="ml-auto m-4 px-6 py-3 rounded-full bg-[var(--color-secondary)] text-white text-sm whitespace-nowrap"
    >
      Connect on WhatsApp
    </a>
  </div>
</div>
{/if}
