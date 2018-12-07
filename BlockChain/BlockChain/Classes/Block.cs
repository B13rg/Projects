using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BlockChain.Classes {
	class Block {
		public double index;
		public string previousHash;
		public DateTime timestamp;
		public string data;
		public string hash;

		public Block() { }

		public Block(Block previous, string Data) {
			this.index = previous.index+1;
			this.previousHash = previous.hash;
			this.timestamp = DateTime.Now;
			this.data = Data;
			this.hash = calculateHash();
		}

		public Block(double index,string previousHash,DateTime timestamp,string data,string hash) {
			this.index = index;
			this.previousHash = previousHash;
			this.timestamp = timestamp;
			this.data = data;
			this.hash = hash;
		}

		private string calculateHash() {
			string storage = this.index.ToString()+this.previousHash+this.timestamp.ToString()+this.data;
			SHA256Managed crypt = new SHA256Managed();
			StringBuilder hash = new StringBuilder();
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(storage), 0, Encoding.UTF8.GetByteCount(storage));
			return Convert.ToBase64String(crypto);
		}

		public string calculateHash(double index,string previousHash,DateTime timestamp,string data) {
			string storage = index.ToString()+previousHash+timestamp.ToString()+data;
			SHA256Managed crypt = new SHA256Managed();
			StringBuilder hash = new StringBuilder();
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(storage), 0, Encoding.UTF8.GetByteCount(storage));
			return Convert.ToBase64String(crypto);
		}

		private Block getPreviousBlock() {
			//make database query
			return null;
		}

		private Block generateNextBlock(string data) {
			Block previousBlock = getPreviousBlock();
			double index = previousBlock.index+1;
			DateTime nextTimestamp = DateTime.UtcNow;
			string nextHash = calculateHash(index, previousBlock.hash, nextTimestamp, data);
			return new Block(index,previousBlock.hash,nextTimestamp,data,nextHash);
		}

		public Block getGenisisBlock() {
			//getcurrentindex
			string hash = calculateHash(0,"0",DateTime.MinValue, "Genisis Block");
			return new Block(0,"0",DateTime.MinValue,"Genisis Block",hash);
		}

	}
}
