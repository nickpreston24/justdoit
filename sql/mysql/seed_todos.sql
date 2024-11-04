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

# 
# 
# select *
# from todos
# # where content like '%test%'
# order by created_at desc
# ;

/*
delete from todos where content like '%zzz%'
 */


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


