using A180.CoreLib.Collections;

namespace A180.Network.Http;

using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;

// https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7
public class HttpServer
{
    public HttpServer(string url)
    {
        Url = url;
        Listener.Prefixes.Add(url + (url.EndsWith("/") ? "" : "/"));
    }

    public readonly HttpListener Listener = new();

    public string Url { get; }

    public int RequestCount { get; private set; } = 0;

    /// <summary>
    /// 返回true继续监听，返回false结束监听
    /// </summary>
    /// <param name="handler"></param>
    public async Task Start(Func<HttpListenerRequest, HttpListenerResponse, Task<bool>> handler)
    {
        _handler = handler;

        // 开始监听
        Listener.Start();

        // 阻塞等待请求
        await HandleIncomingConnections();

        Listener.Close();
    }

    public bool IsListening => Listener.IsListening;

    public PredicateSet Cancelers { get; } = new();

    private Func<HttpListenerRequest, HttpListenerResponse, Task<bool>> _handler = null;

    private async Task HandleIncomingConnections()
    {
        while (true)
        {
            // Will wait here until we hear from a connection
            var ctxRes = Listener.GetContextAsync();
            // ctxRes.Start();

            bool cancel = false;
            while (!ctxRes.IsCompleted)
            {
                Thread.Sleep(100);

                if (Cancelers.Yes)
                {
                    cancel = true;
                    Listener.Stop();
                }
            }

            if (cancel)
            {
                break;
            }

            HttpListenerContext ctx = ctxRes.Result;

            // Peel out the requests and response objects
            HttpListenerRequest req = ctx.Request;
            HttpListenerResponse resp = ctx.Response;

            // Print out some info about the request
            Console.WriteLine("Request #: {0}", ++RequestCount);
            Console.WriteLine(req.Url!.ToString());
            Console.WriteLine(req.HttpMethod);
            Console.WriteLine(req.UserHostName);
            Console.WriteLine(req.UserAgent);
            Console.WriteLine();

            if (!await _handler(req, resp))
            {
                // The handler request to stop
                break;
            }
        }
    }

    public class ExampleHandler
    {
        public int PageViews { get; private set; } = 0;

        private const string PageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public async Task<bool> HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            bool runServer = true;

            // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
            if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
            {
                Console.WriteLine("Shutdown requested");
                runServer = false;
            }

            // Make sure we don't increment the page views counter if `favicon.ico` is requested
            if (req.Url!.AbsolutePath != "/favicon.ico")
                PageViews += 1;

            // Write the response info
            string disableSubmit = !runServer ? "disabled" : "";
            byte[] data = Encoding.UTF8.GetBytes(string.Format(PageData, PageViews, disableSubmit));
            resp.ContentType = "text/html";
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            // Write out to the response stream (asynchronously), then close it
            await resp.OutputStream.WriteAsync(data, 0, data.Length);

            resp.Close();

            return runServer;
        }
    }

    public static async Task RunExampleServer()
    {
        var server = new HttpServer("http://localhost:8000/");
        var handler = new ExampleHandler();

        Console.WriteLine("Listening for connections on {0}", server.Url);

        // Serve, it will return when "shutdown" is clicked
        await server.Start(handler.HandleRequest);
    }
}