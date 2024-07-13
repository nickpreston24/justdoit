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

/*



 */