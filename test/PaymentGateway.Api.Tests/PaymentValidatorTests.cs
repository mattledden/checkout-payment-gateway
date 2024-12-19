using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentValidatorTests
{
    [Fact]
    public void ValidPayment()
    {
        // Arrange
        PaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "GBP";
        request.Amount = 500;
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
        PaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2023;
        request.Currency = "GBP";
        request.Amount = 500;
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
        PaymentRequest request = new();
        request.CardNumber = "ab22405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "GBP";
        request.Amount = 500;
        request.Cvv = "456";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void ShortCardNumber()
    {
        // Arrange
        PaymentRequest request = new();
        request.CardNumber = "43248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "GBP";
        request.Amount = 500;
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
        PaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "CAD";
        request.Amount = 500;
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
        PaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "GBP";
        request.Amount = -500;
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
        PaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 5;
        request.ExpiryYear = 2027;
        request.Currency = "GBP";
        request.Amount = 500;
        request.Cvv = "45655";

        PaymentValidator validator = new(); ;

        // Act
        bool isValid = validator.Validate(request);

        // Assert
        Assert.False(isValid);
    }
}