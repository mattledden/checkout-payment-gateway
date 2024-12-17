using System.Text.Json;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.PaymentProcessing;

public class BankClient
{
    private HttpClient _httpClient;
    private const string Url = "http://localhost:8080/payments";

    public BankClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<PaymentStatus> SendRequest(BankRequest paymentRequest)
    {
        // serialise request then send it to bank simulator and retrieve status from response
        string requestBody = JsonSerializer.Serialize(paymentRequest);
        StringContent requestContent = new(requestBody);

        HttpResponseMessage response = await _httpClient.PostAsync(Url, requestContent);
        string responseContent = await response.Content.ReadAsStringAsync();

        //BankResponse bankResponse = JsonSerializer.Deserialize<BankResponse>(responseContent);
        BankResponse bankResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<BankResponse>(responseContent);

        return bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}