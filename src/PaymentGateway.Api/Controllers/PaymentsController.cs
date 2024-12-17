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
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        Console.WriteLine($"Received GET request for payment with id {id}");

        try
        {
            PostPaymentResponse payment = _paymentsRepository.Get(id);
            return new OkObjectResult(payment);
        }
        catch (PaymentNotFoundException ex)
        {
            return new NotFoundObjectResult(ex.Message);
        }
    }

    // Need a method which receives a post request to process a payment and sends back the appropriate response including the payment status

    [HttpPost("new")]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync(PostPaymentRequest paymentRequest)
    {
        Console.WriteLine("Received POST request for new payment");
        // catch custom exception and return 400 error
        try
        {
            PostPaymentResponse paymentResponse = _paymentsRepository.ProcessPayment(paymentRequest);

            return new OkObjectResult(paymentResponse);
        }
        catch (InvalidPaymentException ex)
        {
            Console.WriteLine($"{ex.Message}");
            return new BadRequestObjectResult(ex.Message);
        }
    }
}