/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Modules
{
  #region Using

  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using DNA.UI.JQuery;

  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for PagePopupModule
  /// </summary>
  [YafModule("Page Popup Module", "Tiny Gecko", 1)]
  public class PagePopupModule : SimpleBaseModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _error popup.
    /// </summary>
    protected PopupDialogNotification _errorPopup;

    #endregion

    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this._errorPopup.Title = this.PageContext.Localization.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
      this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
    }

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.ForumControl.Init += this.ForumControl_Init;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The register load string.
    /// </summary>
    protected void RegisterLoadString()
    {
      this.PageContext.PageElements.RegisterJQuery();

      if (this.PageContext.LoadMessage.LoadString.Length > 0)
      {
        if (ScriptManager.GetCurrent(this.ForumControl.Page) != null)
        {
          string displayMessage = this.PageContext.LoadMessage.LoadStringDelimited("<br />");

          // Get the clean JS string.
          displayMessage = LoadMessage.CleanJsString(displayMessage);
          this.PageContext.PageElements.RegisterJsBlockStartup(
            this.ForumControl.Page, 
            "modalNotification", 
            "var fpModal = function() {1} {3}('{0}'); Sys.Application.remove_load(fpModal); {2}\nSys.Application.add_load(fpModal);\n\n"
              .FormatWith(displayMessage, '{', '}', this._errorPopup.ShowModalFunction));
        }
      }
    }

    /// <summary>
    /// Sets up the Modal Error Popup Dialog
    /// </summary>
    private void AddErrorPopup()
    {
      if (this.ForumControl.FindControl("YafForumPageErrorPopup1") == null)
      {
        // add error control...
        this._errorPopup = new PopupDialogNotification();
        this._errorPopup.ID = "YafForumPageErrorPopup1";

        this.ForumControl.Controls.Add(this._errorPopup);
      }
      else
      {
        // reference existing control...
        this._errorPopup = (PopupDialogNotification)this.ForumControl.FindControl("YafForumPageErrorPopup1");
      }
    }

    /// <summary>
    /// The current forum page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RegisterLoadString();
    }

    /// <summary>
    /// The forum control_ init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_Init([NotNull] object sender, [NotNull] EventArgs e)
    {
      // at this point, init has already been called...
      this.AddErrorPopup();
    }

    #endregion
  }

  /// <summary>
  /// The popup dialog notification.
  /// </summary>
  public class PopupDialogNotification : Dialog
  {
    #region Constants and Fields

    /// <summary>
    ///   The _okay button.
    /// </summary>
    protected DialogButton _okayButton = new DialogButton();

    /// <summary>
    ///   The _template.
    /// </summary>
    protected ErrorPopupCustomTemplate _template = new ErrorPopupCustomTemplate();

    #endregion

    #region Properties

    /// <summary>
    ///   Gets MainTextClientID.
    /// </summary>
    [NotNull]
    public string MainTextClientID
    {
      get
      {
        return this._template.SpanInnerMessage.ClientID;
      }
    }

    /// <summary>
    ///   Gets ShowModalFunction.
    /// </summary>
    public string ShowModalFunction
    {
      get
      {
        return "ShowPopupDialogNotification{0}".FormatWith(this.ClientID);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      // init the popup first...
      base.OnInit(e);

      // make a few changes for this type of modal...
      this.ShowModal = true;
      this.IsDraggable = true;
      this.IsResizable = false;
      this.DialogButtons = DialogButtons.OK;
      this.Width = Unit.Pixel(400);

      this.BodyTemplate = this._template;

      this._okayButton.Text = "OK";
      this._okayButton.OnClientClick = "jQuery(this).dialog('close');";
      this.Buttons.Add(this._okayButton);
    }

    /// <summary>
    /// The on load.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad([NotNull] EventArgs e)
    {
      base.OnLoad(e);

      this._okayButton.Text = YafContext.Current.Localization.GetText("COMMON", "OK");
      this.Title = YafContext.Current.Localization.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      base.OnPreRender(e);

      // add js for client-side error settings...
      string jsFunction =
        "\n{4} = function( newErrorStr ) {2}\n if (newErrorStr != null && newErrorStr != \"\" && jQuery('#{1}') != null) {2}\njQuery('#{1}').html(newErrorStr);\njQuery('#{0}').dialog('open');\n{3}\n{3}\n"
          .FormatWith(this.ClientID, this.MainTextClientID, '{', '}', this.ShowModalFunction);
      YafContext.Current.PageElements.RegisterJsBlock(this, this.ShowModalFunction, jsFunction);
    }

    #endregion

    /// <summary>
    /// The error popup custom template.
    /// </summary>
    public class ErrorPopupCustomTemplate : ITemplate
    {
      #region Constants and Fields

      /// <summary>
      ///   The span inner message.
      /// </summary>
      public HtmlGenericControl SpanInnerMessage = new HtmlGenericControl("span");

      /// <summary>
      ///   The span outer message.
      /// </summary>
      public HtmlGenericControl SpanOuterMessage = new HtmlGenericControl("span");

      #endregion

      #region Implemented Interfaces

      #region ITemplate

      /// <summary>
      /// The instantiate in.
      /// </summary>
      /// <param name="container">
      /// The container.
      /// </param>
      public void InstantiateIn([NotNull] Control container)
      {
        this.SpanOuterMessage.ID = "YafPopupErrorMessageOuter";
        this.SpanOuterMessage.Attributes.Add("class", "modalOuter");

        this.SpanInnerMessage.ID = "YafPopupErrorMessageInner";
        this.SpanInnerMessage.Attributes.Add("class", "modalInner");

        this.SpanInnerMessage.InnerText = "ERROR";

        this.SpanOuterMessage.Controls.Add(this.SpanInnerMessage);

        container.Controls.Add(this.SpanOuterMessage);
      }

      #endregion

      #endregion
    }
  }
}