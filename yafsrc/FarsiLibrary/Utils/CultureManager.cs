namespace FarsiLibrary.Utils;

using System.ComponentModel;
using System.Globalization;

using FarsiLibrary.Utils.Internals;

public class CultureManager
{
    private CultureInfo controlsCulture;

    private CultureManager()
    {
        this.controlsCulture = CultureInfo.InvariantCulture;
        this.UseDefaultCulture = true;
    }

    static CultureManager()
    {
        Instance = new CultureManager();
    }

    public static CultureManager Instance { get; }

    [DefaultValue(true)]
    public bool UseDefaultCulture { get; set; }

    public CultureInfo ControlsCulture
    {
        get => this.UseDefaultCulture ? CultureHelper.CurrentCulture : this.controlsCulture;

        set
        {
            this.controlsCulture = value;
            this.UseDefaultCulture = false;
        }
    }
}