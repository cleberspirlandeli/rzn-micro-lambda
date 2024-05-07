using Newtonsoft.Json;

namespace RznMicroLambdaUserAdd.Message;

public class UserResult
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("fullName")]
    public string FullName { get; set; }

    [JsonProperty("dateBirth")]
    public DateTime DateBirth { get; set; }

    [JsonProperty("active")]
    public bool? Active { get; set; }

    [JsonProperty("gender")]
    public int Gender { get; set; }
}
