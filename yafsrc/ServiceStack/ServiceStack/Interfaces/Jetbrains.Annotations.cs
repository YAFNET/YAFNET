// ***********************************************************************
// <copyright file="Jetbrains.Annotations.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
/* MIT License

Copyright (c) 2016 JetBrains https://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming
namespace JetBrains.Annotations;

/// <summary>
/// Indicates that the value of the marked element could be <c>null</c> sometimes,
/// so the check for <c>null</c> is necessary before its usage.
/// </summary>
/// <example>
///   <code>
/// object Test() =&gt; null;
/// void UseTest() {
/// var p = Test();
/// var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
/// }
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
internal sealed class CanBeNullAttribute : Attribute { }

/// <summary>
/// Indicates that the value of the marked element could never be <c>null</c>.
/// </summary>
/// <example>
///   <code>
/// object Foo() {
/// return null; // Warning: Possible 'null' assignment
/// }
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
internal sealed class NotNullAttribute : Attribute { }

/// <summary>
/// Can be applied to symbols of types derived from IEnumerable as well as to symbols of Task
/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
/// or of the Lazy.Value property can never be null.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field)]
internal sealed class ItemNotNullAttribute : Attribute { }

/// <summary>
/// Can be appplied to symbols of types derived from IEnumerable as well as to symbols of Task
/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
/// or of the Lazy.Value property can be null.
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
    AttributeTargets.Delegate | AttributeTargets.Field)]
internal sealed class ItemCanBeNullAttribute : Attribute { }

/// <summary>
/// Indicates that the marked method builds string by format pattern and (optional) arguments.
/// Parameter, which contains format string, should be given in constructor. The format string
/// should be in <see cref="string.Format(IFormatProvider,string,object[])" />-like form.
/// </summary>
/// <example>
///   <code>
/// [StringFormatMethod("message")]
/// void ShowError(string message, params object[] args) { /* do something */ }
/// void Foo() {
/// ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
/// }
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Constructor | AttributeTargets.Method |
    AttributeTargets.Property | AttributeTargets.Delegate)]
internal sealed class StringFormatMethodAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatMethodAttribute" /> class.
    /// </summary>
    /// <param name="formatParameterName">Specifies which parameter of an annotated method should be treated as format-string</param>
    public StringFormatMethodAttribute(string formatParameterName)
    {
        FormatParameterName = formatParameterName;
    }

    /// <summary>
    /// Gets the name of the format parameter.
    /// </summary>
    /// <value>The name of the format parameter.</value>
    public string FormatParameterName { get; }
}

/// <summary>
/// For a parameter that is expected to be one of the limited set of values.
/// Specify fields of which type should be used as values for this parameter.
/// </summary>
[AttributeUsage(
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field,
    AllowMultiple = true)]
internal sealed class ValueProviderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueProviderAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public ValueProviderAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}

/// <summary>
/// Indicates that the function argument should be string literal and match one
/// of the parameters of the caller function. For example, ReSharper annotates
/// the parameter of <see cref="System.ArgumentNullException" />.
/// </summary>
/// <example>
///   <code>
/// void Foo(string param) {
/// if (param == null)
/// throw new ArgumentNullException("par"); // Warning: Cannot resolve symbol
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class InvokerParameterNameAttribute : Attribute { }

/// <summary>
/// Indicates that the method is contained in a type that implements
/// <c>System.ComponentModel.INotifyPropertyChanged</c> interface and this method
/// is used to notify that some property value changed.
/// </summary>
/// <example>
///   <code>
/// public class Foo : INotifyPropertyChanged {
/// public event PropertyChangedEventHandler PropertyChanged;
/// [NotifyPropertyChangedInvocator]
/// protected virtual void NotifyChanged(string propertyName) { ... }
/// string _name;
/// public string Name {
/// get { return _name; }
/// set { _name = value; NotifyChanged("LastName"); /* Warning */ }
/// }
/// }
/// </code>
/// Examples of generated notifications:
/// <list><item><c>NotifyChanged("Property")</c></item><item><c>NotifyChanged(() =&gt; Property)</c></item><item><c>NotifyChanged((VM x) =&gt; x.Property)</c></item><item><c>SetProperty(ref myField, value, "Property")</c></item></list></example>
/// <remarks>The method should be non-static and conform to one of the supported signatures:
/// <list><item><c>NotifyChanged(string)</c></item><item><c>NotifyChanged(params string[])</c></item><item><c>NotifyChanged{T}(Expression{Func{T}})</c></item><item><c>NotifyChanged{T,U}(Expression{Func{T,U}})</c></item><item><c>SetProperty{T}(ref T, T, string)</c></item></list></remarks>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyPropertyChangedInvocatorAttribute" /> class.
    /// </summary>
    public NotifyPropertyChangedInvocatorAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyPropertyChangedInvocatorAttribute" /> class.
    /// </summary>
    /// <param name="parameterName">Name of the parameter.</param>
    public NotifyPropertyChangedInvocatorAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    /// <value>The name of the parameter.</value>
    public string ParameterName { get; }
}

