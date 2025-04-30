<script>
  import { createEventDispatcher } from 'svelte';
  const dispatch = createEventDispatcher();

  function handleSelect(type) {
    dispatch('select', type);
  }
</script>

<div class="flex flex-col items-center space-y-8">
  <h2 class="text-2xl font-semibold text-gray-800">Choose Your Account Type</h2>
  <div class="flex justify-center space-x-16">
    <button 
      class="flex flex-col items-center space-y-4 transition-transform duration-200 hover:scale-105"
      on:click={() => handleSelect('working')}
    >
      <div class="relative">
        <div class="absolute inset-0 flex items-center justify-center">
          <div class="w-28 h-28 bg-gray-200 rounded-lg"></div>
        </div>
        <img src="/images/working.svg" alt="Working" class="relative w-38 h-38" />
      </div>
      <span class="text-lg font-medium text-gray-700">Working</span>
    </button>
    <button 
      class="flex flex-col items-center space-y-4 transition-transform duration-200 hover:scale-105"
      on:click={() => handleSelect('hiring')}
    >
      <div class="relative">
        <div class="absolute inset-0 flex items-center justify-center">
          <div class="w-28 h-28 bg-gray-200 rounded-lg"></div>
        </div>
        <img src="/images/hiring.svg" alt="Hiring" class="relative w-38 h-38" />
      </div>
      <span class="text-lg font-medium text-gray-700">Hiring</span>
    </button>
  </div>          
</div>
