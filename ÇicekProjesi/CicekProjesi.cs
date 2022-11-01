using System;
using System.Collections.Generic;

namespace ÇicekProjesi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string[] dosyaOkuma =
                System.IO.File.ReadAllLines(@"C:\Users\pc\Desktop\iris.data");

            string[][] ozelliklerDizisi = new string[150][];
            for (int i = 0; i < 150; i++)
            {
                ozelliklerDizisi[i] = new string[5];
                ozelliklerDizisi[i] = dosyaOkuma[i].Split(',');
            }

            NeuralNetwork network = new NeuralNetwork();
            Neuron N1 = new Neuron();
            Neuron N2 = new Neuron();
            Neuron N3 = new Neuron();
            network.NeuronArray.Add(N1);
            network.NeuronArray.Add(N2);
            network.NeuronArray.Add(N3);

            int[] epokdegerleri = { 20, 50, 100 };
            double[] ogrenmekatsayilari = { 0.005, 0.01, 0.025 };

            for (int z = 1; z < 4; z++)
            {
                Console.Write($"Deney Tablosu - {z} -------------------------------------------------------:\n", z);
                Console.Write(string.Format("{0,24} lambda{1,12} lambda{2,12} lambda\n\n", ogrenmekatsayilari[0],
                    ogrenmekatsayilari[1],
                    ogrenmekatsayilari[2]));

                for (int i = 0; i < 3; i++)
                {
                    Console.Write($"{epokdegerleri[i],5}" + " epok");
                    for (int j = 0; j < 3; j++)
                    {
                        network.egitimiYap(network, ozelliklerDizisi, epokdegerleri[i], ogrenmekatsayilari[j]);

                        Console.Write(string.Format("{0,19:0.##}", network.dogrulukYuzdesi(epokdegerleri[i])));
                        network.dogruSayisi = 0;
                    }

                    Console.Write("\n\n");
                }
            }
        }
    }

    public class Neuron
    {
        static Random random = new Random();
        public double[] girdiSayilari = new double[4];
        public string girdiCicekAdi;
        public double[] agirliklar = new double[4];

        public Neuron()
        {
            for (int i = 0; i < agirliklar.Length; i++)
            {
                double xx = random.NextDouble();
                this.agirliklar[i] = xx;
            }
        }

        public void agirliklariYenile()
        {
            for (int i = 0; i < agirliklar.Length; i++)
            {
                double xx = random.NextDouble();
                this.agirliklar[i] = xx;
            }
        }

        public double Ciktisi()
        {
            double cikti = 0;
            for (int i = 0; i < 4; i++)
            {
                cikti = agirliklar[i] * girdiSayilari[i];
            }

            return cikti;
        }
    }

    public class NeuralNetwork
    {
        public List<Neuron> NeuronArray = new List<Neuron>();
        public int dogruSayisi = 0;

        public void agirliklariEgit(double ogrenmekatsayisi)
        {
            int maxNeuron = maxNoronuBul();
            int beklenenNoron = beklenenNoronuBul();

            if (maxNeuron != beklenenNoron)
            {
                for (int j = 0; j < 4; j++)
                {
                    NeuronArray[beklenenNoron].agirliklar[j] +=
                        (ogrenmekatsayisi * NeuronArray[beklenenNoron].girdiSayilari[j]);
                    NeuronArray[maxNeuron].agirliklar[j] -=
                        (ogrenmekatsayisi * NeuronArray[maxNeuron].girdiSayilari[j]);
                }
            }
        }

        public void dogruSayisiniArttir()
        {
            int maxNeuron = maxNoronuBul();
            int beklenenNoron = beklenenNoronuBul();

            if (maxNeuron == beklenenNoron)
            {
                this.dogruSayisi += 1;
            }
        }

        public double dogrulukYuzdesi(int epok)
        {
            double mmm = (double)this.dogruSayisi / (150 * epok) * 100;
            return mmm;
        }

        public int maxNoronuBul()
        {
            double maxCikti = 0;
            int maxNeuron = 0;
            for (int i = 0; i < 3; i++)
            {
                if (NeuronArray[i].Ciktisi() > maxCikti)
                {
                    maxCikti = NeuronArray[i].Ciktisi();
                    maxNeuron = i;
                }
            }

            return maxNeuron;
        }

        public int beklenenNoronuBul()
        {
            int beklenenNoron = 0;

            if (NeuronArray[0].girdiCicekAdi.Equals("Iris-setosa"))
            {
                beklenenNoron = 0;
            }

            if (NeuronArray[1].girdiCicekAdi.Equals("Iris-versicolor"))
            {
                beklenenNoron = 1;
            }

            if (NeuronArray[2].girdiCicekAdi.Equals("Iris-virginica"))
            {
                beklenenNoron = 2;
            }

            return beklenenNoron;
        }

        public void egitimiYap(NeuralNetwork network, String[][] ozelliklerDizisi, int epok, double ogrenmekatsayisi)
        {
            void verileriYerlestir(int k)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        double girdiVerisi = Convert.ToDouble(ozelliklerDizisi[k][j],
                            System.Globalization.CultureInfo.InvariantCulture);
                        girdiVerisi /= 10;
                        network.NeuronArray[i].girdiSayilari[j] = girdiVerisi;
                    }

                    network.NeuronArray[i].girdiCicekAdi = ozelliklerDizisi[k][4];
                }
            }

            for (int e = 0; e < epok; e++)
            {
                for (int k = 0; k < 150; k++)
                {
                    verileriYerlestir(k);
                    network.agirliklariEgit(ogrenmekatsayisi);
                    network.dogruSayisiniArttir();
                }
            }

            for (int i = 0; i < 3; i++)
            {
                network.NeuronArray[i].agirliklariYenile();
            }
        }
    }
}