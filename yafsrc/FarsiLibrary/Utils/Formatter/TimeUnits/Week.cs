namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Week : AbstractTimeUnit
{
    public Week()
    {
        this.MillisPerUnit = 1000L * 60L * 60L * 24L * 7L;
    }

    protected override string GetResourcePrefix()
    {
        return "Week";
    }
}