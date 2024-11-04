drop procedure if exists get_archived_todos;

DELIMITER $$
CREATE PROCEDURE get_archived_todos(IN is_archived bool, search text)

BEGIN

    IF search IS NULL THEN

        select * from todos where todos.is_archived = false;
-- statements ;

    ELSE

        select * from todos where todos.is_archived = true;

-- statements ;

    END IF;

END$$

DELIMITER ;

call get_archived_todos(true, null);
# call get_archived_todos();
