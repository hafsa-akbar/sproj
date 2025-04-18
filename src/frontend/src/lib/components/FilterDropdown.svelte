<script>
  import { createEventDispatcher } from 'svelte';
  import { clickOutside } from '$lib/actions/clickOutside';
  
  const dispatch = createEventDispatcher();

  export let label = '';
  export let field = '';
  export let options = [];
  export let selectedSet = new Set();
  export let wageBands = null;
  export let isWage = false;
  export let isSortBy = false;

  let isOpen = false;
  let selected = Array.from(selectedSet);

  function toggle() {
    isOpen = !isOpen;
  }

  function handleClickOutside() {
    isOpen = false;
  }

  function handleOptionClick(value) {
    if (isSortBy) {
      // For sort, only allow one selection
      selected = [value];
    } else {
      const index = selected.indexOf(value);
      if (index === -1) {
        selected = [...selected, value];
      } else {
        selected = selected.filter(v => v !== value);
      }
    }
    dispatch('change', { field, selected });
  }

  function clearAll() {
    selected = [];
    dispatch('change', { field, selected });
  }

  $: {
    // Update selected when selectedSet changes externally
    const newSelected = Array.from(selectedSet);
    if (JSON.stringify(newSelected) !== JSON.stringify(selected)) {
      selected = newSelected;
    }
  }

  // Update wage option labels based on wage bands
  $: if (isWage && wageBands) {
    options = options.map(opt => ({
      ...opt,
      label: opt.value,
      range: opt.value === 'Value' ? `PKR ${wageBands.low}/hr or less` :
             opt.value === 'Mid-range' ? `PKR ${wageBands.low}/hr - ${wageBands.high}/hr` :
             `PKR ${wageBands.high}/hr or more`
    }));
  }
</script>

<div class="relative {isSortBy ? 'mx-4' : ''}" use:clickOutside on:clickoutside={handleClickOutside}>
  <button
    class="px-4 py-2 {isSortBy ? '' : 'border border-gray-300'} rounded-lg flex items-center gap-1 text-sm hover:shadow-sm transition"
    on:click={toggle}
  >
    {#if isSortBy}
      <span class="font-medium">Sort by</span>
      {#if selected[0]}
        <span class="text-gray-500 ml-1">{selected[0].split(':')[0].trim()}</span>
      {/if}
    {:else}
      <span class="font-medium">{label}</span>
    {/if}
    <svg 
      class="w-4 h-4 text-gray-600 transition-transform {isOpen ? 'rotate-180' : ''}" 
      fill="none" 
      stroke="currentColor" 
      viewBox="0 0 24 24"
    >
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
    </svg>
  </button>

  {#if isOpen}
    <div class="absolute mt-2 {isSortBy ? 'w-56' : 'w-72'} bg-white border border-gray-200 rounded-lg shadow-lg p-4 z-50 {isSortBy ? 'right-0' : ''}">
      <div class="space-y-2 max-h-60 overflow-y-auto">
        {#each options as opt}
          <label class="flex items-start gap-2 text-sm cursor-pointer hover:bg-gray-50 p-2 rounded">
            <input
              type="{isSortBy ? 'radio' : 'checkbox'}"
              class="mt-1 h-4 w-4 border-gray-300 {isSortBy ? 'rounded-full' : 'rounded'}"
              checked={selected.includes(opt.value)}
              on:change={() => handleOptionClick(opt.value)}
              name={isSortBy ? 'sort-option' : undefined}
            />
            <div>
              <div class="font-medium">{opt.label}</div>
              {#if isWage && opt.range}
                <div class="text-xs text-gray-500">{opt.range}</div>
              {/if}
            </div>
          </label>
        {/each}
      </div>

      {#if !isSortBy && selected.length > 0}
        <div class="flex justify-end mt-4 pt-2 border-t border-gray-100">
          <button class="text-xs text-gray-600 hover:text-gray-900" on:click={clearAll}>
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
  
  /* Position dropdown on small screens */
  @media (max-width: 768px) {
    .absolute {
      position: fixed;
      left: 1rem;
      right: 1rem;
      width: auto !important;
    }
  }
</style>