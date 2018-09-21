using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.Lambda.Core;
using Amazon;

namespace S3ToDynamoDB.HelperClasses
{
    class DynamoDBHelper
    {
        private ILambdaContext context;
        //AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
        //private static AmazonDynamoDBClient client = new AmazonDynamoDBClient("AKIAIFCF6YET352QLTHQ", "ZHKRE2zXxybJbJMsHkD0ulMCAGU6QfyISTgAd0GU","ap-south-1");
        private static AmazonDynamoDBClient client;
        private string tableName;

        public DynamoDBHelper(ILambdaContext mContext , string mTableName , bool isLocalDebug)
        {
            try
            {
                this.context = mContext;
                this.tableName = mTableName;
                if (isLocalDebug)
                {
                    client = new AmazonDynamoDBClient(new StoredProfileAWSCredentials("Hackathon"), RegionEndpoint.APSouth1);
                }
                else
                {
                    client = new AmazonDynamoDBClient();
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper  => " + ex.StackTrace);
            }
        }
        public void putItem(KeyDBItem newItem, string TableName)
        {
            try
            {
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- TableName = " + TableName);
                Table table = Table.LoadTable(client, TableName);

                var clientItem = new Document();
                clientItem["TimeStamp"] = newItem.Timestamp;
                clientItem["FileName"] = newItem.FileName;

                table.PutItemAsync(clientItem).GetAwaiter().GetResult();
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- PutOperation succeeded");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- " + ex.Message);
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- " + ex.InnerException.Message);
            }
        }

        /*
        public void putItemWithPublicKey(string clientIDPK, DBItemWithPublicKey securityKey, string TableName)
        {
            try
            {
                Table table = Table.LoadTable(client, TableName);

                var clientItem = new Document();
                clientItem["client_id"] = clientIDPK;
                clientItem["client_name"] = securityKey.ClientName;
                clientItem["client_public_key"] = securityKey.ClientPublicKey;

                table.PutItemAsync(clientItem).GetAwaiter().GetResult();
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- PutOperation succeeded");
            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper::PutItem() -- " + ex.StackTrace);
            }
        }
        */

        public Document getItem(string clientIDPK, string TableName)
        {
            context.Logger.LogLine("DynamoDBHelper::GetItem()=> TableName = " + TableName);
            context.Logger.LogLine("DynamoDBHelper::GetItem()=> ClientID = " + clientIDPK);
            Document clientItem = null;
            try
            {
                Table table = Table.LoadTable(client, TableName);


                // Configuration object that specifies optional parameters.
                GetItemOperationConfig config = new GetItemOperationConfig()
                {
                    AttributesToGet = new List<string>() { "CMKARN", "Encrypted256BitKey", "KMSEncryptedDataKey", "Normal256BitKey", "IV", "ClientSecretOTP", "SNSTopicARN", "OTPValidityInSeconds", "OTPGeneratedLinuxTime" },
                };
                // Pass in the configuration to the GetItem method.
                // 1. Table that has only a partition key as primary key.
                clientItem = table.GetItemAsync(clientIDPK, config).GetAwaiter().GetResult();
                context.Logger.LogLine("DynamoDBHelper::GetItem() -- GetOperation succeeded");

            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper::GetItem() -- " + ex.StackTrace);
            }
            return clientItem;
        }

        public Document getItemWithPublicKey(string clientIDPK, string TableName)
        {
            context.Logger.LogLine("DynamoDBHelper::GetItem()=> TableName = " + TableName);
            Document clientItem = null;
            try
            {
                Table table = Table.LoadTable(client, TableName);


                // Configuration object that specifies optional parameters.
                GetItemOperationConfig config = new GetItemOperationConfig()
                {
                    AttributesToGet = new List<string>() { "client_name", "client_public_key" },
                };
                // Pass in the configuration to the GetItem method.
                // 1. Table that has only a partition key as primary key.
                clientItem = table.GetItemAsync(clientIDPK, config).GetAwaiter().GetResult();
                context.Logger.LogLine("DynamoDBHelper::GetItem() -- GetOperation succeeded");

            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper::GetItem() -- " + ex.StackTrace);
            }
            return clientItem;
        }

        public Document getLogServerDetails(string Environment, string TableName)
        {
            context.Logger.LogLine("DynamoDBHelper::GetLogServerDetails()=> TableName = " + TableName);
            Document clientItem = null;
            try
            {
                Table table = Table.LoadTable(client, TableName);


                // Configuration object that specifies optional parameters.
                GetItemOperationConfig config = new GetItemOperationConfig()
                {
                    AttributesToGet = new List<string>() { "Database", "Environment", "Password", "Port", "Server", "Uid" },
                };
                // Pass in the configuration to the GetItem method.
                // 1. Table that has only a partition key as primary key.
                clientItem = table.GetItemAsync(Environment, config).GetAwaiter().GetResult();
                context.Logger.LogLine("DynamoDBHelper::getLogServerDetails() -- GetOperation succeeded");

            }
            catch (Exception ex)
            {
                context.Logger.LogLine("DynamoDBHelper::getLogServerDetails() -- " + ex.StackTrace);
            }
            return clientItem;
        }
    }
}


