using Newtonsoft.Json;

namespace RznMicroLambdaUserAdd.Message;

public class AddUserMessage
{
    [JsonProperty("user")]
    public AddUserResult User { get; set; }
}
