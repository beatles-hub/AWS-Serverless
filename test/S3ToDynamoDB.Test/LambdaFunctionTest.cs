using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.S3;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using S3Parse;

namespace HelloWorld.Tests
{
  public class LambdaFunctionTest
  {
        private string profileName = "Hackathon";
        private Amazon.S3.IAmazonS3 s3Client = null;

        public LambdaFunctionTest()
        {
            Amazon.Runtime.AWSCredentials credentials = new Amazon.Runtime.StoredProfileAWSCredentials(profileName);
            s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.APSouth1);
        }

        [Fact]
        public void TestDynamoDBPut()
        {
            TestLambdaContext context = new TestLambdaContext();

            var StartupProgram = new StartupProgram(s3Client);
            string testJsonInput = "{\"Records\":[{\"eventVersion\":\"2.0\",\"eventSource\":\"aws: s3\",\"awsRegion\":\"ap - south - 1\",\"eventTime\":\"1970 - 01 - 01T00: 00:00.000Z\",\"eventName\":\"ObjectCreated: Put\",\"userIdentity\":{\"principalId\":\"AIDAJDPLRKLG7UEXAMPLE\"},\"requestParameters\":{\"sourceIPAddress\":\"127.0.0.1\"},\"responseElements\":{\"x - amz - request - id\":\"C3D13FE58DE4C810\",\"x - amz - id - 2\":\"FMyUVURIY8 / IgAtTv8xRjskZQpcIZ9KG4V5Wp6S7S / JRWeUWerMUE5JgHvANOjpD\"},\"s3\":{\"s3SchemaVersion\":\"1.0\",\"configurationId\":\"testConfigRule\",\"bucket\":{\"name\":\"pwc - logfilesource\",\"ownerIdentity\":{\"principalId\":\"A3NL1KOZZKExample\"},\"arn\":\"arn: aws: s3:::pwc - logfilesource\"},\"object\":{\"key\":\"README.md\",\"size\":1024,\"eTag\":\"d41d8cd98f00b204e9800998ecf8427e\",\"versionId\":\"096fKKXTRTtl3on89fVO.nfljtsv6qko\",\"sequencer\":\"0055AED6DCD90281E5\"}}}]}";

            S3Event testEvent = new S3Event();
            S3EventNotification s3EventNotification = S3EventNotification.ParseJson(testJsonInput);
            testEvent.Records = s3EventNotification.Records;

            StartupProgram.ProcessS3Events(testEvent, context);
        }
  }
}
