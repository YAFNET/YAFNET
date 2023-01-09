namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Decade : AbstractTimeUnit
{
    public Decade()
    {
        this.MillisPerUnit = 2629743830L * 12L * 10;
        this.Format.RoundingTolerance = 1;
    }

    protected override string GetResourcePrefix()
    {
        return "Decade";
    }
}