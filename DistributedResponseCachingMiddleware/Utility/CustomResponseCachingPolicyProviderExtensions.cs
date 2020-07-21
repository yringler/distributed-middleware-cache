using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.ResponseCaching;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the ResponseCaching Policies.
    /// </summary>
    public static class CustomResponseCachingPolicyProviderExtensions
    {
        /// <summary>
        /// Add policy that allows caching even if an Authorization header exists.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        public static IServiceCollection AddCacheAuthorizedRequestsResponseCachingPolicy(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IResponseCachingPolicyProvider, CacheAuthorizedRequestsResponseCachingPolicyProvider>();
            return services;
        }
    }
}
