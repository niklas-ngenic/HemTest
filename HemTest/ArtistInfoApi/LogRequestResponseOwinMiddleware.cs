using Microsoft.Owin;
using NLog;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ArtistInfoApi
{

    public static class SetupLogRequestResponseExtension
    {
        public static void AddLogRequestResponse(this IAppBuilder app)
        {
            app.Use(typeof(LogRequestResponseOwinMiddleware));
        }
    }

    public class LogRequestResponseOwinMiddleware : OwinMiddleware
    {
        private const string _logFormat = "[{0}] {1}, Status: {2} executed in {3} ms";
        private ILogger _logger = LogManager.GetLogger("RequestResponseOwinMiddleware");
        public LogRequestResponseOwinMiddleware(OwinMiddleware next)
            : base(next)
        {

        }

        public override async Task Invoke(IOwinContext context)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            await Next.Invoke(context);
            stopwatch.Stop();
            _logger.Debug(string.Format(_logFormat, context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds));
        }
    }
}