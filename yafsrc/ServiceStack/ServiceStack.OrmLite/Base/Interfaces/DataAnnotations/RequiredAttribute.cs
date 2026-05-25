// ***********************************************************************
// <copyright file="RequiredAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************


using System;

namespace ServiceStack.DataAnnotations;

/// <summary>
/// Create NOT NULL Columns in Data Models.
/// Use [ValidateNotNull] to use https://docs.servicestack.net/validation to enforce a not null property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute;