using Newtonsoft.Json;

namespace RznMicroLambdaUserUpdate.Message;

public class UpdateUserMessage
{
    [JsonProperty("user")]
    public UpdateUserResult User { get; set; }
}
