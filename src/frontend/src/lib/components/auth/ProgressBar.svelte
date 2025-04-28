<script>
  export let currentStep;
  export let steps = [
    { number: 1, label: 'Basic Info' },
    { number: 2, label: 'Verification' },
    { number: 3, label: 'Account Type' },
    { number: 4, label: 'Verify ID' }
  ];
</script>

<div class="w-full max-w-md mb-8 px-4 relative">
  <div class="absolute top-2 left-12 right-12 h-0.5 bg-tertiary">
    <!-- Filled portion uses primary -->
    <div
      class="h-full bg-secondary transition-all duration-300"
      style="width: {currentStep >= 2 ? (currentStep >= 3 ? (currentStep >= 4 ? '100%' : '66%') : '33%') : '0%'}"
    ></div>
  </div>
  
  <div class="relative flex justify-between items-center">
    {#each steps as step}
      <div class="flex flex-col items-center">
        <div 
          class="w-5 h-5 rounded-full text-xs flex items-center justify-center border-2 transition-colors duration-300
            {currentStep >= step.number 
              ? 'bg-secondary border-secondary text-white' 
              : 'bg-tertiary border-tertiary text-white'}"
        >
          {step.number}
        </div>
        <span class="mt-2 text-xs font-medium {currentStep >= step.number ? 'text-secondary' : 'text-tertiary'}">
          {step.label}
        </span>
      </div>
    {/each}
  </div>
</div>
