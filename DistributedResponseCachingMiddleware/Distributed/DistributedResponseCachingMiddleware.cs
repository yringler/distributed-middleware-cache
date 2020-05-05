using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.Distributed
{
	class DistributedResponseCachingMiddleware
	{
		private readonly ResponseCachingMiddleware responseCachingMiddleware;

		public DistributedResponseCachingMiddleware(RequestDelegate next,
			IOptions<ResponseCachingOptions> options,
			ILoggerFactory loggerFactory,
			IResponseCachingPolicyProvider policyProvider,
			IDistributedCache cache,
			IResponseCachingKeyProvider keyProvider)
		{
			var distributedResponseCache = new DistributedResponseCache(cache);
			responseCachingMiddleware = new ResponseCachingMiddleware(next, options, loggerFactory, policyProvider, distributedResponseCache, keyProvider);
		}

		public async Task Invoke(HttpContext httpContext)
		{
			await responseCachingMiddleware.Invoke(httpContext);
		}
	}
}
