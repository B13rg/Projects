#Set up query logging
set global log_output = 'FILE';
SET GLOBAL slow_query_log_file='E:\\MySQL\\MySQL Server 5.5\\Logs\\slow.log';
set global general_log_file='E:\\MySQL\\MySQL Server 5.5\\Logs\\general.log';

#Turn logging on
set global long_query_time = 1;
set global slow_query_log = 1;
SET GLOBAL log_slow_queries = 1;
set global log_queries_not_using_indexes = 1;
set global general_log = 1;

#Turn logging off
SET GLOBAL long_query_time = 0;
SET GLOBAL slow_query_log = 0;
SET GLOBAL log_slow_queries = 0;
SET GLOBAL log_queries_not_using_indexes = 0;
SET GLOBAL general_log = 0;