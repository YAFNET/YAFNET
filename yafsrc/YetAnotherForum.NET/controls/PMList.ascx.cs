/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Controls
{
  using System;
  using System.ComponentModel;
  using System.Data;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// The pm list.
  /// </summary>
  public partial class PMList : BaseUserControl
  {
    /// <summary>
    /// Gets or sets the current view for the user's private messages.
    /// </summary>
    [Category("Behavior")]
    [Description("Gets or sets the current view for the user's private messages.")]
    public PMView View
    {
      get
      {
        if (ViewState["View"] != null)
        {
          return (PMView) ViewState["View"];
        }
        else
        {
          return PMView.Inbox;
        }
      }

      set
      {
        ViewState["View"] = value;
      }
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (ViewState["SortField"] == null)
      {
        SetSort("Created", false);
      }

      if (!IsPostBack)
      {
        // setup pager...
        this.MessagesView.AllowPaging = true;
        this.MessagesView.PagerSettings.Visible = false;
        this.MessagesView.AllowSorting = true;

        this.PagerTop.PageSize = 10;
        this.MessagesView.PageSize = 10;
      }
      else
      {
        // make sure addLoadMessage is empty...
        PageContext.LoadMessage.Clear();
      }

      BindData();
    }

    /// <summary>
    /// The get title.
    /// </summary>
    /// <returns>
    /// The get title.
    /// </returns>
    protected string GetTitle()
    {
      if (View == PMView.Outbox)
      {
        return GetLocalizedText("SENTITEMS");
      }
      else if (View == PMView.Inbox)
      {
        return GetLocalizedText("INBOX");
      }
      else
      {
        return GetLocalizedText("ARCHIVE");
      }
    }

    /// <summary>
    /// The get localized text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <returns>
    /// The get localized text.
    /// </returns>
    protected string GetLocalizedText(string text)
    {
      return HtmlEncode(PageContext.Localization.GetText(text));
    }

    /// <summary>
    /// The get p message text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="_total">
    /// The _total.
    /// </param>
    /// <param name="_inbox">
    /// The _inbox.
    /// </param>
    /// <param name="_outbox">
    /// The _outbox.
    /// </param>
    /// <param name="_archive">
    /// The _archive.
    /// </param>
    /// <param name="_limit">
    /// The _limit.
    /// </param>
    /// <returns>
    /// The get p message text.
    /// </returns>
    protected string GetPMessageText(string text, object _total, object _inbox, object _outbox, object _archive, object _limit)
    {
      object _percentage = 0;
      if (Convert.ToInt32(_limit) != 0)
      {
        _percentage = decimal.Round((Convert.ToDecimal(_total) / Convert.ToDecimal(_limit)) * 100, 2);
      }

      if (YafContext.Current.IsAdmin)
      {
        _limit = "\u221E";
        _percentage = 0;
      }

      return HtmlEncode(PageContext.Localization.GetTextFormatted(text, _total, _inbox, _outbox, _archive, _limit, _percentage));
    }

    /// <summary>
    /// The get message user header.
    /// </summary>
    /// <returns>
    /// The get message user header.
    /// </returns>
    protected string GetMessageUserHeader()
    {
      return GetLocalizedText(View == PMView.Outbox ? "to" : "from");
    }

    /// <summary>
    /// The get message link.
    /// </summary>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// The get message link.
    /// </returns>
    protected string GetMessageLink(object messageId)
    {
      return YafBuildLink.GetLink(ForumPages.cp_message, "pm={0}&v={1}", messageId, PMViewConverter.ToQueryStringParam(View));
    }

    /// <summary>
    /// The format body.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The format body.
    /// </returns>
    protected string FormatBody(object o)
    {
      var row = (DataRowView) o;
      return (string) row["Body"];
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      object toUserID = null;
      object fromUserID = null;
      if (View == PMView.Outbox)
      {
        fromUserID = PageContext.PageUserID;
      }
      else
      {
        toUserID = PageContext.PageUserID;
      }

      using (DataView dv = DB.pmessage_list(toUserID, fromUserID, null).DefaultView)
      {
        if (View == PMView.Inbox)
        {
          dv.RowFilter = "IsDeleted = False AND IsArchived = False";
        }
        else if (View == PMView.Outbox)
        {
          dv.RowFilter = "IsInOutbox = True";
        }
        else if (View == PMView.Archive)
        {
          dv.RowFilter = "IsArchived = True";
        }

        dv.Sort = String.Format("{0} {1}", ViewState["SortField"], (bool) ViewState["SortAsc"] ? "asc" : "desc");
        this.PagerTop.Count = dv.Count;

        this.MessagesView.PageIndex = this.PagerTop.CurrentPageIndex;
        this.MessagesView.DataSource = dv;
        this.MessagesView.DataBind();
      }

      Stats_Renew();
    }

    /// <summary>
    /// The archive selected_ click.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ArchiveSelected_Click(object source, EventArgs e)
    {
      if (View != PMView.Inbox)
      {
        return;
      }

      long archivedCount = 0;
      foreach (GridViewRow item in this.MessagesView.Rows)
      {
        if (((CheckBox) item.FindControl("ItemCheck")).Checked)
        {
          DB.pmessage_archive(this.MessagesView.DataKeys[item.RowIndex].Value);
          archivedCount++;
        }
      }

      BindData();

      if (archivedCount == 1)
      {
        PageContext.AddLoadMessage(PageContext.Localization.GetText("MSG_ARCHIVED"));
      }
      else
      {
        PageContext.AddLoadMessage(String.Format(PageContext.Localization.GetText("MSG_ARCHIVED+"), archivedCount));
      }
    }

    /// <summary>
    /// The archive all_ click.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ArchiveAll_Click(object source, EventArgs e)
    {
      if (View != PMView.Inbox)
      {
        return;
      }

      long archivedCount = 0;
      using (DataView dv = DB.pmessage_list(PageContext.PageUserID, null, null).DefaultView)
      {
        dv.RowFilter = "IsDeleted = False AND IsArchived = False";

        foreach (DataRowView item in dv)
        {
          DB.pmessage_archive(item["UserPMessageID"]);
          archivedCount++;
        }
      }

      BindData();
      PageContext.AddLoadMessage(String.Format(PageContext.Localization.GetText("MSG_ARCHIVED+"), archivedCount));
    }

    /// <summary>
    /// The mark as read_ click.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MarkAsRead_Click(object source, EventArgs e)
    {
      if (View == PMView.Outbox)
      {
        return;
      }

      using (DataView dv = DB.pmessage_list(PageContext.PageUserID, null, null).DefaultView)
      {
        if (View == PMView.Inbox)
        {
          dv.RowFilter = "IsRead = False AND IsDeleted = False AND IsArchived = False";
        }
        else if (View == PMView.Archive)
        {
          dv.RowFilter = "IsRead = False AND IsArchived = True";
        }

        foreach (DataRowView item in dv)
        {
          DB.pmessage_markread(item["UserPMessageID"]);
        }
      }

      BindData();
    }

    /// <summary>
    /// The delete selected_ click.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteSelected_Click(object source, EventArgs e)
    {
      long nItemCount = 0;
      foreach (GridViewRow item in this.MessagesView.Rows)
      {
        if (((CheckBox) item.FindControl("ItemCheck")).Checked)
        {
          if (View == PMView.Outbox)
          {
            DB.pmessage_delete(this.MessagesView.DataKeys[item.RowIndex].Value, true);
          }
          else
          {
            DB.pmessage_delete(this.MessagesView.DataKeys[item.RowIndex].Value);
          }

          nItemCount++;
        }
      }

      BindData();
      if (nItemCount == 1)
      {
        PageContext.AddLoadMessage(PageContext.Localization.GetText("msgdeleted1"));
      }
      else
      {
        PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("msgdeleted2", nItemCount));
      }
    }

    /// <summary>
    /// The delete all_ click.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAll_Click(object source, EventArgs e)
    {
      long nItemCount = 0;

      object toUserID = null;
      object fromUserID = null;
      bool isoutbox = false;

      if (View == PMView.Outbox)
      {
        fromUserID = PageContext.PageUserID;
        isoutbox = true;
      }
      else
      {
        toUserID = PageContext.PageUserID;
      }

      using (DataView dv = DB.pmessage_list(toUserID, fromUserID, null).DefaultView)
      {
        if (View == PMView.Inbox)
        {
          dv.RowFilter = "IsDeleted = False AND IsArchived = False";
        }
        else if (View == PMView.Outbox)
        {
          dv.RowFilter = "IsInOutbox = True";
        }
        else if (View == PMView.Archive)
        {
          dv.RowFilter = "IsArchived = True";
        }

        foreach (DataRowView item in dv)
        {
          if (isoutbox)
          {
            DB.pmessage_delete(item["UserPMessageID"], true);
          }
          else
          {
            DB.pmessage_delete(item["UserPMessageID"]);
          }

          nItemCount++;
        }
      }

      BindData();
      PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("msgdeleted2", nItemCount));
    }

    /// <summary>
    /// The get image.
    /// </summary>
    /// <param name="o">
    /// The o.
    /// </param>
    /// <returns>
    /// The get image.
    /// </returns>
    protected string GetImage(object o)
    {
      if (SqlDataLayerConverter.VerifyBool(((DataRowView) o)["IsRead"]))
      {
        return PageContext.Theme.GetItem("ICONS", "TOPIC");
      }
      else
      {
        return PageContext.Theme.GetItem("ICONS", "TOPIC_NEW");
      }
    }

    /// <summary>
    /// The set sort.
    /// </summary>
    /// <param name="field">
    /// The field.
    /// </param>
    /// <param name="asc">
    /// The asc.
    /// </param>
    private void SetSort(string field, bool asc)
    {
      if (ViewState["SortField"] != null && (string) ViewState["SortField"] == field)
      {
        ViewState["SortAsc"] = !(bool) ViewState["SortAsc"];
      }
      else
      {
        ViewState["SortField"] = field;
        ViewState["SortAsc"] = asc;
      }
    }

    /// <summary>
    /// The subject link_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SubjectLink_Click(object sender, EventArgs e)
    {
      SetSort("Subject", true);
      BindData();
    }

    /// <summary>
    /// The from link_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void FromLink_Click(object sender, EventArgs e)
    {
      if (View == PMView.Outbox)
      {
        SetSort("ToUser", true);
      }
      else
      {
        SetSort("FromUser", true);
      }

      BindData();
    }

    /// <summary>
    /// The date link_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DateLink_Click(object sender, EventArgs e)
    {
      SetSort("Created", false);
      BindData();
    }

    /// <summary>
    /// The archive all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ArchiveAll_Load(object sender, EventArgs e)
    {
      ((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", PageContext.Localization.GetText("CONFIRM_ARCHIVEALL"));
    }

    /// <summary>
    /// The delete selected_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteSelected_Load(object sender, EventArgs e)
    {
      ((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", PageContext.Localization.GetText("CONFIRM_DELETE"));
    }

    /// <summary>
    /// The delete all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteAll_Load(object sender, EventArgs e)
    {
      ((ThemeButton) sender).Attributes["onclick"] = String.Format("return confirm('{0}')", PageContext.Localization.GetText("CONFIRM_DELETEALL"));
    }

    /// <summary>
    /// The messages view_ row created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void MessagesView_RowCreated(object sender, GridViewRowEventArgs e)
    {
      if (e.Row.RowType == DataControlRowType.Header)
      {
        var oGridView = (GridView) sender;
        var oGridViewRow = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert);
        var oTableCell = new TableCell();

        // Add Header to top with column span of 5... no need for two tables.
        oTableCell.Text = GetTitle();
        oTableCell.CssClass = "header1";
        oTableCell.ColumnSpan = 5;
        oGridViewRow.Cells.Add(oTableCell);
        oGridView.Controls[0].Controls.AddAt(0, oGridViewRow);

        var SortFrom = (Image) e.Row.FindControl("SortFrom");
        var SortSubject = (Image) e.Row.FindControl("SortSubject");
        var SortDate = (Image) e.Row.FindControl("SortDate");

        SortFrom.Visible = (View == PMView.Outbox) ? (string) ViewState["SortField"] == "ToUser" : (string) ViewState["SortField"] == "FromUser";
        SortFrom.ImageUrl = PageContext.Theme.GetItem("SORT", (bool) ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");

        SortSubject.Visible = (string) ViewState["SortField"] == "Subject";
        SortSubject.ImageUrl = PageContext.Theme.GetItem("SORT", (bool) ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");

        SortDate.Visible = (string) ViewState["SortField"] == "Created";
        SortDate.ImageUrl = PageContext.Theme.GetItem("SORT", (bool) ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
      }
      else if (e.Row.RowType == DataControlRowType.Footer)
      {
        int rolCount = e.Row.Cells.Count;

        for (int i = rolCount - 1; i >= 1; i--)
        {
          e.Row.Cells.RemoveAt(i);
        }

        e.Row.Cells[0].ColumnSpan = rolCount;
      }
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
      // rebind
      BindData();
    }

    /// <summary>
    /// The stats_ renew.
    /// </summary>
    protected void Stats_Renew()
    {
      // Renew PM Statistics
      DataTable dt = DB.user_pmcount(PageContext.PageUserID);
      if (dt.Rows.Count > 0)
      {
        this.PMInfoLink.Text = GetPMessageText(
          "PMLIMIT_ALL", dt.Rows[0]["NumberTotal"], dt.Rows[0]["NumberIn"], dt.Rows[0]["NumberOut"], dt.Rows[0]["NumberArchived"], dt.Rows[0]["NumberAllowed"]);
      }
    }
  }

  /// <summary>
  /// Indicates the mode of the PMList.
  /// </summary>
  public enum PMView
  {
    /// <summary>
    /// The inbox.
    /// </summary>
    Inbox = 0, 

    /// <summary>
    /// The outbox.
    /// </summary>
    Outbox, 

    /// <summary>
    /// The archive.
    /// </summary>
    Archive
  }

  /// <summary>
  /// Converts <see cref="PMView"/>s to and from their URL query string representations.
  /// </summary>
  public static class PMViewConverter
  {
    /// <summary>
    /// Converts a <see cref="PMView"/> to a string representation appropriate for inclusion in a URL query string.
    /// </summary>
    /// <param name="view">
    /// </param>
    /// <returns>
    /// The to query string param.
    /// </returns>
    public static string ToQueryStringParam(PMView view)
    {
      if (view == PMView.Outbox)
      {
        return "out";
      }
      else if (view == PMView.Inbox)
      {
        return "in";
      }
      else if (view == PMView.Archive)
      {
        return "arch";
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Returns a <see cref="PMView"/> based on its URL query string value.
    /// </summary>
    /// <param name="param">
    /// </param>
    /// <returns>
    /// </returns>
    public static PMView FromQueryString(string param)
    {
      if (String.IsNullOrEmpty(param))
      {
        return PMView.Inbox;
      }

      switch (param.ToLower())
      {
        case "out":
          return PMView.Outbox;
        case "in":
          return PMView.Inbox;
        case "arch":
          return PMView.Archive;
        default: // Inbox by default
          return PMView.Inbox;
      }
    }
  }
}