/// <summary>
/// Describes dependency between method input and output.
/// </summary>
/// <syntax>
///   <p>Function Definition Table syntax:</p>
///   <list>
///     <item>FDT      ::= FDTRow [;FDTRow]*</item>
///     <item>FDTRow   ::= Input =&gt; Output | Output &lt;= Input</item>
///     <item>Input    ::= ParameterName: Value [, Input]*</item>
///     <item>Output   ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
///     <item>Value    ::= true | false | null | notnull | canbenull</item>
///   </list>
/// If method has single input parameter, it's name could be omitted.<br />
/// Using <c>halt</c> (or <c>void</c>/<c>nothing</c>, which is the same) for method output
/// means that the methos doesn't return normally (throws or terminates the process).<br />
/// Value <c>canbenull</c> is only applicable for output parameters.<br />
/// You can use multiple <c>[ContractAnnotation]</c> for each FDT row, or use single attribute
/// with rows separated by semicolon. There is no notion of order rows, all rows are checked
/// for applicability and applied per each program state tracked by R# analysis.<br /></syntax>
/// <examples>
///   <list>
///     <item>
///       <code>
/// [ContractAnnotation("=&gt; halt")]
/// public void TerminationMethod()
/// </code>
///     </item>
///     <item>
///       <code>
/// [ContractAnnotation("halt &lt;= condition: false")]
/// public void Assert(bool condition, string text) // regular assertion method
/// </code>
///     </item>
///     <item>
///       <code>
/// [ContractAnnotation("s:null =&gt; true")]
/// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
/// </code>
///     </item>
///     <item>
///       <code>
/// // A method that returns null if the parameter is null,
/// // and not null if the parameter is not null
/// [ContractAnnotation("null =&gt; null; notnull =&gt; notnull")]
/// public object Transform(object data)
/// </code>
///     </item>
///     <item>
///       <code>
/// [ContractAnnotation("=&gt; true, result: notnull; =&gt; false, result: null")]
/// public bool TryParse(string s, out Person result)
/// </code>
///     </item>
///   </list>
/// </examples>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class ContractAnnotationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContractAnnotationAttribute" /> class.
    /// </summary>
    /// <param name="contract">The contract.</param>
    public ContractAnnotationAttribute(string contract)
        : this(contract, false) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractAnnotationAttribute" /> class.
    /// </summary>
    /// <param name="contract">The contract.</param>
    /// <param name="forceFullStates">if set to <c>true</c> [force full states].</param>
    public ContractAnnotationAttribute(string contract, bool forceFullStates)
    {
        Contract = contract;
        ForceFullStates = forceFullStates;
    }

    /// <summary>
    /// Gets the contract.
    /// </summary>
    /// <value>The contract.</value>
    public string Contract { get; }

    /// <summary>
    /// Gets a value indicating whether [force full states].
    /// </summary>
    /// <value><c>true</c> if [force full states]; otherwise, <c>false</c>.</value>
    public bool ForceFullStates { get; }
}

