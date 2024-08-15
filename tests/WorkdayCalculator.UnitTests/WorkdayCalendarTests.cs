using FluentAssertions;
using WorkdayCalculator.Service.Services;

namespace WorkdayCalculator.UnitTests;

public class WorkdayCalendarTests
{
    private static readonly string _dateFormat = "dd-MM-yyyy HH:mm";

    [Fact]
    public void TestBasicIncrement_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);

        var start = new DateTime(2024, 8, 12, 10, 0, 0);
        decimal increment = 1m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.Should().Be(new DateTime(2024, 8, 13, 10, 0, 0));
    }

    [Fact]
    public void TestBasicIncrement_NoStartStop_Error()
    {
        var calendar = new WorkdayCalendar();

        var start = new DateTime(2024, 8, 12, 10, 0, 0);
        decimal increment = 1m;

        Action act = () => calendar.GetWorkdayIncrement(start, increment);

        act.Should().Throw<Exception>().WithMessage("There is no workday end");
    }

    [Fact]
    public void TestBasicIncrement_InvalidRecurringHolidayDate_Error()
    {
        var calendar = new WorkdayCalendar();

        var start = new DateTime(2024, 8, 12, 10, 0, 0);

        Action act = () => calendar.SetRecurringHoliday(2, 31);

        act.Should().Throw<ArgumentException>().WithMessage("Invalid recurring holiday date");
    }

    [Fact]
    public void TestExample1_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));
        
        var start = new DateTime(2004, 5, 24, 18, 5, 0);
        decimal increment = -5.5m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.ToString(_dateFormat).Should().Be("14-05-2004 12:00");
    }

    [Fact]
    public void TestExample2_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 19, 3, 0);
        decimal increment = 44.723656m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.ToString(_dateFormat).Should().Be("27-07-2004 13:47");
    }

    [Fact]
    public void TestExample3_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 18, 3, 0);
        decimal increment = -6.7470217m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.ToString(_dateFormat).Should().Be("13-05-2004 10:02"); // failing
    }

    [Fact]
    public void TestExample4_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 08, 3, 0);
        decimal increment = 12.782709m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.ToString(_dateFormat).Should().Be("10-06-2004 14:18");
    }

    [Fact]
    public void TestExample5_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 07, 3, 0);
        decimal increment = 8.276628m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.ToString(_dateFormat).Should().Be("04-06-2004 10:12");
    }

    [Fact]
    public void TestHolidayAdjustment_Ok()
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetHoliday(new DateTime(2024, 8, 13));

        var start = new DateTime(2024, 8, 12, 10, 0, 0);
        decimal increment = 1m;
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        incrementedDate.Should().Be(new DateTime(2024, 8, 14, 10, 0, 0)); 
    }
}