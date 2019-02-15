# Applying Caching Concerns to Specific `IQueryHandler<TQuery, TResult>` and `IAsyncQueryHandler<TQuery, TResult>` Implementations

The caching decorator requires that a query handler have a corresponding [`IQueryResultCachingStrategy<TQuery, TResult>`](../../../src/Functional.CQS.AOP.Caching/IQueryResultCachingStrategy.cs) implementation (hereafter referred to as **caching strategy implementation**) defined.

The `IQueryResultCachingStrategy<TQuery, TResult>` contract is defined by the `Functional.CQS.AOP.Caching` NuGet package.  **As best practice, caching strategy implementations should be defined near the composition root**.  In other words, caching strategy implementations should be defined in the same assembly containing the `Functional.CQS.AOP` installer code, or in a dedicated component registration assembly.  It should _not_ be defined in the same assembly as query parameter objects (`TQuery`) or query handler implementations.  This is because caching is an infrastructural concern, and thus belongs [at the boundary of the system and not within the application domain](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/).

Here is an example query parameter object and query handler:

```
// a Functional query parameter object
// assume SystemSettingsForCompany is a POCO (plain old C# object)
// query returns Option<SystemSettingsForCompany> because it is possible that a company entity ID is invalid and no result would be returned in that case
// constructor boilerplate has been left out for brevity
public class GetSystemSettingsForCompanyQuery : IQuery<Result<Option<SystemSettingsForCompany>, Exception>>
{
    public int CompanyEntityID { get; }
}
```

```
// an Functional query handler implementation
// actual implementation has not been included as it is not relevant to caching 
public class GetSystemSettingsForCompanyQueryHandler : IQueryHandler<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>
{
    public Result<Option<SystemSettingsForCompany>, Exception> Handle(GetSystemSettingsForCompanyQuery query) => ...;
}
```

Note that this `IQueryHandler<TQuery, TResult>` implementation defines `TQuery` as `GetSystemSettingsForCompanyQuery` and `TResult` as `Result<Option<SystemSettingsForCompany>, Exception>`.  To apply caching to the query handler implementation shown above, we define a `IQueryResultCachingStrategy<TQuery, TResult>` implementation with `TQuery` and `TResult` types that are identical to those defined for the query handler implementation, like so: 

```
// the Functional.CQS.AOP.Caching.IQueryResultCachingStrategy implementation
// it is required for caching to be applied to the query handler implementation above
public class GetSystemSettingsForCompanyQueryCachingStrategy : IQueryResultCachingStrategy<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>
{
    public string BuildCacheKeyForQuery(GetSystemSettingsForCompanyQuery query) => query.CompanyEntityID.ToString();
    public Option<string> BuildCacheGroupKeyForQuery(GetSystemSettingsForCompanyQuery query) => Option.None<string>();  
    public TimeSpan TimeToLive => TimeSpan.FromMinutes(30);
    public bool ShouldCacheResult(Result<Option<SystemSettingsForCompany>, Exception> result) => result.Match(x => x.HasValue(), _ => false);
}
```

The `IQueryResultCachingStrategy<TQuery, TResult>` contract requires the following to be implemented:
- `BuildCacheKeyForQuery` takes a query parameter object and generates a cache key based on the values within the parameter object.  The cache key should be uniquely identifiable among all possible instances of the query parameter object.
- `BuildCacheGroupKeyForQuery` takes a query parameter object and potentially generates a cache grouping key based on the values within that parameter object.  A cache grouping key is used to identify cache items that are conceptually related.  The cache grouping key is used when issuing calls to invalidate a subset of data stored in the cache.  You can return `Option.None<string>` from this method if no grouping is needed.
- `TimeToLive` defines the amount of time that query result will be stored in the cached before being automatically invalidated.
- `ShouldCacheResult` indicates if particular query results should _not_ be stored in the cache.  In the example above, we opt to not store `Option.None<string>` in the cache because the data may become available shortly after some eventual consistency mechanism completes.

Once the caching strategy implementation has been defined, [ensure that the assembly containing the strategy is fed into the CQS handler installation method](installation.md).