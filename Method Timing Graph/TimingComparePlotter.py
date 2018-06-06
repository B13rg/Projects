import numpy as np
import matplotlib.pyplot as plt
import csv

title=""
name1="Old Method"
name2="New Method"
xAxisName="Time for old Method (Ticks)"
yAxisName="Time for new method (Ticks)"
filenames=["data.csv"]

# Create data
colors = ("green", "orange") 

# Create plot
fig = plt.figure()
ax = fig.add_subplot(1, 1, 1)

x1=[]
y1=[]
x2=[]
y2=[]
for fileID in filenames:
	with open(fileID,'r') as csvfile:
		plots = csv.reader(csvfile, delimiter=',')
		for row in plots:
			if int(row[1])<int(row[2]):
				x1.append(int(row[1]))
				y1.append(int(row[2]))
			else:
				x2.append(int(row[1]))
				y2.append(int(row[2]))

ax.scatter(x2, y2, alpha=0.8, color=colors[0])
ax.scatter(x1, y1, alpha=0.8, color=colors[1])


ax.text(.2,.9,'Old Method was faster',horizontalalignment='center',verticalalignment='center',transform = ax.transAxes)
ax.text(.8,.1,'New Method was faster',horizontalalignment='center',verticalalignment='center',transform = ax.transAxes)
plt.title(title)
maxmax=max(max(max(x1),max(x2)),max(max(y1),max(y2)))

plt.xlim(0,maxmax)
plt.ylim(0,maxmax)
ax.plot(ax.get_xlim(), ax.get_ylim(), ls="--", c=".3")

plt.xlabel(xAxisName)
plt.ylabel(yAxisName)
plt.tight_layout()
plt.show()