/// <summary>
/// Indicates that marked element should be localized or not.
/// </summary>
/// <example>
///   <code>
/// [LocalizationRequiredAttribute(true)]
/// class Foo {
/// string str = "my string"; // Warning: Localizable string
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.All)]
internal sealed class LocalizationRequiredAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationRequiredAttribute" /> class.
    /// </summary>
    public LocalizationRequiredAttribute() : this(true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationRequiredAttribute" /> class.
    /// </summary>
    /// <param name="required">if set to <c>true</c> [required].</param>
    public LocalizationRequiredAttribute(bool required)
    {
        Required = required;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="LocalizationRequiredAttribute" /> is required.
    /// </summary>
    /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
    public bool Required { get; }
}

/// <summary>
/// Indicates that the value of the marked type (or its derivatives)
/// cannot be compared using '==' or '!=' operators and <c>Equals()</c>
/// should be used instead. However, using '==' or '!=' for comparison
/// with <c>null</c> is always permitted.
/// </summary>
/// <example>
///   <code>
/// [CannotApplyEqualityOperator]
/// class NoEquality { }
/// class UsesNoEquality {
/// void Test() {
/// var ca1 = new NoEquality();
/// var ca2 = new NoEquality();
/// if (ca1 != null) { // OK
/// bool condition = ca1 == ca2; // Warning
/// }
/// }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
internal sealed class CannotApplyEqualityOperatorAttribute : Attribute { }

/// <summary>
/// When applied to a target attribute, specifies a requirement for any type marked
/// with the target attribute to implement or inherit specific type or types.
/// </summary>
/// <example>
///   <code>
/// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
/// class ComponentAttribute : Attribute { }
/// [Component] // ComponentAttribute requires implementing IComponent interface
/// class MyComponent : IComponent { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[BaseTypeRequired(typeof(Attribute))]
internal sealed class BaseTypeRequiredAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTypeRequiredAttribute" /> class.
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    public BaseTypeRequiredAttribute(Type baseType)
    {
        BaseType = baseType;
    }

    /// <summary>
    /// Gets the type of the base.
    /// </summary>
    /// <value>The type of the base.</value>
    public Type BaseType { get; }
}

/// <summary>
/// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
/// so this symbol will not be marked as unused (as well as by other usage inspections).
/// </summary>
[AttributeUsage(AttributeTargets.All)]
internal sealed class UsedImplicitlyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
    /// </summary>
    public UsedImplicitlyAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
    /// </summary>
    /// <param name="useKindFlags">The use kind flags.</param>
    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
    /// </summary>
    /// <param name="targetFlags">The target flags.</param>
    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
    /// </summary>
    /// <param name="useKindFlags">The use kind flags.</param>
    /// <param name="targetFlags">The target flags.</param>
    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>
    /// Gets the use kind flags.
    /// </summary>
    /// <value>The use kind flags.</value>
    public ImplicitUseKindFlags UseKindFlags { get; }

    /// <summary>
    /// Gets the target flags.
    /// </summary>
    /// <value>The target flags.</value>
    public ImplicitUseTargetFlags TargetFlags { get; }
}

/// <summary>
/// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes
/// as unused (as well as by other usage inspections)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter)]
internal sealed class MeansImplicitUseAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
    /// </summary>
    public MeansImplicitUseAttribute()
        : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
    /// </summary>
    /// <param name="useKindFlags">The use kind flags.</param>
    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
        : this(useKindFlags, ImplicitUseTargetFlags.Default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
    /// </summary>
    /// <param name="targetFlags">The target flags.</param>
    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
        : this(ImplicitUseKindFlags.Default, targetFlags) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
    /// </summary>
    /// <param name="useKindFlags">The use kind flags.</param>
    /// <param name="targetFlags">The target flags.</param>
    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
        UseKindFlags = useKindFlags;
        TargetFlags = targetFlags;
    }

    /// <summary>
    /// Gets the use kind flags.
    /// </summary>
    /// <value>The use kind flags.</value>
    [UsedImplicitly] public ImplicitUseKindFlags UseKindFlags { get; private set; }

    /// <summary>
    /// Gets the target flags.
    /// </summary>
    /// <value>The target flags.</value>
    [UsedImplicitly] public ImplicitUseTargetFlags TargetFlags { get; private set; }
}

/// <summary>
/// Enum ImplicitUseKindFlags
/// </summary>
[Flags]
internal enum ImplicitUseKindFlags
{
    /// <summary>
    /// The default
    /// </summary>
    Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

