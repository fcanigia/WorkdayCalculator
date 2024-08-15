using WorkdayCalculator.Service.Interfaces;

namespace WorkdayCalculator.Service.Services;

public class WorkdayCalendar : IWorkdayCalendar
{
    private TimeSpan _workdayStart;
    private TimeSpan _workdayEnd;
    private readonly HashSet<DateTime> _holidays = [];
    private readonly HashSet<DateTime> _recurringHolidays = [];

    public void SetHoliday(DateTime date)
    {
        _holidays.Add(date.Date);
    }

    public void SetRecurringHoliday(int month, int day)
    {
        if (!IsValidDate(month, day))
        {
            throw new ArgumentException("Invalid recurring holiday date");
        }
        
        _recurringHolidays.Add(new DateTime(1, month, day));
    }

    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        if (startHours < 0 && startHours > 12)
        {
            throw new ArgumentException("Invalid start hour");
        }

        if (startMinutes < 0 && startMinutes > 59)
        {
            throw new ArgumentException("Invalid start minutes");
        }

        if (stopHours < 0 && stopHours > 12)
        {
            throw new ArgumentException("Invalid stop hours");
        }

        if (stopMinutes < 0 && stopMinutes > 59)
        {
            throw new ArgumentException("Invalid stop minutes");
        }

        _workdayStart = new TimeSpan(startHours, startMinutes, 0);
        _workdayEnd = new TimeSpan(stopHours, stopMinutes, 0);
    }

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        if (_workdayEnd == TimeSpan.Zero) { throw new Exception("There is no workday end"); }
        if (_workdayStart == TimeSpan.Zero) { throw new Exception("There is no workday start"); }

        var currentDate = AdjustToWorkday(startDate);
        var workdayLength = (_workdayEnd - _workdayStart).TotalHours;
        var direction = incrementInWorkdays >= 0 ? 1 : -1;
        var remainingWorkdays = Math.Abs(incrementInWorkdays);

        while (remainingWorkdays > 0)
        {
            var timeRemainingToday = direction > 0
                ? (_workdayEnd - currentDate.TimeOfDay).TotalHours
                : (currentDate.TimeOfDay - _workdayStart).TotalHours;

            var workdaysInCurrentDay = timeRemainingToday / workdayLength;

            if (remainingWorkdays <= (decimal)workdaysInCurrentDay)
            {
                currentDate = currentDate.AddHours((double)remainingWorkdays * workdayLength * direction);
                remainingWorkdays = 0;
            }
            else
            {
                currentDate = direction > 0
                    ? currentDate.Date.AddDays(1).Add(_workdayStart)
                    : currentDate.Date.AddDays(-1).Add(_workdayEnd);
                remainingWorkdays -= (decimal)workdaysInCurrentDay;
            }

            currentDate = AdjustToWorkday(currentDate, direction);
        }

        return currentDate;
    }

    private DateTime AdjustToWorkday(DateTime date, int direction = 1)
    {
        while (!IsWorkday(date))
        {
            date = direction > 0
                ? date.Date.AddDays(1).Add(_workdayStart)
                : date.Date.AddDays(-1).Add(_workdayEnd);
        }

        if (date.TimeOfDay < _workdayStart)
        {
            date = date.Date.Add(_workdayStart);
        }
        else if (date.TimeOfDay > _workdayEnd)
        {
            date = date.Date.AddDays(direction > 0 ? 1 : -1).Add(_workdayStart);
        }

        return date;
    }

    private bool IsWorkday(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        if (_holidays.Contains(date.Date))
            return false;

        if (_recurringHolidays.Contains(new DateTime(1, date.Month, date.Day)))
            return false;

        return true;
    }

    public static bool IsValidDate(int month, int day)
    {
        try
        {
            DateTime date = new(1, month, day);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }
}
