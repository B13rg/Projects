#include "cpu.h"
#include <bits/stdc++.h>

using namespace std;

void chip8::initialize(){
	pc = 0x200;
	opcode = 0;
	I = 0;
	sp = 0;	
	//Clear display
	for(int i=0; i<64*32;i++){
		gfx[i]=0;
	}
	//ClearStack
	sp=0;
	for(int i=0; i<16; i++){
		stack[i]=0;
	}
	//Clear Registers
	for(int i=0; i<16; i++){
		V[i]=0;
	}
	//Clear Memory
	for(int i=0; i<4096; i++){
		memory[i]=0;
	}
	//Load fontset
	for(int i=0; i<80; i++){
		memory[i]=chip8_fontset[i];
	}
	//Reset Timers
	delay_timer=0;
	sound_timer=0;
}

bool chip8::loadApplication(string filename){
	ifstream fl(filename);
	fl.seekg( 0, ios::end );
	size_t len = fl.tellg();
	char *buffer = new char[len];
	fl.seekg(0, ios::beg); 
	fl.read(buffer, len);
	fl.close();

	for(int i=0; i<len; i++){
		memory[i+512]=buffer[i];
	}
	return true;
}


void chip8::emulateCycle(){
	//Fetch Opcode
	opcode = memory[pc] << 8 | memory[pc+1];
	//Decode and Execute OpCode
	switch(opcode & 0xF000){
		case 0x0000:
			switch(opcode & 0x000F){
				case 0x0000:	//0x00E0 Clears the screen
					for(int i=0; i<64*32;i++){
						gfx[i]=0;
					}
					break;
				case 0x000E:	//0x00EE returns from subroutine
					pc=stack[sp];
					sp--;
					break;
				default:
					cout << "Unknown opcode [0x0000]: 0x" << opcode << endl;
			}
			break;
		case 0x1000:
			pc=opcode & 0xFFF;
			break;
		case 0x2000:
			stack[sp]=pc;
			sp++;
			pc=opcode & 0x0FFF;
			break;
		case 0x3000:
			if(V[(opcode & 0x0F00) >> 8] == opcode & 0x00FF){
				//skip next instruction
				pc += 4;
			}
			break;
		case 0x4000:
			if(V[(opcode & 0x0F00) >> 8] != opcode & 0x00FF){
				//skip next instruction
				pc += 4;
			}
			break;
		case 0x5000:
			if(V[(opcode & 0x0F00) >> 8] == V[(opcode & 0x00F0) >> 4]){
				//skip next instruction
				pc += 4;
			}
			break;
		case 0x6000:
			V[(opcode & 0x0F00) >> 8]=opcode & 0x00FF;
			pc += 2;
			break;
		case 0x7000:
			V[(opcode & 0x0F00) >> 8]+=opcode & 0x00FF;
			pc += 2;
			break;
		case 0x8000:
			{
			unsigned char x = (opcode & 0x0F00) >> 8;
			unsigned char y = (opcode & 0x00F0) >> 4;
			switch(opcode & 0x000F){
				case 0x0000:
					V[x]=V[y];
					pc += 2;
					break;
				case 0x0001:
					V[x] = V[x] | V[y];
					pc += 2;
					break;
				case 0x0002:
					V[x] = V[x] & V[y];
					pc += 2;
					break;
				case 0x0003:
					V[x] = V[x] ^ V[y];
					pc += 2;
					break;
				case 0x0004: 
					{
						unsigned char temp = V[x] + V[y];
						if(temp > 256){
							V[0xF]=1;
						} else{
							V[0xF]=0;
						}
						V[x]=temp & 0x00FF;
						pc += 2;
					}
					break;
				case 0x0005:
					if(V[x] > V[y]){
						V[0xF] = 1;
					} else{
						V[0xF]=0;
					}
					V[x] = V[x] - V[y];
					pc += 2;
					break;
				case 0x0006:
					if((V[x] & 0x000F) == 1){
						V[0xF] = 1;
					} else{
						V[0xF] = 0;
					}
					V[x] = V[x] /2;
					pc += 2;
					break;
				case 0x0007:
					if(V[y] > V[x]){
						V[0xF] = 1;
					} else{
						V[0xF] = 0;
					}
					V[x] = V[y] - V[x];
					pc += 2; 
					break;
				case 0x000E:
					if((V[x] & 0xF000) == 1){
						V[0xF] = 1;
					} else{
						V[0xF] = 0;
					}
					V[x] *= 2;
					pc += 2;
					break;
				default:
					cout << "Unknown opcode [0x0000]: 0x" << opcode << endl;
					pc += 2;
			}
			}
			break;
		case 0x9000:
			switch(opcode & 0x000F){
				case 0x0000:
					{
						unsigned char x = (opcode & 0x0F00) >> 8;
						unsigned char y = (opcode & 0x00F0) >> 4;
						if(x != y){
							pc += 4;
						}
					}
					break;
				default:
					cout << "Unknown opcode [0x0000]: 0x" << opcode << endl;
					pc += 2;

			}
		case 0xA000:	
			I=opcode & 0x0FFF;
			pc+=2;
			break;
		case 0xB000:
			pc=V[0]+(opcode & 0x0FFF);
			break;
		case 0xC000:
			V[(opcode & 0x0F00) >> 8] = rand() & (opcode & 0x00FF);
			break;
		case 0xD000:
			{
			unsigned short x = V[(opcode & 0x0F00) >> 8];
			unsigned short y = V[(opcode & 0x00F0) >> 4];
			unsigned short height = opcode & 0x000F;
			unsigned short pixel;
			
			V[0xF] = 0;
			for (int yline = 0; yline < height; yline++)
			{
				pixel = memory[I + yline];
				for(int xline = 0; xline < 8; xline++)
				{
					if((pixel & (0x80 >> xline)) != 0)
					{
					if(gfx[(x + xline + ((y + yline) * 64))] == 1)
						V[0xF] = 1;                                 
					gfx[x + xline + ((y + yline) * 64)] ^= 1;
					}
				}
			}			
			drawFlag = true;
			pc+=2;
			}
			break;
		case 0xE000:
			{
			unsigned char x = (opcode & 0x0F00) >> 8;
			if((opcode & 0x00FF) == 0x9E){
				if(key[V[x]]){
					pc+=4;
				}else{
					pc+=2;
				}
			}else if((opcode & 0x00FF) == 0xA1){
				if(!key[V[x]]){
					pc+=4;
				} else{
					pc+=2;
				}
			}else{
				cout << "Unknown opcode [0x0000]: 0x" << opcode << endl;
				pc += 2;
			}
			}
			break;
		case 0xF000:
			{
			unsigned char x = (opcode & 0x0F00) >> 8;
			switch(opcode & 0x00FF){
				case 0x000A:
					//TODO
					//wait for kepress
					//unsigned char pressedKey = 5;
					//V[x] = pressedKey;
					pc+=2;
					break;
				case 0x0015:
					delay_timer=V[x];
					pc+=2;
					break;
				case 0x0018:
					sound_timer=V[x];
					pc+=2;
					break;
				case 0x001E:
					I += V[x];
					pc+=2;
					break;
				case 0x0029:
					//Set I = location of sprite for digit Vx.
					I = memory[V[x]*5];
					pc+=2;
					break;
				case 0x0033:
					memory[I] = V[x]/100;
					memory[I+1] = V[x]/10%10;
					memory[I+2] = V[x]%100%10;
					pc+=2;
					break;
				case 0x0055:
					for(int i=0; i<x; i++){
						memory[I+i] = V[i];
						
					}
					pc+=2;
					break;
				case 0x0065:
					for(int i=0; i<x; i++){
						V[i] = memory[I+i];
					}
					pc+=2;
					break;
			}
			}
			break;
		default:
			cout << "Unknown opcode [0x0000]: 0x" << opcode << endl;
			pc += 2;
	}

	//Update Timers
	if(delay_timer > 0)
		--delay_timer;

	if(sound_timer > 0){
		if(sound_timer == 1)
			cout << "BEEP" << endl;
		--sound_timer;
	}
}