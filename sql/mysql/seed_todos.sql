drop table if exists todos;
create table if not exists todos
(
    id            INT NOT NULL AUTO_INCREMENT,
    content       text,
    status        int,
    priority      int,

    due           datetime,
    created_at    datetime     default now(),
    created_by    varchar(150) default null,
    last_modified datetime     default null,

    # PK's
    PRIMARY KEY (id)
);

select content, created_at, due, status, priority
from todos
order by created_at;

select count(id)
from todos;


## nuke
### delete from todos where id > 0


