using System;
using Cache.WebAPI;
using Microsoft.Owin.Hosting;
using NReco.PhantomJS;
using NUnit.Framework;

namespace Cache
{
    [TestFixture]
    public class Tests : IDisposable
    {
        private readonly IDisposable _app;
        private const string Address = "http://localhost:18458/";

        public Tests()
        {
            _app = WebApp.Start<Startup>(Address);
        }

        [Test]
        public void NoCache()
        {
            RunFor(DataController.RouteNoCache);
        }

        [Test]
        public void Cache10s()
        {
            RunFor(DataController.RouteCache10s);
        }

        [Test]
        public void ETag()
        {
            RunFor(DataController.RouteETag);
        }

        private static void RunFor(string routeNoCache)
        {
            var start = DataController.Calls;

            var phantomJs = new PhantomJS();
            phantomJs.OutputReceived += (sender, e) => Console.WriteLine("{0}", e.Data);
            phantomJs.RunScript(GetJson(routeNoCache), null);

            var end = DataController.Calls;

            Console.WriteLine("Controller was called {0} times", end - start);
        }

        private static string GetJson(string address)
        {
            return @"var page = require('webpage').create(),
                server = '" + Address + address + @"';
                console.log('Opening following url twice: ' + server);
                page.open(server, function (status) {
                    if (status !== 'success') {
                        console.log('get failed');
                    } else {
                        console.log(page.content);
                    }

                    page.open(server, function (status) {
                        if (status !== 'success') {
                            console.log('get failed');
                        } else {
                            console.log(page.content);
                        }
                        phantom.exit();
                        });
                });";
        }

        public void Dispose()
        {
            _app.Dispose();
        }
    }
}