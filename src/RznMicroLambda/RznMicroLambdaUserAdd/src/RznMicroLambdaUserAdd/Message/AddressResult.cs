namespace RznMicroLambdaUserAdd.Message;

public class AddressResult
{
    public Guid Id { get; set; }
    public Guid IdUser { get; set; }
    public string ZipCode { get; set; }
    public string Street { get; set; }
    public int Number { get; set; }
    public string AdditionalInformation { get; set; }
    public int? TypeOfAddress { get; set; }
}
