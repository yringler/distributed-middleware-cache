using ExternalNetcoreExtensions.Custom;
using ExternalNetcoreExtensions.Distributed;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.ModifiableDistributed
{
	/// <summary>
	/// An implementation of <see cref="ICustomResponseCache"/> response cache which allows for custom cache control
	/// with <see cref="IModifiableCacheController"/>.
	/// For use with <see cref="CustomResponseCachingMiddleware"/>.
	/// In order to use, must inject a <see cref="IModifiableCacheController"/>. 
	/// </summary>
	internal class ModifiableDistributedResponseCache : ICustomResponseCache
	{
		private readonly DistributedResponseCache cache;
		private readonly IDistributedCache distributedCache;
		private readonly IModifiableCacheController cacheController;

		public ModifiableDistributedResponseCache(IDistributedCache distributedCache, IModifiableCacheController cacheController)
		{
			cache = new DistributedResponseCache(distributedCache);
			this.distributedCache = distributedCache;
			this.cacheController = cacheController;
		}

		public IResponseCacheEntry Get(string key)
		{
			if (cacheController.ShouldClearCache) distributedCache.Remove(key);
			if (!cacheController.ShouldReadFromCache || cacheController.ShouldClearCache) return null;
			return cache.Get(key);
		}

		public async Task<IResponseCacheEntry> GetAsync(string key)
		{
			if (cacheController.ShouldClearCache) await distributedCache.RemoveAsync(key);
			if (!cacheController.ShouldReadFromCache || cacheController.ShouldClearCache) return null;
			return await cache.GetAsync(key);
		}

		public void Set(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			if (!cacheController.ShouldWriteToCache) return;
			cache.Set(key, entry, validFor);
		}

		public async Task SetAsync(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			if (!cacheController.ShouldWriteToCache) return;
			await cache.SetAsync(key, entry, validFor);
		}
	}
}
