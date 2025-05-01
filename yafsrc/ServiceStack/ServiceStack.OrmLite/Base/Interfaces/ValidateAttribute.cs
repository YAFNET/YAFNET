// ***********************************************************************
// <copyright file="ValidateAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

namespace ServiceStack;

using System;
using System.Text;

using ServiceStack.DataAnnotations;

/// <summary>
/// Assert pre-conditions before DTO's Fluent Validation properties are evaluated
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[Tag("PropertyOrder")]
public class ValidateRequestAttribute : AttributeBase, IValidateRule, IReflectAttributeConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateRequestAttribute" /> class.
    /// </summary>
    public ValidateRequestAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateRequestAttribute" /> class.
    /// </summary>
    /// <param name="validator">The validator.</param>
    public ValidateRequestAttribute(string validator)
    {
        this.Validator = validator;
    }

    /// <summary>
    /// Script Expression to create an IPropertyValidator registered in Validators.Types
    /// </summary>
    /// <value>The validator.</value>
    public string Validator { get; set; }

    /// <summary>
    /// Boolean #Script Code Expression to Test
    /// ARGS:
    /// - Request: IRequest
    /// -     dto: Request DTO
    /// -      it: Request DTO
    /// </summary>
    /// <value>The condition.</value>
    public string Condition { get; set; }

    /// <summary>
    /// Combine multiple conditions
    /// </summary>
    /// <value>The conditions.</value>
    [Ignore]
    public string[] Conditions
    {
        get => [this.Condition];
        set => this.Condition = ValidateAttribute.Combine("&&", value);
    }

    /// <summary>
    /// Custom ErrorCode to return
    /// </summary>
    /// <value>The error code.</value>
    public string ErrorCode { get; set; }

    /// <summary>
    /// Custom Error Message to return
    /// - {PropertyName}
    /// - {PropertyValue}
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Custom Status Code to return when invalid
    /// </summary>
    /// <value>The status code.</value>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets all conditions.
    /// </summary>
    /// <value>All conditions.</value>
    /// <exception cref="System.NotSupportedException">AllConditions</exception>
    [Ignore]
    public string[] AllConditions
    {
        get => throw new NotSupportedException(nameof(this.AllConditions));
        set => this.Condition = ValidateAttribute.Combine("&&", value);
    }

    /// <summary>
    /// Gets or sets any conditions.
    /// </summary>
    /// <value>Any conditions.</value>
    /// <exception cref="System.NotSupportedException">AnyConditions</exception>
    [Ignore]
    public string[] AnyConditions
    {
        get => throw new NotSupportedException(nameof(this.AnyConditions));
        set => this.Condition = ValidateAttribute.Combine("||", value);
    }
}
//Default ITypeValidator defined in ValidateScripts
/// <summary>
/// Class ValidateIsAuthenticatedAttribute.
/// Implements the <see cref="ServiceStack.ValidateRequestAttribute" />
/// </summary>
/// <seealso cref="ServiceStack.ValidateRequestAttribute" />
public class ValidateIsAuthenticatedAttribute : ValidateRequestAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateIsAuthenticatedAttribute" /> class.
    /// </summary>
    public ValidateIsAuthenticatedAttribute() : base("IsAuthenticated") { }
}
/// <summary>
/// Class ValidateIsAdminAttribute.
/// Implements the <see cref="ServiceStack.ValidateRequestAttribute" />
/// </summary>
/// <seealso cref="ServiceStack.ValidateRequestAttribute" />
public class ValidateIsAdminAttribute : ValidateRequestAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateIsAdminAttribute" /> class.
    /// </summary>
    public ValidateIsAdminAttribute() : base("IsAdmin") { }
}
/// <summary>
/// Class ValidateHasRoleAttribute.
/// Implements the <see cref="ServiceStack.ValidateRequestAttribute" />
/// </summary>
/// <seealso cref="ServiceStack.ValidateRequestAttribute" />
public class ValidateHasRoleAttribute : ValidateRequestAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateHasRoleAttribute" /> class.
    /// </summary>
    /// <param name="role">The role.</param>
    public ValidateHasRoleAttribute(string role) : base("HasRole(`" + role + "`)") { }
}
/// <summary>
/// Class ValidateHasPermissionAttribute.
/// Implements the <see cref="ServiceStack.ValidateRequestAttribute" />
/// </summary>
/// <seealso cref="ServiceStack.ValidateRequestAttribute" />
public class ValidateHasPermissionAttribute : ValidateRequestAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateHasPermissionAttribute" /> class.
    /// </summary>
    /// <param name="permission">The permission.</param>
    public ValidateHasPermissionAttribute(string permission) : base("HasPermission(`" + permission + "`)") { }
}