    /// <summary>
    /// Only entity marked with attribute considered used.
    /// </summary>
    Access = 1,

    /// <summary>
    /// Indicates implicit assignment to a member.
    /// </summary>
    Assign = 2,

    /// <summary>
    /// Indicates implicit instantiation of a type with fixed constructor signature.
    /// That means any unused constructor parameters won't be reported as such.
    /// </summary>
    InstantiatedWithFixedConstructorSignature = 4,

    /// <summary>
    /// Indicates implicit instantiation of a type.
    /// </summary>
    InstantiatedNoFixedConstructorSignature = 8,
}

/// <summary>
/// Specify what is considered used implicitly when marked
/// with <see cref="MeansImplicitUseAttribute" /> or <see cref="UsedImplicitlyAttribute" />.
/// </summary>
[Flags]
internal enum ImplicitUseTargetFlags
{
    /// <summary>
    /// The default
    /// </summary>
    Default = Itself,

    /// <summary>
    /// The itself
    /// </summary>
    Itself = 1,

    /// <summary>
    /// Members of entity marked with attribute are considered used.
    /// </summary>
    Members = 2,

    /// <summary>
    /// Entity marked with attribute and all its members considered used.
    /// </summary>
    WithMembers = Itself | Members
}

/// <summary>
/// This attribute is intended to mark publicly available API
/// which should not be removed and so is treated as used.
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
internal sealed class PublicAPIAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicAPIAttribute" /> class.
    /// </summary>
    public PublicAPIAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicAPIAttribute" /> class.
    /// </summary>
    /// <param name="comment">The comment.</param>
    public PublicAPIAttribute(string comment)
    {
        Comment = comment;
    }

    /// <summary>
    /// Gets the comment.
    /// </summary>
    /// <value>The comment.</value>
    public string Comment { get; }
}

/// <summary>
/// Tells code analysis engine if the parameter is completely handled when the invoked method is on stack.
/// If the parameter is a delegate, indicates that delegate is executed while the method is executed.
/// If the parameter is an enumerable, indicates that it is enumerated while the method is executed.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class InstantHandleAttribute : Attribute { }

/// <summary>
/// Indicates that a method does not make any observable state changes.
/// The same as <c>System.Diagnostics.Contracts.PureAttribute</c>.
/// </summary>
/// <example>
///   <code>
/// [Pure] int Multiply(int x, int y) =&gt; x * y;
/// void M() {
/// Multiply(123, 42); // Waring: Return value of pure method is not used
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class PureAttribute : Attribute { }

/// <summary>
/// Indicates that the return value of method invocation must be used.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class MustUseReturnValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MustUseReturnValueAttribute" /> class.
    /// </summary>
    public MustUseReturnValueAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MustUseReturnValueAttribute" /> class.
    /// </summary>
    /// <param name="justification">The justification.</param>
    public MustUseReturnValueAttribute(string justification)
    {
        Justification = justification;
    }

    /// <summary>
    /// Gets the justification.
    /// </summary>
    /// <value>The justification.</value>
    public string Justification { get; }
}

/// <summary>
/// Indicates the type member or parameter of some type, that should be used instead of all other ways
/// to get the value that type. This annotation is useful when you have some "context" value evaluated
/// and stored somewhere, meaning that all other ways to get this value must be consolidated with existing one.
/// </summary>
/// <example>
///   <code>
/// class Foo {
/// [ProvidesContext] IBarService _barService = ...;
/// void ProcessNode(INode node) {
/// DoSomething(node, node.GetGlobalServices().Bar);
/// //              ^ Warning: use value of '_barService' field
/// }
/// }
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Method |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.GenericParameter)]
internal sealed class ProvidesContextAttribute : Attribute { }

/// <summary>
/// Indicates that a parameter is a path to a file or a folder within a web project.
/// Path can be relative or absolute, starting from web root (~).
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class PathReferenceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathReferenceAttribute" /> class.
    /// </summary>
    public PathReferenceAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathReferenceAttribute" /> class.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    public PathReferenceAttribute([NotNull, PathReference] string basePath)
    {
        BasePath = basePath;
    }

    /// <summary>
    /// Gets the base path.
    /// </summary>
    /// <value>The base path.</value>
    public string BasePath { get; }
}

