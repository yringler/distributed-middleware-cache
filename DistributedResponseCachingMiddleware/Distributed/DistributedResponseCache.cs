using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			using var reader = new BinaryReader(stream, Encoding.UTF8, true);

			string cachedType = reader.ReadString();

			if (cachedType == typeof(SerializableCachedResponse).Name)
			{
				var data = await JsonSerializer.DeserializeAsync<SerializableCachedResponse>(stream);
				return data?.ToCachedResponse();
			}
			else if (cachedType == typeof(SerializableCacheVaryByRules).Name)
			{
				var data = await JsonSerializer.DeserializeAsync<SerializableCacheVaryByRules>(stream);
				return data?.ToCachedVaryByRules();
			}
			else
			{
				return null;
			}
		}

		public void Set(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			SetAsync(key, entry, validFor).Wait();
		}

		public async Task SetAsync(string key, IResponseCacheEntry entry, TimeSpan validFor)
		{
			object serializableEntry = null;

			if (entry is CachedResponse cachedResponse)
			{
				serializableEntry = SerializableCachedResponse.From(cachedResponse);
			}
			else if (entry is CachedVaryByRules varyByRules)
			{
				serializableEntry = SerializableCacheVaryByRules.From(varyByRules);
			}

			using var stream = new MemoryStream();

			using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
			{
				writer.Write(serializableEntry.GetType().Name);
				writer.Flush();
			}

			await JsonSerializer.SerializeAsync(stream, serializableEntry, serializableEntry.GetType());

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
					header => header.Value.Any() ? new StringValues(header.Value.ToArray()) : StringValues.Empty)),
				StatusCode = StatusCode
			};
		}
	}

	public class SerializableCacheVaryByRules
	{
		public string VaryByKeyPrefix { get; set; }

		public List<string> Headers { get; set; }

		public List<string> QueryKeys { get; set; }

		public static SerializableCacheVaryByRules From(CachedVaryByRules varyByRules)
		{
			if (varyByRules == null)
			{
				return null;
			}

			return new SerializableCacheVaryByRules
			{
				VaryByKeyPrefix = varyByRules.VaryByKeyPrefix,
				Headers = varyByRules.Headers.ToList(),
				QueryKeys = varyByRules.QueryKeys.ToList()
			};
		}

		public CachedVaryByRules ToCachedVaryByRules()
		{
			return new CachedVaryByRules
			{
				VaryByKeyPrefix = VaryByKeyPrefix,
				Headers = new StringValues(Headers.ToArray()),
				QueryKeys = new StringValues(QueryKeys.ToArray())
			};
		}
	}
}
