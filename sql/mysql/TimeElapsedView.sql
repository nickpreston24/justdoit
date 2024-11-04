
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
