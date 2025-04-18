<script>
  import MobileFilters from './MobileFilters.svelte';
  import DesktopFilters from './DesktopFilters.svelte';
  import FilterDropdown from './FilterDropdown.svelte';

  export let filters;
  export let sortOption = '';
  export let jobs;

  let showMobileFilters = false;

  function closeMobileFilters() {
    showMobileFilters = false;
  }

  const sortOptions = [
    { value: 'Wage: Low to High', label: 'Wage: Low to High' },
    { value: 'Wage: High to Low', label: 'Wage: High to Low' },
    { value: 'Experience: Low to High', label: 'Experience: Low to High' },
    { value: 'Experience: High to Low', label: 'Experience: High to Low' }
  ];
</script>

<div class="space-y-6">
  <!-- Mobile View -->
  <div class="md:hidden space-y-4">
    <div class="flex justify-between items-center">
      <button
        class="px-4 py-2 border border-gray-300 rounded-lg flex items-center gap-2 text-sm font-medium"
        on:click={() => showMobileFilters = true}
        aria-label="Open filters panel"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
        </svg>
        Filters
      </button>
    </div>
    <div class="flex justify-end">
      <FilterDropdown
        label="Sort by"
        field="sortOption"
        options={sortOptions}
        selectedSet={sortOption ? [sortOption] : []}
        isSortBy={true}
      />
    </div>
  </div>

  <!-- Mobile Panel -->
  <MobileFilters
    {filters}
    {sortOption}
    {jobs}
    bind:showMobileFilters
    close={closeMobileFilters}
    on:updateFilters
  />

  <!-- Desktop Panel -->
  <DesktopFilters {filters} {sortOption} {jobs} on:updateFilters />
</div>

<style>
  :global(.grid) {
    margin-top: 2rem;
  }
</style>
  