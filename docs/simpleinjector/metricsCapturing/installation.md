# Installation

Installation and decorator code for metrics-capturing is defined by the `Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing` NuGet package.  It functions as an extension of the `Functional.CQS.AOP.IoC.SimpleInjector` package.

Rather than reference your application configuration file directly, `Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing` requires that a [`MetricsCapturingModuleConfigurationParameters`](../../../src/Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing/Configuration/MetricsCapturingModuleConfigurationParameters.cs) instance be supplied as part of the installation process.  Your application configuration can be any format you choose.  You can also supply the configuration values directly in code rather than loading them from an application configuration file.

```
// instantiate the module configuration
var metricsCapturingModuleConfiguration = new MetricsCapturingModuleConfigurationParameters(true, true, true, true);

// register CQS handlers, metrics-capturing components
// container.RegisterAllFunctionalCQSHandlers returns an object that enables fluent chaining, exposing WithMetricsCapturingDecorator extension method
container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(CompositionRootClass).Assembly, typeof(InfrastructureComponentClass).Assembly, ...)
	.WithMetricsCapturingDecorator<YourUniversalMetricsCapturingStrategyType>(metricsCapturingModuleConfiguration);
```

The listed assemblies will be scanned for all implementations of metrics-capturing strategy contracts and register those implementations with the container.  Metrics-capturing strategy implementations must be stateless, as they are registered using `Lifestyle.Singleton`.