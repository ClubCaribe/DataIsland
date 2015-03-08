using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataislandcommon.Utilities
{
    public class DiConsts
    {
        //data model
        public const string DataIslandSate = "dataislandstate";
        public const string UserSessions = "userssessions";
        public const string DatabaseSettings = "databasesettings";

        //placeholders
        public const string DbSettingsFileNamePlaceholder = "[filename]";
        public const string DbSettingsDatabaseNamePlaceholder = "[databasename]";

        //settings
        public const string PublicKey = "publickey";
        public const string PrivateKey = "privatekey";
        public const string DataIslandID = "dataislandid";
        public const string DataIslandPassword = "dataislandpassword";
        public const string DataIslandName = "dataislandname";
        public const string DataIslandDomain = "dataislanddomain";
        public const string DataIslandWebPage = "dataislandwebpage";
        public const string DataIslandAdminEmail = "dataislandadminemail";
        public const string DataIslandIsPublic = "dataislandispublic";
        public const string DataIslandIsInitialised = "dataislandisinitialised";
        public const string DataIslandDescription = "dataislanddescription";
        public const string DatabaseMainDatabaseConnectionName = "maindatabase";
        public const string DataIslandSessionTimeOut = "dataisandsessiontimeout";


        //user account
        public static readonly List<string> UserAccountPersonalSettings = new List<string> { "Birthdate", "Gender", "Country", "City" };


        //common
        public const string OK = "ok";
        public const string Cancel = "cancel";
        public const string Error = "error";

        //roles
        public const string RoleUser = "user";
        public const string RoleAdmin = "admin";
        public const string RoleAll = "all";

        public const string DefaultAvatarPath = "~/Content/panel/images/profile-picture.png";

        public static MemoryCacheConsts MemoryCache = new MemoryCacheConsts();
        public static IconsConsts IconsCssClasses = new IconsConsts();
    }
}
