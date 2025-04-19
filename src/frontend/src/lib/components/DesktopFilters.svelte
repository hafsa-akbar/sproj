<script>
  import { createEventDispatcher } from 'svelte';
  import FilterDropdown from './FilterDropdown.svelte';
  import { 
    jobTypeOptions,
    experienceLevelOptions as experienceOptions,
    GenderType
  } from '$lib/config/jobConfig';

  const dispatch = createEventDispatcher();

  export let filters = {};
  export let sortOption = '';
  export let jobs = [];

  const genderOptions = [
    { value: GenderType.MALE.toString(), label: 'Male' },
    { value: GenderType.FEMALE.toString(), label: 'Female' },
    { value: GenderType.COUPLE.toString(), label: 'Couple' }
  ];

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
      options={experienceOptions}
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
      options={[
        { value: 'Value', label: 'Value' },
        { value: 'Mid-range', label: 'Mid-range' },
        { value: 'High-end', label: 'High-end' }
      ]}
      bind:selectedSet={filters.wageFilters}
      on:change={handleFilterChange}
    />

    <div class="ml-auto flex items-center gap-2">
      {#if Object.values(filters).some(set => set.size > 0)}
        <button
          class="text-sm text-gray-500 hover:text-gray-700"
          on:click={clearFilters}
        >
          Clear all
        </button>
      {/if}

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
  