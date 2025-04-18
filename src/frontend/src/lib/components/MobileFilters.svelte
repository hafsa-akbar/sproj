<script>
  import { createEventDispatcher } from 'svelte';
  import { 
    jobTypeOptions,
    experienceLevelOptions as experienceOptions,
    GenderType
  } from '$lib/config/jobConfig';

  const dispatch = createEventDispatcher();

  export let filters = {};
  export let sortOption = '';
  export let jobs = [];
  export let showMobileFilters;
  export let close;

  const genderOptions = [
    { value: GenderType.MALE.toString(), label: 'Male' },
    { value: GenderType.FEMALE.toString(), label: 'Female' },
    { value: GenderType.COUPLE.toString(), label: 'Couple' }
  ];

  $: cityOptions = Array.from(new Set(jobs.map(j => j.locale.toLowerCase())))
    .map(c => ({ value: c, label: c.charAt(0).toUpperCase() + c.slice(1) }));

  function clearFilters() {
    filters.jobTypes = new Set();
    filters.genders = new Set();
    filters.experiences = new Set();
    sortOption = '';
    updateFilters();
  }

  function updateFilters() {
    dispatch('updateFilters', { filters, sortOption });
  }
</script>

{#if showMobileFilters}
  <div class="fixed inset-0 z-50 flex h-screen">
    <!-- Sliding panel -->
    <aside
      class="w-3/4 max-w-sm h-screen bg-white shadow-xl transform transition-transform duration-300 ease-out flex flex-col"
    >
      <div class="p-6 flex-1 overflow-auto">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-lg font-medium">Filters</h2>
          <button on:click={close} class="text-gray-500 hover:text-gray-700" aria-label="Close filters">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {#each [
          { title: 'Job Type', field: 'jobTypes', opts: jobTypeOptions },
          { title: 'Gender', field: 'genders', opts: genderOptions },
          { title: 'Experience', field: 'experiences', opts: experienceOptions },
          { title: 'City', field: 'locales', opts: cityOptions }
        ] as group}
          <div class="mb-6">
            <h3 class="font-semibold mb-2">{group.title}</h3>
            <div class="space-y-2">
              {#each group.opts as opt}
                <label class="flex items-center gap-2">
                  <input
                    type="checkbox"
                    class="h-4 w-4 accent-[var(--color-secondary)]"
                    checked={filters[group.field].has(opt.value)}
                    on:change={() => {
                      const s = new Set(filters[group.field]);
                      s.has(opt.value) ? s.delete(opt.value) : s.add(opt.value);
                      filters[group.field] = s;
                      updateFilters();
                    }}
                  />
                  <span class="text-sm">{opt.label}</span>
                </label>
              {/each}
            </div>
          </div>
        {/each}
      </div>

      <div class="mt-auto p-6">
        <button
          class="w-full text-sm text-[var(--color-secondary)] font-medium py-2"
          on:click={clearFilters}
        >
          Clear all
        </button>
      </div>
    </aside>

    <!-- Invisible click‑catcher on the rest of the screen -->
    <div
      class="flex-1"
      role="button"
      tabindex="0"
      aria-label="Close filters"
      on:click={close}
      on:keydown={(e) => (e.key === 'Enter' || e.key === ' ') && close()}
    ></div>
  </div>
{/if}
  
<style></style>
  