using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using ExternalNetcoreExtensions.Utility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;

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
            ObjectPoolProvider objectPoolProvider)
        {
            responseCachingMiddleware = new ResponseCachingMiddleware(next, options, loggerFactory, policyProvider, cache,
                new ResponseCachingKeyProvider(objectPoolProvider, options));
        }

		public async Task Invoke(HttpContext httpContext)
		{
			await responseCachingMiddleware.Invoke(httpContext);
		}
    }
}
