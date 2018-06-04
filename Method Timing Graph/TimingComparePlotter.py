import numpy as np
import matplotlib.pyplot as plt
import csv

title="Performance Comparison"
name1="Sequential Method"
name2="Threaded Method"
xAxisName=""
yAxisName="Ticks to Finish"


# Create data
colors = ("red", "green")
groups = ("coffee", "tea") 

# Create plot
fig = plt.figure()
ax = fig.add_subplot(1, 1, 1)

x1=[]
y1=[]
x2=[]
y2=[]
with open('data.csv','r') as csvfile:
	plots = csv.reader(csvfile, delimiter=',')
	for row in plots:
		if int(row[1])<int(row[2]):
			x1.append(int(row[1]))
			y1.append(int(row[2]))
		else:
			x2.append(int(row[1]))
			y2.append(int(row[2]))

ax.scatter(x1, y1, alpha=0.8, color="orange")
ax.scatter(x2, y2, alpha=0.8, color="green")

ax.text(.2,.9,'Old Method was faster',horizontalalignment='center',verticalalignment='center',transform = ax.transAxes)
ax.text(.8,.1,'New Method was faster',horizontalalignment='center',verticalalignment='center',transform = ax.transAxes)

plt.xlim(0,300000)
plt.ylim(0,300000)
ax.plot(ax.get_xlim(), ax.get_ylim(), ls="--", c=".3")

plt.xlabel(xAxisName)
plt.ylabel(yAxisName)
plt.show()