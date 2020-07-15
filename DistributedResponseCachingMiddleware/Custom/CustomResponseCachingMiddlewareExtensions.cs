using ExternalNetcoreExtensions.Custom;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class CustomResponseCachingMiddlewareExtensions
	{
		public static IApplicationBuilder UseCustomResponseCaching(this IApplicationBuilder app)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			return app.UseMiddleware<CustomResponseCachingMiddleware>();
		}
    }
}
