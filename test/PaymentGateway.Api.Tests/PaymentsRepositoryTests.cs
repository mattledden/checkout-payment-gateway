using NSubstitute;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Tests;

public class PaymentsRepositoryTests
{

    /// <summary>
    /// Make a payment which is authorized by the acquiring bank
    /// </summary>
    /// 
    [Fact]
    public async Task AuthorizedPayment()
    {
        // Arrange
        PaymentRequest request = new()
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 11,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 60000,
            Cvv = "456",
        };

        PaymentResponse expectedResponse = ResponseHelper.GenerateResponse(request, PaymentStatus.Authorized);

        PaymentValidator paymentValidator = new();

        // mock bankclient to return Authorized status
        IBankClient mockedBank = Substitute.For<BankClient>();
        mockedBank.SendRequest(Arg.Any<BankRequest>()).Returns(PaymentStatus.Authorized);

        PaymentsRepository paymentsRepository = new(paymentValidator, mockedBank);

        // Act
        PaymentResponse response = await paymentsRepository.ProcessPayment(request);

        // Assert
        Assert.NotNull(response);

        TestUtilities.CompareResponses(expectedResponse, response);
    }

    /// <summary>
    /// Make a payment which is declined by the acquiring bank
    /// </summary>
    /// 
    [Fact]
    public async Task DeclinedPayment()
    {
        // Arrange
        PaymentRequest request = new()
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 11,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 8888888,
            Cvv = "999",
        };

        PaymentResponse expectedResponse = ResponseHelper.GenerateResponse(request, PaymentStatus.Declined);

        PaymentValidator paymentValidator = new();

        // mock bankclient to return Declined status
        IBankClient mockedBank = Substitute.For<BankClient>();
        mockedBank.SendRequest(Arg.Any<BankRequest>()).Returns(PaymentStatus.Declined);

        PaymentsRepository paymentsRepository = new(paymentValidator, mockedBank);

        // Act
        PaymentResponse response = await paymentsRepository.ProcessPayment(request);

        // Assert
        Assert.NotNull(response);

        TestUtilities.CompareResponses(expectedResponse, response);
    }

    /// <summary>
    /// Make a payment which is invalid so no request is sent to the acquiring bank
    /// </summary>
    /// 
    [Fact]
    public async Task InvalidPayment()
    {
        // Arrange
        PaymentRequest request = new()
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 11,
            ExpiryYear = 2022,
            Currency = "USD",
            Amount = 8888888,
            Cvv = "999",
        };

        PaymentResponse expectedResponse = ResponseHelper.GenerateResponse(request, PaymentStatus.Rejected);

        PaymentValidator paymentValidator = new();

        // mock bankclient although it won't be contacted
        IBankClient mockedBank = Substitute.For<BankClient>();

        PaymentsRepository paymentsRepository = new(paymentValidator, mockedBank);

        // Act
        PaymentResponse response = await paymentsRepository.ProcessPayment(request);

        // Assert
        Assert.NotNull(response);

        TestUtilities.CompareResponses(expectedResponse, response);
    }
}