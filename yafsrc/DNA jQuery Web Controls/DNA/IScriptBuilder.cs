///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace DNA
{
    public interface IScriptBuilder
    {
        bool IsPrepared { get;}
        bool IsBuilded { get; }
        void Prepare();
        void Build();
        void Reset();
        string GetApplicationLoadScript();
        string GetApplicaitonInitScript();
        string GetDocumentReadyScript();
    }
}
