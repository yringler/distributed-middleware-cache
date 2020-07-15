using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
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
			ICustomResponseCache cache,
			ObjectPoolProvider poolProvider)
			: this(next, options, loggerFactory, policyProvider, cache, new ResponseCachingKeyProvider(poolProvider, options))
		{
		}

		internal CustomResponseCachingMiddleware(RequestDelegate next,
			IOptions<ResponseCachingOptions> options,
			ILoggerFactory loggerFactory,
			IResponseCachingPolicyProvider policyProvider,
			ICustomResponseCache cache,
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
