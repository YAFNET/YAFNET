using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace yaf.editor
{

	/// <summary>
	/// Summary description for ForumEditorBase.
	/// </summary>
	public class ForumEditor : Control
	{
		protected	string m_baseDir = string.Empty;

		public new string ResolveUrl(string relativeUrl)
		{
			if(m_baseDir!=string.Empty)
				return m_baseDir + relativeUrl;
			else
				return base.ResolveUrl(relativeUrl);
		}

		protected virtual string Replace(string txt,string match,string replacement) 
		{
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
			while (Regex.IsMatch(txt,match,options)) txt = Regex.Replace(txt,match,replacement,options);
			return txt;
		}

		#region Virtual Properties
		public virtual string Text
		{
			get	{	return string.Empty; }
			set	{	; }
		}

		public virtual string BaseDir
		{
			set
			{
				m_baseDir = value;
				if (!m_baseDir.EndsWith("/"))
					m_baseDir += "/";
			}
		}
		public virtual string StyleSheet
		{
			get { return string.Empty; }
			set { ;	}
		}
		public virtual bool UsesHTML
		{
			get	{ return false; }		
		}
		public virtual bool UsesBBCode
		{
			get { return false; }
		}
		#endregion
	}


	public class TextEditor : ForumEditor
	{
		protected TextBox	m_textCtl;

		protected override void OnInit(EventArgs e)
		{			
			Load += new EventHandler(Editor_Load);
			m_textCtl = new TextBox();
			m_textCtl.ID = "edit";
			m_textCtl.Attributes.Add("style","height:100%;width:100%;");
			m_textCtl.TextMode = TextBoxMode.MultiLine;
			Controls.Add(m_textCtl);
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (this.Visible)
			{
				Page.RegisterClientScriptBlock("yafeditorjs",string.Format("<script language='javascript' src='{0}'></script>",ResolveUrl("yafEditor/yafEditor.js")));

				Page.RegisterClientScriptBlock("insertsmiley",
					"<script language='javascript'>\n"+
					"function insertsmiley(code) {\n"+
					"	" + SafeID + ".InsertSmiley(code);\n"+
					"}\n"+
					"</script>\n");

				Page.RegisterClientScriptBlock("createyafeditor",
					"\n<script language='javascript'>\n"+
					"var "+SafeID+"=new yafEditor('"+SafeID+"');\n"+
					"function setStyle(style,option) {\n"+
          "	"+SafeID+".FormatText(style,option);\n"+
					"}\n"+"</script>");
			}
		}

		public override string Text
		{
			get
			{
				return m_textCtl.Text;
			}
			set
			{
				m_textCtl.Text = value;
			}
		}

		protected string SafeID
		{
			get	{	return m_textCtl.ClientID.Replace("$","_");	}
		}

		public override bool UsesHTML
		{
			get	{ return false; }		
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
	}


	public class BBCodeEditor : TextEditor
	{
		private void RenderButton(HtmlTextWriter writer,string id,string cmd,string title,string image)
		{
			//writer.WriteLine("		<td><img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"{1}.{3}\"></td><td>&nbsp;</td>",ResolveUrl(image),SafeID,title,cmd,id);
			writer.WriteLine("<img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"setStyle('{4}','')\">",ResolveUrl(image),SafeID,title,cmd,id);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table border='0' cellpadding='0' cellspacing='2' width='100%' height='300'>");
			writer.WriteLine("<tr><td valign='top'>");
			writer.WriteLine("	<table border='0' cellpadding='1' cellspacing='2'>");
			writer.WriteLine("	<tr><td valign='middle'>");

			RenderButton(writer,"bold","FormatText('bold','')","Bold","yafEditor/bold.gif");
			RenderButton(writer,"italic","FormatText('italic','')","Italic","yafEditor/italic.gif");
			RenderButton(writer,"underline","FormatText('underline','')","Underline","yafEditor/underline.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer,"quote","FormatText('quote','')","Quote","yafEditor/quote.gif");
			RenderButton(writer,"code","FormatText('code','')","Code","yafEditor/code.gif");
			RenderButton(writer,"img","FormatText('img','')","Image","yafEditor/image.gif");
			RenderButton(writer,"createlink","FormatText('createlink','')","Create Link","yafEditor/link.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer,"justifyleft","FormatText('justifyleft','')","Left","yafEditor/justifyleft.gif");
			RenderButton(writer,"justifycenter","FormatText('justifycenter','')","Center","yafEditor/justifycenter.gif");
			RenderButton(writer,"justifyright","FormatText('justifyright','')","Right","yafEditor/justifyright.gif");

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	<tr><td valign='middle'>");

			writer.WriteLine("Font color:");
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">",SafeID);
			writer.WriteLine("<option value=\"\">Default</option>");
			
			string[] Colors = {"Dark Red","Red","Orange","Brown","Yellow","Green","Olive","Cyan","Blue","Dark Blue","Indigo","Violet","White","Black"};
			foreach(string color in Colors)
			{
				string tValue = color.Replace(" ","").ToLower();
				writer.WriteLine(string.Format("<option style=\"color:{0}\" value=\"{0}\">{1}</option>",tValue,color));
			}

			writer.WriteLine("</select>");

			writer.WriteLine("Font size:");
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">",SafeID);
			writer.WriteLine("<option value=\"1\">1</option>");
			writer.WriteLine("<option value=\"2\">2</option>");
			writer.WriteLine("<option value=\"3\">3</option>");
			writer.WriteLine("<option value=\"4\">4</option>");
			writer.WriteLine("<option selected=\"selected\" value=\"5\">Default</option>");
			writer.WriteLine("<option value=\"6\">6</option>");
			writer.WriteLine("<option value=\"7\">7</option>");
			writer.WriteLine("<option value=\"8\">8</option>");
			writer.WriteLine("<option value=\"9\">9</option>");
			writer.WriteLine("</select>");

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	</table>");
			writer.WriteLine("</td></tr><tr><td height='99%'>");
			base.Render(writer);
			writer.WriteLine("</td></tr></table>");
		}

		protected override void OnInit(EventArgs e)
		{
			
			base.OnInit(e);
		}

		protected override void Editor_Load(object sender,EventArgs e)
		{
			if(this.Visible)
			{
				Page.RegisterClientScriptBlock("richeditstyles",
					"<style>\n"+
					".ButtonOut\n"+
					"{\n"+
					"	filter: alpha(opacity=70);\n"+
					"	border: #7F9DB9 1px solid;\n"+
					"}\n"+
					".ButtonOver\n"+
					"{\n"+
					"	background-color: #FFE1AC;\n"+
					"	border: #FFAD55 1px solid;\n"+
					"}\n"+
					".ButtonChecked\n"+
					"{\n"+
					"	background-color: #FFCB7E;\n"+
					"	border: #FFAD55 1px solid;\n"+
					"}\n"+
					".ButtonOff\n"+
					"{\n"+
					"	filter: gray() alpha(opacity=30);\n"+
					"	background-color: #C0C0C0;\n"+
					"	border: #7F9DB9 1px solid;\n"+
					"}\n"+
					"</style>\n");
			}
			base.Editor_Load(sender,e);
		}

		#region Properties

		public override bool UsesBBCode
		{
			get { return true; }
		}
		#endregion
	}


	public class RichClassEditor : ForumEditor
	{
		protected bool bInit;
		protected Type typEditor;
		protected System.Web.UI.Control objEditor;
		protected Assembly cBin;
		protected string FStyleSheet;

		public RichClassEditor(string BinFile,string ClassName)
		{
			bInit = false;
			FStyleSheet = string.Empty;
			objEditor = null;
			typEditor = null;
			BinFile = System.Web.HttpContext.Current.Server.MapPath(BinFile);
	
			try
			{
				cBin = Assembly.LoadFrom(BinFile);
				// get the interface needed
				typEditor = GetInterfaceInAssembly(cBin,ClassName);

				if (typEditor != null)
				{
					// create instance of main class
					objEditor = (System.Web.UI.Control)Activator.CreateInstance(typEditor);
					bInit = true;
				}
			}
			catch(Exception)
			{
				// dll is not accessible
			}
		}

		protected Type GetInterfaceInAssembly(Assembly cAssembly, string ClassName)
		{
			Type[] types = cAssembly.GetTypes();
			foreach (Type typ in types)
			{
				// dynamically create or activate(if exist) object
				if (typ.FullName == ClassName)
				{
					return typ;
				}
			}
			return null;
		}

		#region Properties

		protected string SafeID
		{
			get
			{
				if (bInit)
				{
					return objEditor.ClientID.Replace("$","_");
				}
				return string.Empty;															 
			}
		}

		public bool IsInitialized
		{
			get { return bInit; }
		}

		public override string StyleSheet
		{
			get { return FStyleSheet; }
			set { FStyleSheet = value;	}
		}

		public override bool UsesHTML
		{
			get	{ return true; }		
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
		#endregion

	}


	public class FCKEditor : RichClassEditor
	{
		public FCKEditor() : base("bin\\FredCK.FCKeditorV2.dll","FredCK.FCKeditorV2.FCKeditor")
		{
			
		}

		protected override void OnInit(EventArgs e)
		{			
			if (bInit)
			{
				Load += new EventHandler(Editor_Load);
				PropertyInfo pInfo = typEditor.GetProperty("ID");
				pInfo.SetValue(objEditor,"edit",null);
				Controls.Add(objEditor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (bInit && objEditor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = typEditor.GetProperty("BasePath");
				pInfo.SetValue(objEditor,ResolveUrl("FCKEditor/"),null);

				Page.RegisterClientScriptBlock("fckeditorjs",string.Format("<script language='javascript' src='{0}'></script>",ResolveUrl("FCKEditor/FCKEditor.js")));
			}
		}

		#region Properties
		public override string Text
		{
			get
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Value");
					return Convert.ToString(pInfo.GetValue(objEditor,null));
				}
				else return string.Empty;
			}
			set
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Value");
					pInfo.SetValue(objEditor,value,null);
				}
			}
		}
		#endregion
	}


	public class FreeTextBoxEditor : RichClassEditor
	{
		public FreeTextBoxEditor() : base("bin\\FreeTextBox.dll","FreeTextBoxControls.FreeTextBox")
		{
			
		}

		protected override void OnInit(EventArgs e)
		{			
			if (bInit)
			{
				Load += new EventHandler(Editor_Load);
				PropertyInfo pInfo = typEditor.GetProperty("ID");
				pInfo.SetValue(objEditor,"edit",null);
				pInfo = typEditor.GetProperty("AutoGenerateToolbarsFromString");
				pInfo.SetValue(objEditor,true,null);
				pInfo = typEditor.GetProperty("ToolbarLayout");
				pInfo.SetValue(objEditor,"FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent",null);
				Controls.Add(objEditor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender,EventArgs e)
		{
			if (bInit && objEditor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = typEditor.GetProperty("SupportFolder");
				pInfo.SetValue(objEditor,ResolveUrl("FreeTextBox/"),null);
				pInfo = typEditor.GetProperty("Width");
				pInfo.SetValue(objEditor,Unit.Percentage(100),null);
				pInfo = typEditor.GetProperty("DesignModeCss");
				pInfo.SetValue(objEditor,StyleSheet,null);
				//pInfo = typEditor.GetProperty("EnableHtmlMode");
				//pInfo.SetValue(objEditor,false,null);

				Page.RegisterClientScriptBlock("insertsmiley",
					"<script language='javascript'>\n"+
					"function insertsmiley(code){"+
					"FTB_InsertText('" + SafeID + "',code);"+
					"}\n"+
					"</script>\n");
			}
		}

		#region Properties
		public override string Text
		{
			get
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Text");
					return Convert.ToString(pInfo.GetValue(objEditor,null));
				}
				else return string.Empty;
			}
			set
			{
				if (bInit)
				{
					PropertyInfo pInfo = typEditor.GetProperty("Text");
					pInfo.SetValue(objEditor,value,null);
				}
			}
		}
		#endregion
	}


	/// <summary>
	/// This class provides a way to
	/// get information on the editors. All 
	/// functions and variables are static.
	/// </summary>
	public class EditorHelper
	{
		public enum EditorType
		{
			etText = 0,
			etBBCode = 1,
			etFCK = 2,
			etFreeTextBox = 3
		}

		public static int EditorCount = 4;

		public static string [] EditorTypeText =
		{
			"Text Editor",
			"BBCode Editor",
			"FCK Editor v2 (HTML)",
			"FreeTextBox v2 (HTML)"
		};

		public static ForumEditor CreateEditorFromType(int Value)
		{
			if (Value < EditorCount)
			{
				return CreateEditorFromType((EditorType)Value);
			}
			return null;
		}

		public static ForumEditor CreateEditorFromType(EditorType etValue)
		{	
			switch (etValue)
			{
				case EditorType.etText: return new TextEditor();
				case EditorType.etBBCode: return new BBCodeEditor();
				case EditorType.etFCK: return new FCKEditor();
				case EditorType.etFreeTextBox: return new FreeTextBoxEditor();
			}

			return null;
		}

		public static DataTable GetEditorsTable()
		{
			using(DataTable dt = new DataTable("TimeZone")) 
			{
				dt.Columns.Add("Value",Type.GetType("System.Int32"));
				dt.Columns.Add("Name",Type.GetType("System.String"));

				for (int i=0;i<EditorCount;i++)
				{
					dt.Rows.Add(new object[]{i,EditorTypeText[i]});
				}
				return dt;
			}
		}
	}
}
