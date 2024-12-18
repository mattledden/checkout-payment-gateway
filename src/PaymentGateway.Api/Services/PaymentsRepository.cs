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
    private IBankClient _bankClient;

    public PaymentsRepository(PaymentValidator paymentValidator, IBankClient bankClient)
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
        PostPaymentResponse paymentResponse = ResponseHelper.GenerateResponse(paymentRequest, status);
        Add(paymentResponse);

        return paymentResponse;
    }
}