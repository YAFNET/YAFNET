using System;
using System.Web;
using YAF.Classes.Data;
using YAF.Types.Constants;

namespace YAF.Utils.Helpers
{
    public class UserAgentLogger
    {
        public void WriteLog(string userAgent, HttpRequestBase request, string platform, string browser, string user)
        {
            if (request == null) return;

            if (userAgent.IsNotSet())
            {
                LegacyDb.eventlog_create(null, this, "UserAgent string is empty.", EventLogTypes.Warning);
            }
            else
            {
                if (request.Browser != null && platform.ToLower().Contains("unknown") ||
                    browser.ToLower().Contains("unknown"))
                {
                    LegacyDb.eventlog_create(
                        null,
                        this,
                        "Unhandled UserAgent string:'{0}' /r/nPlatform:'{1}' /r/nBrowser:'{2}' /r/nSupports cookies='{3}' /r/nSupports EcmaScript='{4}' /r/nUserID='{5}'."
                            .FormatWith(
                                userAgent,
                                request.Browser.Platform,
                                request.Browser.Browser,
                                request.Browser.Cookies,
                                request.Browser.EcmaScriptVersion.ToString(),
                                user ?? String.Empty),
                        EventLogTypes.Warning);
                }
            }
        }
    }
}
