using IsTakipSistemi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemi.Controllers
{
    public class YoneticiController : Controller
    {
        // Veritabanı işlemleri için IsTakipDBEntities adlı entity nesnesi oluşturuluyor.
        IsTakipDBEntities entity = new IsTakipDBEntities();

        // Yonetici kontrolcüsü için ana sayfa (Index) aksiyonu.
        public ActionResult Index()
        {

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);  // Session'dan "PersonelYetkiTurId" bilgisini alıp yetki türünü int'e dönüştürüyor.


            if (yetkiTurId == 1)  // Eğer yetki türü 1 ise (muhtemelen yönetici yetkisi).
            {

                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);  // Session'dan "PersonelBirimId" alıp birimId'ye atıyor.


                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault(); // Veritabanından birimId'si eşleşen birimi sorguluyor.

                ViewBag.birimAd = birim.birimAd;

                return View();  // Birim sorgulandıktan sonra View'e yönlendiriliyor.
            }
            else
            {

                return RedirectToAction("Index", "Login");  // Yetki türü 1 değilse, kullanıcı Login sayfasına yönlendiriliyor.
            }
        }

        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]); // Kullanıcının yetki türünü Session'dan alır ve int'e çevirir.

            if (yetkiTurId == 1)  // Eğer kullanıcının yetki türü 1 ise (muhtemelen yönetici).
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]); // Kullanıcının bağlı olduğu birimId'yi Session'dan alır.

                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  select p).ToList(); // Bu birime bağlı ve yetki türü 2 olan personelleri veritabanından sorgular ve listeye dönüştürür.

                ViewBag.personeller = calisanlar;


                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault(); // Veritabanından birimId'si eşleşen birimi sorguluyor.

                ViewBag.birimAd = birim.birimAd;

                return View();  // Ata view'ini döner.
            }
            else
            {

                return RedirectToAction("Index", "Login");    // Eğer yetki türü 1 değilse, kullanıcıyı Login sayfasına yönlendirir.
            }
        }

        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string isAd = formCollection["isAd"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);

            Isler yeniIs = new Isler();

            yeniIs.isAd = isAd;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId; //
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;

            entity.Isler.Add(yeniIs);
            entity.SaveChanges(); //

            return RedirectToAction("Takip", "Yonetici");
        }


        public ActionResult Takip()
        {

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller
                                  where p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  select p).ToList();

                ViewBag.personeller = calisanlar;


                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;

                return View();
            }
            else
            {

                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]

        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller where p.personelId == selectPer select p).FirstOrDefault();

            TempData["secilen"] = secilenPersonel;

            return RedirectToAction("Listele", "Yonetici");
        }

        [HttpGet]  //seçilen personel bilgileri listelenecek
        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];
                try
                {
                    var isler = (from i in entity.Isler where i.isPersonelId == secilenPersonel.personelId select i).ToList().OrderByDescending(i => i.iletilenTarih);

                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                    return View();

                }
                catch (Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
           
        }

        
    }    
}

  
