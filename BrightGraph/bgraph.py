import argparse
import cv2
import matplotlib.pyplot as plt

oldPointArray=[]
pointArray=[]

def GetPoint(event, x, y, flags, param):
	global pointArray
	global oldPointArray
	if event == cv2.EVENT_LBUTTONUP:
		pointArray.append((x,y))
		if len(pointArray)==2:
			cv2.line(image,pointArray[0],pointArray[1],(0,255,0),1)
			cv2.imshow("image", image)
			oldPointArray.append(pointArray)
			pointArray=[]

#all pixels are coming up black?
def getEquation(img, pts1, pts2):
	slope = (float)(pts2[1]-pts1[1])/(pts2[0]-pts1[0])
	intercept=(float)(pts1[1]-slope*pts1[0])
	pixelLums=[]
	indices=[]
	for xcord in range(pts1[0],pts2[0]):
		ycord=(int)(slope*xcord+intercept)
		pixel = img[xcord+2,ycord+2]
		luminance = 0.299*pixel[0] + 0.587*pixel[1] + 0.114*pixel[2]
		pixelLums.append(luminance)
		indices.append(xcord)
	plt.scatter(indices,pixelLums)
	plt.show()

	



# construct the argument parser and parse the arguments
""" ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", required=True, help="Path to the image")
args = vars(ap.parse_args()) """
 
# load the image, clone it, and setup the mouse callback function
image = cv2.imread("1.JPG")
clone = image.copy()
cv2.namedWindow("image")
cv2.setMouseCallback("image", GetPoint)
 
# display the image and wait for a keypress
cv2.imshow("image", image)
while True:
	key = cv2.waitKey(1) & 0xff
	if key == ord('c'):
		break
getEquation(clone,oldPointArray[0][0],oldPointArray[0][1])