/// <summary>
/// An extension method marked with this attribute is processed by ReSharper code completion
/// as a 'Source Template'. When extension method is completed over some expression, it's source code
/// is automatically expanded like a template at call site.
/// </summary>
/// <example>
/// In this example, the 'forEach' method is a source template available over all values
/// of enumerable types, producing ordinary C# 'foreach' statement and placing caret inside block:
/// <code>
/// [SourceTemplate]
/// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; xs) {
/// foreach (var x in xs) {
/// //$ $END$
/// }
/// }
/// </code></example>
/// <remarks>Template method body can contain valid source code and/or special comments starting with '$'.
/// Text inside these comments is added as source code when the template is applied. Template parameters
/// can be used either as additional method parameters or as identifiers wrapped in two '$' signs.
/// Use the <see cref="MacroAttribute" /> attribute to specify macros for parameters.</remarks>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class SourceTemplateAttribute : Attribute { }

/// <summary>
/// Allows specifying a macro for a parameter of a <see cref="SourceTemplateAttribute">source template</see>.
/// </summary>
/// <example>
/// Applying the attribute on a source template method:
/// <code>
/// [SourceTemplate, Macro(Target = "item", Expression = "suggestVariableName()")]
/// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; collection) {
/// foreach (var item in collection) {
/// //$ $END$
/// }
/// }
/// </code>
/// Applying the attribute on a template method parameter:
/// <code>
/// [SourceTemplate]
/// public static void something(this Entity x, [Macro(Expression = "guid()", Editable = -1)] string newguid) {
/// /*$ var $x$Id = "$newguid$" + x.ToString();
/// x.DoSomething($x$Id); */
/// }
/// </code></example>
/// <remarks>You can apply the attribute on the whole method or on any of its additional parameters. The macro expression
/// is defined in the <see cref="MacroAttribute.Expression" /> property. When applied on a method, the target
/// template parameter is defined in the <see cref="MacroAttribute.Target" /> property. To apply the macro silently
/// for the parameter, set the <see cref="MacroAttribute.Editable" /> property value = -1.</remarks>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
internal sealed class MacroAttribute : Attribute
{
    /// <summary>
    /// Allows specifying a macro that will be executed for a <see cref="SourceTemplateAttribute">source template</see>
    /// parameter when the template is expanded.
    /// </summary>
    /// <value>The expression.</value>
    public string Expression { get; set; }

    /// <summary>
    /// Allows specifying which occurrence of the target parameter becomes editable when the template is deployed.
    /// </summary>
    /// <value>The editable.</value>
    /// &gt;
    /// <remarks>If the target parameter is used several times in the template, only one occurrence becomes editable;
    /// other occurrences are changed synchronously. To specify the zero-based index of the editable occurrence,
    /// use values &gt;= 0. To make the parameter non-editable when the template is expanded, use -1.</remarks>
    public int Editable { get; set; }

    /// <summary>
    /// Identifies the target parameter of a <see cref="SourceTemplateAttribute">source template</see> if the
    /// <see cref="MacroAttribute" /> is applied on a template method.
    /// </summary>
    /// <value>The target.</value>
    public string Target { get; set; }
}

