//include the library
#include "LedControlMS.h"

/*
Configuring the LEDMatrix:
Digital 2 is conneted to DIN (Data IN)
Digital 3 is connected to CLK (CLocK)
Digital 4 is connected to CS (LOAD)
5V is connected to VCC
GND is connected to GND
There is only one MAX7219 display module.
*/

#define NBR_MTX 1
LedControl lc=LedControl(2,3,4, NBR_MTX);
//LedControl lc=LedControl(2,3,4, NBR_MTX);
/* wait time between updates of the display */
unsigned long delaytime=100;

bool stateCur[8][8];
bool stateNew[8][8];

void createBlinker(int xcord,int ycord){
  stateCur[0+xcord][0+ycord]=true;
  stateCur[0+xcord][1+ycord]=true;
  stateCur[0+xcord][2+ycord]=true;
}

void createBeacon(int xcord,int ycord){
  /*
   * xx__
   * xx__
   * __xx
   * __xx
   */
  stateCur[2+xcord][2+ycord]=true;
  stateCur[2+xcord][3+ycord]=true;
  stateCur[3+xcord][2+ycord]=true;
  stateCur[3+xcord][3+ycord]=true;
  
  stateCur[0+xcord][0+ycord]=true;
  stateCur[0+xcord][1+ycord]=true;
  stateCur[1+xcord][0+ycord]=true;
  stateCur[1+xcord][1+ycord]=true;
}

void createToad(int xcord,int ycord){
  /*
   * __X_
   * X__X
   * X__X
   * _X__ 
   */
  stateCur[0+xcord][1+ycord]=true;
  stateCur[0+xcord][2+ycord]=true;
  stateCur[1+xcord][3+ycord]=true;
  stateCur[2+xcord][0+ycord]=true;
  stateCur[3+xcord][1+ycord]=true;
  stateCur[3+xcord][2+ycord]=true;
}

void createGlider(int xcord,int ycord){
  /*
   * _X_
   * __X
   * XXX
   */
  stateCur[0+xcord][1+ycord]=true;
  stateCur[1+xcord][3+ycord]=true;
  stateCur[2+xcord][0+ycord]=true;
  stateCur[2+xcord][1+ycord]=true;
  stateCur[2+xcord][2+ycord]=true;
}

void createRandom(int numItems){
  for(int i=0; i<numItems; i++){
    int ycord=random(0,8);
    int xcord=random(0,8);
    stateCur[xcord][ycord]=true;
  }
}

void createRandom(){
  for(int i=0; i<8; i++){
    for(int k=0; k<8; k++){
      stateCur[i][k]=false;
    }
  }
  switch(random(0,5)){
    case 0: createBlinker(4,4);break;
    case 1: createBeacon(4,4);break;
    case 2: createToad(4,4);break;
    case 3: createGlider(4,4);break;
    case 4: createRandom(8);break;
  }
}



void setup() { // initalizes and sets up the initial values. Declaring function setup.
  /* The display module is in power-saving mode on startup.
  Do a wakeup call */
  Serial.begin(9600); // setting data rate as 9600 bits per second for serial data communication to computer
  Serial.println("Setup"); //prints data to serial port as human-readable text
  randomSeed(analogRead(0));
  for (int i=0; i< NBR_MTX; i++){
    lc.shutdown(i,false); //keep the screen on
    lc.setIntensity(i,8); // set brightness to medium values
    lc.clearDisplay(i); //clear the display after each letter
  }
  //createRandom();
  createRandom();
}

int getNeighborCount(int xcord, int ycord){
  int leftSide = max(xcord-1,0);
  int rightSide = min(xcord+1,8);
  int topSide = max(ycord-1,0);
  int botSide = min(ycord+1,8);
  int neighbors=0;

  for(int i=leftSide; i<rightSide+1; i++){
    for(int k=topSide; k<botSide+1; k++){
      if(i==xcord || k==ycord){
        //continue;
      }
      if(stateCur[i][k]==true){
        neighbors++;
      }
    }
  }
  if(stateCur[xcord][ycord]==true){
    neighbors=max(neighbors-1,0);
  }
  return neighbors;
}


bool checkLife(int xcord, int ycord){
  int numNeighbors = getNeighborCount(xcord,ycord);
  bool curState=stateCur[xcord][ycord];
  if(curState){
    if(numNeighbors == 2 || numNeighbors == 3){
      return true;
    }
  }
  else{
    if(numNeighbors == 3){
      return true;
    }
  }
  return false;
}

void drawState(){
  for(int i=0; i<8; i++){
    for(int k=0; k<8; k++){
      lc.setLed(0,i,k,stateCur[i][k]);
    }
  }
}



void transitionState(){
  for(int i=0; i<8; i++){
    for(int k=0; k<8; k++){
      stateCur[i][k]=stateNew[i][k];
    }
  }
}

int drawCount=0;

void loop() { //declaring function loop
  drawState();
  //Turn each led on in turn
  for(int i=0; i<8; i++){
    for(int k=0; k<8; k++){
      bool lifeState=checkLife(i,k);
      stateNew[i][k]=lifeState;
    }
  }
  transitionState();
  if(drawCount>20){
    createRandom();
  }
  else{
    drawCount++;
  }  
  delay(200);
}
