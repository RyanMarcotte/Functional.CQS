# Adding Caching With `IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching`

`IQ.Vanilla.CQS.AOP.IoC.PureDI.Caching` supplies a caching decorator implementation, while `IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Caching` supplies a set of extension methods that make it easy to register that decoration with the `SimpleInjector` container.

The caching decorator can only be applied to query handler (`IQueryHandler<TQuery, TResult>` and `IAsyncQueryHandler<TQuery, TResult>`) implementations.  Command handler implementations (`ICommandHandler<TCommand>`, `IAsyncCommandHandler<TCommand>`, `IResultCommandHandler<TCommand, TError>`, and `IAsyncResultCommandHandler<TCommand, TError>`) can never have the decorator applied to them.

The caching decorator works by checking if a particular query result is stored in an internal cache.  If so, then that result is returned and the query handler is not run; otherwise, execution of the query handler proceeds as normal and the result of that query is stored in the cache.

Consult the following documentation for usage instructions and suggestions for best practice:
- [Installation](installation.md)
- [Applying Caching Concerns to Specific `IQueryHandler<TQuery, TResult>` and `IAsyncQueryHandler<TQuery, TResult>` Implementations](applyingCaching.md)
- [Invalidating Cache Items Explicitly](cacheInvalidation.md)
- [Overwriting Existing Cache Items Explicitly](cacheOverwriting.md)