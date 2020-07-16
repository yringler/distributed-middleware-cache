using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.Custom
{
    internal class CustomResponseCachingMiddleware
	{
		private readonly ResponseCachingMiddleware responseCachingMiddleware;

        public CustomResponseCachingMiddleware(RequestDelegate next,
            IOptions<ResponseCachingOptions> options,
            ILoggerFactory loggerFactory,
            IResponseCachingPolicyProvider policyProvider,
            IResponseCache cache,
            IResponseCachingKeyProvider keyProvider)
        {
            responseCachingMiddleware = new ResponseCachingMiddleware(next, options, loggerFactory, policyProvider, cache, keyProvider);
        }

		public async Task Invoke(HttpContext httpContext)
		{
			await responseCachingMiddleware.Invoke(httpContext);
		}
    }
}
