/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Data;
using System.ComponentModel;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Controls
{
  public partial class PMList : YAF.Classes.Base.BaseUserControl
  {
    protected void Page_Load( object sender, EventArgs e )
    {
      if ( ViewState ["SortField"] == null )
        SetSort( "Created", false );

			if ( !IsPostBack )
			{
				// setup pager...
				MessagesView.AllowPaging = true;
				MessagesView.PagerSettings.Visible = false;
				MessagesView.AllowSorting = true;

				PagerTop.PageSize = 10;
				MessagesView.PageSize = 10;				
			}

			BindData();
    }

    /// <summary>
    /// Gets or sets the current view for the user's private messages.
    /// </summary>
    [Browsable( true ),
    Category( "Behavior" ),
    Description( "Gets or sets the current view for the user's private messages." ),
    TypeConverter( typeof( PMViewTypeConverter ) )]
    public PMView View
    {
      get
      {
        if ( ViewState ["View"] != null )
          return ( PMView ) ViewState ["View"];
        else
          return PMView.Inbox;
      }
      set { ViewState ["View"] = value; }
    }

    protected string GetTitle()
    {
      if ( View == PMView.Outbox )
        return GetLocalizedText( "SENTITEMS" );
      else if ( View == PMView.Inbox )
        return GetLocalizedText( "INBOX" );
      else
        return GetLocalizedText( "ARCHIVE" );
    }

    protected string GetLocalizedText( string text )
    {
      return HtmlEncode( PageContext.Localization.GetText( text ) );
    }

    protected string GetArchiveSelectedText()
    {
      return GetLocalizedText( "ARCHIVESELECTED" );
    }
    protected string GetDeleteSelectedText()
    {
      return GetLocalizedText( "DELETESELECTED" );
    }

    protected string GetMessageUserHeader()
    {
      return GetLocalizedText( View == PMView.Outbox ? "to" : "from" );
    }

    protected string GetMessageLink( object messageId )
    {
      return YafBuildLink.GetLink( ForumPages.cp_message, "pm={0}&v={1}", messageId,
                                   View.ToQueryStringParam() );
    }

    protected string FormatBody( object o )
    {
      DataRowView row = ( DataRowView ) o;
      return ( string ) row ["Body"];
    }

    private void BindData()
    {
      object toUserID = null;
      object fromUserID = null;
      if ( View == PMView.Outbox )
        fromUserID = PageContext.PageUserID;
      else
        toUserID = PageContext.PageUserID;
      using ( DataView dv = DB.pmessage_list( toUserID, fromUserID, null ).DefaultView )
      {
        if ( View == PMView.Outbox )
          dv.RowFilter = "IsInOutbox = True";
        else if ( View == PMView.Archive )
          dv.RowFilter = "IsArchived = True";
        else
          dv.RowFilter = "IsArchived = False";

        dv.Sort = String.Format( "{0} {1}", ViewState ["SortField"], ( bool ) ViewState ["SortAsc"] ? "asc" : "desc" );

				PagerTop.Count = dv.Count;

				MessagesView.PageIndex = PagerTop.CurrentPageIndex;
        MessagesView.DataSource = dv;				
        MessagesView.DataBind();
      }
    }

    protected void ArchiveSelected_Click( object source, EventArgs e )
    {
      if ( this.View != PMView.Inbox )
        return;

      long archivedCount = 0;
      foreach ( GridViewRow item in MessagesView.Rows )
      {
        if ( ( ( CheckBox ) item.FindControl( "ItemCheck" ) ).Checked )
        {
          DB.pmessage_archive( MessagesView.DataKeys [item.RowIndex].Value );
          archivedCount++;
        }
      }

      BindData();

      if ( archivedCount == 1 )
        PageContext.AddLoadMessage( PageContext.Localization.GetText( "MSG_ARCHIVED" ) );
      else
        PageContext.AddLoadMessage( String.Format( PageContext.Localization.GetText( "MSG_ARCHIVED+" ), archivedCount ) );
    }

    protected void DeleteSelected_Click( object source, EventArgs e )
    {
      long nItemCount = 0;
      foreach ( GridViewRow item in MessagesView.Rows )
      {
        if ( ( ( CheckBox ) item.FindControl( "ItemCheck" ) ).Checked )
        {
          if ( View == PMView.Outbox )
            DB.pmessage_delete( MessagesView.DataKeys [item.RowIndex].Value, true );
          else
            DB.pmessage_delete( MessagesView.DataKeys [item.RowIndex].Value );
          nItemCount++;
        }
      }

      BindData();
      if ( nItemCount == 1 )
        PageContext.AddLoadMessage( PageContext.Localization.GetText( "msgdeleted1" ) );
      else
        PageContext.AddLoadMessage(
            String.Format( PageContext.Localization.GetText( "msgdeleted2" ), nItemCount ) );
    }

    protected string GetImage( object o )
    {
      if ( ( bool ) ( ( DataRowView ) o ) ["IsRead"] )
        return PageContext.Theme.GetItem( "ICONS", "TOPIC" );
      else
        return PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW" );
    }

    private void SetSort( string field, bool asc )
    {
      if ( ViewState ["SortField"] != null && ( string ) ViewState ["SortField"] == field )
      {
        ViewState ["SortAsc"] = !( bool ) ViewState ["SortAsc"];
      }
      else
      {
        ViewState ["SortField"] = field;
        ViewState ["SortAsc"] = asc;
      }
    }

    protected void SubjectLink_Click( object sender, EventArgs e )
    {
      SetSort( "Subject", true );
      BindData();
    }

    protected void FromLink_Click( object sender, EventArgs e )
    {
      if ( View == PMView.Outbox )
        SetSort( "ToUser", true );
      else
        SetSort( "FromUser", true );
      BindData();
    }

    protected void DateLink_Click( object sender, EventArgs e )
    {
      SetSort( "Created", false );
      BindData();
    }

    protected void DeleteSelected_Load( object sender, EventArgs e )
    {
      ( ( Button ) sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", PageContext.Localization.GetText( "confirm_delete" ) );
    }

    protected void MessagesView_RowCreated( object sender, GridViewRowEventArgs e )
    {
      if ( e.Row.RowType == DataControlRowType.Header )
      {
        GridView oGridView = ( GridView ) sender;
        GridViewRow oGridViewRow = new GridViewRow( 0, 0, DataControlRowType.Header, DataControlRowState.Insert );
        TableCell oTableCell = new TableCell();

        // Add Header to top with column span of 5... no need for two tables.
        oTableCell.Text = GetTitle();
        oTableCell.CssClass = "header1";
        oTableCell.ColumnSpan = 5;
        oGridViewRow.Cells.Add( oTableCell );
        oGridView.Controls [0].Controls.AddAt( 0, oGridViewRow );        

        Image SortFrom = ( Image ) e.Row.FindControl( "SortFrom" );
        Image SortSubject = ( Image ) e.Row.FindControl( "SortSubject" );
        Image SortDate = ( Image ) e.Row.FindControl( "SortDate" );

        if ( View == PMView.Outbox )
          SortFrom.Visible = ( string ) ViewState ["SortField"] == "ToUser";
        else
          SortFrom.Visible = ( string ) ViewState ["SortField"] == "FromUser";
        SortFrom.ImageUrl =
            PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );
        SortSubject.Visible = ( string ) ViewState ["SortField"] == "Subject";
        SortSubject.ImageUrl =
            PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );
        SortDate.Visible = ( string ) ViewState ["SortField"] == "Created";
        SortDate.ImageUrl =
            PageContext.Theme.GetItem( "SORT", ( bool ) ViewState ["SortAsc"] ? "ASCENDING" : "DESCENDING" );
      }
    }

		protected void PagerTop_PageChange( object sender, EventArgs e )
		{
			// rebind
			BindData();
		}
}

  /// <summary>
  /// Represents possible views for the PMList control.
  /// </summary>
  public class PMView
  {
    private string name;

    private PMView( string name )
    {
      this.name = name;
    }

    /// <summary>
    /// View of the user's Inbux
    /// </summary>
    public static PMView Inbox = new PMView( "Inbox" );
    /// <summary>
    /// View of the user's Outbox
    /// </summary>
    public static PMView Outbox = new PMView( "Outbox" );
    /// <summary>
    /// View of the user's Archive
    /// </summary>
    public static PMView Archive = new PMView( "Archive" );

    public string ToQueryStringParam()
    {
      if ( this == Outbox )
        return "out";
      else if ( this == Inbox )
        return "in";
      else if ( this == Archive )
        return "arch";
      else
        return null;
    }
    public static PMView FromQueryString( string param )
    {
      if ( String.IsNullOrEmpty( param ) )
        return PMView.Inbox;

      switch ( param.ToLower() )
      {
        case "out":
          return PMView.Outbox;
        case "in":
          return PMView.Inbox;
        case "arch":
          return PMView.Archive;
        default:    // Inbox by default
          return PMView.Inbox;
      }
    }

    public static PMView FromName( string name )
    {
      switch ( name.ToLower() )
      {
        case "inbox":
          return Inbox;
        case "outbox":
          return Outbox;
        case "archive":
          return Archive;
        default:
          return null;
      }
    }

    public override string ToString()
    {
      return name;
    }
  }

  /// <summary>
  /// Type converter for converting from string to PMView.
  /// </summary>
  public class PMViewTypeConverter : TypeConverter
  {
    private string [] values = { "Inbox", "Outbox", "Archive" };

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <returns>
    /// true if this converter can perform the conversion; otherwise, false.
    /// </returns>
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
    ///<param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from. </param>
    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
    {
      if ( sourceType == typeof( string ) )
        return true;
      else
        return base.CanConvertFrom( context, sourceType );
    }

    ///<summary>
    ///Converts the given object to the type of this converter, using the specified context and culture information.
    ///</summary>
    ///<returns>
    ///An <see cref="T:System.Object"></see> that represents the converted value.
    ///</returns>
    ///<param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture. </param>
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
    ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
    ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
    public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
    {
      if ( value is string )
      {
        string viewString = ( ( string ) value );
        return PMView.FromName( viewString );
      }
      else
        return base.ConvertFrom( context, culture, value );
    }


    ///<summary>
    ///Returns whether this converter can convert the object to the specified type, using the specified context.
    ///</summary>
    ///
    ///<returns>
    ///true if this converter can perform the conversion; otherwise, false.
    ///</returns>
    ///
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
    ///<param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to. </param>
    public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
    {
      if ( destinationType == typeof( System.ComponentModel.Design.Serialization.InstanceDescriptor ) )
        return true;
      else
        return base.CanConvertTo( destinationType );
    }

    ///<summary>
    ///Converts the given value object to the specified type, using the specified context and culture information.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Object"></see> that represents the converted value.
    ///</returns>
    ///
    ///<param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed. </param>
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
    ///<param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to. </param>
    ///<param name="value">The <see cref="T:System.Object"></see> to convert. </param>
    ///<exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
    ///<exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
    public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value,
                                         Type destinationType )
    {
      if ( destinationType == typeof( System.ComponentModel.Design.Serialization.InstanceDescriptor ) )
      {
        System.Reflection.MemberInfo [] ms =
            typeof( PMView ).GetMember( "FromName",
                                      System.Reflection.BindingFlags.Static |
                                      System.Reflection.BindingFlags.Public );

        if ( ms.Length > 0 )
          return
              new System.ComponentModel.Design.Serialization.InstanceDescriptor( ms [0], new object [] { value.ToString() } );
      }

      return base.ConvertTo( context, culture, value, destinationType );
    }

    ///<summary>
    ///Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
    ///</summary>
    ///<returns>
    ///A <see cref="T:System.ComponentModel.TypeConverter.StandardValuesCollection"></see> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
    ///</returns>
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null. </param>
    public override StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
    {
      return new StandardValuesCollection( values );
    }

    ///<summary>
    ///Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
    ///</summary>
    ///<returns>
    ///true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"></see> should be called to find a common set of values the object supports; otherwise, false.
    ///</returns>
    ///<param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context. </param>
    public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
    {
      return true;
    }
  }
}