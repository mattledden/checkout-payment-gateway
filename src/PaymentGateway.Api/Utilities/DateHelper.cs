namespace PaymentGateway.Api.Utilities;

public static class DateHelper
{
    public static DateTime FormatDate(int month, int year)
    {
        return new DateTime(year, month, DateTime.Now.Day);
    }

    public static string MonthYearToString(DateTime date)
    {
        string dateString = date.Month + "/" + date.Year;
        if (dateString.Length < 7)
        {
            // add leading zero to month if necessary
            dateString = "0" + dateString;
        }
        return dateString;
    }
}