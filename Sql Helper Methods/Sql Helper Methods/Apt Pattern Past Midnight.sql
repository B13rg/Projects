select * from appointment


Select Pattern,AptDateTime, Length(Pattern) from appointment

Select AptDateTime, Pattern,5*LENGTH(Pattern) as patlen, DATE_ADD(AptDateTime,INTERVAL (5*LENGTH(Pattern)) MINUTE) as modetime from appointment having Date(modetime)>Date(AptDateTime)