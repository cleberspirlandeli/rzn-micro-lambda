using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;
using static Amazon.Lambda.SQSEvents.SQSEvent;
using Amazon;
using Amazon.Runtime.Internal.Settings;
using RznMicroLambdaUserAdd.Schema;
using RznMicroLambdaUserAdd.Message;
using System;
using System.Reflection.Emit;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RznMicroLambdaUserAdd
{
    public class Function
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {

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
            foreach (var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            try
            {


                context.Logger.LogInformation($"Processed message {message.Body}");

                var _amazonDynamoDBClient = new AmazonDynamoDBClient("", "", RegionEndpoint.USEast1);
                var _dynamoDBContext = new DynamoDBContext(_amazonDynamoDBClient);

                var userMessage = JsonConvert.DeserializeObject<AddUserMessage>(message.Body);

                var user = new UserSchema
                {
                    // User
                    IdUser = userMessage.User.User.Id.ToString(),
                    FullName = userMessage.User.User.FullName,
                    FullNameSearch = userMessage.User.User.FullName.ToUpper(),
                    DateBirth = userMessage.User.User.DateBirth.ToString(),
                    Active = userMessage.User.User.Active,
                    Gender = userMessage.User.User.Gender,

                    // Address
                    IdAddress = userMessage.User.Address.Id.ToString(),
                    ZipCode = userMessage.User.Address.ZipCode,
                    Street = userMessage.User.Address.Street,
                    Number = userMessage.User.Address.Number,
                    AdditionalInformation = userMessage.User.Address.AdditionalInformation,
                    TypeOfAddress = userMessage.User.Address.TypeOfAddress,
                };

                await _dynamoDBContext.SaveAsync(user);
            }
            catch (Exception ex)
            {

                throw;
            }

            await Task.CompletedTask;
        }
    }
}