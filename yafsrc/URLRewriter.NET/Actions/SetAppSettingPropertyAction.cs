// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace Intelligencia.UrlRewriter.Actions
{
    /// <summary>
    /// Action that sets a property in the context from AppSettings, i.e the appSettings collection
    /// in web.config.
    /// </summary>
    public class SetAppSettingPropertyAction : IRewriteAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="appSettingsKey">The name of the key in AppSettings.</param>
        public SetAppSettingPropertyAction(string name, string appSettingsKey)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (appSettingsKey == null)
            {
                throw new ArgumentNullException("appSettingsKey");
            }

            _name = name;
            _appSettingsKey = appSettingsKey;
        }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The name of the key in AppSettings.
        /// </summary>
        public string AppSettingKey
        {
            get { return _appSettingsKey; }
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="context">The rewrite context.</param>
        public RewriteProcessing Execute(IRewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // If the value cannot be found in AppSettings, default to an empty string.
            string appSettingValue = context.ConfigurationManager.AppSettings[_appSettingsKey] ?? String.Empty;
            context.Properties.Set(Name, appSettingValue);

            return RewriteProcessing.ContinueProcessing;
        }

        private string _name;
        private string _appSettingsKey;
    }
}
