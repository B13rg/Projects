from os import listdir
from os.path import isfile, join
import xml.etree.ElementTree as ET
import re
import xml.dom.minidom
import random, sys, getopt

def cmp(a, b):
    return (a > b) - (a < b) 

def compareVersions(version1, version2):
    def normalize(v):
        return [int(x) for x in re.sub(r'(\.0+)*$','', v).split(".")]
    return cmp(normalize(version1), normalize(version2))

xmlDBLocation="./DatabaseXML/"
outFileName="DBDesign.xml"
try:
    opts, args = getopt.getopt(sys.argv,"do")
except getopt.GetoptError:
    print('dbHistory.py -d <inputDirectory> -o <outputFile>')
for opt, arg in opts:
    if opt=="-d":
        xmlDBLocation=arg
    elif opt=="-o":
        outFileName=arg

#Get list of XML files in directory
onlyfiles = [f for f in listdir(xmlDBLocation) if isfile(join(xmlDBLocation, f))]
#create a tree to save to
output=ET.Element("database")
#go through each of the files and merge their attributes
for f in onlyfiles:
    #db schemas are stored in this folder
    tree = ET.parse("./DatabaseXML/"+f)
    root = tree.getroot()
    curVersion=root.get('version')
    #go through each table element present
    for tbl in root.iter('table'):
        name=tbl.get('name')
        tblOut=None
        #Search for the correct name attribute
        for node in output.findall('./table'):
            if node.attrib['name']==name:
                tblOut=node
                break        
        if tblOut is None:   #if we can't find the table, add it
            tblOut=ET.SubElement(output,'table')
            tblOut.set('name', name)
            tblVer=ET.SubElement(tblOut,'version')
            tblVer.text=curVersion
        tblVer=tblOut.find('version')
        #go through each column in table
        for item in tbl.iter('column'):
            #get the name and element from the output element
            name=item.get('name')
            col=None
            #Search for the correct name attribute
            for node in tblOut.findall('./column'):
                if node.attrib['name']==name:
                    col=node
                    break         
            if col is None: #if we can't find the element, add it
                col=ET.SubElement(tblOut,'column')
                col.set('name', name)
                col.text=curVersion
            else:
                #This will record the earliest version the column was present
                if(compareVersions(col.text,curVersion)>0):
                    col.text=curVersion
                #this updates the version element for the table
                if(compareVersions(col.text,tblVer.text)<0):
                    tblVer.text=col.text

#Write out final tree to file.
output=ET.ElementTree(output)
output.write(outFileName)
