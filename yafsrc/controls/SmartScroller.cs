using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for SmartScroller.
	/// </summary>
	public class SmartScroller : System.Web.UI.Control
	{
		private HtmlForm m_theForm = new HtmlForm();		
		
		public SmartScroller()
		{
		}
		
		private HtmlForm GetServerForm(ControlCollection parent)
		{
			HtmlForm tmpHtmlForm;
            
			foreach (Control child in parent)
			{                                
				Type t = child.GetType();
				if (t == typeof(System.Web.UI.HtmlControls.HtmlForm))
					return (HtmlForm)child;
                
				if (child.HasControls())    
				{
					tmpHtmlForm=GetServerForm(child.Controls);
					if(tmpHtmlForm.ClientID !=null)
						return tmpHtmlForm;
				}
			}
        
			return new HtmlForm();
		}

		protected override void OnInit(EventArgs e)
		{
			m_theForm = GetServerForm(Page.Controls);
										
			HtmlInputHidden hidScrollLeft = new HtmlInputHidden();
			hidScrollLeft.ID = "scrollLeft";
	
			HtmlInputHidden hidScrollTop = new HtmlInputHidden();
			hidScrollTop.ID = "scrollTop";
	
			this.Controls.Add(hidScrollLeft);
			this.Controls.Add(hidScrollTop);						
	
			string scriptString = @"
<!-- yaf.controls.SmartScroller ASP.NET Generated Code -->
<script language = ""javascript"">
<!--
  function yaf_SmartScroller_GetCoords()
  {
    var scrollX, scrollY;
    if (document.all)
    {
      if (!document.documentElement.scrollLeft)
        scrollX = document.body.scrollLeft;
      else
        scrollX = document.documentElement.scrollLeft;

      if (!document.documentElement.scrollTop)
        scrollY = document.body.scrollTop;
      else
        scrollY = document.documentElement.scrollTop;
    }
    else
    {
      scrollX = window.pageXOffset;
      scrollY = window.pageYOffset;
    }
    document.forms[""" + m_theForm.ClientID + @"""]." + hidScrollLeft.ClientID + @".value = scrollX;
    document.forms[""" + m_theForm.ClientID + @"""]." + hidScrollTop.ClientID + @".value = scrollY;
  }


  function yaf_SmartScroller_Scroll()
  {
    var x = document.forms[""" + m_theForm.ClientID + @"""]." + hidScrollLeft.ClientID + @".value;
    var y = document.forms[""" + m_theForm.ClientID + @"""]." + hidScrollTop.ClientID + @".value;
    window.scrollTo(x, y);
  }

  
  window.onload = yaf_SmartScroller_Scroll;
  window.onscroll = yaf_SmartScroller_GetCoords;
  window.onclick = yaf_SmartScroller_GetCoords;
  window.onkeypress = yaf_SmartScroller_GetCoords;
// -->
</script>
<!-- End yaf.controls.SmartScroller ASP.NET Generated Code -->";

	
			Page.RegisterStartupScript("SmartScroller", scriptString);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			Page.VerifyRenderingInServerForm(this);
			base.Render(writer);
		}
	}
}