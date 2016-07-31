using System;
using System.Text;
using PodNit;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace GlavnaNit
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //otvaranje osluskujuceg soketa
            IPAddress ipAd = IPAddress.Parse("192.168.1.3");
            TcpListener osluskujuciSoket = new TcpListener(ipAd, 8001);
            osluskujuciSoket.Start();
            Console.WriteLine("Server se BU nalazi na portu 8001...");
            Console.WriteLine("IP adresa je" + osluskujuciSoket.LocalEndpoint);
            Console.WriteLine("Cekanje na uspostavljanje veze...");
            try
            {
                while (true)
                {
                    Socket soketZaKomunikaciju = osluskujuciSoket.AcceptSocket();
                    Igrac igracI = igrac(soketZaKomunikaciju);
                    soketZaKomunikaciju.Send(Encoding.ASCII.GetBytes("Sacekaj da udje drugi igrac\r\n"));
                    Socket soketZaKomunikaciju1 = osluskujuciSoket.AcceptSocket();
                    Igrac igracII = igrac(soketZaKomunikaciju1);
                    Thread t = new Thread(() => Class1.igra(igracI, igracII));
                    t.Start();
                }
            }
            catch (Exception)
            {
                osluskujuciSoket.Stop();
            }
        }


        public static Igrac igrac(Socket soketZaKomunikaciju)
        {
            byte[] b = new byte[100];
            soketZaKomunikaciju.Receive(b);
            string ime = Encoding.ASCII.GetString(b);
            return new Igrac(ime, soketZaKomunikaciju);

        }


    }
    
   
}
