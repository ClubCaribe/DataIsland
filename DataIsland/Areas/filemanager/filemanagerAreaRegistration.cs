using System.Web.Mvc;

namespace DataIsland.Areas.filemanager
{
    public class filemanagerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "filemanager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                "filemanager_default",
                "filemanager/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}