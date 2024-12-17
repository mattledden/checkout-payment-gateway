using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Models.Requests;

public class BankRequest
{
    public string Card_Number { get; set; }
    public string Expiry_Date { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string Cvv { get; set; }

    public BankRequest(PostPaymentRequest paymentRequest)
    {
        DateTime expiryDate = DateHelper.FormatDate(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear);
        string expiryDateString = DateHelper.MonthYearToString(expiryDate);

        Card_Number = paymentRequest.CardNumber;
        Expiry_Date = expiryDateString;
        Currency = paymentRequest.Currency;
        Amount = paymentRequest.Amount;
        Cvv = paymentRequest.Cvv;
    }
}