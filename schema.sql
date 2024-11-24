create table locales
(
    locale_id   serial primary key,
    locale_name varchar(32) not null
);
insert into locales (locale_name)
values ('lahore'),
       ('islamabad');

create table job_categories
(
    job_category_id          serial primary key,
    job_category_description varchar(32) not null
);
insert into job_categories (job_category_description)
values ('cooking'),
       ('cleaning'),
       ('driving'),
       ('laundry'),
       ('gardening'),
       ('babysitting'),
       ('pet care'),
       ('security guard');

create table job_types
(
    job_type_id          serial primary key,
    job_type_description varchar(32) not null
);
insert into job_types (job_type_description)
values ('one shot'),
       ('permanent hire');

create table job_experiences
(
    job_experience_id          serial primary key,
    job_experience_description varchar(32) not null
);
insert into job_experiences (job_experience_description)
values ('beginner'),
       ('intermediate'),
       ('expert');

create table roles
(
    role_id          serial primary key,
    role_description varchar(32) not null
);
insert into roles (role_description)
values ('unregistered'),
       ('employer'),
       ('worker');

-- reference tables end

create table users
(
    user_id         serial primary key,
    phone_number    varchar(15)  not null,
    password        varchar(128) not null,
    role_id         integer      not null,
    full_name       varchar(128) not null,
    address         text         not null,
    birthdate       date         not null,
    cnic_number     varchar(32) null,
    driving_license varchar(32) null,

    foreign key (role_id) references roles (role_id)
);

create table couples
(
    male_id   integer not null,
    female_id integer not null,

    primary key (male_id, female_id),
    foreign key (female_id) references users (user_id),
    foreign key (male_id) references users (user_id)
);

create table sms_verifications
(
    user_id           integer    not null,
    verification_code varchar(6) not null,

    primary key (user_id),
    foreign key (user_id) references users (user_id)
);

create table cnic_verifications
(
    user_id    integer not null,
    cnic_image bytea   not null,

    primary key (user_id),
    foreign key (user_id) references users (user_id)
);

create table user_prefs
(
    user_id    integer not null,
    job_locale integer null,

    primary key (user_id),
    foreign key (user_id) references users (user_id),
    foreign key (job_locale) references locales (locale_id)
);

create table job_category_prefs
(
    user_id         integer not null,
    job_category_id integer not null,

    primary key (user_id, job_category_id),
    foreign key (user_id) references users (user_id),
    foreign key (job_category_id) references job_categories (job_category_id)
);

create table job_type_prefs
(
    user_id     integer not null,
    job_type_id integer not null,

    primary key (user_id, job_type_id),
    foreign key (user_id) references users (user_id),
    foreign key (job_type_id) references job_types (job_type_id)
);

create table job_experience_prefs
(
    user_id           integer not null,
    job_experience_id integer not null,

    primary key (user_id, job_experience_id),
    foreign key (user_id) references users (user_id),
    foreign key (job_experience_id) references job_experiences (job_experience_id)
);

create table worker_details
(
    user_id integer not null,

    primary key (user_id),
    foreign key (user_id) references users (user_id)
);

create table job
(
    job_id            serial primary key,
    user_id           integer not null,
    job_category_id   integer not null,
    job_type_id       integer not null,
    job_experience_id integer not null,
    is_couple_job     boolean not null,
    wage_rate         integer not null,
    locale_id         integer not null,

    foreign key (user_id) references worker_details (user_id),
    foreign key (job_experience_id) references job_experiences (job_experience_id),
    foreign key (job_category_id) references job_categories (job_category_id),
    foreign key (job_type_id) references job_types (job_type_id),
    foreign key (locale_id) references locales (locale_id)
);

create table permanent_job
(
    job_id       integer primary key,
    trial_period integer not null,

    foreign key (job_id) references job (job_id)
);

-- TODO:
-- 1. End of interaction (payment chat)
-- 2. Recomendation system
