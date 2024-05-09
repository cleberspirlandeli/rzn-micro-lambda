namespace RznMicroLambdaUserDelete.Message;

public class DeleteUserMessage
{
    public Guid IdUser { get; set; }
    public Guid IdAddress { get; set; }
}
