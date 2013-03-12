// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Tiny Gecko" file="AssemblyInfo.cs">
//   Copyright (c) 2006-2011 Jaben Cargman
// </copyright>
// <summary>
//   AssemblyInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

using YAF.Types.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("YAF.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tiny Gecko")]
[assembly: AssemblyProduct("YAF.Core")]
[assembly: AssemblyCopyright("Copyright © 2006-2013 Yet Another Forum.NET")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyModuleSortOrder(10)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5d0e8405-371c-4cf8-817d-bd1000631c65")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

#if (!COMPACT_FRAMEWORK)

[assembly: AllowPartiallyTrustedCallers]
#endif

#if !NCRUNCH
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("..\\YetAnotherForum.NET.snk")]
#endif