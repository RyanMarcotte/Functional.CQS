# Integrating `Functional.CQS.AOP` Into Your Application When Using the [`SimpleInjector`](https://simpleinjector.readthedocs.io/en/latest/quickstart.html) Dependency Injection Container

A series of NuGet packages are provided for integration with `SimpleInjector`, all prefixed with `Functional.CQS.AOP.IoC.SimpleInjector`.  Those packages are listed below.  Consult the linked documentation of individual packages for detailed information.  As a matter of best practice, all `Functional.CQS.AOP.IoC.SimpleInjector`-prefixed packages should only be installed in your application's [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/), as the packages work directly with the `SimpleInjector` container.

The following NuGet packages are core dependencies and must be installed:
- [`Functional.CQS.AOP.IoC.SimpleInjector`](gettingStarted.md)

The following `Functional.CQS` handler decoration modules are available via NuGet and are optional:
- [`Functional.CQS.AOP.IoC.SimpleInjector.Caching`](caching/README.md) (for caching results returned from query handlers)
- [`Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing`](metricsCapturing/README.md) (for capturing performance metrics and unhandled errors)