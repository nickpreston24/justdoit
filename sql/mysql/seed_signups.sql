drop table if exists signups;
create table if not exists signups
(
    id               INT NOT NULL AUTO_INCREMENT,
    email            varchar(150),
    credit_card      varchar(50),
    application_name varchar(250),

    # PK's
    PRIMARY KEY (id)
);

select distinct email, credit_card
from signups;
# 
# SELECT email, credit_card, COUNT(email) AS `email_count`
# FROM signups
# ORDER BY email_count desc;

select count(email)
from signups;

## nuke
### delete from signups where id > 0


