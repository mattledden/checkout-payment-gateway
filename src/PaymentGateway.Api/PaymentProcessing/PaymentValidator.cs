using System.Text.RegularExpressions;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.PaymentProcessing;

public class PaymentValidator
{
    /// <summary>
    /// Validate length of card number and that it only contains numbers.
    /// Validate dates and whether they're in future.
    /// Check currency is an accepted value.
    /// Check amount > 0.
    /// Check CVV is 3-4 chars long and only numeric
    /// </summary>
    /// 
    public bool Validate(PostPaymentRequest paymentRequest)
    {
        return ValidateCardNumber(paymentRequest.CardNumber) &&
            ValidateExpiryDate(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear) &&
            ValidateCurrency(paymentRequest.Currency) &&
            ValidateAmount(paymentRequest.Amount) &&
            ValidateCvv(paymentRequest.Cvv);
    }

    private bool ValidateCardNumber(string cardNumber)
    {
        int length = cardNumber.Length;
        bool isNumeric = Regex.IsMatch(cardNumber, @"^\d+$");

        Console.WriteLine($"Card number is numeric: {isNumeric}");

        return length >= 14 && length <= 19 && isNumeric;
    }

    private bool ValidateExpiryDate(int expiryMonth, int expiryYear)
    {
        try
        {
            DateTime expiryDate = DateHelper.FormatDate(expiryMonth, expiryYear);

            return expiryDate > DateTime.Now;
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Expiry date is not a valid date");
            return false;
        }
    }

    private bool ValidateCurrency(string currency)
    {
        return currency.Equals("USD") || currency.Equals("GBP") || currency.Equals("EUR");
    }

    private bool ValidateAmount(int amount)
    {
        return amount > 0;
    }

    private bool ValidateCvv(string cvv)
    {
        int length = cvv.Length;
        bool isNumeric = Regex.IsMatch(cvv, @"^\d+$");

        return length >= 3 && length <= 4 && isNumeric;
    }
}