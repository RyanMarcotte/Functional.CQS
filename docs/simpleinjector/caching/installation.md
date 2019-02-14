# Installation

Installation and decorator code for caching is defined by the `IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching` NuGet package.  It functions as an extension of the `IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Installation` package.

Rather than reference your application configuration file directly, `IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching` requires that a [`CachingModuleConfigurationParameters`](../../../src/IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching/Configuration/CachingModuleConfigurationParameters.cs) instance be supplied as part of the installation process.  Your application configuration can be any format you choose.  You can also supply the configuration values directly in code rather than loading them from an application configuration file.

```
// instantiate the module configuration
var cachingModuleConfiguration = new CachingModuleConfigurationParameters(true);

// register CQS handlers, caching components
// container.RegisterAllIQVanillaCQSHandlers returns an object that enables fluent chaining, exposing WithCachingDecorator extension method
container.RegisterAllIQVanillaCQSHandlers(Lifestyle.Singleton, typeof(Program).Assembly)
	.WithCachingDecorator<YourOwnCacheImplementation, YourOwnCacheKeySuffixFactoryImplementation>(new YourOwnCacheImplementation(...), cachingModuleConfiguration);
```

`YourOwnCacheImplementation` must implement [`IIQVanillaCQSAOPCache`](../../../src/IQ.Vanilla.CQS.AOP.Caching.Infrastructure/IIQVanillaCQSAOPCache.cs) and `YourOwnCacheKeySuffixFactoryImplementation` must implement [`IIQVanillaCQSAOPCacheKeySuffixFactory`](../../../src/IQ.Vanilla.CQS.AOP.Caching.Infrastructure\IIQVanillaCQSAOPCacheKeySuffixFactory.cs).

Multiple implementations of `IIQVanillaCQSAOPCache` exist:
- an in-memory cache is provided by the `IQ.Vanilla.CQS.AOP.Caching.Infrastructure.MemoryCache` NuGet package
- a distributed cache backed by Redis is provided by the `IQ.Vanilla.CQS.AOP.Caching.Infrastructure.DistributedCache.Redis` NuGet package

[Multiple overloads of the `RegisterCachingComponents` extension method exist](../../../src/IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching/ContainerExtensions.cs).

The listed assemblies will be scanned for all implementations of caching strategy contracts and register those implementations with the container.  Caching strategy implementations must be stateless, as they are registered using `Lifestyle.Singleton`.

Consult the [testbed application](../../../src/IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Testbed\Program.cs) and [application configuration](../../../src/IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Testbed\appsettings.json) for an example of loading JSON from disk, mapping that JSON object to `CachingModuleConfigurationParameters`, and supplying that `CachingModuleConfigurationParameters` instance as part of the `IQ.Vanilla.CQS.AOP` installation.