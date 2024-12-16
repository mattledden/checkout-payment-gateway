# Design considerations

## Assumptions made

- The PostPaymentRequest model contained a field called `CardNumberLastFour`. I changed this to `CardNumber` because the spec states that payment requests should contain a full card number

## Process followed

1. I started by reading through the existing code in the repo and made notes on where I'd need to add methods/functionality.
2. I then got the bank simulator up and running and sent test requests using Postman.
3. Next, I ran the existing unit tests in the codebase and thought about what was needed to get the second one to pass as well as potential test cases I could write.
