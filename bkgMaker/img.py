from PIL import Image
import os
srcDir = "./images/"
outDir = "./output/"
outsize = (1920,1080)

for filename in os.listdir(srcDir):
	#Open the image
	srcImg = srcDir+filename
	orig = Image.open(srcImg)
	#Save some info about the image
	insize = orig.size
	#sample a color 10x10 pixels in
	backColor = orig.getpixel((10,10))
	#Resize
	result = orig.transform(outsize,Image.EXTENT,data=(0,0,100,100),fillcolor=backColor)
	#calculate placement
	leftEdge = int((outsize[0]-insize[0])/2)
	topEdge = int((outsize[1]-insize[1])/2)
	pastLoc = (leftEdge,topEdge,leftEdge+insize[0],topEdge+insize[1])
	#Paste original on new canvas
	result.paste(orig,pastLoc)

	#Save result
	outFile = outDir+str(outsize[0])+"x"+str(outsize[1])+filename
	result.save(outFile)