# Invalidating Cache Items Explicitly

In special cases, you may wish to explicitly invalidate specific items in the cache.  We wish to adhere to architectural best-practice and keep caching concerns confined to the boundary of our application (i.e. where `SimpleInjector` is used to wire everything together).  The most common use case for cache invalidation is immediately after some update is performed; thus, we can use the [decorator pattern and register that decorator using `SimpleInjector`](https://simpleinjector.readthedocs.io/en/latest/aop.html#decoration).  The [testbed application](../../../src/IQ.Vanilla.CQS.AOP.IoC.SimpleInjector.Testbed/Program.cs) demonstrates how to do this.

Cache invalidation functionality can be added by first installing the `IQ.Vanilla.CQS.AOP.Caching.Invalidation` NuGet package in the same project containing `SimpleInjector` dependencies and then creating the appropriate decorator in that project.  The decorator will have an instance of [`IInvalidateIQVanillaCQSAOPCacheItem<TQuery, TResult>`](../../../src/IQ.Vanilla.CQS.AOP.Caching.Invalidation/IInvalidateIQVanillaCQSAOPCacheItem.cs) constructor-injected into it, like so:

```
public ResultCommandHandlerThatInvalidatesSingleItemsInCache : IResultCommandHandler<CommandParameterObject, CommandError>
{
    private readonly IResultCommandHandler<CommandParameterObject, CommandError> _decoratedHandler;
    private readonly IInvalidateIQVanillaCQSAOPCacheItem<QueryParameterObject1, bool> _query1CacheItemInvalidator;
    private readonly IInvalidateIQVanillaCQSAOPCacheItem<QueryParameterObject2, int> _query2CacheItemInvalidator;

    public ResultCommandHandlerThatInvalidatesSingleItemsInCache(
        IResultCommandHandler<CommandParameterObject, CommandError> decoratedHandler,
        IInvalidateIQVanillaCQSAOPCacheItem<QueryParameterObject1, bool> query1CacheItemInvalidator,
        IInvalidateIQVanillaCQSAOPCacheItem<QueryParameterObject2, int> query2CacheItemInvalidator)
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

To invalidate cache items that are related by a cache grouping key or to invalidate all cache items, follow a similar decorator-based approach as above.  You must instead constructor-inject an [`IInvalidateIQVanillaCQSAOPCacheItems`](../../../src/IQ.Vanilla.CQS.AOP.Caching.Invalidation/IInvalidateIQVanillaCQSAOPCacheItems.cs) instance into that decorator.  For example:

```
public ResultCommandHandlerThatInvalidatesItemsInCache : IResultCommandHandler<CommandParameterObject, CommandError>
{
    private readonly IResultCommandHandler<CommandParameterObject, CommandError> _decoratedHandler;
    private readonly IInvalidateIQVanillaCQSAOPCacheItems _cacheItemInvalidator;

    public CommandHandlerThatInvalidatesItemsInCache(
        IResultCommandHandler<CommandParameterObject, CommandError> decoratedHandler,
        IInvalidateIQVanillaCQSAOPCacheItems cacheItemInvalidator)
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