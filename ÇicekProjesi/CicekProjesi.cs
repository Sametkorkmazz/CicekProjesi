using System;
using System.Globalization;
using System.IO;

namespace ÇicekProjesi
{
    class Cicek // Her çiçeğin ait olacağı Çiçek sınıfı
    {
        public double[] ozellikleri = new double[4];
        public string ad;
    }

    class Neuron // Nöron sınıfı
    {
        public double[] agırlıklar = new double[4];
        public double toplam;
    }

    class NeuralNetwork // Yapay sinir ağı sınıfı
    {
        static readonly Random random = new Random();
        static readonly Neuron[] nöronlar = new Neuron[3]; // Sırasıyla 1, 2 ve 3. Nöronu içeren liste

        public NeuralNetwork()
        {
            for (int i = 0; i < 3; i++)
            {
                nöronlar[i] = new Neuron(); // 3 Nöron için bellekte yer açılımı
            }
        }

        public void programiEgit() // Ana method
        {
            double[] LambdaDegerleri = { 0.01, 0.005, 0.025 };
            int[] EpokMiktarlari = { 50, 20, 100 };
            Cicek[]
                CicekListesi =
                    cicekListesiniOlustur(); // Elemanları Çiçek sınıfından oluşan ,150 elemanlı ÇiçekListesi oluşturma
            Console.Clear();
            for (int tabloDeneyi = 0; tabloDeneyi < 3; tabloDeneyi++) // 3 tablo oluşturmak için en dış döngü
            {
                double[,]
                    dogrulukDegerleri =
                        new double[3,
                            3]; // Doğruluk yüzdelerinin tutulacağı 2 boyutlu dizi, satırlar epok değerleri sütünlar lambda değerleri için
                for (int Lambdaİndexi = 0; Lambdaİndexi < 3; Lambdaİndexi++) // LambdaDegerleri dizisini gezen index. 
                {
                    for (int Epokİndexi = 0; Epokİndexi < 3; Epokİndexi++) // EpokMiktarlari dizisini gezen index
                    {
                        agirliklariOlustur(random); // Her
                        for (int l = 0; l < EpokMiktarlari[Epokİndexi]; l++)
                        {
                            for (int m = 0; m < 150; m++)
                            {
                                hesaplamaYap(CicekListesi, m, LambdaDegerleri[Lambdaİndexi]);
                                noronKontrol(CicekListesi, m, LambdaDegerleri[Lambdaİndexi]);
                            }
                        }

                        for (int l = 0; l < 150; l++)
                        {
                            hesaplamaYap(CicekListesi, l, LambdaDegerleri[Lambdaİndexi]);
                            dogrulukDegerleri[Epokİndexi, Lambdaİndexi] += dogrulukKontrol(CicekListesi, l);
                        }

                        dogrulukDegerleri[Epokİndexi, Lambdaİndexi] =
                            (dogrulukDegerleri[Epokİndexi, Lambdaİndexi] / 150) * 100;
                    }
                }

                sonuclariYaz(EpokMiktarlari, LambdaDegerleri, dogrulukDegerleri, tabloDeneyi);
            }
        }


        static void agirliklariOlustur(Random random)
        {
            double sayi;
            for (int i = 0; i < 3; i++)
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

                    nöronlar[i].agırlıklar[k] = Math.Round(sayi, 1);
                }
            }
        }

        static Cicek[] cicekListesiniOlustur()
        {
            string[] dosyaOkuma =
                File.ReadAllLines(@"C:\Users\debim\source\repos\ÇicekProjesi\ÇicekProjesi\iris.data");
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

        static void hesaplamaYap(Cicek[] cicekListesi, int index, double lamda)
        {
            double toplam;
            for (int i = 0; i < 3; i++)
            {
                toplam = 0;
                for (int j = 0; j < 4; j++)
                {
                    toplam += cicekListesi[index].ozellikleri[j] * nöronlar[i].agırlıklar[j];
                }

                nöronlar[i].toplam = toplam;
            }
        }

        static void noronKontrol(Cicek[] cicekListesi, int index, double lamda)
        {
            int[] degisimNoronları = new int[2];
            double enBuyukDeger = nöronlar[0].toplam;
            int enBuyukNoron = 0;
            for (int i = 0; i < 3; i++)
            {
                if (enBuyukDeger < nöronlar[i].toplam)
                {
                    enBuyukDeger = nöronlar[i].toplam;
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
                        agırlıkDegistir(cicekListesi, degisimNoronları, index, lamda);
                    }

                    break;

                case "Iris-versicolor":
                    if (enBuyukNoron != 1)
                    {
                        degisimNoronları[1] = 1;
                        agırlıkDegistir(cicekListesi, degisimNoronları, index, lamda);
                    }

                    break;

                case "Iris-virginica":
                    if (enBuyukNoron != 2)
                    {
                        degisimNoronları[1] = 2;
                        agırlıkDegistir(cicekListesi, degisimNoronları, index, lamda);
                    }

                    break;
            }
        }

        static void agırlıkDegistir(Cicek[] cicekListesi, int[] degisenNoronlar, int index,
            double lamda)

        {
            double çiçekGirdisi;
            for (int i = 0; i < 4; i++)
            {
                çiçekGirdisi = cicekListesi[index].ozellikleri[i];
                nöronlar[degisenNoronlar[0]].agırlıklar[i] -= (lamda * çiçekGirdisi);
                nöronlar[degisenNoronlar[1]].agırlıklar[i] += (lamda * çiçekGirdisi);
            }
        }

        static int dogrulukKontrol(Cicek[] cicekListesi, int index)
        {
            double enBuyukDeger = 0;
            int enBuyukNoron = 0;
            for (int i = 0; i < 3; i++)
            {
                if (enBuyukDeger < nöronlar[i].toplam)
                {
                    enBuyukDeger = nöronlar[i].toplam;
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

        static void sonuclariYaz(int[] deneyMiktarlari, double[] lamdaDegerleri, double[,] dogrulukDegerleri, int deney)
        {
            int[] sira = { 1, 0, 2 };

            Console.Write(string.Format("{0}.Deney Dogruluk Degerleri:\n\n", deney + 1));
            Console.Write(string.Format("{0,28}λ {1,17}λ {2,17}λ{3,25}\n\n", lamdaDegerleri[1], lamdaDegerleri[0],
                lamdaDegerleri[2], "Ortalama Doğruluk"));
            for (int i = 0; i < 3; i++)
            {
                double ortalamaDogruluk = 0;
                string epok = $"{deneyMiktarlari[sira[i]]} Epok";
                Console.Write(string.Format("{0,-10}", epok));
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(string.Format("{0,18:0.##}%", dogrulukDegerleri[sira[i], sira[j]]));
                    ortalamaDogruluk += dogrulukDegerleri[sira[i], sira[j]];
                }

                Console.Write(string.Format("{0,24:0.##}%", ortalamaDogruluk / 3));
                Console.Write("\n\n");
            }
        }
    }

    internal class CicekProjesi
    {
        static void Main(string[] args)
        {
            NeuralNetwork network = new NeuralNetwork();
            network.programiEgit();
        }
    }
}