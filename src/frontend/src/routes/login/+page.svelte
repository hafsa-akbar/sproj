<script>
    import LoginForm from '$lib/components/auth/LoginForm.svelte';
    import { authUser } from '$lib/stores';
    console.log(authUser);
  </script>
  
  <main class="flex items-center justify-center min-h-screen bg-primary split-bg">
    <LoginForm />
  </main>
  