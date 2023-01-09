namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Second : AbstractTimeUnit
{
    public Second()
    {
        this.MillisPerUnit = 1000L;
    }

    protected override string GetResourcePrefix()
    {
        return "Second";
    }
}