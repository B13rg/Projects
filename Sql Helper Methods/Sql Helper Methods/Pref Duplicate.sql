SELECT t1.* FROM preference as t1 
LEFT JOIN preference as t2 ON t1.prefname=t2.prefname and t1.prefnum!=t2.prefnum 
WHERE t2.prefnum IS NOT NULL 
ORDER BY t1.prefname