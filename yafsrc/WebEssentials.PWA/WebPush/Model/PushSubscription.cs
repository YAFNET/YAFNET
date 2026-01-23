namespace WebEssentials.AspNetCore.Pwa.WebPush.Model;

public class PushSubscription
{
    public PushSubscription()
    {
    }

    public PushSubscription(string endpoint, string p256dh, string auth)
    {
        this.Endpoint = endpoint;
        this.P256DH = p256dh;
        this.Auth = auth;
    }

    public string Endpoint { get; set; }
    public string P256DH { get; set; }
    public string Auth { get; set; }
}