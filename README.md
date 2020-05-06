# flex-middleware-cache
A fork of ResponseCachingMiddleware which provides more flexibility. Note that it uses official aspnetcore code directly (via git submodules and csproj linking).

## DistributedResponseCache
For usage see [ResponseCachingMiddleware](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1), just replace AddResponseCaching with AddDistributedResponseCaching, and UseResponseCaching with UseDistributedResponseCaching.<br>
It will use whatever implentation of IDistributedCache that is injected into it.

## CustomReponseCache
The sky is the limit! Do whatever you want in your own custom response cache. Use AddCustomResponseCaching() and UseCustomResponseCaching().<br>
And add you're custom response cache to the service provider. It must implement ICustomResponseCache. Note that DistributedResponseCache itself is an ICustomResponseCache, so you could use it internaly and implement the interface through it, adding whatever custom wrapper logic you like.

## ModifiableDistributedResponseCache
This is a wrapper around DistributedResponseCache which allows you to easily exert some basic control over the underlying IDistributedCache. You can use your own custom logic to force the cache to clear, or to just ignore it.<br>
To use, call AddModifiableDistributedResponseCache() and UseCustomResponseCaching().<br>
And inject IModifiableCacheController into the service provider.
