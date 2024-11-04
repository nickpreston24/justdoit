## filter sample todos...
update todos
set is_sample_data = 1
where content like '%zzz%';

select due, content, id
from todos
where todos.is_sample_data = 1;

## test zzz's only
select id,
       status,
       created_at,
       is_archived,
       content,
       is_enabled,
       is_deleted,
       is_sample_data
from todos
where content like '%zzz%'
order by created_at desc