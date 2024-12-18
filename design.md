# Design considerations

I've decided to utilise the existing structure of the sample code (i.e. the use of a controller, service and models) then will add classes where necessary.
I plan to add a class for verifying payment requests so that this is separate from the PaymentsRepository service and can be encapsulated well.
I also plan to add a class responsible for sending requests to the acquiring bank.
I will make a class for custom exceptions like `InvalidPaymentException`.

I may add a class containing helper functions for my unit tests.

Both the `PaymentValidator` and `BankClient` classes needed to be added as singleton services because the `PaymentsRepository` class depends on them and that class is a singleton.
`BankClient` implements an interface (`IBankClient`) so it can be mocked in my unit tests.

I made a `DateHelper` class to contain utility methods because I will need to interact with dates in multiple classes.

I created a `BankRequest` model so the `CardNumber` field could be renamed to `Card_Number` so it would be accepted by the bank simulator and also so I could add the `Expiry_Date` field as a combination of `ExpiryMonth` and `ExpiryYear`.

## Assumptions made

- The `PostPaymentRequest` model contained a field called `CardNumberLastFour`. I changed this to `CardNumber` because the spec states that payment requests should contain a full card number. I renamed `PostPaymentRequest` to `PaymentRequest` to be consistent with `PaymentResponse`.
- I changed the type of the `CardNumber`, `CardNumberLastFour` and `CVV` fields to be strings in case they contain leading zeros and because they are strings when sent to the bank simulator.
- I left the `ExpiryMonth` and `ExpiryYear` fields as integers to simplify verifying their values.
- I deleted the `GetPaymentResponse` model because it is identical to `PostPaymentResponse` and the sample code already uses `PostPaymentResponse` for GET requests. I could have changed this to use `GetPaymentResponse` instead but decided it was unnecessary because the two models require the same fields. I renamed the remaining object to `PaymentResponse` to reflect this choice.

## Process followed

1. I started by reading through the existing code in the repo and made notes on where I'd need to add methods/classes/functionality.
2. I then got the bank simulator up and running and sent test requests using Postman.
3. Next, I ran the existing unit tests in the codebase and thought about what was needed to get the second one to pass as well as potential test cases I could write.
4. I then decided which classes I was likely to add and started implementing the code to handle POST requests.
5. I added a class for custom exceptions.
6. I implemented the class which validates payments then tested it using cURL and some basic unit tests which I will improve once more functionality has been added.
7. I then added the functionality for sending requests to the acquiring bank. 
8. Finally, I added more unit tests and refactored my code.

## Potential improvements

- Could add more custom exceptions e.g. to specify which field made the payment invalid.
- The SwaggerUI package being depended on has a vulnerability so should be updated.
- Use a logger (via dependency injection) rather than Console.WriteLine.
- Better way of testing objects returned by POST requests- need to mock out Guid generation. Also, would be better to deserialise the returned object rather than comparing the strings.
- I could make a wrapper class and interface for `HttpClient` in order to mock it when testing the `BankClient` class.
- Could revert to using `GetPaymentResponse` and make the fields on there readonly.
- Could add more unit tests for `PaymentValidator` to cover edge cases