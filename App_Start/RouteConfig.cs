using System.Web.Mvc;
using System.Web.Routing;

namespace U_Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GirisYap",
                url: "giris-yap",
                defaults: new { controller = "Sayfalar", action = "Giris" }
            );
            routes.MapRoute(
              name: "AnaSayfa",
              url: "ana-sayfa",
              defaults: new { controller = "Sayfalar", action = "AnaSayfa" }
          );
       //     routes.MapRoute(
       //    name: "BeklemedeTalepler",
       //    url: "beklemede-talepler",
       //    defaults: new { controller = "Sayfalar", action = "BeklemedeTalepler" }
       //);
       //     routes.MapRoute(
       //    name: "BeklemedeTeklifler",
       //    url: "beklemede-teklifler",
       //    defaults: new { controller = "Sayfalar", action = "BeklemedeTeklifler" }
       //);
            routes.MapRoute(
             name: "BeklemedeOnaylananTalepler",
             url: "beklemede-onaylanan-talepler",
             defaults: new { controller = "Sayfalar", action = "BeklemedeOnaylananTalepler" }
         );
            routes.MapRoute(
            name: "ReddedilenTalepler",
            url: "reddedilen-talepler",
            defaults: new { controller = "Sayfalar", action = "ReddedilenTalepler" }
        );
            routes.MapRoute(
          name: "MalzemeCikis",
          url: "malzeme-cikis",
          defaults: new { controller = "Sayfalar", action = "MalzemeCikis" }
      ); 
            routes.MapRoute(
          name: "StokKullan",
          url: "stok-kullan",
          defaults: new { controller = "Sayfalar", action = "StokKullan" }
      );
                    routes.MapRoute(
               name: "StokDurum",
               url: "stok-durum",
               defaults: new { controller = "Sayfalar", action = "StokDurum" }
           );
            routes.MapRoute(
             name: "BeklemedeOnaylananTeklifler",
             url: "beklemede-onaylanan-teklifler",
             defaults: new { controller = "Sayfalar", action = "BeklemedeOnaylananTeklifler" }
         );
            routes.MapRoute(
          name: "ReddedilenTeklifler",
          url: "reddedilen-teklifler",
          defaults: new { controller = "Sayfalar", action = "ReddedilenTeklifler" }
      );
            routes.MapRoute(
            name: "Profil",
            url: "profil",
            defaults: new { controller = "Sayfalar", action = "Profil" }
        );
            routes.MapRoute(
           name: "Siparisler",
           url: "siparisler",
           defaults: new { controller = "Sayfalar", action = "Siparisler" }
       );
            routes.MapRoute(
           name: "TalepOlustur",
           url: "talep-olustur",
           defaults: new { controller = "Sayfalar", action = "TalepOlustur" }
       );
            routes.MapRoute(
        name: "Raporlar",
        url: "raporlar",
        defaults: new { controller = "Sayfalar", action = "Raporlar" }
    );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Sayfalar", action = "Giris", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "API",
                url: "api/{action}",
                defaults: new { controller = "API", action = "Logout" }
            );
        }
    }
}