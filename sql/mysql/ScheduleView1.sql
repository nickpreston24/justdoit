/** SCHEDULE VIEW ATTEMPT1 */


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


### wip - trying to figure out a smart way to sort.
select te.days_until_due, todos.priority
from todos
         join TimeElapsed te on te.id
         join Schedule S on te.id = S.id
order by te.days_until_due;

