using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.PaymentProcessing;

public interface IBankClient
{
    Task<PaymentStatus> SendRequest(BankRequest paymentRequest);
}