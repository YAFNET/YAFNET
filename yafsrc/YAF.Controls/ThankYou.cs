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

using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using YAF.Classes.Core;
using YAF.Classes.Data;

namespace YAF.Controls
{
    /// <summary>
    /// Class for Thank you button
    /// </summary>
    public class ThankYou
    {
        public string Text;
        public string Title;
        public string messageID;
        public string ThanksInfo;
        public string Thanks;
        public ThankYou()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        // <summary> This method is called asynchronously when the user clicks 
        //           "Thank" button. </summary>

        [AjaxPro.AjaxMethod]
        public ThankYou AddThanks(object msgID)
        {
            //Why does this throw the exception "Database is not initialized."?
            YafServices.InitializeDb.Run();
            this.messageID = msgID.ToString();
            MembershipUser membershipUser = Membership.GetUser();
            int FromUserID = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);
            string Username = DB.message_AddThanks(FromUserID, this.messageID);
            if (Username == "")
                return null;
            this.ThanksInfo = ThanksNumber(Username);
            this.Thanks = GetThanks();
            this.Text = YafContext.Current.Localization.GetText("BUTTON", "BUTTON_THANKSDELETE");
            this.Title = YafContext.Current.Localization.GetText("BUTTON", "BUTTON_THANKSDELETE_TT");
            ThankYou objThankYou = new ThankYou();
            objThankYou.messageID = messageID;
            objThankYou.ThanksInfo = ThanksInfo;
            objThankYou.Thanks = Thanks;
            objThankYou.Text = Text;
            objThankYou.Title = Title;
            return objThankYou;
        }

        // <summary> This method is called asynchronously when the user clicks 
        //           on "Remove Thank Note" button. </summary>
        [AjaxPro.AjaxMethod]
        public ThankYou RemoveThanks(object msgID)
        {
            YafServices.InitializeDb.Run();
            this.messageID = msgID.ToString();
            MembershipUser membershipUser = Membership.GetUser();
            int FromUserID = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);
            string Username = DB.message_RemoveThanks(FromUserID, this.messageID);
            this.ThanksInfo = ThanksNumber(Username);
            this.Thanks = GetThanks();
            this.Text = YafContext.Current.Localization.GetText("BUTTON", "BUTTON_THANKS");
            this.Title = YafContext.Current.Localization.GetText("BUTTON", "BUTTON_THANKS_TT");
            ThankYou objThankYou = new ThankYou();
            objThankYou.messageID = messageID;
            objThankYou.ThanksInfo = ThanksInfo;
            objThankYou.Thanks = Thanks;
            objThankYou.Text = Text;
            objThankYou.Title = Title;
            return objThankYou;
        }

        // <summary> This method returns a string containing the HTML code for
        //           showing the the post footer. the HTML content is the name of users
        //           who thanked the post and the date they thanked. </summary>
        protected string GetThanks()
        {
            string Filler = "";
            using (DataTable dt = DB.message_GetThanks(Convert.ToInt32(messageID)))
                foreach (DataRow dr in dt.Rows)
                {
                    Filler += "<a id=\"" + dr["UserID"] + "\"href=\"" + String.Format("{0}{1}?{2}", YAF.Classes.UrlBuilder.BaseUrl, "/default.aspx", String.Format("g={0}&u={1}", YAF.Classes.ForumPages.profile, dr["UserID"])) + "\"><u>" + dr["Name"] + "</u></a>  " + String.Format(YafContext.Current.Localization.GetText("DEFAULT", "OnDate"), YafServices.DateTime.FormatDateShort(dr["ThanksDate"]).ToString()) + ",&nbsp;&nbsp;";
                }
            if (Filler != "")
                Filler = Filler.Remove(Filler.LastIndexOf(","));
            return Filler;
        }

        // <summary> Same as the above method. To use in other classes. (this
        //          method is used in Controls/DisplayPost.ascx.cs</summary>
        public static string GetThanks(object msgID)
        {
            string Filler = "";
            using (DataTable dt = DB.message_GetThanks(Convert.ToInt32(msgID.ToString())))
                foreach (DataRow dr in dt.Rows)
                    Filler += "<a id=\"" + dr["UserID"] + "\"href=\"" + String.Format("{0}{1}?{2}", YAF.Classes.UrlBuilder.BaseUrl, "/default.aspx", String.Format("g={0}&u={1}", YAF.Classes.ForumPages.profile, dr["UserID"])) + "\"><u>" + dr["Name"] + "</u></a>  " + String.Format(YafContext.Current.Localization.GetText("DEFAULT", "ONDATE"), YafServices.DateTime.FormatDateShort(dr["ThanksDate"]).ToString()) + ",&nbsp;&nbsp;";
            if (Filler != "")
                Filler = Filler.Remove(Filler.LastIndexOf(","));
            return Filler;
        }

        // <summary> This method returns a string which shows how many times users have
        // thanked the message with the provided messageID. Returns an empty string </summary>
        protected string ThanksNumber(string Username)
        {
            int ThanksNumber = DB.message_ThanksNumber(messageID);
            if (ThanksNumber == 0) return "";
            else if (ThanksNumber == 1) return String.Format(YafContext.Current.Localization.GetText("POSTS", "THANKSINFOSINGLE"), Username);
            else return String.Format(YafContext.Current.Localization.GetText("POSTS", "THANKSINFO"), ThanksNumber, Username);
        }
    }
}

