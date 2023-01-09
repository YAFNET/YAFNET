namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Hour : AbstractTimeUnit
{
    public Hour()
    {
        this.MillisPerUnit = 1000L * 60L * 60L;
    }

    protected override string GetResourcePrefix()
    {
        return "Hour";
    }
}