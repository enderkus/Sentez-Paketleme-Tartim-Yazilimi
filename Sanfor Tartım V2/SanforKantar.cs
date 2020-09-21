using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.Windows.Forms;
using Sanfor_Tartım_V2;
using System.Diagnostics.Eventing.Reader;
using RawDataPrint;
using System.IO;

namespace Sanfor_Tartım_V2
{
    class SanforKantar
    {
        
        PartiBilgileri pb = new PartiBilgileri();
        SqlConnection con;
        SqlCommand cmd;
        int personelid;
        string personeladi;
        string personelsoyadi;
        public string[] parcalar;
        public string partino;
        public int satirno;
        public int partiid;
        public int musteriid;
        public string siparisno;
        public int receteid;
        public string musteriadi;
        public int satirid;
        public int kumasid;
        public int en;
        public int gramaj;
        public string kumasadi;
        public string renkkodu;
        public string renkadi;

        
    

        public void baglanti() {
            
            con = new SqlConnection("Server=192.168.10.250;Database=SentezLive;Uid=sa;Password=boyteks123***;");
            cmd = new SqlCommand();
            cmd.Connection = con;
            
        }

        public bool personelsorgula(string personelkodu)
        {
            // Bağlantı nesenesini çağırarak sql işlemlerini yapıyoruz.
            baglanti();
            // Personel kodu SPR*** şeklinde geliyor bu yüzden baştaki S harfini boşluk ile değiştirip daha sonra trim ile boşlukları temizledik.
            string personelkodu2 = personelkodu.Replace('S',' ').Trim();
            // Personel kodu üzerinden veri tabanında sayım yaptırıyoruz.Böylece varlığını kontrol etmiş oluyoruz.
            cmd.CommandText = "SELECT COUNT(RecId) FROM Erp_Employee WHERE EmployeeCode = @perkod";
            cmd.CommandType = CommandType.Text;
            // Değişiklik yaptığımız gelen veriyi @perkod ile değiştiriyoruz.
            cmd.Parameters.AddWithValue("@perkod", personelkodu2);

            // Bağlantı açtık
            con.Open();
            // Sql sorgumuzu çalıştırdık.
            object donen = cmd.ExecuteScalar();

            // Burada sayım sorgusu sonrası kontrol yapıyoruz.

            if(Convert.ToInt32(donen) > 0) {
                // Eğer dönen bilgi 0 dan büyükse true döndürüyoruz.
                return true;
            } else
            {
                // Eğer dönen değeri 0 dan büyük değilse hata bastıracak ve false döndürecek.
                MessageBox.Show("GEÇERSİZ BİR PERSONEL KODU GİRDİNİZ !","HATA !", MessageBoxButtons.OK,MessageBoxIcon.Error);
                return false;
            }
        }

        public void personelbilgileri(string personelkodu) {
            
            // Bağlantı nesenesini çağırarak sql işlemlerini yapıyoruz.
            baglanti();
            // Personel kodu SPR*** şeklinde geliyor bu yüzden baştaki S harfini boşluk ile değiştirip daha sonra trim ile boşlukları temizledik.
            string personelkodu2 = personelkodu.Replace('S', ' ').Trim();
            // Eğer kullanıcı varsa bilgilerini çekiyoruz.
            cmd.CommandText = "SELECT * FROM Erp_Employee WHERE EmployeeCode = @perkod";
            cmd.CommandType = CommandType.Text;
            // Personel kodu bilgisini parametremize aktarıyoruz.
            cmd.Parameters.AddWithValue("perkod",personelkodu2);

            // Sql bağlantısını açtık.
            con.Open();
            // Sql sonucunda dönen bilgileri read ediyoruz.
            SqlDataReader personelbilgileri = cmd.ExecuteReader();
            // While ile gelen bilgileri alıyoruz.
            while(personelbilgileri.Read())
            {
                // Sql'den gelen personel RecId(Kayıt değeri) bilgisini personelid değişkenine atıyoruz.
                personelid = Convert.ToInt32(personelbilgileri["RecId"]);
                // Sql'den gelen personel adı bilgisini personeladi değişkenine aktarıyoruz.
                personeladi = personelbilgileri["EmployeeName"].ToString();
                // Sql'den gelen personel soyadı bilgisini personelsoyadi değişkenine aktarıyoruz.
                personelsoyadi = personelbilgileri["EmployeeSurname"].ToString();
            }
            // Personel bilgileri reader ini kapatıyoruz.
            personelbilgileri.Close();
            // Sql komutunu dispose ediyoruz.
            cmd.Dispose();
            // Sql bağlantısını kapattık.
            con.Close();
            // Personel sınıfındaki personelid değişkenine sql den gelen personelid bilgisini set ediyoruz.
            Personel.Personelid = personelid;
            // Personel sınıfındaki personeladi değişkenine sql den gelen personeladi bilgisini set ediyoruz.
            Personel.personeladi = personeladi;
            // Personel sınıfındaki personelsoyadi değişkenine sql den gelen personelsoyadi bilgisini set ediyoruz.
            Personel.personelsoyadi = personelsoyadi;
     
        }


