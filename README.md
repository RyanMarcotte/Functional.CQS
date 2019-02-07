# Functional.CQS.AOP

`Functional.CQS.AOP` is a set of NuGet packages that provide decoration facilities over implementations of [`Functional.CQS` handler contracts](src/Functional.CQS), specifically for [aspect-oriented programming](https://en.wikipedia.org/wiki/Aspect-oriented_programming) / applying [cross-cutting concerns](https://stackoverflow.com/questions/23700540/cross-cutting-concern-example).

The following contracts are defined:
- [`IQueryParameters<TResult>`](src/Functional.CQS/IQueryParameters.cs), a marker interface for query parameter objects that associates the query parameter object with the type of result that is returned when invoking the query handler
- [`IQueryHandler<TQuery, TResult>`](src/Functional.CQS/IQueryHandler.cs), for synchronous query handling
- [`IAsyncQueryHandler<TQuery, TResult>`](src/Functional.CQS/IAsyncQueryHandler.cs), for asynchronous query handling
- [`ICommandParameters<TError>`](src/Functional.CQS/ICommandParameters.cs), a marker interface for command parameter objects that associates the command parameter object with the type of error that can potentially occur when invoking the command handler
- [`ICommandHandler<TCommand, TError>`](src/Functional.CQS/ICommandHandler.cs), for synchronous command handling
- [`IAsyncCommandHandler<TCommand, TError>`](src/Functional.CQS/IAsyncCommandHandler.cs), for asynchronous command handling

[What is CQS?  Why use CQS?  Why use Functional.CQS.AOP?](docs/cqs_primer.md)

## Library Usage Cookbook

You can find examples demonstrating how to use the various `Functional.CQS.AOP` libraries via the links below.  Note that a general understanding of [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection), the [decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern), and [onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/) is assumed.
- [Integrating `Functional.CQS.AOP` when using the `SimpleInjector` dependency injection container](docs/simpleinjector/README.md)

## Information

[Do you wish to contribute?](contributing.md)

If you encounter problems with any `Functional.CQS.AOP` package, some documentation is unclear, or you have questions not adequately answered by the existing documentation, please [report an issue](https://github.com/RyanMarcotte/Functional.CQS/issues).