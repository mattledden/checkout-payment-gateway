using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Services;

public interface IBankClient
{
    Task<PaymentStatus> SendRequest(BankRequest paymentRequest);
}