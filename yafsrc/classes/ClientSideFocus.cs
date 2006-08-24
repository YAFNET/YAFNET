/**
 * Author: Damien McGivern
 * email: dotnet@mcgiv.com
 * 
 * Free to use provided this header is left within the source.
 * Please inform me if you find any bugs or improve the code.
 * 
 * Helper class that generates and registers a client side 
 * script which sets focus to a control during the loading of the page.
 * 
 * 
 * ------------------------------------------------------------------------
 * Version 1.3	: Wednesday 21 Jan 2003
 * Changes:
 *		Fixed null object JavaScript bug 
 * 
 * 
 * ------------------------------------------------------------------------
 * Version 1.2	: Tuesday 20 Jan 2003
 * Changes:
 *		Moved the call to setFocus from the document.body.onload event to 
 *		the document.body.onfocus event. This stops the focus being taken from 
 *		another browser window that has the focus.
 * 
 *		Discovered that the code only works with Smart Navigation on my 
 *		local machine & not my remote host. Don't know if this is a security 
 *		issue or a loading time isssue.
 * 
 * 
 * -------------------------------------------------------------------------
 * Version 1.1	: Wednesday 14 Jan 2003
 * Changes:
 *		Remove script from onload event
 *			Allows it to work with Mozilla browsers too
 * 
 *		Used getElementById([control name]) instead of using [form name].[control name]
 *			Removes need to search for form's ID - with APS.Net only 1 form is used 
 *			normally removing the need to supply the form ID
 * 
 *		Overrides restoreFocus Smart Navigation function
 *			Now works when smart navigation is enabled
 * 
 * 
 * ----------------------------------------------------------------------------
 * Version 1.0 : Tuesday 13 Jan 2003
 *		Will function if Smart Navigation is enabled on the WebForm.
 *		Only tested on IE 6.1
 * 
 *			
 * 
 * */
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf
{
	/// <summary>
	/// Helper class that sets that generates and registers a client side script that
	/// gives focus to a control during the loading of the page.
	/// </summary>
	public class ClientSideFocus
	{
		

		/// <summary>
		/// Sets the focus to the control with the given name
		/// </summary>
		/// <param name="page">Page Control which hosts the control</param>
		/// <param name="formID">The ID of the html form that holds the control</param>
		/// <param name="controlID">The ID of the control that focus will be set to</param>
		public static void setFocus(Page page, string controlID)
		{

			string javascript = string.Format( @"
<script language=""javascript"">

	var called = false;

	if(document.body.onfocus)
		orig_document_body_onfocus = document.body.onfocus;

	document.body.onfocus = document_body_onfocus;


	function setFocus()
	{{
		var control = document.getElementById(""{0}"");
		if(control)
			control.focus();
	}}


	function orig_document_body_onfocus(){{}}
	
	function document_body_onfocus ()
	{{
		if( !called )
		{{
			called = true;
			setFocus();
		}}
			
		orig_document_body_onfocus();
	}}



	if( window.__smartNav && window.__smartNav.restoreFocus)
	{{
		orig_window__smartNav_restoreFocus = window.__smartNav.restoreFocus;
		window.__smartNav.restoreFocus = window__smartNav_restoreFocus;
	}}


	function orig_window__smartNav_restoreFocus(){{}}

	function window__smartNav_restoreFocus()
	{{
		setFocus();
		orig_window__smartNav_restoreFocus
	}}


</script>", controlID);



			page.ClientScript.RegisterStartupScript(page.GetType(),"focus", javascript);

		}// end setFocus






		/// <summary>
		/// Sets the focus to a System.Web.UI.WebControls 
		/// </summary>
		/// <param name="webControl">The control that focus will be set to</param>
		public static void setFocus(WebControl control)
		{
			string tRealID = control.NamingContainer.ClientID;
			if (tRealID.Length > 0) tRealID += "_";
			tRealID += control.ID;

			setFocus(control.Page, tRealID);
			
		}// end setFocus

		


		/// <summary>
		/// Sets the focus to a System.Web.UI.HtmlControls 
		/// </summary>
		/// <param name="htmlControl">The control that focus will be set to</param>
		public static void setFocus(HtmlControl control)
		{
			setFocus(control.Page, control.ID);

		}// end setFocus


	


	}// end class

}// end namespace