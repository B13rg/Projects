import csv
import numpy

oldTiming=[]
newTiming=[]
filenames=['data1000.csv','data2000.csv','data3000.csv']
for name in filenames:
    with open(name,'r') as csvfile:
        plots = csv.reader(csvfile, delimiter=',')
        for row in plots:
            oldTiming.append(int(row[1]))
            newTiming.append(int(row[2]))

oldMean = numpy.mean(oldTiming, axis=0)
oldStdDeviation = numpy.std(oldTiming, axis=0)
newMean = numpy.mean(newTiming, axis=0)
newStdDeviation = numpy.std(newTiming, axis=0)

print("-----Timings comparison info-----")
print("\t\tOld\t\tNew\t\tPercent Faster")
print("Average:\t"+str(oldMean)+"\t"+str(newMean)+"\t"+str((oldMean-newMean)/oldMean*100))
print("StdDev:\t\t"+str(oldStdDeviation)+"\t"+str(newStdDeviation))

fold = [x for x in oldTiming if (x > oldMean - 2 * oldStdDeviation)]
fold = [x for x in fold if (x < oldMean + 2 * oldStdDeviation)]
fnew = [x for x in newTiming if (x > newMean - 2 * newStdDeviation)]
fnew = [x for x in fnew if (x < newMean + 2 * newStdDeviation)]


oldMean=numpy.mean(fold,axis=0)
newMean=numpy.mean(fnew,axis=0)

print("Avg. in 3 std:\t"+str(oldMean)+"\t"+str(newMean)+"\t"+str((oldMean-newMean)/oldMean*100))