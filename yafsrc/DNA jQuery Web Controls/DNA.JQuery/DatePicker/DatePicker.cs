//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

#region Using

using System.Web.UI;

#endregion

[assembly: WebResource("DNA.UI.JQuery.DatePicker.DatePicker.gif", "image/gif")]

namespace DNA.UI.JQuery
{
  #region Using

  using System;
  using System.Collections.Specialized;
  using System.ComponentModel;
  using System.ComponentModel.Design;
  using System.Drawing;
  using System.Drawing.Design;
  using System.Security.Permissions;
  using System.Text;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.Design;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery.Design;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// <para>
  /// DatePicker is a highly configurable WebControl encapsulated the jQuery UI
  ///     datepicker plugin. You can customize the date format and language, restrict the
  ///     selectable date ranges and add in buttons and other navigation options
  ///     easily.
  /// </para>
  /// </summary>
  /// <example>
  /// <code lang="ASP.NET" title="DatePicker propeties">
  /// <![CDATA[
  /// <DotNetAge:DatePicker ID="MyDatePicker"
  ///    AllowChangeMonth="true"
  ///    AllowChangeYear="true"
  ///    AnimationOnShow="show"
  ///    AnotherFormatString="yyyy/MM/dd"
  ///    AppendText=""
  ///    AutoPostBack="false"
  ///    ButtonImageUrl=""
  ///    ButtonText="..."
  ///    CloseButtonText="Done"
  ///    ConstrainInput="true"
  ///    DateFormatString="yyyy/MM/dd"
  ///    DayNames="Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"
  ///    DefaultDateString=""
  ///    DisplayMode="Picker"
  ///    Duration="0"
  ///    FirstDayOfWeek="0"
  ///    HideIfNoPrevNext="false"
  ///    IsRightToLeft="false"
  ///    LocID="en-Us"
  ///    MaxDateFormat=""
  ///    MinDateFormat=""
  ///    MonthNames="January,February,March,April,May,June,July,August,September,October,November,December"
  ///    MonthShortNames="Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
  ///    MoveToSelectedDate="true"
  ///    NavigationAsDateFormat="false"
  ///    NextMonthText="Next"
  ///    NumberOfMonths="1"
  ///    OnClientBeforeShow=""
  ///    OnClientBeforeShowDay=""
  ///    OnClientChangeMonthYear=""
  ///    OnClientClose=""
  ///    OnClientSelect=""
  ///    PrevMonthText="Prev"
  ///    ShortDayNames="Sun,Mon,Tue,Wed,Thu,Fri,Sat"
  ///    ShortYearCutOff=""
  ///    ShowButtonImageOnly="false"
  ///    ShowButtonPanel="false"
  ///    ShowCurrentAtPos="0"
  ///    ShowDefaultButtonImage="false"
  ///    ShowIconMode="Focus"
  ///    ShowMonthAfterYear="false"
  ///    ShowOptions=""
  ///    ShowOtherMonths="false"
  ///    StepMonths="1"
  ///    TextForToday="ToDay"
  ///    Value=""
  ///    YearRange=""
  /// >
  ///   <AnotherField/>
  /// </DotNetAge:DatePicker>]]>
  ///   </code>
  /// </example>
  /// <remarks>
  /// DatePicker Control provides client-side date-picking functionality with
  ///   customizable date format and UI in a popup control. You can interact with the
  ///   calendar by clicking on a day to set the date, or the "Today" link to set the
  ///   current date.
  ///   <para>
  /// By default, the DatePicker calendar opens in a small overlay onFocus and
  ///     closes automatically onBlur or when a date if selected. For an inline Calendar,
  ///     simply attach the Datepicker to a div or span.
  /// </para>
  /// <para>
  /// In addition, the left and right arrows can be used to move forward or back a
  ///     month. By clicking on the title of the calendar you can change the view from Days
  ///     in the current month, to Months in the current year. Another click will switch to
  ///     Years in the current Decade. This action allows you to easily jump to dates in the
  ///     past or the future from within the calendar control.
  /// </para>
  /// </remarks>
  [JQuery(Name = "datepicker", Assembly = "jQueryNet", DisposeMethod = "destroy", 
    ScriptResources = new[] { "ui.core.js", "ui.datepicker.js" }, StartEvent = ClientRegisterEvents.ApplicationLoad)]
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [ToolboxBitmap(typeof(DatePicker), "DatePicker.DatePicker.ico")]
  [ToolboxData("<{0}:DatePicker runat=\"server\" ID=\"DatePicker1\"></{0}:DatePicker>")]
  [Designer(typeof(DatePickerDesigner))]
  [ParseChildren(true)]
  public class DatePicker : WebControl, IPostBackDataHandler, INamingContainer
  {
    #region Constants and Fields

    /// <summary>
    /// The another field.
    /// </summary>
    private JQuerySelector anotherField;

    /// <summary>
    /// The day names.
    /// </summary>
    private string[] dayNames;

    /// <summary>
    /// The min day names.
    /// </summary>
    private string[] minDayNames;

    /// <summary>
    /// The month names.
    /// </summary>
    private string[] monthNames;

    /// <summary>
    /// The month short names.
    /// </summary>
    private string[] monthShortNames;

    /// <summary>
    /// The short day names.
    /// </summary>
    private string[] shortDayNames;

    #endregion

