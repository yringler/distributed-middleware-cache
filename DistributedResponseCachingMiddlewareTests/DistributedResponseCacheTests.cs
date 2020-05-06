using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Threading;
using ExternalNetcoreExtensions.Distributed;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DistributedResponseCachingMiddlewareTests
{
	[TestFixture]
	public class DistributedResponseCacheTests
	{
		[Test]
		public void CanWriteCachedResponse()
		{
			var mockDistributedCache = new Mock<IDistributedCache>();
			var responseCache = new DistributedResponseCache(mockDistributedCache.Object);

			using var stream = new MemoryStream();
			using (var writer = new StreamWriter(stream, leaveOpen: true))
			{
				writer.Write("data");
			}
			stream.Position = 0;

			Assert.That(() => responseCache.Set("test", new CachedResponse
			{
				Body = stream,
				Headers = new HeaderDictionary(new Dictionary<string, StringValues>
				{
					["x-test-header"] = "testing...testing"
				})
			}, TimeSpan.FromMinutes(1)), Throws.Nothing);
		}

		[Test]
		public void CanReadCache()
		{
			string json = "{\"Created\":\"0001-01-01T00:00:00+00:00\",\"StatusCode\":0,\"Headers\":[{\"Key\":\"x-test-header\",\"Value\":[\"testing...testing\"]}],\"Body\":\"ZGF0YQ==\"}";
			using var stream = new MemoryStream();

			using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
			{
				binaryWriter.Write("SerializableCachedResponse");
				binaryWriter.Flush();
			}

			using var writer = new StreamWriter(stream, leaveOpen: true);
			writer.Write(json);
			writer.Flush();

			stream.Position = 0;

			var mockDistributedCache = new Mock<IDistributedCache>();
			mockDistributedCache.Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(stream.ToArray());

			var responseCache = new DistributedResponseCache(mockDistributedCache.Object);

			var savedValue = responseCache.Get("somekey") as CachedResponse;

			Assert.That(savedValue, Is.Not.Null);
			Assert.That(savedValue.Headers["x-test-header"], Is.EqualTo("testing...testing"));

			using var reader = new StreamReader(savedValue.Body);
			string body = reader.ReadToEnd();

			Assert.That(body, Is.EqualTo("data"));
		}

		[Test]
		public void CanWriteCacheVaryByRules()
		{
			var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
			var responseCache = new DistributedResponseCache(cache);

			using var stream = new MemoryStream();
			using (var writer = new StreamWriter(stream, leaveOpen: true))
			{
				writer.Write("data");
			}
			stream.Position = 0;

			Assert.That(() => responseCache.Set("test", new CachedVaryByRules
			{
				Headers = new StringValues("x-test-header"),
				QueryKeys = new StringValues("x-test-query"),
				VaryByKeyPrefix = "test"
			}, TimeSpan.FromMinutes(1)), Throws.Nothing);

			var item = responseCache.Get("test");
			Assert.That(item, Is.Not.Null);
			var realItem = item as CachedVaryByRules;
			Assert.That(item, Is.Not.Null);
		}
	}
}