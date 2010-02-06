/* Yet Another Forum.NET
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
 * Written by vzrus (c) 2009 for Yet Another Forum.NET  */
using System;
using System.Data;
using System.Web.UI;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
  /// <summary>
  /// Shows a Reporters for reported posts
  /// </summary>
  public class ReportedPosts : BaseControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ReportedPosts"/> class.
    /// </summary>
    public ReportedPosts()
      : base()
    {
    }

    /// <summary>
    /// Gets or sets MessageID.
    /// </summary>
    public int MessageID
    {
      get
      {
        if (ViewState["MessageID"] != null)
        {
          return Convert.ToInt32( ViewState["MessageID"] );
        }

        return 0;
      }

      set
      {
        ViewState["MessageID"] = value;
      }
    }
    /// <summary>
    /// Gets or sets ResolvedBy. It returns UserID as string value
    /// </summary>
    public string ResolvedBy
    {
        get
        {      
                return ViewState["ResolvedBy"].ToString();           
        }

        set
        {
            ViewState["ResolvedBy"] = value;
        }
    }
    /// <summary>
    /// Gets or sets Resolved.
    /// </summary>
    public string Resolved
    {
        get
        {
           
                return ViewState["Resolved"].ToString();           
        }

        set
        {
            ViewState["Resolved"] = value;
        }
    }
    /// <summary>
    /// Gets or sets ResolvedDate.
    /// </summary>
    public string ResolvedDate
    {
        get
        {        
                return ViewState["ResolvedDate"].ToString() ;           
        }

        set
        {
            ViewState["ResolvedDate"] = value;
        }
    }



    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      // TODO: Needs better commentting.
      writer.WriteLine(String.Format(@"<div id=""{0}"" class=""yafReportedPosts"">", ClientID));

      DataTable reportersList = DB.message_listreporters(MessageID);
      if (reportersList.Rows.Count > 0)
      {
        int i = 0;
        writer.BeginRender();

        foreach (DataRow reporter in reportersList.Rows)
        {
          string howMany = null;
          if (Convert.ToInt32(reporter["ReportedNumber"]) > 1)
          {
            howMany = "(" + reporter["ReportedNumber"].ToString() + ")";
          }
          
          writer.WriteLine(@"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafreportedtable{0}"">", ClientID);
         // If the message was previously resolved we have not null string
         // and can add an info about last user who resolved the message
            if (  !string.IsNullOrEmpty( ResolvedDate ) )
          {
              writer.Write(@"<tr><td class=""header2"">");
              writer.Write(@"<span class=""postheader"">{0}</span><a class=""YafReported_Link"" href=""{1}""> {2}</a><span class=""YafReported_ResolvedBy""> : {3}</span>",
             PageContext.Localization.GetText( "RESOLVEDBY" ),
             YafBuildLink.GetLink( ForumPages.profile, "u={0}", Convert.ToInt32( ResolvedBy ) ),
             DB.user_list( PageContext.PageBoardID, Convert.ToInt32( ResolvedBy ), true ).Rows[0]["Name"],             
             YafServices.DateTime.FormatDateTimeTopic( ResolvedDate ));
             writer.WriteLine(@"</td></tr>");
          }
          writer.Write(@"<tr><td class=""post"">");
          writer.Write(@"<tr><td class=""header2"">");
          writer.Write(@"<span class=""YafReported_Complainer"">{5}</span><a class=""YafReported_Link"" href=""{3}""> {2}{4} </a>", 
            i, 
            Convert.ToInt32(reporter["UserID"]),
            string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(reporter["UserID"]))) ? reporter["UserName"].ToString() : UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(reporter["UserID"])), 
            YafBuildLink.GetLink(ForumPages.profile, "u={0}", Convert.ToInt32(reporter["UserID"])), 
            howMany, 
            PageContext.Localization.GetText("REPORTEDBY"));
          writer.WriteLine(@"</td></tr>");
          string[] reportString = reporter["ReportText"].ToString().Trim().Split('|');

          for (int istr = 0; istr < reportString.Length; istr++)
          {
            string[] textString = reportString[istr].Split("??".ToCharArray());
            writer.Write(@"<tr><td class=""post"">");
            writer.Write(@"<span class=""YafReported_DateTime"">{0}:</span>", YafServices.DateTime.FormatDateTimeTopic(textString[0]));
              
              // Apply style if a post was previously resolved
              string resStyle = "post_res";
              try 
              {
               if ( !( string.IsNullOrEmpty( textString[0].ToString() ) && string.IsNullOrEmpty( ResolvedDate ) ) ) 
               {
                  if ( Convert.ToDateTime( textString[0] ) < Convert.ToDateTime( ResolvedDate ) )
                  { resStyle ="post" ; }
               }
              }
              catch ( Exception )
	          {
                  resStyle = "post_res";		
	          }

              if (textString.Length > 2)
            {              
              writer.Write( @"<tr><td class=""{0}"">", resStyle );
              writer.Write(textString[2]);
              writer.WriteLine(@"</td></tr>");
            }
            else
            {
              writer.WriteLine(@"<tr><td class=""post"">");
              writer.Write(reportString[istr]);
              writer.WriteLine(@"</td></tr>");
            }
          }

          writer.WriteLine(@"<tr><td class=""postfooter"">");
          writer.Write(
            @"<a class=""YafReported_Link"" href=""{3}"">{4} {2}</a>", 
            i, 
            Convert.ToInt32(reporter["UserID"]), 
            reporter["UserName"].ToString(), 
            YafBuildLink.GetLink(ForumPages.pmessage, "u={0}&r={1}", Convert.ToInt32(reporter["UserID"]), MessageID), 
            PageContext.Localization.GetText("REPLYTO"));
          writer.WriteLine(@"</td></tr>");

          // TODO: Remove hard-coded formatting.
          if (i < reportersList.Rows.Count - 1)
          {
            writer.Write("<br></br>");
          }
          else
          {
            writer.WriteLine(@"</td></tr>");
          }

          i++;
        }

        // render controls...
        writer.Write(@"</table>");
        base.Render(writer);

        writer.WriteLine("</div>");
        writer.EndRender();
      }
    }
  }
}