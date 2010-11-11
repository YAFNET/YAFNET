//  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

#region Using

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;

#endregion

[assembly: AssemblyTitle("DNA Common UI Framework")]
[assembly: AssemblyDescription("This library is the base library of all DNA Control or Component to uses")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("DotNetAge")]
[assembly: AssemblyProduct("DNA Common UI Framework")]
[assembly: AssemblyCopyright("Copyright © 2009 Ray Liang (http://www.dotnetage.com)")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("75b79da8-5ef9-4b20-8721-9da462202b2f")]
[assembly: AssemblyVersion("1.1.15.0")]
[assembly: AssemblyFileVersion("1.1.15.0")]
[assembly: WebResource("DNA.UI.ClientScripts.CallBack.js", "text/javascript")]

#if (!COMPACT_FRAMEWORK)

[assembly: AllowPartiallyTrustedCallers]
#endif

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("..\\..\\YetAnotherForum.NET.snk")]