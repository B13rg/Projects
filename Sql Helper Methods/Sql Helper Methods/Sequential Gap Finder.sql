SELECT (t1.id + 1) as gap_starts_at, 
       (SELECT MIN(t3.id) -1 FROM arrc_vouchers t3 WHERE t3.id > t1.id) as gap_ends_at
FROM arrc_vouchers t1
WHERE NOT EXISTS (SELECT t2.id FROM arrc_vouchers t2 WHERE t2.id = t1.id + 1)
HAVING gap_ends_at IS NOT NULL