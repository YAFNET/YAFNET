namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

/// <summary>
/// 
/// </summary>
public class PushSubscription
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PushSubscription"/> class.
    /// </summary>
    public PushSubscription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushSubscription"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="p256dh">The P256DH.</param>
    /// <param name="auth">The authentication.</param>
    public PushSubscription(string endpoint, string p256dh, string auth)
    {
        this.Endpoint = endpoint;
        this.P256DH = p256dh;
        this.Auth = auth;
    }

    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    /// <value>
    /// The endpoint.
    /// </value>
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the P256 dh.
    /// </summary>
    /// <value>
    /// The P256 dh.
    /// </value>
    public string P256DH { get; set; }

    /// <summary>
    /// Gets or sets the authentication.
    /// </summary>
    /// <value>
    /// The authentication.
    /// </value>
    public string Auth { get; set; }
}