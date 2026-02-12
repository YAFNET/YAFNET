using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using WebEssentials.AspNetCore.Pwa.WebPush.Model;
using WebEssentials.AspNetCore.Pwa.WebPush.Util;

namespace WebEssentials.AspNetCore.Pwa.WebPush;

public class WebPushClient : IWebPushClient
{
    // default TTL is 4 weeks.
    private const int DefaultTtl = 2419200;

    /// <summary>
    /// The HTTP client handler
    /// </summary>
    private readonly HttpClientHandler _httpClientHandler;

    private string _gcmApiKey;
    private HttpClient _httpClient;
    private VapidDetails _vapidDetails;

    // Used so we only cleanup internally created http clients
    private bool _isHttpClientInternallyCreated;

    public WebPushClient()
    {
    }

    public WebPushClient(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    public WebPushClient(HttpClientHandler httpClientHandler)
    {
        this._httpClientHandler = httpClientHandler;
    }

    protected HttpClient HttpClient {
        get {
            if (this._httpClient != null)
            {
                return this._httpClient;
            }

            this._isHttpClientInternallyCreated = true;
            this._httpClient = this._httpClientHandler == null
                ? new HttpClient()
                : new HttpClient(this._httpClientHandler);

            return this._httpClient;
        }
    }

    /// <summary>
    ///     When sending messages to a GCM endpoint you need to set the GCM API key
    ///     by either calling setGcmApiKey() or passing in the API key as an option
    ///     to sendNotification()
    /// </summary>
    /// <param name="gcmApiKey">The API key to send with the GCM request.</param>
    public void SetGcmApiKey(string gcmApiKey)
    {
        if (gcmApiKey == null)
        {
            this._gcmApiKey = null;
            return;
        }

        if (string.IsNullOrEmpty(gcmApiKey))
        {
            throw new ArgumentException("The GCM API Key should be a non-empty string or null.");
        }

        this._gcmApiKey = gcmApiKey;
    }

    /// <summary>
    ///     When marking requests where you want to define VAPID details, call this method
    ///     before sendNotifications() or pass in the details and options to
    ///     sendNotification.
    /// </summary>
    /// <param name="vapidDetails"></param>
    public void SetVapidDetails(VapidDetails vapidDetails)
    {
        VapidHelper.ValidateSubject(vapidDetails.Subject);
        VapidHelper.ValidatePublicKey(vapidDetails.PublicKey);
        VapidHelper.ValidatePrivateKey(vapidDetails.PrivateKey);

        this._vapidDetails = vapidDetails;
    }

    /// <summary>
    ///     When marking requests where you want to define VAPID details, call this method
    ///     before sendNotifications() or pass in the details and options to
    ///     sendNotification.
    /// </summary>
    /// <param name="subject">This must be either a URL or a 'mailto:' address</param>
    /// <param name="publicKey">The public VAPID key as a base64 encoded string</param>
    /// <param name="privateKey">The private VAPID key as a base64 encoded string</param>
    public void SetVapidDetails(string subject, string publicKey, string privateKey)
    {
        this.SetVapidDetails(new VapidDetails(subject, publicKey, privateKey));
    }

    /// <summary>
    ///     To get a request without sending a push notification call this method.
    ///     This method will throw an ArgumentException if there is an issue with the input.
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="options">
    ///     Options for the GCM API key and vapid keys can be passed in if they are unique for each
    ///     notification.
    /// </param>
    /// <returns>A HttpRequestMessage object that can be sent.</returns>
    public HttpRequestMessage GenerateRequestDetails(PushSubscription subscription, string payload,
        Dictionary<string, object> options = null)
    {
        if (!Uri.IsWellFormedUriString(subscription.Endpoint, UriKind.Absolute))
        {
            throw new ArgumentException("You must pass in a subscription with at least a valid endpoint");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, subscription.Endpoint);

        if (!string.IsNullOrEmpty(payload) && (string.IsNullOrEmpty(subscription.Auth) ||
                                               string.IsNullOrEmpty(subscription.P256DH)))
        {
            throw new ArgumentException(
                "To send a message with a payload, the subscription must have 'auth' and 'p256dh' keys.");
        }

        var currentGcmApiKey = this._gcmApiKey;
        var currentVapidDetails = this._vapidDetails;
        var timeToLive = DefaultTtl;
        var extraHeaders = new Dictionary<string, object>();

        if (options != null)
        {
            var validOptionsKeys = new List<string> { "headers", "gcmAPIKey", "vapidDetails", "TTL" };

            if (options.Keys.Any(key => !validOptionsKeys.Contains(key)))
            {
                var key = options.Keys.Select(key => !validOptionsKeys.Contains(key)).First();

                throw new ArgumentException(
                    $"{key} is an invalid options. The valid options are{string.Join(",", validOptionsKeys)}");
            }

            if (options.TryGetValue("headers", out var option))
            {
                var headers = option as Dictionary<string, object>;

                extraHeaders = headers ??
                               throw new ArgumentException("options.headers must be of type Dictionary<string,object>");
            }

            if (options.TryGetValue("gcmAPIKey", out var option1))
            {
                var gcmApiKey = option1 as string;

                currentGcmApiKey = gcmApiKey ?? throw new ArgumentException("options.gcmAPIKey must be of type string");
            }

            if (options.TryGetValue("vapidDetails", out var option2))
            {
                var vapidDetails = option2 as VapidDetails;
                currentVapidDetails = vapidDetails ??
                                      throw new ArgumentException("options.vapidDetails must be of type VapidDetails");
            }

            if (options.TryGetValue("TTL", out var option3))
            {
                if (option3 is not int ttl)
                {
                    throw new ArgumentException("options.TTL must be of type int");
                }

                //at this stage ttl cannot be null.
                timeToLive = ttl;
            }
        }

        string cryptoKeyHeader = null;
        request.Headers.Add("TTL", timeToLive.ToString());

        foreach (var header in extraHeaders)
        {
            request.Headers.Add(header.Key, header.Value.ToString());
        }

        if (!string.IsNullOrEmpty(payload))
        {
            if (string.IsNullOrEmpty(subscription.P256DH) || string.IsNullOrEmpty(subscription.Auth))
            {
                throw new ArgumentException(
                    "Unable to send a message with payload to this subscription since it doesn't have the required encryption key");
            }

            var encryptedPayload = EncryptPayload(subscription, payload);

            request.Content = new ByteArrayContent(encryptedPayload.Payload);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            request.Content.Headers.ContentLength = encryptedPayload.Payload.Length;
            request.Content.Headers.ContentEncoding.Add("aesgcm");
            request.Headers.Add("Encryption", $"salt={encryptedPayload.Base64EncodeSalt()}");
            cryptoKeyHeader = $"dh={encryptedPayload.Base64EncodePublicKey()}";
        }
        else
        {
            request.Content = new ByteArrayContent([]);
            request.Content.Headers.ContentLength = 0;
        }

        var isGcm = subscription.Endpoint.StartsWith("https://android.googleapis.com/gcm/send");
        var isFcm = subscription.Endpoint.StartsWith("https://fcm.googleapis.com/fcm/send/");

        if (isGcm)
        {
            if (!string.IsNullOrEmpty(currentGcmApiKey))
            {
                request.Headers.TryAddWithoutValidation("Authorization", $"key={currentGcmApiKey}");
            }
        }
        else if (currentVapidDetails != null)
        {
            var uri = new Uri(subscription.Endpoint);
            var audience = $"{uri.Scheme}://{uri.Host}";

            var vapidHeaders = VapidHelper.GetVapidHeaders(audience, currentVapidDetails.Subject,
                currentVapidDetails.PublicKey, currentVapidDetails.PrivateKey, currentVapidDetails.Expiration);
            request.Headers.Add("Authorization", vapidHeaders["Authorization"]);
            if (string.IsNullOrEmpty(cryptoKeyHeader))
            {
                cryptoKeyHeader = vapidHeaders["Crypto-Key"];
            }
            else
            {
                cryptoKeyHeader += $";{vapidHeaders["Crypto-Key"]}";
            }
        }
        else if (isFcm && !string.IsNullOrEmpty(currentGcmApiKey))
        {
            request.Headers.TryAddWithoutValidation("Authorization", $"key={currentGcmApiKey}");
        }

        request.Headers.Add("Crypto-Key", cryptoKeyHeader);
        return request;
    }

    /// <summary>
    /// Encrypts the payload.
    /// </summary>
    /// <param name="subscription">The subscription.</param>
    /// <param name="payload">The payload.</param>
    /// <returns></returns>
    /// <exception cref="InvalidEncryptionDetailsException">Unable to encrypt the payload with the encryption key of this subscription.</exception>
    private static EncryptionResult EncryptPayload(PushSubscription subscription, string payload)
    {
        try
        {
            return Encryptor.Encrypt(subscription.P256DH, subscription.Auth, payload);
        }
        catch (Exception ex)
        {
            if (ex is FormatException or ArgumentException)
            {
                throw new InvalidEncryptionDetailsException(
                    "Unable to encrypt the payload with the encryption key of this subscription.", subscription);
            }

            throw;
        }
    }

    /// <summary>
    ///     To send a push notification call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="options">
    ///     Options for the GCM API key and vapid keys can be passed in if they are unique for each
    ///     notification.
    /// </param>
    public void SendNotification(PushSubscription subscription, string payload = null,
        Dictionary<string, object> options = null)
    {
        this.SendNotificationAsync(subscription, payload, options).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     To send a push notification call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="vapidDetails">The vapid details for the notification.</param>
    public void SendNotification(PushSubscription subscription, string payload, VapidDetails vapidDetails)
    {
        var options = new Dictionary<string, object> { ["vapidDetails"] = vapidDetails };
        this.SendNotification(subscription, payload, options);
    }

    /// <summary>
    ///     To send a push notification call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="gcmApiKey">The GCM API key</param>
    public void SendNotification(PushSubscription subscription, string payload, string gcmApiKey)
    {
        var options = new Dictionary<string, object> { ["gcmAPIKey"] = gcmApiKey };
        this.SendNotification(subscription, payload, options);
    }


    /// <summary>
    ///     To send a push notification asynchronous call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="options">
    ///     Options for the GCM API key and vapid keys can be passed in if they are unique for each
    ///     notification.
    /// </param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    public async Task SendNotificationAsync(PushSubscription subscription, string payload = null,
        Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
    {
        var request = this.GenerateRequestDetails(subscription, payload, options);
        var response = await this.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        await HandleResponse(response, subscription).ConfigureAwait(false);
    }

    /// <summary>
    ///     To send a push notification asynchronous call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="vapidDetails">The vapid details for the notification.</param>
    /// <param name="cancellationToken"></param>
    public async Task SendNotificationAsync(PushSubscription subscription, string payload,
        VapidDetails vapidDetails, CancellationToken cancellationToken = default)
    {
        var options = new Dictionary<string, object> { ["vapidDetails"] = vapidDetails };
        await this.SendNotificationAsync(subscription, payload, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     To send a push notification asynchronous call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="gcmApiKey">The GCM API key</param>
    /// <param name="cancellationToken"></param>
    public async Task SendNotificationAsync(PushSubscription subscription, string payload, string gcmApiKey,
        CancellationToken cancellationToken = default)
    {
        var options = new Dictionary<string, object> { ["gcmAPIKey"] = gcmApiKey };
        await this.SendNotificationAsync(subscription, payload, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Handle Web Push responses.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="subscription"></param>
    private async static Task HandleResponse(HttpResponseMessage response, PushSubscription subscription)
    {
        // Successful
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        // Error
        var responseCodeMessage = $"Received unexpected response code: {(int)response.StatusCode}";
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                responseCodeMessage = "Bad Request";
                break;

            case HttpStatusCode.RequestEntityTooLarge:
                responseCodeMessage = "Payload too large";
                break;

            case (HttpStatusCode)429:
                responseCodeMessage = "Too many request";
                break;

            case HttpStatusCode.NotFound:
            case HttpStatusCode.Gone:
                responseCodeMessage = "Subscription no longer valid";
                break;
        }

        var details = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var message = string.IsNullOrEmpty(details)
            ? responseCodeMessage
            : $"{responseCodeMessage}. Details: {details}";

        throw new WebPushException(message, subscription, response);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (this._httpClient == null || !this._isHttpClientInternallyCreated)
        {
            return;
        }

        this._httpClient.Dispose();
        this._httpClient = null;
    }
}