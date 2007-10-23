using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace YAF.Controls
{
    public class ThemeButton : BaseControl, IPostBackEventHandler
    {
        protected string _cssClass;
        protected string _text;
        protected string _link;
        protected string _title;
        protected static object _clickEvent = new object();

        public ThemeButton()
            : base()
        {
            // Constructor logic
        }

        protected virtual void OnClick(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[_clickEvent];
            if (handler != null)
                handler(this, e);
        }

        // ????????????????????????????????????????????????????????????
		protected virtual void RaisePostBackEvent (string eventArgument)
		{
			throw new NotImplementedException ();
		}

		void IPostBackEventHandler.RaisePostBackEvent (string ea)
		{
			OnClick (EventArgs.Empty);
		}



        protected override void Render(HtmlTextWriter output)
        {
            output.BeginRender();
            output.WriteBeginTag("a");
            if (!String.IsNullOrEmpty(_cssClass)) output.WriteAttribute("class", _cssClass);
            if (!String.IsNullOrEmpty(_title)) output.WriteAttribute("title", _title);
            if (!String.IsNullOrEmpty(_link))
                output.WriteAttribute("href", _link);
            else
                output.WriteAttribute("href", string.Format("javascript:__doPostBack('{0}','{1}')",this.ClientID,""));
            
            // IE fix
            // output.WriteAttribute("onclick", "this.blur();");
            output.Write(">");
            output.WriteFullBeginTag("span");
            if (!String.IsNullOrEmpty(_text)) output.WriteEncodedText(_text);
            output.WriteEndTag("span");
            output.WriteEndTag("a");
            output.EndRender();
        }

        public event EventHandler Click
        {
            add { Events.AddHandler(_clickEvent, value); }
            remove { Events.RemoveHandler(_clickEvent, value); }
        }

        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Link
        {
            get { return _link; }
            set { _link = value; }

        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


    }
}
