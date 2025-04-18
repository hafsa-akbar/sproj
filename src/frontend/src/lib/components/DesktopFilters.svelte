<script>
  import { createEventDispatcher } from 'svelte';
  import FilterDropdown from '$lib/components/FilterDropdown.svelte';
  
  const dispatch = createEventDispatcher();
  
  export let filters;
  export let sortOption = '';
  export let jobs = [];

  const jobTypeOptions = [
    { value: '1', label: 'One Shot Job' },
    { value: '2', label: 'Permanent Hire' }
  ];
  const genderOptions = [
    { value: '1', label: 'Male' },
    { value: '2', label: 'Female' },
    { value: '3', label: 'Couple' }
  ];
  const experienceOptions = [
    { value: '1', label: 'Beginner' },
    { value: '2', label: 'Intermediate' },
    { value: '3', label: 'Expert' }
  ];
  const sortOptions = [
    { value: 'Wage: Low to High', label: 'Wage: Low to High' },
    { value: 'Wage: High to Low', label: 'Wage: High to Low' },
    { value: 'Experience: Low to High', label: 'Experience: Low to High' },
    { value: 'Experience: High to Low', label: 'Experience: High to Low' }
  ];

  $: cityOptions = Array.from(new Set(jobs.map(j => j.locale.toLowerCase())))
    .map(c => ({ value: c, label: c.charAt(0).toUpperCase() + c.slice(1) }));

  $: wageBands = (() => {
    if (!jobs.length) return { low: 0, high: 0 };
    const arr = jobs.map(j => j.wageRate).sort((a, b) => a - b);
    const n = arr.length;
    return { low: arr[Math.floor(n * 0.4)], high: arr[Math.floor(n * 0.8)] };
  })();

    function handleChange(event) {
     const { field, selected } = event.detail;
     if (field === 'sortOption') {
       // single‑select
       sortOption = selected[0] || '';
    } else {
       // multi‑select: write back into filters[field]
       filters[field] = new Set(selected);
    }
     dispatch('updateFilters', { filters, sortOption });
    }
</script>

<div class="hidden md:block space-y-6">
  <div class="flex flex-wrap gap-4 items-center">
    <FilterDropdown 
      label="Job Type" 
      field="jobTypes" 
      options={jobTypeOptions} 
      selectedSet={filters.jobTypes}
      on:change={handleChange}
    />
    <FilterDropdown 
      label="Gender" 
      field="genders" 
      options={genderOptions} 
      selectedSet={filters.genders}
      on:change={handleChange}
    />
    <FilterDropdown 
      label="Experience" 
      field="experiences" 
      options={experienceOptions} 
      selectedSet={filters.experiences}
      on:change={handleChange}
    />
    <FilterDropdown 
      label="City" 
      field="locales" 
      options={cityOptions} 
      selectedSet={filters.locales}
      on:change={handleChange}
    />
    <FilterDropdown 
      label="Budget" 
      field="wageFilters" 
      options={[
        { value: 'Value', label: 'Value' }, 
        { value: 'Mid-range', label: 'Mid-range' }, 
        { value: 'High-end', label: 'High-end' }
      ]} 
      selectedSet={filters.wageFilters} 
      wageBands={wageBands} 
      isWage={true}
      on:change={handleChange}
    />
  </div>

  <div class="flex justify-end">
    <FilterDropdown
      label="Sort by"
      field="sortOption"
      options={sortOptions}
      selectedSet={sortOption ? [sortOption] : []}
      isSortBy={true}
      on:change={handleChange}
    />
  </div>
</div>

<style>
  :global(.grid) {
    margin-top: 2rem;
  }
</style>
  