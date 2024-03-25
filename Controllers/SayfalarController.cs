using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using U_Project.Models;

namespace U_Project.Controllers
{
    [Authorize]
    public class SayfalarController : System.Web.Mvc.Controller
    {
        // GET: Hesap
        [AllowAnonymous]
        [Route("giris-yap")]
        public ActionResult Giris()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return Redirect("/ana-sayfa");
            //}
            //else
            //{
                return View();
            //}
        }

        [Route("profil")]
        public ActionResult Profil()
        {
            return View();
        }

        [Route("ana-sayfa")]
        public async Task<ActionResult> AnaSayfa()
        {
            using (U_Entities db = new U_Entities())
            {
                var userId = Convert.ToInt16(User.Identity.Name);
                var userSube = db.TBL_KULLANICI.Where(x => x.ID == userId).Select(z => z.SUBE_ID).FirstOrDefault();
                string sql = @"SELECT
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_TALEP WHERE TALEP_DURUM_ID = 18 AND SUBE_ID=" + userSube + @") AS BEKLEMEDE_TALEP,
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_TEKLIF WHERE TEKLIF_DURUM_ID = 19 AND SUBE_ID= " + userSube + @") AS BEKLEMEDE_TEKLIF,
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_SIPARIS WHERE SIPARIS_DURUM_ID = 15 AND SUBE_ID=" + userSube + @") AS BEKLEMEDE_SIPARIS,
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_TALEP WHERE TALEP_DURUM_ID = 20 AND SUBE_ID=" + userSube + @") AS ONAYLANAN_TALEP,
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_TEKLIF WHERE TEKLIF_DURUM_ID = 21 AND SUBE_ID=" + userSube + @") AS ONAYLANAN_TEKLIF,
                            (SELECT ISNULL(COUNT(*), 0) FROM dbo.TBL_SIPARIS WHERE SIPARIS_DURUM_ID = 17 AND SUBE_ID=" + userSube + @") AS TAMAMLANAN_SIPARIS";

                var dashboardData = await db.Database.SqlQuery<VW_DASHBOARD>(sql).FirstOrDefaultAsync();

                if (dashboardData != null)
                {
                    ViewBag.BEKLEMEDE_TALEP = dashboardData.BEKLEMEDE_TALEP;
                    ViewBag.BEKLEMEDE_TEKLIF = dashboardData.BEKLEMEDE_TEKLIF;
                    ViewBag.BEKLEMEDE_SIPARIS = dashboardData.BEKLEMEDE_SIPARIS;

                    ViewBag.ONAYLANAN_TALEP = dashboardData.ONAYLANAN_TALEP;
                    ViewBag.ONAYLANAN_TEKLIF = dashboardData.ONAYLANAN_TEKLIF;
                    ViewBag.TAMAMLANAN_SIPARIS = dashboardData.TAMAMLANAN_SIPARIS;
                }
            }

            return View();
        }
        //[Route("beklemede-talepler")]
        //[Authorize(Roles = "1,2,3,4")]
        //public ActionResult BeklemedeTalepler()
        //{
        //    return View();
        //}
        //[Route("beklemede-teklifler")]
        //[Authorize(Roles = "1,2,3,4")]
        //public ActionResult BeklemedeTeklifler()
        //{
        //    return View();
        //}
        [Route("beklemede-onaylanan-teklifler")]
        [Authorize(Roles = "1,2,3,4")]
        public ActionResult BeklemedeOnaylananTeklifler()
        {
            return View();
        }
        [Route("reddedilen-teklifler")]
        [Authorize(Roles = "1,2,3,4")]
        public ActionResult ReddedilenTeklifler()
        {
            return View();
        }
        [Route("beklemede-onaylanan-talepler")]
        [Authorize(Roles = "1,2,3,4")]
        public ActionResult BeklemedeOnaylananTalepler()
        {
            return View();
        }
        [Route("reddedilen-talepler")]
        [Authorize(Roles = "1,2,3,4")]
        public ActionResult ReddedilenTalepler()
        {
            return View();
        }
     
        [Route("talep-olustur")]
        [Authorize(Roles = "1,4,5")]
        public ActionResult TalepOlustur()
        {
            return View();
        }

        [Route("stok-kullan")]
        [Authorize(Roles = "1,4,5")]
        public ActionResult StokKullan()
        {
            return View();
        }

        [Route("malzeme-cikis")]
        [Authorize(Roles = "1,4")]
        public ActionResult MalzemeCikis()
        {
            return View();
        }

        [Route("stok-durum")]
        [Authorize(Roles = "1,2,3,4")]
        public ActionResult StokDurum()
        {
            return View();
        }

        [Route("siparisler")]
        [Authorize(Roles = "1,4")]
        public ActionResult Siparisler()
        {
            return View();
        }
        [Route("raporlar")]
        [Authorize(Roles = "1,4")]
        public ActionResult Raporlar()
        {
            return View();
        }
    }
}