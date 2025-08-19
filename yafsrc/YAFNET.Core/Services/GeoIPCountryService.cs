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
using System.IO;
using System.Threading;

using Microsoft.AspNetCore.Hosting;

using YAF.Types.Objects;

namespace YAF.Core.Services;

/// <summary>
/// This code is based on MaxMind's original C# code, which was ported from Java.
/// This version is very simplified and does not support a majority of features for speed.
/// </summary>
public class GeoIpCountryService : IDisposable, IGeoIpCountryService, IHaveServiceLocator
{
    /// <summary>
    /// The geo data
    /// </summary>
    private readonly MemoryStream geoData;

    /// <summary>
    /// hard coded position of where country data starts in the data file.
    /// </summary>
    private const long CountryBegin = 16776960;

    /// <summary>
    /// The GeoIP database name
    /// </summary>
    private const string DatabaseName = "GeoIP.dat";

    private readonly Lock myLock = new();

    /// <summary>
    /// The country codes
    /// </summary>
    private readonly static string[] CountryCodes =
    [
        "--","AP","EU","AD","AE","AF","AG","AI","AL","AM","AN","AO","AQ","AR","AS",
        "AT","AU","AW","AZ","BA","BB","BD","BE","BF","BG","BH","BI","BJ","BM","BN",
        "BO","BR","BS","BT","BV","BW","BY","BZ","CA","CC","CD","CF","CG","CH","CI",
        "CK","CL","CM","CN","CO","CR","CU","CV","CX","CY","CZ","DE","DJ","DK","DM",
        "DO","DZ","EC","EE","EG","EH","ER","ES","ET","FI","FJ","FK","FM","FO","FR",
        "FX","GA","GB","GD","GE","GF","GH","GI","GL","GM","GN","GP","GQ","GR","GS",
        "GT","GU","GW","GY","HK","HM","HN","HR","HT","HU","ID","IE","IL","IN","IO",
        "IQ","IR","IS","IT","JM","JO","JP","KE","KG","KH","KI","KM","KN","KP","KR",
        "KW","KY","KZ","LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY","MA",
        "MC","MD","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU",
        "MV","MW","MX","MY","MZ","NA","NC","NE","NF","NG","NI","NL","NO","NP","NR",
        "NU","NZ","OM","PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT",
        "PW","PY","QA","RE","RO","RU","RW","SA","SB","SC","SD","SE","SG","SH","SI",
        "SJ","SK","SL","SM","SN","SO","SR","ST","SV","SY","SZ","TC","TD","TF","TG",
        "TH","TJ","TK","TM","TN","TO","TL","TR","TT","TV","TW","TZ","UA","UG","UM",
        "US","UY","UZ","VA","VC","VE","VG","VI","VN","VU","WF","WS","YE","YT","RS",
        "ZA","ZM","ME","ZW","A1","A2","O1","AX","GG","IM","JE","BL","MF"
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
            address = IPAddress.Parse(ipAddress).MapToIPv4();
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
    /// Loads the Geo data file in to a memory stream.
    /// </summary>
    /// <returns>MemoryStream of the GeoIP.dat file</returns>
    private MemoryStream GeoDataFileToMemory()
    {
        var stream = new MemoryStream();

        var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, DatabaseName);

        var fi = new FileInfo(path);

        if (fi.Exists)
        {
            stream = new MemoryStream(File.ReadAllBytes(fi.FullName));
        }

        return stream;
    }

    private int FindIndex(IPAddress ip)
    {
        return Convert.ToInt32(this.SeekCountry(0, AddressToLong(ip), 31));
    }

    /// <summary>
    /// Converts an IPv4 address into a long, for reading from geo database
    /// </summary>
    /// <param name="ip">The ip.</param>
    /// <returns>System.Int64.</returns>
    private static long AddressToLong(IPAddress ip)
    {
        // Convert an IP Address, (e.g. 127.0.0.1), to the numeric equivalent
        var address = ip.ToString().Split('.');
        return address.Length == 4
            ? Convert.ToInt64(16777216 * Convert.ToDouble(address[0]) + 65536 * Convert.ToDouble(address[1]) +
                              256 * Convert.ToDouble(address[2]) + Convert.ToDouble(address[3]))
            : 0;
    }

    /// <summary>
    /// Traverses the GeoIP binary data looking for a country code based
    /// on the IP address mask
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="ipNumber">The ip number.</param>
    /// <param name="depth">The depth.</param>
    /// <returns>System.Int64.</returns>
    private long SeekCountry(long offset, long ipNumber, int depth)
    {
        lock (this.myLock)
        {
            var buffer = new byte[6]; // 2 * MAX_RECORD_LENGTH
            var x = new long[2];
            if (depth < 0)
            {
                throw new IOException("Cannot seek GeoIP database");
            }

            this.geoData.Seek(6 * offset, SeekOrigin.Begin);
            this.geoData.ReadExactly(buffer, 0, 6);

            for (var i = 0; i < 2; i++)
            {
                x[i] = 0;
                for (var j = 0; j < 3; j++)
                {
                    int y = buffer[i * 3 + j];

                    x[i] += y << (j * 8);
                }
            }

            if ((ipNumber & (1 << depth)) > 0)
            {
                if (x[1] >= CountryBegin)
                {
                    return x[1] - CountryBegin;
                }

                return this.SeekCountry(x[1], ipNumber, depth - 1);
            }

            if (x[0] >= CountryBegin)
            {
                return x[0] - CountryBegin;
            }

            return this.SeekCountry(x[0], ipNumber, depth - 1);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        lock (this.myLock)
        {
            this.geoData.Dispose();
        }
    }
}