        public void KullanicidanGelen(string kullaniciverisi)
        {
            if(kullaniciverisi.Length == 12)
            {
                baglanti();
                // Kullanıcıdan gelen 02-200123-1 gibi pari ve satır numarasını aldık ve parçaladık.
                 parcalar = kullaniciverisi.Split('-');
                 partino = parcalar[0] + "-" + parcalar[1];
                 satirno = Convert.ToInt32(parcalar[2]);
                con.Open();
                // Parti bilgilerini alıyoruz.
                cmd.CommandText = "SELECT * FROM Erp_WorkOrder WHERE WorkOrderNo = @workorderno";
                cmd.Parameters.AddWithValue("workorderno",partino);
                cmd.CommandType = CommandType.Text;
                SqlDataReader partibilgileri = cmd.ExecuteReader();

                while (partibilgileri.Read())
                {
                    partiid = Convert.ToInt32(partibilgileri["RecId"]);
                    musteriid = Convert.ToInt32(partibilgileri["CurrentAccountId"]);
                    siparisno = partibilgileri["CustomerOrderNo"].ToString();
                    receteid = Convert.ToInt32(partibilgileri["LabRecipeId"]);
                }

                partibilgileri.Close();
                cmd.Dispose();
                con.Close();

                // Parti no üzerinden aldığımız bilgileri diğer tablolardan sorgulayıp alıyoruz.


                // Bu alanda satır bilgilerini alacağız.

                con.Open();

                cmd.CommandText = "SELECT * FROM Erp_WorkOrderItem WHERE WorkOrderId = @workorderid";
                cmd.Parameters.AddWithValue("workorderid",partiid);
                cmd.CommandType = CommandType.Text;
                SqlDataReader satirbilgileri = cmd.ExecuteReader();

                while (satirbilgileri.Read())
                {
                    satirid = Convert.ToInt32(satirbilgileri["RecId"]);
                    kumasid = Convert.ToInt32(satirbilgileri["InventoryId"]);
                    en = Convert.ToInt32(satirbilgileri["FabricGram"]);
                    gramaj = Convert.ToInt32(satirbilgileri["FabricWidth"]);
                }

                satirbilgileri.Close();
                cmd.Dispose();
                con.Close();

                // Bu alanda kumaş adını alacağız.

                con.Open();

                cmd.CommandText = "SELECT * FROM Erp_Inventory WHERE RecId = @kumasid";
                cmd.Parameters.AddWithValue("kumasid",kumasid);
                cmd.CommandType = CommandType.Text;
                SqlDataReader kumasbilgileri = cmd.ExecuteReader();

                while(kumasbilgileri.Read())
                {
                    kumasadi = kumasbilgileri["InventoryName"].ToString();
                }


                kumasbilgileri.Close();
                cmd.Dispose();
                con.Close();

                // Bu alanda renk kodu ve renk adı bilgilerini alacağız.

                con.Open();

                cmd.CommandText = "SELECT * FROM Erp_LabRecipe WHERE RecId = @receteid";
                cmd.Parameters.AddWithValue("receteid",receteid);
                cmd.CommandType = CommandType.Text;
                SqlDataReader recetebilgileri = cmd.ExecuteReader();

                while(recetebilgileri.Read())
                {
                    renkkodu = recetebilgileri["LabRecipeCode"].ToString();
                    renkadi = recetebilgileri["LabRecipeName"].ToString();
                }

                recetebilgileri.Close();
                cmd.Dispose();
                con.Close();



                // Bu alanda müşteri adını alacağız.
                con.Open();

                cmd.CommandText = "SELECT * FROM Erp_CurrentAccount WHERE RecId = @musteriid";
                cmd.Parameters.AddWithValue("musteriid",musteriid);
                cmd.CommandType = CommandType.Text;
                SqlDataReader musteribilgileri = cmd.ExecuteReader();

                while(musteribilgileri.Read())
                {
                    musteriadi = musteribilgileri["CurrentAccountName"].ToString();
                }


                musteribilgileri.Close();
                cmd.Dispose();
                con.Close();

                // Gelen bilgileri PartiBilgileri sınfımıza aktarıyoruz. Oradan da Form2 içerisine yazılacak.
                PartiBilgileri.PartiNo = partino;
                PartiBilgileri.SatirNo = satirno.ToString();
                PartiBilgileri.MusteriAdi = musteriadi;
                PartiBilgileri.SatirId = satirid;
                PartiBilgileri.KumasId = kumasid;
                PartiBilgileri.En = en;
                PartiBilgileri.Gramaj = gramaj;
                PartiBilgileri.KumasAdi = kumasadi;
                PartiBilgileri.RenkKodu = renkkodu;
                PartiBilgileri.RenkAdi = renkadi;
                PartiBilgileri.PartiId = partiid;
            } else if(kullaniciverisi.Substring(0,6) == "etiket")
            {
                PartiBilgileri.EtiketAdi = kullaniciverisi;
            } else if (kullaniciverisi == "NUMUNE")
            {
                Form2 frm2 = new Form2();
                String tempFile = frm2.etiketText.Text + ".prn";
                String PrinterName = Properties.Settings.Default.yazici.ToString();
                StreamReader SR = new StreamReader(tempFile, Encoding.Default);
                String all = SR.ReadToEnd();
                SR.Close();
                all = all.Replace("{CariAdi}", musteriadi);
                all = all.Replace("{StokAdi}", kumasadi);
                all = all.Replace("{PartiNo}", partino);
                all = all.Replace("{PartiSatir}", satirno.ToString());
                all = all.Replace("{RenkAdi}", renkadi);
                all = all.Replace("{RenkKodu}", renkkodu);
                all = all.Replace("{En}", en.ToString());
                all = all.Replace("{Grm}", gramaj.ToString());
                all = all.Replace("{BrutYazi}", "0");
                all = all.Replace("{NetYazi}", "0");
                all = all.Replace("{SiparisNo}", siparisno);
                all = all.Replace("{SysTarih}", DateTime.Now.ToString());
                all = all.Replace("{TopSira}", "0");
                RawPrinterHelper.SendStringToPrinter(PrinterName, all);
                // Gelen metin numune ise numune etiketi yazdıracağımız alan
            }
        }

       

        










    }
}
