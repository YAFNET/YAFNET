/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.Profile;
using YAF.Classes.Core;
using YAF.Providers.Utils;

namespace YAF.Providers.Profile
{
  /// <summary>
  /// YAF Custom Profile Provider
  /// </summary>
  public class YafProfileProvider : ProfileProvider
  {
    /// <summary>
    /// The conn str app key name.
    /// </summary>
    public static string ConnStrAppKeyName = "YafProfileConnectionString";

    /// <summary>
    /// The _app name.
    /// </summary>
    private string _appName;

    /// <summary>
    /// The _conn str name.
    /// </summary>
    private string _connStrName;

    /// <summary>
    /// The _properties setup.
    /// </summary>
    private bool _propertiesSetup = false;

    /// <summary>
    /// The _property lock.
    /// </summary>
    private object _propertyLock = new object();

    /// <summary>
    /// The _settings columns list.
    /// </summary>
    private List<SettingsPropertyColumn> _settingsColumnsList = new List<SettingsPropertyColumn>();

    #region Override Public Properties

    /// <summary>
    /// Gets or sets ApplicationName.
    /// </summary>
    public override string ApplicationName
    {
      get
      {
        return this._appName;
      }

      set
      {
        this._appName = value;
      }
    }

    #endregion

    /// <summary>
    /// Gets UserProfileCache.
    /// </summary>
    private Dictionary<string, SettingsPropertyValueCollection> UserProfileCache
    {
      get
      {
        string key = GenerateCacheKey("UserProfileDictionary");
        return YafContext.Current.Cache.GetItem(key, 999, () => new Dictionary<string, SettingsPropertyValueCollection>());
      }
    }

    /// <summary>
    /// The delete from profile cache if exists.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    private void DeleteFromProfileCacheIfExists(string key)
    {
      if (UserProfileCache.ContainsKey(key))
      {
        UserProfileCache.Remove(key);
      }
    }

    /// <summary>
    /// The clear user profile cache.
    /// </summary>
    private void ClearUserProfileCache()
    {
      YafContext.Current.Cache.Remove(GenerateCacheKey("UserProfileDictionary"));
    }

    /// <summary>
    /// The generate cache key.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The generate cache key.
    /// </returns>
    private string GenerateCacheKey(string name)
    {
      return String.Format("YafProfileProvider-{0}-{1}", name, ApplicationName);
    }

    #region Overriden Public Methods

    /// <summary>
    /// Sets up the profile providers
    /// </summary>
    /// <param name="name">
    /// </param>
    /// <param name="config">
    /// </param>
    public override void Initialize(string name, NameValueCollection config)
    {
      // verify that the configuration section was properly passed
      if (config == null)
      {
        throw new ArgumentNullException("config");
      }

      // Connection String Name
      this._connStrName = Transform.ToString(config["connectionStringName"] ?? String.Empty);

      // application name
      this._appName = config["applicationName"];
      if (string.IsNullOrEmpty(this._appName))
      {
        this._appName = "YetAnotherForum";
      }

      // is the connection string set?
      if (!String.IsNullOrEmpty(this._connStrName))
      {
        string connStr = ConfigurationManager.ConnectionStrings[this._connStrName].ConnectionString;

        // set the app variable...
        if (YafContext.Application[ConnStrAppKeyName] == null)
        {
          YafContext.Application.Add(ConnStrAppKeyName, connStr);
        }
        else
        {
          YafContext.Application[ConnStrAppKeyName] = connStr;
        }
      }

      base.Initialize(name, config);
    }

    /// <summary>
    /// The load from property collection.
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    protected void LoadFromPropertyCollection(SettingsPropertyCollection collection)
    {
      if (!this._propertiesSetup)
      {
        lock (this._propertyLock)
        {
          // clear it out just in case something is still in there...
          this._settingsColumnsList.Clear();

          // validiate all the properties and populate the internal settings collection
          foreach (SettingsProperty property in collection)
          {
            SqlDbType dbType;
            int size;

            // parse custom provider data...
            GetDbTypeAndSizeFromString(property.Attributes["CustomProviderData"].ToString(), out dbType, out size);

            // default the size to 256 if no size is specified
            if (dbType == SqlDbType.NVarChar && size == -1)
            {
              size = 256;
            }

            this._settingsColumnsList.Add(new SettingsPropertyColumn(property, dbType, size));
          }

          // sync profile table structure with the db...
          DataTable structure = DB.Current.GetProfileStructure();

          // verify all the columns are there...
          foreach (SettingsPropertyColumn column in this._settingsColumnsList)
          {
            // see if this column exists
            if (!structure.Columns.Contains(column.Settings.Name))
            {
              // if not, create it...
              DB.Current.AddProfileColumn(column.Settings.Name, column.DataType, column.Size);
            }
          }

          // it's setup now...
          this._propertiesSetup = true;
        }
      }
    }

