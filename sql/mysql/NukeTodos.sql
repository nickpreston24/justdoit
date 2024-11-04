## nuke ALL Todos.  CANNOT BE UNDONE!!!

drop procedure if exists nuke_todos;
DELIMITER ^_^
CREATE PROCEDURE nuke_todos(
#     days_from_now int
)
BEGIN
    delete from todos where id > 0;
end ^_^