using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("jQuery javascript library")]
[assembly: AssemblyDescription("This library is complie the javascript files in assembly for .net please visit http://www.jQueryNet.com to get the latest version of jquery")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("jQuery javascript library")]
[assembly: AssemblyCopyright("Copyright ©  2009 jQuery team")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("8d7616b5-4296-4f5a-8c93-dbbe72f4a47a")]
[assembly: AssemblyVersion("1.3.2.0")]
[assembly: AssemblyFileVersion("1.3.2.0")]

#if (!COMPACT_FRAMEWORK)

[assembly: AllowPartiallyTrustedCallers]
#endif

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("..\\..\\YetAnotherForum.NET.snk")]

#region plugins
[assembly: WebResource("jQueryNet.plugins.bgiframe.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.cookie.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.simulate.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.jsdiff.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.testrunner.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.lightbox.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.easing.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.easing.1.3.js", "text/javascript")]
[assembly: WebResource("jQueryNet.plugins.pngFix.js", "text/javascript")]
#endregion

#region effects
[assembly: WebResource("jQueryNet.effects.blind.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.bounce.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.clip.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.core.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.drop.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.explode.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.fold.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.highlight.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.pulsate.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.scale.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.shake.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.slide.js", "text/javascript")]
[assembly: WebResource("jQueryNet.effects.transfer.js", "text/javascript")]
#endregion

#region ui
[assembly: WebResource("jQueryNet.ui.core.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.datepicker.loc.all.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.all.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.accordion.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.autocomplete.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.button.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.datepicker.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.dialog.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.draggable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.dialog.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.draggable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.droppable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.progressbar.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.resizable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.selectable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.slider.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.sortable.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.tabs.js", "text/javascript")]
[assembly: WebResource("jQueryNet.ui.widget.js", "text/javascript")]
#endregion

[assembly: WebResource("jQueryNet.core.js", "text/javascript")]

#region datepicker languages
[assembly: WebResource("jQueryNet.lang.ar.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.bg.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ca.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.cs.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.da.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.de.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.el.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.eo.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.es.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.fa.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.fi.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.fr.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.he.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.hr.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.id.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.is.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.it.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ja.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ko.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.lt.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.lv.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ms.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.nl.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.no.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.pl.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.pt-BR.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ro.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.ru.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.sk.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.sl.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.sq.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.sv.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.th.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.tr.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.uk.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.zh-CN.js", "text/javascript")]
[assembly: WebResource("jQueryNet.lang.zh-TW.js", "text/javascript")]
#endregion