using System;
using System.Collections.Generic;
using FarsiLibrary.Utils.Formatter.TimeUnits;

namespace FarsiLibrary.Utils.Formatter;

public class PrettyTime
{
    private readonly DateTime baseDate;

    private IList<ITimeUnit> timeUnits;

    public PrettyTime() : this(DateTime.Now)
    {
    }

    public PrettyTime(DateTime baseDate)
    {
        this.baseDate = baseDate;
        this.InitTimeUnits();
    }

    private void InitTimeUnits()
    {
        this.timeUnits = new List<ITimeUnit>
                             {
                                 new JustNow(),
                                 new Millisecond(),
                                 new Second(),
                                 new Minute(),
                                 new Hour(),
                                 new Day(),
                                 new Week(),
                                 new Month(),
                                 new Year(),
                                 new Decade(),
                                 new Century(),
                                 new Millennium()
                             };
    }

    public Duration ApproximateDuration(DateTime then)
    {
        var difference = (then - this.baseDate).TotalMilliseconds;
        return this.CalculateDuration(difference);
    }

    private Duration CalculateDuration(double difference)
    {
        var absoluteDifference = Math.Abs(difference);
        var result = new Duration();

        for (var i = 0; i < this.timeUnits.Count; i++)
        {
            var unit = this.timeUnits[i];
            var millisPerUnit = Math.Abs(unit.MillisPerUnit);
            var quantity = Math.Abs(unit.MaxQuantity);
            var isLastUnit = i == this.timeUnits.Count - 1;

            if (quantity == 0 && !isLastUnit)
            {
                quantity = this.timeUnits[i + 1].MillisPerUnit / unit.MillisPerUnit;
            }

            // does our unit encompass the time duration?
            if (!(millisPerUnit * quantity > absoluteDifference) && !isLastUnit)
            {
                continue;
            }

            result.Unit = unit;
            result.Quantity = difference / millisPerUnit;
            result.Delta = difference - result.Quantity * millisPerUnit;
            break;
        }

        return result;
    }

    public string Format(DateTime then)
    {
        var duration = this.ApproximateDuration(then);
        var formatter = duration.Unit.Format;

        return formatter.Format(duration);
    }
}