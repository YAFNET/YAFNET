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
  using System.Web.UI.WebControls;

  using ClientScriptManager = DNA.UI.ClientScriptManager;

  #endregion

  /// <summary>
  /// JQuerySelector this a class that define the jQuery selector string build rules,this class can build jQuery selector
  ///   string for html elements or write the selector string in Selector property directly. Or use TargetID,TargetIDs to 
  ///   get the server controls client id for render.
  /// </summary>
  [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public class JQuerySelector
  {
    #region Constants and Fields

    /// <summary>
    /// The _selector.
    /// </summary>
    private string _selector = string.Empty;

    /// <summary>
    /// The expression only.
    /// </summary>
    private bool expressionOnly;

    /// <summary>
    /// The no conflict.
    /// </summary>
    private bool noConflict = true;

    /// <summary>
    /// The target id.
    /// </summary>
    private string targetID;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="JQuerySelector"/> class.
    /// </summary>
    public JQuerySelector()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQuerySelector"/> class.
    /// </summary>
    /// <param name="selector">
    /// The selector.
    /// </param>
    public JQuerySelector(string selector)
    {
      this._selector = selector;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQuerySelector"/> class.
    /// </summary>
    /// <param name="selectors">
    /// The selectors.
    /// </param>
    public JQuerySelector(params string[] selectors)
    {
      this._selector = string.Join(",", selectors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JQuerySelector"/> class.
    /// </summary>
    /// <param name="control">
    /// The control.
    /// </param>
    public JQuerySelector(Control control)
    {
      this._selector = "#" + control.ClientID;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets/Sets whether the selector output append "$()"
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ExpressionOnly
    {
      get
      {
        return this.expressionOnly;
      }

      set
      {
        this.expressionOnly = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsEmpty.
    /// </summary>
    [Browsable(false)]
    public bool IsEmpty
    {
      get
      {
        return string.IsNullOrEmpty(this._selector) && (string.IsNullOrEmpty(this.TargetID)) &&
                (this.TargetIDs == null);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether NoConflict.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public bool NoConflict
    {
      get
      {
        return this.noConflict;
      }

      set
      {
        this.noConflict = value;
      }
    }

    /// <summary>
    /// Gets or sets Selector.
    /// </summary>
    [NotifyParentProperty(true)]
    [Bindable(true)]
    public string Selector
    {
      get
      {
        return this._selector;
      }

      set
      {
        this._selector = value;
      }
    }

    /// <summary>
    /// Gets or sets TargetID.
    /// </summary>
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(ControlIDConverter))]
    [Bindable(true)]
    public string TargetID
    {
      get
      {
        return this.targetID;
      }

      set
      {
        this.targetID = value;
      }
    }

    /// <summary>
    /// Gets or sets TargetIDs.
    /// </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(StringArrayConverter))]
    public string[] TargetIDs { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clear the selector and the IsEmpty will be return true.
    /// </summary>
    public void Clear()
    {
      this._selector = string.Empty;
      this.targetID = string.Empty;
      this.TargetIDs = null;
    }

    /// <summary>
    /// Format the selector to string in jQuery format.
    /// </summary>
    /// <remarks>
    /// this method will not transfer the targetid or targetids to ClientID
    /// </remarks>
    /// <returns>
    /// The to string.
    /// </returns>
    public override string ToString()
    {
      if (this.IsEmpty)
      {
        if (this.noConflict)
        {
          return "jQuery";
        }
        else
        {
          return "$";
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(this._selector))
        {
          return this.GetSelectorString(this._selector);
        }

        if (this.TargetIDs != null)
        {
          var ts = new string[this.TargetIDs.Length];
          for (int i = 0; i < ts.Length; i++)
          {
            if (!this.TargetIDs[i].StartsWith("#"))
            {
              ts[i] = "#" + this.TargetIDs[i];
            }
            else
            {
              ts[i] = this.TargetIDs[i];
            }
          }

          return this.GetSelectorString(string.Join(",", ts));
        }

        return this.GetSelectorString("#" + this.targetID);
      }
    }

    /// <summary>
    /// Format the selector to string in jQuery format
    /// </summary>
    /// <remarks>
    /// If targetID,targetIDs sets this method will get their ClientID into the output string.
    /// </remarks>
    /// <param name="page">
    /// </param>
    /// <returns>
    /// The to string.
    /// </returns>
    public string ToString(Page page)
    {
      if (this.IsEmpty)
      {
        if (this.noConflict)
        {
          return "jQuery";
        }
        else
        {
          return "$";
        }
      }
      else
      {
        if (!string.IsNullOrEmpty(this._selector))
        {
          return this.GetSelectorString(this._selector);
        }

        if (this.TargetIDs != null)
        {
          var ts = new string[this.TargetIDs.Length];
          for (int i = 0; i < ts.Length; i++)
          {
            ts[i] = "#" + this.FormattedServerSelector(page, this.TargetIDs[i]);
          }

          return this.GetSelectorString(string.Join(",", ts));
        }

        return this.GetSelectorString("#" + this.FormattedServerSelector(page, this.targetID));
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The formatted server selector.
    /// </summary>
    /// <param name="page">
    /// The page.
    /// </param>
    /// <param name="exp">
    /// The exp.
    /// </param>
    /// <returns>
    /// The formatted server selector.
    /// </returns>
    private string FormattedServerSelector(Page page, string exp)
    {
      int index = this.SubExpIndex(exp);
      if (index == -1)
      {
        return ClientScriptManager.GetControlClientID(page, exp);
      }

      return ClientScriptManager.GetControlClientID(page, exp.Substring(0, index)) +
             exp.Substring(index, exp.Length - index);
    }

    /// <summary>
    /// The get selector string.
    /// </summary>
    /// <param name="_s">
    /// The _s.
    /// </param>
    /// <returns>
    /// The get selector string.
    /// </returns>
    private string GetSelectorString(string _s)
    {
      if (this.expressionOnly)
      {
        return "'" + _s + "'";
      }
      else
      {
        if (this.noConflict)
        {
          return "jQuery(\"" + _s + "\")";
        }
        else
        {
          return "$(\"" + _s + "\")";
        }
      }
    }

    /// <summary>
    /// The sub exp index.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// The sub exp index.
    /// </returns>
    private int SubExpIndex(string id)
    {
      if (id.IndexOf(" ") > -1)
      {
        return id.IndexOf(" ");
      }

      if (id.IndexOf(">") > -1)
      {
        return id.IndexOf(">");
      }

      if (id.IndexOf("+") > -1)
      {
        return id.IndexOf("+");
      }

      if (id.IndexOf(":") > -1)
      {
        return id.IndexOf(":");
      }

      if (id.IndexOf("[") > -1)
      {
        return id.IndexOf("]");
      }

      return -1;
    }

    #endregion
  }
}