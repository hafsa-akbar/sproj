<script>
  import { createEventDispatcher } from 'svelte';
  import { clickOutside } from '$lib/actions/clickOutside';

  const dispatch = createEventDispatcher();

  export let label = '';
  export let field;
  export let options = [];
  export let selectedSet = new Set();
  export let wageBands = null;
  export let isWage = false;
  export let value = '';

  let isOpen = false;
  let selected = Array.from(selectedSet);

  function toggle() {
    isOpen = !isOpen;
  }

  function handleClickOutside() {
    isOpen = false;
  }

  function handleOptionClick(optValue) {
    if (field === undefined) {
      value = value === optValue ? '' : optValue;
      dispatch('change');
      isOpen = false;
    } else {
      const newSet = new Set(selectedSet);
      newSet.has(optValue) ? newSet.delete(optValue) : newSet.add(optValue);
      selectedSet = newSet;
      dispatch('change');
    }
  }

  function clearSelection() {
    selectedSet = new Set();
    dispatch('change');
  }

  $: {
    const newSelected = Array.from(selectedSet);
    if (JSON.stringify(newSelected) !== JSON.stringify(selected)) {
      selected = newSelected;
    }
  }

  $: if (isWage && wageBands) {
    options = options.map(opt => ({
      ...opt,
      label: opt.value,
      range: opt.value === 'Value'
        ? `PKR ${wageBands.low}/hr or less`
        : opt.value === 'Mid-range'
        ? `PKR ${wageBands.low}/hr - ${wageBands.high}/hr`
        : `PKR ${wageBands.high}/hr or more`
    }));
  }

  $: sortCategory = value.includes('Wage') ? 'Wage'
                  : value.includes('Experience') ? 'Experience'
                  : value ? value.split(':')[0].trim()
                  : '';
</script>

<div class="relative {field === undefined ? 'ml-2' : ''}" use:clickOutside on:clickoutside={handleClickOutside}>
  <button
    class="px-4 py-2 {field === undefined ? '' : 'border border-gray-300 rounded-lg'} flex items-center gap-1 text-sm hover:shadow-sm transition"
    on:click={toggle}
  >
    {#if field === undefined}
      <span class="font-medium">Sort By:</span>
      {#if sortCategory}
        <span class="text-gray-500 ml-1 text-sm">{sortCategory}</span>
      {/if}
    {:else}
      <span class="font-medium">{label}</span>
      {#if selectedSet.size > 0}
        <span class="text-gray-500 ml-1">({selectedSet.size})</span>
      {/if}
    {/if}
    <svg 
      class="w-4 h-4 text-gray-400 transition-transform {isOpen ? 'rotate-180' : ''}" 
      fill="none" 
      stroke="currentColor" 
      viewBox="0 0 24 24"
    >
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
    </svg>
  </button>

  {#if isOpen}
    <div 
      class="absolute mt-2 {field === undefined ? 'w-56 right-0' : 'w-72'} bg-white border border-gray-200 rounded-lg shadow-lg p-4 z-50"
    >
      <div class="space-y-2 max-h-60 overflow-y-auto">
        {#each options as opt}
          <label class="flex items-start gap-2 text-sm cursor-pointer hover:bg-gray-50 p-2 rounded">
            <input
              type={field === undefined ? 'radio' : 'checkbox'}
              class="mt-1 h-4 w-4 border-gray-300 {field === undefined ? 'rounded-full' : 'rounded'}"
              checked={field === undefined ? value === opt.value : selectedSet.has(opt.value)}
              on:change={() => handleOptionClick(opt.value)}
              name={field === undefined ? 'sort-option' : undefined}
            />
            <div>
              <div class="font-medium">{opt.label}</div>
              {#if opt.range}
                <div class="text-xs text-gray-500">{opt.range}</div>
              {/if}
            </div>
          </label>
        {/each}
      </div>

      {#if field !== undefined && selectedSet.size > 0 && field !== 'sort-option'}
        <div class="flex justify-end mt-4 pt-2 border-t border-gray-100">
          <button 
            class="text-xs text-gray-600 hover:text-gray-900" 
            on:click={clearSelection}
          >
            Clear all
          </button>
        </div>
      {/if}
    </div>
  {/if}
</div>

<style>
  :global(input[type="checkbox"], input[type="radio"]) {
    accent-color: var(--color-secondary);
  }

  @media (max-width: 768px) {
    .absolute {
      position: fixed;
      left: 1rem;
      right: 1rem;
      width: auto !important;
      max-width: calc(100% - 2rem);
    }

    button {
      width: 100%;
      justify-content: space-between;
    }
  }
</style>
