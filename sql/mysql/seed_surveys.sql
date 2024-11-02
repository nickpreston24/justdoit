drop table if exists surveys;
create table if not exists surveys
(
    id               INT NOT NULL AUTO_INCREMENT,
    question         text,
    answer           text,
    application_name varchar(150),
    created_at       datetime default now(),
#     user_email       varchar(150),
    # PK's
    PRIMARY KEY (id)
);

### Sample
insert into surveys
    (question, answer, application_name)
values ( 'nicholas.preston24@gmail.com'
       , 'What is the airspeed velocity of an unladen swallow?'
       , 'African or European?');

select *
from surveys;