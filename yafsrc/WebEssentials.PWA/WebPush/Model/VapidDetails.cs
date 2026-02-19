namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

/// <summary>
/// 
/// </summary>
public class VapidDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VapidDetails"/> class.
    /// </summary>
    public VapidDetails()
    {
    }

    /// <param name="subject">This should be a URL or a 'mailto:' email address</param>
    /// <param name="publicKey">The VAPID public key as a base64 encoded string</param>
    /// <param name="privateKey">The VAPID private key as a base64 encoded string</param>
    public VapidDetails(string subject, string publicKey, string privateKey)
    {
        this.Subject = subject;
        this.PublicKey = publicKey;
        this.PrivateKey = privateKey;
    }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>
    /// The subject.
    /// </value>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the public key.
    /// </summary>
    /// <value>
    /// The public key.
    /// </value>
    public string PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the private key.
    /// </summary>
    /// <value>
    /// The private key.
    /// </value>
    public string PrivateKey { get; set; }

    /// <summary>
    /// Gets or sets the expiration.
    /// </summary>
    /// <value>
    /// The expiration.
    /// </value>
    public long Expiration { get; set; } = -1;
}