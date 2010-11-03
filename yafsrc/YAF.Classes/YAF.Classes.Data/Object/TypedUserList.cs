using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAF.Classes.Data
{
  using System.Data;

  public class TypedUserList
  {
    public int? UserID { get; set; }
    public int? BoardID { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime? Joined { get; set; }
    public DateTime? LastVisit { get; set; }
    public string IP { get; set; }
    public int? NumPosts { get; set; }
    public int? TimeZone { get; set; }
    public string Avatar { get; set; }
    public string Signature { get; set; }
    public byte[] AvatarImage { get; set; }
    public int? RankID { get; set; }
    public DateTime? Suspended { get; set; }
    public string LanguageFile { get; set; }
    public string ThemeFile { get; set; }
    public int? Flags { get; set; }
    public bool? PMNotification { get; set; }
    public int? Points { get; set; }
    public bool? IsAdmin { get; set; }
    public bool? IsApproved { get; set; }
    public bool? IsActiveExcluded { get; set; }
    public string ProviderUserKey { get; set; }
    public bool? OverrideDefaultThemes { get; set; }
    public string AvatarImageType { get; set; }
    public bool? AutoWatchTopics { get; set; }
    public string DisplayName { get; set; }
    public string Culture { get; set; }
    public int? NotificationType { get; set; }
    public bool? DailyDigest { get; set; }
    public int? NumPosts1 { get; set; }
    public string CultureUser { get; set; }
    public string Style { get; set; }
    public int? IsAdmin1 { get; set; }
    public int? IsGuest { get; set; }
    public int? IsHostAdmin { get; set; }
    public int? RankID1 { get; set; }
    public string RankName { get; set; }

    public TypedUserList(DataRow row)
    {
      this.UserID = row.Field<int?>("UserID");
      this.BoardID = row.Field<int?>("BoardID");
      this.Name = row.Field<string>("Name");
      this.Password = row.Field<string>("Password");
      this.Email = row.Field<string>("Email");
      this.Joined = row.Field<DateTime?>("Joined");
      this.LastVisit = row.Field<DateTime?>("LastVisit");
      this.IP = row.Field<string>("IP");
      this.NumPosts = row.Field<int?>("NumPosts");
      this.TimeZone = row.Field<int?>("TimeZone");
      this.Avatar = row.Field<string>("Avatar");
      this.Signature = row.Field<string>("Signature");
      this.AvatarImage = row.Field<byte[]>("AvatarImage");
      this.RankID = row.Field<int?>("RankID");
      this.Suspended = row.Field<DateTime?>("Suspended");
      this.LanguageFile = row.Field<string>("LanguageFile");
      this.ThemeFile = row.Field<string>("ThemeFile");
      this.Flags = row.Field<int?>("Flags");
      this.PMNotification = row.Field<bool?>("PMNotification");
      this.Points = row.Field<int?>("Points");
      this.IsAdmin = row.Field<bool?>("IsAdmin");
      this.IsApproved = row.Field<bool?>("IsApproved");
      this.IsActiveExcluded = row.Field<bool?>("IsActiveExcluded");
      this.ProviderUserKey = row.Field<string>("ProviderUserKey");
      this.OverrideDefaultThemes = row.Field<bool?>("OverrideDefaultThemes");
      this.AvatarImageType = row.Field<string>("AvatarImageType");
      this.AutoWatchTopics = row.Field<bool?>("AutoWatchTopics");
      this.DisplayName = row.Field<string>("DisplayName");
      this.Culture = row.Field<string>("Culture");
      this.NotificationType = row.Field<int?>("NotificationType");
      this.DailyDigest = row.Field<bool?>("DailyDigest");
      this.NumPosts1 = row.Field<int?>("NumPosts1");
      this.CultureUser = row.Field<string>("CultureUser");
      this.Style = row.Field<string>("Style");
      this.IsAdmin1 = row.Field<int?>("IsAdmin1");
      this.IsGuest = row.Field<int?>("IsGuest");
      this.IsHostAdmin = row.Field<int?>("IsHostAdmin");
      this.RankID1 = row.Field<int?>("RankID1");
      this.RankName = row.Field<string>("RankName");
    }

    public TypedUserList(int? userid, int? boardid, string name, string password, string email, DateTime? joined, DateTime? lastvisit, string ip, int? numposts, int? timezone, string avatar, string signature, byte[] avatarimage, int? rankid, DateTime? suspended, string languagefile, string themefile, int? flags, bool? pmnotification, int? points, bool? isadmin, bool? isapproved, bool? isactiveexcluded, string provideruserkey, bool? overridedefaultthemes, string avatarimagetype, bool? autowatchtopics, string displayname, string culture, int? notificationtype, bool? dailydigest, int? numposts1, string cultureuser, string style, int? isadmin1, int? isguest, int? ishostadmin, int? rankid1, string rankname)
    {
      this.UserID = userid;
      this.BoardID = boardid;
      this.Name = name;
      this.Password = password;
      this.Email = email;
      this.Joined = joined;
      this.LastVisit = lastvisit;
      this.IP = ip;
      this.NumPosts = numposts;
      this.TimeZone = timezone;
      this.Avatar = avatar;
      this.Signature = signature;
      this.AvatarImage = avatarimage;
      this.RankID = rankid;
      this.Suspended = suspended;
      this.LanguageFile = languagefile;
      this.ThemeFile = themefile;
      this.Flags = flags;
      this.PMNotification = pmnotification;
      this.Points = points;
      this.IsAdmin = isadmin;
      this.IsApproved = isapproved;
      this.IsActiveExcluded = isactiveexcluded;
      this.ProviderUserKey = provideruserkey;
      this.OverrideDefaultThemes = overridedefaultthemes;
      this.AvatarImageType = avatarimagetype;
      this.AutoWatchTopics = autowatchtopics;
      this.DisplayName = displayname;
      this.Culture = culture;
      this.NotificationType = notificationtype;
      this.DailyDigest = dailydigest;
      this.NumPosts1 = numposts1;
      this.CultureUser = cultureuser;
      this.Style = style;
      this.IsAdmin1 = isadmin1;
      this.IsGuest = isguest;
      this.IsHostAdmin = ishostadmin;
      this.RankID1 = rankid1;
      this.RankName = rankname;
    }
  } 
}
