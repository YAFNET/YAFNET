/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj?rnar Henden
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

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Data.SqlTypes;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  public enum ThanksListMode
  {
    FromUser,
    ToUser
  }

  /// <summary>
  /// Summary description for buddies.
  /// </summary>
  public partial class ViewThanksList : BaseUserControl
  {
    /* Data Fields */
    #region Constants and Fields

    /// <summary>
    /// the current mode.
    /// </summary>
     private ThanksListMode _controlMode;

    /// <summary>
    /// the thanks info.
    /// </summary>
    private DataTable _thanksInfo = null;

    /// <summary>
    /// The _user id.
    /// </summary>
    private int _userID;

    #endregion



    /* Properties */
    #region Properties


    /// <summary>
    /// The Thanks Info.
    /// </summary>
    public DataTable ThanksInfo
    {
        get
        {
            return this._thanksInfo;
        }
        set
        {
            this._thanksInfo = value;
        }

    }


    /// <summary>
    /// Determines what is th current mode of the control.
    /// </summary>
    public ThanksListMode CurrentMode
    {
      get
      {
        return this._controlMode;
      }

      set
      {
        this._controlMode = value;
      }
    }
    /// <summary>
    /// The User ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return this._userID;
        }

        set
        {
            this._userID = value;
        }
    }

    #endregion

    /* Event Handlers */

    /* Methods */
    #region Public Methods

    /// <summary>
    /// The bind data.
    /// </summary>
    public void BindData()
    {
        // we want to page results
        var pds = new PagedDataSource();
        pds.AllowPaging = true;
        if (!(ThanksInfo.Columns.Contains("MessageThanksNumber")))
        {
            ThanksInfo.Columns.Add("MessageThanksNumber");
        }

        // now depending on mode filter the table
        DataView dvThanksInfo = ThanksInfo.DefaultView;
        if (dvThanksInfo.Count == 0)
        {
            NoResults.Visible = true;
            return;
        }

        dvThanksInfo.RowFilter = "";
        if (this.CurrentMode == ThanksListMode.FromUser)
        {
            dvThanksInfo.RowFilter = String.Format("ThanksFromUserID = {0}", UserID);
        }
        else if (this.CurrentMode == ThanksListMode.ToUser)
        {
            foreach (DataRowView dr in dvThanksInfo)
            {
                //Extract the number of thanks for each message.
                //DataView dvThanksInfo = dvThanksInfo;
                dvThanksInfo.RowFilter = String.Format(
                "MessageID = {0} AND ThanksToUserID={1}", Convert.ToInt32(dr["MessageID"]), UserID);
                dr["MessageThanksNumber"] = dvThanksInfo.Count;
            }
            
            // Just select the rows in which the specified user is thanked.
            dvThanksInfo.RowFilter = String.Format("ThanksToUserID = {0}", UserID);
            
            //Remove duplicates.
            dvThanksInfo = DistinctMessageID();
        }

        // let's page the results
        pds.DataSource = dvThanksInfo;
        this.PagerTop.Count = dvThanksInfo.Count;

        // TODO : page size definable?
        this.PagerTop.PageSize = 15;
        pds.PageSize = this.PagerTop.PageSize;
        pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;

        // set datasource of repeater
        this.ThanksRes.DataSource = pds;

        // data bind controls
        this.DataBind();
    }
    #endregion
    
    /* Methods */
    #region Methods
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
        this.BindData();
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange(object sender, EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// Handles the ItemCreated event of the ThanksRes control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> 
    /// instance containing the event data.
    /// </param>
    protected void ThanksRes_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        // In what mode should this control work?
        // 1: Just display the buddy list
        // 2: display the buddy list and ("Remove Buddy") buttons.
        // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
        //    "deny all","approve and add", "approve and add all") buttons.
        // 4: show the pending requests posted from the current user.
        switch (CurrentMode)
        {
            case ThanksListMode.FromUser:
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                   HtmlTableCell thanksNumberCell = (HtmlTableCell)e.Item.FindControl("ThanksNumberCell");
                    thanksNumberCell.Visible = false;
                }
                break;
            case ThanksListMode.ToUser:
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HtmlTableCell nameCell = (HtmlTableCell)e.Item.FindControl("NameCell");
                    nameCell.Visible = false;
                }
                break;
        }
    }

    /// <summary>
    /// removes rows with duplicate MessageIDs. 
    /// </summary>
    private DataView DistinctMessageID()
    {
        DataView dvThanksInfo = ThanksInfo.DefaultView;
        dvThanksInfo.Sort = "MessageID";
        DataRowView currentRow;
        int prevMessageID = (int)(dvThanksInfo[0]["MessageID"]);
        int counter = 1;
        
        //Iterate through the rows. Delete multiple rows.
        while (counter < dvThanksInfo.Count)
        {
            currentRow = dvThanksInfo[counter];
            if ((int)currentRow["MessageID"] == prevMessageID)
                dvThanksInfo.Delete(counter);
            else
            {
                prevMessageID = (int)currentRow["MessageID"];
                counter += 1;
            }
        }
        return dvThanksInfo;
    }
    #endregion
  }
}