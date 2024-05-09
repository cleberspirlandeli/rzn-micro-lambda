using Newtonsoft.Json;

namespace RznMicroLambdaUserUpdate.Message;

public class AddressResult
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("idUser")]
    public Guid IdUser { get; set; }

    [JsonProperty("zipCode")]
    public string ZipCode { get; set; }

    [JsonProperty("street")]
    public string Street { get; set; }

    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("additionalInformation")]
    public string AdditionalInformation { get; set; }

    [JsonProperty("typeOfAddress")]
    public int? TypeOfAddress { get; set; }
}
