using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace YAF.Providers.Membership
{
    public class Security
    {
        public enum MembershipPasswordFormat
        {
            // Summary:
            //     Passwords are not encrypted.
            Clear = 0,
            //
            // Summary:
            //     Passwords are encrypted one-way using the MD5 hashing algorithm.
            MD5Hashed = 1,
            //
            // Summary:
            //     Passwords are encrypted one-way using the SHA1 hashing algorithm.
            SHA1Hashed = 2,
            //
            // Summary:
            //     Passwords are encrypted one-way using the SHA1 hashing algorithm.
            SHA2Hashed = 3,
            //
            // Summary:
            //     Passwords are encrypted using the encryption settings determined by the machineKey
            //     element configuration.
            Encrypted = 4,
        }

    }
}
