using System;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

public class InvalidEncryptionDetailsException(string message, PushSubscription pushSubscription) : Exception(message)
{
    public PushSubscription PushSubscription { get; } = pushSubscription;
}