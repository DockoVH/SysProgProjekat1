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
            return Directory.GetFiles(rootDir, "*.txt").ToList();
        }

        public static List<string> BinFajlovi(string rootDir)
        {
            return Directory.GetFiles(rootDir, "*.bin").ToList();
        }
    }
}
