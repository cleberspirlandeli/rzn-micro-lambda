using Newtonsoft.Json;

namespace RznMicroLambdaUserAdd.Message;

public class AddUserResult
{
    [JsonProperty("user")]
    public UserResult User { get; set; }

    [JsonProperty("address")]
    public AddressResult Address { get; set; }
}
