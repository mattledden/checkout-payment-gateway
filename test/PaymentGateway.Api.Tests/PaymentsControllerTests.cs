using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.PaymentProcessing;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();

    /// <summary>
    /// Retrieve a previously made payment
    /// </summary>
    ///
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

    /// <summary>
    /// Attempt to retrieve a payment which doesn't exist
    /// </summary>
    /// 
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

    /// <summary>
    /// Attempt to make a payment with an invalid expiry date.
    /// Should receive a 400 response with Rejected status.
    /// </summary>
    /// 
    [Fact]
    public async Task PostInvalidDate()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();
        string url = $"/api/Payments/new";

        PostPaymentRequest request = new()
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 13,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 60000,
            Cvv = "456"
        };

        PostPaymentResponse expectedResponse = ResponseHelper.GenerateResponse(request, PaymentStatus.Rejected);

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage response = await client.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        PostPaymentResponse actualResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PostPaymentResponse>(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(actualResponse);

        TestUtilities.CompareResponses(expectedResponse, actualResponse);
    }

    /// <summary>
    /// Attempt to make a payment which is declined by the acquiring bank.
    /// Should receive a 400 response with Declined status.
    /// </summary>
    /// 
    [Fact]
    public async Task PostUnsupportedPayment()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();
        string url = $"/api/Payments/new";

        PostPaymentRequest request = new()
        {
            CardNumber = "1234567890987654",
            ExpiryMonth = 2,
            ExpiryYear = 2027,
            Currency = "USD",
            Amount = 4300,
            Cvv = "623"
        };

        JsonContent content = JsonContent.Create(request);

        PostPaymentResponse expectedResponse = ResponseHelper.GenerateResponse(request, PaymentStatus.Declined);

        // Act
        HttpResponseMessage response = await client.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        PostPaymentResponse actualResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PostPaymentResponse>(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(actualResponse);

        TestUtilities.CompareResponses(expectedResponse, actualResponse);
    }

    /// <summary>
    /// Retrieve a previously declined payment.
    /// Should receive a 200 response containing the same object received when making the payment.
    /// </summary>
    /// 
    [Fact]
    public async Task PostAndRetrieveDeclinedPayment()
    {
        // Arrange
        WebApplicationFactory<PaymentsController> webApplicationFactory = new();
        HttpClient client = webApplicationFactory.CreateClient();
        string url = $"/api/Payments/new";

        PostPaymentRequest request = new()
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 11,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 60000,
            Cvv = "456",
        };

        JsonContent content = JsonContent.Create(request);

        // Act
        HttpResponseMessage postResponseMessage = await client.PostAsync(url, content);
        string postResponseBody = await postResponseMessage.Content.ReadAsStringAsync();
        PostPaymentResponse postResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PostPaymentResponse>(postResponseBody);

        string getUrl = "/api/Payments/" + postResponse.Id;

        HttpResponseMessage getResponseMessage = await client.GetAsync(getUrl);
        string getResponseBody = await getResponseMessage.Content.ReadAsStringAsync();
        PostPaymentResponse getResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PostPaymentResponse>(getResponseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponseMessage.StatusCode);
        Assert.NotNull(getResponse);

        // check the response from the GET request is the same as the one returned by the POST request
        TestUtilities.CompareResponses(postResponse, getResponse);
    }
}