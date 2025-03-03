<script>
    import { writable } from 'svelte/store';
    import { register } from '$lib/api';
    import { loginUser } from '$lib/stores';
    import { goto } from '$app/navigation';
  
    let phoneNumber = writable('');
    let password = writable('');
    let fullName = writable('');
    let address = writable('');
    let birthdate = writable('');
    let gender = writable(1);
    let errorMessage = writable('');
  
    async function handleSignup() {
      errorMessage.set('');
  
      const user = {
        phoneNumber: $phoneNumber,
        password: $password,
        fullName: $fullName,
        address: $address,
        birthdate: $birthdate,
        gender: $gender
      };
  
      try {
        const response = await register(user);
        loginUser(response.user);
        goto('/'); // Redirect to home after signup
      } catch (error) {
        errorMessage.set(error.message);
      }
    }
  </script>
  
  <div class="max-w-md mx-auto p-6 bg-white rounded-lg shadow-md">
    <h2 class="text-2xl font-bold mb-4">Sign Up</h2>
  
    <form on:submit|preventDefault={handleSignup} class="space-y-4">
      <input type="text" bind:value={$phoneNumber} placeholder="Phone Number"
        class="w-full p-2 border rounded-md" required />
  
      <input type="password" bind:value={$password} placeholder="Password"
        class="w-full p-2 border rounded-md" required />
  
      <input type="text" bind:value={$fullName} placeholder="Full Name"
        class="w-full p-2 border rounded-md" required />
      
      <input type="text" bind:value={$address} placeholder="Address"
        class="w-full p-2 border rounded-md" required />
  
      <input type="date" bind:value={$birthdate} placeholder="Birthdate"
        class="w-full p-2 border rounded-md" required />
  
      <select bind:value={$gender} class="w-full p-2 border rounded-md">
        <option value="1">Male</option>
        <option value="2">Female</option>
      </select>
  
      <button type="submit" class="w-full bg-blue-500 text-white p-2 rounded-md">
        Sign Up
      </button>
  
      {#if $errorMessage}
        <p class="text-red-500">{$errorMessage}</p>
      {/if}
    </form>
  
    <p class="mt-4 text-sm">
      Already have an account? <a href="/login" class="text-blue-500">Login</a>
    </p>
  </div>
  