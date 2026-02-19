using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

/// <summary>
/// 
/// </summary>
/// <seealso cref="System.Exception" />
public class WebPushException(string message, PushSubscription pushSubscription, HttpResponseMessage responseMessage) : Exception(message)
{
    /// <summary>
    /// Gets the status code.
    /// </summary>
    /// <value>
    /// The status code.
    /// </value>
    public HttpStatusCode StatusCode => this.HttpResponseMessage.StatusCode;

    /// <summary>
    /// Gets the headers.
    /// </summary>
    /// <value>
    /// The headers.
    /// </value>
    public HttpResponseHeaders Headers => this.HttpResponseMessage.Headers;

    /// <summary>
    /// Gets or sets the push subscription.
    /// </summary>
    /// <value>
    /// The push subscription.
    /// </value>
    public PushSubscription PushSubscription { get; set; } = pushSubscription;

    /// <summary>
    /// Gets or sets the HTTP response message.
    /// </summary>
    /// <value>
    /// The HTTP response message.
    /// </value>
    public HttpResponseMessage HttpResponseMessage { get; set; } = responseMessage;
}