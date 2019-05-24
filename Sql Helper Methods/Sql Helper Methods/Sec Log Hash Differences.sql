Select securitylognum,permtype,logtext,logdatetime 
from securitylog 
where securitylognum in 
	(SELECT securitylognum 
	FROM securitylog t1 
	WHERE NOT EXISTS 
		(SELECT t2.securitylognum 
		FROM securityloghash t2 
		WHERE t2.SecurityLogNum = t1.SecurityLogNum)) 
order by securitylognum desc