using FarsiLibrary.Localization;

namespace FarsiLibrary.Utils.Formatter;

public abstract class AbstractTimeUnit : ITimeUnit
{
    protected AbstractTimeUnit()
    {
        this.MaxQuantity = 0;
        this.MillisPerUnit = 1;
        this.LoadStringKeys();
    }

    protected FALocalizeManager LocalizeManager => FALocalizeManager.Instance;

    public ITimeFormat Format { get; set; }
    public string Name { get; set; }
    public string PluralName { get; set; }
    public double MaxQuantity { get; set; }
    public double MillisPerUnit { get; set; }

    private void LoadStringKeys()
    {
        var resPrefix = this.GetResourcePrefix();
        var pattern = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "Pattern");
        var futurePrefix = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "FuturePrefix");
        var futureSuffix = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "FutureSuffix");
        var pastPrefix = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PastPrefix");
        var pastSuffix = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PastSuffix");

        this.Name = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "Name");
        this.PluralName = this.LocalizeManager.GetLocalizer().GetFormatterString(resPrefix + "PluralName");

        this.Format = new BasicTimeFormat().SetPattern(pattern)
            .SetFuturePrefix(futurePrefix)
            .SetFutureSuffix(futureSuffix)
            .SetPastPrefix(pastPrefix)
            .SetPastSuffix(pastSuffix);
    }

    protected abstract string GetResourcePrefix();
}