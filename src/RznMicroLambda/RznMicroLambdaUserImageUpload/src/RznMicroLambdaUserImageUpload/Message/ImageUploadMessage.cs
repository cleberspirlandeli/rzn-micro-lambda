namespace RznMicroLambdaUserImageUpload.Message;

public class ImageUploadMessage
{
    public Guid IdUser { get; set; }
    public string Url { get; set; }
    public string ImageKey { get; set; }
}
