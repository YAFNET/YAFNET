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
  using System;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using DNA.UI.JQuery;
  using YAF.Classes.Core;

  /// <summary>
  /// Summary description for PagePopupModule
  /// </summary>
  [YafModule("Page Popup Module", "Tiny Gecko", 1)]
  public class PagePopupModule : SimpleBaseModule
  {
    /// <summary>
    /// The _error popup.
    /// </summary>
    protected PopupDialogNotification _errorPopup = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagePopupModule"/> class.
    /// </summary>
    public PagePopupModule()
    {
    }

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      ForumControl.Init += new EventHandler(ForumControl_Init);
    }

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this._errorPopup.Title = PageContext.Localization.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
      CurrentForumPage.PreRender += new EventHandler(CurrentForumPage_PreRender);
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
    private void CurrentForumPage_PreRender(object sender, EventArgs e)
    {
      RegisterLoadString();
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
    private void ForumControl_Init(object sender, EventArgs e)
    {
      // at this point, init has already been called...
      AddErrorPopup();
    }

    /// <summary>
    /// Sets up the Modal Error Popup Dialog
    /// </summary>
    private void AddErrorPopup()
    {
      if (ForumControl.FindControl("YafForumPageErrorPopup1") == null)
      {
        // add error control...
        this._errorPopup = new PopupDialogNotification();
        this._errorPopup.ID = "YafForumPageErrorPopup1";

        ForumControl.Controls.Add(this._errorPopup);
      }
      else
      {
        // reference existing control...
        this._errorPopup = (PopupDialogNotification) ForumControl.FindControl("YafForumPageErrorPopup1");
      }
    }

    /// <summary>
    /// The register load string.
    /// </summary>
    protected void RegisterLoadString()
    {
      PageContext.PageElements.RegisterJQuery();

      if (PageContext.LoadMessage.LoadString.Length > 0)
      {
        if (ScriptManager.GetCurrent(ForumControl.Page) != null)
        {
          string displayMessage = PageContext.LoadMessage.LoadStringDelimited("<br />");
          
          //Get the clean JS string.
          displayMessage = YAF.Classes.Core.LoadMessage.CleanJsString(displayMessage);
          PageContext.PageElements.RegisterJsBlockStartup(
            ForumControl.Page, 
            "modalNotification", 
            String.Format(
              "var fpModal = function() {1} {3}('{0}'); Sys.Application.remove_load(fpModal); {2}\nSys.Application.add_load(fpModal);\n\n", 
              displayMessage, 
              '{', 
              '}', 
              this._errorPopup.ShowModalFunction));
        }
      }
    }
  }

  /// <summary>
  /// The popup dialog notification.
  /// </summary>
  public class PopupDialogNotification : Dialog
  {
    /// <summary>
    /// The _okay button.
    /// </summary>
    protected DialogButton _okayButton = new DialogButton();

    /// <summary>
    /// The _template.
    /// </summary>
    protected ErrorPopupCustomTemplate _template = new ErrorPopupCustomTemplate();

    /// <summary>
    /// Initializes a new instance of the <see cref="PopupDialogNotification"/> class.
    /// </summary>
    public PopupDialogNotification()
      : base()
    {
    }

    /// <summary>
    /// Gets ShowModalFunction.
    /// </summary>
    public string ShowModalFunction
    {
      get
      {
        return string.Format("ShowPopupDialogNotification{0}", ClientID);
      }
    }

    /// <summary>
    /// Gets MainTextClientID.
    /// </summary>
    public string MainTextClientID
    {
      get
      {
        return this._template.SpanInnerMessage.ClientID;
      }
    }

    /// <summary>
    /// The on load.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this._okayButton.Text = YafContext.Current.Localization.GetText("COMMON", "OK");
      Title = YafContext.Current.Localization.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);

      // add js for client-side error settings...
      string jsFunction =
        String.Format(
          "\n{4} = function( newErrorStr ) {2}\n if (newErrorStr != null && newErrorStr != \"\" && jQuery('#{1}') != null) {2}\njQuery('#{1}').html(newErrorStr);\njQuery('#{0}').dialog('open');\n{3}\n{3}\n", 
          ClientID, 
          MainTextClientID, 
          '{', 
          '}', 
          ShowModalFunction);
      YafContext.Current.PageElements.RegisterJsBlock(this, ShowModalFunction, jsFunction);
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // init the popup first...
      base.OnInit(e);

      // make a few changes for this type of modal...
      ShowModal = true;
      IsDraggable = true;
      IsResizable = false;
      DialogButtons = DialogButtons.OK;
      Width = Unit.Pixel(400);

      BodyTemplate = this._template;

      this._okayButton.Text = "OK";
      this._okayButton.OnClientClick = "jQuery(this).dialog('close');";
      Buttons.Add(this._okayButton);
    }

    #region Nested type: ErrorPopupCustomTemplate

    /// <summary>
    /// The error popup custom template.
    /// </summary>
    public class ErrorPopupCustomTemplate : ITemplate
    {
      /// <summary>
      /// The span inner message.
      /// </summary>
      public HtmlGenericControl SpanInnerMessage = new HtmlGenericControl("span");

      /// <summary>
      /// The span outer message.
      /// </summary>
      public HtmlGenericControl SpanOuterMessage = new HtmlGenericControl("span");

      #region ITemplate Members

      /// <summary>
      /// The instantiate in.
      /// </summary>
      /// <param name="container">
      /// The container.
      /// </param>
      public void InstantiateIn(Control container)
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
    }

    #endregion
  }
}