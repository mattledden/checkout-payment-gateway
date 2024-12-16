using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    public List<PostPaymentResponse> Payments = new();
    
    public void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostPaymentResponse Get(Guid id)
    {
        // throw error if payment is not found
        return Payments.FirstOrDefault(p => p.Id == id);
    }

    // need a method for processing a payment and determining the status
    // need to validate fields on payment requests (dates should be valid etc). Could have separate class
}