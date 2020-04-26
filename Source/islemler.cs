using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using System.Net.Http;
using System.Web.Script.Serialization;


namespace DataSet
{
    class islemler
    {
        private static readonly HttpClient client = new HttpClient();
        
        static public List<float> data = new List<float>();
        static public List<float> dataTemp = new List<float>();
        static public List<double> dataNorm = new List<double>();
        static public List<float> ortFeatures = new List<float>();
        static public Random rastgele = new Random();
        static public List<String> feature = new List<String>();
        static public List<int> range = new List<int>();


        static public List<float> x = new List<float>(); 
        static public List<List<float>> y = new List<List<float>>();

        public void dogrusalOlmayanVeriOlustur(int sinir1, int sinir2, int dataCount,int selectIndex)
        {
            dataTemp.Clear();
            int a = 0;
            for (int i = 0; i < dataCount; i++)
            {
                a = rastgele.Next(sinir1, sinir2);
                data.Add(a);
                dataTemp.Add(a);
            }
            if (selectIndex == 1)
            {
                minMaxNormalizasyon();
            }
            else if (selectIndex == 0)
            {
                zScoreNormalizasyon();
            }
        }

        public void minMaxNormalizasyon()
        {
            double min = dataTemp.Min();
            double max = dataTemp.Max();
            double temp = 0;
            for (int i = 0; i < dataTemp.Count; i++)
            {
                temp = dataTemp.ElementAt(i);
                dataNorm.Add((temp - min) / (max - min));
            }

        }

        public void minMaxNormalizasyon2()
        {
            double min = x.Min();
            double max = x.Max();
            double temp = 0;
            for (int i = 0; i < x.Count; i++)
            {
                temp = x.ElementAt(i);
                dataNorm.Add((temp - min) / (max - min));
            }

            for (int i = 0; i < y.Count; i++)
            {
                for (int j = 0; j < y.ElementAt(i).Count; j++)
                {
                    min = y.ElementAt(i).Min();
                    max = y.ElementAt(i).Max();

                    temp = y.ElementAt(i).ElementAt(j);
                    dataNorm.Add((temp - min) / (max - min));
                }               
            }

        }

        public void zScoreNormalizasyon()
        {
            float avrg = dataTemp.Average();

            double st = Math.Sqrt(dataTemp.Sum(x => Math.Pow(x - avrg, 2)) / dataTemp.Count);

            float temp = 0;
            for (int i = 0; i < dataTemp.Count; i++)
            {
                temp = dataTemp.ElementAt(i);
                dataNorm.Add((temp - avrg) / st);
            }

        }

        public void zScoreNormalizasyon2()
        {

            float avrg = x.Average();

            double st = Math.Sqrt(x.Sum(x => Math.Pow(x - avrg, 2)) / x.Count);

            float temp = 0;
            for (int i = 0; i < x.Count; i++)
            {
                temp = x.ElementAt(i);
                dataNorm.Add((temp - avrg) / st);
            }

            for (int i = 0; i < y.Count; i++)
            {
                for (int j = 0; j < y.ElementAt(i).Count; j++)
                {
                    avrg = y.ElementAt(i).Average();

                    temp = y.ElementAt(i).ElementAt(j);
                    dataNorm.Add((temp - avrg) / st);
                }
            }

        }



