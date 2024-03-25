using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using U_Project.Models;

namespace U_Project.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class APIController : ApiController
    {
        [HttpPost, Route("logout")]
        public IHttpActionResult Logout()
        {
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1); 
            HttpContext.Current.Response.Cookies.Add(cookie);
            FormsAuthentication.SignOut();

            return Ok();
        }
        [AllowAnonymous]
        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login()
        {
            try
            {
                var username = HttpContext.Current.Request.Params["username"];
                var pass = HttpContext.Current.Request.Params["pass"];
                var isAuthenticated = await PerformAuthentication(username, pass);
                if (isAuthenticated != null)
                {
                    var authTicket = new FormsAuthenticationTicket(
                                isAuthenticated.ID.ToString(),
                                true,
                                (int)FormsAuthentication.Timeout.TotalMinutes
                    );

                    string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                    HttpContext.Current.Response.Cookies.Add(authCookie);

                    return Ok();
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet, Route("get-user-for-layout")]
        public List<string> GetUserForLayout()
        {
            using (U_Entities db = new U_Entities())
            {
                var userId = Convert.ToInt32(User.Identity.Name);

                if (userId != 0)
                {
                    var kullanici = db.VW_KULLANICI
                        .Where(u => u.ID == userId)
                        .Select(u => new List<string> { u.ISIM_SOYISIM, u.ROL_AD })
                        .FirstOrDefault();

                    return kullanici;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<VW_KULLANICI> PerformAuthentication(string username, string password)
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var find = await db.VW_KULLANICI.FirstOrDefaultAsync(x => x.KULLANICI_ADI == username && x.SIFRE == password);

                    if (find != null)
                    {
                        return find;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        [Route("stok-gruplar")]
        public IHttpActionResult StokGruplar()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var grup_tbl = db.TBL_STOK_GRUP.ToList();
                    if (grup_tbl != null || grup_tbl.Count > 0)
                    {
                        return Ok(grup_tbl);
                    }
                    else
                    {
                        return BadRequest("Stok grupları tablosunda veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("depo-gruplar")]
        public IHttpActionResult DepoGruplar()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var depo_tbl = db.TBL_DEPO.ToList();
                    if (depo_tbl != null || depo_tbl.Count > 0)
                    {
                        return Ok(depo_tbl);
                    }
                    else
                    {
                        return BadRequest("Depo grupları tablosunda veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("stoklar")]
        public IHttpActionResult StokListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    int grup_id = Convert.ToInt32(HttpContext.Current.Request.Params["grup_id"]);
                    var stok_tbl = db.TBL_STOK.Where(x => x.GRUP_KODU == grup_id).ToList();
                    if (stok_tbl != null || stok_tbl.Count > 0)
                    {
                        return Ok(stok_tbl);
                    }
                    else
                    {
                        return BadRequest("Stok tablosunda veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("firma-olustur")]
        public async Task<IHttpActionResult> FirmaOlustur()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var firma_ad = HttpContext.Current.Request.Params["firma_ad"];

                    TBL_FIRMA yeniFirma = new TBL_FIRMA
                    {
                        FIRMA_AD = firma_ad
                    };

                    if (yeniFirma != null)
                    {
                        db.TBL_FIRMA.Add(yeniFirma);
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Firma eklenemedi.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("depo-olustur")]
        public async Task<IHttpActionResult> DepoOlustur()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var depo_ad = HttpContext.Current.Request.Params["depo_ad"];

                    int sube_kodu = await db.TBL_KULLANICI.Where(x => x.ID == userId).Select(x => x.SUBE_ID).FirstOrDefaultAsync();

                    TBL_DEPO yeniDepo = new TBL_DEPO
                    {
                        SUBE_KODU = sube_kodu,
                        DEPO_AD = depo_ad,
                    };

                    if (yeniDepo != null)
                    {
                        db.TBL_DEPO.Add(yeniDepo);
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Depo eklenemedi.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("stok-olustur")]
        public async Task<IHttpActionResult> StokOlustur()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var stok_ad = HttpContext.Current.Request.Params["stok_ad"];
                    var stok_olcu = HttpContext.Current.Request.Params["stok_olcu"];
                    string stok_no_function_tag = "USTK" + DateTime.Now.ToString("yyyy");
                    var stok_kod = await db.Database.SqlQuery<string>("SELECT DBO.IDF_ORDER_STOKNO(@p0)", stok_no_function_tag).FirstOrDefaultAsync();
                    //var stok_a_fiyat = Convert.ToDouble(HttpContext.Current.Request.Params["stok_a_fiyat"]);
                    //var stok_s_fiyat = Convert.ToDouble(HttpContext.Current.Request.Params["stok_s_fiyat"]);
                    //var stok_kdv = Convert.ToInt32(HttpContext.Current.Request.Params["stok_kdv"]);
                    var stok_depo = Convert.ToInt32(HttpContext.Current.Request.Params["stok_depo"]);
                    var stok_grup = Convert.ToInt32(HttpContext.Current.Request.Params["stok_grup"]);

                    int sube_kodu = await db.TBL_KULLANICI.Where(x => x.ID == userId).Select(x => x.SUBE_ID).FirstOrDefaultAsync();
                    TBL_STOK yeniStok = new TBL_STOK
                    {
                        SUBE_KODU = sube_kodu,
                        STOK_KODU = stok_kod,
                        STOK_ADI = stok_ad,
                        OLCU_BR = stok_olcu,
                        GRUP_KODU = stok_grup,
                        //A_FIYAT = stok_a_fiyat,
                        //S_FIYAT = stok_s_fiyat,
                        //KDV_ORAN = stok_kdv,
                        DEPO_KODU = stok_depo,
                    };

                    if (yeniStok != null)
                    {
                        db.TBL_STOK.Add(yeniStok);
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Stok eklenemedi.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("firmalar")]
        public IHttpActionResult FirmaListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;

                    var firma_tbl = db.TBL_FIRMA.ToList();
                    if (firma_tbl != null || firma_tbl.Count > 0)
                    {
                        return Ok(firma_tbl);
                    }
                    else
                    {
                        return BadRequest("Firma tablosunda veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("stok-birim")]
        public async Task<IHttpActionResult> StokBirim()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var stok_kodu = HttpContext.Current.Request.Params["stok_kodu"];
                    var stok_birim = await db.TBL_STOK
                         .Where(x => x.STOK_KODU == stok_kodu)
                         .Select(x => x.OLCU_BR)
                         .FirstOrDefaultAsync();

                    if (stok_birim != null)
                    {
                        return Ok(stok_birim);
                    }
                    else
                    {
                        return BadRequest("Stok birimi bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("stok-mevcut-miktar")]
        public async Task<IHttpActionResult> StokMevcutMiktar()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    db.Configuration.LazyLoadingEnabled = false;
                    var stok_kodu = HttpContext.Current.Request.Params["stok_kodu"];
                    var stok_mevcut_miktar = await db.TBL_STOK_HAREKET2
                         .Where(x => x.STOK_KODU == stok_kodu)
                         .Select(x => x.MIKTAR)
                         .FirstOrDefaultAsync();

                    return Ok(stok_mevcut_miktar);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("onay-talep-liste")]
        public IHttpActionResult OnayTalepListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var onay_talep_tbl = db.IDP_TALEP_GRID(userId, 1).ToList();
                    if (onay_talep_tbl != null || onay_talep_tbl.Count > 0)
                    {
                        return Ok(onay_talep_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("red-talep-liste")]
        public IHttpActionResult RedTalepListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var red_talep_tbl = db.IDP_TALEP_GRID(userId, 2).ToList();
                    if (red_talep_tbl != null || red_talep_tbl.Count > 0)
                    {
                        return Ok(red_talep_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("talep-detay")]
        public async Task<IHttpActionResult> TalepDetay()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var talep_detay = await db.VW_TALEP.FirstOrDefaultAsync(x => x.ID == id);
                    if (talep_detay != null)
                    {
                        return Ok(talep_detay);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("siparis-detay")]
        public async Task<IHttpActionResult> SiparisDetay()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var siparis_detay = await db.VW_SIPARIS.FirstOrDefaultAsync(x => x.SIPARIS_ID == id);
                    if (siparis_detay != null)
                    {
                        return Ok(siparis_detay);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("teklif-detay")]
        public async Task<IHttpActionResult> TeklifDetay()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var teklif_detay = await db.VW_TEKLIF.FirstOrDefaultAsync(x => x.ID == id);
                    if (teklif_detay != null)
                    {
                        return Ok(teklif_detay);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("plaka-birim-liste")]
        public IHttpActionResult PlakaBirimListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var departman_id = Convert.ToInt32(HttpContext.Current.Request.Params["departman_id"]);

                    var plaka_birim_tbl = db.IDP_PLAKA_BIRIM_LIST(userId, departman_id).ToList();
                    if (plaka_birim_tbl != null || plaka_birim_tbl.Count > 0)
                    {
                        return Ok(plaka_birim_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("siparis-liste")]
        public IHttpActionResult SiparisListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var siparis_tbl = db.IDP_SIPARIS_GRID(userId).ToList();
                    if (siparis_tbl != null || siparis_tbl.Count > 0)
                    {
                        return Ok(siparis_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("malzeme-cikis-liste")]
        public IHttpActionResult MalzemeCikisListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var stok_kullanim = db.IDP_STOK_HAREKET_LOG_GRID(userId).ToList();
                    if (stok_kullanim != null || stok_kullanim.Count > 0)
                    {
                        return Ok(stok_kullanim);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("stok-durum-liste")]
        public IHttpActionResult StokDurumListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var stok_durum_tbl = db.IDP_STOK_HAREKET_GRID(userId).ToList();
                    if (stok_durum_tbl != null || stok_durum_tbl.Count > 0)
                    {
                        return Ok(stok_durum_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpGet]
        [Route("teklif-asamasindaki-talepler")]
        public IHttpActionResult TeklifAsamasindakiTalepler()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var tatg = db.IDP_TEKLIF_ASAMASINDAKI_TALEPLER_GRID(userId).ToList();
                    if (tatg != null || tatg.Count > 0)
                    {
                        return Ok(tatg);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("talebe-ait-teklif-liste")]
        public IHttpActionResult TalebeAitTeklifListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var talep_id = HttpContext.Current.Request.Params["tlpid"];
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var teklif_tbl = db.IDP_TEKLIF_GRID(userId, 1, Convert.ToInt32(talep_id)).ToList();
                    if (teklif_tbl != null || teklif_tbl.Count > 0)
                    {
                        return Ok(teklif_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        [Route("red-teklif-liste")]
        public IHttpActionResult RedTeklifListe()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var teklif_tbl = db.IDP_TEKLIF_GRID(userId, 2, null).ToList();
                    if (teklif_tbl != null || teklif_tbl.Count > 0)
                    {
                        return Ok(teklif_tbl);
                    }
                    else
                    {
                        return BadRequest("View'da veri yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("silme-islemi")]
        public async Task<IHttpActionResult> SilmeIslemi()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var tablo = HttpContext.Current.Request.Params["tablo"];
                    if (tablo == null)
                    {
                        return BadRequest("Tablo bilgisi alınamadı.");
                    }
                    else if (tablo == "talep")
                    {
                        var silinecekTalep = await db.TBL_TALEP.FirstOrDefaultAsync(x => x.ID == id);
                        if (silinecekTalep != null)
                        {
                            var silinecekTalepTeklifi = await db.TBL_TEKLIF.FirstOrDefaultAsync(x => x.TALEP_ID == id);
                            if (silinecekTalepTeklifi != null)
                            {
                                db.TBL_TEKLIF.Remove(silinecekTalepTeklifi);
                            }
                            db.TBL_TALEP.Remove(silinecekTalep);
                        }
                        else
                        {
                            return BadRequest($"{id} ID'li kayıt talep tablosunda bulunamadı.");
                        }
                    }
                    else if (tablo == "teklif")
                    {
                        var silinecekTeklif = await db.TBL_TEKLIF.FirstOrDefaultAsync(x => x.ID == id);
                        if (silinecekTeklif != null)
                        {
                            db.TBL_TEKLIF.Remove(silinecekTeklif);
                        }
                        else
                        {
                            return BadRequest($"{id} ID'li kayıt teklif tablosunda bulunamadı.");
                        }
                    }
                    await db.SaveChangesAsync();
                    return Ok($"{id} ID'li kayıt başarıyla silindi.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("teklif-onayla")]
        public async Task<IHttpActionResult> TeklifOnayla()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var durum = HttpContext.Current.Request.Params["durum"];
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var aciklama = HttpContext.Current.Request.Params["aciklama"];
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var teklif = await db.TBL_TEKLIF.FirstOrDefaultAsync(x => x.ID == id);

                    if (teklif != null)
                    {
                        if (durum == "onay")
                        {
                            if (teklif.ONAY1 == null)
                            {
                                teklif.ONAY1 = true;
                                teklif.ONAY1_TARIH = DateTime.Now;
                                teklif.ONAY1_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 8; // TEKLİF ASAMA 1 ONAYLANDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok(teklif.TALEP_ID);
                            }
                            else if (teklif.ONAY1 != null && teklif.ONAY2 == null)
                            {
                                teklif.ONAY2 = true;
                                teklif.ONAY2_TARIH = DateTime.Now;
                                teklif.ONAY2_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 9; // TEKLİF ASAMA 2 REDDEDILDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok(teklif.TALEP_ID);
                            }
                            else if (teklif.ONAY1 != null && teklif.ONAY2 != null && teklif.ONAY3 == null)
                            {
                                teklif.ONAY3 = true;
                                teklif.ONAY3_TARIH = DateTime.Now;
                                teklif.ONAY3_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 10; // TEKLİF ASAMA 3 ONAYLANDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;

                                if (teklif.ONAY1 == true && teklif.ONAY2 == true && teklif.ONAY3 == true)
                                {
                                    string siparis_no_function_tag = "S" + DateTime.Now.ToString("yyyy");
                                    var siparis_no = db.Database.SqlQuery<string>("SELECT DBO.IDF_ORDER_SIPARISNO(@p0)", siparis_no_function_tag).FirstOrDefault();
                                    TBL_SIPARIS yeniSiparis = new TBL_SIPARIS
                                    {
                                        SIPARIS_NO = siparis_no,
                                        TALEP_ID = teklif.TALEP_ID,
                                        OLUSTURULMA_TARIH = DateTime.Now,
                                        TEKLIF_ID = teklif.ID,
                                        SIPARIS_DURUM_ID = 15, // SİPARİŞ BEKLEMEDE
                                        OLUSTURAN_ID = userId,
                                        SUBE_ID = teklif.SUBE_ID
                                    };
                                    if (yeniSiparis != null)
                                    {
                                        db.TBL_SIPARIS.Add(yeniSiparis);
                                        teklif.TEKLIF_DURUM_ID = 21; // TEKLİF İÇİN SİPARİŞ OLUŞTURULDU
                                        var digerTeklifler = db.TBL_TEKLIF.Where(x => x.TALEP_ID == teklif.TALEP_ID && x.ID != teklif.ID).ToList();
                                        foreach (var digerTeklif in digerTeklifler)
                                        {
                                            digerTeklif.TEKLIF_DURUM_ID = 22;
                                        }
                                        await db.SaveChangesAsync();
                                        return Ok(teklif.TALEP_ID);
                                    }
                                    else
                                    {
                                        return BadRequest("Sipariş oluşturulamadı bu yüzden onayınız kabul edilmedi. Tekrar deneyiniz.");
                                    }
                                }
                            }
                            return BadRequest("Teklif onay sisteminde hata!");
                        }
                        else if (durum == "red")
                        {
                            if (teklif.ONAY1 == null)
                            {
                                teklif.ONAY1 = false;
                                teklif.ONAY1_TARIH = DateTime.Now;
                                teklif.ONAY1_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 11; // TEKLİF ASAMA 1 REDDEDILDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (teklif.ONAY1 != null && teklif.ONAY2 == null)
                            {
                                teklif.ONAY2 = false;
                                teklif.ONAY2_TARIH = DateTime.Now;
                                teklif.ONAY2_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 13; // TEKLİF ASAMA 2 REDDEDILDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (teklif.ONAY1 != null && teklif.ONAY2 != null && teklif.ONAY3 == null)
                            {
                                teklif.ONAY3 = false;
                                teklif.ONAY3_TARIH = DateTime.Now;
                                teklif.ONAY3_KULLANICI = userId;
                                teklif.TEKLIF_DURUM_ID = 14; // TEKLİF ASAMA 3 REDDEDILDI
                                teklif.TEKLIF_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            return BadRequest("Teklif onay sisteminde hata!");
                        }
                        else
                        {
                            return BadRequest("Durum okunamadı.");
                        }
                    }
                    else
                    {
                        return BadRequest("Veri bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("talep-onayla")]
        public async Task<IHttpActionResult> TalepOnayla()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var durum = HttpContext.Current.Request.Params["durum"];
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var aciklama = HttpContext.Current.Request.Params["aciklama"];
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var talep = await db.TBL_TALEP.FirstOrDefaultAsync(x => x.ID == id);
                    if (talep != null)
                    {
                        if (durum == "onay")
                        {
                            if (talep.ONAY1 == null)
                            {
                                talep.ONAY1 = true;
                                talep.ONAY1_TARIH = DateTime.Now;
                                talep.ONAY1_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 2; // TALEP ASAMA 1 ONAYLANDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (talep.ONAY1 != null && talep.ONAY2 == null)
                            {
                                talep.ONAY2 = true;
                                talep.ONAY2_TARIH = DateTime.Now;
                                talep.ONAY2_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 3; // TALEP ASAMA 2 REDDEDILDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (talep.ONAY1 != null && talep.ONAY2 != null && talep.ONAY3 == null)
                            {
                                talep.ONAY3 = true;
                                talep.ONAY3_TARIH = DateTime.Now;
                                talep.ONAY3_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 4; // TALEP ASAMA 3 ONAYLANDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            return BadRequest("Talep onay sisteminde hata!");
                        }
                        else if (durum == "red")
                        {
                            if (talep.ONAY1 == null)
                            {
                                talep.ONAY1 = false;
                                talep.ONAY1_TARIH = DateTime.Now;
                                talep.ONAY1_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 5; // TALEP ASAMA 1 REDDEDILDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (talep.ONAY1 != null && talep.ONAY2 == null)
                            {
                                talep.ONAY2 = false;
                                talep.ONAY2_TARIH = DateTime.Now;
                                talep.ONAY2_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 6; // TALEP ASAMA 2 REDDEDILDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            else if (talep.ONAY1 != null && talep.ONAY2 != null && talep.ONAY3 == null)
                            {
                                talep.ONAY3 = false;
                                talep.ONAY3_TARIH = DateTime.Now;
                                talep.ONAY3_KULLANICI = userId;
                                talep.TALEP_DURUM_ID = 7; // TALEP ASAMA 3 REDDEDILDI
                                talep.TALEP_ISLEM_ACIKLAMA = !string.IsNullOrEmpty(aciklama) ? aciklama : null;
                                await db.SaveChangesAsync();
                                return Ok();
                            }
                            return BadRequest("Talep onay sisteminde hata!");
                        }
                        else
                        {
                            return BadRequest("Durum okunamadı.");
                        }
                    }
                    else
                    {
                        return BadRequest("Veri bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("siparis-durum-guncelle")]
        public async Task<IHttpActionResult> SiparisDurumGuncelle()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var durum = HttpContext.Current.Request.Params["durum"];
                    var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var aciklama = HttpContext.Current.Request.Params["aciklama"];
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var guncelle = await db.TBL_SIPARIS.FirstOrDefaultAsync(x => x.ID == id);
                    if (guncelle != null)
                    {
                        guncelle.ISLEM_YAPAN_ID = userId;

                        if (durum == "teslim")
                        {
                            guncelle.DURUM_GUNCELLEME_TARIH = DateTime.Now;
                            guncelle.SIPARIS_DURUM_ID = 16; // SİPARİŞ TESLİM ALINDI
                            await db.SaveChangesAsync();
                            return Ok();
                        }
                        else if (durum == "tamam")
                        {
                            guncelle.SIPARIS_DURUM_ID = 17; // SİPARİŞ TAMAMLANDI
                            guncelle.DURUM_GUNCELLEME_TARIH = DateTime.Now;
                            var siparis_bilgiler = await db.VW_SIPARIS.Where(x => x.SIPARIS_ID == id).FirstOrDefaultAsync();
                            await db.Database.ExecuteSqlCommandAsync("EXEC [dbo].[IDP_STOK_HAR_I] @FISNO, @STOK_KODU, @MIKTAR, @GCKOD, @TARIH, @KULLANICI_ID, @DEPARMTAN_ID, @SIPARIS_NO, @KULLANILDIMI, @PLAKA_BIRIM, @H_NOT",
                              new SqlParameter("FISNO", siparis_bilgiler.SIPARIS_NO),
                              new SqlParameter("STOK_KODU", siparis_bilgiler.STOK_KODU),
                              new SqlParameter("MIKTAR", siparis_bilgiler.TALEP_MIKTAR),
                              new SqlParameter("GCKOD", "G"),
                              new SqlParameter("TARIH", siparis_bilgiler.SIPARIS_OLUSTURULMA_TARIH),
                              new SqlParameter("KULLANICI_ID", userId),
                              new SqlParameter("DEPARMTAN_ID", siparis_bilgiler.DEPARTMAN_ID),
                              new SqlParameter("SIPARIS_NO", siparis_bilgiler.SIPARIS_NO),
                              new SqlParameter("KULLANILDIMI", false),
                              new SqlParameter("PLAKA_BIRIM", siparis_bilgiler.PLAKA_BIRIM),
                              new SqlParameter("H_NOT", string.Empty)
                          );

                            await db.SaveChangesAsync();
                            return Ok();
                        }
                        else
                        {
                            return BadRequest("Durum okunamadı.");
                        }
                    }
                    else
                    {
                        return BadRequest("Veri bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("stok-hareket-log")]
        public async Task<IHttpActionResult> StokHareketLog()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var stok_hareket_miktar = HttpContext.Current.Request.Form.GetValues("stok_hareket_miktar[]");
                    var stok_hareket_plaka_birim = HttpContext.Current.Request.Form.GetValues("stok_hareket_plaka_birim[]");
                    var stok_hareket_departman_id = HttpContext.Current.Request.Form.GetValues("stok_hareket_departman_id[]");
                    var stok_hareket_stok = HttpContext.Current.Request.Form.GetValues("stok_hareket_stok[]");
                    var stok_hareket_not = HttpContext.Current.Request.Form.GetValues("stok_hareket_not[]");
                    var userId = Convert.ToInt32(User.Identity.Name);
                    var kullanici_sube = db.TBL_KULLANICI.Where(x => x.ID == userId).Select(x => x.SUBE_ID).FirstOrDefault();

                    for (int i = 0; i < stok_hareket_stok.Length; i++)
                    {
                        var plaka_birim = stok_hareket_plaka_birim[i];
                        if (string.IsNullOrEmpty(plaka_birim) || plaka_birim == "0" || plaka_birim == "null" || plaka_birim == null)
                        {
                            plaka_birim = null;
                        }

                        await db.Database.ExecuteSqlCommandAsync("EXEC [dbo].[IDP_STOK_HAR_I] @FISNO, @STOK_KODU, @MIKTAR, @GCKOD, @TARIH, @KULLANICI_ID, @DEPARMTAN_ID, @SIPARIS_NO, @KULLANILDIMI, @PLAKA_BIRIM, @H_NOT",
                            new SqlParameter("FISNO", string.Empty),
                            new SqlParameter("STOK_KODU", stok_hareket_stok[i]),
                            new SqlParameter("MIKTAR", Convert.ToSingle(stok_hareket_miktar[i])),
                            new SqlParameter("GCKOD", "C"),
                            new SqlParameter("TARIH", DateTime.Now),
                            new SqlParameter("KULLANICI_ID", userId),
                            new SqlParameter("DEPARMTAN_ID", stok_hareket_departman_id[i]),
                            new SqlParameter("SIPARIS_NO", string.Empty),
                            new SqlParameter("KULLANILDIMI", true),
                            new SqlParameter("PLAKA_BIRIM", stok_hareket_plaka_birim[i]),
                            new SqlParameter("H_NOT", !string.IsNullOrEmpty(stok_hareket_not[i]) ? stok_hareket_not[i] : string.Empty)
                            );
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("teklif-ekle")]
        public async Task<IHttpActionResult> TeklifEkle()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var talep_id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var teklif_fiyat = HttpContext.Current.Request.Params["teklif_fiyat"];
                    var teklif_not = HttpContext.Current.Request.Params["teklif_not"];
                    var firma = Convert.ToInt32(HttpContext.Current.Request.Params["firma"]);
                    var marka = HttpContext.Current.Request.Params["marka"];
                    string teklif_no_function_tag = "UTKF" + DateTime.Now.ToString("yyyy");
                    var teklif_no = db.Database.SqlQuery<string>("SELECT DBO.IDF_ORDER_TEKLIFNO(@p0)", teklif_no_function_tag).FirstOrDefault();

                    decimal decimalFiyat = Convert.ToDecimal(teklif_fiyat);
                    var userId = Convert.ToInt32(User.Identity.Name);

                    var talep = await db.TBL_TALEP.Where(x => x.ID == talep_id).FirstOrDefaultAsync();

                    TBL_TEKLIF yeniTeklif = new TBL_TEKLIF
                    {
                        TEKLIF_NO = teklif_no,
                        TEKLIF_FIYAT = decimalFiyat,
                        MARKA = !string.IsNullOrEmpty(marka) ? marka : null,
                        TEKLIF_NOT = !string.IsNullOrEmpty(teklif_not) ? teklif_not : null,
                        TEKLIF_DURUM_ID = 19,  // TEKLİF ONAY BEKLEMEDE
                        OLUSTURAN_ID = userId,
                        OLUSTURULMA_TARIH = DateTime.Now,
                        TALEP_ID = talep_id, // İlişkili talebin id'si
                        SUBE_ID = talep.SUBE_ID,
                        FIRMA_ID = firma
                    };

                    if (yeniTeklif != null && talep != null)
                    {
                        db.TBL_TEKLIF.Add(yeniTeklif);
                        //talep.TALEP_DURUM_ID = 20; // TALEP İÇİN TEKLİF OLUŞTURULDU
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Teklif oluşturulamadı!");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        [Route("teklif-kapat")]
        public async Task<IHttpActionResult> TeklifKapat()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {
                    var talep_id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
                    var talep = await db.TBL_TALEP.Where(x => x.ID == talep_id).FirstOrDefaultAsync();
                    talep.TALEP_DURUM_ID = 20; // TALEP İÇİN TEKLİF OLUŞTURULDU
                    await db.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("rapor-al")]
        public async Task<IHttpActionResult> RaporAl()
        {
            try
            {
                var userId = Convert.ToInt32(User.Identity.Name);
                var tablo = HttpContext.Current.Request.Params["tablo"];
                var departmanStr = HttpContext.Current.Request.Params["departman"];
                var plakaStr = HttpContext.Current.Request.Params["plaka"];
                var stok_grupStr = HttpContext.Current.Request.Params["stok_grup"];
                var stokStr = HttpContext.Current.Request.Params["stok"];
                var yilStr = HttpContext.Current.Request.Params["yil"];
                var plaka = plakaStr == "null" ? null : plakaStr;
                var stok = stokStr == "null" ? null : stokStr;
                int? yil = null;
                int? departman = null;
                int? stok_grup = null;

                if (!string.IsNullOrEmpty(departmanStr) && int.TryParse(departmanStr, out int departmanValue))
                {
                    departman = departmanValue;
                }

                if (!string.IsNullOrEmpty(stok_grupStr) && int.TryParse(stok_grupStr, out int stok_grupValue))
                {
                    stok_grup = stok_grupValue;
                }
                if (!string.IsNullOrEmpty(yilStr) && int.TryParse(yilStr, out int yilValue))
                {
                    yil = yilValue;
                }
                using (var context = new U_Entities())
                {
                    object dataList = null;
                    var subeId = context.TBL_KULLANICI.Where(k => k.ID == userId).Select(k => k.SUBE_ID).FirstOrDefault();
                    if (tablo == "SIPARIS")
                    {
                        List<VW_SIPARIS> siparisList = await context.VW_SIPARIS.Where(s => s.SUBE_ID == subeId &&
                                         (departman == null || s.DEPARTMAN_ID == departman) &&
                                         (plaka == null || s.PLAKA_BIRIM.Contains(plaka)) &&
                                         (stok_grup == null || s.STOK_GRUP_ID == stok_grup) &&
                                         (stok == null || s.STOK_KODU.Contains(stok)) &&
                                         (yil == null || s.SIPARIS_OLUSTURULMA_TARIH.Year == yil)).ToListAsync();

                        dataList = siparisList;
                    }
                    else if (tablo == "TALEP")
                    {
                        List<VW_TALEP> talepList = await context.VW_TALEP.Where(tl => tl.SUBE_ID == subeId &&
                                         (departman == null || tl.TALEP_DEPARTMAN_ID == departman) &&
                                         (plaka == null || tl.PLAKA_BIRIM.Contains(plaka)) &&
                                         (stok_grup == null || tl.STOK_GRUP_ID == stok_grup) &&
                                         (stok == null || tl.STOK_KODU.Contains(stok)) &&
                                         (yil == null || tl.OLUSTURULMA_TARIH.Year == yil)).ToListAsync();

                        dataList = talepList;
                    }
                    else if (tablo == "TEKLIF")
                    {
                        List<VW_TEKLIF> teklifList = await context.VW_TEKLIF.Where(tk => tk.SUBE_ID == subeId &&
                                         (departman == null || tk.DEPARTMAN_ID == departman) &&
                                         (plaka == null || tk.PLAKA_BIRIM.Contains(plaka)) &&
                                         (stok_grup == null || tk.STOK_GRUP_ID == stok_grup) &&
                                         (stok == null || tk.STOK_KODU.Contains(stok)) &&
                                         (yil == null || tk.OLUSTURULMA_TARIH.Year == yil)).ToListAsync();

                        dataList = teklifList;

                    }
                    else if (tablo == "STOKHAREKET")
                    {
                        List<VW_STOK_HAREKET_LOG> stokList = await context.VW_STOK_HAREKET_LOG.Where(sh => sh.SUBE_ID == subeId &&
                                         (departman == null || sh.DEPARTMAN_ID == departman) &&
                                         (plaka == null || sh.PLAKA_BIRIM.Contains(plaka)) &&
                                         (stok_grup == null || sh.STOK_GRUP_ID == stok_grup) &&
                                         (stok == null || sh.STOK_KODU.Contains(stok)) &&
                                         (yil == null || sh.TARIH.Year == yil)).ToListAsync();

                        dataList = stokList;
                    }
                    else
                    {
                        return BadRequest("Tablo bilgisi alınamadı.");
                    }

                    // Convert the data list to an Excel file
                    var memoryStream = GenerateExcel(dataList);
                    if (memoryStream == null) { return BadRequest("Herhangi bir kayıt bulunamadı."); }
                    memoryStream.Position = 0;

                    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(memoryStream.ToArray())
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{tablo}_Rapor.xlsx"
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    var response = ResponseMessage(result);
                    return response;

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
        private MemoryStream GenerateExcel(object dataList)
        {
            // Check if dataList is null or empty
            if (dataList == null || !(dataList is IEnumerable<object> enumerable) || !enumerable.Any())
            {
                // Return null or throw an exception, depending on your requirements
                return null;
            }
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Rapor");

            // Use reflection to get property names for header row
            var props = dataList.GetType().GetGenericArguments()[0].GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = props[i].Name;
            }

            // Fill in the data
            int row = 2;
            foreach (var item in (IEnumerable<object>)dataList)
            {
                for (int col = 0; col < props.Length; col++)
                {
                    var value = props[col].GetValue(item, null);
                    worksheet.Cell(row, col + 1).Value = value != null ? value.ToString() : "";
                }

                row++;
            }

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream;
        }
        [HttpPost]
        [Route("talep-ekle")]
        public async Task<IHttpActionResult> TalepEkle()
        {
            try
            {
                using (U_Entities db = new U_Entities())
                {

                    var talep_miktar = HttpContext.Current.Request.Form.GetValues("talep_miktar[]");
                    var talep_plaka_birim = HttpContext.Current.Request.Form.GetValues("talep_plaka_birim[]");
                    var talep_departman_id = HttpContext.Current.Request.Form.GetValues("talep_departman_id[]");
                    var talep_stok = HttpContext.Current.Request.Form.GetValues("talep_stok[]");
                    var talep_not = HttpContext.Current.Request.Form.GetValues("talep_not[]");
                    int userId = Convert.ToInt32(User.Identity.Name);
                    int kullanici_sube = db.TBL_KULLANICI.Where(x => x.ID == userId).Select(x => x.SUBE_ID).FirstOrDefault();
                    string talep_no_function_tag = "UTLP" + DateTime.Now.ToString("yyyy");
                    for (int i = 0; i < talep_stok.Length; i++)
                    {
                        string plaka_birim = talep_plaka_birim[i];
                        if (string.IsNullOrEmpty(plaka_birim) || plaka_birim == "0" || plaka_birim == "null" || plaka_birim == null)
                        {
                            plaka_birim = null;
                        }
                        TBL_TALEP yeniTalep = new TBL_TALEP
                        {
                            TALEP_NO = db.Database.SqlQuery<string>("SELECT DBO.IDF_ORDER_TALEPNO(@p0)", talep_no_function_tag).FirstOrDefault(),
                            TALEP_MIKTAR = Convert.ToInt32(talep_miktar[i]),
                            PLAKA_BIRIM = plaka_birim,
                            TALEP_DEPARTMAN_ID = Convert.ToInt32(talep_departman_id[i]),
                            TALEP_NOT = !string.IsNullOrEmpty(talep_not[i]) ? talep_not[i] : null,
                            STOK_KODU = talep_stok[i],
                            TALEP_DURUM_ID = 18,
                            OLUSTURAN_ID = userId,
                            OLUSTURULMA_TARIH = DateTime.Now,
                            SUBE_ID = kullanici_sube,
                        };
                        if (yeniTalep != null)
                        {
                            db.TBL_TALEP.Add(yeniTalep);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            return BadRequest(talep_stok[i]);
                        }
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}