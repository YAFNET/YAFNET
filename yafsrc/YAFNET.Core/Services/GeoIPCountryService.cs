/* GeoIPCountry.cs
 * Legal Notice:
 * Copyright (C) 2008 MaxMind, Inc.  All Rights Reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Buffers.Binary;
using System.IO;

using Microsoft.AspNetCore.Hosting;

using YAF.Types.Objects;

namespace YAF.Core.Services;

/// <summary>
/// This code is based on MaxMind's original C# code, which was ported from Java.
/// This version is very simplified and does not support a majority of features for speed.
/// </summary>
public class GeoIpCountryService : IGeoIpCountryService, IHaveServiceLocator
{
    /// <summary>
    /// The size of a record (two 3 byte pointers) in the database.
    /// </summary>
    private const int RecordLength = 6;

    /// <summary>
    /// The geo data, the cached database is shared by all requests, it is only read (no stream position).
    /// </summary>
    private readonly byte[] geoData;

    /// <summary>
    /// hard coded position of where country data starts in the data file.
    /// </summary>
    private const long CountryBegin = 16776960;

    /// <summary>
    /// The GeoIP database name. This is the combined MaxMind GeoIP Legacy database
    /// (COUNTRY_EDITION_V6) which contains both IPv4 and IPv6 data.
    /// </summary>
    private const string DatabaseName = "GeoIP.dat";

    /// <summary>
    /// The country codes
    /// </summary>
    private readonly static string[] CountryCodes =
    [
        "--", "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AN", "AO", "AQ", "AR", "AS",
        "AT", "AU", "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BM", "BN",
        "BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI",
        "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM",
        "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR",
        "FX", "GA", "GB", "GD", "GE", "GF", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS",
        "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IN", "IO",
        "IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE", "KG", "KH", "KI", "KM", "KN", "KP", "KR",
        "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA",
        "MC", "MD", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU",
        "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR",
        "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT",
        "PW", "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI",
        "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SV", "SY", "SZ", "TC", "TD", "TF", "TG",
        "TH", "TJ", "TK", "TM", "TN", "TO", "TL", "TR", "TT", "TV", "TW", "TZ", "UA", "UG", "UM",
        "US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "RS",
        "ZA", "ZM", "ME", "ZW", "A1", "A2", "O1", "AX", "GG", "IM", "JE", "BL", "MF"
    ];

    /// <summary>
    /// List of countries names
    /// </summary>
    private readonly static string[] CountryNames =
    [
        "N/A","Asia/Pacific Region","Europe","Andorra","United Arab Emirates","Afghanistan",
            "Antigua and Barbuda","Anguilla","Albania","Armenia","Netherlands Antilles","Angola",
            "Antarctica","Argentina","American Samoa","Austria","Australia","Aruba","Azerbaijan",
            "Bosnia and Herzegovina","Barbados","Bangladesh","Belgium","Burkina Faso","Bulgaria",
            "Bahrain","Burundi","Benin","Bermuda","Brunei Darussalam","Bolivia","Brazil","Bahamas",
            "Bhutan","Bouvet Island","Botswana","Belarus","Belize","Canada","Cocos (Keeling) Islands",
            "Congo, The Democratic Republic of the","Central African Republic","Congo","Switzerland",
            "Cote D'Ivoire","Cook Islands","Chile","Cameroon","China","Colombia","Costa Rica","Cuba",
            "Cape Verde","Chrismas Island","Cyprus","Czech Republic","Germany","Djibouti","Denmark",
            "Dominica","Dominican Republic","Algeria","Ecuador","Estonia","Egypt","Western Sahara",
            "Eritrea","Spain","Ethiopia","Finland","Fiji","Falkland Islands (Malvinas)",
            "Micronesia, Federated States of","Faroe Islands","France","France, Metropolitan","Gabon",
            "United Kingdom","Grenada","Georgia","French Guiana","Ghana","Gibraltar","Greenland",
            "Gambia","Guinea","Guadeloupe","Equatorial Guinea","Greece",
            "South Georgia and the South Sandwich Islands","Guatemala","Guam","Guinea-Bissau","Guyana",
            "Hong Kong","Heard Island and McDonald Islands","Honduras","Croatia","Haiti","Hungary",
            "Indonesia","Ireland","Israel","India","British Indian Ocean Territory","Iraq",
            "Iran, Islamic Republic of","Iceland","Italy","Jamaica","Jordan","Japan","Kenya",
            "Kyrgyzstan","Cambodia","Kiribati","Comoros","Kitts and Nevis",
            "Korea, Democratic People's Republic of","Korea, Republic of","Kuwait","Cayman Islands",
            "Kazakstan","Lao People's Democratic Republic","Lebanon","Lucia","Liechtenstein",
            "Sri Lanka","Liberia","Lesotho","Lithuania","Luxembourg","Latvia","Libyan Arab Jamahiriya",
            "Morocco","Monaco","Moldova, Republic of","Madagascar","Marshall Islands","Macedonia",
            "Mali","Myanmar","Mongolia","Macau","Northern Mariana Islands","Martinique","Mauritania",
            "Montserrat","Malta","Mauritius","Maldives","Malawi","Mexico","Malaysia","Mozambique",
            "Namibia","New Caledonia","Niger","Norfolk Island","Nigeria","Nicaragua","Netherlands",
            "Norway","Nepal","Nauru","Niue","New Zealand","Oman","Panama","Peru","French Polynesia",
            "Papua New Guinea","Philippines","Pakistan","Poland","Pierre and Miquelon",
            "Pitcairn Islands","Puerto Rico","Palestinian Territory","Portugal","Palau","Paraguay",
            "Qatar","Reunion","Romania","Russian Federation","Rwanda","Saudi Arabia",
            "Solomon Islands","Seychelles","Sudan","Sweden","Singapore","Helena","Slovenia",
            "Svalbard and Jan Mayen","Slovakia","Sierra Leone","San Marino","Senegal","Somalia",
            "Suriname","Sao Tome and Principe","El Salvador","Syrian Arab Republic","Swaziland",
            "Turks and Caicos Islands","Chad","French Southern Territories","Togo","Thailand",
            "Tajikistan","Tokelau","Turkmenistan","Tunisia","Tonga","Timor-Leste","Turkey",
            "Trinidad and Tobago","Tuvalu","Taiwan","Tanzania, United Republic of","Ukraine","Uganda",
            "United States Minor Outlying Islands","United States","Uruguay","Uzbekistan",
            "Vatican City State","Vincent and the Grenadines","Venezuela",
            "Virgin Islands, British","Virgin Islands, U.S.","Vietnam","Vanuatu","Wallis and Futuna",
            "Samoa","Yemen","Mayotte","Serbia","South Africa","Zambia","Montenegro","Zimbabwe",
            "Anonymous Proxy","Satellite Provider","Other","Aland Islands","Guernsey","Isle of Man",
            "Jersey","Barthelemy","Martin"
    ];

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoIpCountryService"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    /// <exception cref="ArgumentException">File does not exist!</exception>
    public GeoIpCountryService(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;

        this.geoData = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.GeoIpData,
            this.GeoDataFileToMemory);
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Checks if the Database Exists
    /// </summary>
    /// <returns>System.Boolean.</returns>
    public bool DatabaseExists()
    {
        return this.Get<IDataCache>().GetOrSet(
            Constants.Cache.GeoIpDataCheck,
            this.CheckDatabaseFileExists);
    }

    /// <summary>
    /// Gets the name of the country from the ip address.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns>The country name and code</returns>
    public GeoIpData GetCountry(string ipAddress)
    {
        IPAddress address;
        try
        {
            address = IPAddress.Parse(ipAddress);
        }
        catch (FormatException)
        {
            return new GeoIpData
            {
                CountryName = "N/A",
                CountryCode = "--"
            };
        }

        return this.GetCountry(address);
    }

    /// <summary>
    /// Gets the name of the country from the ip address.
    /// </summary>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns>The country name and code</returns>
    public GeoIpData GetCountry(IPAddress ipAddress)
    {
        var index = this.FindIndex(ipAddress);

        var data = new GeoIpData
        {
            CountryName = CountryNames[index],
            CountryCode = CountryCodes[index]
        };

        return data;
    }

    /// <summary>
    /// Checks if the database file exists.
    /// </summary>
    /// <returns>System.Boolean.</returns>
    private bool CheckDatabaseFileExists()
    {
        var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, DatabaseName);

        var fi = new FileInfo(path);

        return fi.Exists;
    }

    /// <summary>
    /// Loads the Geo data file in to memory.
    /// </summary>
    /// <returns>The geo database file (empty if the file does not exist)</returns>
    private byte[] GeoDataFileToMemory()
    {
        var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, DatabaseName);

        var fi = new FileInfo(path);

        return fi.Exists ? File.ReadAllBytes(fi.FullName) : [];
    }

    private int FindIndex(IPAddress ip)
    {
        var index = this.SeekCountry(AddressToUInt128(ip));

        // a missing or damaged database gives no country
        return index < CountryCodes.Length ? Convert.ToInt32(index) : 0;
    }

    /// <summary>
    /// Converts an IPv4 or IPv6 address into its 128-bit numeric equivalent, for reading
    /// from the combined IPv4/IPv6 geo database. IPv4 addresses are mapped to their
    /// IPv4-mapped IPv6 representation (::ffff:a.b.c.d), which is how the database stores them.
    /// </summary>
    /// <param name="ip">The ip.</param>
    /// <returns>System.UInt128.</returns>
    private static UInt128 AddressToUInt128(IPAddress ip)
    {
        var bytes = ip.MapToIPv6().GetAddressBytes();

        var upper = BinaryPrimitives.ReadUInt64BigEndian(bytes.AsSpan(0, 8));
        var lower = BinaryPrimitives.ReadUInt64BigEndian(bytes.AsSpan(8, 8));

        return new UInt128(upper, lower);
    }

    /// <summary>
    /// Traverses the GeoIP binary data looking for a country code based
    /// on the IP address mask
    /// </summary>
    /// <param name="ipNumber">The ip number.</param>
    /// <returns>The index of the country (0 if the database has no entry).</returns>
    private long SeekCountry(UInt128 ipNumber)
    {
        long offset = 0;

        for (var depth = 127; depth >= 0; depth--)
        {
            var position = RecordLength * offset;

            if (position + RecordLength > this.geoData.Length)
            {
                return 0;
            }

            // each record holds the pointers for bit 0 and bit 1 (3 bytes little endian each)
            var start = (int)position + ((ipNumber & (UInt128.One << depth)) > 0 ? 3 : 0);

            long next = this.geoData[start] | this.geoData[start + 1] << 8 | this.geoData[start + 2] << 16;

            if (next >= CountryBegin)
            {
                return next - CountryBegin;
            }

            offset = next;
        }

        throw new IOException("Cannot seek GeoIP database");
    }
}