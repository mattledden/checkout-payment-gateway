namespace PaymentGateway.Api.Utilities;

public static class DateHelper
{
    public static DateTime FormatDate(int month, int year)
    {
        return new DateTime(year, month, DateTime.Now.Day);
    }

    public static string MonthYearToString(DateTime date)
    {
        return date.Month + "/" + date.Year;
    }
}