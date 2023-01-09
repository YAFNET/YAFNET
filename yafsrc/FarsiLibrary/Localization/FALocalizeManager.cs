namespace FarsiLibrary.Localization;

using System;
using System.Globalization;

/// <summary>
/// Localizer class to work with internal localized strings.
/// </summary>
public class FALocalizeManager
{
    private readonly FALocalizer fa = new();

    private readonly ARLocalizer ar = new();
    private readonly ENLocalizer en = new();
    private BaseLocalizer customLocalizer;
    private static FALocalizeManager instance;

    /// <summary>
    /// Prevents a default instance of the <see cref="FALocalizeManager"/> class from being created.
    /// </summary>
    private FALocalizeManager()
    {
        this.FarsiCulture = new CultureInfo("fa-IR");
        this.ArabicCulture = new CultureInfo("ar-SA");
        this.InvariantCulture = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Fired when Localizer has changed.
    /// </summary>
    public event EventHandler LocalizerChanged;

    /// <summary>
    /// Returns an instance of the localized based on CurrentUICulture of the thread.
    /// </summary>
    /// <returns></returns>
    public BaseLocalizer GetLocalizer()
    {
        return this.GetLocalizerByCulture(CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Returns a localizer instance based on the culture.
    /// </summary>
    internal BaseLocalizer GetLocalizerByCulture(CultureInfo ci)
    {
        if (this.customLocalizer != null)
        {
            return this.customLocalizer;
        }
            
        if (ci.Equals(this.FarsiCulture))
        {
            return this.fa;
        }
            
        if (ci.Equals(this.ArabicCulture))
        {
            return this.ar;
        }
            
        return this.en;
    }

    /// <summary>
    /// Singleton Instance of FALocalizeManager.
    /// </summary>
    public static FALocalizeManager Instance => instance ??= new FALocalizeManager();

    /// <summary>
    /// Custom culture, when set , is used across all controls.
    /// </summary>
    public CultureInfo CustomCulture
    {
        get;
        set;
    }

    /// <summary>
    /// Farsi Culture
    /// </summary>
    public CultureInfo FarsiCulture
    {
        get;
    }

    /// <summary>
    /// Arabic Culture
    /// </summary>
    public CultureInfo ArabicCulture
    {
        get;
    }

    /// <summary>
    /// Invariant Culture
    /// </summary>
    public CultureInfo InvariantCulture
    {
        get;
    }

    /// <summary>
    /// Gets or Sets a new instance of Localizer. If this value is initialized (default is null), Localize Manager class will use the custom class provided, to interpret localized strings.
    /// </summary>
    public BaseLocalizer CustomLocalizer
    {
        get => this.customLocalizer;
        set
        {
            if(this.customLocalizer == value)
                return;

            this.customLocalizer = value;
            this.OnLocalizerChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Fires the LocalizerChanged event.
    /// </summary>
    /// <param name="e"></param>
    protected void OnLocalizerChanged(EventArgs e)
    {
        this.LocalizerChanged?.Invoke(null, e);
    }

    internal bool IsCustomArabicCulture => this.CustomCulture != null && this.CustomCulture.Equals(this.ArabicCulture);

    internal bool IsCustomFarsiCulture => this.CustomCulture != null && this.CustomCulture.Equals(this.FarsiCulture);

    internal bool IsThreadCultureFarsi => CultureInfo.CurrentUICulture.Equals(this.FarsiCulture);

    internal bool IsThreadCultureArabic => CultureInfo.CurrentUICulture.Equals(this.ArabicCulture);
}