// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The stream extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Utils
{
	using System.IO;
	using System.Text;

	using YAF.Types;

	/// <summary>
	/// The stream extensions.
	/// </summary>
	public static class StreamExtensions
	{
		#region Public Methods

		/// <summary>
		/// Converts a Stream to a String.
		/// </summary>
		/// <param name="theStream">
		/// </param>
		/// <returns>
		/// The stream to string.
		/// </returns>
		[NotNull]
		public static string AsString([NotNull] this Stream theStream, [CanBeNull] Encoding encoding = null)
		{
			CodeContracts.ArgumentNotNull(theStream, "theStream");

			return new StreamReader(theStream, encoding ?? Encoding.Default).ReadToEnd();
		}

		/// <summary>
		/// The copy stream.
		/// </summary>
		/// <param name="inputStream">
		/// The input.
		/// </param>
		/// <param name="outputStream">
		/// The output.
		/// </param>
		public static void CopyTo([NotNull] this Stream inputStream, [NotNull] Stream outputStream)
		{
			CodeContracts.ArgumentNotNull(inputStream, "inputStream");
			CodeContracts.ArgumentNotNull(outputStream, "outputStream");

			var buffer = new byte[16 * 1024];
			int read;

			while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				outputStream.Write(buffer, 0, read);
			}
		}

		/// <summary>
		/// Converts the input stream into a byte[] array.
		/// </summary>
		/// <param name="inputStream">
		/// The input stream.
		/// </param>
		/// <returns>
		/// </returns>
		[NotNull]
		public static byte[] ToArray([NotNull] this Stream inputStream)
		{
			CodeContracts.ArgumentNotNull(inputStream, "inputStream");

			// set the position to the beginning...
			inputStream.Position = 0L;

			using (var memoryStream = new MemoryStream())
			{
				inputStream.CopyTo(memoryStream);

				return memoryStream.ToArray();
			}
		}

		#endregion
	}
}