using System.Text.RegularExpressions;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Services;

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
    public bool Validate(PaymentRequest paymentRequest)
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

        bool isValid = length >= 14 && length <= 19 && isNumeric;
        Console.WriteLine($"Card number {cardNumber} is valid: {isValid}");

        return isValid;
    }

    private bool ValidateExpiryDate(int expiryMonth, int expiryYear)
    {
        try
        {
            DateTime expiryDate = DateHelper.FormatDate(expiryMonth, expiryYear);
            bool isValid = expiryDate > DateTime.Now;

            Console.WriteLine($"Expiry date {expiryDate} is valid: {isValid}");

            return isValid;
        }
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Expiry date is not a valid date");
            return false;
        }
    }

    private bool ValidateCurrency(string currency)
    {
        bool isValid = currency.Equals("USD") || currency.Equals("GBP") || currency.Equals("EUR");
        Console.WriteLine($"Currency {currency} is valid: {isValid}");

        return isValid;
    }

    private bool ValidateAmount(int amount)
    {
        bool isValid = amount > 0;
        Console.WriteLine($"Amount {amount} is valid: {isValid}");

        return isValid;
    }

    private bool ValidateCvv(string cvv)
    {
        int length = cvv.Length;
        bool isNumeric = Regex.IsMatch(cvv, @"^\d+$");

        bool isValid = length >= 3 && length <= 4 && isNumeric;
        Console.WriteLine($"Cvv {cvv} is valid: {isValid}");

        return isValid;
    }
}