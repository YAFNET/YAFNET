using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Controls.Statistics
{
    [ToolboxData("<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>")]
    public class MostActiveUsers : BaseControl
    {
        private int _displayNumber = 10;
        /// <summary>
        /// The default constructor for MostActiveUsers.
        /// </summary>
        public MostActiveUsers()
        {

        }

        public int DisplayNumber
        {
            get
            {
                return _displayNumber;
            }
            set
            {
                _displayNumber = value;
            }
        }

        /// <summary>
        /// Renders the MostActiveUsers class.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            string act_rank = "";

            act_rank += "<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
            act_rank += "<tr><td class=\"header1\">Most Active Users</td></tr>";
            System.Data.DataTable rank = YAF.Classes.Data.DB.user_activity_rank(DisplayNumber);
            int i = 1;

            act_rank += "<tr><td class=post><table cellspacing=0 cellpadding=0 align=center>";

            foreach (System.Data.DataRow r in rank.Rows)
            {
                // string img = "<img src='/yetanotherforum.net/themes/standard/user_rank1.gif'/>";
                i++;
                act_rank += "<tr class=\"post\">";

                // Immagine
                // act_rank += string.Format("<td align=\"center\">{0}</td>", img);

                // Nome autore
                act_rank += string.Format("<td width=\"75%\">&nbsp;<a href='{1}'>{0}</a></td>", r["Name"], Forum.GetLink(ForumPages.profile, "u={0}", r["ID"]));

                // Numero post
                act_rank += string.Format("<td align=\"center\">{0}</td></tr>", r["NumOfPosts"]);

                act_rank += "</tr>";
            }

            act_rank += "</table></td></tr>";

            act_rank += "</table>";
            writer.Write(act_rank);
        }
    }
}
