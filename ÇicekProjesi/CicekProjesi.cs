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

        public void programiEgit() // Ana metot
        {
            double[] LambdaDegerleri = { 0.01, 0.005, 0.025 };
            int[] EpokMiktarlari = { 50, 20, 100 };
            // Elemanları Çiçek sınıfından oluşan ,150 elemanlı ÇiçekListesi oluşturma

            Cicek[] CicekListesi = cicekListesiniOlustur();
            Console.Clear();
            for (int tabloDeneyi = 0; tabloDeneyi < 3; tabloDeneyi++) // 3 tablo oluşturmak için en dış döngü
            {
                // Doğruluk yüzdelerinin tutulacağı 2 boyutlu dizi, satırlar epok değerleri sütünlar lambda değerleri için
                double[,] dogrulukDegerleri = new double[3, 3];

                for (int Lambdaİndexi = 0; Lambdaİndexi < 3; Lambdaİndexi++) // LambdaDegerleri dizisini gezen index
                {
                    for (int Epokİndexi = 0; Epokİndexi < 3; Epokİndexi++) // EpokMiktarlari dizisini gezen index
                    {
                        agirliklariOlustur(
                            random); // Yeni epek miktarina başlamadan önce ağırlıkları tekrardan random oluşturma
                        for (int EpokMiktari = 0; EpokMiktari < EpokMiktarlari[Epokİndexi]; EpokMiktari++)
                        {
                            for (int ÇiçekListesiİndexi = 0;
                                 ÇiçekListesiİndexi < 150;
                                 ÇiçekListesiİndexi++) // ÇiçekListesi dizisini gecen index
                            {
                                NöronToplamDegeriHesapla(CicekListesi, ÇiçekListesiİndexi,
                                    LambdaDegerleri[Lambdaİndexi]); // Her Nöronun toplam degerini hesaplama
                                EnBuyükNöronuBul(CicekListesi, ÇiçekListesiİndexi,
                                    LambdaDegerleri[
                                        Lambdaİndexi]); // Değeri en büyük çıkan Nöronu bulup, bakılan Çiçekle karşılaştırma
                            }
                        }

                        for (int ÇiçekListesiİndexi = 0;
                             ÇiçekListesiİndexi < 150;
                             ÇiçekListesiİndexi++) // Epoklar tamamlandıktan sonra, ağırlıklar değiştirilmeden doğruluk değerini bulma
                        {
                            NöronToplamDegeriHesapla(CicekListesi, ÇiçekListesiİndexi, LambdaDegerleri[Lambdaİndexi]);
                            dogrulukDegerleri[Epokİndexi, Lambdaİndexi] +=
                                dogrulukKontrol(CicekListesi,
                                    ÇiçekListesiİndexi); // Bakılan çiçek doğruysa 1 değilse 0 döndüren metot
                        }

                        dogrulukDegerleri[Epokİndexi, Lambdaİndexi] =
                            (dogrulukDegerleri[Epokİndexi, Lambdaİndexi] / 150) * 100;
                    }
                }

                sonuclariYaz(EpokMiktarlari, LambdaDegerleri, dogrulukDegerleri,
                    tabloDeneyi); // Konsola sonuçları yazan metot
            }
        }


        static void
            agirliklariOlustur(
                Random random) // Her nöron için 4, toplam 12 (0,1) arasında ağırlık oluşturma (0 ve 1 hariç)
        {
            double sayi;
            for (int NöronlarListesiİndex = 0; NöronlarListesiİndex < 3; NöronlarListesiİndex++)
            {
                for (int Ağırlıkİndexi = 0; Ağırlıkİndexi < 4; Ağırlıkİndexi++)
                {
                    while (true) // random double metotunun 0 oluşturmaması için döngü
                    {
                        sayi = random.NextDouble();
                        if (sayi != 0) // ağırlık 0 değilse döngüyü kır
                        {
                            break;
                        }
                    }

                    nöronlar[NöronlarListesiİndex].agırlıklar[Ağırlıkİndexi] =
                        Math.Round(sayi, 1); // Ağırlıklarınğın virgül sonrasını 1 basamağa yuvarlama
                }
            }
        }

        static Cicek[] cicekListesiniOlustur() // iris.data dosyasını okuyup ,Çiçek sınıfı elemanlı dizi oluşturma
        {
            string path = Path.Combine(Directory.GetCurrentDirectory()) +
                          "\\iris.data"; /*Bu satirdaki path stringi iris.data dosyasının -
            bilgisayardaki konumunu göstermek içindir. iris.data dosyasının bulunduğu yere göre değiştirilebilir.Değiştirilmedi sürece projenin bulunduğu klasördeki -
            bin/debug klasöründen iris.data yı okumaya çalışacaktır. iris.data dosyasını bu klasöre atmak yeterlidir. */
            string[]
                dosyaOkuma =
                    File.ReadAllLines(
                        path); // Elemanları iris.data dosyasının satırlarından oluşan 150 elemanlı string dizisi
            string[][] dataDizisi = new string[150][];
            for (int i = 0;
                 i < 150;
                 i++) // Çiçeklerin özelliklerini ve isimlerini her virgülde ayırarak yeni diziye atama
            {
                dataDizisi[i] = new string[5];
                dataDizisi[i] = dosyaOkuma[i].Split(',');
            }

            Cicek[] CicekListesi = new Cicek[150]; // Çicek nesnelerinden oluşan ÇiçekListesi
            for (int i = 0; i < 150; i++)
            {
                CicekListesi[i] = new Cicek(); // Bellekte yer atama
            }

            for (int ÇiçekListesiİndexi = 0;
                 ÇiçekListesiİndexi < 150;
                 ÇiçekListesiİndexi++) // Çiçek Listesine çiçek adları ve özelliklerini ekleme
            {
                for (int ÇiçekÖzellikİndexi = 0; ÇiçekÖzellikİndexi < 4; ÇiçekÖzellikİndexi++)
                {
                    CicekListesi[ÇiçekListesiİndexi].ozellikleri[ÇiçekÖzellikİndexi] =
                        (double.Parse(dataDizisi[ÇiçekListesiİndexi][ÇiçekÖzellikİndexi],
                            CultureInfo.InvariantCulture)) /
                        10; // String olarak okunan özellikleri double sayılara çevirme ve ona bölme
                }

                CicekListesi[ÇiçekListesiİndexi].ad = dataDizisi[ÇiçekListesiİndexi][4];
            }

            return CicekListesi;
        }

        static void
            NöronToplamDegeriHesapla(Cicek[] cicekListesi, int index,
                double lamda) // Nöron ağırlıkları çiçek girdileriyle çarpıp toplayan metot
        {
            double toplam;
            for (int Nöronİndexi = 0; Nöronİndexi < 3; Nöronİndexi++)
            {
                toplam = 0;
                for (int Ağırlıkİndexi = 0; Ağırlıkİndexi < 4; Ağırlıkİndexi++)
                {
                    toplam += cicekListesi[index].ozellikleri[Ağırlıkİndexi] *
                              nöronlar[Nöronİndexi].agırlıklar[Ağırlıkİndexi];
                }

                nöronlar[Nöronİndexi].toplam = toplam;
            }
        }

        static void
            EnBuyükNöronuBul(Cicek[] cicekListesi, int index,
                double lamda) // Değeri en büyük çıkan Nöronu bulup ,bakılan Çiçeğin Nöronuyla karşılaştıran metot
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
            switch (cicekListesi[index].ad) // Bakılan çiçeğin adına göre aşağıdaki caselerden birine girer
            {
                case "Iris-setosa":
                    if (enBuyukNoron !=
                        0) // Bakılan çiçeğin nöronuyla ,büyük çıkan nöron aynı olmazsa ağırlıkları değiştirir
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

        static void
            agırlıkDegistir(Cicek[] cicekListesi, int büyükNöron, int küçükNöron, int index,
                double lamda) // Büyük çıkması gereken Nöronun ağırlığını artırma ve diğerini küçültme

        {
            double çiçekGirdisi;
            for (int i = 0; i < 4; i++)
            {
                çiçekGirdisi = cicekListesi[index].ozellikleri[i]; // Bakılan Çiçeğin özellikleri de hesaba katılır
                nöronlar[büyükNöron].agırlıklar[i] -= (lamda * çiçekGirdisi);
                nöronlar[küçükNöron].agırlıklar[i] += (lamda * çiçekGirdisi);
            }
        }

        static int
            dogrulukKontrol(Cicek[] cicekListesi,
                int index) /* Epok miktarı tamamlandıktan sonra doğruluk yüzdesini bulam metot.
                                                                     Bu metotda Nöron ağırlıkları değiştirilmez.*/
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

            switch (cicekListesi[index].ad) // Çiçeğin ismine göre caselerden birine girme
            {
                case "Iris-setosa":
                    if (enBuyukNoron ==
                        0) // En büyük çıkan Nöronla çiçeğin nöronunu karşılaştırır. Doğruysa 1 döndürür yanlış ise 0 döndürür.
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
            int deney) // 3 Deney sonucunu birer tablo halinde konsola yazan metot.
        {
            int[] sira = { 1, 0, 2 }; // Epok ve lambda değerlerinin yazılış sırası.

            // string.Format metotları ile yazılar ve değerler alt alta daha okunaklı yazılır.
            // {0,28} şeklinde, "0" ilk verilen parametre ve 28 değeri ise 28 boşluk bırakıp sağdan sola doğru yazılacağını söyler.

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
            NeuralNetwork network = new NeuralNetwork(); // NeuralNetwork sınıfından network nesnesi oluşturma.
            network.programiEgit(); // Makineyi eğitme.
        }
    }
}