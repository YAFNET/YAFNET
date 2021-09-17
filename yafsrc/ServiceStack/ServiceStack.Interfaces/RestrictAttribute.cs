// ***********************************************************************
// <copyright file="RestrictAttribute.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceStack
{
    /// <summary>
    /// Decorate on Request DTO's to alter the accessibility of a service and its visibility on /metadata pages
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RestrictAttribute : AttributeBase
    {
        /// <summary>
        /// Allow access but hide from metadata to requests from Localhost only
        /// </summary>
        /// <value><c>true</c> if [visible internal only]; otherwise, <c>false</c>.</value>
        /// <exception cref="System.Exception">Only true allowed</exception>
        public bool VisibleInternalOnly
        {
            get => CanShowTo(RequestAttributes.InternalNetworkAccess);
            set
            {
                if (value == false)
                    throw new Exception("Only true allowed");

                VisibilityTo = RequestAttributes.InternalNetworkAccess.ToAllowedFlagsSet();
            }
        }

        /// <summary>
        /// Allow access but hide from metadata to requests from Localhost and Local Intranet only
        /// </summary>
        /// <value><c>true</c> if [visible localhost only]; otherwise, <c>false</c>.</value>
        /// <exception cref="System.Exception">Only true allowed</exception>
        public bool VisibleLocalhostOnly
        {
            get => CanShowTo(RequestAttributes.Localhost);
            set
            {
                if (value == false)
                    throw new Exception("Only true allowed");

                VisibilityTo = RequestAttributes.Localhost.ToAllowedFlagsSet();
            }
        }

        /// <summary>
        /// Restrict access and hide from metadata to requests from Localhost only
        /// </summary>
        /// <value><c>true</c> if [localhost only]; otherwise, <c>false</c>.</value>
        /// <exception cref="System.Exception">Only true allowed</exception>
        public bool LocalhostOnly
        {
            get => HasAccessTo(RequestAttributes.Localhost) && CanShowTo(RequestAttributes.Localhost);
            set
            {
                if (value == false)
                    throw new Exception("Only true allowed");

                AccessTo = RequestAttributes.Localhost.ToAllowedFlagsSet();
                VisibilityTo = RequestAttributes.Localhost.ToAllowedFlagsSet();
            }
        }

        /// <summary>
        /// Restrict access and hide from metadata to requests from Localhost and Local Intranet only
        /// </summary>
        /// <value><c>true</c> if [internal only]; otherwise, <c>false</c>.</value>
        /// <exception cref="System.Exception">Only true allowed</exception>
        public bool InternalOnly
        {
            get => HasAccessTo(RequestAttributes.InternalNetworkAccess) && CanShowTo(RequestAttributes.InternalNetworkAccess);
            set
            {
                if (value == false)
                    throw new Exception("Only true allowed");

                AccessTo = RequestAttributes.InternalNetworkAccess.ToAllowedFlagsSet();
                VisibilityTo = RequestAttributes.InternalNetworkAccess.ToAllowedFlagsSet();
            }
        }

        /// <summary>
        /// Restrict access and hide from metadata to requests from External only
        /// </summary>
        /// <value><c>true</c> if [external only]; otherwise, <c>false</c>.</value>
        /// <exception cref="System.Exception">Only true allowed</exception>
        public bool ExternalOnly
        {
            get => HasAccessTo(RequestAttributes.External) && CanShowTo(RequestAttributes.External);
            set
            {
                if (value == false)
                    throw new Exception("Only true allowed");

                AccessTo = RequestAttributes.External.ToAllowedFlagsSet();
                VisibilityTo = RequestAttributes.External.ToAllowedFlagsSet();
            }
        }

        /// <summary>
        /// Sets a single access restriction
        /// </summary>
        /// <value>Restrict Access to.</value>
        public RequestAttributes AccessTo
        {
            get => this.AccessibleToAny.Length == 0
                ? RequestAttributes.Any
                : this.AccessibleToAny[0];

            set => this.AccessibleToAny = new[] { value };
        }

        /// <summary>
        /// Restrict access to any of the specified access scenarios
        /// </summary>
        /// <value>Access restrictions</value>
        public RequestAttributes[] AccessibleToAny { get; private set; }

        /// <summary>
        /// Sets a single metadata Visibility restriction
        /// </summary>
        /// <value>Restrict metadata Visibility to.</value>
        public RequestAttributes VisibilityTo
        {
            get => this.VisibleToAny.Length == 0
                ? RequestAttributes.Any
                : this.VisibleToAny[0];

            set => this.VisibleToAny = new[] { value };
        }

        /// <summary>
        /// Restrict metadata visibility to any of the specified access scenarios
        /// </summary>
        /// <value>Visibility restrictions</value>
        public RequestAttributes[] VisibleToAny { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictAttribute"/> class.
        /// </summary>
        public RestrictAttribute()
        {
            this.AccessTo = RequestAttributes.Any;
            this.VisibilityTo = RequestAttributes.Any;
        }

        /// <summary>
        /// Restrict access and metadata visibility to any of the specified access scenarios
        /// </summary>
        /// <param name="restrictAccessAndVisibilityToScenarios">The restrict access and visibility to scenarios.</param>
        /// <value>The restrict access to scenarios.</value>
        public RestrictAttribute(params RequestAttributes[] restrictAccessAndVisibilityToScenarios)
        {
            this.AccessibleToAny = ToAllowedFlagsSet(restrictAccessAndVisibilityToScenarios);
            this.VisibleToAny = ToAllowedFlagsSet(restrictAccessAndVisibilityToScenarios);
        }

        /// <summary>
        /// Restrict access and metadata visibility to any of the specified access scenarios
        /// </summary>
        /// <param name="allowedAccessScenarios">The allowed access scenarios.</param>
        /// <param name="visibleToScenarios">The visible to scenarios.</param>
        /// <value>The restrict access to scenarios.</value>
        public RestrictAttribute(RequestAttributes[] allowedAccessScenarios, RequestAttributes[] visibleToScenarios)
            : this()
        {
            this.AccessibleToAny = ToAllowedFlagsSet(allowedAccessScenarios);
            this.VisibleToAny = ToAllowedFlagsSet(visibleToScenarios);
        }

        /// <summary>
        /// Returns the allowed set of scenarios based on the user-specified restrictions
        /// </summary>
        /// <param name="restrictToAny">The restrict to any.</param>
        /// <returns>RequestAttributes[].</returns>
        private static RequestAttributes[] ToAllowedFlagsSet(RequestAttributes[] restrictToAny)
        {
            if (restrictToAny.Length == 0)
                return new[] { RequestAttributes.Any };

            var scenarios = new List<RequestAttributes>();
            foreach (var restrictToScenario in restrictToAny)
            {
                var restrictTo = restrictToScenario.ToAllowedFlagsSet();

                scenarios.Add(restrictTo);
            }

            return scenarios.ToArray();
        }

        /// <summary>
        /// Determines whether this instance [can show to] the specified restrictions.
        /// </summary>
        /// <param name="restrictions">The restrictions.</param>
        /// <returns><c>true</c> if this instance [can show to] the specified restrictions; otherwise, <c>false</c>.</returns>
        public bool CanShowTo(RequestAttributes restrictions)
        {
            return this.VisibleToAny.Any(scenario => (restrictions & scenario) == restrictions);
        }

        /// <summary>
        /// Determines whether [has access to] [the specified restrictions].
        /// </summary>
        /// <param name="restrictions">The restrictions.</param>
        /// <returns><c>true</c> if [has access to] [the specified restrictions]; otherwise, <c>false</c>.</returns>
        public bool HasAccessTo(RequestAttributes restrictions)
        {
            return this.AccessibleToAny.Any(scenario => (restrictions & scenario) == restrictions);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has no access restrictions.
        /// </summary>
        /// <value><c>true</c> if this instance has no access restrictions; otherwise, <c>false</c>.</value>
        public bool HasNoAccessRestrictions => this.AccessTo == RequestAttributes.Any;

        /// <summary>
        /// Gets a value indicating whether this instance has no visibility restrictions.
        /// </summary>
        /// <value><c>true</c> if this instance has no visibility restrictions; otherwise, <c>false</c>.</value>
        public bool HasNoVisibilityRestrictions => this.VisibilityTo == RequestAttributes.Any;
    }

    /// <summary>
    /// Class RestrictExtensions.
    /// </summary>
    public static class RestrictExtensions
    {
        /// <summary>
        /// Converts from a User intended restriction to a flag with all the allowed attribute flags set, e.g:
        /// If No Network restrictions were specified all Network access types are allowed, e.g:
        /// restrict EndpointAttributes.None =&gt; ... 111
        /// If a Network restriction was specified, only it will be allowed, e.g:
        /// restrict EndpointAttributes.LocalSubnet =&gt; ... 010
        /// The returned Enum will have a flag with all the allowed attributes set
        /// </summary>
        /// <param name="restrictTo">The restrict to.</param>
        /// <returns>RequestAttributes.</returns>
        public static RequestAttributes ToAllowedFlagsSet(this RequestAttributes restrictTo)
        {
            if (restrictTo == RequestAttributes.Any)
                return RequestAttributes.Any;

            var allowedAttrs = RequestAttributes.None;

            //Network access
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnyNetworkAccessType))
                allowedAttrs |= RequestAttributes.AnyNetworkAccessType;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnyNetworkAccessType;

            //Security
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnySecurityMode))
                allowedAttrs |= RequestAttributes.AnySecurityMode;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnySecurityMode;

            //Http Method
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnyHttpMethod))
                allowedAttrs |= RequestAttributes.AnyHttpMethod;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnyHttpMethod;

            //Call Style
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnyCallStyle))
                allowedAttrs |= RequestAttributes.AnyCallStyle;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnyCallStyle;

            //Format
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnyFormat))
                allowedAttrs |= RequestAttributes.AnyFormat;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnyFormat;

            //Endpoint
            if (!HasAnyRestrictionsOf(restrictTo, RequestAttributes.AnyEndpoint))
                allowedAttrs |= RequestAttributes.AnyEndpoint;
            else
                allowedAttrs |= restrictTo & RequestAttributes.AnyEndpoint;

            return allowedAttrs;
        }

        /// <summary>
        /// Determines whether [has any restrictions of] [the specified all restrictions].
        /// </summary>
        /// <param name="allRestrictions">All restrictions.</param>
        /// <param name="restrictions">The restrictions.</param>
        /// <returns><c>true</c> if [has any restrictions of] [the specified all restrictions]; otherwise, <c>false</c>.</returns>
        public static bool HasAnyRestrictionsOf(RequestAttributes allRestrictions, RequestAttributes restrictions)
        {
            return (allRestrictions & restrictions) != 0;
        }
    }
}