/// <summary>
/// Class AspMvcAreaMasterLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcAreaMasterLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcAreaMasterLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcAreaMasterLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// Class AspMvcAreaPartialViewLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcAreaPartialViewLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcAreaPartialViewLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcAreaPartialViewLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// Class AspMvcAreaViewLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcAreaViewLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcAreaViewLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcAreaViewLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// Class AspMvcMasterLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcMasterLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcMasterLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcMasterLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// Class AspMvcPartialViewLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcPartialViewLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcPartialViewLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcPartialViewLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// Class AspMvcViewLocationFormatAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
internal sealed class AspMvcViewLocationFormatAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcViewLocationFormatAttribute" /> class.
    /// </summary>
    /// <param name="format">The format.</param>
    public AspMvcViewLocationFormatAttribute(string format)
    {
        Format = format;
    }

    /// <summary>
    /// Gets the format.
    /// </summary>
    /// <value>The format.</value>
    public string Format { get; }
}

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC action. If applied to a method, the MVC action name is calculated
/// implicitly from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class AspMvcActionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcActionAttribute" /> class.
    /// </summary>
    public AspMvcActionAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcActionAttribute" /> class.
    /// </summary>
    /// <param name="anonymousProperty">The anonymous property.</param>
    public AspMvcActionAttribute(string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    /// <summary>
    /// Gets the anonymous property.
    /// </summary>
    /// <value>The anonymous property.</value>
    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC area.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcAreaAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcAreaAttribute" /> class.
    /// </summary>
    public AspMvcAreaAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcAreaAttribute" /> class.
    /// </summary>
    /// <param name="anonymousProperty">The anonymous property.</param>
    public AspMvcAreaAttribute(string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    /// <summary>
    /// Gets the anonymous property.
    /// </summary>
    /// <value>The anonymous property.</value>
    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter is
/// an MVC controller. If applied to a method, the MVC controller name is calculated
/// implicitly from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class AspMvcControllerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcControllerAttribute" /> class.
    /// </summary>
    public AspMvcControllerAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspMvcControllerAttribute" /> class.
    /// </summary>
    /// <param name="anonymousProperty">The anonymous property.</param>
    public AspMvcControllerAttribute(string anonymousProperty)
    {
        AnonymousProperty = anonymousProperty;
    }

    /// <summary>
    /// Gets the anonymous property.
    /// </summary>
    /// <value>The anonymous property.</value>
    public string AnonymousProperty { get; }
}

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC Master. Use this attribute
/// for custom wrappers similar to <c>System.Web.Mvc.Controller.View(String, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcMasterAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC model type. Use this attribute
/// for custom wrappers similar to <c>System.Web.Mvc.Controller.View(String, Object)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcModelTypeAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter is an MVC
/// partial view. If applied to a method, the MVC partial view name is calculated implicitly
/// from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class AspMvcPartialViewAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. Allows disabling inspections for MVC views within a class or a method.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class AspMvcSuppressViewErrorAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC display template.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.DisplayExtensions.DisplayForModel(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcDisplayTemplateAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC editor template.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Html.EditorExtensions.EditorForModel(HtmlHelper, String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcEditorTemplateAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. Indicates that a parameter is an MVC template.
/// Use this attribute for custom wrappers similar to
/// <c>System.ComponentModel.DataAnnotations.UIHintAttribute(System.String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcTemplateAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC view component. If applied to a method, the MVC view name is calculated implicitly
/// from the context. Use this attribute for custom wrappers similar to
/// <c>System.Web.Mvc.Controller.View(Object)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class AspMvcViewAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC view component name.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AspMvcViewComponentAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. If applied to a parameter, indicates that the parameter
/// is an MVC view component view. If applied to a method, the MVC view component view name is default.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class AspMvcViewComponentViewAttribute : Attribute { }

/// <summary>
/// ASP.NET MVC attribute. When applied to a parameter of an attribute,
/// indicates that this parameter is an MVC action name.
/// </summary>
/// <example>
///   <code>
/// [ActionName("Foo")]
/// public ActionResult Login(string returnUrl) {
/// ViewBag.ReturnUrl = Url.Action("Foo"); // OK
/// return RedirectToAction("Bar"); // Error: Cannot resolve action
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
internal sealed class AspMvcActionSelectorAttribute : Attribute { }

/// <summary>
/// Class HtmlElementAttributesAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
internal sealed class HtmlElementAttributesAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlElementAttributesAttribute" /> class.
    /// </summary>
    public HtmlElementAttributesAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlElementAttributesAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public HtmlElementAttributesAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}

/// <summary>
/// Class HtmlAttributeValueAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
internal sealed class HtmlAttributeValueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HtmlAttributeValueAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public HtmlAttributeValueAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}

/// <summary>
/// Razor attribute. Indicates that a parameter or a method is a Razor section.
/// Use this attribute for custom wrappers similar to
/// <c>System.Web.WebPages.WebPageBase.RenderSection(String)</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
internal sealed class RazorSectionAttribute : Attribute { }

