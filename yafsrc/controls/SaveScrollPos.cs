using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumJump.
	/// </summary>
	public class SaveScrollPos : BaseControl, System.Web.UI.IPostBackDataHandler
	{
		private int ScrollPos 
		{
			get 
			{
				return ViewState["ScrollPos"]!=null ? (int)ViewState["ScrollPos"] : 0;
			}
			set 
			{
				ViewState["ScrollPos"] = value;
			}
		}

		#region IPostBackDataHandler
		public virtual bool LoadPostData(string postDataKey,System.Collections.Specialized.NameValueCollection postCollection) 
		{
			ScrollPos = int.Parse(postCollection[postDataKey]);
			return false;
		}

		public virtual void RaisePostDataChangedEvent() 
		{
		}
		#endregion

		protected override void OnPreRender(EventArgs e)
		{
			if(!Page.IsStartupScriptRegistered("scrollToPos"))
				Page.RegisterStartupScript("scrollToPos",String.Format("<script language='javascript'>scrollTo(0,{0});</script>",ScrollPos));

			if(!Page.IsClientScriptBlockRegistered("doScroll"))
				Page.RegisterClientScriptBlock("doScroll",
					"<script language='javascript'>\n"+
					"onscroll=function(){\n"+
					"	document.getElementById('" + this.UniqueID + "').value=document.body.scrollTop;\n"+
					"}\n"+
					"</script>");
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteLine(String.Format("<input type='hidden' name='{0}' id='{0}' value='{1}'/>",this.UniqueID,ScrollPos));
		}
	}
}
