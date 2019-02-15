# Invalidating Cache Items Explicitly

In special cases, you may wish to explicitly invalidate specific items in the cache.  We wish to adhere to architectural best-practice and keep caching concerns confined to the boundary of our application (i.e. where `SimpleInjector` is used to wire everything together).  The most common use case for cache invalidation is immediately after some update is performed; thus, we can use the [decorator pattern and register that decorator using `SimpleInjector`](https://simpleinjector.readthedocs.io/en/latest/aop.html#decoration).

Cache invalidation functionality can be added by first installing the `Functional.CQS.AOP.Caching.Infrastructure` NuGet package in the same project containing `SimpleInjector` dependencies and then creating the appropriate decorator in that project.  The decorator will have an instance of [`IInvalidateFunctionalCacheItem<TQuery, TResult>`](../../../src/Functional.CQS.AOP.Caching.Infrastructure/Invalidation/IInvalidateFunctionalCacheItem.cs) constructor-injected into it, like so:

```
public CommandHandlerThatInvalidatesSingleItemsInCache : ICommandHandler<CommandParameterObject, CommandError>
{
    private readonly ICommandHandler<CommandParameterObject, CommandError> _decoratedHandler;
    private readonly IInvalidateFunctionalCacheItem<QueryParameterObject1, bool> _query1CacheItemInvalidator;
    private readonly IInvalidateFunctionalCacheItem<QueryParameterObject2, int> _query2CacheItemInvalidator;

    public CommandHandlerThatInvalidatesSingleItemsInCache(
        ICommandHandler<CommandParameterObject, CommandError> decoratedHandler,
        IInvalidateFunctionalCacheItem<QueryParameterObject1, bool> query1CacheItemInvalidator,
        IInvalidateFunctionalCacheItem<QueryParameterObject2, int> query2CacheItemInvalidator)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _query1CacheItemInvalidator = query1CacheItemInvalidator ?? throw new ArgumentNullException(nameof(query1CacheItemInvalidator));
        _query2CacheItemInvalidator = query2CacheItemInvalidator ?? throw new ArgumentNullException(nameof(query2CacheItemInvalidator));
    }

    public Result<Unit, CommandError> Handle(CommandParameterObject parameters)
    {
        ...

        var result = _decoratedHandler.Handle(parameters);
        result.Apply(_ =>
        {
            ...

            // will need to map values stored in CommandParameterObject to generate the QueryParameterObject1 instance
            _query1CacheItemInvalidator.InvalidateCacheItem(new QueryParameterObject1(...)).Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...

            // will need to map values stored in CommandParameterObject to generate the QueryParameterObject2 instance
            _query2CacheItemInvalidator.InvalidateCacheItem(new QueryParameterObject2(...)).Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...
        }, failure => { /* DO NOTHING */ });     

        ...

        return result;
    }
}
```

To invalidate cache items that are related by a cache grouping key or to invalidate all cache items, follow a similar decorator-based approach as above.  You must instead constructor-inject an [`IInvalidateFunctionalCacheItems`](../../../src/Functional.CQS.AOP.Caching.Infrastructure/Invalidation/IInvalidateFunctionalCacheItems.cs) instance into that decorator.  For example:

```
public CommandHandlerThatInvalidatesItemsInCache : ICommandHandler<CommandParameterObject, CommandError>
{
    private readonly ICommandHandler<CommandParameterObject, CommandError> _decoratedHandler;
    private readonly IInvalidateFunctionalCacheItems _cacheItemInvalidator;

    public CommandHandlerThatInvalidatesItemsInCache(
        ICommandHandler<CommandParameterObject, CommandError> decoratedHandler,
        IInvalidateFunctionalCacheItems cacheItemInvalidator)
    {
        _decoratedHandler = decoratedHandler ?? throw new ArgumentNullException(nameof(decoratedHandler));
        _cacheItemInvalidator = cacheItemInvalidator ?? throw new ArgumentNullException(nameof(cacheItemInvalidator));
    }

    public Result<Unit, CommandError> Handle(CommandParameterObject parameters)
    {
        ...

        var result = _decoratedHandler.Handle(parameters);
        result.Apply(_ =>
        {
            ...

            // invalidate a group of cache items
            const string GROUP_KEY = "some group key you should define in a public static class";
            _cacheItemInvalidator.InvalidateCacheItemGroup(GROUP_KEY).Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...

            // invalidate all cache items
            _cacheItemInvalidator.InvalidateAllCacheItems().Apply(
                _ => { /* DO NOTHING */ },
                exception => { /* HANDLE ERROR */}
            );

            ...
        }, failure => { /* DO NOTHING */ });

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
container.RegisterDecorator<ICommandHandler<CommandParameterObject, CommandError>, CommandHandlerThatInvalidatesItemsInCache>();
```