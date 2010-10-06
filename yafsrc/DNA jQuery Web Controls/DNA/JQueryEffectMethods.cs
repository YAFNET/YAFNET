
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
    public enum JQueryEffectMethods
    {
        none,
        /// <summary>
        /// The enhanced show method optionally accepts jQuery UI advanced effects.
        /// </summary>
        show,
        hide,
        toggle,
        slideDown,
        slideUp,
        slideToggle,
        fadeIn,
        fadeOut,
        fadeTo
    }
}
