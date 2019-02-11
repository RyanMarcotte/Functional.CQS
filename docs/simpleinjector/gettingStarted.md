# Getting Started with `Functional.CQS.AOP.IoC.SimpleInjector.Installation`

Download the `Functional.CQS.AOP.IoC.SimpleInjector.Installation` package from NuGet, installing it in the project acting as your application's [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/).  Then, in your application's startup code (which holds a reference to the `SimpleInjector` container):

```
// register all CQS handlers contained in the specified assemblies
// all CQS handler implementations will be registered with the specified lifestyle
container.RegisterAllFunctionalHandlers(Lifestyle.Singleton, typeof(CompositionRootClass).Assembly, typeof(InfrastructureComponentClass).Assembly, ...)
```

The above code will scan the listed assemblies for all implementations of the following `Functional.CQS` contracts, registering them with the container using the specified `Lifestyle`:
- `IQueryHandler<TQuery, TResult>`
- `IAsyncQueryHandler<TQuery, TResult>`
- `ICommandHandler<TCommand, TError>`
- `IAsyncCommandHandler<TCommand, TError>`

Example usage can be found [here](../../src/Functional.CQS.AOP.IoC.SimpleInjector.Tests/ContainerExtensionsTests.cs).