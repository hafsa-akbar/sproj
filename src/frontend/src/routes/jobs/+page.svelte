<script>
  import { onMount } from 'svelte';
  import JobCategories from '$lib/components/jobs/JobCategories.svelte';
  import FilterControls from '$lib/components/jobs/FilterControls.svelte';
  import JobCard from '$lib/components/jobs/JobCard.svelte';
  import JobCardModal from '$lib/components/jobs/JobCardModal.svelte';
  import { getJobs } from '$lib/api.js';
  import { jobsStore } from '$lib/stores';

  let jobs = $jobsStore;
  let filteredJobs = [];
  let selectedCategories = new Set();
  let selectedJobId = null;

  let filters = {
    jobTypes: new Set(),
    wageFilters: new Set(),
    genders: new Set(),
    experiences: new Set(),
    locales: new Set()
  };
  let sortOption = '';

  function getDisplayWage(job) {
    return Number(job.jobType) === 2 ? job.wageRate * 300 : job.wageRate;
  }

  function calculateWageThresholds(list) {
    const arr = list.map(getDisplayWage).sort((a,b)=>a-b);
    if (!arr.length) return { low: 0, high: 0 };
    return {
      low: arr[Math.floor(arr.length * 0.2)],
      high: arr[Math.floor(arr.length * 0.8)]
    };
  }

  function applyFilters() {
    filteredJobs = jobs.filter(job => {
      if (selectedCategories.size && !selectedCategories.has(Number(job.jobCategory))) return false;
      if (filters.jobTypes.size && !filters.jobTypes.has(job.jobType.toString())) return false;
      if (filters.genders.size && !filters.genders.has(job.jobGender.toString())) return false;
      if (filters.experiences.size && !filters.experiences.has(job.jobExperience.toString())) return false;
      if (filters.locales.size && !filters.locales.has(job.locale.toLowerCase())) return false;
      return true;
    });

    if (filters.wageFilters.size) {
      const { low, high } = calculateWageThresholds(filteredJobs);
      filteredJobs = filteredJobs.filter(job => {
        const w = getDisplayWage(job);
        return Array.from(filters.wageFilters).some(f =>
          f === 'Value'     ? w <= low :
          f === 'Mid-range' ? w >  low && w < high :
          f === 'High-end'  ? w >= high :
          true
        );
      });
    }

    if (sortOption) {
      const dir = sortOption.includes('Low to High') ? 1 : -1;
      if (sortOption.startsWith('Wage')) {
        filteredJobs.sort((a,b) => dir * (getDisplayWage(a) - getDisplayWage(b)));
      } else {
        filteredJobs.sort((a,b) => dir * ((+a.jobExperience) - (+b.jobExperience)));
      }
    }
  }

  function updateFilters(evt) {
    filters    = evt.detail.filters;
    sortOption = evt.detail.sortOption;
    applyFilters();
  }

  function handleJobClick(jobId) {
    selectedJobId = jobId;
  }

  function closeModal() {
    selectedJobId = null;
  }

  onMount(async () => {
    try {
      const result = await getJobs();
      jobsStore.set(result.jobs);
      applyFilters();
    } catch (err) {
      console.error('Could not load jobs:', err);
    }
  });

  // Watch for changes in jobs store and reapply filters
  $: {
    jobs = $jobsStore;
    applyFilters();
  }
</script>

<div class="relative min-h-screen">
  <!-- Background SVG -->
  <div class="fixed bottom-[-40px] right-0 w-96 h-96 md:w-[400px] md:h-[400px] pointer-events-none">
    <img src="/images/bg.svg" alt="Decorative background" class="w-full h-full object-contain" />
  </div>

  <div class="max-w-7xl mx-auto px-4 py-8">
    <!-- Breadcrumb -->
    <div class="flex items-center gap-2 text-gray-500 mb-2 text-sm">
      <a href="/" class="hover:text-gray-700" aria-label="Home">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
          <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"/>
        </svg>
      </a>
      <span>/</span>
      <span>Job Listings</span>
    </div>

    <div class="mb-1">
      <JobCategories
        bind:selectedCategories
        on:updateCategories={e => { selectedCategories = e.detail.selectedCategories; applyFilters(); }}
      />
    </div>

    <!-- Heading Section -->
    <div class="mb-8 text-left md:text-left">
      <h1 class="text-4xl font-bold mb-2">Browse Job Postings</h1>
      <p class="text-gray-500 max-w-2xl">
        Lorem Ipsum is simply dummy text of the printing and typesetting industry.
      </p>
    </div>

    <!-- Filter Section -->
    <div class="mb-12">
      <FilterControls {filters} {sortOption} {jobs} on:updateFilters={updateFilters}/>
    </div>

    <!-- Job Cards Section -->
    <div class="grid sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8 mt-2 pt-2">
      {#if filteredJobs.length}
        {#each filteredJobs as job}
        <JobCard {job} on:click={() => handleJobClick(job.jobId)} />
        {/each}
      {:else}
        <p class="col-span-full text-center text-gray-500 py-8">
          No jobs found matching your filters.
        </p>
      {/if}
    </div>

    {#if selectedJobId}
      <JobCardModal jobId={selectedJobId} closeModal={closeModal} />
    {/if}
  </div>
</div>