using System;
using System.Globalization;
using System.IO;

namespace ÇicekProjesi
{
    class Cicek
    {
        public double[] ozellikleri = new double[4];
        public string ad;
    }

    class Neuron
    {
        public double[] agırlıklar = new double[4];
        public double toplam;
    }

    class NeuralNetwork
    {
        static readonly Random random = new Random();
        static readonly Neuron[] nöronlar = new Neuron[3];

        public NeuralNetwork()
        {
            for (int i = 0; i < 3; i++)
            {
                nöronlar[i] = new Neuron();
            }
        }

        public void programiEgit()
        {
            double[] LambdaDegerleri = { 0.01, 0.005, 0.025 };
            int[] EpokMiktarlari = { 50, 20, 100 };

            Cicek[] CicekListesi = cicekListesiniOlustur();
            Console.Clear();
            for (int tabloDeneyi = 0; tabloDeneyi < 3; tabloDeneyi++)
            {
                double[,] dogrulukDegerleri = new double[3, 3];

                for (int Lambdaİndexi = 0; Lambdaİndexi < 3; Lambdaİndexi++)
                {
                    for (int Epokİndexi = 0; Epokİndexi < 3; Epokİndexi++)
                    {
                        agirliklariOlustur(random);
                        for (int EpokMiktari = 0; EpokMiktari < EpokMiktarlari[Epokİndexi]; EpokMiktari++)
                        {
                            for (int ÇiçekListesiİndexi = 0; ÇiçekListesiİndexi < 150; ÇiçekListesiİndexi++)
                            {
                                NöronToplamDegeriHesapla(CicekListesi, ÇiçekListesiİndexi,
                                    LambdaDegerleri[Lambdaİndexi]);
                                EnBuyükNöronuBul(CicekListesi, ÇiçekListesiİndexi, LambdaDegerleri[Lambdaİndexi]);
                            }
                        }

                        for (int ÇiçekListesiİndexi = 0; ÇiçekListesiİndexi < 150; ÇiçekListesiİndexi++)
                        {
                            NöronToplamDegeriHesapla(CicekListesi, ÇiçekListesiİndexi, LambdaDegerleri[Lambdaİndexi]);
                            dogrulukDegerleri[Epokİndexi, Lambdaİndexi] +=
                                dogrulukKontrol(CicekListesi, ÇiçekListesiİndexi);
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
            for (int NöronlarListesiİndex = 0; NöronlarListesiİndex < 3; NöronlarListesiİndex++)
            {
                for (int Ağırlıkİndexi = 0; Ağırlıkİndexi < 4; Ağırlıkİndexi++)
                {
                    while (true)
                    {
                        sayi = random.NextDouble();
                        if (sayi != 0)
                        {
                            break;
                        }
                    }

                    nöronlar[NöronlarListesiİndex].agırlıklar[Ağırlıkİndexi] = Math.Round(sayi, 1);
                }
            }
        }

        static Cicek[] cicekListesiniOlustur()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory()) + "\\iris.data";
            string[] dosyaOkuma = File.ReadAllLines(path);
            string[][] dataDizisi = new string[150][];
            for (int satir = 0; satir < 150; satir++)
            {
                dataDizisi[satir] = new string[5];
                dataDizisi[satir] = dosyaOkuma[satir].Split(',');
            }

            Cicek[] CicekListesi = new Cicek[150];
            for (int i = 0; i < 150; i++)
            {
                CicekListesi[i] = new Cicek();
            }

            for (int ÇiçekListesiİndexi = 0; ÇiçekListesiİndexi < 150; ÇiçekListesiİndexi++)
            {
                for (int ÇiçekÖzellikİndexi = 0; ÇiçekÖzellikİndexi < 4; ÇiçekÖzellikİndexi++)
                {
                    CicekListesi[ÇiçekListesiİndexi].ozellikleri[ÇiçekÖzellikİndexi] = (double.Parse(dataDizisi[ÇiçekListesiİndexi][ÇiçekÖzellikİndexi], CultureInfo.InvariantCulture)) / 10;
                }

                CicekListesi[ÇiçekListesiİndexi].ad = dataDizisi[ÇiçekListesiİndexi][4];
            }

            return CicekListesi;
        }

        static void NöronToplamDegeriHesapla(Cicek[] cicekListesi, int index, double lamda)
        {
            double toplam;
            for (int Nöronİndexi = 0; Nöronİndexi < 3; Nöronİndexi++)
            {
                toplam = 0;
                for (int Ağırlıkİndexi = 0; Ağırlıkİndexi < 4; Ağırlıkİndexi++)
                {
                    toplam += cicekListesi[index].ozellikleri[Ağırlıkİndexi] * nöronlar[Nöronİndexi].agırlıklar[Ağırlıkİndexi];
                }

                nöronlar[Nöronİndexi].toplam = toplam;
            }
        }

        static void EnBuyükNöronuBul(Cicek[] cicekListesi, int index, double lamda)
        {
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

            int küçükNöron;
            switch (cicekListesi[index].ad)
            {
                case "Iris-setosa":
                    if (enBuyukNoron !=
                        0)
                    {
                        küçükNöron = 0;
                        agırlıkDegistir(cicekListesi, enBuyukNoron, küçükNöron, index, lamda);
                    }

                    break;

                case "Iris-versicolor":
                    if (enBuyukNoron != 1)
                    {
                        küçükNöron = 1;
                        agırlıkDegistir(cicekListesi, enBuyukNoron, küçükNöron, index, lamda);
                    }

                    break;

                case "Iris-virginica":
                    if (enBuyukNoron != 2)
                    {
                        küçükNöron = 2;
                        agırlıkDegistir(cicekListesi, enBuyukNoron, küçükNöron, index, lamda);
                    }

                    break;
            }
        }

        static void agırlıkDegistir(Cicek[] cicekListesi, int büyükNöron, int küçükNöron, int index, double lamda)

        {
            double çiçekGirdisi;
            for (int i = 0; i < 4; i++)
            {
                çiçekGirdisi = cicekListesi[index].ozellikleri[i];
                nöronlar[büyükNöron].agırlıklar[i] -= (lamda * çiçekGirdisi);
                nöronlar[küçükNöron].agırlıklar[i] += (lamda * çiçekGirdisi);
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
                    if (enBuyukNoron ==
                        0)
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

        static void sonuclariYaz(int[] deneyMiktarlari, double[] lamdaDegerleri, double[,] dogrulukDegerleri,
            int deney)
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