<script>
  import { onMount } from 'svelte';
  import JobCategories from '$lib/components/jobs/JobCategories.svelte';
  import FilterControls from '$lib/components/jobs/FilterControls.svelte';
  import JobCard from '$lib/components/jobs/JobCard.svelte';
  import JobCardModal from '$lib/components/jobs/JobCardModal.svelte';
  import { getJobs } from '$lib/api.js';
  import { jobsStore } from '$lib/stores';

  // ——————————————————————————————————————————————
  // Reactive state
  // ——————————————————————————————————————————————
  // 1. Subscribe to the store
  $: jobs = $jobsStore;

  // 2. Filter criteria
  let selectedCategories = new Set();
  let filters = {
    jobTypes:    new Set(),
    wageFilters: new Set(),
    genders:     new Set(),
    experiences: new Set(),
    locales:     new Set()
  };
  let sortOption = '';

  // 3. What modal is open?
  let selectedJobId = null;

  // ——————————————————————————————————————————————
  // Derive filteredJobs in one go
  // ——————————————————————————————————————————————
  $: filteredJobs = (() => {
    // start from the full list
    let list = jobs;

    // category filter
    if (selectedCategories.size) {
      list = list.filter(job =>
        selectedCategories.has(Number(job.jobCategory))
      );
    }

    // other discrete filters
    const { jobTypes, genders, experiences, locales, wageFilters } = filters;
    if (jobTypes.size)
      list = list.filter(j => jobTypes.has(j.jobType.toString()));
    if (genders.size)
      list = list.filter(j => genders.has(j.jobGender.toString()));
    if (experiences.size)
      list = list.filter(j => experiences.has(j.jobExperience.toString()));
    if (locales.size)
      list = list.filter(j => locales.has(j.locale.toLowerCase()));

    // wage buckets
    if (wageFilters.size && list.length) {
      const displayWages = list.map(j =>
        Number(j.jobType) === 2 ? j.wageRate * 300 : j.wageRate
      ).sort((a,b)=>a-b);
      const low  = displayWages[Math.floor(displayWages.length * 0.2)];
      const high = displayWages[Math.floor(displayWages.length * 0.8)];

      list = list.filter(job => {
        const w = Number(job.jobType) === 2 ? job.wageRate*300 : job.wageRate;
        return Array.from(wageFilters).some(f =>
          f === 'Value'     ? w <= low :
          f === 'Mid-range' ? w >  low && w < high :
          f === 'High-end'  ? w >= high :
          true
        );
      });
    }

    // sorting
    if (sortOption) {
      const dir = sortOption.includes('Low to High') ? 1 : -1;
      if (sortOption.startsWith('Wage')) {
        list = [...list].sort((a,b) =>
          dir * (
            (Number(a.jobType) === 2 ? a.wageRate*300 : a.wageRate)
            - (Number(b.jobType) === 2 ? b.wageRate*300 : b.wageRate)
          )
        );
      } else {
        list = [...list].sort((a,b) =>
          dir * (Number(a.jobExperience) - Number(b.jobExperience))
        );
      }
    }

    return list;
  })();

  // ——————————————————————————————————————————————
  // Lifecycle & handlers
  // ——————————————————————————————————————————————
  onMount(async () => {
    try {
      const { jobs } = await getJobs();
      jobsStore.set(jobs);
    } catch (err) {
      console.error('Could not load jobs:', err);
    }
  });

  function handleJobClick(id) {
    selectedJobId = id;
  }
  function closeModal() {
    selectedJobId = null;
  }
  function updateCategories(e) {
    selectedCategories = e.detail.selectedCategories;
  }
  function updateFilters(e) {
    filters    = e.detail.filters;
    sortOption = e.detail.sortOption;
  }
</script>

<div class="relative min-h-screen">
  <!-- ... background, breadcrumb, heading ... -->

  <JobCategories
    bind:selectedCategories
    on:updateCategories={updateCategories}
  />

  <FilterControls
    {filters}
    {sortOption}
    {jobs}
    on:updateFilters={updateFilters}
  />

  <div class="grid sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-8 mt-2 pt-2">
    {#if filteredJobs.length}
      {#each filteredJobs as job}
        <JobCard {job} on:select={e => handleJobClick(e.detail)} />
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
