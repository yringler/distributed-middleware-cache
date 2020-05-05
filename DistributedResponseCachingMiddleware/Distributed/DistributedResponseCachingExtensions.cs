using ExternalNetcoreExtensions.Distributed;
using System;

namespace Microsoft.AspNetCore.Builder
{
	public static class DistributedResponseCachingExtensions
	{
		public static IApplicationBuilder UseDistributedResponseCaching(this IApplicationBuilder app)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			return app.UseMiddleware<DistributedResponseCachingMiddleware>();
		}
	}
}
