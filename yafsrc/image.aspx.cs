using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Summary description for image.
	/// </summary>
	public class image : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			using(SqlCommand cmd = new SqlCommand("yaf_user_avatarimage",DataManager.GetConnection())) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",Request.QueryString["u"]);
				using(SqlDataAdapter da = new SqlDataAdapter(cmd)) 
				{
					using(SqlCommandBuilder cb = new SqlCommandBuilder(da)) 
					{
						using(DataSet ds = new DataSet()) 
						{
							da.Fill(ds);

							byte[] data = (byte[])ds.Tables[0].Rows[0]["AvatarImage"];

							Response.Clear();
							Response.ContentType = "image/jpg";
							Response.OutputStream.Write(data,0,data.Length);
							Response.End();
						}
					}
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
