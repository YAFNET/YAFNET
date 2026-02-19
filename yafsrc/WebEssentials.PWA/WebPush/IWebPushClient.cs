using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using WebEssentials.AspNetCore.Pwa.WebPush.Model;

namespace WebEssentials.AspNetCore.Pwa.WebPush;

/// <summary>
/// 
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IWebPushClient : IDisposable
{
    /// <summary>
    ///     When sending messages to a GCM endpoint you need to set the GCM API key
    ///     by either calling setGcmApiKey() or passing in the API key as an option
    ///     to sendNotification()
    /// </summary>
    /// <param name="gcmApiKey">The API key to send with the GCM request.</param>
    void SetGcmApiKey(string gcmApiKey);

    /// <summary>
    ///     When marking requests where you want to define VAPID details, call this method
    ///     before sendNotifications() or pass in the details and options to
    ///     sendNotification.
    /// </summary>
    /// <param name="vapidDetails"></param>
    void SetVapidDetails(VapidDetails vapidDetails);

    /// <summary>
    ///     When marking requests where you want to define VAPID details, call this method
    ///     before sendNotifications() or pass in the details and options to
    ///     sendNotification.
    /// </summary>
    /// <param name="subject">This must be either a URL or a 'mailto:' address</param>
    /// <param name="publicKey">The public VAPID key as a base64 encoded string</param>
    /// <param name="privateKey">The private VAPID key as a base64 encoded string</param>
    void SetVapidDetails(string subject, string publicKey, string privateKey);

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
    HttpRequestMessage GenerateRequestDetails(PushSubscription subscription, string payload,
        Dictionary<string, object> options = null);

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
    void SendNotification(PushSubscription subscription, string payload = null,
        Dictionary<string, object> options = null);

    /// <summary>
    ///     To send a push notification call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="vapidDetails">The vapid details for the notification.</param>
    void SendNotification(PushSubscription subscription, string payload, VapidDetails vapidDetails);

    /// <summary>
    ///     To send a push notification call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="gcmApiKey">The GCM API key</param>
    void SendNotification(PushSubscription subscription, string payload, string gcmApiKey);

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
    Task SendNotificationAsync(PushSubscription subscription, string payload = null,
        Dictionary<string, object> options = null, CancellationToken cancellationToken=default);

    /// <summary>
    ///     To send a push notification asynchronous call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="vapidDetails">The vapid details for the notification.</param>
    /// <param name="cancellationToken"></param>
    Task SendNotificationAsync(PushSubscription subscription, string payload,
        VapidDetails vapidDetails, CancellationToken cancellationToken=default);

    /// <summary>
    ///     To send a push notification asynchronous call this method with a subscription, optional payload and any options
    ///     Will exception if unsuccessful
    /// </summary>
    /// <param name="subscription">The PushSubscription you wish to send the notification to.</param>
    /// <param name="payload">The payload you wish to send to the user</param>
    /// <param name="gcmApiKey">The GCM API key</param>
    /// <param name="cancellationToken"></param>
    Task SendNotificationAsync(PushSubscription subscription, string payload, string gcmApiKey, CancellationToken cancellationToken=default);
}