<script>
  import { createEventDispatcher } from 'svelte';
  import { JobCategoryMap, JobExperienceMap, GenderAvatarMap } from '$lib/config/jobConfig';
  
  const dispatch = createEventDispatcher();
  export let job;
  console.log(job);

  function getExperienceWidth(level) {
    return level === 1 ? '33%' : level === 2 ? '66%' : '100%';
  }

  function getDisplayWage(job) {
    return Number(job.jobType) === 2 ? job.wageRate * 300 : job.wageRate;
  }

  function getFirstName(fullName) {
    return fullName?.split(' ')[0] ?? '';
  }

  $: isCouple = Number(job.jobGender) === 3;

  function handleClick() {
    dispatch('select', job.jobId);
  }

  $: img_src = `https://avatar.iran.liara.run/public/${job.workers[0].gender === 1 ? 'boy' : 'girl'}?username=${job.workers[0].fullName.split(' ').join('')}`
  $: img_src_couple = `https://avatar.iran.liara.run/public/${job.workers[1]?.gender === 1 ? 'boy' : 'girl'}?username=${job.workers[1]?.fullName.split(' ').join('')}`

</script>

<div 
  class="card-container hover:shadow-lg hover:scale-[1.015] transition duration-300 my-4"
  on:click={handleClick}
  role="button"
  tabindex="0"
  on:keydown={(e) => e.key === 'Enter' && handleClick()}
  aria-label="View job details"
>

  <!-- Floating images -->
  <div class="image-wrapper {isCouple ? 'couple-hover-wrapper' : ''}">
    {#if isCouple}
    <div class="couple-images">
      {#if img_src}
        <img
          src={img_src}
          alt="Worker 1"
          class="profile-image first"
          width="80"
          height="80"
        />
      {/if}
  
      {#if img_src_couple}
        <img
          src={img_src_couple}
          alt="Worker 2"
          class="profile-image second"
          width="80"
          height="80"
        />
      {/if}
    </div>
    {:else}
    {#if img_src}
      <img
        src={img_src}
        alt="Worker"
        class="profile-image"
        width="80"
        height="80"
      />
      {/if}
    {/if}
  </div>

  <div class="card-body">
    <h3 class="worker-name">
      {#if isCouple}
        {getFirstName(job.workers[0].fullName)} & {getFirstName(job.workers[1].fullName)}
      {:else}
        {getFirstName(job.workers[0].fullName)}
      {/if}
    </h3>
    <p class="category">{JobCategoryMap[job.jobCategory]}</p>

    <p class="wage">
      {job.jobType === 2 
        ? `PKR ${(job.wageRate * 300).toLocaleString()}/month` 
        : `PKR ${job.wageRate}/hour`}
    </p>

    <div class="location">
      <span class="icon">📍</span>
      <span>{job.locale}</span>
    </div>

    <!-- Compact Experience + Tags Block -->
    <div class="bottom-align">
      <div class="experience-container">
        <div class="experience-bar">
          <div class="fill" style="width: {getExperienceWidth(job.jobExperience)};"></div>
        </div>
        <span class="level-text">{JobExperienceMap[job.jobExperience]}</span>
      </div>

      <div class="tags">
        <div class="job-type-badge {job.jobType === 1 ? 'badge-secondary' : 'badge-primary'}">
          {job.jobType === 1 ? 'One Shot' : 'Permanent'}
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  .card-container {
    background: white;
    border-radius: 2rem;
    padding: 2.5rem 1.5rem 1.75rem;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
    font-family: var(--font-poppins);
    position: relative;
    transition: all 0.3s ease;
    height: 280px; /* reduced height */
    display: flex;
    flex-direction: column;
  }

  .image-wrapper {
    position: absolute;
    top: -40px;
    left: 1.5rem;
    width: 80px;
    height: 80px;
  }

  .profile-image {
    width: 80px;
    height: 80px;
    object-fit: cover;
    border-radius: 9999px;
    border: 4px solid white;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  }

  .couple-images {
    position: relative;
    width: 100px;
    height: 80px;
    transition: all 0.3s ease;
  }

  .couple-images .first {
    position: absolute;
    top: 0;
    left: 0;
    z-index: 10;
    transition: transform 0.3s;
  }

  .couple-images .second {
    position: absolute;
    top: 0;
    left: 20px;
    z-index: 5;
    opacity: 0.9;
    transition: transform 0.3s;
  }

  /* Decouple more */
  .couple-hover-wrapper:hover .first {
    transform: translateX(-20px);
  }

  .couple-hover-wrapper:hover .second {
    transform: translateX(30px);
  }

  .card-body {
    margin-top: 2rem;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
  }

  .worker-name {
    font-size: 1.25rem;
    font-weight: 600;
    margin-bottom: 0.05rem;
  }

  .category {
    font-size: 0.875rem;
    font-weight: 500;
    color: #888;
    margin-bottom: 0.4rem;
  }

  .wage {
    font-size: 1rem;
    font-weight: 500;
    color: #222;
    margin-bottom: 0.4rem;
  }

  .location {
    font-size: 0.875rem;
    color: #555;
    display: flex;
    align-items: center;
  }

  .bottom-align {
    margin-top: auto;
  }

  .experience-container {
    margin-bottom: 0.5rem;
    position: relative;
  }

  .experience-bar {
    height: 6px;
    background-color: #e5e5e5;
    border-radius: 8px;
    overflow: hidden;
  }

  .experience-bar .fill {
    height: 100%;
    background-color: var(--color-secondary);
    border-radius: 8px;
  }

  .level-text {
    font-size: 0.75rem;
    color: #666;
    position: absolute;
    right: 0;
    bottom: -1.25rem;
  }

  .tags {
    display: flex;
    align-items: center;
    gap: 0.5rem;
  }

  .job-type-badge {
    display: inline-block;
    padding: 0.3rem 0.75rem;
    font-size: 0.75rem;
    font-weight: 600;
    border-radius: 9999px;
    border: 1.5px solid;
    background: transparent;
  }

  .badge-primary {
    color: var(--color-primary);
    border-color: var(--color-primary);
  }

  .badge-secondary {
    color: var(--color-secondary);
    border-color: var(--color-secondary);
  }
</style>
