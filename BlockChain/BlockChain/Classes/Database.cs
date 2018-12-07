using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlockChain.Classes;

namespace BlockChain.Classes {
	class Database {
		private AmazonDynamoDBConfig ddbConfig;
		private static AmazonDynamoDBClient client;
		private static string tablename = "Blocks";
		private Table blockchain;
		public Database() {
			// First, set up a DynamoDB client for DynamoDB Local
			ddbConfig = new AmazonDynamoDBConfig();
			//ddbConfig.ServiceURL = "dynamodb.us-west-2.amazonaws.com	";
			ddbConfig.RegionEndpoint = RegionEndpoint.USWest2;
			try {
				client = new AmazonDynamoDBClient(ddbConfig);
			}
			catch(Exception ex) {
				Console.WriteLine("\n Error: failed to create a DynamoDB client; " + ex.Message);
				PauseForDebugWindow();
				return;
			}
			UpdateDatabase();
		}

		public void UpdateDatabase() {
			if(!Table.TryLoadTable(client,"Blocks",out blockchain)) {
				CreateDatabase();
			}
		}

		public void Insert(Block block) {
			var newblock = new Document();
			
			newblock["Index"] = block.index;
			newblock["PreviousHash"] = block.previousHash;
			newblock["Timestamp"] = block.timestamp;
			newblock["Data"] = block.data;
			newblock["Hash"] = block.hash;

			blockchain.PutItem(newblock);
		}

		private void CreateGenesisBlock() {
			var newblock = new Document();
			
			Block blk = new Block();
			blk = blk.getGenisisBlock();

			newblock["Index"] = blk.index;
			newblock["PreviousHash"] = blk.previousHash;
			newblock["Timestamp"] = blk.timestamp.ToString();
			newblock["Data"] = blk.data;
			newblock["Hash"] = blk.hash;

			blockchain.PutItem(newblock);
		}

