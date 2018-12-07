using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502 {
	class Program {
		static void Main(string[] args) {
			mos6502 CPU=new mos6502();
			CPU.LoadProgramIntoMemory(File.ReadAllBytes("./TestFiles/6502_functional_test.bin"));
			CPU.Start();
		}
	}
}
