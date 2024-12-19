﻿using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentsRepository _paymentsRepository;

    public PaymentsController(PaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentResponse?>> GetPaymentAsync(Guid id)
    {
        Console.WriteLine($"Received GET request for payment with id {id}");

        try
        {
            PaymentResponse payment = _paymentsRepository.Get(id);
            return new OkObjectResult(payment);
        }
        catch (PaymentNotFoundException ex)
        {
            Console.WriteLine($"Failed to find payment with id {id}");
            return new NotFoundObjectResult(ex.Message);
        }
    }

    [HttpPost("new")]
    public async Task<ActionResult<PaymentResponse?>> PostPaymentAsync(PaymentRequest paymentRequest)
    {
        Console.WriteLine("Received POST request for new payment");

        PaymentResponse paymentResponse = await _paymentsRepository.ProcessPayment(paymentRequest);

        Console.WriteLine($"Sending response with status {paymentResponse.Status}");

        return paymentResponse.Status == Models.PaymentStatus.Authorized
            ? (ActionResult<PaymentResponse?>)new OkObjectResult(paymentResponse)
            : (ActionResult<PaymentResponse?>)new BadRequestObjectResult(paymentResponse);
    }
}