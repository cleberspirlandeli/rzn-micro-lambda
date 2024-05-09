using Newtonsoft.Json;

namespace RznMicroLambdaUserUpdate.Message;

public class UpdateUserResult
{
    [JsonProperty("user")]
    public UserResult User { get; set; }

    [JsonProperty("address")]
    public AddressResult Address { get; set; }
}
