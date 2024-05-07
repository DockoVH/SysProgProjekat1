﻿using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Backend;

internal class WebServer
{
    private HttpListener listener = new();
    private const int port = 5050;
    private readonly string[] prefix = [$"http://localhost:{port}/", $"http://127.0.0.1:{port}/"];
    private readonly FajlKes kes = new(100, TimeSpan.FromSeconds(180));

    public WebServer()
    {
        foreach (var pr in prefix)
        {
            listener.Prefixes.Add(pr);
        }
    }

    public void Start()
    {
        ThreadPool.SetMaxThreads(4, 50);

        try
        {
            listener.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška prilikom pokretanja servera: {ex.Message}");
            return;
        }

        Console.WriteLine($"Server pokrenut:\n{String.Join("\n", prefix)}");

        while(true)
        {
            HttpListenerContext context;

            try
            {
                context = listener.GetContext();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
                continue;
            }

            ThreadPool.QueueUserWorkItem(ObradiZahtev, context);
        }
    }

    private void ObradiZahtev(object? obj)
    {
        if(!(obj is HttpListenerContext context))
        {
            return;
        }

        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        string putanja = request.Url.LocalPath.TrimStart('/');

        if(request.Url.LocalPath == "/fajlovi/txt")
        {
            List<string> txtFajlovi = ListaFajlova.TxtFajlovi(AppDomain.CurrentDomain.BaseDirectory);
            string jsonOdg = JsonConvert.SerializeObject(txtFajlovi);
            PosaljiOdgovor(response, jsonOdg);
            return;
        }
        else if(request.Url.LocalPath == "/fajlovi/bin")
        {
            List<string> binFajlovi = ListaFajlova.BinFajlovi(AppDomain.CurrentDomain.BaseDirectory);
            string jsonOdg = JsonConvert.SerializeObject(binFajlovi);
            PosaljiOdgovor(response, jsonOdg);
            return;
        }

        if(kes.TryGet(putanja, out string kesiraniResponse))
        {
            Console.WriteLine($"Vraćen keširani odgovor za: {putanja}");
            PosaljiOdgovor(response, kesiraniResponse);
            return;
        }

        try
        {
            string punaPutanja = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, putanja);
            string tekst;

            if (!File.Exists(punaPutanja))
            {
                Console.WriteLine("Fajl ne postoji.");
                response.StatusCode = (int)HttpStatusCode.NotFound;
                PosaljiOdgovor(response, "Fajl ne postoji");
                return;
            }

            string ext = Path.GetExtension(punaPutanja);
            if (ext.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(punaPutanja);
                string binPutanja = $"{punaPutanja.Remove(punaPutanja.IndexOf(".txt"))}Bin.bin";
                tekst = TekstKonverter.TekstualniUBinarni(punaPutanja, binPutanja);
            }
            else
            {
                string txtPutanja = $"{punaPutanja.Remove(punaPutanja.IndexOf("Bin.bin"))}.txt";
                tekst = TekstKonverter.BinarniUTekstualni(punaPutanja, txtPutanja);
            }

            kes.DodajIliAzuriraj(putanja, tekst);
            PosaljiOdgovor(response, tekst);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Greška prilikom obrade zahteva: {ex.Message}");
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            PosaljiOdgovor(response, "Greška prilikom obrade zahteva");
        }
        finally
        {
            response.Close();
        }
    }

    private void PosaljiOdgovor(HttpListenerResponse response, string body)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.ContentType = "application/json";
        byte[] buff = Encoding.UTF8.GetBytes(body);
        response.ContentLength64 = buff.Length;
        response.OutputStream.Write(buff, 0, buff.Length);
    }
}
