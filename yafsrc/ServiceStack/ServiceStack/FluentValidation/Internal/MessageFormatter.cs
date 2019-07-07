namespace ServiceStack.FluentValidation.Internal {
	using System.Collections.Generic;

	/// <summary>
	/// Assists in the construction of validation messages.
	/// </summary>
	public class MessageFormatter {
        private bool _shouldUseAdditionalArgs;

		/// <summary>
		/// Default Property Name placeholder.
		/// </summary>
		public const string PropertyName = "PropertyName";

		/// <summary>
		/// Default Property Value placeholder.
		/// </summary>
		public const string PropertyValue = "PropertyValue";

		public MessageFormatter() {
			
		}

		/// <summary>
		/// Adds a value for a validation message placeholder.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public MessageFormatter AppendArgument(string name, object value) {
			this.PlaceholderValues[name] = value;
			return this;
		}

		/// <summary>
		/// Appends a property name to the message.
		/// </summary>
		/// <param name="name">The name of the property</param>
		/// <returns></returns>
		public MessageFormatter AppendPropertyName(string name) {
			return AppendArgument(PropertyName, name);
		}

		/// <summary>
		/// Appends a property value to the message.
		/// </summary>
		/// <param name="value">The value of the property</param>
		/// <returns></returns>
		public MessageFormatter AppendPropertyValue(object value)
		{
			return AppendArgument(PropertyValue, value);
		}

		/// <summary>
		/// Adds additional arguments to the message for use with standard string placeholders.
		/// </summary>
		/// <param name="additionalArgs">Additional arguments</param>
		/// <returns></returns>
		public MessageFormatter AppendAdditionalArguments(params object[] additionalArgs) {
			this.AdditionalArguments = additionalArgs;
			_shouldUseAdditionalArgs = this.AdditionalArguments != null && this.AdditionalArguments.Length > 0;
			return this;
		}

		/// <summary>
		/// Constructs the final message from the specified template. 
		/// </summary>
		/// <param name="messageTemplate">Message template</param>
		/// <returns>The message with placeholders replaced with their appropriate values</returns>
		public virtual string BuildMessage(string messageTemplate) {

			var result = messageTemplate;

			foreach(var pair in this.PlaceholderValues) {
				result = ReplacePlaceholderWithValue(result, pair.Key, pair.Value);
			}

			if(_shouldUseAdditionalArgs) {
				return string.Format(result, this.AdditionalArguments);
			}
			return result;
		}

		/// <summary>
		/// Additional arguments to use
		/// </summary>
		public object[] AdditionalArguments { get; private set; } = new object[0];

        /// <summary>
		/// Additional placeholder values
		/// </summary>
		public Dictionary<string, object> PlaceholderValues { get; } = new Dictionary<string, object>(2);

        protected virtual string ReplacePlaceholderWithValue(string template, string key, object value) {
			var placeholder =  GetPlaceholder(key);
			return template.Replace(placeholder, value?.ToString());
		}

		protected string GetPlaceholder(string key) {
			// Performance: String concat causes much overhead when not needed. Concatting constants results in constants being compiled.
			switch (key) {
				case PropertyName:
					return "{" + PropertyName + "}";
				case PropertyValue:
					return "{" + PropertyValue + "}";
				default:
					return "{" + key + "}";
			}
		}
	}
}