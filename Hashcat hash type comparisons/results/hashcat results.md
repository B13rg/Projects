# Homework 4

---

## Brendan Byers

## Problem 1

use unsaltedSHA256.txt

### Parameters

* -a	attack mode
* -m	hash type
* --potfile-disable	stops potfile from being saved
* --quiet	doesn't output cracked hashes to screen
* --runtime=600	limits runtime to 600 seconds (10 minutes)
* -O	uses optimized kernel code
* 
### Wordlist

.\hashcat64.exe -a 0 -m 1400 --potfile-disable --runtime=600 -O ..\unsaltedSHA256.txt ..\lists\rockyou.txt

### Wordlist with rules

.\hashcat64.exe -a 0 -m 1400 --potfile-disable --runtime=600 -O ..\unsaltedSHA256.txt ..\lists\rockyou.txt -r .\rules\rockyou-30000.rule

### Bruteforce
6 character:
.\hashcat64.exe -a 3 -m 1400 --potfile-disable --runtime=600 -O ..\unsaltedSHA256.txt ?a?a?a?a?a?a

7 character:
.\hashcat64.exe -a 3 -m 1400 --potfile-disable --runtime=1200 -O ..\unsaltedSHA256.txt ?a?a?a?a?a?a?a

8 character:
.\hashcat64.exe -a 3 -m 1400 --potfile-disable --runtime=1200 -O ..\unsaltedSHA256.txt ?a?a?a?a?a?a?a?a

### Combinator

.\hashcat64.exe -a 1 -m 1400 --potfile-disable --runtime=1200 ..\unsaltedSHA256.txt ..\lists\rockyou.txt ..\lists\xato-net-10-million-passwords.txt

## Results

### Wordlist

