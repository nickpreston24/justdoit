drop table if exists signups;
create table if not exists signups
(
    id          INT NOT NULL AUTO_INCREMENT,
    email       text,
    credit_card text,

    # PK's
    PRIMARY KEY (id)
);

# select distinct count(email) as 'signed_up'
# from signups;
# 

SELECT email, credit_card, COUNT(email) AS `email_count`
FROM signups
GROUP BY email, credit_card
ORDER BY email_count desc;

# 
# select distinct email
# from signups;