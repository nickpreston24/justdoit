drop procedure if exists search_todos;
DELIMITER ^_^
CREATE PROCEDURE search_todos(
    search_term varchar(50)
)
BEGIN

    #     #     compute temp vars first
#     set @search_term = IF(TRIM(COALESCE(search_term, '')) = '', '%', search_term);
# 
#     -- https://regex101.com/r/YYkqhs/1
#     #     set @regex_symbols = '[[\{\}\.\s\d\w]\+?\*?]+';
#     #     set @regex_symbols = '(\\[sdw]|[.])[+*?]*(\{\d*,?\d*\})?';
#     set @regex_symbols = '^(\\[sdw]|[.])[+*?]*$';
#     set @swapped = REGEXP_REPLACE(concat(@search_term, ''), @regex_symbols, '');
# 
#     #     set @symbols_found = regexp_substr(search_term, @regex_symbols);
#     set @regex_symbol_count = abs(length(@search_term) - length(@swapped));
#     set @is_regex_search = @regex_symbol_count > 0;
# 
#     -- debug
#     #     select -- @symbols_found,
#     #            @regex_symbol_count,
#     #            @search_term,
#     #            @swapped,
#     #            @is_regex_search;
# 
#     # Aggregate anything you wish to search with wildcards (%).
#     set @all_text = concat(
#             @search_term
#         );
# 
#     # wildcards can come from user inputs or COALESCES
#     set @wildcard_count = abs(LENGTH(@all_text) - LENGTH(REPLACE(@all_text, '%', '')));
# 
#     #  full text search if search term is empty
#     set @use_full_text_search = IF(TRIM(COALESCE(@search_term, '')) = '', 0, 1);
#     # Toggle exact match search
#     -- if full-text search is on, we don't want to match exact.
#     set @match_exact =
#             case
#                 when
#                     (@wildcard_count = 0 AND @use_full_text_search = 0 and @is_regex_search = 0) then 1
#                 else
#                     @wildcard_count = 0
#                 end;
    #debug
    #     set @debug_mode = 1;
#     select @search_term
#          , @all_text
#          , @is_regex_search
#          , @regex_symbol_count
#          , @swapped
#          , @wildcard_count
#          , @match_exact;
#     )

    SELECT id,
           content,
           description,
           status,
           priority
    FROM todos
    WHERE MATCH(content, description) AGAINST(
            search_term IN NATURAL LANGUAGE MODE
        );

END ^_^

DELIMITER ;

call search_todos('zzz');

# insights:
# show create procedure search_todos;
# todo: run this to cache along with embeds, validate if sproc exits, 
# ANNNNND check whether the DEFINITION is actually INSTALLED! :D

select *
from todos;
