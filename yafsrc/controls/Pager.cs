using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for Pager.
	/// </summary>
	public class Pager : LinkButton, System.Web.UI.IPostBackEventHandler
	{
		public Pager()
		{
			this.Load += new EventHandler(Pager_Load);
		}

		private void Pager_Load(object sender,EventArgs e)
		{
		}

		protected override void Render(HtmlTextWriter output)
		{
			if(PageCount<2) return;

			output.WriteLine("<span class='navlinks'>");
			output.WriteLine("{0:N0} Pages:",PageCount);
			for(int i=0;i<PageCount;i++)
			{
				if(i==CurrentPageIndex)
					output.WriteLine("[{0}]",i+1);
				else
					output.WriteLine("<a href=\"{0}\">{1}</a>",Page.GetPostBackClientHyperlink(this,i.ToString()),i+1);
			}
			output.WriteLine("</span>");
		}

		public int Count
		{
			get
			{
				if(ViewState["Count"]!=null)
					return (int)ViewState["Count"];
				else
					return 0;
			}
			set
			{
				ViewState["Count"] = value;
			}
		}

		public int CurrentPageIndex
		{
			get
			{
				if(ViewState["CurrentPageIndex"]!=null)
					return (int)ViewState["CurrentPageIndex"];
				else
					return 0;
			}
			set
			{
				ViewState["CurrentPageIndex"] = value;
			}
		}

		public int PageSize
		{
			get
			{
				if(ViewState["PageSize"]!=null)
					return (int)ViewState["PageSize"];
				else
					return 20;
			}
			set
			{
				ViewState["PageSize"] = value;
			}
		}

		public int PageCount
		{
			get
			{
				return (int)Math.Ceiling((double)Count/PageSize);
			}
		}

		#region IPostBackEventHandler
		public event EventHandler PageChange;

		public void RaisePostBackEvent(string eventArgument)
		{
			if(PageChange!=null)
			{
				CurrentPageIndex = int.Parse(eventArgument);
				PageChange(this,new EventArgs());
			}
		}
		#endregion
	}
}
