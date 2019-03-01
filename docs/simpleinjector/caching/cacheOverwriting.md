# Overwriting Existing Cache Items Explicitly

In special cases, you may wish to explicitly replace specific items in the cache prior to their automatic expiry.  We wish to adhere to architectural best-practice and keep caching concerns confined to the boundary of our application (i.e. where `SimpleInjector` is used to wire everything together).  The most common use case for cache overwriting is immediately after some update is performed; thus, we can use the [decorator pattern](https://simpleinjector.readthedocs.io/en/latest/aop.html#decoration).

Cache overwriting functionality can be added by first installing the `Functional.CQS.AOP.Caching.Infrastructure` NuGet package in the same project containing `SimpleInjector` dependencies and then creating the appropriate decorator in that project.  The decorator will have an instance of [`IReplaceFunctionalCacheItem<TQuery, TResult>`](../../../src/Functional.CQS.AOP.Caching.Infrastructure/Invalidation/IReplaceFunctionalCacheItem.cs) constructor-injected into it, like so:

```
public CommandHandlerDecoratorThatReplacesItemsInCache : ICommandHandler<CommandParameterObject, CommandError>
{
    private readonly ICommandHandler<CommandParameterObject, CommandError> _decoratedHandler;
    private readonly IReplaceFunctionalCacheItem<QueryParameterObject1, bool> _query1CacheItemReplacer;
    private readonly IReplaceFunctionalCacheItem<QueryParameterObject2, int> _query2CacheItemReplacer;

    public CommandHandlerDecoratorThatReplacesItemsInCache(
        ICommandHandler<CommandParameterObject, CommandError> decoratedHandler,
        IReplaceFunctionalCacheItem<QueryParameterObject1, bool> query1CacheItemReplacer,
        IReplaceFunctionalCacheItem<QueryParameterObject2, int> query2CacheItemReplacer)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _query1CacheItemReplacer = query1CacheItemReplacer ?? throw new ArgumentNullException(nameof(query1CacheItemReplacer));
        _query2CacheItemReplacer = query2CacheItemReplacer ?? throw new ArgumentNullException(nameof(query2CacheItemReplacer));
    }

    public Result<Unit, CommandError> Handle(CommandParameterObject parameters)
    {
        ...

        var result = _decoratedHandler.Handle(parameters);
        result.Apply(_ =>
        {
            ...

            const bool NEW_BOOL_VALUE = true;
            // will need to map values stored in CommandParameterObject to generate the QueryParameterObject1 instance
            // the replacement item will have the same lifetime specified by IQueryResultCachingStrategy<QueryParameterObject1, bool> (not shown here)
            _query1CacheItemReplacer.ReplaceCacheItem(new QueryParameterObject1(...), NEW_BOOL_VALUE).Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...

            const int NEW_INT_VALUE = 10;
            // will need to map values stored in CommandParameterObject to generate the QueryParameterObject2 instance
            // the replacement item will use the time-to-live value supplied here instead of what IQueryResultCachingStrategy<QueryParameterObject2, int> (not shown here) would normally supply
            _query2CacheItemReplacer.ReplaceCacheItem(new QueryParameterObject2(...), NEW_INT_VALUE, TimeSpan.FromMinutes(15)).Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...
        }, failure => { /* DO NOTHING */});

        ...

        return result;
    }
}
```

Then, register your decorator after registering all CQS handlers, like so:

```
// pre-existing registration of all CQS handler implementations
container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(Program).Assembly)
	.WithCachingDecorator(() => new FunctionalMemoryCache(new MemoryCacheOptions()), cachingModuleConfiguration);

// new code to add decorator to specific command handler implementation
container.RegisterDecorator<ICommandHandler<CommandParameterObject, CommandError>, CommandHandlerDecoratorThatReplacesItemsInCache>();
```
