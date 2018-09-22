using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.IdentityManagement;
using AccessKeyValidator.HelperClasses;
using Amazon.IdentityManagement.Model;
using AccessKeyValidator.Model;
using System.Net;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AccessKeyValidator
{
    public class StartupProgram
    {
        private AmazonIdentityManagementServiceClient iamClient;
        private bool isLocalDebug;

        public StartupProgram()
        {
            this.isLocalDebug = false;
            Setup();
        }

        public StartupProgram(bool isLocalDebug)
        {
            this.isLocalDebug = true;
            Setup();
        }

        private void Setup()
        {
            if (isLocalDebug)
            {         
                var chain = new CredentialProfileStoreChain();
                AWSCredentials awsCredentials;
                if (chain.TryGetAWSCredentials(Constants.AWSProfileName, out awsCredentials))
                {
                    // use awsCredentials
                    iamClient = new AmazonIdentityManagementServiceClient(
                                        awsCredentials, Amazon.RegionEndpoint.APSouth1);
                }               
            }
            else
            {
                iamClient = new AmazonIdentityManagementServiceClient();
            }
        }

        private void ListAccessKeys(string userName, int maxItems, 
                                    List<Model.AccessKeyMetadata> AccessKeyMetadataList)
        {
            ListAccessKeysResponse accessKeysResponse = new ListAccessKeysResponse();
            var accessKeysRequest = new ListAccessKeysRequest
            {
                // Use the user created in the CreateAccessKey example
                UserName = userName,
                MaxItems = maxItems
            };
            do
            {
                accessKeysResponse = iamClient.ListAccessKeysAsync(accessKeysRequest).GetAwaiter().GetResult();
                foreach (var accessKey in accessKeysResponse.AccessKeyMetadata)
                {
                    Model.AccessKeyMetadata accesskeymetadata = new Model.AccessKeyMetadata();
                    accesskeymetadata.AccessKeyId = accessKey.AccessKeyId;
                    accesskeymetadata.CreateDate = accessKey.CreateDate.ToLongDateString();
                    accesskeymetadata.Status = accessKey.Status;
                    accesskeymetadata.UserName = accessKey.UserName;

                    GetAccessKeyLastUsedRequest request = new GetAccessKeyLastUsedRequest()
                                        { AccessKeyId = accessKey.AccessKeyId };

                    GetAccessKeyLastUsedResponse response = 
                                            iamClient.GetAccessKeyLastUsedAsync(request).GetAwaiter().GetResult();

                    accesskeymetadata.LastUsedDate = response.AccessKeyLastUsed.LastUsedDate.ToLongDateString();
                    AccessKeyMetadataList.Add(accesskeymetadata);
                }
                accessKeysRequest.Marker = accessKeysResponse.Marker;
            } while (accessKeysResponse.IsTruncated);
        }

        public APIGatewayProxyResponse ValidateAccessKey(APIGatewayProxyRequest request,
                                                        ILambdaContext context)
        {
            APIGatewayProxyResponse response = new APIGatewayProxyResponse();
            StandardErrorObject errorObj = new StandardErrorObject();
            try
            {
                AccessKeyValidatorAPIRequest requestObj = new AccessKeyValidatorAPIRequest();
                requestObj = JsonConvert.DeserializeObject<AccessKeyValidatorAPIRequest>(request.Body);                          

                string msg = string.Empty;

                if (request == null)
                {
                    context.Logger.LogLine("API Request is NULL . Terminating. \n");
                    errorObj.setError("API Request is empty");
                    msg = "API Request is empty";
                }
                else if (String.IsNullOrEmpty(request.Body))
                {
                    context.Logger.LogLine("API Request Body is NULL/Empty . Terminating. \n");
                    errorObj.setError("API Request Body is empty");
                    msg = "API Request Body is empty";
                }

                if (!String.IsNullOrEmpty(errorObj.getError()))
                {
                    response = new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Body = JsonConvert.SerializeObject(errorObj),
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }
                else
                {
                    if (String.IsNullOrEmpty(requestObj.AccessKeyID) || String.IsNullOrEmpty(requestObj.AccessKeyID))
                    {
                        context.Logger.LogLine("AccessKeyID  in request body is NULL/Empty . Terminating. \n");
                        context.Logger.LogLine("Request body => " + request.Body);
                        errorObj.setError("AccessKeyID in request body is Empty");
                        msg = "AccessKeyID in request body is Empty";

                        response = new APIGatewayProxyResponse
                        {
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Body = JsonConvert.SerializeObject(errorObj),
                            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                        };
                    }
                    else
                    {
                        ListUsersResponse allUsersListResponse = new ListUsersResponse();
                        List<Model.AccessKeyMetadata> AccessKeyMetadataList = new List<Model.AccessKeyMetadata>();

                        var userRequest = new ListUsersRequest { MaxItems = 20 } ;
                        do
                        {
                            allUsersListResponse = iamClient.ListUsersAsync(userRequest).GetAwaiter().GetResult();
                            ProcessUserDetails(allUsersListResponse, AccessKeyMetadataList);
                            userRequest.Marker = allUsersListResponse.Marker;
                        } while (allUsersListResponse.IsTruncated);                       

                        Model.AccessKeyMetadata validAcessDetails =
                            AccessKeyMetadataList.Where(x => x.AccessKeyId == requestObj.AccessKeyID).FirstOrDefault();

                        if (validAcessDetails == null)
                        {
                            errorObj.setError("Access Key Provided is not valid for this account");
                            response = new APIGatewayProxyResponse
                            {
                                StatusCode = (int)HttpStatusCode.OK,
                                Body = JsonConvert.SerializeObject(errorObj),
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }
                        else
                        {
                            response = new APIGatewayProxyResponse
                            {
                                Body = JsonConvert.SerializeObject(validAcessDetails),
                                StatusCode = (int)HttpStatusCode.OK,
                                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error . Message =  {ex.Message}");
                context.Logger.LogLine($"Error . StackTrace =  {ex.StackTrace}");
                context.Logger.LogLine($"Error . InnerException =  {ex.InnerException.Message}");

                errorObj.setError("Error in API Call . Please contact system administrator");
                response = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = JsonConvert.SerializeObject(errorObj),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            return response;
        }

        private void ProcessUserDetails(ListUsersResponse allUsersListResponse, 
                                        List<Model.AccessKeyMetadata> accessKeyMetadataList)
        {
            foreach (var user in allUsersListResponse.Users)
            {
                ListAccessKeys(user.UserName, 20, accessKeyMetadataList);
            }
        }
    }
}
