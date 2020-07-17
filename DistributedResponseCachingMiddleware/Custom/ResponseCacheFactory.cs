using ExternalNetcoreExtensions.Distributed;
using ExternalNetcoreExtensions.Utility;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ExternalNetcoreExtensions.Custom
{
    internal class ResponseCacheFactory : IResponseCacheFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMemoryCache memoryCache;
        private readonly CustomResponseCachingOptions customResponseCachingOptions;

        public ResponseCacheFactory(IServiceProvider serviceProvider, IOptions<CustomResponseCachingOptions> customResponseCachingOptions, IMemoryCache memoryCache)
        {
            this.serviceProvider = serviceProvider;
            this.memoryCache = memoryCache;
            this.customResponseCachingOptions = customResponseCachingOptions.Value;
        }

        IResponseCache IResponseCacheFactory.Create()
        {
            return customResponseCachingOptions.ResponseCachingStrategy switch
            {
                ResponseCachingStrategy.Local => new MemoryResponseCache(memoryCache),
                _ => this.serviceProvider.GetService<IResponseCache>()
            };
        }
    }

    internal interface IResponseCacheFactory
    {
        internal IResponseCache Create();
    }
}
