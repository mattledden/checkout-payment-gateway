using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.PaymentProcessing;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();

    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        PostPaymentResponse payment = new()
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP"
        };

        BankClient bankClient = new();
        PaymentValidator paymentValidator = new();

        PaymentsRepository paymentsRepository = new(paymentValidator, bankClient);
        paymentsRepository.Add(payment);

        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton(paymentsRepository)))
            .CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/Payments/{payment.Id}");
        PostPaymentResponse? paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // need tests for requests to process a payment. have different tests which expect different payment statuses
    // also test failure cases e.g. invalid request bodies

    [Fact]
    public async Task PostValidPayment()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();
        string url = $"/api/Payments/new";

        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 11;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "456";

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync(url, content);

        // Assert
        // Need to check the response is as expected. Also status code will be Ok not NoContent once implementation is completed
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PostInvalidDate()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();
        string url = $"/api/Payments/new";

        PostPaymentRequest request = new();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 13;
        request.ExpiryYear = 2026;
        request.Currency = "USD";
        request.Amount = 60000;
        request.Cvv = "456";

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync(url, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}