# flex-middleware-cache
A fork of ResponseCachingMiddleware which provides more flexibility. Note that it uses official aspnetcore code directly (via git submodules and csproj linking).

## DistributedResponseCache
Usage is similar to [ResponseCachingMiddleware](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-3.1). Set the ResponeCachingStrategy to ResponseCachingStrategy.Distributed when calling AddResponseCaching and replace UseResponseCaching with UseCustomResponseCaching.<br>
It will use whatever implentation of IDistributedCache that is injected into it.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddResponseCaching(options =>
    {
        options.ResponseCachingStrategy = ResponseCachingStrategy.Distributed;
    });
}
```
```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseCustomResponseCaching();
}
```

## ModifiableDistributedResponseCache
This is a wrapper around DistributedResponseCache which allows you to easily exert some basic control over the underlying IDistributedCache. You can use your own custom logic to force the cache to clear, or to just ignore it.<br>
To use, set the ResponseCachingStrategy to ResponseCachingStrategy.ModifiableDistributed and call UseCustomResponseCaching().<br>
And inject IModifiableCacheController into the service provider.
