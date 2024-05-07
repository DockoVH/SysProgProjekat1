using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend;

internal class TekstKonverter
{
    public static string BinarniUTekstualni(string putanjaDoBinarnog, string putanjaDoTekstualnog)
    {
        byte[] binSadrzaj = File.ReadAllBytes(putanjaDoBinarnog);
        string textSadrzaj = Encoding.UTF8.GetString(binSadrzaj);

        File.WriteAllText(putanjaDoTekstualnog, textSadrzaj, Encoding.UTF8);
        Console.WriteLine($"Binarni fajl {putanjaDoBinarnog} konvertovan u tekstualni fajl {putanjaDoTekstualnog}.");

        return textSadrzaj;
    }

    public static string TekstualniUBinarni(string putanjaDoTekstualnog, string putanjaDoBinarnog)
    {
        string textSadrzaj = File.ReadAllText(putanjaDoTekstualnog, Encoding.UTF8);
        byte[] binSadrzaj = Encoding.UTF8.GetBytes(textSadrzaj);

        File.WriteAllBytes(putanjaDoBinarnog, binSadrzaj);
        Console.WriteLine($"Tekstualni fajl {putanjaDoTekstualnog} konvertovan u binarni fajl {putanjaDoBinarnog}.");

        return Convert.ToBase64String(binSadrzaj);
    }
}
