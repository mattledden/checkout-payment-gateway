﻿using Microsoft.AspNetCore.Mvc;

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
        PostPaymentResponse payment = _paymentsRepository.Get(id);
        // return 404 if payment is not found

        return new OkObjectResult(payment);
    }

    // Need a method which receives a post request to process a payment and sends back the appropriate response including the payment status

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync(PostPaymentRequest paymentRequest)
    {
        // catch custom exception and return 400 error
        PostPaymentResponse paymentResponse = _paymentsRepository.ProcessPayment(paymentRequest);

        return new OkObjectResult(paymentResponse);
    }
}