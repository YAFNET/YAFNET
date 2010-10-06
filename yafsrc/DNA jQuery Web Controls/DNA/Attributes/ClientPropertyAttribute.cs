  
/// Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple=false,Inherited=true)]
    public sealed class ClientPropertyAttribute:Attribute
    {
        string _name = "";
        ClientPropertyTypes type = ClientPropertyTypes.Property;
        bool registNullValue = false;
        object defaultValue = null;
        bool isUrl = false;

        public bool IsUrl
        {
            get { return isUrl; }
            set { isUrl = value; }
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

        public ClientPropertyTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ClientPropertyAttribute(string name) { this._name = name; }
    }

}
