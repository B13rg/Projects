UPDATE preference SET ValueString='2999-01-01 00:00:00' WHERE PrefName='BackupReminderLastDateRun';
UPDATE preference SET ValueString='C:\\OpenDentImages\\' WHERE PrefName='DocPath';
UPDATE preference SET ValueString='' WHERE PrefName='WebServiceServerName';
UPDATE userod SET PASSWORD='';
SELECT * FROM preference WHERE PrefName LIKE '%version%';