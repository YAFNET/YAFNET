///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;

[assembly: WebResource("DNA.UI.JQuery.DatePicker.DatePicker.gif", "image/gif")]
namespace DNA.UI.JQuery
{
    /// <summary>
    /// 	<para>DatePicker is a highly configurable WebControl encapsulated the jQuery UI
    ///     datepicker plugin. You can customize the date format and language, restrict the
    ///     selectable date ranges and add in buttons and other navigation options
    ///     easily.</para>
    /// </summary>
    /// <example>
    /// 	<code lang="ASP.NET" title="DatePicker propeties">
    /// 		<![CDATA[
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
    ///   <AnotherField />
    /// </DotNetAge:DatePicker>]]>
    /// 	</code>
    /// </example>
    /// <remarks>
    ///     DatePicker Control provides client-side date-picking functionality with
    ///     customizable date format and UI in a popup control. You can interact with the
    ///     calendar by clicking on a day to set the date, or the "Today" link to set the
    ///     current date.
    ///     <para>By default, the DatePicker calendar opens in a small overlay onFocus and
    ///     closes automatically onBlur or when a date if selected. For an inline Calendar,
    ///     simply attach the Datepicker to a div or span.</para>
    /// 	<para>In addition, the left and right arrows can be used to move forward or back a
    ///     month. By clicking on the title of the calendar you can change the view from Days
    ///     in the current month, to Months in the current year. Another click will switch to
    ///     Years in the current Decade. This action allows you to easily jump to dates in the
    ///     past or the future from within the calendar control.</para>
    /// </remarks>
    [JQuery(Name = "datepicker", Assembly = "jQueryNet", DisposeMethod = "destroy", ScriptResources = new string[] { "ui.core.js", "ui.datepicker.js" }, StartEvent = ClientRegisterEvents.ApplicationLoad)]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    [System.Drawing.ToolboxBitmap(typeof(DatePicker), "DatePicker.DatePicker.ico")]
    [ToolboxData("<{0}:DatePicker runat=\"server\" ID=\"DatePicker1\"></{0}:DatePicker>")]
    [Designer(typeof(Design.DatePickerDesigner))]
    [ParseChildren(true)]
    public class DatePicker : WebControl, IPostBackDataHandler, INamingContainer
    {
        private JQuerySelector anotherField;
        private string[] monthNames;
        private string[] dayNames;
        private string[] shortDayNames;
        private string[] minDayNames;
        private string[] monthShortNames;
        public event EventHandler DateSelected;

        #region Properties

