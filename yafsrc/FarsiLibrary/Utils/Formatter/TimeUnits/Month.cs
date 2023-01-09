namespace FarsiLibrary.Utils.Formatter.TimeUnits;

public class Month : AbstractTimeUnit
{
    public Month()
    {
        this.MillisPerUnit = 1000L * 60L * 60L * 24L * 30L;
    }

    protected override string GetResourcePrefix()
    {
        return "Month";
    }
}