    /// <summary>
    /// The load from property value collection.
    /// </summary>
    /// <param name="collection">
    /// The collection.
    /// </param>
    protected void LoadFromPropertyValueCollection(SettingsPropertyValueCollection collection)
    {
      if (!this._propertiesSetup)
      {
        // clear it out just in case something is still in there...
        this._settingsColumnsList.Clear();

        // validiate all the properties and populate the internal settings collection
        foreach (SettingsPropertyValue value in collection)
        {
          SqlDbType dbType;
          int size;

          // parse custom provider data...
          GetDbTypeAndSizeFromString(value.Property.Attributes["CustomProviderData"].ToString(), out dbType, out size);

          // default the size to 256 if no size is specified
          if (dbType == SqlDbType.NVarChar && size == -1)
          {
            size = 256;
          }

          this._settingsColumnsList.Add(new SettingsPropertyColumn(value.Property, dbType, size));
        }

        // sync profile table structure with the db...
        DataTable structure = DB.Current.GetProfileStructure();

        // verify all the columns are there...
        foreach (SettingsPropertyColumn column in this._settingsColumnsList)
        {
          // see if this column exists
          if (!structure.Columns.Contains(column.Settings.Name))
          {
            // if not, create it...
            DB.Current.AddProfileColumn(column.Settings.Name, column.DataType, column.Size);
          }
        }

        // it's setup now...
        this._propertiesSetup = true;
      }
    }

    /// <summary>
    /// The get db type and size from string.
    /// </summary>
    /// <param name="providerData">
    /// The provider data.
    /// </param>
    /// <param name="dbType">
    /// The db type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <returns>
    /// The get db type and size from string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// </exception>
    private bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
    {
      size = -1;
      dbType = SqlDbType.NVarChar;

      if (String.IsNullOrEmpty(providerData))
      {
        return false;
      }

      // split the data
      string[] chunk = providerData.Split(
        new[]
          {
            ';'
          });

      // first item is the column name...
      string columnName = chunk[0];

      // get the datatype and ignore case...
      dbType = (SqlDbType) Enum.Parse(typeof (SqlDbType), chunk[1], true);

      if (chunk.Length > 2)
      {
        // handle size...
        if (!Int32.TryParse(chunk[2], out size))
        {
          throw new ArgumentException("Unable to parse as integer: " + chunk[2]);
        }
      }

      return true;
    }

    /// <summary>
    /// The delete inactive profiles.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="userInactiveSinceDate">
    /// The user inactive since date.
    /// </param>
    /// <returns>
    /// The delete inactive profiles.
    /// </returns>
    public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      if (authenticationOption == ProfileAuthenticationOption.Anonymous)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
      }

      // just clear the whole thing...
      ClearUserProfileCache();

