
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false,Inherited=true)]
    public sealed  class CallBackMethodAttribute : Attribute
    {
        private bool hasCallbackResult = false;
        private string friendlyName = "";

        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }

        public bool HasCallbackResult
        {
            get { return hasCallbackResult; }
            set { hasCallbackResult = value; }
        }
    }
}
