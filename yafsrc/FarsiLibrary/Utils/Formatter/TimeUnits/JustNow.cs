namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class JustNow : AbstractTimeUnit
{
    public JustNow()
    {
        this.MaxQuantity = 1000L * 60L * 5L;
    }

    protected override string GetResourcePrefix()
    {
        return "JustNow";
    }
}