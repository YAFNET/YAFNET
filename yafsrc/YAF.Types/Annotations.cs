/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Types
{
  #region Using

  using System;
  using System.Collections.Generic;

  #endregion

  /// <summary>
  /// Indicates that marked element should be localized or not.
  /// </summary>
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public sealed class LocalizationRequiredAttribute : Attribute
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationRequiredAttribute"/> class.
    /// </summary>
    /// <param name="required">
    /// <c>true</c> if a element should be localized; otherwise, <c>false</c>.
    /// </param>
    public LocalizationRequiredAttribute(bool required)
    {
      this.Required = required;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether a element should be localized.
    ///   <value><c>true</c> if a element should be localized; otherwise, <c>false</c>.</value>
    /// </summary>
    public bool Required { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the value of the given object is equal to the current <see cref="LocalizationRequiredAttribute"/>.
    /// </summary>
    /// <param name="obj">
    /// The object to test the value equality of. 
    /// </param>
    /// <returns>
    /// <c>true</c> if the value of the given object is equal to that of the current; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals([NotNull] object obj)
    {
      var attribute = obj as LocalizationRequiredAttribute;
      return attribute != null && attribute.Required == this.Required;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="LocalizationRequiredAttribute"/>.
    /// </returns>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    #endregion
  }

  /// <summary>
  /// Indicates that marked method builds string by format pattern and (optional) arguments. 
  ///   Parameter, which contains format string, should be given in constructor.
  ///   The format string should be in <see cref="string.Format(IFormatProvider,string,object[])"/> -like form
  /// </summary>
  [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class StringFormatMethodAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The my format parameter name.
    /// </summary>
    private readonly string myFormatParameterName;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringFormatMethodAttribute"/> class. 
    /// Initializes new instance of StringFormatMethodAttribute
    /// </summary>
    /// <param name="formatParameterName">
    /// Specifies which parameter of an annotated method should be treated as format-string
    /// </param>
    public StringFormatMethodAttribute([NotNull] string formatParameterName)
    {
      this.myFormatParameterName = formatParameterName;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets format parameter name
    /// </summary>
    public string FormatParameterName
    {
      get
      {
        return this.myFormatParameterName;
      }
    }

    #endregion
  }

  /// <summary>
  /// Indicates that the function argument should be string literal and match one  of the parameters of the caller function.
  ///   For example, <see cref="ArgumentNullException"/> has such parameter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public sealed class InvokerParameterNameAttribute : Attribute
  {
  }

  /// <summary>
  /// Indicates that the marked method is assertion method, i.e. it halts control flow if one of the conditions is satisfied. 
  ///   To set the condition, mark one of the parameters with <see cref="AssertionConditionAttribute"/> attribute
  /// </summary>
  /// <seealso cref="AssertionConditionAttribute"/>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class AssertionMethodAttribute : Attribute
  {
  }

  /// <summary>
  /// Indicates the condition parameter of the assertion method. 
  ///   The method itself should be marked by <see cref="AssertionMethodAttribute"/> attribute.
  ///   The mandatory argument of the attribute is the assertion type.
  /// </summary>
  /// <seealso cref="AssertionConditionType"/>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
  public sealed class AssertionConditionAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The my condition type.
    /// </summary>
    private readonly AssertionConditionType myConditionType;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertionConditionAttribute"/> class. 
    /// Initializes new instance of AssertionConditionAttribute
    /// </summary>
    /// <param name="conditionType">
    /// Specifies condition type
    /// </param>
    public AssertionConditionAttribute(AssertionConditionType conditionType)
    {
      this.myConditionType = conditionType;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets condition type
    /// </summary>
    public AssertionConditionType ConditionType
    {
      get
      {
        return this.myConditionType;
      }
    }

    #endregion
  }

  /// <summary>
  /// Specifies assertion type. If the assertion method argument satisifes the condition, then the execution continues. 
  ///   Otherwise, execution is assumed to be halted
  /// </summary>
  public enum AssertionConditionType
  {
    /// <summary>
    ///   Indicates that the marked parameter should be evaluated to true
    /// </summary>
    IS_TRUE = 0, 

    /// <summary>
    ///   Indicates that the marked parameter should be evaluated to false
    /// </summary>
    IS_FALSE = 1, 

    /// <summary>
    ///   Indicates that the marked parameter should be evaluated to null value
    /// </summary>
    IS_NULL = 2, 

    /// <summary>
    ///   Indicates that the marked parameter should be evaluated to not null value
    /// </summary>
    IS_NOT_NULL = 3, 
  }

  /// <summary>
  /// Indicates that the marked method unconditionally terminates control flow execution.
  ///   For example, it could unconditionally throw exception
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class TerminatesProgramAttribute : Attribute
  {
  }

  /// <summary>
  /// Indicates that the value of marked element could be <c>null</c> sometimes, so the check for <c>null</c> is necessary before its usage
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
    AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public sealed class CanBeNullAttribute : Attribute
  {
  }

  /// <summary>
  /// Indicates that the value of marked element could never be <c>null</c>
  /// </summary>
  [AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Delegate |
    AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public sealed class NotNullAttribute : Attribute
  {
  }

  /// <summary>
  /// Indicates that the value of marked type (or its derivatives) cannot be compared using '==' or '!=' operators.
  ///   There is only exception to compare with <c>null</c>, it is permitted
  /// </summary>
  [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, 
    Inherited = true)]
  public sealed class CannotApplyEqualityOperatorAttribute : Attribute
  {
  }

  /// <summary>
  /// When applied to target attribute, specifies a requirement for any type which is marked with 
  ///   target attribute to implement or inherit specific type or types
  /// </summary>
  /// <example>
  /// <code>
  /// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
  ///     public class ComponentAttribute : Attribute 
  ///     {}
  /// 
  ///     [Component] // ComponentAttribute requires implementing IComponent interface
  ///     public class MyComponent : IComponent
  ///     {}
  ///   </code>
  /// </example>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  [BaseTypeRequired(typeof(Attribute))]
  public sealed class BaseTypeRequiredAttribute : Attribute
  {
    #region Constants and Fields

    /// <summary>
    /// The my base types.
    /// </summary>
    private readonly Type[] myBaseTypes;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTypeRequiredAttribute"/> class. 
    /// Initializes new instance of BaseTypeRequiredAttribute
    /// </summary>
    /// <param name="baseTypes">
    /// Specifies which types are required
    /// </param>
    public BaseTypeRequiredAttribute([NotNull] params Type[] baseTypes)
    {
      this.myBaseTypes = baseTypes;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets enumerations of specified base types
    /// </summary>
    public IEnumerable<Type> BaseTypes
    {
      get
      {
        return this.myBaseTypes;
      }
    }

    #endregion
  }

  /// <summary>
  /// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library),
  ///   so this symbol will not be marked as unused (as well as by other usage inspections)
  /// </summary>
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public sealed class UsedImplicitlyAttribute : Attribute
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    [UsedImplicitly]
    public UsedImplicitlyAttribute()
      : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    /// <param name="useKindFlags">
    /// The use kind flags.
    /// </param>
    /// <param name="targetFlags">
    /// The target flags.
    /// </param>
    [UsedImplicitly]
    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      this.UseKindFlags = useKindFlags;
      this.TargetFlags = targetFlags;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    /// <param name="useKindFlags">
    /// The use kind flags.
    /// </param>
    [UsedImplicitly]
    public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
      : this(useKindFlags, ImplicitUseTargetFlags.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
    /// </summary>
    /// <param name="targetFlags">
    /// The target flags.
    /// </param>
    [UsedImplicitly]
    public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
      : this(ImplicitUseKindFlags.Default, targetFlags)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets value indicating what is meant to be used
    /// </summary>
    [UsedImplicitly]
    public ImplicitUseTargetFlags TargetFlags { get; private set; }

    /// <summary>
    /// Gets UseKindFlags.
    /// </summary>
    [UsedImplicitly]
    public ImplicitUseKindFlags UseKindFlags { get; private set; }

    #endregion
  }

  /// <summary>
  /// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes as unused (as well as by other usage inspections)
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class MeansImplicitUseAttribute : Attribute
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
    /// </summary>
    [UsedImplicitly]
    public MeansImplicitUseAttribute()
      : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
    /// </summary>
    /// <param name="useKindFlags">
    /// The use kind flags.
    /// </param>
    /// <param name="targetFlags">
    /// The target flags.
    /// </param>
    [UsedImplicitly]
    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
    {
      this.UseKindFlags = useKindFlags;
      this.TargetFlags = targetFlags;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
    /// </summary>
    /// <param name="useKindFlags">
    /// The use kind flags.
    /// </param>
    [UsedImplicitly]
    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
      : this(useKindFlags, ImplicitUseTargetFlags.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
    /// </summary>
    /// <param name="targetFlags">
    /// The target flags.
    /// </param>
    [UsedImplicitly]
    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
      : this(ImplicitUseKindFlags.Default, targetFlags)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets value indicating what is meant to be used
    /// </summary>
    [UsedImplicitly]
    public ImplicitUseTargetFlags TargetFlags { get; private set; }

    /// <summary>
    /// Gets UseKindFlags.
    /// </summary>
    [UsedImplicitly]
    public ImplicitUseKindFlags UseKindFlags { get; private set; }

    #endregion
  }

  /// <summary>
  /// The implicit use kind flags.
  /// </summary>
  [Flags]
  public enum ImplicitUseKindFlags
  {
    /// <summary>
    /// The default.
    /// </summary>
    Default = Access | Assign | Instantiated, 

    /// <summary>
    ///   Only entity marked with attribute considered used
    /// </summary>
    Access = 1, 

    /// <summary>
    ///   Indicates implicit assignment to a member
    /// </summary>
    Assign = 2, 

    /// <summary>
    ///   Indicates implicit instantiation of a type
    /// </summary>
    Instantiated = 4, 
  }

  /// <summary>
  /// Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>
  /// </summary>
  [Flags]
  public enum ImplicitUseTargetFlags
  {
    /// <summary>
    /// The default.
    /// </summary>
    Default = Itself, 

    /// <summary>
    /// The itself.
    /// </summary>
    Itself = 1, 

    /// <summary>
    ///   Members of entity marked with attribute are considered used
    /// </summary>
    Members = 2, 

    /// <summary>
    ///   Entity marked with attribute and all its members considered used
    /// </summary>
    WithMembers = Itself | Members
  }
}