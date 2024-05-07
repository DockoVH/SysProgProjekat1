using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend;

internal class FajlKes
{
    private readonly int maxVelicina;
    private readonly TimeSpan timeToLive;
    private readonly ConcurrentDictionary<string, Fajl> kes = new();
    private readonly LinkedList<string> kljucevi = new();
    private readonly object lockObj = new();

    public FajlKes(int maxVelicina, TimeSpan timeToLive)
    {
        this.maxVelicina = maxVelicina;
        this.timeToLive = timeToLive;
    }

    public bool TryGet(string putanja, out string response)
    {
        if(kes.TryGetValue(putanja, out Fajl fajl))
        {
            if(fajl.RokTrajanja < DateTime.Now)
            {
                Izbaci(putanja);
                response = default;
                return false;
            }
            lock (lockObj)
            {
                kljucevi.Remove(putanja);
                kljucevi.AddLast(putanja);
            }

            response = fajl.Response;
            return true;
        }

        response = default;
        return false;
    }

    public void DodajIliAzuriraj(string putanja, string response)
    {
        DateTime rokTrajanja = DateTime.Now.Add(timeToLive);
        Fajl noviFajl = new Fajl(response, rokTrajanja);

        kes[putanja] = noviFajl;

        lock (lockObj)
        {
            kljucevi.AddLast(putanja);

            if(kljucevi.Count > maxVelicina)
            {
                string najstarijiKljuc = kljucevi.First.Value;
                kljucevi.RemoveFirst();
                kes.TryRemove(najstarijiKljuc, out _);
            }
        }
    }

    private void Izbaci(string putanja)
    {
        kes.TryRemove(putanja, out _);

        lock (lockObj)
        {
            kljucevi.Remove(putanja);   
        }
    }
}
