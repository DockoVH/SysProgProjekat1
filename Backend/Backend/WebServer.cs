using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
    internal class WebServer
    {
        private HttpListener listener = new();
        private const int port = 8000;
        private readonly string[] prefix = [$"http://localhost:{port}/", $"http://127.0.0.1:{port}/"];

        public WebServer()
        {
            foreach (var pr in prefix)
            {
                listener.Prefixes.Add(pr);
            }
        }
    }
}
