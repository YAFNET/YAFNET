using System;

namespace YAF.Classes
{
	/// <summary>
	/// Defines interface for UrlBuilder class.
	/// </summary>
	public interface IUrlBuilder
	{
		/// <summary>
		/// Builds URL for calling page with URL argument as and parameter.
		/// </summary>
		/// <param name="url">URL to use as a parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		string BuildUrl(string url);
	}
}
