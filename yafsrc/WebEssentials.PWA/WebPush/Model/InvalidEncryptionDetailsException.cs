using System;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

/// <summary>
/// 
/// </summary>
/// <seealso cref="System.Exception" />
public class InvalidEncryptionDetailsException(string message, PushSubscription pushSubscription) : Exception(message)
{
    /// <summary>
    /// Gets the push subscription.
    /// </summary>
    /// <value>
    /// The push subscription.
    /// </value>
    public PushSubscription PushSubscription { get; } = pushSubscription;
}