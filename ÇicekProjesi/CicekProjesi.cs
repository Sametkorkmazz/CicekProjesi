using System;
using System.Globalization;

namespace ÇicekProjesi
{
    class Cicek
    {
        public double[] ozellikleri = new double[4];
        public string ad;
    }

    class Noron
    {
        public double[,] agırlıklar = new double[150, 4];
        public double toplam;
    }

    internal class CicekProjesi
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            while (true)
            {
                tekSetDeneyYap(random);
                Console.ReadLine();
                Console.Clear();
            }
        }

        static void tekSetDeneyYap(Random random)
        {

            double[] lamdaDegerleri = { 0.01, 0.005, 0.025 };
            int[] deneyMiktarlari = { 50, 20, 100 };
            Cicek[] CicekListesi = cicekListesiniOlustur();
            Noron[] noronlar = noronlariOlustur();
            agirliklariOlustur(noronlar, random);
            Console.Write("Deney Sayisi: ");
            int deneySayisi = Int32.Parse(Console.ReadLine());
            Console.Clear();
            for (int i = 0; i < deneySayisi; i++)
            {
                double[,] dogrulukDegerleri = new double[3, 3];
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < deneyMiktarlari[k]; l++)
                        {
                            for (int m = 0; m < 150; m++)
                            {
                                hesaplamaYap(noronlar, CicekListesi, m, lamdaDegerleri[j]);
                                noronKontrol(noronlar, CicekListesi, m, lamdaDegerleri[j]);
                            }
                        }

                        for (int l = 0; l < 150; l++)
                        {
                            hesaplamaYap(noronlar, CicekListesi, l, lamdaDegerleri[j]);
                            dogrulukDegerleri[k, j] += dogrulukKontrol(noronlar, CicekListesi, l);
                        }

                        dogrulukDegerleri[k, j] = (dogrulukDegerleri[k, j] / 150) * 100;
                        agirliklariOlustur(noronlar, random);
                    }
                }

                sonuclariYaz(deneyMiktarlari, lamdaDegerleri, dogrulukDegerleri, i);
            }
        }

        static void sonuclariYaz(int[] deneyMiktarlari, double[] lamdaDegerleri, double[,] dogrulukDegerleri, int deney)
        {
            int[] sira = { 1, 0, 2 };

            Console.Write("{0}.Deney Dogruluk Degerleri:\n\n", deney + 1);
            Console.Write(string.Format("{0,28}λ {1,17}λ {2,17}λ\n\n", lamdaDegerleri[1], lamdaDegerleri[0],
                lamdaDegerleri[2]));
            for (int i = 0; i < 3; i++)
            {
                string epok = $"{deneyMiktarlari[sira[i]]} Epok";
                Console.Write(string.Format("{0,-10}", epok));
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(string.Format("{0,18:0.##}%", dogrulukDegerleri[sira[i], sira[j]]));
                }

                Console.Write("\n\n");
            }
        }

        static void agirliklariOlustur(Noron[] noronlar, Random random)
        {
            double sayi;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 150; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        while (true)
                        {
                            sayi = random.NextDouble();
                            if (sayi != 0)
                            {
                                break;
                            }
                        }

                        noronlar[i].agırlıklar[j, k] = Math.Round(sayi, 1);
                    }
                }
            }
        }

        static Cicek[] cicekListesiniOlustur()
        {
            string[] dosyaOkuma =
                System.IO.File.ReadAllLines(@"C:\Users\debim\source\repos\ÇicekProjesi\ÇicekProjesi\iris.data");
            string[][] dataDizisi = new string[150][];
            for (int i = 0; i < 150; i++)
            {
                dataDizisi[i] = new string[5];
                dataDizisi[i] = dosyaOkuma[i].Split(',');
            }

            Cicek[] CicekListesi = new Cicek[150];
            for (int i = 0; i < 150; i++)
            {
                CicekListesi[i] = new Cicek();
            }

            for (int i = 0; i < 150; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    CicekListesi[i].ozellikleri[j] =
                        (double.Parse(dataDizisi[i][j], CultureInfo.InvariantCulture)) / 10;
                }

                CicekListesi[i].ad = dataDizisi[i][4];
            }

            return CicekListesi;
        }

        static Noron[] noronlariOlustur()
        {
            Noron[] noronlar = new Noron[3];
            for (int i = 0; i < 3; i++)
            {
                noronlar[i] = new Noron();
            }

            return noronlar;
        }

        static void hesaplamaYap(Noron[] noronlar, Cicek[] cicekListesi, int index, double lamda)
        {
            double toplam;
            for (int i = 0; i < 3; i++)
            {
                toplam = 0;
                for (int j = 0; j < 4; j++)
                {
                    toplam += cicekListesi[index].ozellikleri[j] * noronlar[i].agırlıklar[index, j];
                }

                noronlar[i].toplam = toplam;
            }
        }

        static void noronKontrol(Noron[] noronlar, Cicek[] cicekListesi, int index, double lamda)
        {
            int[] degisimNoronları = new int[2];
            double enBuyukDeger = noronlar[0].toplam;
            int enBuyukNoron = 0;
            for (int i = 0; i < 3; i++)
            {
                if (enBuyukDeger < noronlar[i].toplam)
                {
                    enBuyukDeger = noronlar[i].toplam;
                    enBuyukNoron = i;
                }
            }

            degisimNoronları[0] = enBuyukNoron;
            switch (cicekListesi[index].ad)
            {
                case "Iris-setosa":
                    if (enBuyukNoron != 0)
                    {
                        degisimNoronları[1] = 0;
                        agırlıkDegistir(cicekListesi, noronlar, degisimNoronları, index, lamda);
                    }

                    break;

                case "Iris-versicolor":
                    if (enBuyukNoron != 1)
                    {
                        degisimNoronları[1] = 1;
                        agırlıkDegistir(cicekListesi, noronlar, degisimNoronları, index, lamda);
                    }

                    break;

                case "Iris-virginica":
                    if (enBuyukNoron != 2)
                    {
                        degisimNoronları[1] = 2;
                        agırlıkDegistir(cicekListesi, noronlar, degisimNoronları, index, lamda);
                    }

                    break;
            }
        }

        static void agırlıkDegistir(Cicek[] cicekListesi, Noron[] noronlar, int[] degisenNoronlar, int index,
            double lamda)
        {
            double x;
            for (int i = 0; i < 4; i++)
            {
                x = cicekListesi[index].ozellikleri[i];
                noronlar[degisenNoronlar[0]].agırlıklar[index, i] -= (lamda * x);
                noronlar[degisenNoronlar[1]].agırlıklar[index, i] += (lamda * x);
            }
        }

        static int dogrulukKontrol(Noron[] noronlar, Cicek[] cicekListesi, int index)
        {
            double enBuyukDeger = 0;
            int enBuyukNoron = 0;
            for (int i = 0; i < 3; i++)
            {
                if (enBuyukDeger < noronlar[i].toplam)
                {
                    enBuyukDeger = noronlar[i].toplam;
                    enBuyukNoron = i;
                }
            }

            switch (cicekListesi[index].ad)
            {
                case "Iris-setosa":
                    if (enBuyukNoron == 0)
                    {
                        return 1;
                    }

                    return 0;
                case "Iris-versicolor":
                    if (enBuyukNoron == 1)
                    {
                        return 1;
                    }

                    return 0;

                case "Iris-virginica":
                    if (enBuyukNoron == 2)
                    {
                        return 1;
                    }

                    return 0;
            }

            return 0;
        }

        static void siralamaYap(double[] lamdaListesi, int[] deneyListesi)
        {
            for (int i = 0; i < lamdaListesi.Length; i++)
            {
            }
        }
    }
}