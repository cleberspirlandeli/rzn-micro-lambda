using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using RznMicroLambdaUserDelete.Message;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RznMicroLambdaUserDelete
{
    public class Function
    {
        private const string TableName = "users";
        private readonly string AccessKeyId = Environment.GetEnvironmentVariable("AwsAccessKeyId");
        private readonly string AwsSecretAccessKey = Environment.GetEnvironmentVariable("AwsSecretAccessKey");
        private readonly string QueueUrl = Environment.GetEnvironmentVariable("QueueUrl");
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
            context.Logger.LogInformation($"Processed RznMicroLambdaUserDelete starting...");
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            try
            {
                var userMessage = JsonConvert.DeserializeObject<DeleteUserMessage>(message.Body);

                await DeleteUserAsync(userMessage);
                await DeleteMessageFromQueueAsync(message);
                context.Logger.LogInformation($"Processed message {message.Body}");
            }
            catch (Exception ex)
            {
                context.Logger.LogInformation($"Error processing RznMicroLambdaUserDelete the Lambda: \n {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task DeleteUserAsync(DeleteUserMessage user)
        {
            var request = new DeleteItemRequest
            {
                TableName = TableName,
                Key = new Dictionary<string, AttributeValue>
                    {
                        { "id", new AttributeValue { S = user.IdUser.ToString() } },
                    }
            };

            var response = await _amazonDynamoDBClient.DeleteItemAsync(request);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(response.ResponseMetadata.Metadata["message"]);
        }

        private async Task DeleteMessageFromQueueAsync(SQSEvent.SQSMessage message)
        {
            var deleteRequest = new DeleteMessageRequest
            {
                QueueUrl = QueueUrl,
                ReceiptHandle = message.ReceiptHandle
            };

            await _amazonSQSClient.DeleteMessageAsync(deleteRequest);
        }
    }
}