Session..........: hashcat
Status...........: Exhausted
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Thu May 16 18:45:41 2019 (7 secs)
Time.Estimated...: Thu May 16 18:45:48 2019 (0 secs; Runtime limited: 9 mins, 53 secs)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2213.6 kH/s (2.03ms) @ Accel:128 Loops:1 Thr:1024 Vec:1
Recovered........: 3985/10000 (39.85%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:N/A,N/A,N/A AVG:33997,2039869,48956874 (Min,Hour,Day)
Progress.........: 14344384/14344384 (100.00%)
Rejected.........: 3094/14344384 (0.02%)
Restore.Point....: 14344384/14344384 (100.00%)
Restore.Sub.#1...: Salt:0 Amplifier:0-1 Iteration:0-1
Candidates.#1....: $HEX[3430323231383532333334] -> $HEX[042a0337c2a156616d6f732103]
Hardware.Mon.#1..: Temp: 61c Fan:  0% Util: 10% Core:1860MHz Mem:3802MHz Bus:16

Started: Thu May 16 18:45:38 2019
Stopped: Thu May 16 18:45:49 2019

### Wordlist with rules

Session..........: hashcat
Status...........: Exhausted
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Thu May 16 18:46:27 2019 (4 mins, 2 secs)
Time.Estimated...: Thu May 16 18:50:29 2019 (0 secs; Runtime limited: 5 mins, 58 secs)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Mod........: Rules (.\rules\rockyou-30000.rule)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  1881.4 MH/s (5.29ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 8527/10000 (85.27%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:111,N/A,N/A AVG:2111,126660,3039853 (Min,Hour,Day)
Progress.........: 430331520000/430331520000 (100.00%)
Rejected.........: 92820000/430331520000 (0.02%)
Restore.Point....: 14344384/14344384 (100.00%)
Restore.Sub.#1...: Salt:0 Amplifier:29984-30000 Iteration:0-16
Candidates.#1....: $HEX[30383532393731373930383130] -> $HEX[042a0369c2a156616d6f7321033832]
Hardware.Mon.#1..: Temp: 74c Fan: 59% Util: 19% Core:1607MHz Mem:3802MHz Bus:16

Started: Thu May 16 18:46:24 2019
Stopped: Thu May 16 18:50:31 2019

### Bruteforce

#### 5 character

Approaching final keyspace - workload adjusted.


Session..........: hashcat
Status...........: Exhausted
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Thu May 16 19:01:51 2019 (5 mins, 7 secs)
Time.Estimated...: Thu May 16 19:06:58 2019 (0 secs; Runtime limited: 4 mins, 53 secs)
Guess.Mask.......: ?a?a?a?a?a?a [6]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2452.1 MH/s (3.15ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 1969/10000 (19.69%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:384,23066,553596 (Min,Hour,Day)
Progress.........: 735091890625/735091890625 (100.00%)
Rejected.........: 0/735091890625 (0.00%)
Restore.Point....: 81450625/81450625 (100.00%)
Restore.Sub.#1...: Salt:0 Amplifier:9024-9025 Iteration:0-16
Candidates.#1....:  ~nc.} ->  ~ ~}?
Hardware.Mon.#1..: Temp: 80c Fan: 67% Util: 95% Core:1771MHz Mem:3802MHz Bus:16

Started: Thu May 16 19:01:48 2019
Stopped: Thu May 16 19:06:59 2019

#### 7 Character

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Sun May 19 21:56:44 2019 (20 mins, 0 secs)
Time.Estimated...: Mon May 20 05:42:07 2019 (7 hours, 25 mins; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a [7]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2501.5 MH/s (7.59ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 958/10000 (9.58%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:47,2873,68953 (Min,Hour,Day)
Progress.........: 2985590128640/69833729609375 (4.28%)
Rejected.........: 0/2985590128640 (0.00%)
Restore.Point....: 2490368/81450625 (3.06%)
Restore.Sub.#1...: Salt:0 Amplifier:682960-682976 Iteration:0-16
Candidates.#1....: aG(N<]h -> eQ:.`+a
Hardware.Mon.#1..: Temp: 77c Fan: 63% Util: 98% Core:1784MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Sun May 19 21:56:40 2019
Stopped: Sun May 19 22:16:46 2019

#### 8 Character

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Sun May 19 22:46:12 2019 (20 mins, 0 secs)
Time.Estimated...: Wed Jun 19 15:39:25 2019 (30 days, 16 hours; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a?a [8]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2500.8 MH/s (7.94ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 248/10000 (2.48%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:12,744,17862 (Min,Hour,Day)
Progress.........: 3012565794816/6634204312890625 (0.05%)
Rejected.........: 0/3012565794816 (0.00%)
Restore.Point....: 2490368/7737809375 (0.03%)
Restore.Sub.#1...: Salt:0 Amplifier:704624-704640 Iteration:0-16
Candidates.#1....: jjZN<]ha -> vr\.`+an
Hardware.Mon.#1..: Temp: 77c Fan: 64% Util: 95% Core:1797MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Sun May 19 22:46:09 2019
Stopped: Sun May 19 23:06:13 2019

### Combinator

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: SHA2-256
Hash.Target......: ..\unsaltedSHA256.txt
Time.Started.....: Sun May 19 22:18:16 2019 (20 mins, 0 secs)
Time.Estimated...: Mon May 20 09:24:24 2019 (10 hours, 46 mins; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt), Left Side
Guess.Mod........: File (..\lists\xato-net-10-million-passwords.txt), Right Side
Speed.#1.........:  1863.6 MH/s (10.35ms) @ Accel:64 Loops:32 Thr:512 Vec:1
Recovered........: 4923/10000 (49.23%) Digests, 0/1 (0.00%) Salts
Recovered/Time...: CUR:28,N/A,N/A AVG:246,14775,354613 (Min,Hour,Day)
Progress.........: 2190069465088/74439520926336 (2.94%)
Rejected.........: 0/2190069465088 (0.00%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:0 Amplifier:3517664-3517696 Iteration:0-32
Candidates.#1....: 123456a150904 -> magkawasa15050824
Hardware.Mon.#1..: Temp: 77c Fan: 63% Util: 98% Core:1809MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Sun May 19 22:18:13 2019
Stopped: Sun May 19 22:38:17 2019

---

## Problem 2

### Wordlist

.\hashcat64.exe -a 0 -m 1420 --potfile-disable --runtime=1200 -O ..\saltedSHA256.txt ..\lists\rockyou.txt

### Wordlist with rules

.\hashcat64.exe -a 0 -m 1420 --potfile-disable --runtime=1200 -O ..\saltedSHA256.txt ..\lists\rockyou.txt -r .\rules\rockyou-30000.rule

### Bruteforce
6 character:
.\hashcat64.exe -a 3 -m 1420 --potfile-disable --runtime=1200 -O ..\saltedSHA256.txt ?a?a?a?a?a?a

7 character:
.\hashcat64.exe -a 3 -m 1420 --potfile-disable --runtime=1200 -O ..\saltedSHA256.txt ?a?a?a?a?a?a?a

8 character:
.\hashcat64.exe -a 3 -m 1420 --potfile-disable --runtime=1200 -O ..\saltedSHA256.txt ?a?a?a?a?a?a?a?a

### Combinator

.\hashcat64.exe -a 1 -m 1400 --potfile-disable --runtime=1200 ..\saltedSHA256.txt ..\lists\rockyou.txt ..\lists\xato-net-10-million-passwords.txt

### Results

### Wordlist

Session..........: hashcat
Status...........: Exhausted
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Thu May 16 20:21:52 2019 (1 min, 33 secs)
Time.Estimated...: Thu May 16 20:23:25 2019 (0 secs; Runtime limited: 8 mins, 27 secs)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  1114.0 MH/s (1.02ms) @ Accel:128 Loops:1 Thr:896 Vec:1
Recovered........: 3987/10000 (39.87%) Digests, 3987/10000 (39.87%) Salts
Recovered/Time...: CUR:745,N/A,N/A AVG:2555,153317,3679615 (Min,Hour,Day)
Progress.........: 143443840000/143443840000 (100.00%)
Rejected.........: 30940000/143443840000 (0.02%)
Restore.Point....: 14344384/14344384 (100.00%)
Restore.Sub.#1...: Salt:9999 Amplifier:0-1 Iteration:0-1
Candidates.#1....: $HEX[3139383430373232] -> $HEX[042a0337c2a156616d6f732103]
Hardware.Mon.#1..: Temp: 68c Fan: 45% Util: 60% Core:1847MHz Mem:3802MHz Bus:16

Started: Thu May 16 20:21:46 2019
Stopped: Thu May 16 20:23:27 2019

### Wordlist with rules

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Thu May 16 20:27:44 2019 (20 mins, 0 secs)
Time.Estimated...: Wed Jun 12 12:17:54 2019 (26 days, 15 hours; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Mod........: Rules (.\rules\rockyou-30000.rule)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  1840.9 MH/s (9.37ms) @ Accel:64 Loops:16 Thr:896 Vec:1
Recovered........: 147/10000 (1.47%) Digests, 147/10000 (1.47%) Salts
Recovered/Time...: CUR:3,N/A,N/A AVG:7,441,10588 (Min,Hour,Day)
Progress.........: 6707132462848/4303315200000000 (0.16%)
Rejected.........: 8700000000/6707132462848 (0.13%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:204 Amplifier:27968-27984 Iteration:0-16
Candidates.#1....: 123y56143 -> 3152538301
Hardware.Mon.#1..: Temp: 80c Fan: 69% Util: 99% Core:1797MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 20:27:41 2019
Stopped: Thu May 16 20:47:45 2019

### Bruteforce
6 character:

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Thu May 16 20:57:38 2019 (20 mins, 0 secs)
Time.Estimated...: Sat Jun 22 15:43:28 2019 (36 days, 18 hours; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a [6]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2310.3 MH/s (8.46ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 12/10000 (0.12%) Digests, 12/10000 (0.12%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,35,863 (Min,Hour,Day)
Progress.........: 2894346780672/7350918906250000 (0.04%)
Rejected.........: 0/2894346780672 (0.00%)
Restore.Point....: 0/81450625 (0.00%)
Restore.Sub.#1...: Salt:257 Amplifier:5008-5024 Iteration:0-16
Candidates.#1....: Xdnier -> %Gu{*1
Hardware.Mon.#1..: Temp: 80c Fan: 70% Util: 98% Core:1784MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 20:57:35 2019
Stopped: Thu May 16 21:17:40 2019

7 character:

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Fri May 17 16:27:26 2019 (20 mins, 0 secs)
Time.Estimated...: Fri Nov 24 02:03:45 2028 (9 years, 191 days; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a [7]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2323.6 MH/s (8.42ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 2792457109504/698337296093750000 (0.00%)
Rejected.........: 0/2792457109504 (0.00%)
Restore.Point....: 0/81450625 (0.00%)
Restore.Sub.#1...: Salt:2 Amplifier:527856-527872 Iteration:0-16
Candidates.#1....: A7@eran -> Fd/0{/9
Hardware.Mon.#1..: Temp: 79c Fan: 67% Util: 98% Core:1784MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Fri May 17 16:27:22 2019
Stopped: Fri May 17 16:47:27 2019

8 character:

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Fri May 17 16:49:02 2019 (20 mins, 0 secs)
Time.Estimated...: Tue Jun 05 15:06:17 2170 (151 years, 18 days; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a?a [8]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:  2308.0 MH/s (8.62ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 2772593934336/11001810907777595152 (0.00%)
Rejected.........: 0/2772593934336 (0.00%)
Restore.Point....: 0/7737809375 (0.00%)
Restore.Sub.#1...: Salt:2 Amplifier:511904-511920 Iteration:0-16
Candidates.#1....: T*Eerane -> #~`0{/99
Hardware.Mon.#1..: Temp: 80c Fan: 69% Util: 98% Core:1784MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Fri May 17 16:48:59 2019
Stopped: Fri May 17 17:09:04 2019

### Combinator

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: sha256($salt.$pass)
Hash.Target......: ..\saltedSHA256.txt
Time.Started.....: Thu May 16 21:18:10 2019 (20 mins, 0 secs)
Time.Estimated...: Wed Nov 20 16:26:41 2030 (11 years, 187 days; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt), Left Side
Guess.Mod........: File (..\lists\xato-net-10-million-passwords.txt), Right Side
Speed.#1.........:  2047.9 MH/s (9.50ms) @ Accel:64 Loops:16 Thr:1024 Vec:1
Recovered........: 3/10000 (0.03%) Digests, 3/10000 (0.03%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,8,215 (Min,Hour,Day)
Progress.........: 23882576673824/744395209263360000 (0.00%)
Rejected.........: 2023887060000/23882576673824 (8.47%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:3 Amplifier:1986224-1986240 Iteration:0-16
Candidates.#1....: 123456maria1129 -> swellchillin08maria061090
Hardware.Mon.#1..: Temp: 81c Fan: 72% Util: 98% Core:1784MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 21:18:06 2019
Stopped: Thu May 16 21:38:12 2019

---

## Problem 3

### Wordlist

.\hashcat64.exe -a 0 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ..\lists\rockyou.txt

### Wordlist with rules

.\hashcat64.exe -a 0 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ..\lists\rockyou.txt -r .\rules\rockyou-30000.rule

### Bruteforce
6 character:
.\hashcat64.exe -a 3 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ?a?a?a?a?a?a

7 character:
.\hashcat64.exe -a 3 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ?a?a?a?a?a?a?a

8 character:
.\hashcat64.exe -a 3 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ?a?a?a?a?a?a?a?a

### Combinator

.\hashcat64.exe -a 1 -m 8900 --potfile-disable --runtime=1200 -O ..\scrypt.txt ..\lists\rockyou.txt ..\lists\xato-net-10-million-passwords.txt

### Results

### Wordlist

Runtime limit reached, aborting...

Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Thu May 16 21:39:42 2019 (20 mins, 0 secs)
Time.Estimated...: Sun Dec 15 02:24:12 2019 (212 days, 5 hours; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:     7804 H/s (622.19ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 23/10000 (0.23%) Digests, 23/10000 (0.23%) Salts
Recovered/Time...: CUR:2,N/A,N/A AVG:1,68,1655 (Min,Hour,Day)
Progress.........: 9363200/143443840000 (0.01%)
Rejected.........: 0/9363200 (0.00%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:1925 Amplifier:0-1 Iteration:0-1
Candidates.#1....: 123456 -> daryl
Hardware.Mon.#1..: Temp: 67c Fan: 43% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 21:39:33 2019
Stopped: Thu May 16 21:59:43 2019

### Wordlist with rules

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Thu May 16 23:17:06 2019 (20 mins, 1 sec)
Time.Estimated...: Next Big Bang (> 10 years; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt)
Guess.Mod........: Rules (.\rules\rockyou-30000.rule)
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:     7809 H/s (621.92ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 9372928/4303315200000000 (0.00%)
Rejected.........: 0/9372928 (0.00%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:0 Amplifier:1927-1928 Iteration:0-1
Candidates.#1....: 1234577 -> daryl
Hardware.Mon.#1..: Temp: 67c Fan: 41% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 23:16:56 2019
Stopped: Thu May 16 23:37:08 2019

### Bruteforce
6 character:

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Fri May 17 08:53:33 2019 (20 mins, 0 secs)
Time.Estimated...: Next Big Bang (> 10 years; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a [6]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:     7835 H/s (619.63ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 9397248/7350918906250000 (0.00%)
Rejected.........: 0/9397248 (0.00%)
Restore.Point....: 0/7737809375 (0.00%)
Restore.Sub.#1...: Salt:20 Amplifier:32-33 Iteration:0-1
Candidates.#1....: Marier -> M6LLER
Hardware.Mon.#1..: Temp: 65c Fan: 39% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Fri May 17 08:53:24 2019
Stopped: Fri May 17 09:13:35 2019

7 character:

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Fri May 17 09:15:21 2019 (20 mins, 0 secs)
Time.Estimated...: Next Big Bang (> 10 years; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a [7]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:     7815 H/s (621.15ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 9372928/698337296093750000 (0.00%)
Rejected.........: 0/9372928 (0.00%)
Restore.Point....: 0/735091890625 (0.00%)
Restore.Sub.#1...: Salt:20 Amplifier:27-28 Iteration:0-1
Candidates.#1....: oarieri -> o6LLERI
Hardware.Mon.#1..: Temp: 66c Fan: 41% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Fri May 17 09:15:12 2019
Stopped: Fri May 17 09:35:23 2019

8 character:

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Fri May 17 17:10:05 2019 (20 mins, 0 secs)
Time.Estimated...: Next Big Bang (> 10 years; Runtime limit exceeded)
Guess.Mask.......: ?a?a?a?a?a?a?a?a [8]
Guess.Queue......: 1/1 (100.00%)
Speed.#1.........:     7673 H/s (634.20ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 9202688/11001810907777595152 (0.00%)
Rejected.........: 0/9202688 (0.00%)
Restore.Point....: 0/69833729609375 (0.00%)
Restore.Sub.#1...: Salt:19 Amplifier:87-88 Iteration:0-1
Candidates.#1....: \arierin -> \6LLERIN
Hardware.Mon.#1..: Temp: 66c Fan: 42% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Fri May 17 17:09:56 2019
Stopped: Fri May 17 17:30:07 2019

### Combinator

Runtime limit reached, aborting...
Session..........: hashcat
Status...........: Aborted (Runtime)
Hash.Type........: scrypt
Hash.Target......: ..\scrypt.txt
Time.Started.....: Thu May 16 23:48:20 2019 (20 mins, 1 sec)
Time.Estimated...: Next Big Bang (> 10 years; Runtime limit exceeded)
Guess.Base.......: File (..\lists\rockyou.txt), Left Side
Guess.Mod........: File (..\lists\xato-net-10-million-passwords.txt), Right Side
Speed.#1.........:     7811 H/s (619.06ms) @ Accel:16 Loops:1 Thr:16 Vec:1
Recovered........: 0/10000 (0.00%) Digests, 0/10000 (0.00%) Salts
Recovered/Time...: CUR:0,N/A,N/A AVG:0,0,0 (Min,Hour,Day)
Progress.........: 9368064/744395209263360000 (0.00%)
Rejected.........: 0/9368064 (0.00%)
Restore.Point....: 0/14344384 (0.00%)
Restore.Sub.#1...: Salt:0 Amplifier:1926-1927 Iteration:0-1
Candidates.#1....: 123456charlott -> darylcharlott
Hardware.Mon.#1..: Temp: 67c Fan: 42% Util:100% Core:1847MHz Mem:3802MHz Bus:16

Runtime limit reached, aborting...
Started: Thu May 16 23:48:11 2019
Stopped: Fri May 17 00:08:22 2019


## Problem 4

Command:
`.\hashcat64.exe -a 0 -m 99999 --potfile-disable --runtime=600 -O --outfile-format 10  ..\myspace_subset.txt ..\lists\rockyou.txt -r .\rules\rockyou-30000.rule`


