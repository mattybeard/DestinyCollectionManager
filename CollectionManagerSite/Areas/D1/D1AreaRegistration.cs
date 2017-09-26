using System.Web.Mvc;

namespace CollectionManagerSite.Areas.D1
{
    public class D1AreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "D1";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "D1_default",
                "D1/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}