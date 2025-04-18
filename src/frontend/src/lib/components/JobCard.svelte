<script>
  export let job;

  function getExperienceWidth(level) {
    return level === 1 ? '33%' : level === 2 ? '66%' : '100%';
  }

  function getExperienceLabel(level) {
    return level === 1 ? 'Beginner' : level === 2 ? 'Intermediate' : 'Expert';
  }

  function getCategoryName(id) {
    const map = {
      1: 'Cooking', 2: 'Cleaning', 3: 'Driving', 4: 'Laundry',
      5: 'Gardening', 6: 'Babysitting', 7: 'PetCare', 8: 'SecurityGuard'
    };
    return map[id] || 'Unknown';
  }

  function getDisplayWage(job) {
    return Number(job.jobType) === 2 ? job.wageRate * 300 : job.wageRate;
  }

  function getImageType(job) {
    return Number(job.jobGender) === 2 ? 'girl' : 'boy';
  }

  $: imageUrl = `https://avatar.iran.liara.run/public/${getImageType(job)}?username=${job.workerName}`;
</script>

<div class="card-container hover:shadow-lg hover:scale-[1.015] transition duration-300 my-4">
  <!-- Floating image -->
  <div class="image-wrapper">
    <img
      src={imageUrl}
      alt="Worker"
      class="profile-image"
      width="80"
      height="80"
    />
  </div>

  <div class="card-body">
    <h3 class="worker-name">{job.workerName}</h3>
    <p class="category">{getCategoryName(job.jobCategory)}</p>

    <p class="wage">
      {job.jobType === 2 
        ? `PKR ${(job.wageRate * 300).toLocaleString()}/month` 
        : `PKR ${job.wageRate}/hour`}
    </p>

    <div class="location">
      <span class="icon">📍</span>
      <span>{job.locale}</span>
    </div>

    <!-- Experience -->
    <div class="experience-container">
      <div class="experience-bar">
        <div class="fill" style="width: {getExperienceWidth(job.jobExperience)};"></div>
      </div>
      <span class="level-text">{getExperienceLabel(job.jobExperience)}</span>
    </div>

    <!-- Job type -->
    <div class="job-type-badge {job.jobType === 1 ? 'badge-secondary' : 'badge-primary'}">
      {job.jobType === 1 ? 'One Shot' : 'Permanent'}
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
  }

  .image-wrapper {
    position: absolute;
    top: -40px;
    left: 1.5rem;
    width: 80px;
    aspect-ratio: 1 / 1;
    border-radius: 9999px;
    overflow: hidden;
    border: 4px solid white;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  }

  .profile-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .card-body {
    margin-top: 2rem;
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
    margin-bottom: 0.75rem;
  }

  .wage {
    font-size: 1rem;
    font-weight: 500;
    color: #222;
    margin-bottom: 0.75rem;
  }

  .location {
    font-size: 0.875rem;
    color: #555;
    display: flex;
    align-items: center;
    gap: 0.25rem;
    margin-bottom: 1rem;
  }

  .experience-container {
    margin-bottom: 1rem;
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