    #region Events

    /// <summary>
    /// The date selected.
    /// </summary>
    public event EventHandler DateSelected;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether the DatePicker allows you to change the month by selecting from a drop-down list. You can enable this feature by setting the property to true.
    /// </summary>
    [JQueryOption("changeMonth", IgnoreValue = false)]
    [Category("Behavior")]
    [Description(
      " Gets/Sets whether the DatePicker allows you to change the month by selecting from a drop-down list. You can enable this feature by setting the property to true"
      )]
    public bool AllowChangeMonth
    {
      get
      {
        object obj = this.ViewState["AllowChangeMonth"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AllowChangeMonth"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the DatePicker allows you to change the year by selecting from a drop-down list. You can enable this feature by setting the attribute to true.
    /// </summary>
    [JQueryOption("changeYear", IgnoreValue = false)]
    [Category("Behavior")]
    [Description(
      "Gets/Sets whether the DatePicker allows you to change the year by selecting from a drop-down list. You can enable this feature by setting the attribute to true."
      )]
    public bool AllowChangeYear
    {
      get
      {
        object obj = this.ViewState["AllowChangeYear"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AllowChangeYear"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the name of the animation used to show/hide the datepicker.
    /// </summary>
    /// <remarks>
    ///   Use 'show' (the default), 'slideDown', 'fadeIn', or any of the show/hide jQuery UI effects.
    /// </remarks>
    [JQueryOption("showAnim")]
    [Bindable(true)]
    [Category("Appearance")]
    [Editor(typeof(AnimationForShowEditor), typeof(UITypeEditor))]
    [Description("Gets/Sets the name of the animation used to show/hide the datepicker")]
    public string AnimationOnShow
    {
      get
      {
        object obj = this.ViewState["AnimationOnShow"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["AnimationOnShow"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the jQuery server side selector for another field that is to be updated with the selected date from the datepicker. Use the altFormat setting below to change the format of the date within this field. Leave as blank for no alternate field.
    /// </summary>
    [Category("Behavior")]
    [Description(
      "Gets/Sets the control ID for another field that is to be updated with the selected date from the datepicker. Use the altFormat setting below to change the format of the date within this field. Leave as blank for no alternate field."
      )]
    [JQueryOption("altField")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public JQuerySelector AnotherField
    {
      get
      {
        if (this.anotherField == null)
        {
          this.anotherField = new JQuerySelector();
        }

        return this.anotherField;
      }

      set
      {
        this.anotherField = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the dateFormat to be used for the altField option. This allows one date format to be shown to the user for selection purposes, while a different format is actually sent behind the scenes. For a full list of the possible formats see the formatDate function
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    [Editor(typeof(DateFormatEditor), typeof(UITypeEditor))]
    [Description(
      "Gets/Sets the dateFormat to be used for the altField option. This allows one date format to be shown to the user for selection purposes, while a different format is actually sent behind the scenes. For a full list of the possible formats see the formatDate function"
      )]
    public string AnotherFormatString
    {
      get
      {
        object obj = this.ViewState["AnotherFormatString"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["AnotherFormatString"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the text to display after each date field, e.g. to show the required format.
    /// </summary>
    [Bindable(true)]
    [Localizable(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the text to display after each date field, e.g. to show the required format.")]
    [JQueryOption("appendText")]
    public string AppendText
    {
      get
      {
        object obj = this.ViewState["AppendText"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["AppendText"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the datepicker post back to server when selected date changed
    /// </summary>
    [Category("Behavior")]
    [Description("Gets/Sets whether the datepicker post back to server when selected date changed")]
    [Bindable(true)]
    public bool AutoPostBack
    {
      get
      {
        object obj = this.ViewState["AutoPostBack"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["AutoPostBack"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the URL for the popup button image. If set, button text becomes the alt value and is not directly displayed.
    /// </summary>
    [Bindable(true)]
    [Themeable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets the URL for the popup button image. If set, button text becomes the alt value and is not directly displayed."
      )]
    [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
    [UrlProperty]
    public string ButtonImageUrl
    {
      get
      {
        object obj = this.ViewState["ButtonImageUrl"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["ButtonImageUrl"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the text to display on the trigger button.
    /// </summary>
    [JQueryOption("buttonText")]
    [Category("Appearance")]
    [Localizable(true)]
    [Description("Gets/Sets the text to display on the trigger button.")]
    [Bindable(true)]
    public string ButtonText
    {
      get
      {
        object obj = this.ViewState["ButtonText"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["ButtonText"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets The text to display for the close link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.
    /// </summary>
    [JQueryOption("closeText")]
    [Category("Appearance")]
    [Localizable(true)]
    [Description(
      "Gets/Sets The text to display for the close link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button."
      )]
    public string CloseButtonText
    {
      get
      {
        object obj = this.ViewState["CloseButtonText"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["CloseButtonText"] = value;
      }
    }

    /// <summary>
    ///   True if the input field is constrained to the current date format.
    /// </summary>
    [JQueryOption("constrainInput", IgnoreValue = false)]
    [Category("Behavior")]
    [Description("True if the input field is constrained to the current date format.")]
    public bool ConstrainInput
    {
      get
      {
        object obj = this.ViewState["ConstrainInput"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ConstrainInput"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the format for parsed and displayed dates.
    /// </summary>
    /// <remarks>
    ///   DatePicker Web Control not use jQuery's dateFormat,you can using the DateFormat in .Net.
    /// </remarks>
    [Category("Appearance")]
    [DefaultValue("MM/dd/yyyy")]
    [Description("Gets/Sets the format for parsed and displayed dates. ")]
    [Editor(typeof(DateFormatEditor), typeof(UITypeEditor))]
    public string DateFormatString
    {
      get
      {
        object obj = this.ViewState["DateFormatString"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["DateFormatString"] = value;
      }
    }

    /// <summary>
    ///   Gets the collection of long day names, starting from Sunday, for use as requested via the dateFormat setting. 
    ///   They also appear as popup hints when hovering over the corresponding column headings. 
    ///   This attribute is one of the regionalisation attributes.
    /// </summary>
    /// <remarks>
    ///   <para>The DayNames default value is below</para>
    ///   <para>'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday',
    ///     'Saturday'</para>
    /// </remarks>
    /// <example>
    ///   <code lang = "ASP.NET" title = "Set DatePicker DayNames">
    ///     <![CDATA[
    /// <DotNetAge:DatePicker runat="server" 
    ///        ID="DatePicker1"
    ///        DayNames="Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"
    /// >
    /// </DotNetAge>]]>
    ///   </code>
    /// </example>
    [Category("Appearance")]
    [Description("Gets the list of long day names")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] DayNames
    {
      get
      {
        return this.dayNames;
      }

      set
      {
        this.dayNames = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the date to highlight on first opening if the field is blank.
    /// </summary>
    /// <remarks>
    ///   Specify either an actual date via a Date object, or a number of days from today 
    ///   (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, 
    ///   e.g. '+1m +7d'), or null for today.
    /// </remarks>
    [JQueryOption("defaultDate")]
    [Category("Data")]
    [Description("Gets/Sets the date to highlight on first opening if the field is blank. ")]
    public string DefaultDateString
    {
      get
      {
        object obj = this.ViewState["DefaultDateString"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["DefaultDateString"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the DatePicker's DisplayMode
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the DatePicker's DisplayMode")]
    public DatePickerDisplayModes DisplayMode
    {
      get
      {
        object obj = this.ViewState["DisplayMode"];
        return (obj == null) ? DatePickerDisplayModes.Picker : (DatePickerDisplayModes)obj;
      }

      set
      {
        this.ViewState["DisplayMode"] = value;
      }
    }

    /// <summary>
    ///   Control the speed at which the datepicker appears, it may be a time in milliseconds
    /// </summary>
    [JQueryOption("duration", IgnoreValue = 0)]
    [Category("Behavior")]
    [Description("Control the speed at which the datepicker appears, it may be a time in milliseconds,")]
    public int Duration
    {
      get
      {
        object obj = this.ViewState["Duration"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["Duration"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the first day of the week:
    /// </summary>
    /// <remarks>
    ///   Sunday is 0, Monday is 1, ... This property is one of the regionalisation properties.
    /// </remarks>
    [JQueryOption("firstDay", IgnoreValue = 0)]
    [Category("Appearance")]
    [Description(" Gets/Sets the first day of the week:")]
    public int FirstDayOfWeek
    {
      get
      {
        object obj = this.ViewState["FirstDay"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["FirstDay"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets normally the previous and next links are disabled when not applicable (see minDate/maxDate). 
    ///   You can hide them altogether by setting this attribute to true.
    /// </summary>
    [JQueryOption("hideIfNoPrevNext", IgnoreValue = false)]
    [Category("Appearance")]
    [Description("Gets/Sets normally the previous and next links are disabled when not applicable (see minDate/maxDate)"
      )]
    public bool HideIfNoPrevNext
    {
      get
      {
        object obj = this.ViewState["HideIfNoPrevNext"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["HideIfNoPrevNext"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the current language is drawn from right to left.
    /// </summary>
    [JQueryOption("isRTL", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets whether the current language is drawn from right to left")]
    public bool IsRightToLeft
    {
      get
      {
        object obj = this.ViewState["IsRightToLeft"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["IsRightToLeft"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the region id for the datepicker
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets the region id for the datepicker")]
    [Editor(typeof(LanguageDropDownEditor), typeof(UITypeEditor))]
    public string LocID
    {
      get
      {
        object obj = this.ViewState["LocID"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["LocID"] = value;
      }
    }

    ///<summary>
    ///  Gets/Sets a maximum selectable date via a Date object, or a number of days from today (e.g. +7) or a string
    ///  of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '+1m +1w'), or null 
    ///  for no limit.
    ///</summary>
    [Bindable(true)]
    [Themeable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets a maximum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '+1m +1w'), or null for no limit."
      )]
    [JQueryOption("maxDate")]
    public string MaxDateFormat
    {
      get
      {
        object obj = this.ViewState["MaxDateFormat"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["MaxDateFormat"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets a minimum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '-1y -1m'), or null for no limit.
    /// </summary>
    [Bindable(true)]
    [Themeable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets a minimum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '-1y -1m'), or null for no limit."
      )]
    [JQueryOption("minDate")]
    public string MinDateFormat
    {
      get
      {
        object obj = this.ViewState["MinDateFormat"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["MinDateFormat"] = value;
      }
    }

    ///<summary>
    ///  Gets the collection of minimised day names, starting from Sunday, for use as column headers within the datepicker. 
    ///  This attribute is one of the regionalisation attributes.
    ///</summary>
    ///<example>
    ///  <code lang = "ASP.NET" title = "Set DatePicker MinDayNames">
    ///    <![CDATA[
    /// <DotNetAge:DatePicker runat="server" 
    ///        ID="DatePicker1"
    ///        MinDayNames="Su,Mo,Tu,We,Th,Fr,Sa"
    /// >
    /// </DotNetAge>]]>
    ///  </code>
    ///</example>
    ///<remarks>
    ///  Default value:'Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'
    ///</remarks>
    [Category("Appearance")]
    [Description("Gets the collection of minimised day names")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] MinDayNames
    {
      get
      {
        return this.minDayNames;
      }

      set
      {
        this.minDayNames = value;
      }
    }

    /// <summary>
    ///   Gets the collection of full month names, as used in the month header on each datepicker and as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.
    /// </summary>
    /// <example>
    ///   <code lang = "ASP.NET" title = "Set DatePicker MonthNames">
    ///     <![CDATA[
    /// <DotNetAge:DatePicker runat="server" 
    ///        ID="DatePicker1"
    ///        MonthNames="January,February,March,April,May,June,July,August,September,October,November,December"
    /// >
    /// </DotNetAge>]]>
    ///   </code>
    /// </example>
    /// <remarks>
    ///   Default value:'January', 'February', 'March', 'April', 'May', 'June', 'July',
    ///   'August', 'September', 'October', 'November', 'December'
    /// </remarks>
    [Category("Appearance")]
    [Description(
      "Gets the collection of full month names, as used in the month header on each datepicker and as requested via the dateFormat setting. This attribute is one of the regionalisation attributes."
      )]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] MonthNames
    {
      get
      {
        return this.monthNames;
      }

      set
      {
        this.monthNames = value;
      }
    }

    /// <summary>
    ///   Gets the collection of abbreviated month names, for use as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.
    /// </summary>
    /// <example>
    ///   <code lang = "ASP.NET" title = "Set DatePicker MonthShortNames">
    ///     <![CDATA[
    /// <DotNetAge:DatePicker runat="server" 
    ///        ID="DatePicker1"
    ///        MonthShortNames="Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
    /// >
    /// </DotNetAge>]]>
    ///   </code>
    /// </example>
    /// <remarks>
    ///   Default value:'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
    /// </remarks>
    [Category("Appearance")]
    [Description(
      "Gets the collection of abbreviated month names, for use as requested via the dateFormat setting. This attribute is one of the regionalisation attributes."
      )]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] MonthShortNames
    {
      get
      {
        return this.monthShortNames;
      }

      set
      {
        this.monthShortNames = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether the current day link moves to the currently selected date instead of today.
    /// </summary>
    [JQueryOption("gotoCurrent", IgnoreValue = false)]
    [Category("Behavior")]
    [Description("Gets/Sets whether the current day link moves to the currently selected date instead of today.")]
    public bool MoveToSelectedDate
    {
      get
      {
        object obj = this.ViewState["MoveToSelectedDate"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["MoveToSelectedDate"] = value;
      }
    }

    /// <summary>
    ///   Gets/sets the formatDate function is applied to the prevText, nextText, 
    ///   and currentText values before display, allowing them to display the target month names for example.
    /// </summary>
    [JQueryOption("navigationAsDateFormat", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Behavior")]
    [Description(
      "Gets/sets the formatDate function is applied to the prevText, nextText, and currentText values before display, allowing them to display the target month names for example."
      )]
    public bool NavigationAsDateFormat
    {
      get
      {
        object obj = this.ViewState["NavigationAsDateFormat"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["NavigationAsDateFormat"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the text to display for the next month link. This attribute is one of the regionalisation attributes.
    ///   With the standard ThemeRoller styling, this value is replaced by an icon.
    /// </summary>
    [JQueryOption("nextText")]
    [Bindable(true)]
    [Localizable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets the text to display for the next month link. This attribute is one of the regionalisation attributes.")
    ]
    public string NextMonthText
    {
      get
      {
        object obj = this.ViewState["NextText"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["NextText"] = value;
      }
    }

    ///<summary>
    ///  Gets/Sets how many months to show at once. The value can be a straight integer, or can be 
    ///  a two-element array to define the number of rows and columns to display.
    ///</summary>
    [JQueryOption("numberOfMonths", IgnoreValue = 0)]
    [Bindable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets how many months to show at once. The value can be a straight integer, or can be a two-element array to define the number of rows and columns to display."
      )]
    public int NumberOfMonths
    {
      get
      {
        object obj = this.ViewState["NumberOfMonths"];
        return (obj == null) ? 0 : Convert.ToInt16(obj);
      }

      set
      {
        this.ViewState["NumberOfMonths"] = value;
      }
    }

    /// <summary>
    ///   Can be a function that takes an input field and current datepicker instance and returns an 
    ///   options object to update the datepicker with. It is called just before the datepicker is displayed.
    /// </summary>
    [Bindable(true)]
    [Category("ClientEvents")]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [JQueryOption("beforeShow", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "input" })]
    public string OnClientBeforeShow { get; set; }

    /// <summary>
    ///   The function takes a date as a parameter and must return an array with [0] equal to 
    ///   true/false indicating whether or not this date is selectable, [1] equal to a CSS class name(s) or 
    ///   '' for the default presentation and [2] an optional popup tooltip for this date. It is called for each 
    ///   day in the datepicker before is it displayed.
    /// </summary>
    [Bindable(true)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Category("ClientEvents")]
    [JQueryOption("beforeShowDay", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "date" })]
    public string OnClientBeforeShowDay { get; set; }

    /// <summary>
    ///   Allows you to define your own event when the datepicker moves to a new month and/or year. 
    ///   The function receives the selected year, month and the datepicker instance as parameters. this refers
    ///   to the associated input field.
    /// </summary>
    [Bindable(true)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Category("ClientEvents")]
    [JQueryOption("onChangeMonthYear", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "date" })]
    public string OnClientChangeMonthYear { get; set; }

    /// <summary>
    ///   Allows you to define your own event when the datepicker is closed, whether or not a date is selected.
    ///   The function receives the selected date as a Date and the datepicker instance as parameters. 
    ///   this refers to the associated input field.
    /// </summary>
    [Bindable(true)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Category("ClientEvents")]
    [JQueryOption("onClose", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "date" })]
    public string OnClientClose { get; set; }

    /// <summary>
    ///   Allows you to define your own event when the datepicker is selected. 
    ///   The function receives the selected date(s) as text and the datepicker instance as parameters. 
    ///   this refers to the associated input field.
    /// </summary>
    [Bindable(true)]
    [TypeConverter(typeof(MultilineStringConverter))]
    [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
    [Category("ClientEvents")]
    [JQueryOption("onSelect", Type = JQueryOptionTypes.Function, FunctionParams = new[] { "dateText" })]
    public string OnClientSelect { get; set; }

    /// <summary>
    ///   Gets/Sets the text to display for the previous month link.
    /// </summary>
    [JQueryOption("prevText")]
    [Bindable(true)]
    [Localizable(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the text to display for the previous month link. ")]
    public string PrevMonthText
    {
      get
      {
        object obj = this.ViewState["PrevText"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["PrevText"] = value;
      }
    }

    /// <summary>
    ///   Gets the colleciton of abbreviated day names, starting from Sunday, for use as requested via the 
    ///   dateFormat setting. This attribute is one of the regionalisation attributes.
    /// </summary>
    /// <example>
    ///   <code lang = "ASP.NET" title = "Set the ShortDayNames of DatePicker">
    ///     <![CDATA[
    /// <DotNetAge:DatePicker runat="server"  
    ///        ID="DatePicker1" 
    ///        DayNames="Sun,Mon,Tue,Wed,Thu,Fri,Sat" 
    /// > 
    /// </DotNetAge>]]>
    ///   </code>
    /// </example>
    /// <remarks>
    ///   <para>default value:'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'</para>
    /// </remarks>
    [Category("Appearance")]
    [Description(" Gets the colleciton of abbreviated day names")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] ShortDayNames
    {
      get
      {
        return this.shortDayNames;
      }

      set
      {
        this.shortDayNames = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the cutoff year for determining the century for a date (used in conjunction with dateFormat 'y').
    ///   If a numeric value (0-99) is provided then this value is used directly. If a string value is provided then 
    ///   it is converted to a number and added to the current year. Once the cutoff year is calculated, any dates
    ///   entered with a year value less than or equal to it are considered to be in the current century, while those
    ///   greater than it are deemed to be in the previous century.
    /// </summary>
    [JQueryOption("shortYearCutoff", IgnoreValue = 0)]
    [Bindable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets the cutoff year for determining the century for a date (used in conjunction with dateFormat 'y').")]
    public int ShortYearCutOff
    {
      get
      {
        object obj = this.ViewState["ShortYearCutOff"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ShortYearCutOff"] = value;
      }
    }

    /// <summary>
    ///   Set to true to place an image after the field to use as the trigger without it appearing on a button.
    /// </summary>
    [JQueryOption("buttonImageOnly", IgnoreValue = false)]
    [Themeable(true)]
    [Category("Appearance")]
    [Description("Set to true to place an image after the field to use as the trigger without it appearing on a button."
      )]
    public bool ShowButtonImageOnly
    {
      get
      {
        object obj = this.ViewState["ShowButtonImageOnly"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowButtonImageOnly"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether to show the button panel.
    /// </summary>
    [JQueryOption("showButtonPanel", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets whether to show the button panel.")]
    public bool ShowButtonPanel
    {
      get
      {
        object obj = this.ViewState["ShowButtonPanel"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowButtonPanel"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets where in a multi-month display the current month shows, starting from 0 at the top/left.
    /// </summary>
    [JQueryOption("showCurrentAtPos", IgnoreValue = 0)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets where in a multi-month display the current month shows, starting from 0 at the top/left.")]
    public int ShowCurrentAtPos
    {
      get
      {
        object obj = this.ViewState["ShowCurrentAtPos"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["ShowCurrentAtPos"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether show the datepicker's button's image
    /// </summary>
    [Category("Appearance")]
    [Description("Gets/Sets whether show the datepicker's button's image")]
    public bool ShowDefaultButtonImage
    {
      get
      {
        object obj = this.ViewState["ShowDefaultButtonImage"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowDefaultButtonImage"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the datepicker appear automatically when the field receives focus ('focus'), appear only when a button is clicked ('button'), or appear when either event takes place ('both').
    /// </summary>
    [JQueryOption("showOn")]
    [Bindable(true)]
    [Category("Appearance")]
    [Description("Gets/Sets the datepicker appear automatically when the field receives focus ('focus')")]
    public DateIconShowModes ShowIconMode
    {
      get
      {
        object obj = this.ViewState["ShowIconMode"];
        return (obj == null) ? DateIconShowModes.Focus : (DateIconShowModes)obj;
      }

      set
      {
        this.ViewState["ShowIconMode"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets whether to show the month after the year in the header.
    /// </summary>
    [JQueryOption("showMonthAfterYear", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets whether to show the month after the year in the header.")]
    public bool ShowMonthAfterYear
    {
      get
      {
        object obj = this.ViewState["ShowMonthAfterYear"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowMonthAfterYear"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets  if using one of the jQuery UI effects for AnimationOnShow, you can provide additional settings for that animation via this option.
    /// </summary>
    [JQueryOption("showOptions")]
    [Bindable(true)]
    [Category("Behavior")]
    [Description("Gets/Sets  if using one of the jQuery UI effects for AnimationOnShow")]
    public string ShowOptions
    {
      get
      {
        object obj = this.ViewState["ShowOptions"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["ShowOptions"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets display dates in other months (non-selectable) at the start or end of the current month.
    /// </summary>
    [JQueryOption("showOtherMonths", IgnoreValue = false)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets display dates in other months (non-selectable) at the start or end of the current month.")]
    public bool ShowOtherMonths
    {
      get
      {
        object obj = this.ViewState["ShowOtherMonths"];
        return (obj == null) ? false : (bool)obj;
      }

      set
      {
        this.ViewState["ShowOtherMonths"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets how many months to move when clicking the Previous/Next links.
    /// </summary>
    [JQueryOption("stepMonths", IgnoreValue = 0)]
    [Bindable(true)]
    [Category("Layout")]
    [Description("Gets/Sets how many months to move when clicking the Previous/Next links.")]
    public int StepMonths
    {
      get
      {
        object obj = this.ViewState["StepMonths"];
        return (obj == null) ? 0 : (int)obj;
      }

      set
      {
        this.ViewState["StepMonths"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the text to display for the current day link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.
    /// </summary>
    [JQueryOption("currentText")]
    [Localizable(true)]
    [Category("Appearance")]
    [Description(
      "Gets/Sets the text to display for the current day link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button."
      )]
    public string TextForToday
    {
      get
      {
        object obj = this.ViewState["TextForToday"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["TextForToday"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the DatePicker 's select value
    /// </summary>
    [Bindable(true)]
    [Category("Data")]
    [Description("Gets/Sets the DatePicker 's select value")]
    public DateTime Value
    {
      get
      {
        object obj = this.ViewState["Value"];
        return (obj == null) ? DateTime.Today : (DateTime)obj;
      }

      set
      {
        this.ViewState["Value"] = value;
      }
    }

    /// <summary>
    ///   Gets/Sets control the range of years displayed in the year drop-down: either relative to current year (-nn:+nn) or absolute (nnnn:nnnn).
    /// </summary>
    [JQueryOption("yearRange")]
    [Bindable(true)]
    [Category("Data")]
    [Description(
      "Gets/Sets control the range of years displayed in the year drop-down: either relative to current year (-nn:+nn) or absolute (nnnn:nnnn)."
      )]
    public string YearRange
    {
      get
      {
        object obj = this.ViewState["YearRange"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["YearRange"] = value;
      }
    }

    /// <summary>
    /// Gets TagKey.
    /// </summary>
    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        if (this.DisplayMode == DatePickerDisplayModes.Picker)
        {
          return HtmlTextWriterTag.Input;
        }
        else
        {
          return HtmlTextWriterTag.Div;
        }
      }
    }

    /// <summary>
    /// Gets ClientKey.
    /// </summary>
    private string ClientKey
    {
      get
      {
        return this.ClientID + "_date";
      }
    }

    /// <summary>
    /// Gets or sets EventHolder.
    /// </summary>
    private string EventHolder
    {
      get
      {
        object obj = this.ViewState["EventHolder"];
        return (obj == null) ? null : (string)obj;
      }

      set
      {
        this.ViewState["EventHolder"] = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The render begin tag.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    public override void RenderBeginTag(HtmlTextWriter writer)
    {
      writer.AddAttribute("name", this.UniqueID);
      if (this.DisplayMode == DatePickerDisplayModes.Picker)
      {
        if (!string.IsNullOrEmpty(this.DateFormatString))
        {
          writer.AddAttribute("value", this.Value.ToString(this.DateFormatString));
        }
        else
        {
          writer.AddAttribute("value", this.Value.ToString("MM/dd/yyyy"));
        }
      }

      base.RenderBeginTag(writer);
    }

    #endregion

    #region Implemented Interfaces

    #region IPostBackDataHandler

    /// <summary>
    /// The load post data.
    /// </summary>
    /// <param name="postDataKey">
    /// The post data key.
    /// </param>
    /// <param name="postCollection">
    /// The post collection.
    /// </param>
    /// <returns>
    /// The load post data.
    /// </returns>
    bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
    {
      string value = string.Empty;

      if (this.DisplayMode == DatePickerDisplayModes.Calendar)
      {
        value = postCollection[this.ClientKey];
      }
      else
      {
        value = postCollection[postDataKey];
      }

      DateTime dateValue = DateTime.MinValue;
      DateTime.TryParse(value, out dateValue);

      if (dateValue != this.Value)
      {
        this.Value = dateValue;
        return true;
      }

      return false;
    }

    /// <summary>
    /// The raise post data changed event.
    /// </summary>
    void IPostBackDataHandler.RaisePostDataChangedEvent()
    {
      if (this.DateSelected != null)
      {
        this.DateSelected(this, EventArgs.Empty);
      }
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      this.Page.RegisterRequiresPostBack(this);
      base.OnInit(e);
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      var builder = new JQueryScriptBuilder(this);
      builder.Prepare();

      if (!string.IsNullOrEmpty(this.ButtonImageUrl))
      {
        builder.AddOption("buttonImage", this.ResolveUrl(this.ButtonImageUrl), true);
      }
      else
      {
        if (this.ShowDefaultButtonImage)
        {
          builder.AddOption(
            "buttonImage", 
            this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif"), 
            true);
        }
      }

      if (this.DisplayMode == DatePickerDisplayModes.Calendar)
      {
        this.Page.ClientScript.RegisterHiddenField(this.ClientKey, this.Value.ToString());
      }

      if ((this.dayNames != null) && (this.dayNames.Length > 0))
      {
        builder.AddOption("dayNames", this.dayNames);
      }

      if ((this.minDayNames != null) && (this.minDayNames.Length > 0))
      {
        builder.AddOption("dayNamesMin", this.minDayNames);
      }

      if ((this.shortDayNames != null) && (this.shortDayNames.Length > 0))
      {
        builder.AddOption("dayNamesShort", this.shortDayNames);
      }

      if ((this.monthNames != null) && (this.monthNames.Length > 0))
      {
        builder.AddOption("monthNames", this.monthNames);
      }

      if ((this.monthShortNames != null) && (this.monthShortNames.Length > 0))
      {
        builder.AddOption("monthNamesShort", this.monthShortNames);
      }

      if (!string.IsNullOrEmpty(this.DateFormatString))
      {
        builder.AddOption("dateFormat", this.GetjQueryDateFormat(this.DateFormatString), true);
      }

      if (!string.IsNullOrEmpty(this.AnotherFormatString))
      {
        builder.AddOption("altFormat", this.GetjQueryDateFormat(this.AnotherFormatString), true);
      }

      builder.Build();

      if (this.DisplayMode == DatePickerDisplayModes.Calendar)
      {
        if (!this.Page.IsPostBack)
        {
          if (!string.IsNullOrEmpty(this.OnClientSelect))
          {
            this.EventHolder = this.OnClientSelect;
          }
        }

        var selectScripts = new StringBuilder();
        if (!string.IsNullOrEmpty(this.EventHolder))
        {
          selectScripts.Append(this.GetFunctionBody());
        }

        selectScripts.Append("$get('" + this.ClientKey + "').value=jQuery('#" + this.ClientID + "').datepicker('getDate');");
        if (this.AutoPostBack)
        {
          selectScripts.Append(this.Page.ClientScript.GetPostBackEventReference(this, string.Empty) + ";");
        }

        this.OnClientSelect = selectScripts.ToString();
      }

      if (!string.IsNullOrEmpty(this.LocID))
      {
        string name = "jQueryNet.lang." + this.LocID + ".js";
        ScriptManager sm = ScriptManager.GetCurrent(this.Page);
        ClientScriptManager.AddCompositeScript(this, name, "jQueryNet");
        ClientScriptManager.RegisterClientApplicationInitScript(
          this, 
          this.ClientID + "_sys_localize_init", 
          "$.datepicker.setDefaults($.datepicker.regional['" + this.LocID + "']);");
      }

      if (this.DisplayMode == DatePickerDisplayModes.Calendar)
      {
        builder.AppendSelector();
        builder.Scripts.Append(".datepicker('setDate','" + this.Value + "');");
      }

      // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_loaded", "jQuery('#" + ClientID + "').datepicker('setDate','" + Value.ToString() + "');");

      // base.OnPreRender(e);
      ClientScriptManager.RegisterJQueryControl(this, builder);

      

      // Dictionary<string, string> options = new Dictionary<string, string>();

      // if (!string.IsNullOrEmpty(ButtonImageUrl))
      // options.Add("buttonImage", "'" + ResolveUrl(ButtonImageUrl) + "'");
      // else
      // {
      // if (ShowDefaultButtonImage)
      // options.Add("buttonImage", "'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif") + "'");
      // }

      // if (DisplayMode == DatePickerDisplayModes.Calendar)
      // Page.ClientScript.RegisterHiddenField(ClientKey, Value.ToString());

      // if (dayNames.Count > 0)
      // {
      // List<string> ln = new List<string>();
      // foreach (DateName dayName in dayNames)
      // ln.Add("'" + dayName.ToString() + "'");
      // options.Add("dayNames", "[" + String.Join(",", ln.ToArray()) + "]");
      // }

      // if (minDayNames.Count > 0)
      // {
      // List<string> ln = new List<string>();
      // foreach (DateName dayName in minDayNames)
      // ln.Add("'" + dayName.ToString() + "'");
      // options.Add("dayNamesMin", "[" + String.Join(",", ln.ToArray()) + "]");
      // }

      // if (shortDayNames.Count > 0)
      // {
      // List<string> ln = new List<string>();
      // foreach (DateName dayName in shortDayNames)
      // ln.Add("'" + dayName.ToString() + "'");
      // options.Add("dayNamesShort", "[" + String.Join(",", ln.ToArray()) + "]");
      // }

      // if (monthNames.Count > 0)
      // {
      // List<string> ln = new List<string>();
      // foreach (DateName dayName in monthNames)
      // ln.Add("'" + dayName.ToString() + "'");
      // options.Add("monthNames", "[" + String.Join(",", ln.ToArray()) + "]");
      // }

      // if (monthShortNames.Count > 0)
      // {
      // List<string> ln = new List<string>();
      // foreach (DateName dayName in monthShortNames)
      // ln.Add("'" + dayName.ToString() + "'");
      // options.Add("monthNamesShort", "[" + String.Join(",", ln.ToArray()) + "]");
      // }

      // if (!string.IsNullOrEmpty(AnotherFieldTargetID))
      // {
      // string id = ClientScriptManager.GetControlClientID(Page, AnotherFieldTargetID);
      // options.Add("altField", "'#" + id + "'");
      // }

      // if (!string.IsNullOrEmpty(DateFormatString))
      // options.Add("dateFormat", "'" + GetjQueryDateFormat(DateFormatString) + "'");

      // if (!string.IsNullOrEmpty(AnotherFormatString))
      // options.Add("altFormat", "'" + GetjQueryDateFormat(AnotherFormatString) + "'");

      // if (DisplayMode == DatePickerDisplayModes.Calendar)
      // {
      // if (!Page.IsPostBack)
      // {
      // if (!string.IsNullOrEmpty(OnClientSelect))
      // EventHolder = OnClientSelect;
      // }

      // StringBuilder selectScripts = new StringBuilder();
      // if (!string.IsNullOrEmpty(EventHolder))
      // selectScripts.Append(GetFunctionBody());
      // selectScripts.Append("$get('" + ClientKey + "').value=jQuery('#" + ClientID + "').datepicker('getDate');");
      // selectScripts.Append(Page.ClientScript.GetPostBackEventReference(this, "") + ";");
      // this.OnClientSelect = selectScripts.ToString();
      // }

      // if (!string.IsNullOrEmpty(LocID))
      // {
      // string name = "jQueryNet.lang." + LocID + ".js";
      // ScriptManager sm = ScriptManager.GetCurrent(Page);
      // ClientScriptManager.AddCompositeScript(this, name, "jQuery");
      // //sm.CompositeScript.Scripts.Add(new ScriptReference(name, "jQuery"));
      // ClientScriptManager.RegisterClientApplicationInitScript(this, ClientID + "_sys_localize_init", "$.datepicker.setDefaults($.datepicker.regional['" + LocID + "']);");
      // }

      // if (options.Count == 0)
      // ClientScriptManager.RegisterJQueryControl(this);
      // else
      // ClientScriptManager.RegisterJQueryControl(this, options);

      // if (DisplayMode == DatePickerDisplayModes.Calendar)
      // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_loaded", "jQuery('#" + ClientID + "').datepicker('setDate','" + Value.ToString() + "');");
      // base.OnPreRender(e);
      
    }

    /// <summary>
    /// The get function body.
    /// </summary>
    /// <returns>
    /// The get function body.
    /// </returns>
    private string GetFunctionBody()
    {
      string formatted = this.EventHolder;

      if (!string.IsNullOrEmpty(formatted))
      {
        if (formatted.StartsWith("function"))
        {
          int startAt = formatted.IndexOf("{");
          if (startAt != -1)
          {
            int len = formatted.Length - 1 - startAt;
            if (len > 0)
            {
              formatted = formatted.Substring(startAt, len);
            }
          }
        }
      }

      return formatted;
    }

    /// <summary>
    /// Convert .Net DateTime Format to jQuery DateTime Format
    /// </summary>
    /// <param name="format">
    /// </param>
    /// <returns>
    /// The getj query date format.
    /// </returns>
    private string GetjQueryDateFormat(string format)
    {
      string formatted = format;

      // Format year
      if (formatted.IndexOf("yy") > -1)
      {
        formatted = formatted.Replace("yy", "y");
      }

      // if (formatted.IndexOf("yy") > -1)
      // formatted = formatted.Replace("yy", "y");
      if (formatted.IndexOf("MMMM") > -1)
      {
        formatted = formatted.Replace("MMMM", "MM");
      }
      else
      {
        if (formatted.IndexOf("MMM") > -1)
        {
          formatted = formatted.Replace("MMM", "M");
        }
        else
        {
          if (formatted.IndexOf("MM") > -1)
          {
            formatted = formatted.Replace("MM", "mm");
          }
          else if (formatted.IndexOf("M") > -1)
          {
            formatted = formatted.Replace("M", "m");
          }
        }
      }

      if (formatted.IndexOf("dddd") > -1)
      {
        formatted = formatted.Replace("dddd", "DD");
      }

      if (formatted.IndexOf("ddd") > -1)
      {
        formatted = formatted.Replace("ddd", "D");
      }

      return formatted;
    }

    #endregion
  }
}