using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.PaymentProcessing;
using PaymentGateway.Api.Utilities;

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

    private void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    private PostPaymentResponse GenerateResponse(PostPaymentRequest paymentRequest, PaymentStatus status)
    {
        // might need tostring method in datehelper class
        DateTime expiryDate = DateHelper.FormatDate(paymentRequest.ExpiryMonth, paymentRequest.ExpiryYear);
        string expiryDateString = DateHelper.MonthYearToString(expiryDate);
        if (expiryDateString.Length < 7)
        {
            // add leading zero to month if necessary
            expiryDateString = "0" + expiryDateString;
        }
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
        PostPaymentResponse paymentResponse = GenerateResponse(paymentRequest, bankResponse);
        Add(paymentResponse);

        return paymentResponse;
    }
}