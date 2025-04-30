<script>
  import { createEventDispatcher } from 'svelte';
  import { toasts } from 'svelte-toasts';

  export let initialData = {};

  const dispatch = createEventDispatcher();

  // Local form fields, seeded from initialData
  let phoneNumber = initialData.phoneNumber || '';
  let password    = initialData.password    || '';
  let fullName    = initialData.fullName    || '';
  let address     = initialData.address     || '';
  let birthdate   = initialData.birthdate   || '';
  let gender      = initialData.gender      || '';

  // Validation state
  let errors = {
    phoneNumber: '',
    password:    '',
    fullName:    '',
    address:     '',
    birthdate:   '',
    gender:      ''
  };

  // Simple validators
  const validatePhoneNumber = v => !v.trim() ? 'Phone number is required' : '';
  const validatePassword    = v => !v.trim() 
    ? 'Password is required' 
    : v.length < 6 
      ? 'Password must be at least 6 characters' 
      : '';
  const validateFullName    = v => !v.trim() ? 'Full name is required' : '';
  const validateAddress     = v => !v.trim() ? 'Address is required' : '';
  const validateBirthdate   = v => {
    if (!v) return 'Birthdate is required';
    const today = new Date(), b = new Date(v);
    let age = today.getFullYear() - b.getFullYear();
    const m = today.getMonth() - b.getMonth();
    if (m<0 || (m===0 && today.getDate()<b.getDate())) age--;
    return age < 18 ? 'You must be at least 18 years old' : '';
  };
  const validateGender      = v => !v ? 'Gender is required' : '';

  function validateForm() {
    errors.phoneNumber = validatePhoneNumber(phoneNumber);
    errors.password    = validatePassword(password);
    errors.fullName    = validateFullName(fullName);
    errors.address     = validateAddress(address);
    errors.birthdate   = validateBirthdate(birthdate);
    errors.gender      = validateGender(gender);
    return !Object.values(errors).some(e => e);
  }

  function handleSubmit() {
    if (!validateForm()) {
      toasts.add({
        title: 'Error',
        description: 'Please fix all errors before proceeding.',
        type: 'error',
        duration: 3000
      });
      return;
    }
    dispatch('complete', {
      phoneNumber,
      password,
      fullName,
      address,
      birthdate,
      gender: +gender
    });
  }
</script>

<div class="w-full max-w-md mx-auto text-center mb-4">
  <h2 class="text-2xl font-bold">Basic Info</h2>
</div>

<div class="w-full max-w-md mx-4 p-8 bg-transparent">
  <form on:submit|preventDefault={handleSubmit} class="space-y-4">
    <!-- Phone Number -->
    <div>
      <input
        type="text"
        bind:value={phoneNumber}
        placeholder="Phone Number"
        class="w-full p-2 border rounded-md {errors.phoneNumber ? 'border-red-500' : 'border-secondary'}"
        on:input={() => errors.phoneNumber = validatePhoneNumber(phoneNumber)}
      />
      {#if errors.phoneNumber}
        <p class="text-red-500 text-sm mt-1">{errors.phoneNumber}</p>
      {/if}
    </div>

    <!-- Password -->
    <div>
      <input
        type="password"
        bind:value={password}
        placeholder="Password"
        class="w-full p-2 border rounded-md {errors.password ? 'border-red-500' : 'border-secondary'}"
        on:input={() => errors.password = validatePassword(password)}
      />
      {#if errors.password}
        <p class="text-red-500 text-sm mt-1">{errors.password}</p>
      {/if}
    </div>

    <!-- Full Name -->
    <div>
      <input
        type="text"
        bind:value={fullName}
        placeholder="Full Name"
        class="w-full p-2 border rounded-md {errors.fullName ? 'border-red-500' : 'border-secondary'}"
        on:input={() => errors.fullName = validateFullName(fullName)}
      />
      {#if errors.fullName}
        <p class="text-red-500 text-sm mt-1">{errors.fullName}</p>
      {/if}
    </div>

    <!-- Address -->
    <div>
      <input
        type="text"
        bind:value={address}
        placeholder="Address"
        class="w-full p-2 border rounded-md {errors.address ? 'border-red-500' : 'border-secondary'}"
        on:input={() => errors.address = validateAddress(address)}
      />
      {#if errors.address}
        <p class="text-red-500 text-sm mt-1">{errors.address}</p>
      {/if}
    </div>

    <!-- Birthdate -->
    <div>
      <input
        type="date"
        bind:value={birthdate}
        class="w-full p-2 border rounded-md {errors.birthdate ? 'border-red-500' : 'border-secondary'}"
        on:input={() => errors.birthdate = validateBirthdate(birthdate)}
      />
      {#if errors.birthdate}
        <p class="text-red-500 text-sm mt-1">{errors.birthdate}</p>
      {/if}
    </div>

    <!-- Gender -->
    <div>
      <select
        bind:value={gender}
        class="w-full p-2 border rounded-md {errors.gender ? 'border-red-500' : 'border-secondary'}"
        on:change={() => errors.gender = validateGender(gender)}
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
  <p class="mt-4 text-sm text-center">
    Already have an account?
    <a href="/login" class="text-secondary underline">Login</a>
  </p>
</div>
