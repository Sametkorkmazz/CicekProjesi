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

    internal class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            Cicek[] CicekListesi = cicekListesiniOlustur();
            Noron[] noronlar = noronlariOlustur();
            agirliklariOlustur(noronlar, random);
            double[,] dogrulukDegerleri = new double[3, 3];
            double[] lamdaDegerleri = { 0.01, 0.005, 0.025 };
            int[] deneyMiktarlari = { 10, 20, 50 };
            for (int i = 0; i < 3; i++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int j = 0; j < deneyMiktarlari[i]; j++)
                    {
                        for (int k = 0; k < 150; k++)
                        {
                            hesaplamaYap(noronlar, CicekListesi, k, lamdaDegerleri[m]);
                            noronKontrol(noronlar, CicekListesi, k, lamdaDegerleri[m]);
                        }
                    }

                    for (int j = 0; j < 150; j++)
                    {
                        hesaplamaYap(noronlar, CicekListesi, j, lamdaDegerleri[m]);
                        dogrulukDegerleri[i, m] += dogrulukKontrol(noronlar, CicekListesi, j, lamdaDegerleri[m]);
                    }

                    dogrulukDegerleri[i, m] = (dogrulukDegerleri[i, m] / 150) * 100;
                    agirliklariOlustur(noronlar, random);
                }
            }

            Console.Write("Dogruluk Degerleri:\n");
            Console.Write(string.Format("{0,25}{1,19}{2,19}\n", lamdaDegerleri[0], lamdaDegerleri[1],
                lamdaDegerleri[2]));
            for (int i = 0; i < 3; i++)
            {
                Console.Write("" + deneyMiktarlari[i] + "epok");
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(string.Format("{0,19:0.##}", dogrulukDegerleri[i, j]));
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
                    CicekListesi[i].ozellikleri[j] = double.Parse(dataDizisi[i][j], CultureInfo.InvariantCulture);
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

            switch (cicekListesi[index].ad)
            {
                case "Iris-setosa":
                    if (enBuyukNoron != 0)
                    {
                        agırlıkDegistir(noronlar, enBuyukNoron, index, lamda);
                    }

                    break;

                case "Iris-versicolor":
                    if (enBuyukNoron != 1)
                    {
                        agırlıkDegistir(noronlar, enBuyukNoron, index, lamda);
                    }

                    break;

                case "Iris-virginica":
                    if (enBuyukNoron != 2)
                    {
                        agırlıkDegistir(noronlar, enBuyukNoron, index, lamda);
                    }

                    break;
            }
        }

        static void agırlıkDegistir(Noron[] noronlar, int enBuyukDeger, int index, double lamda)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i == enBuyukDeger)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        noronlar[i].agırlıklar[index, j] -= lamda;
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        noronlar[i].agırlıklar[index, j] += lamda;
                    }
                }
            }
        }

        static int dogrulukKontrol(Noron[] noronlar, Cicek[] cicekListesi, int index, double lamda)
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
    }
}