        /// <summary>
        ///  Gets/Sets the text to display after each date field, e.g. to show the required format.
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
                Object obj = ViewState["AppendText"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["AppendText"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the URL for the popup button image. If set, button text becomes the alt value and is not directly displayed.
        /// </summary>
        [Bindable(true)]
        [Themeable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the URL for the popup button image. If set, button text becomes the alt value and is not directly displayed.")]
        [Editor(typeof(ImageUrlEditor), typeof(UITypeEditor))]
        [UrlProperty]
        public string ButtonImageUrl
        {
            get
            {
                Object obj = ViewState["ButtonImageUrl"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["ButtonImageUrl"] = value;
            }
        }

        /// <summary>
        /// Set to true to place an image after the field to use as the trigger without it appearing on a button.
        /// </summary>
        [JQueryOption("buttonImageOnly", IgnoreValue = false)]
        [Themeable(true)]
        [Category("Appearance")]
        [Description("Set to true to place an image after the field to use as the trigger without it appearing on a button.")]
        public bool ShowButtonImageOnly
        {
            get
            {
                Object obj = ViewState["ShowButtonImageOnly"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowButtonImageOnly"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether show the datepicker's button's image
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets whether show the datepicker's button's image")]
        public bool ShowDefaultButtonImage
        {
            get
            {
                Object obj = ViewState["ShowDefaultButtonImage"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowDefaultButtonImage"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the dateFormat to be used for the altField option. This allows one date format to be shown to the user for selection purposes, while a different format is actually sent behind the scenes. For a full list of the possible formats see the formatDate function
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Editor(typeof(Design.DateFormatEditor), typeof(UITypeEditor))]
        [Description("Gets/Sets the dateFormat to be used for the altField option. This allows one date format to be shown to the user for selection purposes, while a different format is actually sent behind the scenes. For a full list of the possible formats see the formatDate function")]
        public string AnotherFormatString
        {
            get
            {
                Object obj = ViewState["AnotherFormatString"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["AnotherFormatString"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the jQuery server side selector for another field that is to be updated with the selected date from the datepicker. Use the altFormat setting below to change the format of the date within this field. Leave as blank for no alternate field.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets the control ID for another field that is to be updated with the selected date from the datepicker. Use the altFormat setting below to change the format of the date within this field. Leave as blank for no alternate field.")]
        [JQueryOption("altField")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public JQuerySelector AnotherField
        {
            get
            {
                if (anotherField == null)
                    anotherField = new JQuerySelector();
                return anotherField;
            }
            set
            {
                anotherField = value;
            }
        }

        /// <summary>
        /// Gets/Sets the text to display on the trigger button.
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
                Object obj = ViewState["ButtonText"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["ButtonText"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the DatePicker allows you to change the month by selecting from a drop-down list. You can enable this feature by setting the property to true.
        /// </summary>
        [JQueryOption("changeMonth", IgnoreValue = false)]
        [Category("Behavior")]
        [Description(" Gets/Sets whether the DatePicker allows you to change the month by selecting from a drop-down list. You can enable this feature by setting the property to true")]
        public bool AllowChangeMonth
        {
            get
            {
                Object obj = ViewState["AllowChangeMonth"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AllowChangeMonth"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the datepicker post back to server when selected date changed
        /// </summary>
        [Category("Behavior")]
        [Description("Gets/Sets whether the datepicker post back to server when selected date changed")]
        [Bindable(true)]
        public bool AutoPostBack
        {
            get
            {
                Object obj = ViewState["AutoPostBack"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AutoPostBack"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the DatePicker allows you to change the year by selecting from a drop-down list. You can enable this feature by setting the attribute to true.
        /// </summary>
        [JQueryOption("changeYear", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("Gets/Sets whether the DatePicker allows you to change the year by selecting from a drop-down list. You can enable this feature by setting the attribute to true.")]
        public bool AllowChangeYear
        {
            get
            {
                Object obj = ViewState["AllowChangeYear"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["AllowChangeYear"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets The text to display for the close link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.
        /// </summary>
        [JQueryOption("closeText")]
        [Category("Appearance")]
        [Localizable(true)]
        [Description("Gets/Sets The text to display for the close link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.")]
        public string CloseButtonText
        {
            get
            {
                Object obj = ViewState["CloseButtonText"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["CloseButtonText"] = value;
            }
        }

        /// <summary>
        /// True if the input field is constrained to the current date format.
        /// </summary>
        [JQueryOption("constrainInput", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("True if the input field is constrained to the current date format.")]
        public bool ConstrainInput
        {
            get
            {
                Object obj = ViewState["ConstrainInput"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ConstrainInput"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the text to display for the current day link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.
        /// </summary>
        [JQueryOption("currentText")]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the text to display for the current day link. This attribute is one of the regionalisation attributes. Use the ShowButtonPanel to display this button.")]
        public string TextForToday
        {
            get
            {
                Object obj = ViewState["TextForToday"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["TextForToday"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the region id for the datepicker
        /// </summary>
        [Category("Appearance")]
        [Description("Gets/Sets the region id for the datepicker")]
        [Editor(typeof(Design.LanguageDropDownEditor), typeof(UITypeEditor))]
        public string LocID
        {
            get
            {
                Object obj = ViewState["LocID"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["LocID"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the format for parsed and displayed dates. 
        /// </summary>
        /// <remarks>
        ///  DatePicker Web Control not use jQuery's dateFormat,you can using the DateFormat in .Net.
        /// </remarks>
        [Category("Appearance")]
        [DefaultValue("MM/dd/yyyy")]
        [Description("Gets/Sets the format for parsed and displayed dates. ")]
        [Editor(typeof(Design.DateFormatEditor), typeof(UITypeEditor))]
        public string DateFormatString
        {
            get
            {
                Object obj = ViewState["DateFormatString"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["DateFormatString"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of long day names, starting from Sunday, for use as requested via the dateFormat setting. 
        /// They also appear as popup hints when hovering over the corresponding column headings. 
        /// This attribute is one of the regionalisation attributes.
        /// </summary>
        /// <remarks>
        /// 	<para>The DayNames default value is below</para>
        /// 	<para>'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday',
        ///     'Saturday'</para>
        /// </remarks>
        /// <example>
        /// 	<code lang="ASP.NET" title="Set DatePicker DayNames">
        /// 		<![CDATA[
        /// <DotNetAge:DatePicker runat="server" 
        ///        ID="DatePicker1"
        ///        DayNames="Sunday,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday"
        /// >
        /// </DotNetAge>]]>
        /// 	</code>
        /// </example>
        [Category("Appearance")]
        [Description("Gets the list of long day names")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] DayNames
        {
            get { return dayNames; }
            set { dayNames = value; }
        }

        /// <summary>
        /// Gets the colleciton of abbreviated day names, starting from Sunday, for use as requested via the 
        /// dateFormat setting. This attribute is one of the regionalisation attributes.
        /// </summary>
        /// <example>
        /// 	<code lang="ASP.NET" title="Set the ShortDayNames of DatePicker">
        /// 		<![CDATA[
        /// <DotNetAge:DatePicker runat="server"  
        ///        ID="DatePicker1" 
        ///        DayNames="Sun,Mon,Tue,Wed,Thu,Fri,Sat" 
        /// > 
        /// </DotNetAge>]]>
        /// 	</code>
        /// </example>
        /// <remarks><para>default value:'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'</para></remarks>
        [Category("Appearance")]
        [Description(" Gets the colleciton of abbreviated day names")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] ShortDayNames
        {
            get { return shortDayNames; }
            set { shortDayNames = value; }
        }

        /// <summary>
        ///Gets the collection of minimised day names, starting from Sunday, for use as column headers within the datepicker. 
        ///This attribute is one of the regionalisation attributes.
        /// </summary>
        /// <example>
        /// 	<code lang="ASP.NET" title="Set DatePicker MinDayNames">
        /// 		<![CDATA[
        /// <DotNetAge:DatePicker runat="server" 
        ///        ID="DatePicker1"
        ///        MinDayNames="Su,Mo,Tu,We,Th,Fr,Sa"
        /// >
        /// </DotNetAge>]]>
        /// 	</code>
        /// </example> 
        /// <remarks>
        /// Default value:'Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'
        /// </remarks>
        [Category("Appearance")]
        [Description("Gets the collection of minimised day names")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] MinDayNames
        {
            get { return minDayNames; }
            set { minDayNames = value; }
        }

        /// <summary>
        /// Gets/Sets the date to highlight on first opening if the field is blank. 
        /// </summary>
        /// <remarks>
        /// Specify either an actual date via a Date object, or a number of days from today 
        /// (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, 
        /// e.g. '+1m +7d'), or null for today.
        /// </remarks>
        [JQueryOption("defaultDate")]
        [Category("Data")]
        [Description("Gets/Sets the date to highlight on first opening if the field is blank. ")]
        public string DefaultDateString
        {
            get
            {
                Object obj = ViewState["DefaultDateString"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["DefaultDateString"] = value;
            }
        }

        /// <summary>
        /// Control the speed at which the datepicker appears, it may be a time in milliseconds
        /// </summary>
        [JQueryOption("duration", IgnoreValue = 0)]
        [Category("Behavior")]
        [Description("Control the speed at which the datepicker appears, it may be a time in milliseconds,")]
        public int Duration
        {
            get
            {
                Object obj = ViewState["Duration"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["Duration"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the first day of the week: 
        /// </summary>
        /// <remarks>
        /// Sunday is 0, Monday is 1, ... This property is one of the regionalisation properties.
        /// </remarks>
        [JQueryOption("firstDay", IgnoreValue = 0)]
        [Category("Appearance")]
        [Description(" Gets/Sets the first day of the week:")]
        public int FirstDayOfWeek
        {
            get
            {
                Object obj = ViewState["FirstDay"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["FirstDay"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the current day link moves to the currently selected date instead of today.
        /// </summary>
        [JQueryOption("gotoCurrent", IgnoreValue = false)]
        [Category("Behavior")]
        [Description("Gets/Sets whether the current day link moves to the currently selected date instead of today.")]
        public bool MoveToSelectedDate
        {
            get
            {
                Object obj = ViewState["MoveToSelectedDate"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["MoveToSelectedDate"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets normally the previous and next links are disabled when not applicable (see minDate/maxDate). 
        /// You can hide them altogether by setting this attribute to true.
        /// </summary>
        [JQueryOption("hideIfNoPrevNext", IgnoreValue = false)]
        [Category("Appearance")]
        [Description("Gets/Sets normally the previous and next links are disabled when not applicable (see minDate/maxDate)")]
        public bool HideIfNoPrevNext
        {
            get
            {
                Object obj = ViewState["HideIfNoPrevNext"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["HideIfNoPrevNext"] = value;
            }
        }

        /// <summary>
        ///Gets/Sets a maximum selectable date via a Date object, or a number of days from today (e.g. +7) or a string
        ///of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '+1m +1w'), or null 
        ///for no limit.
        /// </summary>
        [Bindable(true)]
        [Themeable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets a maximum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '+1m +1w'), or null for no limit.")]
        [JQueryOption("maxDate")]
        public string MaxDateFormat
        {
            get
            {
                Object obj = ViewState["MaxDateFormat"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["MaxDateFormat"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets a minimum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '-1y -1m'), or null for no limit.
        /// </summary>
        [Bindable(true)]
        [Themeable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets a minimum selectable date via a Date object, or a number of days from today (e.g. +7) or a string of values and periods ('y' for years, 'm' for months, 'w' for weeks, 'd' for days, e.g. '-1y -1m'), or null for no limit.")]
        [JQueryOption("minDate")]
        public string MinDateFormat
        {
            get
            {
                Object obj = ViewState["MinDateFormat"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["MinDateFormat"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of full month names, as used in the month header on each datepicker and as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.
        /// </summary>
        /// <example>
        /// 	<code lang="ASP.NET" title="Set DatePicker MonthNames">
        /// 		<![CDATA[
        /// <DotNetAge:DatePicker runat="server" 
        ///        ID="DatePicker1"
        ///        MonthNames="January,February,March,April,May,June,July,August,September,October,November,December"
        /// >
        /// </DotNetAge>]]>
        /// 	</code>
        /// </example> 
        /// <remarks>
        /// Default value:'January', 'February', 'March', 'April', 'May', 'June', 'July',
        /// 'August', 'September', 'October', 'November', 'December'
        /// </remarks>
        [Category("Appearance")]
        [Description("Gets the collection of full month names, as used in the month header on each datepicker and as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] MonthNames
        {
            get { return monthNames; }
            set { monthNames = value; }
        }

        /// <summary>
        /// Gets the collection of abbreviated month names, for use as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.
        /// </summary>
        /// <example>
        /// 	<code lang="ASP.NET" title="Set DatePicker MonthShortNames">
        /// 		<![CDATA[
        /// <DotNetAge:DatePicker runat="server" 
        ///        ID="DatePicker1"
        ///        MonthShortNames="Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
        /// >
        /// </DotNetAge>]]>
        /// 	</code>
        /// </example> 
        /// <remarks>
        /// Default value:'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
        /// </remarks>
        [Category("Appearance")]
        [Description("Gets the collection of abbreviated month names, for use as requested via the dateFormat setting. This attribute is one of the regionalisation attributes.")]
        [PersistenceMode(PersistenceMode.Attribute)]
        [NotifyParentProperty(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        public string[] MonthShortNames
        {
            get { return monthShortNames; }
            set { monthShortNames = value; }
        }

        /// <summary>
        /// Gets/sets the formatDate function is applied to the prevText, nextText, 
        /// and currentText values before display, allowing them to display the target month names for example.
        /// </summary>
        [JQueryOption("navigationAsDateFormat", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/sets the formatDate function is applied to the prevText, nextText, and currentText values before display, allowing them to display the target month names for example.")]
        public bool NavigationAsDateFormat
        {
            get
            {
                Object obj = ViewState["NavigationAsDateFormat"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["NavigationAsDateFormat"] = value;
            }
        }

        /// <summary>
        ///  Gets/Sets the text to display for the next month link. This attribute is one of the regionalisation attributes.
        ///  With the standard ThemeRoller styling, this value is replaced by an icon.
        /// </summary>
        [JQueryOption("nextText")]
        [Bindable(true)]
        [Localizable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the text to display for the next month link. This attribute is one of the regionalisation attributes.")]
        public string NextMonthText
        {
            get
            {
                Object obj = ViewState["NextText"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["NextText"] = value;
            }
        }

        ///<summary>
        ///Gets/Sets how many months to show at once. The value can be a straight integer, or can be 
        ///a two-element array to define the number of rows and columns to display.
        ///</summary>
        [JQueryOption("numberOfMonths", IgnoreValue = 0)]
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets how many months to show at once. The value can be a straight integer, or can be a two-element array to define the number of rows and columns to display.")]
        public int NumberOfMonths
        {
            get
            {
                Object obj = ViewState["NumberOfMonths"];
                return (obj == null) ? 0 : Convert.ToInt16(obj);
            }
            set
            {
                ViewState["NumberOfMonths"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the text to display for the previous month link. 
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
                Object obj = ViewState["PrevText"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["PrevText"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the cutoff year for determining the century for a date (used in conjunction with dateFormat 'y').
        /// If a numeric value (0-99) is provided then this value is used directly. If a string value is provided then 
        /// it is converted to a number and added to the current year. Once the cutoff year is calculated, any dates
        /// entered with a year value less than or equal to it are considered to be in the current century, while those
        /// greater than it are deemed to be in the previous century.
        /// </summary>
        [JQueryOption("shortYearCutoff", IgnoreValue = 0)]
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the cutoff year for determining the century for a date (used in conjunction with dateFormat 'y').")]
        public int ShortYearCutOff
        {
            get
            {
                Object obj = ViewState["ShortYearCutOff"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["ShortYearCutOff"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the name of the animation used to show/hide the datepicker.
        /// </summary>
        /// <remarks>
        ///  Use 'show' (the default), 'slideDown', 'fadeIn', or any of the show/hide jQuery UI effects.
        /// </remarks>
        [JQueryOption("showAnim")]
        [Bindable(true)]
        [Category("Appearance")]
        [Editor(typeof(Design.AnimationForShowEditor), typeof(UITypeEditor))]
        [Description("Gets/Sets the name of the animation used to show/hide the datepicker")]
        public string AnimationOnShow
        {
            get
            {
                Object obj = ViewState["AnimationOnShow"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["AnimationOnShow"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether to show the button panel.
        /// </summary>
        [JQueryOption("showButtonPanel", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets whether to show the button panel.")]
        public bool ShowButtonPanel
        {
            get
            {
                Object obj = ViewState["ShowButtonPanel"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowButtonPanel"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets where in a multi-month display the current month shows, starting from 0 at the top/left.
        /// </summary>
        [JQueryOption("showCurrentAtPos", IgnoreValue = 0)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets where in a multi-month display the current month shows, starting from 0 at the top/left.")]
        public int ShowCurrentAtPos
        {
            get
            {
                Object obj = ViewState["ShowCurrentAtPos"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["ShowCurrentAtPos"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether to show the month after the year in the header.
        /// </summary>
        [JQueryOption("showMonthAfterYear", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets whether to show the month after the year in the header.")]
        public bool ShowMonthAfterYear
        {
            get
            {
                Object obj = ViewState["ShowMonthAfterYear"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowMonthAfterYear"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the datepicker appear automatically when the field receives focus ('focus'), appear only when a button is clicked ('button'), or appear when either event takes place ('both').
        /// </summary>
        [JQueryOption("showOn")]
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the datepicker appear automatically when the field receives focus ('focus')")]
        public DateIconShowModes ShowIconMode
        {
            get
            {
                Object obj = ViewState["ShowIconMode"];
                return (obj == null) ? DateIconShowModes.Focus : (DateIconShowModes)obj;
            }
            set
            {
                ViewState["ShowIconMode"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets  if using one of the jQuery UI effects for AnimationOnShow, you can provide additional settings for that animation via this option.
        /// </summary>
        [JQueryOption("showOptions")]
        [Bindable(true)]
        [Category("Behavior")]
        [Description("Gets/Sets  if using one of the jQuery UI effects for AnimationOnShow")]
        public string ShowOptions
        {
            get
            {
                Object obj = ViewState["ShowOptions"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["ShowOptions"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets display dates in other months (non-selectable) at the start or end of the current month.
        /// </summary>
        [JQueryOption("showOtherMonths", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets display dates in other months (non-selectable) at the start or end of the current month.")]
        public bool ShowOtherMonths
        {
            get
            {
                Object obj = ViewState["ShowOtherMonths"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["ShowOtherMonths"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets how many months to move when clicking the Previous/Next links.
        /// </summary>
        [JQueryOption("stepMonths", IgnoreValue = 0)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets how many months to move when clicking the Previous/Next links.")]
        public int StepMonths
        {
            get
            {
                Object obj = ViewState["StepMonths"];
                return (obj == null) ? 0 : (int)obj;
            }
            set
            {
                ViewState["StepMonths"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets control the range of years displayed in the year drop-down: either relative to current year (-nn:+nn) or absolute (nnnn:nnnn).
        /// </summary>
        [JQueryOption("yearRange")]
        [Bindable(true)]
        [Category("Data")]
        [Description("Gets/Sets control the range of years displayed in the year drop-down: either relative to current year (-nn:+nn) or absolute (nnnn:nnnn).")]
        public string YearRange
        {
            get
            {
                Object obj = ViewState["YearRange"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["YearRange"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the DatePicker's DisplayMode
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [Description("Gets/Sets the DatePicker's DisplayMode")]
        public DatePickerDisplayModes DisplayMode
        {
            get
            {
                Object obj = ViewState["DisplayMode"];
                return (obj == null) ? DatePickerDisplayModes.Picker : (DatePickerDisplayModes)obj;
            }
            set
            {
                ViewState["DisplayMode"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the current language is drawn from right to left. 
        /// </summary>
        [JQueryOption("isRTL", IgnoreValue = false)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Gets/Sets whether the current language is drawn from right to left")]
        public bool IsRightToLeft
        {
            get
            {
                Object obj = ViewState["IsRightToLeft"];
                return (obj == null) ? false : (bool)obj;
            }
            set
            {
                ViewState["IsRightToLeft"] = value;
            }
        }

        /// <summary>
        /// Gets/Sets the DatePicker 's select value
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [Description("Gets/Sets the DatePicker 's select value")]
        public DateTime Value
        {
            get
            {
                Object obj = ViewState["Value"];
                return (obj == null) ? DateTime.Today : (DateTime)obj;
            }
            set
            {
                ViewState["Value"] = value;
            }
        }

        /// <summary>
        /// Can be a function that takes an input field and current datepicker instance and returns an 
        /// options object to update the datepicker with. It is called just before the datepicker is displayed.
        /// </summary>
        [Bindable(true)]
        [Category("ClientEvents")]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [JQueryOption("beforeShow", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "input" })]
        public string OnClientBeforeShow { get; set; }

        /// <summary>
        /// The function takes a date as a parameter and must return an array with [0] equal to 
        /// true/false indicating whether or not this date is selectable, [1] equal to a CSS class name(s) or 
        /// '' for the default presentation and [2] an optional popup tooltip for this date. It is called for each 
        /// day in the datepicker before is it displayed.
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("ClientEvents")]
        [JQueryOption("beforeShowDay", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "date" })]
        public string OnClientBeforeShowDay { get; set; }

        /// <summary>
        /// Allows you to define your own event when the datepicker moves to a new month and/or year. 
        /// The function receives the selected year, month and the datepicker instance as parameters. this refers
        /// to the associated input field.
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("ClientEvents")]
        [JQueryOption("onChangeMonthYear", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "date" })]
        public string OnClientChangeMonthYear { get; set; }

        /// <summary>
        /// Allows you to define your own event when the datepicker is closed, whether or not a date is selected.
        /// The function receives the selected date as a Date and the datepicker instance as parameters. 
        /// this refers to the associated input field.
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("ClientEvents")]
        [JQueryOption("onClose", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "date" })]
        public string OnClientClose { get; set; }

        /// <summary>
        /// Allows you to define your own event when the datepicker is selected. 
        /// The function receives the selected date(s) as text and the datepicker instance as parameters. 
        /// this refers to the associated input field.
        /// </summary>
        [Bindable(true)]
        [TypeConverter(typeof(MultilineStringConverter))]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [Category("ClientEvents")]
        [JQueryOption("onSelect", Type = JQueryOptionTypes.Function, FunctionParams = new string[] { "dateText" })]
        public string OnClientSelect { get; set; }

        #endregion

        private string ClientKey
        {
            get
            {
                return ClientID + "_date";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresPostBack(this);
            base.OnInit(e);
        }

        private string EventHolder
        {
            get
            {
                Object obj = ViewState["EventHolder"];
                return (obj == null) ? null : (string)obj;
            }
            set
            {
                ViewState["EventHolder"] = value;
            }
        }

        private string GetFunctionBody()
        {
            string formatted = EventHolder;

            if (!string.IsNullOrEmpty(formatted))
            {
                if (formatted.StartsWith("function"))
                {
                    int startAt = formatted.IndexOf("{");
                    if (startAt != -1)
                    {
                        int len = formatted.Length - 1 - startAt;
                        if (len > 0)
                            formatted = formatted.Substring(startAt, len);
                    }
                }
            }
            return formatted;
        }

        protected override void OnPreRender(EventArgs e)
        {
            JQueryScriptBuilder builder = new JQueryScriptBuilder(this);
            builder.Prepare();

            if (!string.IsNullOrEmpty(ButtonImageUrl))
                builder.AddOption("buttonImage", ResolveUrl(ButtonImageUrl), true);
            else
            {
                if (ShowDefaultButtonImage)
                    builder.AddOption("buttonImage", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif"), true);
            }

            if (DisplayMode == DatePickerDisplayModes.Calendar)
                Page.ClientScript.RegisterHiddenField(ClientKey, Value.ToString());

            if ((dayNames != null) && (dayNames.Length > 0))
                builder.AddOption("dayNames", dayNames);

            if ((minDayNames != null) && (minDayNames.Length > 0))
                builder.AddOption("dayNamesMin", minDayNames);

            if ((shortDayNames != null) && (shortDayNames.Length > 0))
                builder.AddOption("dayNamesShort", shortDayNames);

            if ((monthNames != null) && (monthNames.Length > 0))
                builder.AddOption("monthNames", monthNames);

            if ((monthShortNames != null) && (monthShortNames.Length > 0))
                builder.AddOption("monthNamesShort", monthShortNames);

            if (!string.IsNullOrEmpty(DateFormatString))
                builder.AddOption("dateFormat", GetjQueryDateFormat(DateFormatString), true);

            if (!string.IsNullOrEmpty(AnotherFormatString))
                builder.AddOption("altFormat", GetjQueryDateFormat(AnotherFormatString), true);


            builder.Build();

            if (DisplayMode == DatePickerDisplayModes.Calendar)
            {
                if (!Page.IsPostBack)
                {
                    if (!string.IsNullOrEmpty(OnClientSelect))
                        EventHolder = OnClientSelect;
                }

                StringBuilder selectScripts = new StringBuilder();
                if (!string.IsNullOrEmpty(EventHolder))
                    selectScripts.Append(GetFunctionBody());
                selectScripts.Append("$get('" + ClientKey + "').value=$('#" + ClientID + "').datepicker('getDate');");
                if (AutoPostBack)
                    selectScripts.Append(Page.ClientScript.GetPostBackEventReference(this, "") + ";");
                this.OnClientSelect = selectScripts.ToString();
            }

            if (!string.IsNullOrEmpty(LocID))
            {
                string name = "jQueryNet.lang." + LocID + ".js";
                ScriptManager sm = ScriptManager.GetCurrent(Page);
                ClientScriptManager.AddCompositeScript(this, name, "jQueryNet");
                ClientScriptManager.RegisterClientApplicationInitScript(this, ClientID + "_sys_localize_init", "$.datepicker.setDefaults($.datepicker.regional['" + LocID + "']);");
            }

            if (DisplayMode == DatePickerDisplayModes.Calendar)
            {
                builder.AppendSelector();
                builder.Scripts.Append(".datepicker('setDate','" + Value.ToString() + "');");
            }

            // ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_loaded", "jQuery('#" + ClientID + "').datepicker('setDate','" + Value.ToString() + "');");

            //base.OnPreRender(e);

            ClientScriptManager.RegisterJQueryControl(this, builder);

            #region v1.0.0.0
            //Dictionary<string, string> options = new Dictionary<string, string>();

            //if (!string.IsNullOrEmpty(ButtonImageUrl))
            //    options.Add("buttonImage", "'" + ResolveUrl(ButtonImageUrl) + "'");
            //else
            //{
            //    if (ShowDefaultButtonImage)
            //        options.Add("buttonImage", "'" + Page.ClientScript.GetWebResourceUrl(this.GetType(), "DNA.UI.JQuery.DatePicker.DatePicker.gif") + "'");
            //}

            //if (DisplayMode == DatePickerDisplayModes.Calendar)
            //    Page.ClientScript.RegisterHiddenField(ClientKey, Value.ToString());

            //if (dayNames.Count > 0)
            //{
            //    List<string> ln = new List<string>();
            //    foreach (DateName dayName in dayNames)
            //        ln.Add("'" + dayName.ToString() + "'");
            //    options.Add("dayNames", "[" + String.Join(",", ln.ToArray()) + "]");
            //}

            //if (minDayNames.Count > 0)
            //{
            //    List<string> ln = new List<string>();
            //    foreach (DateName dayName in minDayNames)
            //        ln.Add("'" + dayName.ToString() + "'");
            //    options.Add("dayNamesMin", "[" + String.Join(",", ln.ToArray()) + "]");
            //}

            //if (shortDayNames.Count > 0)
            //{
            //    List<string> ln = new List<string>();
            //    foreach (DateName dayName in shortDayNames)
            //        ln.Add("'" + dayName.ToString() + "'");
            //    options.Add("dayNamesShort", "[" + String.Join(",", ln.ToArray()) + "]");
            //}

            //if (monthNames.Count > 0)
            //{
            //    List<string> ln = new List<string>();
            //    foreach (DateName dayName in monthNames)
            //        ln.Add("'" + dayName.ToString() + "'");
            //    options.Add("monthNames", "[" + String.Join(",", ln.ToArray()) + "]");
            //}

            //if (monthShortNames.Count > 0)
            //{
            //    List<string> ln = new List<string>();
            //    foreach (DateName dayName in monthShortNames)
            //        ln.Add("'" + dayName.ToString() + "'");
            //    options.Add("monthNamesShort", "[" + String.Join(",", ln.ToArray()) + "]");
            //}

            //if (!string.IsNullOrEmpty(AnotherFieldTargetID))
            //{
            //    string id = ClientScriptManager.GetControlClientID(Page, AnotherFieldTargetID);
            //    options.Add("altField", "'#" + id + "'");
            //}

            //if (!string.IsNullOrEmpty(DateFormatString))
            //    options.Add("dateFormat", "'" + GetjQueryDateFormat(DateFormatString) + "'");

            //if (!string.IsNullOrEmpty(AnotherFormatString))
            //    options.Add("altFormat", "'" + GetjQueryDateFormat(AnotherFormatString) + "'");

            //if (DisplayMode == DatePickerDisplayModes.Calendar)
            //{
            //    if (!Page.IsPostBack)
            //    {
            //        if (!string.IsNullOrEmpty(OnClientSelect))
            //            EventHolder = OnClientSelect;
            //    }

            //    StringBuilder selectScripts = new StringBuilder();
            //    if (!string.IsNullOrEmpty(EventHolder))
            //        selectScripts.Append(GetFunctionBody());
            //    selectScripts.Append("$get('" + ClientKey + "').value=$('#" + ClientID + "').datepicker('getDate');");
            //    selectScripts.Append(Page.ClientScript.GetPostBackEventReference(this, "") + ";");
            //    this.OnClientSelect = selectScripts.ToString();
            //}

            //if (!string.IsNullOrEmpty(LocID))
            //{
            //    string name = "jQuery.lang." + LocID + ".js";
            //    ScriptManager sm = ScriptManager.GetCurrent(Page);
            //    ClientScriptManager.AddCompositeScript(this, name, "jQuery");
            //    //sm.CompositeScript.Scripts.Add(new ScriptReference(name, "jQuery"));
            //    ClientScriptManager.RegisterClientApplicationInitScript(this, ClientID + "_sys_localize_init", "$.datepicker.setDefaults($.datepicker.regional['" + LocID + "']);");
            //}

            //if (options.Count == 0)
            //    ClientScriptManager.RegisterJQueryControl(this);
            //else
            //    ClientScriptManager.RegisterJQueryControl(this, options);

            //if (DisplayMode == DatePickerDisplayModes.Calendar)
            //    ClientScriptManager.RegisterClientApplicationLoadScript(this, ClientID + "_sys_loaded", "jQuery('#" + ClientID + "').datepicker('setDate','" + Value.ToString() + "');");
            //base.OnPreRender(e);
            #endregion

        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                if (DisplayMode == DatePickerDisplayModes.Picker)
                    return HtmlTextWriterTag.Input;
                else
                    return HtmlTextWriterTag.Div;
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddAttribute("name", UniqueID);
            if (DisplayMode == DatePickerDisplayModes.Picker)
            {
                if (!string.IsNullOrEmpty(DateFormatString))
                    writer.AddAttribute("value", Value.ToString(DateFormatString));
                else
                    writer.AddAttribute("value", Value.ToString("MM/dd/yyyy"));
            }
            base.RenderBeginTag(writer);
        }

        #region IPostBackDataHandler Members

        bool IPostBackDataHandler.LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            string value = "";

            if (DisplayMode == DatePickerDisplayModes.Calendar)
                value = postCollection[ClientKey];
            else
                value = postCollection[postDataKey];

            DateTime dateValue = DateTime.MinValue;
            DateTime.TryParse(value, out dateValue);

            if (dateValue != Value)
            {
                Value = dateValue;
                return true;
            }

            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            if (DateSelected != null)
                DateSelected(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Convert .Net DateTime Format to jQuery DateTime Format 
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string GetjQueryDateFormat(string format)
        {
            string formatted = format;
            //Format year
            if (formatted.IndexOf("yy") > -1)
                formatted = formatted.Replace("yy", "y");

            //if (formatted.IndexOf("yy") > -1)
            //    formatted = formatted.Replace("yy", "y");

            if (formatted.IndexOf("MMMM") > -1)
                formatted = formatted.Replace("MMMM", "MM");
            else
            {
                if (formatted.IndexOf("MMM") > -1)
                    formatted = formatted.Replace("MMM", "M");
                else
                {
                    if (formatted.IndexOf("MM") > -1)
                        formatted = formatted.Replace("MM", "mm");
                    else
                        if (formatted.IndexOf("M") > -1)
                            formatted = formatted.Replace("M", "m");
                }
            }

            if (formatted.IndexOf("dddd") > -1)
                formatted = formatted.Replace("dddd", "DD");

            if (formatted.IndexOf("ddd") > -1)
                formatted = formatted.Replace("ddd", "D");

            return formatted;
        }
    }
}
