using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sanfor_Tartım_V2;
using System.Threading;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;

namespace Sanfor_Tartım_V2
{
    public partial class Form2 : Form
    {
        SanforKantar sfk = new SanforKantar();
        SqlConnection con;
        SqlCommand cmd;
        public void satirGetir()
        {
            con = new SqlConnection("Server=192.168.10.250;Database=SentezLive;Uid=sa;Password=boyteks123***;");
            cmd = new SqlCommand();
            cmd.Connection = con;

            // Burada datable içerisine o iş emrine ait satırları getirme işini yüklüyoruz.


            con.Open();
            cmd.CommandText = "SELECT OrderNo,replace(str(GrossQuantity, 10, 2), ' ', '') AS GrossQuantity,replace(str(NetQuantity, 10, 2), ' ', '') AS NetQuantity,replace(str(TareQuantity, 10, 2), ' ', '') AS TareQuantity from Erp_InventorySerialCard WHERE WorkOrderId = @woid  AND WorkOrderReceiptItemId = @sid";
            cmd.Parameters.AddWithValue("woid", PartiBilgileri.PartiId);
            cmd.Parameters.AddWithValue("sid", PartiBilgileri.SatirId);
            cmd.CommandType = CommandType.Text;

            //musteriler tablosundaki tüm kayıtları çekecek olan sql sorgusu.

            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "TOP NO";
            dataGridView1.Columns[1].HeaderText = "TOPLAM KİLO";
            dataGridView1.Columns[2].HeaderText = "NET KİLO";
            dataGridView1.Columns[3].HeaderText = "DARA";
            con.Close();
        }


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            personelid.Text = Personel.Personelid.ToString();
            personeladi.Text = Personel.personeladi;
            personelsoyadi.Text = Personel.personelsoyadi;
            saat.Text = "SAAT :" + DateTime.Now.ToShortTimeString();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            saat.Text = "SAAT :" + DateTime.Now.ToShortTimeString();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                
                if(textBox1.Text.Length == 12)
                {
                    sfk.KullanicidanGelen(textBox1.Text);
                    partiText.Text = PartiBilgileri.PartiNo;
                    satirText.Text = PartiBilgileri.SatirNo;
                    musteriText.Text = PartiBilgileri.MusteriAdi;
                    enText.Text = PartiBilgileri.En.ToString();
                    gramajText.Text = PartiBilgileri.Gramaj.ToString();
                    kumasText.Text = PartiBilgileri.KumasAdi;
                    renkKoduText.Text = PartiBilgileri.RenkKodu;
                    renkAdiText.Text = PartiBilgileri.RenkAdi;
                    
                    satirGetir();
                }
                else if (textBox1.Text.Substring(0, 6) == "etiket")
                {
                    sfk.KullanicidanGelen(textBox1.Text);
                    etiketText.Text = PartiBilgileri.EtiketAdi;
                } else if (textBox1.Text == "NUMUNE")
                {
                    sfk.KullanicidanGelen(textBox1.Text);
                }
                 
                

                
              
            } 
        }
    }
}
