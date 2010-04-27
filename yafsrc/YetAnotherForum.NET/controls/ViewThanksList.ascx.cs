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
    private DataView _thanksInfo = null;

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
    public DataView ThanksInfo
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

      // now depending on mode filter the table
      if (this.CurrentMode == ThanksListMode.FromUser)
      {
        ThanksInfo.RowFilter = String.Format("ThanksFromUserID = {0}", UserID);
      }
      else if (this.CurrentMode == ThanksListMode.ToUser)
      {
        ThanksInfo.RowFilter = String.Format("ThanksToUserID = {0}", UserID);

      }
      if (ThanksInfo.Count == 0)
      {
          NoResults.Visible = true;
          return;
      }
        // let's page the results
        pds.DataSource = ThanksInfo;
        this.PagerTop.Count = ThanksInfo.Count;

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
    #endregion
  }
}