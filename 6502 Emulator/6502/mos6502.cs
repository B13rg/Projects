using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _6502 {
	public partial class mos6502 {


		//Stack ranges from $0100-$01FF
		private byte[] stack=new byte[256];
		private void push(byte val) {
			ushort addr=(ushort)(0x0100+SP);
			SetByte(AddressingMode.Absolute,addr,val);
			SP--;
		}

		private void push(short val) {
			byte low=(byte)(val&0xFF);
			byte high=(byte)(val >> 8);
			push(low);
			push(high);
		}

		private byte pop() {
			SP++;
			ushort addr=(ushort)(0x0100+SP);
			byte val=ReadByte(AddressingMode.Absolute,addr);
			return val;
		}

		private byte[] memory=new byte[0xFFFF];

		public byte A;
		public byte X;
		public byte Y;
		public byte SP;
		public ushort PC;
		public byte Flags;

		public void Reset() {
			SP=0xFF;
			byte prt1=ReadByte(AddressingMode.Absolute,0xFFFC);
			byte prt2=ReadByte(AddressingMode.Absolute,0xFFFD);
			PC=BytesToShort(prt1,prt2);
		}

		public void Start() {
			Reset();
			Console.WriteLine("PC\tOpcode");
			while(true) {
				int wait=cycle();
				Thread.Sleep(wait*100);
			}
		}

		public void RunString(string program) {
			memory[0xFFFC]=0x00;
			memory[0xFFFD]=0x40;
			byte[] arrProg=Encoding.ASCII.GetBytes(program);
			//arrProg[0]=System.Convert.ToByte("0xA9", 16);
			for(int i=0;i<arrProg.Length; i++) {
				int addr=0x4000+i;
				memory[addr]=arrProg[i];
			}
			Start();
		}

		private int cycle() {
			byte instr=ReadByte(AddressingMode.Absolute,PC);
			Console.WriteLine(PC.ToString("X2")+"\t"+instr.ToString("X2"));
			PC++;			
			int numOperands=GetNumberOperands(instr);
			int time=0;
			switch(numOperands) {
				case 0:
					time=CallMethod(instr);
					break;
				case 1:
					byte op=ReadByte(AddressingMode.Absolute,PC);
					PC++;
					time=CallMethod(instr,op);
					break;
				case 2:
					byte op1=ReadByte(AddressingMode.Absolute,PC);
					PC++;
					byte op2=ReadByte(AddressingMode.Absolute,PC);
					PC++;
					time=CallMethod(instr,op1,op2);
					break;
			}
			return time;
		}

		private int GetNumberOperands(byte instruction) {
			byte row=(byte)(instruction >> 4);
			byte type=(byte)(instruction & 0xF);
			switch(type) {
				case 0x0:
					if(row==2) {
						return 2;
					}
					if(row>0x8) {
						return 1;
					}
					if(row%2==0) {
						return 0;
					}
					return 1;
				case 0x1:
				case 0x2: return 1;
				case 0x3: return 0;//no opcodes
				case 0x4:
				case 0x5:
				case 0x6: return 1;
				case 0x7: //no opcodes
				case 0x8: return 0;
				case 0x9:
					if(row%2==0) {
						return 1;
					}
					return 2;
				case 0xA:
				case 0xB: return 0; //no opcodes
				case 0xC:
				case 0xD:
				case 0xE: return 2;
				case 0xF: return 0;
			}
			return 0;
		}

		public enum Flag {
			///<summary>Carry Flag</summary>
			C=0,
			///<summary>Zero Flag</summary>
			Z=1,
			///<summary>Interrupt Disable Flag</summary>
			I=2,
			///<summary>Decimal Mode Flag</summary>
			D=3,	//Bit 3: Decimal mode (exists for compatibility, does not function on the Famicom/NES's 2A03/2A07)
			///<summary>Break Command Flag</summary> 
			B=4,	//Bit 4: Clear if interrupt vectoring, set if BRK or PHP
			///<summary>Always Set Flag</summary>
			A=5,
			///<summary>Overflow Flag</summary>
			V=6,
			///<summary>Negative Flag</summary>
			N=7
		}

		public enum AddressingMode {
			//Non-Indexed, Non-Memory
			///<summary>Many instructions can operate on the accumulator, e.g. LSR A. Some assemblers will treat no operand as an implicit A where applicable.</summary>
			Accumulator,
			///<summary>Uses the 8-bit operand itself as the value for the operation, rather than fetching a value from a memory address.</summary>
			Immediate,
			///<summary>Instructions like RTS or CLC have no address operand, the destination of results are implied.</summary>
			Implied,
			//Non-Indexed Memory ops
			///<summary>Branch instructions (e.g. BEQ, BCS) have a relative addressing mode that specifies an 8-bit signed offset relative to the current PC.</summary>
			Relative,
			///<summary>Fetches the value from a 16-bit address anywhere in memory.</summary>
			Absolute,
			///<summary>Fetches the value from an 8-bit address on the zero page.</summary>
			ZeroPage,
			///<summary>The JMP instruction has a special indirect addressing mode that can jump to the address stored in a 16-bit pointer anywhere in memory.</summary>
			Indirect,
			//Indexed Memory ops
			///<summary>val = PEEK(arg + (X|Y))	4+ Cycles</summary>
			AbsoluteIndexed,
			///<summary>val = PEEK((arg + (X|Y)) % 256)	4 Cycles</summary>
			ZeroPageIndexed,
			///<summary>PEEK(PEEK((arg + X) % 256) + PEEK((arg + X + 1) % 256) * 256)	6 cycles</summary>
			IndexedIndirectX,
			///<summary>PEEK(PEEK(arg) + PEEK((arg + 1) % 256) * 256 + Y)	5+ Cycles</summary>
			IndirectIndexedY
		}

		public byte ReadByte(AddressingMode mode,byte address,byte registerVal=0) {
			switch(mode) {
				case AddressingMode.Immediate: return address;
				case AddressingMode.ZeroPage: return (byte)(memory[address]);
				case AddressingMode.ZeroPageIndexed: return memory[(address+registerVal)%256];
				case AddressingMode.AbsoluteIndexed: return memory[address+registerVal];
				case AddressingMode.IndexedIndirectX: return memory[(memory[(address+this.X)%256]+memory[((address+this.X+1)%256)]*256)];
				case AddressingMode.IndirectIndexedY: return memory[(memory[address]+memory[((address+1)%256)]*256+this.Y)];
			}
			throw new NotImplementedException();
		}

		public byte ReadByte(AddressingMode mode, ushort address,byte registerVal=0) {
			ushort addr=(ushort)address;
			switch(mode) {
				case AddressingMode.ZeroPage: return (byte)(memory[addr]);
				case AddressingMode.ZeroPageIndexed: return memory[(addr+registerVal)%256];
				case AddressingMode.AbsoluteIndexed: return memory[addr+registerVal];
				case AddressingMode.IndexedIndirectX: return memory[(memory[(addr+this.X)%256]+memory[((addr+this.X+1)%256)]*256)];
				case AddressingMode.IndirectIndexedY: return memory[(memory[addr]+memory[((addr+1)%256)]*256+this.Y)];
				case AddressingMode.Absolute: return memory[addr];
			}
			throw new NotImplementedException();
		}

		public void SetByte(AddressingMode mode, byte addr, byte value, byte registerVal=0) {
			switch(mode) {
				case AddressingMode.Absolute: memory[addr]=value; break;
				case AddressingMode.ZeroPage: memory[addr]=value; break;
				case AddressingMode.ZeroPageIndexed: memory[(addr+registerVal)%256]=value; break;
				case AddressingMode.AbsoluteIndexed: memory[addr+registerVal]=value; break;
				case AddressingMode.IndexedIndirectX: memory[(memory[(addr+this.X)%256]+memory[((addr+this.X+1)%256)]*256)]=value; break;
				case AddressingMode.IndirectIndexedY: memory[(memory[addr]+memory[((addr+1)%256)]*256+this.Y)]=value; break;
				default: throw new NotImplementedException();
			}
		}

		public void SetByte(AddressingMode mode, ushort addr, byte value, byte registerVal=0) {
			switch(mode) {
				case AddressingMode.Absolute: memory[addr]=value; break;
				case AddressingMode.ZeroPage: memory[addr]=value; break;
				case AddressingMode.ZeroPageIndexed: memory[(addr+registerVal)%256]=value; break;
				case AddressingMode.AbsoluteIndexed: memory[addr+registerVal]=value; break;
				case AddressingMode.IndexedIndirectX: memory[(memory[(addr+this.X)%256]+memory[((addr+this.X+1)%256)]*256)]=value; break;
				case AddressingMode.IndirectIndexedY: memory[(memory[addr]+memory[((addr+1)%256)]*256+this.Y)]=value; break;
				default: throw new NotImplementedException();
			}
		}

		public short GetValueShort(AddressingMode mode, byte val) {
			switch(mode) {
				case AddressingMode.Relative: return memory[this.PC+val];
			}
			throw new NotImplementedException();
		}

		public ushort GetAddr(AddressingMode mode, ushort pointer) {
			switch(mode) {
				case AddressingMode.Absolute: return pointer;
				case AddressingMode.Indirect: return BitConverter.ToUInt16(new byte[] { memory[pointer],memory[pointer+1] },0);
			}
			throw new NotImplementedException();
		}

		public ushort BytesToShort(byte upper,byte lower) {
			return (ushort)BitConverter.ToInt16(new byte[] {upper,lower },0);
		}


		public void SetFlag(Flag f,bool val) {
			if(val) {
				Flags |= (byte)(1 << (int)f);
			}
			else {
				Flags &= (byte)~(1 << (int)f);
			}
		}

		public void SetFlagsForValue(int val,List<Flag> flags) {
			foreach(Flag f in flags) {
				switch(f) {
					case Flag.A: break;
					case Flag.C:
						if(val>255) {
							SetFlag(Flag.C,true);
						}
						else {
							SetFlag(Flag.C,false);
						}
						break;
					case Flag.N: SetFlag(Flag.N,CheckBit((byte)val,7)); break;
					case Flag.V:
						if(val<-128 || val>127) {
							SetFlag(Flag.V,true);
						}
						else {
							SetFlag(Flag.V,false);
						}
						break;
					case Flag.Z: SetFlag(Flag.Z,val==0); break;
					default:
						throw new NotImplementedException();
				}
			}			
		}

		public bool GetFlagBool(Flag f) {
         return (Flags & (1 << (int)f)) != 0; 
		}

		public byte GetFlagByte(Flag f) {
			if(GetFlagBool(f)) {
				return 1;
			}
			return 0;
		}

		public bool CheckBit(byte input,int index) {
			return (input & (1 << (int)index))!=0;
		}

		public byte SetBit(byte input,byte index, bool val) {
			if(val) {
				return (byte)(input & (1 << (int)index));
			}
			return (byte)(input & ~(1 << (int)index));
			
		}

		public bool LoadProgramIntoMemory(Byte[] prog, int offset=0x0000) {
			try {
				prog.CopyTo(memory,offset);
			}
			catch { return false; }
			return true;
		}

		public int CallMethod(byte OpCode, byte param1=0, byte param2=0) {
			switch(OpCode) {
				case 0x21: return AND_21(param1,param2);
				case 0x25: return AND_25(param1);
				case 0x29: return AND_29(param1);
				case 0x2D: return AND_2D(param1,param2);

				case 0x31: return AND_31(param1,param2);
				case 0x35: return AND_35(param1);
				case 0x39: return AND_39(param1,param2);
				case 0x3D: return AND_3D(param1,param2);

				case 0x61: return ADC_61(param1,param2);
				case 0x65: return ADC_65(param1);
				case 0x69: return ADC_69(param1);
				case 0x6D: return ADC_6D(param1,param2);

				case 0x71: return ADC_71(param1,param2);
				case 0x75: return ADC_75(param1);
				case 0x79: return ADC_79(param1,param2);
				case 0x7D: return ADC_7D(param1,param2);

				case 0x84: return STY_84(param1);
				case 0x86: return STX_86(param1);
				case 0x8C: return STY_8C(param1,param2);
				case 0x8E: return STX_8E(param1,param2);

				case 0x94: return STY_94(param1);
				case 0x96: return STX_96(param1);

				case 0xC0: return CPY_C0(param1);
				case 0xC1: return CMP_C1(param1);
				case 0xC4: return CPY_C4(param1);
				case 0xC5: return CMP_C5(param1);
				case 0xC6: return DEC_C6(param1);
				case 0xC9: return CMP_C9(param1);
				case 0xCC: return CPY_CC(param1,param2);
				case 0xCD: return CMP_CD(param1,param2);
				case 0xCE: return DEC_CE(param1,param2);

				case 0xD1: return CMP_D1(param1);
				case 0xD5: return CMP_D5(param1);
				case 0xD6: return DEC_D6(param1);
				case 0xD9: return CMP_D9(param1,param2);
				case 0xDD: return CMP_DD(param1,param2);
				case 0xDE: return DEC_DE(param1,param2);

				case 0xE0: return CPX_E0(param1);
				case 0xE4: return CPX_E4(param1);
				case 0xE6: return INC_E6(param1);
				case 0xEC: return CPX_EC(param1,param2);
				case 0xEE: return INC_EE(param1,param2);

				case 0xF6: return INC_F6(param1);
				case 0xFE: return INC_FE(param1,param2);


				case 0x9A: return TXS_9A();
				case 0xBA: return TSX_BA();
				case 0x48: return PHA_48();
				case 0x68: return PLA_68();
				case 0x08: return PHP_08();
				case 0x28: return PLP_28();
				case 0xAA: return TAX_AA();
				case 0x8A: return TXA_8A();
				case 0xCA: return DEX_CA();
				case 0xE8: return INX_E8();
				case 0xA8: return TAY_A8();
				case 0x98: return TYA_98();
				case 0x88: return DEY_88();
				case 0xC8: return INY_C8();
				case 0x4A: return LSR_4A(param1);
				case 0x46: return LSR_46(param1);
				case 0x56: return LSR_56(param1);
				case 0x4E: return LSR_4E(param1,param2);
				case 0x5E: return LSR_5E(param1,param2);
				case 0xA0: return LDY_A0(param1);
				case 0xA4: return LDY_A4(param1);
				case 0xB4: return LDY_B4(param1);
				case 0xAC: return LDY_AC(param1,param2);
				case 0xBC: return LDY_BC(param1,param2);
				case 0xA2: return LDX_A2(param1);
				case 0xA6: return LDX_A6(param1);
				case 0xB6: return LDX_B6(param1);
				case 0xAE: return LDX_AE(param1,param2);
				case 0xBE: return LDX_BE(param1,param2);
				case 0xA9: return LDA_A9(param1);
				case 0xA5: return LDA_A5(param1);
				case 0xB5: return LDA_B5(param1);
				case 0xAD: return LDA_AD(param1,param2);
				case 0xBD: return LDA_BD(param1,param2);
				case 0xB9: return LDA_B9(param1,param2);
				case 0xA1: return LDA_A1(param1);
				case 0xB1: return LDA_B1(param1);
				case 0x24: return BIT_24(param1);
				case 0x2C: return BIT_2C(param1,param2);
				case 0x0A: return ASL_0A(param1);
				case 0x06: return ASL_06(param1);
				case 0x16: return ASL_16(param1);
				case 0x0E: return ASL_0E(param1,param2);
				case 0x1E: return ASL_1E(param1,param2);
				case 0x4C: return JMP_4C(param1,param2);
				case 0x6C: return JMP_6C(param1,param2);
				case 0xEA: return NOP_EA();
				case 0x85: return STA_85(param1);
				case 0x95: return STA_95(param1);
				case 0x8D: return STA_8D(param1,param2);
				case 0x9D: return STA_9D(param1,param2);
				case 0x99: return STA_99(param1,param2);
				case 0x81: return STA_81(param1);
				case 0x91: return STA_91(param1);
				case 0x18: return CLC_18();
				case 0x38: return SEC_38();
				case 0x58: return CLI_58();
				case 0x78: return SEI_78();
				case 0xB8: return CLV_B8();
				case 0xD8: return CLD_D8();
				case 0xF8: return SED_F8();


				case 0x09: return ORA_09(param1);
				case 0x05: return ORA_05(param1);
				case 0x15: return ORA_15(param1);
				case 0x0D: return ORA_0D(param1,param2);
				case 0x1D: return ORA_1D(param1,param2);
				case 0x19: return ORA_19(param1,param2);
				case 0x01: return ORA_01(param1);
				case 0x11: return ORA_11(param1);

				case 0x49: return EOR_49(param1);
				case 0x45: return EOR_45(param1);
				case 0x55: return EOR_55(param1);
				case 0x4D: return EOR_4D(param1,param2);
				case 0x5D: return EOR_5D(param1,param2);
				case 0x59: return EOR_59(param1,param2);
				case 0x41: return EOR_41(param1);
				case 0x51: return EOR_51(param1);
				case 0x2A: return ROL_2A(param1);
				case 0x26: return ROL_26(param1);
				case 0x36: return ROL_36(param1);
				case 0x2E: return ROL_2E(param1,param2);
				case 0x3E: return ROL_3E(param1,param2);
				case 0x6A: return ROR_6A(param1);
				case 0x66: return ROR_66(param1);
				case 0x76: return ROR_76(param1);
				case 0x6E: return ROR_6E(param1,param2);
				case 0x7E: return ROR_7E(param1,param2);
				case 0x00: return BRK_00();

				case 0x10: return BPL_10(param1);
				case 0x30: return BMI_30(param1);
				case 0x50: return BVC_50(param1);
				case 0x70: return BVS_70(param1);
				case 0x90: return BCC_90(param1);
				case 0xB0: return BCS_B0(param1);
				case 0xD0: return BNE_D0(param1);
				case 0xF0: return BEQ_F0(param1);
				case 0x20: return JSR_20(param1,param2);
				case 0x60: return RTS_60();
				case 0x40: return RTI_40();
				case 0xE9: return SBC_E9(param1);
				case 0xE5: return SBC_E5(param1);
				case 0xF5: return SBC_F5(param1);
				case 0xED: return SBC_ED(param1,param2);
				case 0xFD: return SBC_FD(param1,param2);
				case 0xF9: return SBC_F9(param1,param2);
				case 0xE1: return SBC_E1(param1);
				case 0xF1: return SBC_F1(param1);
				default: return 0;	//assume nop for now
			}
		}
	}
}
