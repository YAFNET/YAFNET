namespace YAF.Classes.Core
{
  #region Using

  using System.Net.Mail;
  using System.Text;

  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The mail message extensions.
  /// </summary>
  public static class MailMessageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The populate.
    /// </summary>
    /// <param name="mailMessage">
    /// The mail message.
    /// </param>
    /// <param name="fromAddress">
    /// The from address.
    /// </param>
    /// <param name="toAddress">
    /// The to address.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="bodyText">
    /// The body text.
    /// </param>
    /// <param name="bodyHtml">
    /// The body html.
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static void Populate([NotNull] this MailMessage mailMessage, 
      [NotNull] MailAddress fromAddress, 
      [NotNull] MailAddress toAddress, 
      [CanBeNull] string subject, 
      [CanBeNull] string bodyText, 
      [CanBeNull] string bodyHtml)
    {
      CodeContracts.ArgumentNotNull(mailMessage, "mailMessage");
      CodeContracts.ArgumentNotNull(fromAddress, "fromAddress");
      CodeContracts.ArgumentNotNull(toAddress, "toAddress");

      mailMessage.To.Add(toAddress);
      mailMessage.From = fromAddress;
      mailMessage.Subject = subject;

      Encoding textEncoding = Encoding.UTF8;

      // TODO: Add code that figures out encoding...
      /*
      if ( !Regex.IsMatch( bodyText, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) ||
              !Regex.IsMatch( subject, @"^([0-9a-z!@#\$\%\^&\*\(\)\-=_\+])", RegexOptions.IgnoreCase ) )
      {
        textEncoding = Encoding.Unicode;
      }
      */

      // add text view...
      mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, textEncoding, "text/plain"));

      // see if html alternative is also desired...
      if (bodyHtml.IsSet())
      {
        mailMessage.AlternateViews.Add(
          AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/html"));
      }
    }

    #endregion
  }
}