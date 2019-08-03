using System.Collections.Generic;
using System.Globalization;
using ServiceStack.Messaging;

namespace ServiceStack
{
    using ServiceStack.Text;

    public static class MqExtensions
    {
        public static Dictionary<string, string> ToHeaders(this IMessage message)
        {
            var map = new Dictionary<string, string>
                {
                    {"CreatedDate",message.CreatedDate.ToString("D")},
                    {"Priority",message.Priority.ToString(CultureInfo.InvariantCulture)},
                    {"RetryAttempts",message.RetryAttempts.ToString(CultureInfo.InvariantCulture)},
                    {"ReplyId",message.ReplyId?.ToString()},
                    {"ReplyTo",message.ReplyTo},
                    {"Options",message.Options.ToString(CultureInfo.InvariantCulture)},
                    {"Error",message.Error.Dump()},
                };
            return map;
        }
    }
}