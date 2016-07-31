using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PodNit
{
    public class Class1
    {
        public static void igra(Igrac igracI, Igrac igracII)
        {
            List<Pojam> pitanja = null;
            pitanja = ucitavanjePojmova();
            Console.Write(pitanja[0].pojam);
            Random rnd = new Random();
            int brojpitanja = rnd.Next(1, pitanja.Count);
            Pojam pitanje = pitanja[brojpitanja];

            try
            {
                // komunikacija sa igracima
                Task<string>[] taskArray = { Task<string>.Factory.StartNew(() => Igrac(igracI, pitanje, igracII.ime)),
                                             Task<string>.Factory.StartNew(() => Igrac(igracII, pitanje, igracI.ime)) };
                try
                {
                    taskArray[0].Wait();
                }
                catch (Exception)
                {
                    igracII.soketZaKomunikaciju.Send(Encoding.ASCII.GetBytes("Igrac je izasao"));
                    return;
                }
                try
                {
                    taskArray[1].Wait();
                }
                catch (Exception)
                {
                    igracI.soketZaKomunikaciju.Send(Encoding.ASCII.GetBytes("Igrac je izasao"));
                    return;
                }
                //komunikacija sa igracima
                string prviIgrac = taskArray[0].Result;
                string drugiIgrac = taskArray[1].Result;
                try
                {
                    igracI.soketZaKomunikaciju.Send(odgovor(prviIgrac, drugiIgrac, pitanje.brojPretraga));

                }
                catch (Exception)
                {
                    Console.Write("Prvi igrac je izasao");
                }
                try
                {
                    igracII.soketZaKomunikaciju.Send(odgovor(drugiIgrac, prviIgrac, pitanje.brojPretraga));

                }
                catch (Exception)
                {
                    Console.Write("Drugi igrac je izasao");
                }
                //zatvaranje oba komunikaciona soketa
                igracI.soketZaKomunikaciju.Close();
                igracII.soketZaKomunikaciju.Close();

                Console.Read();
            }
            catch (Exception)
            {
                Console.WriteLine("Greska! ");
            }
        }
        //sve sto se desava u svakoj niti
        public static List<Pojam> ucitavanjePojmova()
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Pojam>>(File.ReadAllText(@"\text.json"));

            }
            catch
            {
                Console.Write("nema nista");
                return null;
            }
        }

        public static string Igrac(Igrac igrac, Pojam pitanje, string drugiIgrac)
        {
            try
            { 
                Console.WriteLine("Konektovao se igrac" + igrac.ime);
                string st = String.Format(Environment.NewLine+ "Koliko pretraga ima: {0}?", pitanje.pojam);
               string str1 = "Igras sa " + drugiIgrac;
               // string str2 = "Koliko pretraga ima: " + pitanje.pojam + "? \n";
                igrac.soketZaKomunikaciju.Send(Encoding.ASCII.GetBytes(str1));
                igrac.soketZaKomunikaciju.Send(Encoding.ASCII.GetBytes(st));
                // citanje linije od igraca
                byte[] b = new byte[100];
                igrac.soketZaKomunikaciju.Receive(b);
                Console.WriteLine("Primljeno: ");
                string something = Encoding.ASCII.GetString(b);
                Console.Write(something);
                //cuvanje odgovora
                return something;
            }
            catch (SocketException)
            {
                throw;
            }
        }
        public static byte[] odgovor(string prviIgrac, string drugiIgrac, int resenje)
        {
            int odgovorPrvogIgraca;
            int odgovorDrugogIgraca;
            bool rezI = Int32.TryParse(prviIgrac, out odgovorPrvogIgraca);
            bool rezII = Int32.TryParse(drugiIgrac, out odgovorDrugogIgraca);
            if (!rezI)
            {
                return Encoding.ASCII.GetBytes("Nije unet broj! YOU LOSE!");
            }
            if (!rezII)
            {
                return Encoding.ASCII.GetBytes("Drugi igrac nije uneo dobar broj! YOU WIN!");
            }
            if (Math.Abs(odgovorPrvogIgraca - resenje) > Math.Abs(odgovorDrugogIgraca - resenje))
            {
                return Encoding.ASCII.GetBytes("YOU LOSE!");
            }
            else if (Math.Abs(odgovorPrvogIgraca - resenje) < Math.Abs(odgovorDrugogIgraca - resenje))
            {
                return Encoding.ASCII.GetBytes("YOU WIN!");
            }
            else
            {
                return Encoding.ASCII.GetBytes("Izjednaceno je");
            }
        }

    }

    public class Pojam
    {
        public string pojam { get; set; }
        public int brojPretraga { get; set; }
        public override string ToString()
        {
            return "Pojam: " + pojam + "Broj pretraga: " + brojPretraga;
        }
        public Pojam(string pojam, int brojPretraga)
        {
            this.pojam = pojam;
            this.brojPretraga = brojPretraga;
        }
    }

    public class Igrac
    {
        public string ime { get; set; }
        public Socket soketZaKomunikaciju { get; set; }

        public Igrac(string ime, Socket soketZaKomunikaciju)
        {
            this.ime = ime;
            this.soketZaKomunikaciju = soketZaKomunikaciju;
        }

    }



}

