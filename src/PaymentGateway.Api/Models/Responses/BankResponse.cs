namespace PaymentGateway.Api.Models.Responses;

public class BankResponse
{
    public bool Authorized { get; set; }
    public Guid Authorization_Code { get; set; }
}