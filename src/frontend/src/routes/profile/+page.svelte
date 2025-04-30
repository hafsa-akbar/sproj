<script>
    import { onMount } from 'svelte';
    import { authUser, jobsStore } from '$lib/stores';
    import {
      getWorkerDetails,
      getPendingReviews,
      closeJob
    } from '$lib/api';
    import ProgressChart from '$lib/components/profile/ProgressChart.svelte';
    import CloseJobModal from '$lib/components/profile/CloseJobModal.svelte';
    import AddPastJobModal from '$lib/components/profile/AddPastJobModal.svelte';
    import ProfileHeader from '$lib/components/profile/ProfileHeader.svelte';
    import CurrentJobs from '$lib/components/profile/CurrentJobs.svelte';
    import WorkHistory from '$lib/components/profile/WorkHistory.svelte';
    import ReviewModal from '$lib/components/profile/ReviewModal.svelte';
    import PendingReviews from '$lib/components/profile/PendingReviews.svelte';
    import { JobCategoryMap, JobTypeMap } from '$lib/config/jobConfig';

    let data = $state({ rating: 0, pastJobs: [], pendingReviews: [] });
    let showCloseJobModal = $state(false);
    let showAddPastJobModal = $state(false);
    let showReviewModal = $state(false);
    let selectedJob = $state(null);
    let selectedReview = $state(null);

    let user = $state($authUser);
    let role = $derived(user?.role);
    
    onMount(async () => {
      try {
        if (role === 3) { // Worker
          const details = await getWorkerDetails($authUser.userId);
          data = { ...data, ...details };
          jobsStore.set(details.jobs);
          data.pastJobs = details.pastJobs;
        }
        
        if (role > 1) { // Worker and Employer
        const reviews = await getPendingReviews();
        data.pendingReviews = reviews;
        }
      } catch (e) {
        console.error(e);
      }
    });

    function openAddPastJobModal() {
      showAddPastJobModal = true;
    }

    function handleAddPastJobSubmit(event) {
      const newPast = event.detail;
      data.pastJobs = [newPast, ...data.pastJobs];
      showAddPastJobModal = false;
    }

    function handleCloseJob(job) {
      selectedJob = job;
      showCloseJobModal = true;
    }

    function handleDeleteJob() {
      jobsStore.update(js => js.filter(j => j.jobId !== selectedJob.jobId));
      showCloseJobModal = false;
      selectedJob = null;
    }

    function handleCloseAndAddPastJob(event) {
      const newPastJob = event.detail;
      jobsStore.update(js => js.filter(j => j.jobId !== selectedJob.jobId));
      data.pastJobs = [newPastJob, ...data.pastJobs];
      showCloseJobModal = false;
      selectedJob = null;
    }

    function handleReviewClick(e) {
      selectedReview = e.detail;
      showReviewModal = true;
    }

    function handleReviewSuccess() {
      data.pendingReviews = data.pendingReviews.filter(r => r.pastJobId !== selectedReview.pastJobId);
      showReviewModal = false;
      selectedReview = null;
    }
</script>

<div class="flex flex-col md:flex-row p-8 space-y-8 md:space-y-0 md:space-x-8">
  <ProfileHeader rating={data.rating} />

  <!-- Pending Reviews -->

  <div class="flex-1 flex flex-col space-y-10">
    {#if data.pendingReviews.length > 0}
      <PendingReviews
        reviews={data.pendingReviews}
        on:select={handleReviewClick}
      />
    {/if}
    {#if role === 3} <!-- Worker -->

      <!-- Job Analytics -->
      <section>
        <div class="flex items-center space-x-2 text-[#767676] text-lg font-medium mb-6 mt-4">
          <img src="/images/analytics.png" alt="Analytics" class="w-6 h-6" />
          <span>Job Analytics</span>
        </div>
        <ProgressChart />
      </section>

      <!-- Current Jobs -->
      <CurrentJobs onCloseJob={handleCloseJob} />

      <!-- Work History -->
      <WorkHistory pastJobs={data.pastJobs} onAddPastJob={openAddPastJobModal} />
    {:else} <!-- Employer -->
      <section class="bg-gray-50 rounded-xl p-6">
        <div class="flex items-center space-x-3 mb-4">
          <img src="/images/employer.png" alt="Employer" class="w-8 h-8" />
          <h2 class="text-xl font-semibold text-heading-gray">Welcome, Employer!</h2>
        </div>
        <div class="space-y-4 text-gray-700">
          <p>As an employer on our platform, you can:</p>
          <div class="space-y-2">
            <div class="flex items-center space-x-2">
              <img src="/images/check.png" alt="Check" class="w-5 h-5" />
              <span>Post jobs and find qualified workers</span>
            </div>
            <div class="flex items-center space-x-2">
              <img src="/images/check.png" alt="Check" class="w-5 h-5" />
              <span>Review your past hires to help build trust in our community</span>
            </div>
            <div class="flex items-center space-x-2">
              <img src="/images/check.png" alt="Check" class="w-5 h-5" />
              <span>Upgrade to a worker account to access more features</span>
            </div>
          </div>
          <p class="text-sm italic">Your reviews help maintain quality standards and build a reliable workforce.</p>
        </div>
      </section>
    {/if}
  </div>
</div>

{#if showAddPastJobModal}
  <AddPastJobModal
    on:submit={handleAddPastJobSubmit}
    on:close={() => (showAddPastJobModal = false)}
  />
{/if}

{#if showCloseJobModal && selectedJob}
  <CloseJobModal
    jobId={selectedJob.jobId}
    jobCategory={selectedJob.jobCategory}
    jobType={selectedJob.jobType}
    jobGender={selectedJob.jobGender}
    description={selectedJob.description}
    locale={selectedJob.locale}
    on:close={() => (showCloseJobModal = false)}
    on:success={handleDeleteJob}
    on:submit={handleCloseAndAddPastJob}
  />
{/if}

{#if showReviewModal && selectedReview}
  <ReviewModal
    job={selectedReview}
    on:close={() => (showReviewModal = false)}
    on:success={handleReviewSuccess}
  />
{/if}
