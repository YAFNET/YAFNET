namespace FarsiLibrary.Localization;

using System;

public abstract class BaseLocalizer
{
    public abstract string GetLocalizedString(StringID id);

    public string GetFormatterString(string enumKey)
    {
        var key = (FormatterStringID)Enum.Parse(typeof(FormatterStringID), enumKey);
        return this.GetFormatterString(key);
    }

    public abstract string GetFormatterString(FormatterStringID stringID);
}