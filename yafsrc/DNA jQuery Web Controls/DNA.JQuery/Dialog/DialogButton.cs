//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
  #region Using

  using System.ComponentModel;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.UI;

  #endregion

  /// <summary>
  /// The dialog button.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class DialogButton
  {
    #region Constants and Fields

    /// <summary>
    /// The command argument.
    /// </summary>
    private string commandArgument = string.Empty;

    /// <summary>
    /// The command name.
    /// </summary>
    private string commandName = string.Empty;

    /// <summary>
    /// The on client click.
    /// </summary>
    private string onClientClick = string.Empty;

    /// <summary>
    /// The text.
    /// </summary>
    private string text = string.Empty;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets the command arguments
    /// </summary>
    [Category("Action")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [Description("Gets/Sets the command arguments")]
    public string CommandArgument
    {
      get
      {
        return this.commandArgument;
      }

      set
      {
        this.commandArgument = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the command name
    /// </summary>
    [Category("Action")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [Description("Gets/Sets the command name")]
    public string CommandName
    {
      get
      {
        return this.commandName;
      }

      set
      {
        this.commandName = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the Button's client event handler
    /// </summary>
    [Category("Behavior")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [Description("Gets/Sets the Button's client event handler")]
    public string OnClientClick
    {
      get
      {
        return this.onClientClick;
      }

      set
      {
        this.onClientClick = value;
      }
    }

    /// <summary>
    ///   Gets/Sets the display text of button
    /// </summary>
    [Category("Data")]
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [Description("Gets/Sets the display text of button")]
    [Localizable(true)]
    public string Text
    {
      get
      {
        return this.text;
      }

      set
      {
        this.text = value;
      }
    }

    #endregion
  }
}