/// <summary>
/// Indicates how method, constructor invocation or property access
/// over collection type affects content of the collection.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property)]
internal sealed class CollectionAccessAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionAccessAttribute" /> class.
    /// </summary>
    /// <param name="collectionAccessType">Type of the collection access.</param>
    public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
    {
        CollectionAccessType = collectionAccessType;
    }

    /// <summary>
    /// Gets the type of the collection access.
    /// </summary>
    /// <value>The type of the collection access.</value>
    public CollectionAccessType CollectionAccessType { get; }
}

/// <summary>
/// Enum CollectionAccessType
/// </summary>
[Flags]
internal enum CollectionAccessType
{
    /// <summary>
    /// Method does not use or modify content of the collection.
    /// </summary>
    None = 0,

    /// <summary>
    /// Method only reads content of the collection but does not modify it.
    /// </summary>
    Read = 1,

    /// <summary>
    /// Method can change content of the collection but does not add new elements.
    /// </summary>
    ModifyExistingContent = 2,

    /// <summary>
    /// Method can add new elements to the collection.
    /// </summary>
    UpdatedContent = ModifyExistingContent | 4
}

/// <summary>
/// Indicates that the marked method is assertion method, i.e. it halts control flow if
/// one of the conditions is satisfied. To set the condition, mark one of the parameters with
/// <see cref="AssertionConditionAttribute" /> attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class AssertionMethodAttribute : Attribute { }

/// <summary>
/// Indicates the condition parameter of the assertion method. The method itself should be
/// marked by <see cref="AssertionMethodAttribute" /> attribute. The mandatory argument of
/// the attribute is the assertion type.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class AssertionConditionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionConditionAttribute" /> class.
    /// </summary>
    /// <param name="conditionType">Type of the condition.</param>
    public AssertionConditionAttribute(AssertionConditionType conditionType)
    {
        ConditionType = conditionType;
    }

    /// <summary>
    /// Gets the type of the condition.
    /// </summary>
    /// <value>The type of the condition.</value>
    public AssertionConditionType ConditionType { get; }
}

/// <summary>
/// Specifies assertion type. If the assertion method argument satisfies the condition,
/// then the execution continues. Otherwise, execution is assumed to be halted.
/// </summary>
internal enum AssertionConditionType
{
    /// <summary>
    /// Marked parameter should be evaluated to true.
    /// </summary>
    IS_TRUE = 0,

    /// <summary>
    /// Marked parameter should be evaluated to false.
    /// </summary>
    IS_FALSE = 1,

    /// <summary>
    /// Marked parameter should be evaluated to null value.
    /// </summary>
    IS_NULL = 2,

    /// <summary>
    /// Marked parameter should be evaluated to not null value.
    /// </summary>
    IS_NOT_NULL = 3,
}

/// <summary>
/// Indicates that method is pure LINQ method, with postponed enumeration (like Enumerable.Select,
/// .Where). This annotation allows inference of [InstantHandle] annotation for parameters
/// of delegate type by analyzing LINQ method chains.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class LinqTunnelAttribute : Attribute { }

/// <summary>
/// Indicates that IEnumerable, passed as parameter, is not enumerated.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NoEnumerationAttribute : Attribute { }

/// <summary>
/// Indicates that parameter is regular expression pattern.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class RegexPatternAttribute : Attribute { }

/// <summary>
/// Prevents the Member Reordering feature from tossing members of the marked class.
/// </summary>
/// <remarks>The attribute must be mentioned in your member reordering patterns</remarks>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum)]
internal sealed class NoReorderAttribute : Attribute { }

/// <summary>
/// XAML attribute. Indicates the type that has <c>ItemsSource</c> property and should be treated
/// as <c>ItemsControl</c>-derived type, to enable inner items <c>DataContext</c> type resolve.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class XamlItemsControlAttribute : Attribute { }

/// <summary>
/// XAML attribute. Indicates the property of some <c>BindingBase</c>-derived type, that
/// is used to bind some item of <c>ItemsControl</c>-derived type. This annotation will
/// enable the <c>DataContext</c> type resolve for XAML bindings for such properties.
/// </summary>
/// <remarks>Property should have the tree ancestor of the <c>ItemsControl</c> type or
/// marked with the <see cref="XamlItemsControlAttribute" /> attribute.</remarks>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class XamlItemBindingOfItemsControlAttribute : Attribute { }

