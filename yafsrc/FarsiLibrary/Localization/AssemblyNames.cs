namespace FarsiLibrary.Localization;

using System.Collections.Generic;

internal static class AssemblyNames
{
    public const string Description =
        "Library containing farsi controls, which has correct Right-To-Left drawing. Also contains classes to work with Jalali Dates";

    public const string AssemblyTradeMark =
        "Farsi Library and Controls by Hadi Eskandari (H.Eskandari@Gmail.com). All rights are reserved for the author. Contact the author to obtain license to use this product.";

    public const string AssemblyCopyright = "Copyright (c) 2005-2016 SeeSharp Software";

    public const string AssemblyGenericTitle = "Farsi Library";

    public const string Version = "2.7.0.0";

    public const string ShortVersion = "2.7";

    public static Product WinForms = "FarsiLibrary.Win";

    public static Product WebForms = "FarsiLibrary.Web";

    public static Product Wpf = "FarsiLibrary.WPF";

    public static Product Utils = "FarsiLibrary.Utils";

    public static Product Localization = "FarsiLibrary.Localization";

    public static Product DevExpress = "FarsiLibrary.Win.DevExpress";

    public static IList<Product> Products = new[] {WinForms, Wpf, WebForms, Utils, Localization};

    public class Product
    {
        public static implicit operator Product(string name)
        {
            return new(name);
        }

        public Product(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}