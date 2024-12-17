using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.PaymentProcessing;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    private List<PostPaymentResponse> Payments = new();
    private PaymentValidator _paymentValidator;
    private BankClient _bankClient;

    public PaymentsRepository(PaymentValidator paymentValidator, BankClient bankClient)
    {
        _paymentValidator = paymentValidator;
        _bankClient = bankClient;
    }

    public void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostPaymentResponse Get(Guid id)
    {
        // throw error if payment is not found
        PostPaymentResponse? paymentResponse = Payments.FirstOrDefault(p => p.Id == id);
        return paymentResponse is not null ? paymentResponse : throw new PaymentNotFoundException($"Payment with id {id} not found");
    }

    // need a method for processing a payment and determining the status
    // need to validate fields on payment requests (dates should be valid etc). Could have separate class

    public PostPaymentResponse ProcessPayment(PostPaymentRequest paymentRequest)
    {
        bool isValid = _paymentValidator.Validate(paymentRequest);

        if (!isValid)
        {
            throw new InvalidPaymentException("Payment request is invalid");

        }

        // send request to bank
        PaymentStatus bankResponse = _bankClient.SendRequest();
        // create response object and add it to Payments list then return it
    }
}