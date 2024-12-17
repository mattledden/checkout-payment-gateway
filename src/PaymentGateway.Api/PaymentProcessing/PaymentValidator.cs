using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.PaymentProcessing;

public class PaymentValidator
{
    private readonly ILogger _logger;

    public PaymentValidator(ILogger logger)
    {
        _logger = logger;
    }

    public bool Validate(PostPaymentRequest paymentRequest)
    {
        // validate length of card number plus only numbers, dates and whether they're in future, currency, amount > 0, CVV is 3-4 chars long and only numeric
        return ValidateCardNumber(paymentRequest.CardNumber) &&
            ValidateExpiryDate(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear) &&
            ValidateCurrency(paymentRequest.Currency) &&
            ValidateAmount(paymentRequest.Amount) &&
            ValidateCvv(paymentRequest.Cvv);
    }

    private bool ValidateCardNumber(string cardNumber)
    {
        int length = cardNumber.Length;
        int num = 0;
        bool isNumeric = int.TryParse(cardNumber, out num);

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
            // log error
            _logger.LogError("Expiry date is not a valid date");
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
        int num = 0;
        bool isNumeric = int.TryParse(cvv, out num);

        return length >= 3 && length <= 4 && isNumeric;
    }
}