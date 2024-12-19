using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class BankClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private const string Url = "http://localhost:8080/payments";

    public BankClient()
    {
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Serialise request then send it to bank simulator and retrieve status from response.
    /// This method is virtual so it can be mocked in UTs.
    /// </summary>
    /// 
    public virtual async Task<PaymentStatus> SendRequest(BankRequest paymentRequest)
    {
        string requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(paymentRequest);
        StringContent requestContent = new(requestBody);

        HttpResponseMessage response = await _httpClient.PostAsync(Url, requestContent);
        string responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Response from acquiring bank: {responseContent}");

        BankResponse bankResponse = new();
        try
        {
            bankResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<BankResponse>(responseContent);
        }
        catch (Newtonsoft.Json.JsonSerializationException ex)
        {
            Console.WriteLine($"Error deserializing bank response: {ex}");
            bankResponse.Authorized = false;
        }


        return bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}