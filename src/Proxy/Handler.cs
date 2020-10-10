using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using ProxyKit;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Proxy
{
    /// <summary>
    /// Typed Proxy Handler that logs, forward to CDN and patch absolute URLs in manifests.
    /// </summary>
    public class Handler : IProxyHandler
    {
        public Handler(ILogger<Handler> logger, IConfiguration config)
        {
            _logger = logger;
            _forwardTo = config["Forward"];
        }

        // Implementation of IProxyHandler.HandleProxyRequest
        public async Task<HttpResponseMessage> HandleProxyRequest(HttpContext context)
        {

            var requestUrl = context.Request.GetDisplayUrl();
            _request = GetRequestType(requestUrl);
            Log(requestUrl, Flow.IN, _request);

            var stopwatch = Stopwatch.StartNew();
            var response = await context
                .ForwardTo(_forwardTo)
                .Send();
            stopwatch.Stop();
            if (_request == RequestType.MANIFEST)
            {
                await RemoveAbsolutePath(response);
            }
            Log(response.RequestMessage.RequestUri.ToString(), Flow.OUT, _request, stopwatch);
            return response;
        }

        private RequestType GetRequestType(string url)
        {
            var type = AnalyzerUtils.GetRequestType(url);
            if (_request == RequestType.SEGMENT && type == RequestType.MANIFEST)
            {
                _logger.LogInformation("[TRACK SWITCH]");
            }
            else if (type == RequestType.UNKNOWN)
            {
                _logger.LogWarning("Unknown Request type");
            }
            return type;
        }

        private void Log(string url, Flow flow, RequestType type, Stopwatch sw = null)
        {
            var log = $"[{flow}][{type}] {url}";
            if (sw != null)
            {
                log += $" ({sw.ElapsedMilliseconds} ms)";
            }
            _logger.LogInformation(log);
        }

        private async Task RemoveAbsolutePath(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var replace = AnalyzerUtils.PatchToRelative(response.RequestMessage.RequestUri, content);
            if (replace != null)
            {
                _logger.LogInformation("[PATCH]");
                response.Content = new StringContent(replace);
            }
        }


        /// <summary>
        /// Logger object
        /// </summary>
        private readonly ILogger<Handler> _logger;
        /// <summary>
        /// Forward (command-line) parameter
        /// </summary>
        private readonly string _forwardTo;
        /// <summary>
        /// Current Request type
        /// </summary>
        private RequestType _request = RequestType.UNKNOWN;
    }
}