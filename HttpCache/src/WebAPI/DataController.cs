using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;

namespace Cache.WebAPI
{
    public class DataController : ApiController
    {
        public const string RouteNoCache = "api/nocache";
        public const string RouteCache10s = "api/cache10s";
        public const string RouteETag = "api/etag";

        public static int Calls = 0;

        [Route(RouteNoCache), HttpGet]
        public HttpResponseMessage NoCache()
        {
            var msg = GetResponse();
            msg.Headers.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            return msg;
        }

        [Route(RouteCache10s), HttpGet]
        public HttpResponseMessage CacheFor10Seconds()
        {
            var msg = GetResponse();
            msg.Headers.CacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(10),
                Public = true
            };
            return msg;
        }

        private const string Tag = @"""ETagValue1""";

        [Route(RouteETag), HttpGet]
        public HttpResponseMessage ETag()
        {
            // instead of this silly check against const value, one could issue a following SQL for instance:
            // SELECT * FROM X WHERE id=@id AND version <> @etag
            //
            // that would get data only if the version is different from the tag

            var hasValidTag = Request.Headers.IfNoneMatch.Any(value => value.Tag == Tag);
            HttpResponseMessage msg;
            
            if (hasValidTag)
            {
                Calls += 1;
                msg = Request.CreateResponse(HttpStatusCode.NotModified);
            }
            else
            {
                msg = GetResponse();
            }
            
            msg.Headers.ETag = new EntityTagHeaderValue(Tag);
            return msg;
        }

        private HttpResponseMessage GetResponse()
        {
            Calls += 1;
            var msg = Request.CreateResponse(new { Value = Calls });
            return msg;
        }
    }
}