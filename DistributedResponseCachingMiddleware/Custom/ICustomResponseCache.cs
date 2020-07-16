using Microsoft.AspNetCore.ResponseCaching;

namespace ExternalNetcoreExtensions.Custom
{
    /// <summary>
    /// Make your own response cache (via fork or PR)! Note that it is intended to be injected into middleware (via
    /// constructor injection), so functionally it's a singleton, whether or not you add it to the
    /// service collection like that or not.
    /// For now this has been switched to being internal, for simpler upgrade to netcore3.
    /// </summary>
    /// TODO can we remove this interface?
    internal interface ICustomResponseCache : IResponseCache
	{
	}
}