        public void korelasyonHesapla(int sinir1, int sinir2, int vericount, double korelasyon,int sy)
        {

            double r = 0;
            float temp = 0;
            double tempx = 0, tempy = 0;
            double r2 = 0;
            int j = 0;

            
            while (r2 != korelasyon)
            {

                Console.WriteLine("aynı değil " + r + " " + r2 + " " + korelasyon);


                Task.Factory.StartNew(() => sendReq(vericount, korelasyon, sinir1, sinir2)).Wait();

                Thread.Sleep(2000);

                try
                {
                    float ortx = ortFeatures.ElementAt(0);
                    float orty = ortFeatures.ElementAt(1);
             

                    for (int i = 0; i < vericount; i++)
                    {
                        //yukarıda paylaşmış olduğumuz katsayı formülümüzün
                        //pay kısmını temp değişkeni yardımıyla topluyoruz
                        temp += (x.ElementAt(i) - ortx) * (y.ElementAt(sy).ElementAt(i) - orty);

                        //payda kısmında ise toplam işlemleri hem x hem de y göre
                        //ayrı ayrı yapıyoruz
                        //Math.Pow(a,b) üssü işlemi için kullanılan bir metotdur.
                        tempx += Math.Pow((x.ElementAt(i) - ortx), 2);
                        tempy += Math.Pow((y.ElementAt(sy).ElementAt(i) - orty), 2);
                    }
                    //Toplam işlemlerimizi yaptık
                    //katsayı değişkenimizi oluşturalım
                    // Math.Sqrt Köklü ifadeler için kullanılan matematik metodudur.
                    r = (temp / Math.Sqrt(tempx * tempy));

                    r2 = korelasyon;
                    j++;

                }
                catch (Exception)
                {
                    break;
                    throw;
                }
            }
            Console.WriteLine("bulundu");
            
        }

        public async void sendReq(int n, double r, int min, int max)
        {
            ortFeatures.Clear();
            if (x.Count == 0)
            {
                
                var values = new Dictionary<string, string>
                {
                    { "LanguageChoiceWrapper", "31" },
                    { "EditorChoiceWrapper", "1" },
                    { "LayoutChoiceWrapper", "1" },
                    { "Program", "n="+n.ToString()+";r="+r.ToString().Replace(",",".")+";x1=(runif(n,min="+min.ToString()+",max="+max.ToString()+"));x2=(runif(n,min="+min.ToString()+",max="+max.ToString()+")); y1=scale(x2)*r+scale(residuals(lm(x1~x2)))*sqrt(1-r*r);v<-data.frame(x=x2,y=y1);v" },
                    { "Input", "" },
                    { "Privacy", "" },
                    { "PrivacyUsers", "" },
                    { "Title", "" },
                    { "SavedOutput", "" },
                    { "WholeError", "" },
                    { "WholeWarning", "" },
                    { "StatsToSave", "" },
                    { "CodeGuid", "" },
                    { "IsInEditMode", "False" },
                    { "IsLive", "False" },
                };

                var content = new FormUrlEncodedContent(values);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var response = await client.PostAsync("https://rextester.com/rundotnet/Run", content);

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic a = serializer.Deserialize<dynamic>(responseString);
                string nresult = a["Result"].ToString();
                string[] slices = nresult.Split(' ');


                List<float> ynew = new List<float>(); 

                for (int i = 18; i < slices.Length; i++)
                {
                    if (slices[i] != "" & !slices[i].Contains("y\n1"))
                    {
                        if (slices[i].Any(char.IsDigit))
                        {
                            if (slices[i].Contains('\n'))
                            {
                                ynew.Add(float.Parse((slices[i].Split('\n'))[0].Replace(".", ",")));
                            }
                            else
                            {
                                x.Add(float.Parse(slices[i].Replace(".", ",")));
                            }
                        }
                    }
                }
                ortFeatures.Add(x.Average());
                ortFeatures.Add(ynew.Average());
                y.Add(ynew);

            }
            else
            {
                string newx = string.Join("|", x.ToArray());
                newx = newx.Replace(",", ".").Replace("|", ",");
                var values = new Dictionary<string, string>
                {
                    { "LanguageChoiceWrapper", "31" },
                    { "EditorChoiceWrapper", "1" },
                    { "LayoutChoiceWrapper", "1" },
                    { "Program", "n="+n.ToString()+";r="+r.ToString().Replace(",",".")+";x1=(runif(n,min="+min.ToString()+",max="+max.ToString()+"));x2=(c("+newx+")); y1=scale(x2)*r+scale(residuals(lm(x1~x2)))*sqrt(1-r*r);v<-data.frame(x=x2,y=y1);v" },
                    { "Input", "" },
                    { "Privacy", "" },
                    { "PrivacyUsers", "" },
                    { "Title", "" },
                    { "SavedOutput", "" },
                    { "WholeError", "" },
                    { "WholeWarning", "" },
                    { "StatsToSave", "" },
                    { "CodeGuid", "" },
                    { "IsInEditMode", "False" },
                    { "IsLive", "False" },
                };
                var content = new FormUrlEncodedContent(values);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var response = await client.PostAsync("https://rextester.com/rundotnet/Run", content);

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic a = serializer.Deserialize<dynamic>(responseString);
                string nresult = a["Result"].ToString();
                string[] slices = nresult.Split(' ');


                List<float> ynew = new List<float>();  


                for (int i = 18; i < slices.Length; i++)
                {
                    if (slices[i] != "" & !slices[i].Contains("y\n1"))
                    {
                        if (slices[i].Any(char.IsDigit))
                        {
                            if (slices[i].Contains('\n'))
                            {
                                ynew.Add(float.Parse((slices[i].Split('\n'))[0].Replace(".", ",")));
                            }
                        }
                    }
                }
                ortFeatures.Add(x.Average());
                ortFeatures.Add(ynew.Average());
                y.Add(ynew);
            }          
        }


