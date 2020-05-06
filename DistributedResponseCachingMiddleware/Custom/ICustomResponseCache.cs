using Microsoft.AspNetCore.ResponseCaching.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExternalNetcoreExtensions.Custom
{
	/// <summary>
	/// Make your own response cache! Note that it is intended to be injected into middleware (via
	/// constructor injection), so functionally it's a singleton, whether or not you add it to the
	/// service collection like that or not.
	/// </summary>
	public interface ICustomResponseCache : IResponseCache
	{
	}
}
