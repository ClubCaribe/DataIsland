using System.Web.Mvc;

namespace DataIsland.Areas.panel
{
    public class panelAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "panel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "panel_default",
                "panel/{controller}/{action}/{id}",
                new {controller="Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}