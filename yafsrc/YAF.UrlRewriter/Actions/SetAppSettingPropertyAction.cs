// UrlRewriter - A .NET URL Rewriter module
// Version 2.0
//
// Copyright 2011 Intelligencia
// Copyright 2011 Seth Yates
// 

using System;

namespace YAF.UrlRewriter.Actions
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
            this.Name = name ?? throw new ArgumentNullException("name");
            this.AppSettingKey = appSettingsKey ?? throw new ArgumentNullException("appSettingsKey");
        }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The name of the key in AppSettings.
        /// </summary>
        public string AppSettingKey { get; }

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
            var appSettingValue = context.ConfigurationManager.AppSettings[this.AppSettingKey] ?? string.Empty;
            context.Properties.Set(this.Name, appSettingValue);

            return RewriteProcessing.ContinueProcessing;
        }
    }
}
