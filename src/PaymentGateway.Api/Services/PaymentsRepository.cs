using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Utilities;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository
{
    private List<PaymentResponse> Payments = new();
    private readonly PaymentValidator _paymentValidator;
    private readonly IBankClient _bankClient;

    public PaymentsRepository(PaymentValidator paymentValidator, IBankClient bankClient)
    {
        _paymentValidator = paymentValidator;
        _bankClient = bankClient;
    }

    public void Add(PaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PaymentResponse Get(Guid id)
    {
        // throw error if payment is not found
        PaymentResponse? paymentResponse = Payments.FirstOrDefault(p => p.Id == id);
        return paymentResponse is not null ? paymentResponse : throw new PaymentNotFoundException($"Payment with id {id} not found");
    }

    /// <summary>
    /// Validate payments then send them to the acquiring bank
    /// </summary>
    /// 
    public async Task<PaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
    {
        bool isValid = _paymentValidator.Validate(paymentRequest);
        PaymentStatus status;

        if (!isValid)
        {
            Console.WriteLine("Payment is invalid");
            status = PaymentStatus.Rejected;
        }
        else
        {
            Console.WriteLine("Payment is valid");

            // make object to send to bank
            BankRequest bankRequest = new(paymentRequest);

            // send request to bank
            status = await _bankClient.SendRequest(bankRequest);
        }

        // create response object and add it to Payments list then return it
        PaymentResponse paymentResponse = ResponseHelper.GenerateResponse(paymentRequest, status);
        Add(paymentResponse);

        return paymentResponse;
    }
}