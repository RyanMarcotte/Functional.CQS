# Installation

Installation and decorator code for caching is defined by the `Functional.CQS.AOP.IoC.SimpleInjector.Caching` NuGet package.  It functions as an extension of the `Functional.CQS.AOP.IoC.SimpleInjector` package.

Rather than reference your application configuration file directly, `Functional.CQS.AOP.IoC.SimpleInjector.Caching` requires that a [`CachingModuleConfigurationParameters`](../../../src/Functional.CQS.AOP.IoC.SimpleInjector.Caching/Configuration/CachingModuleConfigurationParameters.cs) instance be supplied as part of the installation process.  Your application configuration can be any format you choose.  You can also supply the configuration values directly in code rather than loading them from an application configuration file.

```
// instantiate the module configuration
var cachingModuleConfiguration = new CachingModuleConfigurationParameters(true);

// register CQS handlers, caching components
// container.RegisterAllFunctionalCQSHandlers returns an object that enables fluent chaining, exposing WithCachingDecorator extension method
container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(Program).Assembly)
	.WithCachingDecorator<YourOwnCacheImplementation>(new YourOwnCacheImplementation(...), cachingModuleConfiguration);
```

`YourOwnCacheImplementation` must implement [`IFunctionalCache`](../../../src/Functional.CQS.AOP.Caching.Infrastructure/IFunctionalCache.cs).

Multiple implementations of `IFunctionalCache` exist:
- an in-memory cache is provided by the `Functional.CQS.AOP.Caching.Infrastructure.MemoryCache` NuGet package
- a distributed cache backed by Redis is provided by the `Functional.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis` NuGet package

The listed assemblies will be scanned for all implementations of caching strategy contracts and register those implementations with the container.  Caching strategy implementations must be stateless, as they are registered using `Lifestyle.Singleton`.