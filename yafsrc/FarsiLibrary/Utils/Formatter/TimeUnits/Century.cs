namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Century : AbstractTimeUnit
{
    public Century()
    {
        this.MillisPerUnit = 2629743830L * 12L * 100;
    }

    protected override string GetResourcePrefix()
    {
        return "Century";
    }
}