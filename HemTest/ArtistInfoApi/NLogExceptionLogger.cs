using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace ArtistInfoApi
{
    public class NLogExceptionLogger : ExceptionLogger
    {
        private ILogger _logger = LogManager.GetLogger("NLogExceptionFilter");

        public override void Log(ExceptionLoggerContext context)
        {
            _logger.Error(context.Exception);
        }
    }
}