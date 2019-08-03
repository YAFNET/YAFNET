namespace ServiceStack.FluentValidation.Resources {
	using System;

    /// <summary>
	/// IStringSource implementation that uses the default language manager.
	/// </summary>
	public class LanguageStringSource : IStringSource {
        public LanguageStringSource(string key) {
			this.ResourceName = key;
		}

		public string GetString(object context) {
			return ValidatorOptions.LanguageManager.GetString(this.ResourceName);
		}

		public string ResourceName { get; }

        public Type ResourceType => typeof(LanguageManager);
	}
}