# drop table if exists todos;
create table if not exists todos
(
    id             INT  NOT NULL AUTO_INCREMENT,
    content        text,
    comments       text,
    description    text,
    status         varchar(10),
    priority       int,

    due            datetime,
    start          datetime,
    end            datetime,

    created_at     datetime      default now(),
    created_by     varchar(150)  default null,
    last_modified  datetime      default null,

    # flags
    is_sample_data bool not null default false, # stuff marked with 'zzz' will be treated as sample data.
    is_deleted     bool          default false, # hidden from all queries, but soft deleted.
    is_archived    bool          default false, # hidden from all queries with the intent of holding onto it indefinitely.
    is_enabled     bool          default true,  # when ticked, is still visible (depending on feature), but readonly.

    # primary keys: 
    PRIMARY KEY (id)
);
# 
# alter table todos
#     ADD COLUMN related_todos json not null default '{}';

alter table todos
    ADD COLUMN is_sample_data bool not null default false; # true if the row is fake or sample data, e.g. 'testzzz'

alter table todos
    add column is_recurring bool not null default false;

alter table todos
    ADD COLUMN comments text;

alter table todos
    ADD COLUMN description text;

alter table todos
    ADD COLUMN start datetime,
    add column end   datetime;

alter table todos
    add column is_deleted  bool default false,
    add column is_archived bool default false,
    add column is_enabled  bool default true;

ALTER TABLE todos
    ADD FULLTEXT (content, description);
#, comments, description, status, created_by, modified_by

# alter table todos
CREATE INDEX content
    ON todos (content(500), description);
# DROP INDEX content ON todos;


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

## nuke
### delete from todos where id > 0

drop procedure if exists updatetodo;
DELIMITER ^_^
CREATE PROCEDURE updatetodo(
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

call updatetodo();

select *
from todos
# where content like '%test%'
order by created_at desc
;

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
       priority,
       id
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
     , id
from TimeElapsed as te;

select *
from Schedule;


### Bumps only


CREATE or replace VIEW Bumps AS
select id, content, due
from todos
where content REGEXP '@bump:\s*'
   or comments REGEXP '@bump:\s*';

Select *
from Bumps;

### wip - trying to figure out a smart way to sort.
select te.days_until_due, todos.priority
from todos
         join TimeElapsed te on te.id
         join Schedule S on te.id = S.id
order by te.days_until_due;

## filter sample todos...
update todos
set is_sample_data = 1
where content like '%zzz%';

select due, content, id
from todos
where todos.is_sample_data = 1;

# select count(*)
# from todos
# order by due desc, priority desc

# textzzz Seeding:

INSERT INTO todos (content, due, priority, status, is_sample_data)
VALUES ('testxyzzz', now(), 3, 'Pending', true),
       ('testxyzzz', now(), 4, 'Unknown', true),
       ('testxyzzz', now(), 2, 'Pending', true),
       ('testxyzzz', now(), 1, 'WIP', true),
       ('testxyzzz', now(), 3, 'WIP', true),
       ('testxyzzz', now(), 4, 'Pending', true),
       ('testxyzzz', now(), 2, 'Done', true),
       ('testxyzzz', now(), 1, 'Pending', true),
       ('testxyzzz', now(), 3, 'Done', true),
       ('testxyzzz', now(), 4, 'Pending', true);



select id, content, status, todos.is_sample_data, todos.is_deleted, todos.is_archived, todos.is_enabled
from todos
where !is_sample_data
          && !is_deleted
          && !is_archived
          || is_enabled
;

# 
# update todos
# set status = 'done'
# #     && last_modified = @last_modified
# where id = -1


drop procedure if exists get_all_todos;
DELIMITER ^_^
CREATE PROCEDURE get_all_todos()
BEGIN
    select id, content, status, priority from todos;
end ^_^
DELIMITER ;

call get_all_todos();


select id, content, status, priority, is_sample_data
from todos
where
#         length(content) > 15 or
    content like 'test%'
   or todos.is_sample_data = 1;
#id  = 30


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

# sample task archiving:
update todos
set is_archived = 1
where id = -1;

## tests only
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