/// <summary>
/// Class AspChildControlTypeAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class AspChildControlTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspChildControlTypeAttribute" /> class.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="controlType">Type of the control.</param>
    public AspChildControlTypeAttribute(string tagName, Type controlType)
    {
        TagName = tagName;
        ControlType = controlType;
    }

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName { get; }

    /// <summary>
    /// Gets the type of the control.
    /// </summary>
    /// <value>The type of the control.</value>
    public Type ControlType { get; }
}

/// <summary>
/// Class AspDataFieldAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
internal sealed class AspDataFieldAttribute : Attribute { }

/// <summary>
/// Class AspDataFieldsAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
internal sealed class AspDataFieldsAttribute : Attribute { }

/// <summary>
/// Class AspMethodPropertyAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property)]
internal sealed class AspMethodPropertyAttribute : Attribute { }

/// <summary>
/// Class AspRequiredAttributeAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal sealed class AspRequiredAttributeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspRequiredAttributeAttribute" /> class.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    public AspRequiredAttributeAttribute(string attribute)
    {
        Attribute = attribute;
    }

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    /// <value>The attribute.</value>
    public string Attribute { get; }
}

/// <summary>
/// Class AspTypePropertyAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property)]
internal sealed class AspTypePropertyAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether [create constructor references].
    /// </summary>
    /// <value><c>true</c> if [create constructor references]; otherwise, <c>false</c>.</value>
    public bool CreateConstructorReferences { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AspTypePropertyAttribute" /> class.
    /// </summary>
    /// <param name="createConstructorReferences">if set to <c>true</c> [create constructor references].</param>
    public AspTypePropertyAttribute(bool createConstructorReferences)
    {
        CreateConstructorReferences = createConstructorReferences;
    }
}

/// <summary>
/// Class RazorImportNamespaceAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class RazorImportNamespaceAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorImportNamespaceAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public RazorImportNamespaceAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}

/// <summary>
/// Class RazorInjectionAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class RazorInjectionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorInjectionAttribute" /> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fieldName">Name of the field.</param>
    public RazorInjectionAttribute(string type, string fieldName)
    {
        Type = type;
        FieldName = fieldName;
    }

    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; }

    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName { get; }
}

/// <summary>
/// Class RazorDirectiveAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class RazorDirectiveAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorDirectiveAttribute" /> class.
    /// </summary>
    /// <param name="directive">The directive.</param>
    public RazorDirectiveAttribute(string directive)
    {
        Directive = directive;
    }

    /// <summary>
    /// Gets the directive.
    /// </summary>
    /// <value>The directive.</value>
    public string Directive { get; }
}

/// <summary>
/// Class RazorPageBaseTypeAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
internal sealed class RazorPageBaseTypeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RazorPageBaseTypeAttribute" /> class.
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    public RazorPageBaseTypeAttribute(string baseType)
    {
        BaseType = baseType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RazorPageBaseTypeAttribute" /> class.
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    /// <param name="pageName">Name of the page.</param>
    public RazorPageBaseTypeAttribute(string baseType, string pageName)
    {
        BaseType = baseType;
        PageName = pageName;
    }

    /// <summary>
    /// Gets the type of the base.
    /// </summary>
    /// <value>The type of the base.</value>
    public string BaseType { get; }

    /// <summary>
    /// Gets the name of the page.
    /// </summary>
    /// <value>The name of the page.</value>
    public string PageName { get; }
}

/// <summary>
/// Class RazorHelperCommonAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method)]
internal sealed class RazorHelperCommonAttribute : Attribute { }

/// <summary>
/// Class RazorLayoutAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Property)]
internal sealed class RazorLayoutAttribute : Attribute { }

/// <summary>
/// Class RazorWriteLiteralMethodAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method)]
internal sealed class RazorWriteLiteralMethodAttribute : Attribute { }

/// <summary>
/// Class RazorWriteMethodAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method)]
internal sealed class RazorWriteMethodAttribute : Attribute { }

/// <summary>
/// Class RazorWriteMethodParameterAttribute. This class cannot be inherited.
/// Implements the <see cref="System.Attribute" />
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class RazorWriteMethodParameterAttribute : Attribute { }