# WorkdayCalculator - Facundo Canigia

The calculator will take a date (DateTime), a number of work days to add or subtract (decimal),
and return a new date that is within business hours on a work day. Work days are defined as
any day from Monday to Friday that isn’t a holiday. 

## Usage
Main app is a console application, so running it will console log the result.

Also there is a suite of tests with the provided examples, that could be updated to test with other parameters.

*One of the tests is not passing, probably due the transformations between double and decimal. Most of datetime functions only receive doubles.*

## App (.NET Core 8)
- WorkdayCalculator.Service: console app, that runs upon start and also contains the service to calculate the workdays.
- WorkdayCalculator.UnitTests: test project witn xunit and fluentassertions, that contains the test cases in the pdf file.



