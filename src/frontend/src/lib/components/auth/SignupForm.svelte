<script>
  import { writable } from 'svelte/store';
  import { createEventDispatcher } from 'svelte';
  import { toasts } from 'svelte-toasts';
  import { register } from '$lib/api';

  const dispatch = createEventDispatcher();

  let phoneNumber = writable('');
  let password = writable('');
  let fullName = writable('');
  let address = writable('');
  let birthdate = writable('');
  let gender = writable('');

  let errors = {
    phoneNumber: '',
    password: '',
    fullName: '',
    address: '',
    birthdate: '',
    gender: ''
  };

  // Validation functions
  function validatePhoneNumber(value) {
    if (!value || value.trim() === '') return 'Phone number is required';
    // optionally add regex
    return '';
  }

  function validatePassword(value) {
    if (!value || value.trim() === '') return 'Password is required';
    if (value.length < 6) return 'Password must be at least 6 characters';
    return '';
  }

  function validateFullName(value) {
    if (!value || value.trim() === '') return 'Full name is required';
    return '';
  }

  function validateAddress(value) {
    if (!value || value.trim() === '') return 'Address is required';
    return '';
  }

  function validateBirthdate(value) {
    if (!value) return 'Birthdate is required';
    const today = new Date();
    const bDate = new Date(value);
    let age = today.getFullYear() - bDate.getFullYear();
    const monthDiff = today.getMonth() - bDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < bDate.getDate())) {
      age--;
    }
    if (age < 18) return 'You must be at least 18 years old';
    return '';
  }

  function validateGender(value) {
    if (!value) return 'Gender is required';
    return '';
  }

  function validateForm() {
    errors.phoneNumber = validatePhoneNumber($phoneNumber);
    errors.password = validatePassword($password);
    errors.fullName = validateFullName($fullName);
    errors.address = validateAddress($address);
    errors.birthdate = validateBirthdate($birthdate);
    errors.gender = validateGender($gender);
    return !Object.values(errors).some(error => error !== '');
  }

  async function handleSignup() {
    if (!validateForm()) {
      toasts.add({
        title: 'Error',
        description: 'Please fix all errors before submitting',
        duration: 3000,
        type: 'error',
        theme: 'dark'
      });
      return;
    }

    const userData = {
      phoneNumber: $phoneNumber,
      password: $password,
      fullName: $fullName,
      address: $address,
      birthdate: $birthdate,
      gender: +$gender
    };

    try {
      const response = await register(userData);
      if (response.user) {
        dispatch('complete', userData);
      }
    } catch (error) {
      toasts.add({
        title: 'Error',
        description: error.message || 'Registration failed. Please try again.',
        duration: 3000,
        type: 'error',
        theme: 'dark'
      });
    }
  }
</script>

<!-- Headline outside the form container -->
<div class="w-full max-w-md mx-auto text-center mb-4">
  <h2 class="text-2xl font-bold">Basic Info</h2>
</div>

<div class="w-full max-w-md mx-4 p-10 bg-white rounded-lg shadow-md">
  <form on:submit|preventDefault={handleSignup} class="space-y-4">
    <!-- Phone Number -->
    <div>
      <input
        type="text"
        bind:value={$phoneNumber}
        placeholder="Phone Number"
        class="w-full p-2 border border-gray-600 rounded-md {errors.phoneNumber ? 'border-red-500' : ''}"
        required
        on:input={(e) => {
          phoneNumber.set(e.target.value);
          errors.phoneNumber = validatePhoneNumber(e.target.value);
        }}
      />
      {#if errors.phoneNumber}
        <p class="text-red-500 text-sm mt-1">{errors.phoneNumber}</p>
      {/if}
    </div>

    <!-- Password -->
    <div>
      <input
        type="password"
        bind:value={$password}
        placeholder="Password"
        class="w-full p-2 border border-gray-600 rounded-md {errors.password ? 'border-red-500' : ''}"
        required
        on:input={(e) => {
          password.set(e.target.value);
          errors.password = validatePassword(e.target.value);
        }}
      />
      {#if errors.password}
        <p class="text-red-500 text-sm mt-1">{errors.password}</p>
      {/if}
    </div>

    <!-- Full Name -->
    <div>
      <input
        type="text"
        bind:value={$fullName}
        placeholder="Full Name"
        class="w-full p-2 border border-gray-600 rounded-md {errors.fullName ? 'border-red-500' : ''}"
        required
        on:input={(e) => {
          fullName.set(e.target.value);
          errors.fullName = validateFullName(e.target.value);
        }}
      />
      {#if errors.fullName}
        <p class="text-red-500 text-sm mt-1">{errors.fullName}</p>
      {/if}
    </div>

    <!-- Address -->
    <div>
      <input
        type="text"
        bind:value={$address}
        placeholder="Address"
        class="w-full p-2 border border-gray-600 rounded-md {errors.address ? 'border-red-500' : ''}"
        required
        on:input={(e) => {
          address.set(e.target.value);
          errors.address = validateAddress(e.target.value);
        }}
      />
      {#if errors.address}
        <p class="text-red-500 text-sm mt-1">{errors.address}</p>
      {/if}
    </div>

    <!-- Birthdate -->
    <div>
      <input
        type="date"
        bind:value={$birthdate}
        placeholder="Birthdate"
        class="w-full p-2 border border-gray-600 rounded-md {errors.birthdate ? 'border-red-500' : ''}"
        required
        on:input={(e) => {
          birthdate.set(e.target.value);
          errors.birthdate = validateBirthdate(e.target.value);
        }}
      />
      {#if errors.birthdate}
        <p class="text-red-500 text-sm mt-1">{errors.birthdate}</p>
      {/if}
    </div>

    <!-- Gender -->
    <div>
      <select
        bind:value={$gender}
        class="w-full p-2 border border-gray-600 rounded-md {errors.gender ? 'border-red-500' : ''}"
        on:change={(e) => {
          gender.set(e.target.value);
          errors.gender = validateGender(e.target.value);
        }}
      >
        <option value="">Select Gender</option>
        <option value="1">Male</option>
        <option value="2">Female</option>
      </select>
      {#if errors.gender}
        <p class="text-red-500 text-sm mt-1">{errors.gender}</p>
      {/if}
    </div>

    <button
      type="submit"
      class="w-full bg-secondary text-white p-2 rounded-md hover:opacity-90 transition-colors"
    >
      Next
    </button>
  </form>

  <p class="mt-4 text-sm">
    Already have an account?
    <a href="/login" class="text-secondary">Login</a>
  </p>
</div>
