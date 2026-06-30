namespace TraineeManagement.Api.CustomExceptions;

public class PayloadTooLargeException : Exception
{
    public PayloadTooLargeException(string message) : base(message) { }
}
