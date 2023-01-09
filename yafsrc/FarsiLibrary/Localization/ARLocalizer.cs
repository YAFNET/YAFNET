namespace FarsiLibrary.Localization;

/// <summary>
/// Localizer class used to get string values of Arabic language.
/// </summary>
public class ARLocalizer : FALocalizer
{
    /// <summary>
    /// Gets a localized string for Arabic culture, for specified <see cref="StringID"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override string GetLocalizedString(StringID id)
    {
        return id switch
            {
                StringID.FAMonthView_None => "امح",
                StringID.FAMonthView_Today => "اليوم",
                _ => base.GetLocalizedString(id)
            };
    }
}