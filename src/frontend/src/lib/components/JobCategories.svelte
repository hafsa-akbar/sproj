<script>
  import { createEventDispatcher } from 'svelte';
  const dispatch = createEventDispatcher();
  export let selectedCategories = new Set();

  const categories = [
    { id: 1, name: 'Cooking' },
    { id: 2, name: 'Cleaning' },
    { id: 3, name: 'Driving' },
    { id: 4, name: 'Laundry' },
    { id: 5, name: 'Gardening' },
    { id: 6, name: 'Babysitting' },
    { id: 7, name: 'PetCare' },
    { id: 8, name: 'SecurityGuard' }
  ];

  function toggleCategory(categoryId) {
    const updated = new Set(selectedCategories);
    if (updated.has(categoryId)) {
      updated.delete(categoryId);
    } else {
      updated.add(categoryId);
    }
    selectedCategories = updated;
    dispatch('updateCategories', { selectedCategories });
  }
</script>

<div class="flex justify-between items-center px-8 py-6 overflow-x-auto">
  {#each categories as category}
    <button
      on:click={() => toggleCategory(category.id)}
      class="px-6 py-2 transition-colors relative 
             {selectedCategories.has(category.id) 
               ? 'text-[var(--color-secondary)]' 
               : 'text-gray-600 hover:text-gray-900'}"
    >
      {category.name}
      {#if selectedCategories.has(category.id)}
        <div 
          class="absolute bottom-0 left-1/2 transform -translate-x-1/2 
                 w-1/2 h-0.5 bg-[var(--color-secondary)]">
        </div>
      {/if}
    </button>
  {/each}
</div>  
  
<style></style>
  