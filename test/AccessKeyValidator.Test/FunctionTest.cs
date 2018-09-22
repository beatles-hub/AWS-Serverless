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
using AccessKeyValidator;
using AccessKeyValidator.Model;

namespace HelloWorld.Tests
{
  public class FunctionTest
  {
    [Fact]
    public void TestValidAccessKeyID()
    {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();

            AccessKeyValidatorAPIRequest testRequestObj = new AccessKeyValidatorAPIRequest();
            testRequestObj.AccessKeyID = "[Put Valid ACCESS KEY ID here]";


            StartupProgram mAccessKeyValidator = new StartupProgram(true);

            request.Body = JsonConvert.SerializeObject(testRequestObj);
            response = mAccessKeyValidator.ValidateAccessKey(request, context);

            Assert.Equal(200, response.StatusCode);
    }

        [Fact]
        public void TestInValidAccessKeyID()
        {
            TestLambdaContext context;
            APIGatewayProxyRequest request;
            APIGatewayProxyResponse response;

            request = new APIGatewayProxyRequest();
            context = new TestLambdaContext();

            AccessKeyValidatorAPIRequest testRequestObj = new AccessKeyValidatorAPIRequest();
            testRequestObj.AccessKeyID = "HELLOWORLD";

            StartupProgram mAccessKeyValidator = new StartupProgram(true);

            request.Body = JsonConvert.SerializeObject(testRequestObj);
            response = mAccessKeyValidator.ValidateAccessKey(request, context);

            Assert.Equal(200, response.StatusCode);
        }
    }
}
