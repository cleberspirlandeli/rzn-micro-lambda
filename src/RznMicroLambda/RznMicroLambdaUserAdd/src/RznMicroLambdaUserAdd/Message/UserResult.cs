namespace RznMicroLambdaUserAdd.Message;

public class UserResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateBirth { get; set; }
    public bool? Active { get; set; }
    public int Gender { get; set; }
}
