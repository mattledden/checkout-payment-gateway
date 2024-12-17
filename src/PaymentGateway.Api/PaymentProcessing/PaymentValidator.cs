using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.PaymentProcessing;

public class PaymentValidator
{
    public bool Validate(PostPaymentRequest paymentRequest)
    {
        // validate length of card number plus only numbers, dates and whether they're in future, currency, amount > 0, CVV is 3-4 chars long and only numeric
    }
}