
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI.JQuery
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class JQueryOptionAttribute:Attribute
    {
        private string name;
        private string target = "";
        private bool registNullValue = false;
        private object defaultValue = null;
        private JQueryOptionTypes type = JQueryOptionTypes.Value;
        private object ignoreValue=null;
        private string[] functionParams = null;

        public string[] FunctionParams
        {
            get { return functionParams; }
            set { functionParams = value; }
        }

        public object IgnoreValue
        {
            get { return ignoreValue; }
            set { ignoreValue = value; }
        }

        public JQueryOptionAttribute(){}
        
        public JQueryOptionAttribute(string option)
        {
            this.name=option;
        }

        public JQueryOptionAttribute(string option,JQueryOptionTypes valueType)
        {
            this.name = option;
            this.type = valueType;
        }

        public string Target
        {
            get { return target; }
            set { target = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        public bool RegistNullValue
        {
            get { return registNullValue; }
            set { registNullValue = value; }
        }

        public JQueryOptionTypes Type
        {
            get { return type; }
            set { type = value; }
        }
    }

}
