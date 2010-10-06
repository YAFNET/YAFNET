 
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
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = true) ]
    public sealed class ScriptReferenceAttribute : Attribute
    {
        public ScriptReferenceAttribute(string name)
        {
            _name = name;
            _assembly = this.GetType().Assembly.FullName;
        }

        public ScriptReferenceAttribute(string name, string assembly)
        {
            _name = name;
            _assembly = assembly;
        }
        string _name = "";
        int loadOrder = 1;

        public int LoadOrder
        {
            get { return loadOrder; }
            set { loadOrder = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        string _assembly = "";

        public string Assembly
        {
            get { return _assembly; }
            set { _assembly = value; }
        }
    }
}
