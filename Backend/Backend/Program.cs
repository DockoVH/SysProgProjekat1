using Backend;
using System;

namespace Projekat;

public class Program
{
    static void Main(string[] args)
    {
        WebServer server = new();

        server.Start();
    }
}