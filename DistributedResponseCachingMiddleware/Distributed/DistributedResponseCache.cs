using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExternalNetcoreExtensions.Distributed
{
	public class DistributedResponseCache : IResponseCache
	{
		private readonly IDistributedCache cache;

		public DistributedResponseCache(IDistributedCache cache)
		{
			this.cache = cache;
		}

		public IResponseCacheEntry Get(string key)
		{
			return GetAsync(key).Result;
		}

		public async Task<IResponseCacheEntry> GetAsync(string key)
		{
			var bytes = await cache.GetAsync(key);

			if (bytes == null)
			{
				return null;
			}

			using var stream = new MemoryStream(bytes);
			var data = await JsonSerializer.DeserializeAsync<SerializableCachedResponse>(stream);

			return data.ToCachedResponse();
		}

		public void Set(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			SetAsync(key, entry, validFor).Wait();
		}

		public async Task SetAsync(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			var cachedResponse = SerializableCachedResponse.From(entry as CachedResponse);

			using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, cachedResponse);

			await cache.SetAsync(key, stream.ToArray(), new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = validFor
			});
		}
	}

	internal class SerializableCachedResponse
	{
		public DateTimeOffset Created { get; set; }

		public int StatusCode { get; set; }

		public List<KeyValuePair<string, List<string>>> Headers { get; set; }

		public byte[] Body { get; set; }

		public static SerializableCachedResponse From(CachedResponse cachedResponse)
		{
			if (cachedResponse == null)
			{
				return null;
			}

			var response =  new SerializableCachedResponse
			{
				Created = cachedResponse.Created,
				StatusCode = cachedResponse.StatusCode,
				Headers = cachedResponse.Headers
					.Select(header => new KeyValuePair<string, List<string>>(header.Key, header.Value.ToList()))
					.ToList()
			};

			if (cachedResponse.Body != null)
			{
				using var stream = new MemoryStream();
				cachedResponse.Body.CopyTo(stream);
				response.Body = stream.ToArray();
			}

			return response;
		}

		public CachedResponse ToCachedResponse()
		{
			return new CachedResponse
			{
				Created = Created,
				Body = new MemoryStream(Body),
				Headers = new HeaderDictionary(Headers.ToDictionary(
					header => header.Key,
					header => header.Value.Any() ? new StringValues(header.Value.ToArray()) : StringValues.Empty))
			};
		}
	}
}
