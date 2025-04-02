<script>
	import '../app.css';
	import { ToastContainer, FlatToast } from 'svelte-toasts';
	import Navbar from '$lib/components/Navbar.svelte';
</script>

<Navbar />

<slot />

<ToastContainer let:data>	
	<FlatToast {data} />
</ToastContainer>
  