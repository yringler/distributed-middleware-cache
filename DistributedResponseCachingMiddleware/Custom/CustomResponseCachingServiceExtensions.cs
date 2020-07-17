// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExternalNetcoreExtensions.Distributed;
using ExternalNetcoreExtensions.ModifiableDistributed;
using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for the ResponseCaching middleware.
    /// </summary>
    /// <remarks>
    /// These are just wrappers around AddResponseCaching. For some reason, I get compilation errors
    /// in my aspnetcore project when trying to call AddResponseCaching, because it's defined in two assemblies.
    /// 
    /// Note that this is a copypasta from <see cref="DistributedResponseCachingServicesExtensions"/>. I added it
    /// to make the API symmetrical.
    /// 
    /// In a later version, perhaps DistributedResponseCaching will be deprecated, and the CustomResponseCache will use either
    /// an IDistributedCache, or a ICustomResponseCache if it's available.
    /// </remarks>
    public static class CustomResponseCachingServiceExtensions
    {
        public static IServiceCollection AddDefaultResponseCaching(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            AddResponseCaching(services, options => { });
            return services;
        }

        /// <summary>
        /// Add response caching services and configure the related options.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
        /// <returns></returns>
        public static IServiceCollection AddResponseCaching(this IServiceCollection services, Action<CustomResponseCachingOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new CustomResponseCachingOptions();
            configureOptions(options);
            services.Configure(configureOptions);

            _ = options.ResponseCachingStrategy switch
            {
                ResponseCachingStrategy.Distributed => services.RegisterResponseCache<DistributedResponseCache>(),
                ResponseCachingStrategy.ModifiableDistributed => services.RegisterResponseCache<ModifiableDistributedResponseCache>(),
                _ => services.AddSingleton<IMemoryCache, MemoryCache>()
                    .RegisterResponseCache(x => new MemoryResponseCache(x.GetRequiredService<IMemoryCache>()))

            };
            if (options.CacheAuthorizedRequest)
            {
                services.AddCacheAuthorizedRequestsResponseCachingPolicy();
            }
            services.AddResponseCaching();
            return services;
        }

        private static IServiceCollection RegisterResponseCache<T>(this IServiceCollection services, Func<IServiceProvider, T> responseCache = default)
            where T : class, IResponseCache
        {
            return responseCache == default ? services.AddSingleton<IResponseCache, T>() :
                services.AddSingleton<IResponseCache>(responseCache);
        }
    }
}

