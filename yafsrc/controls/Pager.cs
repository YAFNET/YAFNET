using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for Pager.
	/// </summary>
	public class Pager : BaseControl, System.Web.UI.IPostBackEventHandler
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
			if(LinkedPager!=null)
			{
				Pager linkedPager = (Pager)Parent.FindControl(LinkedPager);
				if(linkedPager==null)
					throw new Exception(string.Format("Failed to link pager to '{0}'.",LinkedPager));
				linkedPager.Render(output);
				return;
			}
				
			if(PageCount<2) return;

			output.WriteLine("<span>");
			output.WriteLine("{0:N0} pages:",PageCount);
			int iStart = CurrentPageIndex - 6;
			int iEnd = CurrentPageIndex + 7;
			if(iStart<0) iStart = 0;
			if(iEnd>PageCount) iEnd = PageCount;

			if(iStart>0)
				output.WriteLine("<a href=\"{0}\">First</a> ...",Page.ClientScript.GetPostBackClientHyperlink(this,"0"));

			for(int i=iStart;i<iEnd;i++)
			{
				if(i==CurrentPageIndex)
					output.WriteLine("[{0}]",i+1);
				else
					output.WriteLine( "<a href=\"{0}\">{1}</a>", Page.ClientScript.GetPostBackClientHyperlink( this, i.ToString() ), i + 1 );
			}

			if(iEnd<PageCount)
				output.WriteLine( "... <a href=\"{0}\">Last</a>", Page.ClientScript.GetPostBackClientHyperlink( this, ( PageCount - 1 ).ToString() ) );

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

		public string LinkedPager
		{
			get
			{
				return (string)ViewState["LinkedPager"];
			}
			set
			{
				ViewState["LinkedPager"] = value;
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
