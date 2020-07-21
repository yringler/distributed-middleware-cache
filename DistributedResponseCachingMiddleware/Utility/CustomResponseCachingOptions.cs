using System;
using ExternalNetcoreExtensions.Custom;
using Microsoft.AspNetCore.ResponseCaching;

namespace ExternalNetcoreExtensions.Utility
{
	/// <summary>
	/// Options for custom cache.
	/// </summary>
	public class CustomResponseCachingOptions : ResponseCachingOptions
	{
		/// <summary>
		/// By default, response cache doesn't cache a request with an authorization header.
		/// If you want such requests to be cached, set this to true.
		/// </summary>
		public bool CacheAuthorizedRequest { get; set; }

        public ResponseCachingStrategy ResponseCachingStrategy { get; set; }
    }

    public enum ResponseCachingStrategy
    {
        Local = 1,
		Distributed = 2,
        ModifiableDistributed = 3
	}
}
