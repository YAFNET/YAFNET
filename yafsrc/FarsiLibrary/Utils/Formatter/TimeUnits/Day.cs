namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Day : AbstractTimeUnit
{
    public Day()
    {
        this.MillisPerUnit = 1000L * 60L * 60L * 24L;
    }

    protected override string GetResourcePrefix()
    {
        return "Day";
    }
}