# Design considerations

I've decided to utilise the existing structure of the sample code (i.e. the use of a controller, service and models) then will add classes where necessary.
I plan to add a class for verifying payment requests so that this is separate from the PaymentsRepository service and can be encapsulated well.
I also plan to add a class responsible for sending requests to the acquiring bank.
I will make a class for custom exceptions like InvalidPaymentRequestException.

I may add a class containing helper functions for my unit tests.

I decided the PaymentValidator class should be added as a singleton service because it is a simple class and only one instance is required.
I decided the BankClient class should be added as a scoped service because it will contain asynchronous methods so separate requests to the PaymentGateway should use separate instances of this service. I may make an interface for this class in order to provide abstraction.

## Assumptions made

- The PostPaymentRequest model contained a field called `CardNumberLastFour`. I changed this to `CardNumber` because the spec states that payment requests should contain a full card number
- I changed the type of the `CardNumber`, `CardNumberLastFour` and `CVV` fields to be strings in case they contain leading zeros and because they are strings when sent to the bank simulator anyway.
- I left the `ExpiryMonth` and `ExpiryYear` fields as integers to simplify verifying their values.

## Process followed

1. I started by reading through the existing code in the repo and made notes on where I'd need to add methods/classes/functionality.
2. I then got the bank simulator up and running and sent test requests using Postman.
3. Next, I ran the existing unit tests in the codebase and thought about what was needed to get the second one to pass as well as potential test cases I could write.
4. I then decided which classes I was likely to add and started implementing the code to handle POST requests.
5. I added a class for custom exceptions.
