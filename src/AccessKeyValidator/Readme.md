# Serverles App for Validating AccessKeys

This is a serverless app developed on c# using AWS SDK for .NET V3

##What does it do?

It is a serverless app which exposes an API endpoint backed by a Lambda function written in c#. You can pass any 
AccessKeyID to the API and it will let you know whether that AccessKeyID is a ValidAcessKey or not in your account. If
the accesskey is valid , then it will also return some metadata about the accesskey in the response like whether it is
active or not , when was it last used , which user it is associated to and so on.

##How to get started quickly. 

This app uses the AWS SAM template . So , you can simply clone the code , and get started using a few basic commands

* Create a bucket to store the packaged code .
aws s3 mb s3://[put your bucket name here] --region [put your region here] --profile [put your profile here]
* This will validate the template for you
sam validate --template template.json --profile [put your profile here]
* This will package the code and push it to the bucket 
sam package --profile [put your profile here] --template-file template.json --output-template-file serverless-output.yaml --s3-bucket [put your bucket name here] --force-upload
* This will create a Cloudformation stack and deploy all resources to your AWS Account
sam deploy --profile [put your profile here] --template-file serverless-output.yaml --stack-name [put your CF stack name here] --capabilities CAPABILITY_IAM --region [put your region here]