using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class TestUtilities
{
    public static void CompareResponses(PaymentResponse expectedResponse, PaymentResponse response)
    {
        Assert.Equal(expectedResponse.Status, response.Status);
        Assert.Equal(expectedResponse.CardNumberLastFour, response.CardNumberLastFour);
        Assert.Equal(expectedResponse.ExpiryMonth, response.ExpiryMonth);
        Assert.Equal(expectedResponse.ExpiryYear, response.ExpiryYear);
        Assert.Equal(expectedResponse.Currency, response.Currency);
        Assert.Equal(expectedResponse.Amount, response.Amount);
    }
}