/// <summary>
/// Class ValidateAttribute.
/// Implements the <see cref="ServiceStack.AttributeBase" />
/// Implements the <see cref="ServiceStack.IValidateRule" />
/// Implements the <see cref="ServiceStack.IReflectAttributeConverter" />
/// </summary>
/// <seealso cref="ServiceStack.AttributeBase" />
/// <seealso cref="ServiceStack.IValidateRule" />
/// <seealso cref="ServiceStack.IReflectAttributeConverter" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class ValidateAttribute : AttributeBase, IValidateRule, IReflectAttributeConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateAttribute" /> class.
    /// </summary>
    public ValidateAttribute() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateAttribute" /> class.
    /// </summary>
    /// <param name="validator">The validator.</param>
    public ValidateAttribute(string validator)
    {
        this.Validator = validator;
    }

    /// <summary>
    /// Script Expression to create an IPropertyValidator registered in Validators.Types
    /// </summary>
    /// <value>The validator.</value>
    public string Validator { get; set; }

    /// <summary>
    /// Boolean #Script Code Expression to Test
    /// ARGS:
    /// - Request: IRequest
    /// -     dto: Request DTO
    /// -   field: Property Name
    /// -      it: Property Value
    /// </summary>
    /// <value>The condition.</value>
    public string Condition { get; set; }

    /// <summary>
    /// Gets or sets all conditions.
    /// </summary>
    /// <value>All conditions.</value>
    /// <exception cref="System.NotSupportedException">AllConditions</exception>
    [Ignore]
    public string[] AllConditions
    {
        get => throw new NotSupportedException(nameof(this.AllConditions));
        set => this.Condition = Combine("&&", value);
    }

    /// <summary>
    /// Gets or sets any conditions.
    /// </summary>
    /// <value>Any conditions.</value>
    /// <exception cref="System.NotSupportedException">AnyConditions</exception>
    [Ignore]
    public string[] AnyConditions
    {
        get => throw new NotSupportedException(nameof(this.AnyConditions));
        set => this.Condition = Combine("||", value);
    }

    /// <summary>
    /// Custom ErrorCode to return
    /// </summary>
    /// <value>The error code.</value>
    public string ErrorCode { get; set; }

    /// <summary>
    /// Refer to FluentValidation docs for Variable
    /// - {PropertyName}
    /// - {PropertyValue}
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// Combines the specified comperand.
    /// </summary>
    /// <param name="comperand">The comperand.</param>
    /// <param name="conditions">The conditions.</param>
    /// <returns>System.String.</returns>
    public static string Combine(string comperand, params string[] conditions)
    {
        var sb = new StringBuilder();
        var joiner = ") " + comperand + " (";
        foreach (var condition in conditions)
        {
            if (string.IsNullOrEmpty(condition))
            {
                continue;
            }

            if (sb.Length > 0)
            {
                sb.Append(joiner);
            }

            sb.Append(condition);
        }

        sb.Insert(0, '(');
        sb.Append(')');
        return sb.ToString();
    }
}

/// <summary>
/// Interface IValidateRule
/// </summary>
public interface IValidateRule
{
    /// <summary>
    /// Gets or sets the validator.
    /// </summary>
    /// <value>The validator.</value>
    string Validator { get; set; }
    /// <summary>
    /// Gets or sets the condition.
    /// </summary>
    /// <value>The condition.</value>
    string Condition { get; set; }
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>The error code.</value>
    string ErrorCode { get; set; }
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    string Message { get; set; }
}

/// <summary>
/// Class ValidateRule.
/// Implements the <see cref="ServiceStack.IValidateRule" />
/// </summary>
/// <seealso cref="ServiceStack.IValidateRule" />
public class ValidateRule : IValidateRule
{
    /// <summary>
    /// Gets or sets the validator.
    /// </summary>
    /// <value>The validator.</value>
    public string Validator { get; set; }
    /// <summary>
    /// Gets or sets the condition.
    /// </summary>
    /// <value>The condition.</value>
    public string Condition { get; set; }
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>The error code.</value>
    public string ErrorCode { get; set; }
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }
}

/// <summary>
/// Data persistence Model
/// </summary>
public class ValidationRule : ValidateRule
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    [AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// The name of the Type
    /// </summary>
    /// <value>The type.</value>
    [Required]
    public string Type { get; set; }

    /// <summary>
    /// The property field for Property Validators, null for Type Validators
    /// </summary>
    /// <value>The field.</value>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the created by.
    /// </summary>
    /// <value>The created by.</value>
    public string CreatedBy { get; set; }
    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    /// <value>The created date.</value>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the modified by.
    /// </summary>
    /// <value>The modified by.</value>
    public string ModifiedBy { get; set; }
    /// <summary>
    /// Gets or sets the modified date.
    /// </summary>
    /// <value>The modified date.</value>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// Gets or sets the suspended by.
    /// </summary>
    /// <value>The suspended by.</value>
    public string SuspendedBy { get; set; }
    /// <summary>
    /// Gets or sets the suspended date.
    /// </summary>
    /// <value>The suspended date.</value>
    [Index]
    public DateTime? SuspendedDate { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    /// <value>The notes.</value>
    public string Notes { get; set; }

    /// <summary>
    /// Equalses the specified other.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    protected bool Equals(ValidationRule other)
    {
        return this.Id == other.Id &&
               this.Type == other.Type && this.Field == other.Field &&
               this.CreatedBy == other.CreatedBy && Nullable.Equals(this.CreatedDate, other.CreatedDate) &&
               this.ModifiedBy == other.ModifiedBy && Nullable.Equals(this.ModifiedDate, other.ModifiedDate) &&
               this.SuspendedBy == other.SuspendedBy && Nullable.Equals(this.SuspendedDate, other.SuspendedDate) &&
               this.Notes == other.Notes;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == this.GetType() && this.Equals((ValidationRule)obj);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = this.Id;
            hashCode = (hashCode * 397) ^ (this.Type != null ? this.Type.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (this.Field != null ? this.Field.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (this.CreatedBy != null ? this.CreatedBy.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ this.CreatedDate.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.ModifiedBy != null ? this.ModifiedBy.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ this.ModifiedDate.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.SuspendedBy != null ? this.SuspendedBy.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ this.SuspendedDate.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.Notes != null ? this.Notes.GetHashCode() : 0);
            return hashCode;
        }
    }
}