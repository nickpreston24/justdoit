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


