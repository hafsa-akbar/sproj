export const JobCategoryMap = {
  1: 'Cooking',
  2: 'Cleaning',
  3: 'Driving',
  4: 'Laundry',
  5: 'Gardening',
  6: 'Babysitting',
  7: 'Pet Care',
  8: 'Security Guard',
};

export const JobExperienceMap = {
  1: 'Beginner',
  2: 'Intermediate',
  3: 'Expert',
};

export const JobTypeMap = {
  1: 'One Shot',
  2: 'Full Time',
};

export const JobGenderMap = {
  1: 'Male',
  2: 'Female',
  3: 'Couple',
};

export const GenderAvatarMap = {
  1: 'boy',
  2: 'girl',
};

export const UserGenderMap = {
  1: 'Male',
  2: 'Female',
};

export const IdTypeMap = {
  1: 'Driving License',
  2: 'CNIC',
};

export const wageOptions = [
  { value: 'Value', label: 'Value' },
  { value: 'Mid-range', label: 'Mid-range' },
  { value: 'High-end', label: 'High-end' }
];

export const ExperienceTooltip = "Experience Level reflects the worker's skill & expertise.";
export const TrialPeriodTooltip = "Trial Period is a probation time for permanent hires.";

// Select options for dropdowns
export const jobCategoryOptions = Object.entries(JobCategoryMap).map(([value, label]) => ({
  value,
  label
}));

export const experienceLevelOptions = Object.entries(JobExperienceMap).map(([value, label]) => ({
  value,
  label
}));

export const jobTypeOptions = Object.entries(JobTypeMap).map(([value, label]) => ({
  value,
  label
}));

export const genderOptions = Object.entries(JobGenderMap).map(([value, label]) => ({
  value,
  label
})); 