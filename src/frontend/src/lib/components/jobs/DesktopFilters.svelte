<script>
  import { createEventDispatcher } from 'svelte';
  import FilterDropdown from '$lib/components/jobs/FilterDropdown.svelte';
  import { 
    jobTypeOptions,
    experienceLevelOptions,
    genderOptions,
    wageOptions
  } from '$lib/config/jobConfig';

  const dispatch = createEventDispatcher();

  export let filters = {};
  export let sortOption = '';
  export let jobs = [];

  const sortOptions = [
    { value: 'Wage: Low to High', label: 'Wage: Low to High' },
    { value: 'Wage: High to Low', label: 'Wage: High to Low' },
    { value: 'Experience: Low to High', label: 'Experience: Low to High' },
    { value: 'Experience: High to Low', label: 'Experience: High to Low' }
  ];

  $: cityOptions = Array.from(new Set(jobs.map(j => j.locale.toLowerCase())))
    .map(c => ({ value: c, label: c.charAt(0).toUpperCase() + c.slice(1) }));

  function handleFilterChange() {
    dispatch('updateFilters', { filters, sortOption });
  }

  function clearFilters() {
    filters.jobTypes = new Set();
    filters.genders = new Set();
    filters.experiences = new Set();
    filters.locales = new Set();
    filters.wageFilters = new Set();
    sortOption = '';
    handleFilterChange();
  }
</script>

<div class="hidden md:block">
  <div class="flex flex-wrap items-center gap-2">
    <FilterDropdown
      label="Job Type"
      field="jobTypes"
      options={jobTypeOptions}
      bind:selectedSet={filters.jobTypes}
      on:change={handleFilterChange}
    />

    <FilterDropdown
      label="Gender"
      field="genders"
      options={genderOptions}
      bind:selectedSet={filters.genders}
      on:change={handleFilterChange}
    />

    <FilterDropdown
      label="Experience"
      field="experiences"
      options={experienceLevelOptions}
      bind:selectedSet={filters.experiences}
      on:change={handleFilterChange}
    />

    <FilterDropdown
      label="Location"
      field="locales"
      options={cityOptions}
      bind:selectedSet={filters.locales}
      on:change={handleFilterChange}
    />

    <FilterDropdown
      label="Wage"
      field="wageFilters"
      options={wageOptions}
      bind:selectedSet={filters.wageFilters}
      on:change={handleFilterChange}
    />

    <div class="ml-auto flex items-center gap-2">
      <FilterDropdown
        options={sortOptions}
        bind:value={sortOption}
        on:change={handleFilterChange}
      />
    </div>
  </div>
</div>

<style>
  :global(.grid) {
    margin-top: 2rem;
  }
</style>
  