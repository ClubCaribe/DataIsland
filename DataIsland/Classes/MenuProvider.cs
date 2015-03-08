using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataIsland.Classes
{
    public class MenuProvider
    {
        public static List<MenuEntry> GetDiAdminMenu()
        {
            List<MenuEntry> menuEntries = new List<MenuEntry>();

            MenuEntry settings = new MenuEntry();
            settings.IconClass = "entypo-window";
            settings.Link = "/panel/dataisland/settings";
            settings.Name = "[tr]Settings[/tr]";

            menuEntries.Add(settings);

            MenuEntry users = new MenuEntry();
            users.IconClass = "entypo-users";
            users.Link = "/panel/dataisland/users";
            users.Name = "[tr]Users[/tr]";

            menuEntries.Add(users);

            return menuEntries;
        }
    }
}