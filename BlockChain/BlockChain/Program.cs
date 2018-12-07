using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BlockChain.Classes;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Add using statements to access AWS SDK for .NET services. 
// Both the Service and its Model namespace need to be added 
// in order to gain access to a service. For example, to access
// the EC2 service, add:
// using Amazon.EC2;
// using Amazon.EC2.Model;

namespace BlockChain {
	class Program {
		public static void Main(string[] args) {
			Database db = new Database();
			Block old = db.GetNewest();
			Block newest = new Block(old, "Test message");
			db.Insert(newest);
		}
	}
}