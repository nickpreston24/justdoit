
### Bumps only


CREATE or replace VIEW Bumps AS
select id, content, due
from todos
where content REGEXP '@bump:\s*'
   or comments REGEXP '@bump:\s*';

Select *
from Bumps;
