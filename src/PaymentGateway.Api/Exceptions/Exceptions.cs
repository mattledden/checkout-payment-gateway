namespace PaymentGateway.Api.Exceptions;

public class InvalidPaymentException : Exception
{
    public InvalidPaymentException(string message) : base(message) { }
}

public class PaymentNotFoundException : Exception
{
    public PaymentNotFoundException(string message) : base(message) { }
}