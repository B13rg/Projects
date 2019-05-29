from zxcvbn import zxcvbn
import csv

outfile = open('myspace_results.csv','w', newline='')

with open('./myspace_subset.txt') as f:
	outWriter = csv.writer(outfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
	outWriter.writerow(['password','score'])
	for line in f:
		result = zxcvbn(line.strip())
		item1 = result['password'].rstrip()
		item2 = str(result['guesses']).rstrip()
		outWriter.writerow([item1,item2])