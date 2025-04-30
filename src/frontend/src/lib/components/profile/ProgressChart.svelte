<script lang="ts">
  import { onMount } from 'svelte';
  import { Chart } from 'frappe-charts/dist/frappe-charts.min.esm';

  const chartData = {
    labels: [
      '12am-3am', '3am-6am', '6am-9am', '9am-12pm',
      '12pm-3pm', '3pm-6pm', '6pm-9pm', '9pm-12am'
    ],
    datasets: [
      { name: 'Some Data', type: 'bar', values: [25, 40, 30, 35, 8, 52, 17, -4] },
      { name: 'Another Set', type: 'line', values: [25, 50, -10, 15, 18, 32, 27, 14] }
    ]
  };

  const chartOptions = {
    title: 'My Awesome Chart',
    data: chartData,
    type: 'axis-mixed',
    height: 250,
    colors: ['#7cd6fd', '#743ee2']
  };

  onMount(() => {
      new Chart('#chart', chartOptions);
  });
</script>

<div id="chart"></div>
  