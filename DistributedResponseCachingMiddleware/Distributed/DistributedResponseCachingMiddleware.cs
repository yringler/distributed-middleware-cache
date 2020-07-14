using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.Distributed
{
	/// <remarks>
	/// In a later version this may be deprecated in favor of <see cref="Custom.CustomResponseCachingMiddleware"/>, which
	/// could be generalized to create either a <see cref="DistributedResponseCache"/> or use an injected <see cref="Custom.ICustomResponseCache"/>
	/// if one is present.
	/// </remarks>
	internal class DistributedResponseCachingMiddleware
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