        public async Task<string> regres()
        {
            List<string> post = new List<string>();
            string newx = string.Join("|", x.ToArray());
            newx = newx.Replace(",", ".").Replace("|", ",");
            string ilksatir = "x0=(c(" + newx + "));";
            string sonsatir = "dogrusal_model<-lm(x0~";


            for (int i = 1; i <= y.Count; i++)
            {
                string alım = "";

                // MessageBox.Show(yler[i].ToString());
                var tmp = y[i - 1];

                for (int j = 0; j < tmp.Count; j++)
                {
                    alım += tmp[j].ToString().Replace(",", ".") + ",";
                }
                alım = alım.Remove(alım.Length - 1);
                string degisen = "";
                degisen += "y" + i.ToString() + "=(c(" + alım + "));";
                post.Add(degisen);
                sonsatir += "+y" + i.ToString();
            }
            sonsatir += ");dogrusal_model;".Replace("~+", "~");
            string son = ilksatir + string.Join("", post.ToArray()) + sonsatir;
            var values = new Dictionary<string, string>
                {
                    { "LanguageChoiceWrapper", "31" },
                    { "EditorChoiceWrapper", "1" },
                    { "LayoutChoiceWrapper", "1" },
                    { "Program", son },
                    { "Input", "" },
                    { "Privacy", "" },
                    { "PrivacyUsers", "" },
                    { "Title", "" },
                    { "SavedOutput", "" },
                    { "WholeError", "" },
                    { "WholeWarning", "" },
                    { "StatsToSave", "" },
                    { "CodeGuid", "" },
                    { "IsInEditMode", "False" },
                    { "IsLive", "False" },
                };
            var content = new FormUrlEncodedContent(values);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var response = await client.PostAsync("https://rextester.com/rundotnet/Run", content);

            var responseString = await response.Content.ReadAsStringAsync();
            dynamic a = serializer.Deserialize<dynamic>(responseString);
            string nresult = a["Result"].ToString();
            string[] slices = nresult.Split(' ');
            string denklem = "y=";
            int xkac = 0;
            foreach (string slice in slices)
            {
                if (slice != "" & !slice.Any(char.IsLetter) & slice.Any(char.IsDigit))
                {
                    if (xkac == 0)
                    {
                        denklem += slice;

                    }
                    else
                    {
                        if (slice.Contains("-"))
                        {
                            denklem += slice + "(x" + xkac.ToString()+")";
                        }
                        else
                        {
                            denklem += "+" + slice + "(x" + xkac.ToString()+")";
                        }

                    }
                    xkac++;
                }
            }
            return denklem;
        }
    }
}
