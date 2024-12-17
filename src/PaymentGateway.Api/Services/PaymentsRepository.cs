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

    private PostPaymentResponse GenerateResponse(PostPaymentRequest paymentRequest, PaymentStatus status)
    {
        PostPaymentResponse response = new();
        response.Id = Guid.NewGuid();
        response.Status = status;

        string cardNumber = paymentRequest.CardNumber;
        response.CardNumberLastFour = cardNumber.Substring(cardNumber.Length - 4);
        response.ExpiryMonth = paymentRequest.ExpiryMonth;
        response.ExpiryYear = paymentRequest.ExpiryYear;
        response.Currency = paymentRequest.Currency;
        response.Amount = paymentRequest.Amount;

        return response;
    }

    public PostPaymentResponse Get(Guid id)
    {
        // throw error if payment is not found
        PostPaymentResponse? paymentResponse = Payments.FirstOrDefault(p => p.Id == id);
        return paymentResponse is not null ? paymentResponse : throw new PaymentNotFoundException($"Payment with id {id} not found");
    }

    // need a method for processing a payment and determining the status
    // need to validate fields on payment requests (dates should be valid etc). Could have separate class

    public async Task<PostPaymentResponse> ProcessPayment(PostPaymentRequest paymentRequest)
    {
        bool isValid = _paymentValidator.Validate(paymentRequest);
        PaymentStatus status;

        if (!isValid)
        {
            status = PaymentStatus.Rejected;
        }
        else
        {
            // make object to send to bank
            BankRequest bankRequest = new(paymentRequest);

            // send request to bank
            status = await _bankClient.SendRequest(bankRequest);
        }

        // create response object and add it to Payments list then return it
        PostPaymentResponse paymentResponse = GenerateResponse(paymentRequest, status);
        Add(paymentResponse);

        return paymentResponse;
    }
}