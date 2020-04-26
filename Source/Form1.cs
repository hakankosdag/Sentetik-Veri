using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office;
using Excel = Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using Worksheet = Microsoft.Office.Interop.Excel.Worksheet;
using Workbook = Microsoft.Office.Interop.Excel.Workbook;
using System.IO;
using System.Threading;

namespace DataSet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        List<TextBox> textboxlar = new List<TextBox>();
        private void button1_Click(object sender, EventArgs e)
        {
            int count2 = 1;
            panel1.Controls.Clear();
            textboxlar.Clear();
            if (textBox2.Text != "")
            {
                int count = Convert.ToInt32(textBox2.Text);

                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        TextBox tb = new TextBox();
                        Label lb = new Label();
                        if (j == 0)
                        {


                            tb.BorderStyle = BorderStyle.Fixed3D;
                            tb.Location = new Point(j * 90, 25 * i);
                            tb.Size = new Size(70, 20);
                            textboxlar.Add(tb);
                            panel1.Controls.Add(tb);
                            count2++;
                        }
                        else if (j == 1)
                        {

                            lb.Location = new Point(j * 75, 25 * i);
                            lb.Size = new Size(15, 15);
                            lb.Text = "=";
                            lb.ForeColor = System.Drawing.Color.DarkOrange;
                            lb.Font = new Font("Arial", 10, FontStyle.Bold);
                            panel1.Controls.Add(lb);

                            tb.BorderStyle = BorderStyle.Fixed3D;
                            tb.Location = new Point(j * 100, 25 * i);
                            tb.Size = new Size(40, 20);
                            textboxlar.Add(tb);
                            panel1.Controls.Add(tb);

                        }
                        else
                        {
                            lb.Location = new Point(j * 72, 25 * i);
                            lb.Size = new Size(15, 15);
                            lb.Text = "-";
                            lb.ForeColor = System.Drawing.Color.DarkOrange;
                            lb.Font = new Font("Arial", 10, FontStyle.Bold);
                            panel1.Controls.Add(lb);

                            tb.BorderStyle = BorderStyle.Fixed3D;
                            tb.Location = new Point(j * 80, 25 * i);
                            tb.Size = new Size(40, 20);
                            textboxlar.Add(tb);
                            panel1.Controls.Add(tb);
                        }
                    }


                }
            }
            else
            {
                MessageBox.Show("Lütfen bir öznitelik sayısı giriniz!");
            }


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        islemler islem = new islemler();
        int control = 1;

        public void button2_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "" && textBox3.Text != "")
            {
                islemler.feature.Clear();
                islemler.range.Clear();
                islemler.data.Clear();
                islemler.dataNorm.Clear();
                islemler.x.Clear();
                islemler.y.Clear();
                textBox4.Clear();

                MessageBox.Show("Lütfen bu uyarıyı onaylayın. Veri oluşturulduğunda ikinci bir uyarı mesajı alıcaksınız!");
                int veriCount = Convert.ToInt32(textBox1.Text);
                foreach (TextBox textBox in textboxlar)
                {

                    if (textBox.Text != "")
                    {
                        if (control == 1)
                        {
                            islemler.feature.Add(textBox.Text);
                            control = control - 2;
                        }
                        else
                        {
                            islemler.range.Add(Convert.ToInt32(textBox.Text));
                            control++;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Lütfen öznitelik ve aralıkları giriniz!");
                        control = 5;
                        break;
                    }
                }
                if (comboBox1.SelectedIndex == 1 & control != 5)
                {
                    for (int i = 0; i < islemler.range.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            islem.dogrusalOlmayanVeriOlustur(islemler.range.ElementAt(i), islemler.range.ElementAt(i + 1), veriCount, comboBox2.SelectedIndex);
                        }
                    }


                }
                else if (comboBox1.SelectedIndex == 0 & control != 5)
                {
                    double korelasyon = Convert.ToDouble(textBox3.Text);
                    for (int i = 0; i < islemler.feature.Count - 1; i++)
                    {

                        Task.Factory.StartNew(() => islem.korelasyonHesapla(islemler.range.ElementAt(0), islemler.range.ElementAt(1), veriCount, korelasyon, i / 2)).Wait();
                        Thread.Sleep(1000);

                    }



                    if (comboBox2.SelectedIndex == 1)
                    {
                        islem.minMaxNormalizasyon2();
                    }
                    else if (comboBox2.SelectedIndex == 0)
                    {
                        islem.zScoreNormalizasyon2();
                    }

                }
                if (control != 5 & islemler.dataNorm.Count != 0)
                {
                    var t1 = Task.Run(async () => await islem.regres());
                    t1.Wait();
                    var result = t1.GetType().GetProperty("Result").GetValue(t1).ToString();
                    textBox4.Text = result;

                    exceleKaydet(veriCount);
                }

            }
            else
            {
                MessageBox.Show("Lütfen veri sayısını ve korelasyon katsayısını girdiğinizden emin olunuz !");
            }
        }

        //Oluşturulan veri seti excell yazılır.
        public void exceleKaydet(int veriCount)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application xlOrn = new Microsoft.Office.Interop.Excel.Application();

                Microsoft.Office.Interop.Excel.Workbook wb = xlOrn.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);

                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)xlOrn.ActiveSheet;


                for (int i = 0; i < islemler.feature.Count; i++)
                {
                    ws.Cells[1, i + 1] = islemler.feature.ElementAt(i);
                    for (int j = 0; j < veriCount; j++)
                    {
                        ws.Cells[j + 2, i + 1] = islemler.dataNorm.ElementAt((i * veriCount) + j);
                    }
                }
                string path = Application.StartupPath.ToString() + "\\data" + islemler.rastgele.Next(1, 100) + ".xlsx";
                wb.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal);
                wb.Close(true);


                MessageBox.Show("Excel dosyası " + path + " komununda oluşturuldu! \nDoğrusal regresyon analizi sonucu ise form ekranın da gösterilmiştir.");

            }
            catch (Exception)
            {

                throw;
            }
           
        }


    }
}
