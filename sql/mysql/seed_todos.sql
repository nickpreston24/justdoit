drop table if exists todos;
create table if not exists todos
(
    id            INT NOT NULL AUTO_INCREMENT,
    content       text,
    status        varchar(10),
    priority      int,

    due           datetime,
    created_at    datetime     default now(),
    created_by    varchar(150) default null,
    last_modified datetime     default null,

    # PK's
    PRIMARY KEY (id)
);

# alter table todos
#     ADD COLUMN related_todos json not null default '{}';

select id,
       content,
       created_at,
       due,
       status,
       priority,
       last_modified,
       created_by
from todos
order by priority asc, due desc;

select count(id)
from todos;

# 
# update todos
# set status = 'done'
# where id = 13

## nuke
### delete from todos where id > 0

drop procedure if exists updatetodo;
DELIMITER ^_^
CREATE PROCEDURE updatetodo(
#     days_from_now int
)
BEGIN

    INSERT INTO todos (id, content, due, priority, status)
    VALUES (1, 'testxyzzz', now(), 3, 'Pending'),
           (2, 'testxyzzz', now(), 4, 'Pending'),
           (3, 'testxyzzz', now(), 2, 'Pending'),
           (8, 'testxyzzz', now(), 1, 'Pending')
    ON DUPLICATE KEY UPDATE content = VALUES(content),
                            priority=VALUES(priority),
                            status=VALUES(status);


    select id, content, due, priority, status
    from todos;


    #     set @start = date_add(now(), days_from_now);
#     select @start;

#     CREATE TEMPORARY TABLE todo_schedule_temp
#     SELECT *
#     FROM todos
#     order by due desc;
# 
#     # return
#     select content, due from todo_schedule_temp;
#     
#     drop temporary table todo_schedule_temp;
END ^_^

DELIMITER ;

call updatetodo();



select *
from todos
where content like '%test%';
/*
delete from todos where content like '%zzz%'
 */

/** SCHEDULE VIEW ATTEMPT1 */


CREATE or replace VIEW TimeElapsed AS

#   @now datetime  = now();
select datediff(last_modified, created_at) as days_since_last_modification,
       datediff(now(), created_at)         as days_old, ## i.e., age
       datediff(created_at, due)           as days_until_due,

       ## Support/debug
       due,
       created_at,
       last_modified,
       status,
       priority
from todos
# order by days_until_due desc, days_old desc, days_since_last_modification desc
;

Select *
from TimeElapsed;


CREATE or replace VIEW Schedule AS
Select
     #     sum(coalesce(timelapsed.days_old, 0)  ,  coalesce(  days_until_due, 0)
#         sum(te.days_old, days_until_due) as days_to_reschedule
#     sum(1,2) as test

        coalesce(te.days_old, 0)
        + coalesce(te.days_until_due, 0)
        + coalesce(te.days_since_last_modification, 0) as sum_total_days
     ## support/debug
     , days_old
     , days_since_last_modification
     , days_until_due
from TimeElapsed as te;

select *
from Schedule;

