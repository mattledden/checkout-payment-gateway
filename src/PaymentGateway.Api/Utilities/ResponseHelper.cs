using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Utilities;

public static class ResponseHelper
{
    public static PostPaymentResponse GenerateResponse(PostPaymentRequest paymentRequest, PaymentStatus status)
    {
        string cardNumber = paymentRequest.CardNumber;
        PostPaymentResponse response = new()
        {
            Id = Guid.NewGuid(),
            Status = status,
            CardNumberLastFour = cardNumber.Substring(cardNumber.Length - 4),
            ExpiryMonth = paymentRequest.ExpiryMonth,
            ExpiryYear = paymentRequest.ExpiryYear,
            Currency = paymentRequest.Currency,
            Amount = paymentRequest.Amount
        };

        Console.WriteLine($"Response id is {response.Id}");

        return response;
    }
}