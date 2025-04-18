/**
 * Job Categories Enum
 * @readonly
 * @enum {number}
 */
export const JobCategory = {
  COOKING: 1,
  CLEANING: 2,
  DRIVING: 3,
  LAUNDRY: 4,
  GARDENING: 5,
  BABYSITTING: 6,
  PET_CARE: 7,
  SECURITY: 8
};

/**
 * Experience Levels Enum
 * @readonly
 * @enum {number}
 */
export const ExperienceLevel = {
  BEGINNER: 1,
  INTERMEDIATE: 2,
  EXPERT: 3
};

/**
 * Job Types Enum
 * @readonly
 * @enum {number}
 */
export const JobType = {
  ONE_SHOT: 1,
  PERMANENT: 2
};

/**
 * Gender Types Enum
 * @readonly
 * @enum {number}
 */
export const GenderType = {
  MALE: 1,
  FEMALE: 2,
  COUPLE: 3
};

/**
 * Job Categories Select Options
 * @readonly
 * @type {Array<{value: string, label: string}>}
 */
export const jobCategoryOptions = [
  { value: JobCategory.COOKING.toString(), label: 'Cooking' },
  { value: JobCategory.CLEANING.toString(), label: 'Cleaning' },
  { value: JobCategory.DRIVING.toString(), label: 'Driving' },
  { value: JobCategory.LAUNDRY.toString(), label: 'Laundry' },
  { value: JobCategory.GARDENING.toString(), label: 'Gardening' },
  { value: JobCategory.BABYSITTING.toString(), label: 'Babysitting' },
  { value: JobCategory.PET_CARE.toString(), label: 'Pet Care' },
  { value: JobCategory.SECURITY.toString(), label: 'Security Guard' }
];

/**
 * Experience Level Select Options
 * @readonly
 * @type {Array<{value: string, label: string}>}
 */
export const experienceLevelOptions = [
  { value: ExperienceLevel.BEGINNER.toString(), label: 'Beginner' },
  { value: ExperienceLevel.INTERMEDIATE.toString(), label: 'Intermediate' },
  { value: ExperienceLevel.EXPERT.toString(), label: 'Expert' }
];

/**
 * Job Type Select Options
 * @readonly
 * @type {Array<{value: string, label: string}>}
 */
export const jobTypeOptions = [
  { value: JobType.ONE_SHOT.toString(), label: 'One Shot' },
  { value: JobType.PERMANENT.toString(), label: 'Permanent' }
];

/**
 * Get gender label from type
 * @param {number} genderType
 * @returns {string}
 */
export function getGenderLabel(genderType) {
  switch (genderType) {
    case GenderType.MALE:
      return 'Male';
    case GenderType.FEMALE:
      return 'Female';
    case GenderType.COUPLE:
      return 'Couple';
    default:
      return 'Unknown';
  }
} 