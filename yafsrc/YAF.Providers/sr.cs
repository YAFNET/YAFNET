//------------------------------------------------------------------------------
// <copyright file="SR.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace YAF.Providers{

    using System;
    internal static class SR {
        internal static string GetString(string strString) {
            return strString;
        }
        internal static string GetString(string strString, string param1) {
            return string.Format(strString, param1);
        }

        internal static string GetString(string strString, string param1, string param2) {
            return string.Format(strString, param1, param2);
        }
        internal static string GetString(string strString, string param1, string param2, string param3) {
            return string.Format(strString, param1, param2, param3);
        }

        internal const string Auth_rule_names_cant_contain_char = "Authorization rule names cannot contain the '{0}' character.";
        internal const string Connection_name_not_specified = "The attribute 'connectionStringName' is missing or empty.";
        internal const string Connection_string_not_found = "The connection name '{0}' was not found in the applications configuration or the connection string is empty.";
        internal const string Membership_AccountLockOut = "The user account has been locked out.";
        internal const string Membership_Custom_Password_Validation_Failure = "The custom password validation failed.";
        internal const string Membership_InvalidAnswer = "The password-answer supplied is invalid.";
        internal const string Membership_InvalidEmail = "The E-mail supplied is invalid.";
        internal const string Membership_InvalidPassword = "The password supplied is invalid.  Passwords must conform to the password strength requirements configured for the default provider.";
        internal const string Membership_InvalidProviderUserKey = "The provider user key supplied is invalid.  It must be of type System.Guid.";
        internal const string Membership_InvalidQuestion = "The password-question supplied is invalid.  Note that the current provider configuration requires a valid password question and answer.  As a result, a CreateUser overload that accepts question and answer parameters must also be used.";
        internal const string Membership_more_than_one_user_with_email = "More than one user has the specified e-mail address.";
        internal const string Membership_password_too_long = "The password is too long: it must not exceed 128 chars after encrypting.";
        internal const string Membership_PasswordRetrieval_not_supported = "This Membership Provider has not been configured to support password retrieval.";
        internal const string Membership_UserNotFound = "The user was not found.";
        internal const string Membership_WrongAnswer = "The password-answer supplied is wrong.";
        internal const string Membership_WrongPassword = "The password supplied is wrong.";
        internal const string PageIndex_bad = "The pageIndex must be greater than or equal to zero.";
        internal const string PageIndex_PageSize_bad = "The combination of pageIndex and pageSize cannot exceed the maximum value of System.Int32.";
        internal const string PageSize_bad = "The pageSize must be greater than zero.";
        internal const string Parameter_array_empty = "The array parameter '{0}' should not be empty.";
        internal const string Parameter_can_not_be_empty = "The parameter '{0}' must not be empty.";
        internal const string Parameter_can_not_contain_comma = "The parameter '{0}' must not contain commas.";
        internal const string Parameter_duplicate_array_element = "The array '{0}' should not contain duplicate values.";
        internal const string Parameter_too_long = "The parameter '{0}' is too long: it must not exceed {1} chars in length.";
        internal const string Password_does_not_match_regular_expression = "The parameter '{0}' does not match the regular expression specified in config file.";
        internal const string Password_need_more_non_alpha_numeric_chars = "Non alpha numeric characters in '{0}' needs to be greater than or equal to '{1}'.";
        internal const string Password_too_short = "The length of parameter '{0}' needs to be greater or equal to '{1}'.";
        internal const string PersonalizationProvider_ApplicationNameExceedMaxLength = "The ApplicationName cannot exceed character length {0}.";
        internal const string PersonalizationProvider_BadConnection = "The specified connectionStringName, '{0}', was not registered.";
        internal const string PersonalizationProvider_CantAccess = "A connection could not be made by the {0} personalization provider using the specified registration.";
        internal const string PersonalizationProvider_NoConnection = "The connectionStringName attribute must be specified when registering a personalization provider.";
        internal const string PersonalizationProvider_UnknownProp = "Invalid attribute '{0}', specified in the '{1}' personalization provider registration.";
        internal const string ProfileSqlProvider_description = "SQL profile provider.";
        internal const string Property_Had_Malformed_Url = "The '{0}' property had a malformed URL: {1}.";
        internal const string Provider_application_name_too_long = "The application name is too long.";
        internal const string Provider_bad_password_format = "Password format specified is invalid.";
        internal const string Provider_can_not_retrieve_hashed_password = "Configured settings are invalid: Hashed passwords cannot be retrieved. Either set the password format to different type, or set supportsPasswordRetrieval to false.";
        internal const string Provider_Error = "The Provider encountered an unknown error.";
        internal const string Provider_Not_Found = "Provider '{0}' was not found.";
        internal const string Provider_role_already_exists = "The role '{0}' already exists.";
        internal const string Provider_role_not_found = "The role '{0}' was not found.";
        internal const string Provider_Schema_Version_Not_Match = "The '{0}' requires a database schema compatible with schema version '{1}'.  However, the current database schema is not compatible with this version.  You may need to either install a compatible schema with aspnet_regsql.exe (available in the framework installation directory), or upgrade the provider to a newer version.";
        internal const string Provider_this_user_already_in_role = "The user '{0}' is already in role '{1}'.";
        internal const string Provider_this_user_not_found = "The user '{0}' was not found.";
        internal const string Provider_unknown_failure = "Stored procedure call failed.";
        internal const string Provider_unrecognized_attribute = "Attribute not recognized '{0}'";
        internal const string Provider_user_not_found = "The user was not found in the database.";
        internal const string Role_is_not_empty = "This role cannot be deleted because there are users present in it.";
        internal const string RoleSqlProvider_description = "SQL role provider.";
        internal const string SiteMapProvider_cannot_remove_root_node = "Root node cannot be removed from the providers, use RemoveProvider(string providerName) instead.";
        internal const string SqlError_Connection_String = "An error occurred while attempting to initialize a System.Data.SqlClient.SqlConnection object. The value that was provided for the connection string may be wrong, or it may contain an invalid syntax.";
        internal const string SqlExpress_file_not_found_in_connection_string = "SQL Express filename was not found in the connection string.";
        internal const string SqlPersonalizationProvider_Description = "Personalization provider that stores data in a SQL Server database.";
        internal const string Value_must_be_boolean = "The value must be boolean (true or false) for property '{0}'.";
        internal const string Value_must_be_non_negative_integer = "The value must be a non-negative 32-bit integer for property '{0}'.";
        internal const string Value_must_be_positive_integer = "The value must be a positive 32-bit integer for property '{0}'.";
        internal const string Value_too_big = "The value '{0}' can not be greater than '{1}'.";
        internal const string XmlSiteMapProvider_cannot_add_node = "SiteMapNode {0} cannot be found in current provider, only nodes in the same provider can be added.";
        internal const string XmlSiteMapProvider_Cannot_Be_Inited_Twice = "XmlSiteMapProvider cannot be initialized twice.";
        internal const string XmlSiteMapProvider_cannot_find_provider = "Provider {0} cannot be found inside XmlSiteMapProvider {1}.";
        internal const string XmlSiteMapProvider_cannot_remove_node = "SiteMapNode {0} does not exist in provider {1}, it must be removed from provider {2}.";
        internal const string XmlSiteMapProvider_Description = "SiteMap provider which reads in .sitemap XML files.";
        internal const string XmlSiteMapProvider_Error_loading_Config_file = "The XML sitemap config file {0} could not be loaded.  {1}";
        internal const string XmlSiteMapProvider_FileName_already_in_use = "The sitemap config file {0} is already used by other nodes or providers.";
        internal const string XmlSiteMapProvider_FileName_does_not_exist = "The file {0} required by XmlSiteMapProvider does not exist.";
        internal const string XmlSiteMapProvider_Invalid_Extension = "The file {0} has an invalid extension, only .sitemap files are allowed in XmlSiteMapProvider.";
        internal const string XmlSiteMapProvider_invalid_GetRootNodeCore = "GetRootNode is returning null from Provider {0}, this method must return a non-empty sitemap node.";
        internal const string XmlSiteMapProvider_invalid_resource_key = "Resource key {0} is not valid, it must contain a valid class name and key pair. For example, $resources:'className','key'";
        internal const string XmlSiteMapProvider_invalid_sitemapnode_returned = "Provider {0} must return a valid sitemap node.";
        internal const string XmlSiteMapProvider_missing_siteMapFile = "The {0} attribute must be specified on the XmlSiteMapProvider.";
        internal const string XmlSiteMapProvider_Multiple_Nodes_With_Identical_Key = "Multiple nodes with the same key '{0}' were found. XmlSiteMapProvider requires that sitemap nodes have unique keys.";
        internal const string XmlSiteMapProvider_Multiple_Nodes_With_Identical_Url = "Multiple nodes with the same URL '{0}' were found. XmlSiteMapProvider requires that sitemap nodes have unique URLs.";
        internal const string XmlSiteMapProvider_multiple_resource_definition = "Cannot have more than one resource binding on attribute '{0}'. Ensure that this attribute is not bound through an implicit expression, for example, {0}=\"$resources:key\".";
        internal const string XmlSiteMapProvider_Not_Initialized = "XmlSiteMapProvider is not initialized. Call Initialize() method first.";
        internal const string XmlSiteMapProvider_Only_One_SiteMapNode_Required_At_Top = "Exactly one <siteMapNode> element is required directly inside the <siteMap> element.";
        internal const string XmlSiteMapProvider_Only_SiteMapNode_Allowed = "Only <siteMapNode> elements are allowed at this location.";
        internal const string XmlSiteMapProvider_resourceKey_cannot_be_empty = "Resource key cannot be empty.";
        internal const string XmlSiteMapProvider_Top_Element_Must_Be_SiteMap = "Top element must be siteMap.";
        internal const string PersonalizationProviderHelper_TrimmedEmptyString = "Input parameter '{0}' cannot be an empty string.";
        internal const string StringUtil_Trimmed_String_Exceed_Maximum_Length = "Trimmed string value '{0}' of input parameter '{1}' cannot exceed character length {2}.";
        internal const string MembershipSqlProvider_description = "SQL membership provider.";
        internal const string MinRequiredNonalphanumericCharacters_can_not_be_more_than_MinRequiredPasswordLength = "The minRequiredNonalphanumericCharacters can not be greater than minRequiredPasswordLength.";
        internal const string PersonalizationProviderHelper_Empty_Collection = "Input parameter '{0}' cannot be an empty collection.";
        internal const string PersonalizationProviderHelper_Null_Or_Empty_String_Entries = "Input parameter '{0}' cannot contain null or empty string entries.";
        internal const string PersonalizationProviderHelper_CannotHaveCommaInString = "Input parameter '{0}' cannot have comma in string value '{1}'.";
        internal const string PersonalizationProviderHelper_Trimmed_Entry_Value_Exceed_Maximum_Length = "Trimmed entry value '{0}' of input parameter '{1}' cannot exceed character length {2}.";
        internal const string PersonalizationProviderHelper_More_Than_One_Path = "Input parameter '{0}' cannot contain more than one entry when '{1}' contains some entries.";
        internal const string PersonalizationProviderHelper_Negative_Integer = "The input parameter cannot be negative.";
        internal const string PersonalizationAdmin_UnexpectedPersonalizationProviderReturnValue = "The negative value '{0}' is returned when calling provider's '{1}' method.  The method should return non-negative integer.";
internal const string PersonalizationProviderHelper_Null_Entries = "Input parameter '{0}' cannot contain null entries.";
        internal const string PersonalizationProviderHelper_Invalid_Less_Than_Parameter = "Input parameter '{0}' must be greater than or equal to {1}.";
        internal const string PersonalizationProviderHelper_No_Usernames_Set_In_Shared_Scope = "Input parameter '{0}' cannot be provided when '{1}' is set to '{2}'.";
        internal const string Provider_this_user_already_not_in_role = "The user '{0}' is already not in role '{1}'.";
        internal const string Not_configured_to_support_password_resets = "This provider is not configured to allow password resets. To enable password reset, set enablePasswordReset to \"true\" in the configuration file.";
        internal const string Parameter_collection_empty = "The collection parameter '{0}' should not be empty.";
        internal const string Provider_can_not_decode_hashed_password = "Hashed passwords cannot be decoded.";
        internal const string DbFileName_can_not_contain_invalid_chars = "The database filename can not contain the following 3 characters: [ (open square brace), ] (close square brace) and ' (single quote)";
        internal const string SQL_Services_Error_Deleting_Session_Job = "The attempt to remove the Session State expired sessions job from msdb did not succeed.  This can occur either because the job no longer exists, or because the job was originally created with a different user account than the account that is currently performing the uninstall.  You will need to manually delete the Session State expired sessions job if it still exists.";
        internal const string SQL_Services_Error_Executing_Command = "An error occurred during the execution of the SQL file '{0}'. The SQL error number is {1} and the SqlException message is: {2}";
        internal const string SQL_Services_Invalid_Feature = "An invalid feature is requested.";
        internal const string SQL_Services_Database_Empty_Or_Space_Only_Arg = "The database name cannot be empty or contain only white space characters.";
        internal const string SQL_Services_Database_contains_invalid_chars = "The custom database name cannot contain the following three characters: single quotation mark ('), left bracket ([) or right bracket (]).";
        internal const string SQL_Services_Error_Cant_Uninstall_Nonexisting_Database = "Cannot uninstall the specified feature(s) because the SQL database '{0}' does not exist.";
        internal const string SQL_Services_Error_Cant_Uninstall_Nonempty_Table = "Cannot uninstall the specified feature(s) because the SQL table '{0}' in the database '{1}' is not empty. You must first remove all rows from the table.";
        internal const string SQL_Services_Error_missing_custom_database = "The database name cannot be null or empty if the session state type is SessionStateType.Custom.";
        internal const string SQL_Services_Error_Cant_use_custom_database = "You cannot specify the database name because it is allowed only if the session state type is SessionStateType.Custom.";
        internal const string SQL_Services_Cant_connect_sql_database = "Unable to connect to SQL Server database.";
        internal const string Error_parsing_sql_partition_resolver_string = "Error parsing the SQL connection string returned by an instance of the IPartitionResolver type '{0}': {1}";
        internal const string Error_parsing_session_sqlConnectionString = "Error parsing <sessionState> sqlConnectionString attribute: {0}";
        internal const string No_database_allowed_in_sqlConnectionString = "The sqlConnectionString attribute or the connection string it refers to cannot contain the connection options 'Database', 'Initial Catalog' or 'AttachDbFileName'. In order to allow this, allowCustomSqlDatabase attribute must be set to true and the application needs to be granted unrestricted SqlClientPermission. Please check with your administrator if the application does not have this permission.";
        internal const string No_database_allowed_in_sql_partition_resolver_string = "The SQL connection string (server='{1}', database='{2}') returned by an instance of the IPartitionResolver type '{0}' cannot contain the connection options 'Database', 'Initial Catalog' or 'AttachDbFileName'. In order to allow this, allowCustomSqlDatabase attribute must be set to true and the application needs to be granted unrestricted SqlClientPermission. Please check with your administrator if the application does not have this permission.";
        internal const string Cant_connect_sql_session_database = "Unable to connect to SQL Server session database.";
        internal const string Cant_connect_sql_session_database_partition_resolver = "Unable to connect to SQL Server session database. The connection string (server='{1}', database='{2}') was returned by an instance of the IPartitionResolver type '{0}'.";
        internal const string Login_failed_sql_session_database = "Failed to login to session state SQL server for user '{0}'.";
        internal const string Need_v2_SQL_Server = "Unable to use SQL Server because ASP.NET version 2.0 Session State is not installed on the SQL server. Please install ASP.NET Session State SQL Server version 2.0 or above.";
        internal const string Need_v2_SQL_Server_partition_resolver = "Unable to use SQL Server because ASP.NET version 2.0 Session State is not installed on the SQL server. Please install ASP.NET Session State SQL Server version 2.0 or above. The connection string (server='{1}', database='{2}') was returned by an instance of the IPartitionResolver type '{0}'.";
        internal const string Invalid_session_state = "The session state information is invalid and might be corrupted.";

        internal const string Missing_required_attribute = "The '{0}' attribute must be specified on the '{1}' tag.";
        internal const string Invalid_boolean_attribute = "The '{0}' attribute must be set to 'true' or 'false'.";
        internal const string Empty_attribute = "The '{0}' attribute cannot be an empty string.";
        internal const string Config_base_unrecognized_attribute = "Unrecognized attribute '{0}'. Note that attribute names are case-sensitive.";
        internal const string Config_base_no_child_nodes = "Child nodes are not allowed.";
        internal const string Unexpected_provider_attribute = "The attribute '{0}' is unexpected in the configuration of the '{1}' provider.";
        internal const string Only_one_connection_string_allowed = "SqlWebEventProvider: Specify either a connectionString or connectionStringName, not both.";
        internal const string Cannot_use_integrated_security = "SqlWebEventProvider: connectionString can only contain connection strings that use Sql Server authentication.  Trusted Connection security is not supported.";
        internal const string Must_specify_connection_string_or_name = "SqlWebEventProvider: Either a connectionString or connectionStringName must be specified.";
        internal const string Invalid_max_event_details_length = "The value '{1}' specified for the maxEventDetailsLength attribute of the '{0}' provider is invalid. It should be between 0 and 1073741823.";
        internal const string Sql_webevent_provider_events_dropped = "{0} events were discarded since last notification was made at {1} because the event buffer capacity was exceeded.";
        internal const string Invalid_provider_positive_attributes = "The attribute '{0}' is invalid in the configuration of the '{1}' provider. The attribute must be set to a non-negative integer.";
    }
}

