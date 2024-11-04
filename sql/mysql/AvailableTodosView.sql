
# get non-done tasks, and anything enabled but not deleted
CREATE or replace VIEW AvailableTodos AS
select id,
       content,
       status,
       priority,
       due,
       is_archived,
       is_deleted,
       is_enabled
from todos
where status <> 'done'
#    or status <> null
  and is_archived = 0
  and is_enabled = 1
  and is_deleted = 0;

select id, content, status, is_archived, is_deleted, is_enabled
from AvailableTodos;