      return DB.Current.DeleteInactiveProfiles(ApplicationName, userInactiveSinceDate);
    }

    /// <summary>
    /// The delete profiles.
    /// </summary>
    /// <param name="usernames">
    /// The usernames.
    /// </param>
    /// <returns>
    /// The delete profiles.
    /// </returns>
    public override int DeleteProfiles(string[] usernames)
    {
      if (usernames == null || usernames.Length < 1)
      {
        return 0;
      }

      // make single string of usernames...
      var userNameBuilder = new StringBuilder();
      bool bFirst = true;

      for (int i = 0; i < usernames.Length; i++)
      {
        string username = usernames[i].Trim();

        if (username.Length > 0)
        {
          if (!bFirst)
          {
            userNameBuilder.Append(",");
          }
          else
          {
            bFirst = false;
          }

          userNameBuilder.Append(username);

          // delete this user from the cache if they are in there...
          DeleteFromProfileCacheIfExists(username.ToLower());
        }
      }

      // call the DB...
      return DB.Current.DeleteProfiles(ApplicationName, userNameBuilder.ToString());
    }

    /// <summary>
    /// The delete profiles.
    /// </summary>
    /// <param name="profiles">
    /// The profiles.
    /// </param>
    /// <returns>
    /// The delete profiles.
    /// </returns>
    public override int DeleteProfiles(ProfileInfoCollection profiles)
    {
      if (profiles == null)
      {
        ExceptionReporter.ThrowArgumentNull("PROFILE", "PROFILESNULL");
      }

      if (profiles.Count < 1)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "PROFILESEMPTY");
      }

      var usernames = new string[profiles.Count];

      int index = 0;
      foreach (ProfileInfo profile in profiles)
      {
        usernames[index++] = profile.UserName;
      }

      return DeleteProfiles(usernames);
    }

    /// <summary>
    /// The find inactive profiles by user name.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <param name="userInactiveSinceDate">
    /// The user inactive since date.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override ProfileInfoCollection FindInactiveProfilesByUserName(
      ProfileAuthenticationOption authenticationOption, 
      string usernameToMatch, 
      DateTime userInactiveSinceDate, 
      int pageIndex, 
      int pageSize, 
      out int totalRecords)
    {
      return GetProfileAsCollection(authenticationOption, pageIndex, pageSize, usernameToMatch, userInactiveSinceDate, out totalRecords);
    }

    /// <summary>
    /// The find profiles by user name.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="usernameToMatch">
    /// The username to match.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override ProfileInfoCollection FindProfilesByUserName(
      ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      return GetProfileAsCollection(authenticationOption, pageIndex, pageSize, usernameToMatch, null, out totalRecords);
    }

    /// <summary>
    /// The get all inactive profiles.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="userInactiveSinceDate">
    /// The user inactive since date.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override ProfileInfoCollection GetAllInactiveProfiles(
      ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
    {
      return GetProfileAsCollection(authenticationOption, pageIndex, pageSize, null, userInactiveSinceDate, out totalRecords);
    }

    /// <summary>
    /// The get all profiles.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
    {
      return GetProfileAsCollection(authenticationOption, pageIndex, pageSize, null, null, out totalRecords);
    }

    /// <summary>
    /// The get number of inactive profiles.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="userInactiveSinceDate">
    /// The user inactive since date.
    /// </param>
    /// <returns>
    /// The get number of inactive profiles.
    /// </returns>
    public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
    {
      if (authenticationOption == ProfileAuthenticationOption.Anonymous)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
      }

      return DB.Current.GetNumberInactiveProfiles(ApplicationName, userInactiveSinceDate);
    }

    /// <summary>
    /// The get profile as collection.
    /// </summary>
    /// <param name="authenticationOption">
    /// The authentication option.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="userNameToMatch">
    /// The user name to match.
    /// </param>
    /// <param name="inactiveSinceDate">
    /// The inactive since date.
    /// </param>
    /// <param name="totalRecords">
    /// The total records.
    /// </param>
    /// <returns>
    /// </returns>
    private ProfileInfoCollection GetProfileAsCollection(
      ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, object userNameToMatch, object inactiveSinceDate, out int totalRecords)
    {
      if (authenticationOption == ProfileAuthenticationOption.Anonymous)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
      }

      if (pageIndex < 0)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "PAGEINDEXTOOSMALL");
      }

      if (pageSize < 1)
      {
        ExceptionReporter.ThrowArgument("PROFILE", "PAGESIZETOOSMALL");
      }

      // get all the profiles...
      DataSet allProfilesDS = DB.Current.GetProfiles(ApplicationName, pageIndex, pageSize, userNameToMatch, inactiveSinceDate);

      // create an instance for the profiles...
      var profiles = new ProfileInfoCollection();

      DataTable allProfilesDT = allProfilesDS.Tables[0];
      DataTable profilesCountDT = allProfilesDS.Tables[1];

      foreach (DataRow profileRow in allProfilesDT.Rows)
      {
        string username;
        DateTime lastActivity;
        DateTime lastUpdated = DateTime.UtcNow;

        username = profileRow["Username"].ToString();
        lastActivity = DateTime.SpecifyKind(Convert.ToDateTime(profileRow["LastActivity"]), DateTimeKind.Utc);
        lastUpdated = DateTime.SpecifyKind(Convert.ToDateTime(profileRow["LastUpdated"]), DateTimeKind.Utc);

        profiles.Add(new ProfileInfo(username, false, lastActivity, lastUpdated, 0));
      }

      // get the first record which is the count...
      totalRecords = Convert.ToInt32(profilesCountDT.Rows[0][0]);

      return profiles;
    }

    /// <summary>
    /// The get property values.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="collection">
    /// The collection.
    /// </param>
    /// <returns>
    /// </returns>
    public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
    {
      var settingPropertyCollection = new SettingsPropertyValueCollection();

      if (collection == null || collection.Count < 1 || context == null)
      {
        return settingPropertyCollection;
      }

      string username = context["UserName"].ToString();

      if (String.IsNullOrEmpty(username))
      {
        return settingPropertyCollection;
      }

      // this provider doesn't support anonymous users
      if (!Convert.ToBoolean(context["IsAuthenticated"]))
      {
        ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
      }

      // load the property collection (sync profile class)
      LoadFromPropertyCollection(collection);

      // see if it's cached...
      if (UserProfileCache.ContainsKey(username.ToLower()))
      {
        // just use the cached version...
        return UserProfileCache[username.ToLower()];
      }
      else
      {
        // transfer properties regardless...
        foreach (SettingsProperty prop in collection)
        {
          settingPropertyCollection.Add(new SettingsPropertyValue(prop));
        }

        // get this profile from the DB
        DataSet profileDS = DB.Current.GetProfiles(ApplicationName, 0, 1, username, null);
        DataTable profileDT = profileDS.Tables[0];

        if (profileDT.Rows.Count > 0)
        {
          DataRow row = profileDT.Rows[0];

          // load the data into the collection...
          foreach (SettingsPropertyValue prop in settingPropertyCollection)
          {
            object val = row[prop.Name];

            // Only initialize a SettingsPropertyValue for non-null values
            if (!(val is DBNull || val == null))
            {
              prop.PropertyValue = val;
              prop.IsDirty = false;
              prop.Deserialized = true;
            }
          }
        }

        // save this collection to the cache
        UserProfileCache.Add(username.ToLower(), settingPropertyCollection);
      }

      return settingPropertyCollection;
    }

    /// <summary>
    /// The set property values.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="collection">
    /// The collection.
    /// </param>
    public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
    {
      var username = (string) context["UserName"];

      if (username == null || username.Length < 1 || collection.Count < 1)
      {
        return;
      }

      // this provider doesn't support anonymous users
      if (!Convert.ToBoolean(context["IsAuthenticated"]))
      {
        ExceptionReporter.ThrowArgument("PROFILE", "NOANONYMOUS");
      }

      bool itemsToSave = false;

      // First make sure we have at least one item to save
      foreach (SettingsPropertyValue pp in collection)
      {
        if (pp.IsDirty)
        {
          itemsToSave = true;
          break;
        }
      }

      if (!itemsToSave)
      {
        return;
      }

      // load the data for the configuration
      LoadFromPropertyValueCollection(collection);

      object userID = DB.Current.GetProviderUserKey(ApplicationName, username);
      if (userID != null)
      {
        // start saving...
        DB.Current.SetProfileProperties(ApplicationName, userID, collection, this._settingsColumnsList);

        // erase from the cache
        DeleteFromProfileCacheIfExists(username.ToLower());
      }
    }

    #endregion
  }

  /// <summary>
  /// The settings property column.
  /// </summary>
  public class SettingsPropertyColumn
  {
    /// <summary>
    /// The data type.
    /// </summary>
    public SqlDbType DataType;

    /// <summary>
    /// The settings.
    /// </summary>
    public SettingsProperty Settings;

    /// <summary>
    /// The size.
    /// </summary>
    public int Size;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPropertyColumn"/> class.
    /// </summary>
    public SettingsPropertyColumn()
    {
      // empty for default constructor...
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPropertyColumn"/> class.
    /// </summary>
    /// <param name="settings">
    /// The settings.
    /// </param>
    /// <param name="dataType">
    /// The data type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    public SettingsPropertyColumn(SettingsProperty settings, SqlDbType dataType, int size)
    {
      this.DataType = dataType;
      this.Settings = settings;
      this.Size = size;
    }
  }
}