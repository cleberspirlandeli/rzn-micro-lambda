using Newtonsoft.Json;

namespace RznMicroLambdaUserAdd.Message;

public class TestMessage
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("info")]
    public string Info { get; set; }
}
