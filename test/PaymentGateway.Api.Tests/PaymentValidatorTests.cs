using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.PaymentProcessing;

namespace PaymentGateway.Api.Tests;

public class PaymentValidatorTests
{
    [Fact]
    public void ValidPayment()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void PastDate()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2023;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void NonNumericCardNumber()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "ab22405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void InvalidCurrency()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "CAD";
        request.Amount = 60000;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void NegativeAmount()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = -60000;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void InvalidCvv()
    {
        // Arrange
        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "45655";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }
}