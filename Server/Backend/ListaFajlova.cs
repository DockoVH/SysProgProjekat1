using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    internal class ListaFajlova
    {

        public static List<string> TxtFajlovi(string rootDir)
        {
            List<string> punePutanje = Directory.GetFiles(rootDir, "*.txt").ToList();
            List<string> imenaFajlova = new();

            foreach (string fajl in punePutanje)
            {
                imenaFajlova.Add(Path.GetFileName(fajl));
            }

            return imenaFajlova;
        }

        public static List<string> BinFajlovi(string rootDir)
        {
            List<string> punePutanje = Directory.GetFiles(rootDir, "*.bin").ToList();
            List<string> imenaFajlova = new();

            foreach (string fajl in punePutanje)
            {
                imenaFajlova.Add(Path.GetFileName(fajl));
            }

            return imenaFajlova;
        }
    }
}
