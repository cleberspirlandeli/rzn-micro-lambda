using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using RznMicroLambdaUserAdd.Message;
using RznMicroLambdaUserAdd.Schema;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RznMicroLambdaUserAdd
{
    public class Function
    {
        private readonly string AccessKeyId = Environment.GetEnvironmentVariable("AwsAccessKeyId");
        private readonly string AwsSecretAccessKey = Environment.GetEnvironmentVariable("AwsSecretAccessKey");
        private readonly BasicAWSCredentials _credentials;

        // DynamoDB
        private readonly AmazonDynamoDBClient _amazonDynamoDBClient;
        private readonly DynamoDBContext _dynamoDBContext;

        // SQS
        private readonly AmazonSQSClient _amazonSQSClient;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            _credentials = new BasicAWSCredentials(AccessKeyId, AwsSecretAccessKey);

            _amazonDynamoDBClient = new AmazonDynamoDBClient(_credentials, RegionEndpoint.USEast1);
            _dynamoDBContext = new DynamoDBContext(_amazonDynamoDBClient);

            _amazonSQSClient = new AmazonSQSClient(_credentials, RegionEndpoint.USEast1);
        }


        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt">The event for the Lambda function handler to process.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            context.Logger.LogInformation($"Processed RznMicroLambdaUserAdd starting...");
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            try
            {
                var userMessage = JsonConvert.DeserializeObject<AddUserMessage>(message.Body);

                var user = new UserSchema
                {
                    // User
                    IdUser = userMessage.User.User.Id.ToString(),
                    FullName = userMessage.User.User.FullName,
                    FullNameSearch = userMessage.User.User.FullName.ToUpper(),
                    DateBirth = userMessage.User.User.DateBirth.ToString("yyyy/MM/dd"),
                    Active = userMessage.User.User.Active ?? false,
                    Gender = userMessage.User.User.Gender,

                    // Address
                    IdAddress = userMessage.User.Address.Id.ToString(),
                    ZipCode = userMessage.User.Address.ZipCode,
                    Street = userMessage.User.Address.Street,
                    Number = userMessage.User.Address.Number,
                    AdditionalInformation = userMessage.User.Address.AdditionalInformation,
                    TypeOfAddress = userMessage.User.Address.TypeOfAddress,
                };

                await AddUserAsync(user);
                await DeleteMessageFromQueueAsync(message);
                context.Logger.LogInformation($"Processed message {message.Body}");
            }
            catch (Exception ex)
            {
                context.Logger.LogInformation($"Error processing RznMicroLambdaUserAdd the Lambda: \n {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task AddUserAsync(UserSchema user)
        {
            await _dynamoDBContext.SaveAsync(user);
        }

        private async Task DeleteMessageFromQueueAsync(SQSEvent.SQSMessage message)
        {
            var queueUrl = await GetQueueUrlFromArnAsync(message.EventSourceArn);

            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = queueUrl,
                ReceiptHandle = message.ReceiptHandle
            };

            await _amazonSQSClient.DeleteMessageAsync(deleteRequest);
        }

        private async Task<string> GetQueueUrlFromArnAsync(string eventSourceArn)
        {
            string[] arnParts = eventSourceArn.Split(':');

            var queueName = arnParts.Last();
            var response = await _amazonSQSClient.GetQueueUrlAsync(queueName);

            return response.QueueUrl;
        }
    }
}
/*
 "{\"User\":{\"User\":{\"Id\":\"50d60674-bec2-4041-8579-55bf4e4dd0a0\",\"FullName\":\"Teste SQS Lambda\",\"DateBirth\":\"1992-12-18T00:00:00\",\"Active\":true,\"Gender\":0},\"Address\":{\"Id\":\"248061e9-8f25-4ee7-a1e6-0476d2d9ed4f\",\"IdUser\":\"50d60674-bec2-4041-8579-55bf4e4dd0a0\",\"ZipCode\":\"14412444\",\"Street\":\"Rua teste SQS Lambda NOVO\",\"Number\":123,\"AdditionalInformation\":\"Add info SQS Lambda\",\"TypeOfAddress\":0}}}"
 "eventSourceARN": "arn:aws:sqs:us-east-1:905418045759:rznapps-micro-sqs-dev-user",

{
  "Records": [
    {
      "messageId": "19dd0b57-b21e-4ac1-bd88-01bbb068cb78",
      "receiptHandle": "ABCDEF123456789+CKfAk9T4eCM8+Lkdfk5O/Bb5uJOWZdakf/sxfStK7EUr7c5vDW7ZTnW7b3BxXz11jPd9oiJkHGRxuf0C7NwtDDjZUz11WnOupknY/NuZQ9c2zW7VtxJgZGTfUIzLFrblcdJC9pBpdlO9qoebhO7Dw",
      "body": "{\"User\":{\"User\":{\"Id\":\"50d60674-bec2-4041-8579-55bf4e4dd0a0\",\"FullName\":\"Teste SQS Lambda\",\"DateBirth\":\"1992-12-18T00:00:00\",\"Active\":true,\"Gender\":0},\"Address\":{\"Id\":\"248061e9-8f25-4ee7-a1e6-0476d2d9ed4f\",\"IdUser\":\"50d60674-bec2-4041-8579-55bf4e4dd0a0\",\"ZipCode\":\"14412444\",\"Street\":\"Rua teste SQS Lambda NOVO\",\"Number\":123,\"AdditionalInformation\":\"Add info SQS Lambda\",\"TypeOfAddress\":0}}}",
      "attributes": {
        "ApproximateReceiveCount": "1",
        "SentTimestamp": "1523232000000",
        "SenderId": "123456789012",
        "ApproximateFirstReceiveTimestamp": "1523232000001"
      },
      "messageAttributes": {},
      "md5OfBody": "{{{md5_of_body}}}",
      "eventSource": "aws:sqs",
      "eventSourceARN": "arn:aws:sqs:us-east-1:905418045759:rznapps-micro-sqs-dev-user",
      "awsRegion": "us-east-1"
    }
  ]
}
 */