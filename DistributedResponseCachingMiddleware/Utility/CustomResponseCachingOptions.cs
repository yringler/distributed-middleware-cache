using Microsoft.AspNetCore.ResponseCaching;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalNetcoreExtensions.Utility
{
	/// <summary>
	/// Options for custom cache.
	/// </summary>
	public class CustomResponseCachingOptions : ResponseCachingOptions
	{
		/// <summary>
		/// By default, response cache doesn't cache a request with an authoriztion header.
		/// If you want such requests to be cached, set this to true.
		/// </summary>
		public bool CacheAuthorizedRequest { get; set; }

		internal static IResponseCachingPolicyProvider GetPolicy(ResponseCachingOptions options)
		{
			if (options is CustomResponseCachingOptions customOptions && customOptions.CacheAuthorizedRequest)
			{
				return new CacheAuthorizedRequestsResponseCachingPolicyProvider();
			}

			return new ResponseCachingPolicyProvider();
		}
	}
}
