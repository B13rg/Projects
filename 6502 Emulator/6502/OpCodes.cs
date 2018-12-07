using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502 {
	public partial class mos6502 {
		//$00-ff = zero page
		//$100-1ff = stack
		//$FFFA-FFFB - NMI vector
		//$FFFC-FFFD - RESET vector
		//$FFFE-FFFF - IRQ/BRK vector

		//When the addition result is 0 to 255, the carry is cleared.
		//When the addition result is greater than 255, the carry is set.
		//When the subtraction result is 0 to 255, the carry is set.
		//When the subtraction result is less than 0, the carry is cleared.
		#region Add Carry
		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     ADC #$44      $69  2   2
		//Zero Page     ADC $44       $65  2   3
		//Zero Page,X   ADC $44,X     $75  2   4
		//Absolute      ADC $4400     $6D  3   4
		//Absolute,X    ADC $4400,X   $7D  3   4+
		//Absolute,Y    ADC $4400,Y   $79  3   4+
		//Indirect,X    ADC ($44,X)   $61  2   6
		//Indirect,Y    ADC ($44),Y   $71  2   5+

		//ADC results are dependant on the setting of the decimal flag. In decimal mode, addition is carried out on the assumption that the values 
		//involved are packed BCD (Binary Coded Decimal).
		//There is no way to add without carry.

		/// <summary>Add with carry</summary>
		/// <param name="input">Immediate</param>
		/// <remarks>Flags: SZ</remarks>
		private int ADC_69(byte input) {
			int val=(input+this.A+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 2;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input">Zero Page</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_65(byte input) {
			int val=(byte)(memory[input]+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 3;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input">Zero Page,X</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_75(byte input) {
			int val=(byte)(memory[input]+this.X+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 4;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input1">Absolute</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_6D(byte input1,byte input2) {
			ushort addr=BytesToShort(input1, input2);
			int val=(byte)(memory[addr]+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 4;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input1">Absolute,X</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_7D(byte input1,byte input2) {
			ushort addr=BytesToShort(input1, input2);
			int val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);//(byte)(memory[addr]+this.X+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 4;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input1">Absolute,Y</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_79(byte input1,byte input2) {
			ushort addr=BytesToShort(input1, input2);
			int val=ReadByte(AddressingMode.Absolute,addr);//(byte)(memory[addr]+this.Y+GetFlagByte(Flag.C));
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 4;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input1">Indirect,X</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_61(byte input1,byte input2) {
			ushort addr=BytesToShort(input1, input2);
			int val=ReadByte(AddressingMode.IndexedIndirectX,addr);//memory[(memory[(addr+this.X)%256]+memory[((addr+this.X+1)%256)]*256)];
			val+=GetFlagByte(Flag.C);
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});;
			return 6;
		}

		/// <summary>Add with carry</summary>
		/// <param name="input1">Indirect,Y</param>
		/// <remarks>Flags: SVZC</remarks>
		private int ADC_71(byte input1,byte input2) {
			ushort addr=BytesToShort(input1, input2);
			int val=ReadByte(AddressingMode.IndirectIndexedY,addr);// memory[(memory[addr]+memory[((addr+1)%256)]*256+this.Y)];
			val+=GetFlagByte(Flag.C);
			val+=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.C,Flag.V,Flag.Z,Flag.N});
			return 5;
		}
		#endregion Add Carry

		#region And
		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     AND #$44      $29  2   2
		//Zero Page     AND $44       $25  2   3
		//Zero Page,X   AND $44,X     $35  2   4
		//Absolute      AND $4400     $2D  3   4
		//Absolute,X    AND $4400,X   $3D  3   4+
		//Absolute,Y    AND $4400,Y   $39  3   4+
		//Indirect,X    AND ($44,X)   $21  2   6
		//Indirect,Y    AND ($44),Y   $31  2   5+

		/// <summary>And</summary>
		/// <param name="input">Immediate</param>
		/// <remarks>Flags: ZN</remarks>
		private int AND_29(byte input) {
			int val=(input+this.A+GetFlagByte(Flag.C));
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 2;
		}

		/// <summary>And</summary>
		/// <param name="input">Zero Page</param>
		/// <remarks>Flags: ZN</remarks>
		private int AND_25(byte input) {
			int val=ReadByte(AddressingMode.ZeroPage,input);
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 3;
		}

		/// <summary>And</summary>
		/// <param name="input">Zero Page Indexed</param>
		/// <remarks>Flags: ZN</remarks>
		private int AND_35(byte input) {
			int val=ReadByte(AddressingMode.ZeroPageIndexed,input,this.X);
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}

		/// <summary>And</summary>
		/// <param name="input">Absolute</param>
		/// <remarks>Flags: ZN</remarks>
		private int AND_2D(byte input1,byte input2) {
			short addr=(short)(input1+input2);
			int val=ReadByte(AddressingMode.Absolute,BytesToShort(input1,input2));
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}

		/// <summary>And</summary>
		/// <param name="input">Absolute X Indexed</param>
		/// <remarks>Flags: ZN</remarks>
		private int AND_3D(byte input1,byte input2) {
			short addr=(short)(input1+input2);
			int val=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.X);
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}

		/// <summary>And</summary>
		/// <param name="input1">Absolute,Y</param>
		/// <remarks>Flags: SVZC</remarks>
		private int AND_39(byte input1,byte input2) {
			short addr=(short)(input1+input2);
			int val=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.Y);
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}

		/// <summary>And</summary>
		/// <param name="input1">Indirect,X</param>
		/// <remarks>Flags: SVZC</remarks>
		private int AND_21(byte input1,byte input2) {
			short addr=(short)(input1+input2);
			int val=ReadByte(AddressingMode.IndexedIndirectX,BytesToShort(input1,input2));
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 6;
		}

		/// <summary>And</summary>
		/// <param name="input1">Indirect,Y</param>
		/// <remarks>Flags: SVZC</remarks>
		private int AND_31(byte input1,byte input2) {
			short addr=(short)(input1+input2);
			int val=ReadByte(AddressingMode.IndirectIndexedY,BytesToShort(input1,input2));
			val&=this.A;
			this.A=(byte)val;
			SetFlagsForValue(val,new List<Flag> { Flag.Z,Flag.N });
			return 5;
		}
		#endregion And

		#region Arithmetic Shift Left
		//MODE           SYNTAX       HEX LEN TIM
		//Accumulator   ASL A         $0A  1   2
		//Zero Page     ASL $44       $06  2   5
		//Zero Page,X   ASL $44,X     $16  2   6
		//Absolute      ASL $4400     $0E  3   6
		//Absolute,X    ASL $4400,X   $1E  3   7

		//ASL shifts all bits left one position. 0 is shifted into bit 0 and the original bit 7 is shifted into the Carry.

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Accumulator</param>
		/// <remarks>Flags: NZC</remarks>
		public int ASL_0A(byte input) {
			SetFlag(Flag.C,CheckBit(input,7));
			this.A=(byte)(input<<1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 2;
		}

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Zero Page</param>
		/// <remarks>Flags: NZC</remarks>
		public int ASL_06(byte input) {
			byte val=ReadByte(AddressingMode.ZeroPage,input);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 5;
		}

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Zero Page, X</param>
		/// <remarks>Flags: NZC</remarks>
		public int ASL_16(byte input) {
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input,this.X);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Absolute</param>
		/// <remarks>Flags: NZC</remarks>
		public int ASL_0E(byte input1,byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.ZeroPage,addr);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Absolute,X</param>
		/// <remarks>Flags: NZC</remarks>
		public int ASL_1E(byte input1,byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 7;
		}
		#endregion Arithmetic Shift Left

		#region Test Bits
		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     BIT $44       $24  2   3
		//Absolute      BIT $4400     $2C  3   4

		//BIT sets the Z flag as though the value in the address tested were ANDed with the accumulator. The S and V flags are set to match 
		//bits 7 and 6 respectively in the value stored at the tested address.

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Accumulator</param>
		/// <remarks>Flags: NZC</remarks>
		public int BIT_24(byte input) {
			byte val = ReadByte(AddressingMode.ZeroPage,input);
			val &=this.A;
			SetFlag(Flag.Z,val==0);
			SetFlag(Flag.V,CheckBit(val,6));
			SetFlag(Flag.V,CheckBit(val,7));
			return 3;
		}

		/// <summary>Arithmetic Shift Left</summary>
		/// <param name="input">Accumulator</param>
		/// <remarks>Flags: NZC</remarks>
		public int BIT_2C(byte input1, byte input2) {
			byte val=ReadByte(AddressingMode.Absolute,BytesToShort(input1,input2));
			val &=this.A;
			SetFlag(Flag.Z,val==0);
			SetFlag(Flag.V,CheckBit(val,6));
			SetFlag(Flag.V,CheckBit(val,7));
			return 4;
		}
		#endregion Test Bits

		#region Load Accumulator
		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     LDA #$44      $A9  2   2
		//Zero Page     LDA $44       $A5  2   3
		//Zero Page,X   LDA $44,X     $B5  2   4
		//Absolute      LDA $4400     $AD  3   4
		//Absolute,X    LDA $4400,X   $BD  3   4+
		//Absolute,Y    LDA $4400,Y   $B9  3   4+
		//Indirect,X    LDA ($44,X)   $A1  2   6
		//Indirect,Y    LDA ($44),Y   $B1  2   5+
		//Flags: S,Z

		public int LDA_A9(byte input1) {
			this.A=input1;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 2;
		}
		public int LDA_A5(byte input1) {
			this.A=ReadByte(AddressingMode.ZeroPage,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 3;
		}
		public int LDA_B5(byte input1) {
			this.A=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDA_AD(byte input1,byte input2) {
			this.A=ReadByte(AddressingMode.Absolute,BytesToShort(input1,input2));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDA_BD(byte input1, byte input2) {
			this.A=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDA_B9(byte input1, byte input2) {
			this.A=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.Y);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDA_A1(byte input1) {
			this.A=ReadByte(AddressingMode.IndexedIndirectX,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 6;
		}
		public int LDA_B1(byte input1) {
			this.A=ReadByte(AddressingMode.IndirectIndexedY,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 5;
		}
		#endregion

		#region Load X Register
		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     LDX #$44      $A2  2   2
		//Zero Page     LDX $44       $A6  2   3
		//Zero Page,Y   LDX $44,Y     $B6  2   4
		//Absolute      LDX $4400     $AE  3   4
		//Absolute,Y    LDX $4400,Y   $BE  3   4+
		//Flags: S,Z

		public int LDX_A2(byte input1) {
			this.X=input1;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 2;
		}
		public int LDX_A6(byte input1) {
			this.X=ReadByte(AddressingMode.ZeroPage,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 3;
		}
		public int LDX_B6(byte input1) {
			this.X=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.Y);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDX_AE(byte input1,byte input2) {
			this.X=ReadByte(AddressingMode.Absolute,BytesToShort(input1,input2));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDX_BE(byte input1, byte input2) {
			this.X=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.Y);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		#endregion

		#region Load Y Register
		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     LDY #$44      $A0  2   2
		//Zero Page     LDY $44       $A4  2   3
		//Zero Page,X   LDY $44,X     $B4  2   4
		//Absolute      LDY $4400     $AC  3   4
		//Absolute,X    LDY $4400,X   $BC  3   4+
		//Flags: S,Z

		public int LDY_A0(byte input1) {
			this.Y=input1;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 2;
		}
		public int LDY_A4(byte input1) {
			this.Y=ReadByte(AddressingMode.ZeroPage,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 3;
		}
		public int LDY_B4(byte input1) {
			this.Y=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDY_AC(byte input1,byte input2) {
			this.Y=ReadByte(AddressingMode.Absolute,BytesToShort(input1,input2));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		public int LDY_BC(byte input1, byte input2) {
			this.Y=ReadByte(AddressingMode.AbsoluteIndexed,BytesToShort(input1,input2),this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.N });
			return 4;
		}
		#endregion

		#region Logical Shift Right
		//MODE           SYNTAX       HEX LEN TIM
		//Accumulator   LSR A         $4A  1   2
		//Zero Page     LSR $44       $46  2   5
		//Zero Page,X   LSR $44,X     $56  2   6
		//Absolute      LSR $4400     $4E  3   6
		//Absolute,X    LSR $4400,X   $5E  3   7
		//Flag: SZC

		public int LSR_4A(byte input) {
			SetFlag(Flag.C,input%2==1);
			this.A=(byte)(input>>1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 2;
		}
		
		public int LSR_46(byte input) {
			byte val=ReadByte(AddressingMode.ZeroPage,input);
			SetFlag(Flag.C,val%2==1);
			this.A=(byte)(val>>1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 5;
		}
		
		public int LSR_56(byte input) {
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input,this.X);
			SetFlag(Flag.C,val%2==1);
			this.A=(byte)(val>>1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}
		
		public int LSR_4E(byte input1,byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.ZeroPage,addr);
			SetFlag(Flag.C,val%2==1);
			this.A=(byte)(val>>1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		public int LSR_5E(byte input1,byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlag(Flag.C,val%2==1);
			this.A=(byte)(val>>1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 7;
		}
		#endregion Logical Shift Right

		#region Register Instrucitons
		//Register Instructions
		//Affect Flags: S Z

		//These instructions are implied mode, have a length of one byte and require two machine cycles.
		//MNEMONIC                 HEX
		//TAX (Transfer A to X)    $AA
		//TXA (Transfer X to A)    $8A
		//DEX (DEcrement X)        $CA
		//INX (INcrement X)        $E8
		//TAY (Transfer A to Y)    $A8
		//TYA (Transfer Y to A)    $98
		//DEY (DEcrement Y)        $88
		//INY (INcrement Y)        $C8

		public int TAX_AA() {
			this.X=this.A;
			SetFlagsForValue(this.X,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int TXA_8A() {
			this.A=this.X;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int DEX_CA() {
			this.X--;
			SetFlagsForValue(this.X,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int INX_E8() {
			this.X++;
			SetFlagsForValue(this.X,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int TAY_A8() {
			this.Y=this.A;
			SetFlagsForValue(this.Y,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int TYA_98() {
			this.A=this.Y;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int DEY_88() {
			this.Y--;
			SetFlagsForValue(this.Y,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		public int INY_C8() {
			this.Y++;
			SetFlagsForValue(this.Y,new List<Flag> { Flag.Z, Flag.N });
			return 2;
		}

		#endregion Register Instructions

		#region Stack Instructions
		//These instructions are implied mode, have a length of one byte and require machine cycles as indicated. 
		//The "PuLl" operations are known as "POP" on most other microprocessors. 
		//With the 6502, the stack is always on page one ($100-$1FF) and works top down.

		//MNEMONIC                        HEX TIM
		//TXS (Transfer X to Stack ptr)   $9A  2
		//TSX (Transfer Stack ptr to X)   $BA  2
		//PHA (PusH Accumulator)          $48  3
		//PLA (PuLl Accumulator)          $68  4
		//PHP (PusH Processor status)     $08  3
		//PLP (PuLl Processor status)     $28  4
		public int TXS_9A() {
			SP=this.X;
			return 2;
		}

		public int TSX_BA() {
			this.X=SP;
			return 2;
		}

		public int PHA_48() {
			push(this.A);
			return 3;
		}

		public int PLA_68() {
			this.A=pop();
			SetFlagsForValue(this.A,new List<Flag>{ Flag.Z, Flag.N});
			return 4;
		}

		public int PHP_08() {
			BitArray bitArr=new BitArray(8);
			bitArr[0]=GetFlagBool(Flag.C);
			bitArr[1]=GetFlagBool(Flag.Z);
			bitArr[2]=GetFlagBool(Flag.I);
			bitArr[3]=GetFlagBool(Flag.D);
			bitArr[4]=GetFlagBool(Flag.B);
			bitArr[5]=true;
			bitArr[6]=GetFlagBool(Flag.V);
			bitArr[7]=GetFlagBool(Flag.N);
			byte[] bytes = new byte[1];
			bitArr.CopyTo(bytes, 0);
			push(bytes[0]);
			return 3;
		}

		public int PLP_28() {
			byte val=pop();
			SetFlagsForValue(val,new List<Flag> {Flag.C, Flag.Z, Flag.I, Flag.D, Flag.B, Flag.V, Flag.N });
			return 4;
		}
		#endregion Stack Instructions

		#region  STX (STore X register)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     STX $44       $86  2   3
		//Zero Page,Y   STX $44,Y     $96  2   4
		//Absolute      STX $4400     $8E  3   4
		public int STX_86(byte input1) {
			SetByte(AddressingMode.ZeroPage,input1,this.X);
			return 3;
		}

		public int STX_96(byte input1) {
			SetByte(AddressingMode.ZeroPageIndexed,input1,this.X,this.Y);
			return 4;
		}
		
		public int STX_8E(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			SetByte(AddressingMode.Absolute,addr,this.X);
			return 4;
		}
		#endregion STX (STore X register)

		#region  STY (STore Y register)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     STY $44       $84  2   3
		//Zero Page,X   STY $44,X     $94  2   4
		//Absolute      STY $4400     $8C  3   4
		public int STY_84(byte input1) {
			SetByte(AddressingMode.ZeroPage,input1,this.Y);
			return 3;
		}

		public int STY_94(byte input1) {
			SetByte(AddressingMode.ZeroPageIndexed,input1,this.Y,this.X);
			return 4;
		}
		
		public int STY_8C(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			SetByte(AddressingMode.Absolute,addr,this.Y);
			return 4;
		}
		#endregion STY (STore Y register)

		#region DECrement Memory
		//DEC (DECrement memory)
		//Affects Flags: S Z

		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     DEC $44       $C6  2   5
		//Zero Page,X   DEC $44,X     $D6  2   6
		//Absolute      DEC $4400     $CE  3   6
		//Absolute,X    DEC $4400,X   $DE  3   7
		public int DEC_C6(byte input1) {
			byte b=ReadByte(AddressingMode.ZeroPage,input1);
			b--;
			SetByte(AddressingMode.ZeroPage,input1,b);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 5;
		}

		public int DEC_D6(byte input1) {
			byte b=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			b--;
			SetByte(AddressingMode.ZeroPageIndexed,input1,b,this.X);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}

		public int DEC_CE(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte b=ReadByte(AddressingMode.Absolute,addr);
			b--;
			SetByte(AddressingMode.Absolute,addr,b);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}

		public int DEC_DE(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte b=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			b--;
			SetByte(AddressingMode.AbsoluteIndexed,addr,b,this.X);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 7;
		}
		#endregion DECrement Memory

		#region INCrement Memory
		//INC (INCrement memory)
		//Affects Flags: S Z

		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     INC $44       $E6  2   5
		//Zero Page,X   INC $44,X     $F6  2   6
		//Absolute      INC $4400     $EE  3   6
		//Absolute,X    INC $4400,X   $FE  3   7

		public int INC_E6(byte input1) {
			byte b=ReadByte(AddressingMode.ZeroPage,input1);
			b++;
			SetByte(AddressingMode.ZeroPage,input1,b);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 5;
		}

		public int INC_F6(byte input1) {
			byte b=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			b++;
			SetByte(AddressingMode.ZeroPageIndexed,input1,b,this.X);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}

		public int INC_EE(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte b=ReadByte(AddressingMode.Absolute,addr);
			b++;
			SetByte(AddressingMode.Absolute,addr,b);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}

		public int INC_FE(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte b=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			b++;
			SetByte(AddressingMode.AbsoluteIndexed,addr,b,this.X);
			SetFlagsForValue(b,new List<Flag> { Flag.Z, Flag.N});
			return 7;
		}
		#endregion INCrement Memory

		#region	CoMPare accumulator
		//CMP (CoMPare accumulator)
		//Affects Flags: S Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     CMP #$44      $C9  2   2
		//Zero Page     CMP $44       $C5  2   3
		//Zero Page,X   CMP $44,X     $D5  2   4
		//Absolute      CMP $4400     $CD  3   4
		//Absolute,X    CMP $4400,X   $DD  3   4+
		//Absolute,Y    CMP $4400,Y   $D9  3   4+
		//Indirect,X    CMP ($44,X)   $C1  2   6
		//Indirect,Y    CMP ($44),Y   $D1  2   5+

		//+ add 1 cycle if page boundary crossed

		//Compare sets flags as if a subtraction had been carried out. If the value in the accumulator is equal or greater than the compared value, the Carry will be set. The equal (Z) and sign (S) flags will be set based on equality or lack thereof and the sign (i.e. A>=$80) of the accumulator.
 
		public int CMP_C9(byte input1) {
			byte val=ReadByte(AddressingMode.Immediate,input1);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 2;
		}

		public int CMP_C5(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPage,input1);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 3;
		}

		public int CMP_D5(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}

		public int CMP_CD(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.Absolute,addr);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}

		public int CMP_DD(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}

		public int CMP_D9(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.Y);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}

		public int CMP_C1(byte input1) {
			byte val=ReadByte(AddressingMode.IndexedIndirectX,input1);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 6;
		}

		public int CMP_D1(byte input1) {
			byte val=ReadByte(AddressingMode.IndirectIndexedY,input1);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.A>=val);
			SetFlag(Flag.Z,this.A==val);
			SetFlag(Flag.N,result<0);
			return 5;
		}
		#endregion CoMPare

		#region Compare X and Y
		//CPX (ComPare X register)
		//Affects Flags: S Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     CPX #$44      $E0  2   2
		//Zero Page     CPX $44       $E4  2   3
		//Absolute      CPX $4400     $EC  3   4

		//Operation and flag results are identical to equivalent mode accumulator CMP ops.
		public int CPX_E0(byte input1) {
			byte val=ReadByte(AddressingMode.Immediate,input1);
			byte result=(byte)(this.X-val);
			SetFlag(Flag.C,this.X>=val);
			SetFlag(Flag.Z,this.X==val);
			SetFlag(Flag.N,result<0);
			return 2;
		}

		public int CPX_E4(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPage,input1);
			byte result=(byte)(this.X-val);
			SetFlag(Flag.C,this.X>=val);
			SetFlag(Flag.Z,this.X==val);
			SetFlag(Flag.N,result<0);
			return 3;
		}

		public int CPX_EC(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.Absolute,addr);
			byte result=(byte)(this.X-val);
			SetFlag(Flag.C,this.X>=val);
			SetFlag(Flag.Z,this.X==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}

		//CPY (ComPare Y register)
		//Affects Flags: S Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     CPY #$44      $C0  2   2
		//Zero Page     CPY $44       $C4  2   3
		//Absolute      CPY $4400     $CC  3   4

		//Operation and flag results are identical to equivalent mode accumulator CMP ops.

		public int CPY_C0(byte input1) {
			byte val=ReadByte(AddressingMode.Immediate,input1);
			byte result=(byte)(this.Y-val);
			SetFlag(Flag.C,this.Y>=val);
			SetFlag(Flag.Z,this.Y==val);
			SetFlag(Flag.N,result<0);
			return 2;
		}

		public int CPY_C4(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPage,input1);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.Y>=val);
			SetFlag(Flag.Z,this.Y==val);
			SetFlag(Flag.N,result<0);
			return 3;
		}

		public int CPY_CC(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.Absolute,addr);
			byte result=(byte)(this.A-val);
			SetFlag(Flag.C,this.Y>=val);
			SetFlag(Flag.Z,this.Y==val);
			SetFlag(Flag.N,result<0);
			return 4;
		}
		#endregion Compare X and Y

		#region Jump
		//JMP (JuMP)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Absolute      JMP $5597     $4C  3   3
		//Indirect      JMP ($5597)   $6C  3   5

		//JMP transfers program execution to the following address (absolute) or to the location contained in the following address (indirect).
		//An original 6502 has does not correctly fetch the target address if the indirect vector falls on a page boundary (e.g. $xxFF where xx is any value from $00 to $FF). In this case fetches the LSB from $xxFF as expected but takes the MSB from $xx00. This is fixed in some later chips like the 65SC02 so for compatibility always ensure the indirect vector is not at the end of the page.


		public int JMP_4C(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			PC=addr;
			return 3;
		}

		public int JMP_6C(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			addr=GetAddr(AddressingMode.Indirect,addr);
			PC=addr;
			return 5;
		}
		#endregion Jump

		public int NOP_EA() {
			return 2;
		}

		#region STore A
		//STA (STore Accumulator)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Zero Page     STA $44       $85  2   3
		//Zero Page,X   STA $44,X     $95  2   4
		//Absolute      STA $4400     $8D  3   4
		//Absolute,X    STA $4400,X   $9D  3   5
		//Absolute,Y    STA $4400,Y   $99  3   5
		//Indirect,X    STA ($44,X)   $81  2   6
		//Indirect,Y    STA ($44),Y   $91  2   6

		public int STA_85(byte input1) {
			SetByte(AddressingMode.ZeroPage,input1,this.A);
			return 3;
		}
		public int STA_95(byte input1) {
			SetByte(AddressingMode.ZeroPageIndexed,input1,this.A,this.X);
			return 4;
		}
		public int STA_8D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			SetByte(AddressingMode.Absolute,addr,this.A);
			return 4;
		}
		public int STA_9D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			SetByte(AddressingMode.AbsoluteIndexed,addr,this.A,this.X);
			return 5;
		}
		public int STA_99(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			SetByte(AddressingMode.AbsoluteIndexed,input1,this.A,this.Y);
			return 5;
		}
		public int STA_81(byte input1) {
			SetByte(AddressingMode.IndexedIndirectX,input1,this.A);
			return 6;
		}
		public int STA_91(byte input1) {
			SetByte(AddressingMode.IndirectIndexedY,input1,this.A);
			return 6;
		}
		#endregion STore A

		#region Flag Instructions
		//MNEMONIC                       HEX
		//CLC (CLear Carry)              $18
		//SEC (SEt Carry)                $38
		//CLI (CLear Interrupt)          $58
		//SEI (SEt Interrupt)            $78
		//CLV (CLear oVerflow)           $B8
		//CLD (CLear Decimal)            $D8
		//SED (SEt Decimal)              $F8
		public int CLC_18() {
			SetFlag(Flag.C,false);
			return 2;
		}
		public int SEC_38() {
			SetFlag(Flag.C,true);
			return 2;
		}
		public int CLI_58() {
			SetFlag(Flag.I,false);
			return 2;
		}
		public int SEI_78() {
			SetFlag(Flag.I,true);
			return 2;
		}
		public int CLV_B8() {
			SetFlag(Flag.V,false);
			return 2;
		}
		public int CLD_D8() {
			SetFlag(Flag.D,false);
			return 2;
		}
		public int SED_F8() {
			SetFlag(Flag.D,true);
			return 2;
		}

		#endregion Flag Instructions

		#region OR on Accumulator
		//ORA (bitwise OR with Accumulator)
		//Affects Flags: S Z

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     ORA #$44      $09  2   2
		//Zero Page     ORA $44       $05  2   3
		//Zero Page,X   ORA $44,X     $15  2   4
		//Absolute      ORA $4400     $0D  3   4
		//Absolute,X    ORA $4400,X   $1D  3   4+
		//Absolute,Y    ORA $4400,Y   $19  3   4+
		//Indirect,X    ORA ($44,X)   $01  2   6
		//Indirect,Y    ORA ($44),Y   $11  2   5+

		public int ORA_09(byte input1) {
			this.A|=input1;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 2;
		}
		public int ORA_05(byte input1) {
			this.A|=ReadByte(AddressingMode.ZeroPage,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 3;
		}
		public int ORA_15(byte input1) {
			this.A|=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int ORA_0D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A|=ReadByte(AddressingMode.Absolute,addr);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int ORA_1D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A|=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int ORA_19(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A|=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.Y);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int ORA_01(byte input1) {
			this.A|=ReadByte(AddressingMode.IndexedIndirectX,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}
		public int ORA_11(byte input1) {
			this.A|=ReadByte(AddressingMode.Absolute,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 5;
		}

		#endregion OR on Accumulator

		#region XOR on Accumulator
		//EOR (bitwise Exclusive OR)
		//Affects Flags: S Z

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     EOR #$44      $49  2   2
		//Zero Page     EOR $44       $45  2   3
		//Zero Page,X   EOR $44,X     $55  2   4
		//Absolute      EOR $4400     $4D  3   4
		//Absolute,X    EOR $4400,X   $5D  3   4+
		//Absolute,Y    EOR $4400,Y   $59  3   4+
		//Indirect,X    EOR ($44,X)   $41  2   6
		//Indirect,Y    EOR ($44),Y   $51  2   5+

		//+ add 1 cycle if page boundary crossed

		public int EOR_49(byte input1) {
			this.A^=input1;
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 2;
		}
		public int EOR_45(byte input1) {
			this.A^=ReadByte(AddressingMode.ZeroPage,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 3;
		}
		public int EOR_55(byte input1) {
			this.A^=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int EOR_4D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A^=ReadByte(AddressingMode.Absolute,addr);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int EOR_5D(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A^=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int EOR_59(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			this.A^=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.Y);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 4;
		}
		public int EOR_41(byte input1) {
			this.A^=ReadByte(AddressingMode.IndexedIndirectX,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 6;
		}
		public int EOR_51(byte input1) {
			this.A^=ReadByte(AddressingMode.Absolute,input1);
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z, Flag.N});
			return 5;
		}
		#endregion XOR on Accumulator

		#region Rotations
		//ROL (ROtate Left)
		//Affects Flags: S Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Accumulator   ROL A         $2A  1   2
		//Zero Page     ROL $44       $26  2   5
		//Zero Page,X   ROL $44,X     $36  2   6
		//Absolute      ROL $4400     $2E  3   6
		//Absolute,X    ROL $4400,X   $3E  3   7

		//ROL shifts all bits left one position. The Carry is shifted into bit 0 and the original bit 7 is shifted into the Carry.

		public int ROL_2A(byte input) {
			bool carry=GetFlagBool(Flag.C);
			SetFlag(Flag.C,CheckBit(input,7));
			this.A=(byte)(input<<1);
			SetBit(this.A,0,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 2;
		}

		public int ROL_26(byte input) {
			bool carry=GetFlagBool(Flag.C);
			byte val=ReadByte(AddressingMode.ZeroPage,input);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetBit(this.A,0,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 5;
		}

		public int ROL_36(byte input) {
			bool carry=GetFlagBool(Flag.C);
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input,this.X);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetBit(this.A,0,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		public int ROL_2E(byte input1,byte input2) {
			bool carry=GetFlagBool(Flag.C);
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.ZeroPage,addr);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetBit(this.A,0,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		public int ROL_3E(byte input1,byte input2) {
			bool carry=GetFlagBool(Flag.C);
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlag(Flag.C,CheckBit(val,7));
			this.A=(byte)(val<<1);
			SetBit(this.A,0,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 7;
		}
		//ROR (ROtate Right)
		//Affects Flags: S Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Accumulator   ROR A         $6A  1   2
		//Zero Page     ROR $44       $66  2   5
		//Zero Page,X   ROR $44,X     $76  2   6
		//Absolute      ROR $4400     $6E  3   6
		//Absolute,X    ROR $4400,X   $7E  3   7

		//ROR shifts all bits right one position. The Carry is shifted into bit 7 and the original bit 0 is shifted into the Carry.

		public int ROR_6A(byte input) {
			bool carry=GetFlagBool(Flag.C);
			SetFlag(Flag.C,CheckBit(input,0));
			this.A=(byte)(input>>1);
			SetBit(this.A,7,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 2;
		}

		public int ROR_66(byte input) {
			bool carry=GetFlagBool(Flag.C);
			byte val=ReadByte(AddressingMode.ZeroPage,input);
			SetFlag(Flag.C,CheckBit(val,0));
			this.A=(byte)(val>>1);
			SetBit(this.A,7,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 5;
		}

		public int ROR_76(byte input) {
			bool carry=GetFlagBool(Flag.C);
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input,this.X);
			SetFlag(Flag.C,CheckBit(val,0));
			this.A=(byte)(val>>1);
			SetBit(this.A,7,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		public int ROR_6E(byte input1,byte input2) {
			bool carry=GetFlagBool(Flag.C);
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.ZeroPage,addr);
			SetFlag(Flag.C,CheckBit(val,0));
			this.A=(byte)(val>>1);
			SetBit(this.A,7,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 6;
		}

		public int ROR_7E(byte input1,byte input2) {
			bool carry=GetFlagBool(Flag.C);
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			SetFlag(Flag.C,CheckBit(val,0));
			this.A=(byte)(val>>1);
			SetBit(this.A,7,carry);
			SetFlagsForValue(this.A,new List<Flag> { Flag.N,Flag.Z });
			return 7;
		}
		#endregion Rotations

		#region Branch Instructions
		//Branch Instructions
		//Affect Flags: none

		//All branches are relative mode and have a length of two bytes. 
		//Syntax is "Bxx Displacement" or (better) "Bxx Label". See the notes on the Program Counter for more on displacements.
		//Branches are dependant on the status of the flag bits when the op code is encountered. 
		//A branch not taken requires two machine cycles. Add one if the branch is taken and add one more if the branch crosses a page boundary.

		//MNEMONIC                       HEX
		//BPL (Branch on PLus)           $10
		//BMI (Branch on MInus)          $30
		//BVC (Branch on oVerflow Clear) $50
		//BVS (Branch on oVerflow Set)   $70
		//BCC (Branch on Carry Clear)    $90
		//BCS (Branch on Carry Set)      $B0
		//BNE (Branch on Not Equal)      $D0
		//BEQ (Branch on EQual)          $F0

		public int BPL_10(byte input1) {
			//If the negative flag is clear, 
			if(!GetFlagBool(Flag.N)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BMI_30(byte input1) {
			//If the negative flag is clear, 
			if(GetFlagBool(Flag.N)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BVC_50(byte input1) {
			//If the negative flag is clear, 
			if(!GetFlagBool(Flag.V)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BVS_70(byte input1) {
			//If the negative flag is clear, 
			if(GetFlagBool(Flag.V)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BCC_90(byte input1) {
			//If the negative flag is clear, 
			if(!GetFlagBool(Flag.C)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BCS_B0(byte input1) {
			//If the negative flag is clear, 
			if(GetFlagBool(Flag.C)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BNE_D0(byte input1) {
			//If the negative flag is clear, 
			if(!GetFlagBool(Flag.Z)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		public int BEQ_F0(byte input1) {
			//If the negative flag is clear, 
			if(GetFlagBool(Flag.Z)) {
				PC+=2;
				return 3;
			}
			else {
				PC++;
				return 2;
			}
		}
		




		#endregion Branch Instructions

		#region Single Instructions
		//BRK (BReaK)
		//Affects Flags: B

		//MODE           SYNTAX       HEX LEN TIM
		//Implied       BRK           $00  1   7

		//BRK causes a non-maskable interrupt and increments the program counter by one. 
		//Therefore an RTI will go to the address of the BRK +2 so that BRK may be used to 
		//replace a two-byte instruction for debugging and the subsequent RTI will be correct.

		public int BRK_00() {
			//PC++;
			SetFlag(Flag.B,true);
			return 7;
		}

		//JSR (Jump to SubRoutine)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Absolute      JSR $5597     $20  3   6

		//JSR pushes the address-1 of the next operation on to the stack before transferring program 
		//control to the following address. Subroutines are normally terminated by a RTS op code.

		public int JSR_20(byte input1, byte input2) {
			push((short)PC--);
			ushort addr=BytesToShort(input1,input2);
			PC=addr;
			return 6;
		}

		//RTI (ReTurn from Interrupt)
		//Affects Flags: all

		//MODE           SYNTAX       HEX LEN TIM
		//Implied       RTI           $40  1   6

		//RTI retrieves the Processor Status Word (flags) and the Program Counter from the stack in that order (interrupts push the PC first and then the PSW).
		//Note that unlike RTS, the return address on the stack is the actual address rather than the address-1.

		public int RTI_40() {
			byte flags=pop();
			SetFlagsForValue(flags,new List<Flag> { Flag.A,Flag.B,Flag.C,Flag.D,Flag.I,Flag.N,Flag.V,Flag.Z});
			byte low=pop();
			byte high=pop();
			ushort addr=(byte)(BytesToShort(high,low));
			PC=addr;
			return 6;
		}

		//RTS (ReTurn from Subroutine)
		//Affects Flags: none

		//MODE           SYNTAX       HEX LEN TIM
		//Implied       RTS           $60  1   6

		//RTS pulls the top two bytes off the stack (low byte first) and transfers program control to 
		//that address+1. It is used, as expected, to exit a subroutine invoked via JSR which pushed the address-1.

		public int RTS_60() {
			byte low=pop();
			byte high=pop();
			ushort addr=(byte)(BytesToShort(high,low)+1);
			PC=addr;
			return 6;
		}


		#endregion Single Instructions

		#region Subtract with Carry
		//A,Z,C,N = A-M-(1-C)
		//SBC (SuBtract with Carry)
		//Affects Flags: S V Z C

		//MODE           SYNTAX       HEX LEN TIM
		//Immediate     SBC #$44      $E9  2   2
		//Zero Page     SBC $44       $E5  2   3
		//Zero Page,X   SBC $44,X     $F5  2   4
		//Absolute      SBC $4400     $ED  3   4
		//Absolute,X    SBC $4400,X   $FD  3   4+
		//Absolute,Y    SBC $4400,Y   $F9  3   4+
		//Indirect,X    SBC ($44,X)   $E1  2   6
		//Indirect,Y    SBC ($44),Y   $F1  2   5+

		public int SBC_E9(byte input1) {
			this.A=(byte)(A-input1-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 2;
		}
		public int SBC_E5(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPage,input1);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 3;
		}
		public int SBC_F5(byte input1) {
			byte val=ReadByte(AddressingMode.ZeroPageIndexed,input1,this.X);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 3;
		}
		public int SBC_ED(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.Absolute,addr);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 4;
		}
		public int SBC_FD(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.X);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 4;
		}
		public int SBC_F9(byte input1, byte input2) {
			ushort addr=BytesToShort(input1,input2);
			byte val=ReadByte(AddressingMode.AbsoluteIndexed,addr,this.Y);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 4;
		}
		public int SBC_E1(byte input1) {
			byte val=ReadByte(AddressingMode.IndexedIndirectX,input1);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 6;
		}
		public int SBC_F1(byte input1) {
			byte val=ReadByte(AddressingMode.IndirectIndexedY,input1);
			this.A=(byte)(A-val-(byte)(1-GetFlagByte(Flag.C)));
			SetFlagsForValue(this.A,new List<Flag> { Flag.Z,Flag.C,Flag.N});
			return 5;
		}
		#endregion Subtract with Carry

	}
}
