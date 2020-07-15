// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.ResponseCaching;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the ResponseCaching middleware.
    /// </summary>
    /// <remarks>
    /// These are just wrappers around AddResponseCaching. For some reason, I get compilation errors
    /// in my aspnetcore project when trying to call AddResponseCaching, because it's defined in two assemblies.
    /// </remarks>
    public static class DistributedResponseCachingServicesExtensions
    {
        /// <summary>
        /// Add response caching services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedResponseCaching(this IServiceCollection services)
        {
			services.AddResponseCaching();

            return services;
        }

        /// <summary>
        /// Add response caching services and configure the related options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ResponseCachingOptions"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedResponseCaching(this IServiceCollection services, Action<CustomResponseCachingOptions> configureOptions)
        {
            var options = new CustomResponseCachingOptions();
            configureOptions?.Invoke(options);
            if (options.CacheAuthorizedRequest)
            {
                services.AddCacheAuthorizedRequestsResponseCachingPolicy();
            }
            services.Configure(configureOptions);
            return services.AddDistributedResponseCaching();
        }
    }
}
