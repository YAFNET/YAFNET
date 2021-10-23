// ***********************************************************************
// <copyright file="MetadataTypes.cs" company="ServiceStack, Inc.">
//     Copyright (c) ServiceStack, Inc. All Rights Reserved.
// </copyright>
// <summary>Fork for YetAnotherForum.NET, Licensed under the Apache License, Version 2.0</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ServiceStack.Script;

namespace ServiceStack
{
    /// <summary>
    /// Class MetadataTypesConfig.
    /// </summary>
    public class MetadataTypesConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataTypesConfig"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="makePartial">if set to <c>true</c> [make partial].</param>
        /// <param name="makeVirtual">if set to <c>true</c> [make virtual].</param>
        /// <param name="addReturnMarker">if set to <c>true</c> [add return marker].</param>
        /// <param name="convertDescriptionToComments">if set to <c>true</c> [convert description to comments].</param>
        /// <param name="addDataContractAttributes">if set to <c>true</c> [add data contract attributes].</param>
        /// <param name="addIndexesToDataMembers">if set to <c>true</c> [add indexes to data members].</param>
        /// <param name="addGeneratedCodeAttributes">if set to <c>true</c> [add generated code attributes].</param>
        /// <param name="addDefaultXmlNamespace">The add default XML namespace.</param>
        /// <param name="baseClass">The base class.</param>
        /// <param name="package">The package.</param>
        /// <param name="addResponseStatus">if set to <c>true</c> [add response status].</param>
        /// <param name="addServiceStackTypes">if set to <c>true</c> [add service stack types].</param>
        /// <param name="addModelExtensions">if set to <c>true</c> [add model extensions].</param>
        /// <param name="addPropertyAccessors">if set to <c>true</c> [add property accessors].</param>
        /// <param name="excludeGenericBaseTypes">if set to <c>true</c> [exclude generic base types].</param>
        /// <param name="settersReturnThis">if set to <c>true</c> [setters return this].</param>
        /// <param name="makePropertiesOptional">if set to <c>true</c> [make properties optional].</param>
        /// <param name="makeDataContractsExtensible">if set to <c>true</c> [make data contracts extensible].</param>
        /// <param name="initializeCollections">if set to <c>true</c> [initialize collections].</param>
        /// <param name="addImplicitVersion">The add implicit version.</param>
        public MetadataTypesConfig(
            string baseUrl = null,
            bool makePartial = true,
            bool makeVirtual = true,
            bool addReturnMarker = true,
            bool convertDescriptionToComments = true,
            bool addDataContractAttributes = false,
            bool addIndexesToDataMembers = false,
            bool addGeneratedCodeAttributes = false,
            string addDefaultXmlNamespace = null,
            string baseClass = null,
            string package = null,
            bool addResponseStatus = false,
            bool addServiceStackTypes = true,
            bool addModelExtensions = true,
            bool addPropertyAccessors = true,
            bool excludeGenericBaseTypes = false,
            bool settersReturnThis = true,
            bool makePropertiesOptional = true,
            bool makeDataContractsExtensible = false,
            bool initializeCollections = true,
            int? addImplicitVersion = null)
        {
            BaseUrl = baseUrl;
            MakePartial = makePartial;
            MakeVirtual = makeVirtual;
            AddReturnMarker = addReturnMarker;
            AddDescriptionAsComments = convertDescriptionToComments;
            AddDataContractAttributes = addDataContractAttributes;
            AddDefaultXmlNamespace = addDefaultXmlNamespace;
            BaseClass = baseClass;
            Package = package;
            MakeDataContractsExtensible = makeDataContractsExtensible;
            AddIndexesToDataMembers = addIndexesToDataMembers;
            AddGeneratedCodeAttributes = addGeneratedCodeAttributes;
            InitializeCollections = initializeCollections;
            AddResponseStatus = addResponseStatus;
            AddServiceStackTypes = addServiceStackTypes;
            AddModelExtensions = addModelExtensions;
            AddPropertyAccessors = addPropertyAccessors;
            ExcludeGenericBaseTypes = excludeGenericBaseTypes;
            SettersReturnThis = settersReturnThis;
            MakePropertiesOptional = makePropertiesOptional;
            AddImplicitVersion = addImplicitVersion;
        }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>The base URL.</value>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Gets or sets the use path.
        /// </summary>
        /// <value>The use path.</value>
        public string UsePath { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [make partial].
        /// </summary>
        /// <value><c>true</c> if [make partial]; otherwise, <c>false</c>.</value>
        public bool MakePartial { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [make virtual].
        /// </summary>
        /// <value><c>true</c> if [make virtual]; otherwise, <c>false</c>.</value>
        public bool MakeVirtual { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [make internal].
        /// </summary>
        /// <value><c>true</c> if [make internal]; otherwise, <c>false</c>.</value>
        public bool MakeInternal { get; set; }
        /// <summary>
        /// Gets or sets the base class.
        /// </summary>
        /// <value>The base class.</value>
        public string BaseClass { get; set; }
        /// <summary>
        /// Gets or sets the package.
        /// </summary>
        /// <value>The package.</value>
        public string Package { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add return marker].
        /// </summary>
        /// <value><c>true</c> if [add return marker]; otherwise, <c>false</c>.</value>
        public bool AddReturnMarker { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add description as comments].
        /// </summary>
        /// <value><c>true</c> if [add description as comments]; otherwise, <c>false</c>.</value>
        public bool AddDescriptionAsComments { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add data contract attributes].
        /// </summary>
        /// <value><c>true</c> if [add data contract attributes]; otherwise, <c>false</c>.</value>
        public bool AddDataContractAttributes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add indexes to data members].
        /// </summary>
        /// <value><c>true</c> if [add indexes to data members]; otherwise, <c>false</c>.</value>
        public bool AddIndexesToDataMembers { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add generated code attributes].
        /// </summary>
        /// <value><c>true</c> if [add generated code attributes]; otherwise, <c>false</c>.</value>
        public bool AddGeneratedCodeAttributes { get; set; }
        /// <summary>
        /// Gets or sets the add implicit version.
        /// </summary>
        /// <value>The add implicit version.</value>
        public int? AddImplicitVersion { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add response status].
        /// </summary>
        /// <value><c>true</c> if [add response status]; otherwise, <c>false</c>.</value>
        public bool AddResponseStatus { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add service stack types].
        /// </summary>
        /// <value><c>true</c> if [add service stack types]; otherwise, <c>false</c>.</value>
        public bool AddServiceStackTypes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add model extensions].
        /// </summary>
        /// <value><c>true</c> if [add model extensions]; otherwise, <c>false</c>.</value>
        public bool AddModelExtensions { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [add property accessors].
        /// </summary>
        /// <value><c>true</c> if [add property accessors]; otherwise, <c>false</c>.</value>
        public bool AddPropertyAccessors { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [exclude generic base types].
        /// </summary>
        /// <value><c>true</c> if [exclude generic base types]; otherwise, <c>false</c>.</value>
        public bool ExcludeGenericBaseTypes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [setters return this].
        /// </summary>
        /// <value><c>true</c> if [setters return this]; otherwise, <c>false</c>.</value>
        public bool SettersReturnThis { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [make properties optional].
        /// </summary>
        /// <value><c>true</c> if [make properties optional]; otherwise, <c>false</c>.</value>
        public bool MakePropertiesOptional { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [export as types].
        /// </summary>
        /// <value><c>true</c> if [export as types]; otherwise, <c>false</c>.</value>
        public bool ExportAsTypes { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [exclude implemented interfaces].
        /// </summary>
        /// <value><c>true</c> if [exclude implemented interfaces]; otherwise, <c>false</c>.</value>
        public bool ExcludeImplementedInterfaces { get; set; }
        /// <summary>
        /// Gets or sets the add default XML namespace.
        /// </summary>
        /// <value>The add default XML namespace.</value>
        public string AddDefaultXmlNamespace { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [make data contracts extensible].
        /// </summary>
        /// <value><c>true</c> if [make data contracts extensible]; otherwise, <c>false</c>.</value>
        public bool MakeDataContractsExtensible { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [initialize collections].
        /// </summary>
        /// <value><c>true</c> if [initialize collections]; otherwise, <c>false</c>.</value>
        public bool InitializeCollections { get; set; }
        /// <summary>
        /// Gets or sets the add namespaces.
        /// </summary>
        /// <value>The add namespaces.</value>
        public List<string> AddNamespaces { get; set; }
        /// <summary>
        /// Gets or sets the default namespaces.
        /// </summary>
        /// <value>The default namespaces.</value>
        public List<string> DefaultNamespaces { get; set; }
        /// <summary>
        /// Gets or sets the default imports.
        /// </summary>
        /// <value>The default imports.</value>
        public List<string> DefaultImports { get; set; }
        /// <summary>
        /// Gets or sets the include types.
        /// </summary>
        /// <value>The include types.</value>
        public List<string> IncludeTypes { get; set; }
        /// <summary>
        /// Gets or sets the exclude types.
        /// </summary>
        /// <value>The exclude types.</value>
        public List<string> ExcludeTypes { get; set; }
        /// <summary>
        /// Gets or sets the treat types as strings.
        /// </summary>
        /// <value>The treat types as strings.</value>
        public List<string> TreatTypesAsStrings { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [export value types].
        /// </summary>
        /// <value><c>true</c> if [export value types]; otherwise, <c>false</c>.</value>
        public bool ExportValueTypes { get; set; }

        /// <summary>
        /// Gets or sets the global namespace.
        /// </summary>
        /// <value>The global namespace.</value>
        public string GlobalNamespace { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [exclude namespace].
        /// </summary>
        /// <value><c>true</c> if [exclude namespace]; otherwise, <c>false</c>.</value>
        public bool ExcludeNamespace { get; set; }

        /// <summary>
        /// Gets or sets the ignore types.
        /// </summary>
        /// <value>The ignore types.</value>
        public HashSet<Type> IgnoreTypes { get; set; }
        /// <summary>
        /// Gets or sets the export types.
        /// </summary>
        /// <value>The export types.</value>
        public HashSet<Type> ExportTypes { get; set; }
        /// <summary>
        /// Gets or sets the export attributes.
        /// </summary>
        /// <value>The export attributes.</value>
        public HashSet<Type> ExportAttributes { get; set; }
        /// <summary>
        /// Gets or sets the ignore types in namespaces.
        /// </summary>
        /// <value>The ignore types in namespaces.</value>
        public List<string> IgnoreTypesInNamespaces { get; set; }
    }

    /// <summary>
    /// Class MetadataTypes.
    /// </summary>
    public class MetadataTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataTypes"/> class.
        /// </summary>
        public MetadataTypes()
        {
            Types = new List<MetadataType>();
            Operations = new List<MetadataOperationType>();
            Namespaces = new List<string>();
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public MetadataTypesConfig Config { get; set; }
        /// <summary>
        /// Gets or sets the namespaces.
        /// </summary>
        /// <value>The namespaces.</value>
        public List<string> Namespaces { get; set; }
        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>The types.</value>
        public List<MetadataType> Types { get; set; }
        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>The operations.</value>
        public List<MetadataOperationType> Operations { get; set; }
    }

    /// <summary>
    /// Class AppMetadata.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class AppMetadata : IMeta
    {
        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        public AppInfo App { get; set; }
        /// <summary>
        /// Gets or sets the content type formats.
        /// </summary>
        /// <value>The content type formats.</value>
        public Dictionary<string, string> ContentTypeFormats { get; set; }
        /// <summary>
        /// Gets or sets the plugins.
        /// </summary>
        /// <value>The plugins.</value>
        public PluginInfo Plugins { get; set; }
        /// <summary>
        /// Gets or sets the custom plugins.
        /// </summary>
        /// <value>The custom plugins.</value>
        public Dictionary<string, CustomPlugin> CustomPlugins { get; set; }
        /// <summary>
        /// Gets or sets the API.
        /// </summary>
        /// <value>The API.</value>
        public MetadataTypes Api { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class PluginInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class PluginInfo : IMeta
    {
        /// <summary>
        /// Gets or sets the loaded.
        /// </summary>
        /// <value>The loaded.</value>
        public List<string> Loaded { get; set; }
        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>The authentication.</value>
        public AuthInfo Auth { get; set; }
        /// <summary>
        /// Gets or sets the automatic query.
        /// </summary>
        /// <value>The automatic query.</value>
        public AutoQueryInfo AutoQuery { get; set; }
        /// <summary>
        /// Gets or sets the validation.
        /// </summary>
        /// <value>The validation.</value>
        public ValidationInfo Validation { get; set; }
        /// <summary>
        /// Gets or sets the sharp pages.
        /// </summary>
        /// <value>The sharp pages.</value>
        public SharpPagesInfo SharpPages { get; set; }
        /// <summary>
        /// Gets or sets the request logs.
        /// </summary>
        /// <value>The request logs.</value>
        public RequestLogsInfo RequestLogs { get; set; }
        /// <summary>
        /// Gets or sets the admin users.
        /// </summary>
        /// <value>The admin users.</value>
        public AdminUsersInfo AdminUsers { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class AuthInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class AuthInfo : IMeta
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has authentication secret.
        /// </summary>
        /// <value><c>null</c> if [has authentication secret] contains no value, <c>true</c> if [has authentication secret]; otherwise, <c>false</c>.</value>
        public bool? HasAuthSecret { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has authentication repository.
        /// </summary>
        /// <value><c>null</c> if [has authentication repository] contains no value, <c>true</c> if [has authentication repository]; otherwise, <c>false</c>.</value>
        public bool? HasAuthRepository { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [includes roles].
        /// </summary>
        /// <value><c>null</c> if [includes roles] contains no value, <c>true</c> if [includes roles]; otherwise, <c>false</c>.</value>
        public bool? IncludesRoles { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [includes o authentication tokens].
        /// </summary>
        /// <value><c>null</c> if [includes o authentication tokens] contains no value, <c>true</c> if [includes o authentication tokens]; otherwise, <c>false</c>.</value>
        public bool? IncludesOAuthTokens { get; set; }
        /// <summary>
        /// Gets or sets the HTML redirect.
        /// </summary>
        /// <value>The HTML redirect.</value>
        public string HtmlRedirect { get; set; }
        /// <summary>
        /// Gets or sets the authentication providers.
        /// </summary>
        /// <value>The authentication providers.</value>
        public List<MetaAuthProvider> AuthProviders { get; set; }
        /// <summary>
        /// Gets or sets the service routes.
        /// </summary>
        /// <value>The service routes.</value>
        public Dictionary<string, string[]> ServiceRoutes { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class AutoQueryInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class AutoQueryInfo : IMeta
    {
        /// <summary>
        /// Gets or sets the maximum limit.
        /// </summary>
        /// <value>The maximum limit.</value>
        public int? MaxLimit { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [untyped queries].
        /// </summary>
        /// <value><c>null</c> if [untyped queries] contains no value, <c>true</c> if [untyped queries]; otherwise, <c>false</c>.</value>
        public bool? UntypedQueries { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [raw SQL filters].
        /// </summary>
        /// <value><c>null</c> if [raw SQL filters] contains no value, <c>true</c> if [raw SQL filters]; otherwise, <c>false</c>.</value>
        public bool? RawSqlFilters { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [automatic query viewer].
        /// </summary>
        /// <value><c>null</c> if [automatic query viewer] contains no value, <c>true</c> if [automatic query viewer]; otherwise, <c>false</c>.</value>
        public bool? AutoQueryViewer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AutoQueryInfo"/> is asynchronous.
        /// </summary>
        /// <value><c>null</c> if [asynchronous] contains no value, <c>true</c> if [asynchronous]; otherwise, <c>false</c>.</value>
        public bool? Async { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [order by primary key].
        /// </summary>
        /// <value><c>null</c> if [order by primary key] contains no value, <c>true</c> if [order by primary key]; otherwise, <c>false</c>.</value>
        public bool? OrderByPrimaryKey { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [crud events].
        /// </summary>
        /// <value><c>null</c> if [crud events] contains no value, <c>true</c> if [crud events]; otherwise, <c>false</c>.</value>
        public bool? CrudEvents { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [crud events services].
        /// </summary>
        /// <value><c>null</c> if [crud events services] contains no value, <c>true</c> if [crud events services]; otherwise, <c>false</c>.</value>
        public bool? CrudEventsServices { get; set; }
        /// <summary>
        /// Gets or sets the access role.
        /// </summary>
        /// <value>The access role.</value>
        public string AccessRole { get; set; }
        /// <summary>
        /// Gets or sets the named connection.
        /// </summary>
        /// <value>The named connection.</value>
        public string NamedConnection { get; set; }
        /// <summary>
        /// Gets or sets the viewer conventions.
        /// </summary>
        /// <value>The viewer conventions.</value>
        public List<AutoQueryConvention> ViewerConventions { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class ValidationInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class ValidationInfo : IMeta
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has validation source.
        /// </summary>
        /// <value><c>null</c> if [has validation source] contains no value, <c>true</c> if [has validation source]; otherwise, <c>false</c>.</value>
        public bool? HasValidationSource { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has validation source admin.
        /// </summary>
        /// <value><c>null</c> if [has validation source admin] contains no value, <c>true</c> if [has validation source admin]; otherwise, <c>false</c>.</value>
        public bool? HasValidationSourceAdmin { get; set; }
        /// <summary>
        /// Gets or sets the service routes.
        /// </summary>
        /// <value>The service routes.</value>
        public Dictionary<string, string[]> ServiceRoutes { get; set; }
        /// <summary>
        /// Gets or sets the type validators.
        /// </summary>
        /// <value>The type validators.</value>
        public List<ScriptMethodType> TypeValidators { get; set; }
        /// <summary>
        /// Gets or sets the property validators.
        /// </summary>
        /// <value>The property validators.</value>
        public List<ScriptMethodType> PropertyValidators { get; set; }
        /// <summary>
        /// Gets or sets the access role.
        /// </summary>
        /// <value>The access role.</value>
        public string AccessRole { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class SharpPagesInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class SharpPagesInfo : IMeta
    {
        /// <summary>
        /// Gets or sets the API path.
        /// </summary>
        /// <value>The API path.</value>
        public string ApiPath { get; set; }
        /// <summary>
        /// Gets or sets the script admin role.
        /// </summary>
        /// <value>The script admin role.</value>
        public string ScriptAdminRole { get; set; }
        /// <summary>
        /// Gets or sets the metadata debug admin role.
        /// </summary>
        /// <value>The metadata debug admin role.</value>
        public string MetadataDebugAdminRole { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [metadata debug].
        /// </summary>
        /// <value><c>null</c> if [metadata debug] contains no value, <c>true</c> if [metadata debug]; otherwise, <c>false</c>.</value>
        public bool? MetadataDebug { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [spa fallback].
        /// </summary>
        /// <value><c>null</c> if [spa fallback] contains no value, <c>true</c> if [spa fallback]; otherwise, <c>false</c>.</value>
        public bool? SpaFallback { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class RequestLogsInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class RequestLogsInfo : IMeta
    {
        /// <summary>
        /// Gets or sets the required roles.
        /// </summary>
        /// <value>The required roles.</value>
        public string[] RequiredRoles { get; set; }
        /// <summary>
        /// Gets or sets the request logger.
        /// </summary>
        /// <value>The request logger.</value>
        public string RequestLogger { get; set; }
        /// <summary>
        /// Gets or sets the service routes.
        /// </summary>
        /// <value>The service routes.</value>
        public Dictionary<string, string[]> ServiceRoutes { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class AdminUsersInfo.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class AdminUsersInfo : IMeta
    {
        /// <summary>
        /// Gets or sets the access role.
        /// </summary>
        /// <value>The access role.</value>
        public string AccessRole { get; set; }
        /// <summary>
        /// Gets or sets the enabled.
        /// </summary>
        /// <value>The enabled.</value>
        public List<string> Enabled { get; set; }
        /// <summary>
        /// Gets or sets the user authentication.
        /// </summary>
        /// <value>The user authentication.</value>
        public MetadataType UserAuth { get; set; }
        /// <summary>
        /// Gets or sets the user authentication details.
        /// </summary>
        /// <value>The user authentication details.</value>
        public MetadataType UserAuthDetails { get; set; }
        /// <summary>
        /// Gets or sets all roles.
        /// </summary>
        /// <value>All roles.</value>
        public List<string> AllRoles { get; set; }
        /// <summary>
        /// Gets or sets all permissions.
        /// </summary>
        /// <value>All permissions.</value>
        public List<string> AllPermissions { get; set; }
        /// <summary>
        /// Gets or sets the query user authentication properties.
        /// </summary>
        /// <value>The query user authentication properties.</value>
        public List<string> QueryUserAuthProperties { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Generic template for adding metadata info about custom plugins
    /// </summary>
    public class CustomPlugin : IMeta
    {
        /// <summary>
        /// Which User Roles have access to this Plugins Services. See RoleNames for built-in Roles.
        /// </summary>
        /// <value>The access role.</value>
        public string AccessRole { get; set; }

        /// <summary>
        /// What Services Types (and their user-defined routes) are enabled in this plugin
        /// </summary>
        /// <value>The service routes.</value>
        public Dictionary<string, string[]> ServiceRoutes { get; set; }

        /// <summary>
        /// List of enabled features in this plugin
        /// </summary>
        /// <value>The enabled.</value>
        public List<string> Enabled { get; set; }

        /// <summary>
        /// Additional custom metadata about this plugin
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class ScriptMethodType.
    /// </summary>
    public class ScriptMethodType
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the parameter names.
        /// </summary>
        /// <value>The parameter names.</value>
        public string[] ParamNames { get; set; }
        /// <summary>
        /// Gets or sets the parameter types.
        /// </summary>
        /// <value>The parameter types.</value>
        public string[] ParamTypes { get; set; }
        /// <summary>
        /// Gets or sets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public string ReturnType { get; set; }
    }

    /// <summary>
    /// Class AutoQueryConvention.
    /// </summary>
    public class AutoQueryConvention
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        /// <value>The types.</value>
        public string Types { get; set; }
        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public string ValueType { get; set; }
    }

    /// <summary>
    /// App Info and
    /// </summary>
    public class AppInfo : IMeta
    {
        /// <summary>
        /// The App's BaseUrl
        /// </summary>
        /// <value>The base URL.</value>
        public string BaseUrl { get; set; }

        /// <summary>
        /// The ServiceStack Version
        /// </summary>
        /// <value>The service stack version.</value>
        public string ServiceStackVersion => Text.Env.VersionString;
        /// <summary>
        /// Name of the ServiceStack Instance
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; set; }
        /// <summary>
        /// Textual description of the ServiceStack App (shown in Home Services list)
        /// </summary>
        /// <value>The service description.</value>
        public string ServiceDescription { get; set; }
        /// <summary>
        /// Icon for this ServiceStack App (shown in Home Services list)
        /// </summary>
        /// <value>The service icon URL.</value>
        public string ServiceIconUrl { get; set; }
        /// <summary>
        /// Link to your website users can click to find out more about you
        /// </summary>
        /// <value>The brand URL.</value>
        public string BrandUrl { get; set; }
        /// <summary>
        /// A custom logo or image that users can click on to visit your site
        /// </summary>
        /// <value>The brand image URL.</value>
        public string BrandImageUrl { get; set; }
        /// <summary>
        /// The default color of text
        /// </summary>
        /// <value>The color of the text.</value>
        public string TextColor { get; set; }
        /// <summary>
        /// The default color of links
        /// </summary>
        /// <value>The color of the link.</value>
        public string LinkColor { get; set; }
        /// <summary>
        /// The default background color of each screen
        /// </summary>
        /// <value>The color of the background.</value>
        public string BackgroundColor { get; set; }
        /// <summary>
        /// The default background image of each screen anchored to the bottom left
        /// </summary>
        /// <value>The background image URL.</value>
        public string BackgroundImageUrl { get; set; }
        /// <summary>
        /// The default icon for each of your App's Services
        /// </summary>
        /// <value>The icon URL.</value>
        public string IconUrl { get; set; }

        /// <summary>
        /// The configured JsConfig.TextCase
        /// </summary>
        /// <value>The js text case.</value>
        public string JsTextCase { get; set; }

        /// <summary>
        /// Custom User-Defined Attributes
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class MetaAuthProvider.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class MetaAuthProvider : IMeta
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the nav item.
        /// </summary>
        /// <value>The nav item.</value>
        public NavItem NavItem { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }
    }

    /// <summary>
    /// Class MetadataOperationType.
    /// </summary>
    public class MetadataOperationType
    {
        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public MetadataType Request { get; set; }
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public MetadataType Response { get; set; }
        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public List<string> Actions { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [returns void].
        /// </summary>
        /// <value><c>true</c> if [returns void]; otherwise, <c>false</c>.</value>
        public bool ReturnsVoid { get; set; }
        /// <summary>
        /// Gets or sets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public MetadataTypeName ReturnType { get; set; }
        /// <summary>
        /// Gets or sets the routes.
        /// </summary>
        /// <value>The routes.</value>
        public List<MetadataRoute> Routes { get; set; }
        /// <summary>
        /// Gets or sets the data model.
        /// </summary>
        /// <value>The data model.</value>
        public MetadataTypeName DataModel { get; set; }
        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public MetadataTypeName ViewModel { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [requires authentication].
        /// </summary>
        /// <value><c>true</c> if [requires authentication]; otherwise, <c>false</c>.</value>
        public bool RequiresAuth { get; set; }
        /// <summary>
        /// Gets or sets the required roles.
        /// </summary>
        /// <value>The required roles.</value>
        public List<string> RequiredRoles { get; set; }
        /// <summary>
        /// Gets or sets the requires any role.
        /// </summary>
        /// <value>The requires any role.</value>
        public List<string> RequiresAnyRole { get; set; }
        /// <summary>
        /// Gets or sets the required permissions.
        /// </summary>
        /// <value>The required permissions.</value>
        public List<string> RequiredPermissions { get; set; }
        /// <summary>
        /// Gets or sets the requires any permission.
        /// </summary>
        /// <value>The requires any permission.</value>
        public List<string> RequiresAnyPermission { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public List<string> Tags { get; set; }
    }

    /// <summary>
    /// Class MetadataType.
    /// Implements the <see cref="ServiceStack.IMeta" />
    /// </summary>
    /// <seealso cref="ServiceStack.IMeta" />
    public class MetadataType : IMeta
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [IgnoreDataMember]
        public Type Type { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [IgnoreDataMember]
        public Dictionary<string, object> Items { get; set; }
        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        /// <value>The type of the request.</value>
        [IgnoreDataMember]
        public MetadataOperationType RequestType { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is class.
        /// </summary>
        /// <value><c>true</c> if this instance is class; otherwise, <c>false</c>.</value>
        [IgnoreDataMember]
        public bool IsClass => Type?.IsClass ?? !(IsEnum == true || IsInterface == true);

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }
        /// <summary>
        /// Gets or sets the generic arguments.
        /// </summary>
        /// <value>The generic arguments.</value>
        public string[] GenericArgs { get; set; }
        /// <summary>
        /// Gets or sets the inherits.
        /// </summary>
        /// <value>The inherits.</value>
        public MetadataTypeName Inherits { get; set; }
        /// <summary>
        /// Gets or sets the implements.
        /// </summary>
        /// <value>The implements.</value>
        public MetadataTypeName[] Implements { get; set; }
        /// <summary>
        /// Gets or sets the display type.
        /// </summary>
        /// <value>The display type.</value>
        public string DisplayType { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is nested.
        /// </summary>
        /// <value><c>null</c> if [is nested] contains no value, <c>true</c> if [is nested]; otherwise, <c>false</c>.</value>
        public bool? IsNested { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enum.
        /// </summary>
        /// <value><c>null</c> if [is enum] contains no value, <c>true</c> if [is enum]; otherwise, <c>false</c>.</value>
        public bool? IsEnum { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enum int.
        /// </summary>
        /// <value><c>null</c> if [is enum int] contains no value, <c>true</c> if [is enum int]; otherwise, <c>false</c>.</value>
        public bool? IsEnumInt { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is interface.
        /// </summary>
        /// <value><c>null</c> if [is interface] contains no value, <c>true</c> if [is interface]; otherwise, <c>false</c>.</value>
        public bool? IsInterface { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is abstract.
        /// </summary>
        /// <value><c>null</c> if [is abstract] contains no value, <c>true</c> if [is abstract]; otherwise, <c>false</c>.</value>
        public bool? IsAbstract { get; set; }
        /// <summary>
        /// Gets or sets the data contract.
        /// </summary>
        /// <value>The data contract.</value>
        public MetadataDataContract DataContract { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public List<MetadataPropertyType> Properties { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public List<MetadataAttribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the inner types.
        /// </summary>
        /// <value>The inner types.</value>
        public List<MetadataTypeName> InnerTypes { get; set; }

        /// <summary>
        /// Gets or sets the enum names.
        /// </summary>
        /// <value>The enum names.</value>
        public List<string> EnumNames { get; set; }
        /// <summary>
        /// Gets or sets the enum values.
        /// </summary>
        /// <value>The enum values.</value>
        public List<string> EnumValues { get; set; }
        /// <summary>
        /// Gets or sets the enum member values.
        /// </summary>
        /// <value>The enum member values.</value>
        public List<string> EnumMemberValues { get; set; }
        /// <summary>
        /// Gets or sets the enum descriptions.
        /// </summary>
        /// <value>The enum descriptions.</value>
        public List<string> EnumDescriptions { get; set; }

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public Dictionary<string, string> Meta { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetFullName() => Namespace + "." + Name;

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected bool Equals(MetadataType other)
        {
            return Name == other.Name && Namespace == other.Namespace;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MetadataType)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Namespace != null ? Namespace.GetHashCode() : 0);
            }
        }
    }

    /// <summary>
    /// Class MetadataTypeName.
    /// </summary>
    public class MetadataTypeName
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [IgnoreDataMember]
        public Type Type { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }
        /// <summary>
        /// Gets or sets the generic arguments.
        /// </summary>
        /// <value>The generic arguments.</value>
        public string[] GenericArgs { get; set; }
    }

    /// <summary>
    /// Class MetadataRoute.
    /// </summary>
    public class MetadataRoute
    {
        /// <summary>
        /// Gets or sets the route attribute.
        /// </summary>
        /// <value>The route attribute.</value>
        [IgnoreDataMember]
        public RouteAttribute RouteAttribute { get; set; }
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path { get; set; }
        /// <summary>
        /// Gets or sets the verbs.
        /// </summary>
        /// <value>The verbs.</value>
        public string Verbs { get; set; }
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }
        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; set; }
    }

    /// <summary>
    /// Class MetadataDataContract.
    /// </summary>
    public class MetadataDataContract
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }
    }

    /// <summary>
    /// Class MetadataDataMember.
    /// </summary>
    public class MetadataDataMember
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int? Order { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>null</c> if [is required] contains no value, <c>true</c> if [is required]; otherwise, <c>false</c>.</value>
        public bool? IsRequired { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [emit default value].
        /// </summary>
        /// <value><c>null</c> if [emit default value] contains no value, <c>true</c> if [emit default value]; otherwise, <c>false</c>.</value>
        public bool? EmitDefaultValue { get; set; }
    }

    /// <summary>
    /// Class MetadataPropertyType.
    /// </summary>
    public class MetadataPropertyType
    {
        /// <summary>
        /// Gets or sets the property information.
        /// </summary>
        /// <value>The property information.</value>
        [IgnoreDataMember]
        public PropertyInfo PropertyInfo { get; set; }
        /// <summary>
        /// Gets or sets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        [IgnoreDataMember]
        public Type PropertyType { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [IgnoreDataMember]
        public Dictionary<string, object> Items { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is value type.
        /// </summary>
        /// <value><c>null</c> if [is value type] contains no value, <c>true</c> if [is value type]; otherwise, <c>false</c>.</value>
        public bool? IsValueType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is system type.
        /// </summary>
        /// <value><c>null</c> if [is system type] contains no value, <c>true</c> if [is system type]; otherwise, <c>false</c>.</value>
        public bool? IsSystemType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enum.
        /// </summary>
        /// <value><c>null</c> if [is enum] contains no value, <c>true</c> if [is enum]; otherwise, <c>false</c>.</value>
        public bool? IsEnum { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary key.
        /// </summary>
        /// <value><c>null</c> if [is primary key] contains no value, <c>true</c> if [is primary key]; otherwise, <c>false</c>.</value>
        public bool? IsPrimaryKey { get; set; }
        /// <summary>
        /// Gets or sets the type namespace.
        /// </summary>
        /// <value>The type namespace.</value>
        public string TypeNamespace { get; set; }
        /// <summary>
        /// Gets or sets the generic arguments.
        /// </summary>
        /// <value>The generic arguments.</value>
        public string[] GenericArgs { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        public MetadataDataMember DataMember { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>null</c> if [read only] contains no value, <c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public string ParamType { get; set; }
        /// <summary>
        /// Gets or sets the display type.
        /// </summary>
        /// <value>The display type.</value>
        public string DisplayType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value><c>null</c> if [is required] contains no value, <c>true</c> if [is required]; otherwise, <c>false</c>.</value>
        public bool? IsRequired { get; set; }
        /// <summary>
        /// Gets or sets the allowable values.
        /// </summary>
        /// <value>The allowable values.</value>
        public string[] AllowableValues { get; set; }
        /// <summary>
        /// Gets or sets the allowable minimum.
        /// </summary>
        /// <value>The allowable minimum.</value>
        public int? AllowableMin { get; set; }
        /// <summary>
        /// Gets or sets the allowable maximum.
        /// </summary>
        /// <value>The allowable maximum.</value>
        public int? AllowableMax { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public List<MetadataAttribute> Attributes { get; set; }
    }

    /// <summary>
    /// Class MetadataAttribute.
    /// </summary>
    public class MetadataAttribute
    {
        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>The attribute.</value>
        [IgnoreDataMember]
        public Attribute Attribute { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the constructor arguments.
        /// </summary>
        /// <value>The constructor arguments.</value>
        public List<MetadataPropertyType> ConstructorArgs { get; set; }
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public List<MetadataPropertyType> Args { get; set; }
    }

    /// <summary>
    /// Class MetadataTypeExtensions.
    /// </summary>
    public static class MetadataTypeExtensions
    {
        /// <summary>
        /// Inheritses any.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeNames">The type names.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool InheritsAny(this MetadataType type, params string[] typeNames) =>
            type.Inherits != null && typeNames.Contains(type.Inherits.Name);

        /// <summary>
        /// Implementses any.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeNames">The type names.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ImplementsAny(this MetadataType type, params string[] typeNames) =>
            type.Implements != null && type.Implements.Any(i => typeNames.Contains(i.Name));

        /// <summary>
        /// Referenceses any.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <param name="typeNames">The type names.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ReferencesAny(this MetadataOperationType op, params string[] typeNames) =>
            op.Request.Inherits != null && (typeNames.Contains(op.Request.Inherits.Name) ||
                                            op.Request.Inherits.GenericArgs?.Length > 0 &&
                                            op.Request.Inherits.GenericArgs.Any(typeNames.Contains))
            ||
            op.Response != null && (typeNames.Contains(op.Response.Name) ||
                                    op.Response.GenericArgs?.Length > 0 &&
                                    op.Response.GenericArgs.Any(typeNames.Contains))
            ||
            op.Request.Implements != null && op.Request.Implements.Any(i =>
                i.GenericArgs?.Length > 0 && i.GenericArgs.Any(typeNames.Contains))
            ||
            op.Response?.Inherits != null && (typeNames.Contains(op.Response.Inherits.Name) ||
                                              op.Response.Inherits.GenericArgs?.Length > 0 &&
                                              op.Response.Inherits.GenericArgs.Any(typeNames.Contains));

        /// <summary>
        /// Gets the routes.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="type">The type.</param>
        /// <returns>List&lt;MetadataRoute&gt;.</returns>
        public static List<MetadataRoute> GetRoutes(this List<MetadataOperationType> operations, MetadataType type) =>
            operations.FirstOrDefault(x => ReferenceEquals(x.Request, type))?.Routes ?? operations.GetRoutes(type.Name);
        /// <summary>
        /// Gets the routes.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>List&lt;MetadataRoute&gt;.</returns>
        public static List<MetadataRoute> GetRoutes(this List<MetadataOperationType> operations, string typeName)
        {
            return operations.FirstOrDefault(x => x.Request.Name == typeName)?.Routes;
        }

        /// <summary>
        /// Converts to scriptsignature.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>System.String.</returns>
        public static string ToScriptSignature(this ScriptMethodType method)
        {
            var paramCount = method.ParamNames?.Length ?? 0;
            var firstParam = method.ParamNames?.Length > 0 ? method.ParamNames[0] : null;
            var ret = method.ReturnType != null && method.ReturnType != nameof(StopExecution) ? " -> " + method.ReturnType : "";
            var sig = paramCount == 0
                ? $"{method.Name}{ret}"
                : paramCount == 1
                    ? $"{firstParam} |> {method.Name}{ret}"
                    : $"{firstParam} |> {method.Name}(" + string.Join(", ", method.ParamNames?.Skip(1) ?? new string[0]) + $"){ret}";
            return sig;
        }

        /// <summary>
        /// Gets the operations by tags.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="tags">The tags.</param>
        /// <returns>List&lt;MetadataOperationType&gt;.</returns>
        public static List<MetadataOperationType> GetOperationsByTags(this MetadataTypes types, string[] tags) =>
            types.Operations.Where(x => x.Tags != null && x.Tags.Any(t => Array.IndexOf(tags, t) >= 0)).ToList();


        /// <summary>
        /// The system type chars
        /// </summary>
        private static readonly char[] SystemTypeChars = { '<', '>', '+' };
        /// <summary>
        /// Determines whether [is system or service stack type] [the specified meta reference].
        /// </summary>
        /// <param name="metaRef">The meta reference.</param>
        /// <returns><c>true</c> if [is system or service stack type] [the specified meta reference]; otherwise, <c>false</c>.</returns>
        public static bool IsSystemOrServiceStackType(this MetadataTypeName metaRef)
        {
            if (metaRef.Namespace == null)
                return false;
            return metaRef.Namespace.StartsWith("System") ||
                   metaRef.Namespace.StartsWith("ServiceStack") ||
                   metaRef.Name.IndexOfAny(SystemTypeChars) >= 0;
        }
    }
}