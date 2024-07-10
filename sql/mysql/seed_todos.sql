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

/*
drop procedure if exists CreateSchedule;
DELIMITER ^_^
CREATE PROCEDURE CreateSchedule(
    days_from_now int
)
BEGIN

    #     set @start = date_add(now(), days_from_now);
#     select @start;

    CREATE TEMPORARY TABLE todo_schedule_temp
    SELECT *
    FROM todos
    order by due desc;

    # return
    select content, due from todo_schedule_temp;
    
    drop temporary table todo_schedule_temp;
END ^_^

DELIMITER ;


call CreateSchedule(8);
*/


INSERT INTO todos (id, content, due)
VALUES (1, 1, 1),
       (2, 2, 3),
       (3, 9, 3),
       (4, 10, 12)
ON DUPLICATE KEY UPDATE Col1=VALUES(Col1),
                        Col2=VALUES(Col2);