using System;
using ExternalNetcoreExtensions.Custom;
using ExternalNetcoreExtensions.ModifiableDistributed;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.AspNetCore.ResponseCaching.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods for the ResponseCaching middleware.
	/// </summary>
	public static class ModifiableDistributedResponseCacheExtensions
	{
		/// <summary>
		/// Add response caching services.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
		/// <returns></returns>
		public static IServiceCollection AddModifiableDistributedResponseCache(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddCustomResponseCaching();

			services.TryAdd(ServiceDescriptor.Singleton<ICustomResponseCache, ModifiableDistributedResponseCache>());

			return services;
		}

		/// <summary>
		/// Add response caching services and configure the related options.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
		/// <param name="configureOptions">A delegate to configure the <see cref="ResponseCachingOptions"/>.</param>
		/// <returns></returns>
		public static IServiceCollection AddModifiableDistributedResponseCache(this IServiceCollection services, Action<ResponseCachingOptions> configureOptions)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}
			if (configureOptions == null)
			{
				throw new ArgumentNullException(nameof(configureOptions));
			}

			services.Configure(configureOptions);
			services.AddModifiableDistributedResponseCache();

			return services;
		}
	}
}