		private bool CreateDatabase() {
			{
				var createTableRequest =
					 new CreateTableRequest()
					 {
						 TableName = tablename,
						 ProvisionedThroughput =
						  new ProvisionedThroughput()
						  {
							  ReadCapacityUnits = (long)1,
							  WriteCapacityUnits = (long)1
						  }
					 };

				var attributeDefinitions = new List<AttributeDefinition>()
		  {
            // Attribute definitions for table primary key
            { new AttributeDefinition() {
						AttributeName = "Index", AttributeType = "N"
				  } },
				{ new AttributeDefinition() {
						AttributeName = "PreviousHash", AttributeType = "S"
				  } },
            // Attribute definitions for index primary key
            { new AttributeDefinition() {
						AttributeName = "Timestamp", AttributeType = "S"
				  } },
				{ new AttributeDefinition() {
						AttributeName = "Hash", AttributeType = "S"
				  } },
					{ new AttributeDefinition() {
						AttributeName = "Data", AttributeType = "S"
					} }
		  };

				createTableRequest.AttributeDefinitions = attributeDefinitions;

				// Key schema for table
				var tableKeySchema = new List<KeySchemaElement>()
		  {
				{ new KeySchemaElement() {
						AttributeName = "Hash", KeyType = "HASH"
				  } },                                                  //Partition key
            { new KeySchemaElement() {
						AttributeName = "Index", KeyType = "RANGE"
				  } }                                                //Sort key
        };

				createTableRequest.KeySchema = tableKeySchema;

				var localSecondaryIndexes = new List<LocalSecondaryIndex>();

				// TimestampIndex
				LocalSecondaryIndex TimestampIndex = new LocalSecondaryIndex()
				{
					IndexName = "TimestampIndex"
				};

				// Key schema for TimestampIndex
				var indexKeySchema = new List<KeySchemaElement>()
		  {
				{ new KeySchemaElement() {
						AttributeName = "Hash", KeyType = "HASH"
				  } },                                                    //Partition key
            { new KeySchemaElement() {
						AttributeName = "Timestamp", KeyType = "RANGE"
				  } }                                                            //Sort key
        };

				TimestampIndex.KeySchema = indexKeySchema;

				// Projection (with list of projected attributes) for
				// TimestampIndex
				var projection = new Projection()
				{
					ProjectionType = "INCLUDE"
				};

				var nonKeyAttributes = new List<string>()
		  {
				"Data"				
		  };
				projection.NonKeyAttributes = nonKeyAttributes;

				TimestampIndex.Projection = projection;

				localSecondaryIndexes.Add(TimestampIndex);

				// HashIndex
				LocalSecondaryIndex HashIndex
					 = new LocalSecondaryIndex()
					 {
						 IndexName = "HashIndex"
					 };

				// Key schema for HashIndex
				indexKeySchema = new List<KeySchemaElement>()
		  {
				{ new KeySchemaElement() {
						AttributeName = "Hash", KeyType = "HASH"
				  }},                                                     //Partition key
            { new KeySchemaElement() {
						AttributeName = "PreviousHash", KeyType = "RANGE"
				  }}                                                  //Sort key
        };

				// Projection (all attributes) for HashIndex
				projection = new Projection() {
					ProjectionType = "ALL"
				};

				HashIndex.KeySchema = indexKeySchema;
				HashIndex.Projection = projection;

				localSecondaryIndexes.Add(HashIndex);

				// DataIndex
				LocalSecondaryIndex DataIndex
					 = new LocalSecondaryIndex()
					 {
						 IndexName = "DataIndex"
					 };

				// Key schema for DataIndex
				indexKeySchema = new List<KeySchemaElement>()
		  {
				{ new KeySchemaElement() {
						AttributeName = "Hash", KeyType = "HASH"
				  }},                                                     //Partition key
            { new KeySchemaElement() {
						AttributeName = "Data", KeyType = "RANGE"
				  }}                                                  //Sort key
        };

				// Projection (all attributes) for DataIndex
				projection = new Projection() {
					ProjectionType = "ALL"
				};

				DataIndex.KeySchema = indexKeySchema;
				DataIndex.Projection = projection;

				localSecondaryIndexes.Add(DataIndex);
				// Add index definitions to CreateTable request
				createTableRequest.LocalSecondaryIndexes = localSecondaryIndexes;

				Console.WriteLine("Creating table " + tablename + "...");
				client.CreateTable(createTableRequest);
				//WaitUntilTableReady(tablename);
			}
			WaitUntilTableReady(tablename);

			Table.TryLoadTable(client, tablename, out blockchain);
			CreateGenesisBlock();

			return true;
		}

		private static void WaitUntilTableReady(string tableName) {
			string status = null;
			// Let us wait until table is created. Call DescribeTable.
			do {
				System.Threading.Thread.Sleep(5000); // Wait 5 seconds.
				try {
					var res = client.DescribeTable(new DescribeTableRequest
						  {
						TableName = tableName
					});

					Console.WriteLine("Table name: {0}, status: {1}",
								 res.Table.TableName,
								 res.Table.TableStatus);
					status = res.Table.TableStatus;
				}
				catch(ResourceNotFoundException) {
					// DescribeTable is eventually consistent. So you might
					// get resource not found. So we handle the potential exception.
				}
			} while(status != "ACTIVE");
		}

		private static void PauseForDebugWindow() {
			// Keep the console open if in Debug mode...
			Console.Write("\n\n ...Press any key to continue");
			Console.ReadKey();
			Console.WriteLine();
		}

		public Block GetNewest() {
			var request = new ScanRequest
			{
				TableName = tablename,
			};

			var response = client.Scan(request);
			var result = response.Items[response.Items.Count-1];

			Block created = new Block();
			created.index = Convert.ToDouble(result["Index"].N);
			created.previousHash = result["PreviousHash"].S;
			created.timestamp = Convert.ToDateTime(result["Timestamp"].S);
			created.data = result["Data"].S;
			created.hash = result["Hash"].S;
			return created;
		}
	}
}
