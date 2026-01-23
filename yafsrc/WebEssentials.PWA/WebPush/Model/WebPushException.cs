using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

public class WebPushException(string message, PushSubscription pushSubscription, HttpResponseMessage responseMessage) : Exception(message)
{
    public HttpStatusCode StatusCode => this.HttpResponseMessage.StatusCode;

    public HttpResponseHeaders Headers => this.HttpResponseMessage.Headers;
    public PushSubscription PushSubscription { get; set; } = pushSubscription;
    public HttpResponseMessage HttpResponseMessage { get; set; } = responseMessage;
}