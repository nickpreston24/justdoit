
drop procedure if exists upserttodo;
DELIMITER ^_^
CREATE PROCEDURE upserttodo(
#     days_from_now int
)
BEGIN

    INSERT INTO todos (id, content, due, priority, status, is_sample_data)
    VALUES (1, 'testxyzzz', now(), 3, 'Pending', true),
           (2, 'testxyzzz', now(), 4, 'Pending', true),
           (3, 'testxyzzz', now(), 2, 'Pending', true),
           (8, 'testxyzzz', now(), 1, 'Pending', true)
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

call upserttodo();
