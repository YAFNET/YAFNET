using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Intelligencia.UrlRewriter.Utilities;

namespace Intelligencia.UrlRewriter
{
	/// <summary>
	/// Replacement for &lt;asp:form&gt; to handle rewritten form postback.
	/// </summary>
	/// <remarks>
	/// <p>This form should be used for pages that use the url rewriter and have
	/// forms that are posted back.  If you use the normal ASP.NET <see cref="System.Web.UI.HtmlControls.HtmlForm">HtmlForm</see>,
	/// then the postback will not be able to correctly resolve the postback data to the form data.
	/// </p>
	/// <p>This form is a direct replacement for the &lt;asp:form&gt; tag.
	/// </p>
	/// <p>The following code demonstrates the usage of this control.</p>
	/// <code>
	/// &lt;%@ Page language="c#" Codebehind="MyPage.aspx.cs" AutoEventWireup="false" Inherits="MyPage" %&gt;
	/// &lt;%@ Register TagPrefix="url" Namespace="Intelligencia.UrlRewriter" Assembly="Intelligencia.UrlRewriter" %&gt;
	/// &lt;html&gt;
	/// ...
	/// &lt;body&gt;
	/// &lt;url:form id="MyForm" runat="server"&gt;
	/// ...
	/// &lt;/url:form&gt;
	/// &lt;/body&gt;
	/// &lt;/html&gt;
	/// </code>
	/// </remarks>
    [ComVisible(false)]
    [ToolboxData("<{0}:Form runat=server></{0}:RewrittenForm>")]
	public class Form : HtmlForm
	{
		/// <summary>
		/// Renders children of the form control.
		/// </summary>
		/// <param name="writer">The output writer.</param>
		/// <exclude />
		protected override void RenderChildren(HtmlTextWriter writer)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			base.RenderChildren(writer);
			writer.RenderEndTag();
		}

		/// <summary>
		/// Renders attributes.
		/// </summary>
		/// <param name="writer">The output writer.</param>
		/// <exclude />
		protected override void RenderAttributes(HtmlTextWriter writer)
		{
			writer.WriteAttribute(Constants.AttrName, GetName());
			Attributes.Remove(Constants.AttrName);

			writer.WriteAttribute(Constants.AttrMethod, GetMethod());
			Attributes.Remove(Constants.AttrMethod);

			writer.WriteAttribute(Constants.AttrAction, GetAction(), true);
			Attributes.Remove(Constants.AttrAction);

			Attributes.Render(writer);

			if (ID != null)
			{
				writer.WriteAttribute(Constants.AttrID, GetID());
			}
		}

		private string GetID()
		{
			return ClientID;
		}

		private string GetName()
		{
			return Name;
		}

		private string GetMethod()
		{
			return Method;
		}

		private string GetAction()
		{
            return RewriterHttpModule.RawUrl;
		}
	}
}
