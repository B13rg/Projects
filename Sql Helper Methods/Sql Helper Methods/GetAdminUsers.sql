SELECT
    Distinct userod.*
FROM
    usergroupattach
    INNER JOIN userod 
        ON (usergroupattach.UserNum = userod.UserNum)
    INNER JOIN grouppermission 
        ON (grouppermission.UserGroupNum = usergroupattach.UserGroupNum)
WHERE (grouppermission.PermType =24);