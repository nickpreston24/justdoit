drop table if exists signups;
create table if not exists signups
(
    id          INT PRIMARY KEY,
    email       text,
    credit_card text
);

select count(id) as 'signed_up'
from signups;
