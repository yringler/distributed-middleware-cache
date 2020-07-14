using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.Custom
{
	internal class CustomResponseCachingMiddleware
	{
		private readonly ResponseCachingMiddleware responseCachingMiddleware;

		public CustomResponseCachingMiddleware(RequestDelegate next,
			IOptions<ResponseCachingOptions> options,
			ILoggerFactory loggerFactory,
			ICustomResponseCache cache,
			ObjectPoolProvider poolProvider)
			: this(next, options, loggerFactory, CustomResponseCachingOptions.GetPolicy(options.Value), cache, new ResponseCachingKeyProvider(poolProvider, options))
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
