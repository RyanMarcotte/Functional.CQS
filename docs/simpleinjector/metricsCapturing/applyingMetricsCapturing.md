# Applying Metrics-Capturing Concerns to Specific CQS Handler Implementations

The metrics-capturing decorators for individual CQS handler types require one of the following as dependencies:
- for query handlers, a corresponding [`IMetricsCapturingStrategyForQuery<TQuery, TResult>`](../../../src/Functional.CQS.AOP.MetricsCapturing/IMetricsCapturingStrategyForQuery.cs) implementation defined
- for command handlers, a corresponding [`IMetricsCapturingStrategyForCommand<TCommand, TError>`](../../../src/Functional.CQS.AOP.MetricsCapturing/IMetricsCapturingStrategyForCommand.cs) implementation defined

The above contracts are defined by the `Functional.CQS.AOP.MetricsCapturing` NuGet package.  **As best practice, metrics-capturing implementations should be defined near the composition root**.  In other words, metrics-capturing strategy implementations should be defined in the same assembly containing the `Functional.CQS.AOP` installer code, or in a dedicated component registration assembly.  They should _not_ be defined in the same assembly as CQS parameter objects (`TQuery`, `TCommand`) or CQS handler implementations.  This is because metrics-capturing is an infrastructural concern, and thus belongs [at the boundary of the system and not within the application domain](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/).

The metrics-capturing decorator works by wrapping invocations of handler implementations with the following method calls:
- `OnInvocationStart` (before invoking the handler)
- `OnInvocationCompletedSuccessfully` (after invoking the handler and having it complete successfully)
- `OnInvocationException` (after invoking the handler and having it fail due to an unhandled exception) 

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
// a Functional query handler implementation
// actual implementation has not been included as it is not relevant to metrics-capturing 
public class GetSystemSettingsForCompanyQueryHandler : IQueryHandler<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>
{
    public Result<Option<SystemSettingsForCompany>, Exception> Handle(GetSystemSettingsForCompanyQuery query) => ...;
}
```

Note that this `IQueryHandler<TQuery, TResult>` implementation defines `TQuery` as `GetSystemSettingsForCompanyQuery` and `TResult` as `Result<Option<SystemSettingsForCompany>, Exception>`.  To apply metrics-capturing to the query handler implementation shown above, we define an `IMetricsCapturingStrategyForQuery<TQuery, TResult>` implementation with `TQuery` and `TResult` types that are identical to those defined for the query handler implementation, like so:

```
// the Functional.CQS.AOP.MetricsCapturing.IMetricsCapturingStrategyForQuery implementation
// it is required for metrics-capturing to be applied to the query handler implementation above
public class GetSystemSettingsForCompanyMetricsCapturingStrategy : IMetricsCapturingStrategyForQuery<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>
{
    // use constructor injection to inject component dependencies required for metrics-capturing
    
    public void OnInvocationStart(GetSystemSettingsForCompanyQuery)
    {
        // perhaps post counts to Hosted Graphite so that 'number of calls' can be analyzed  
    }

    public void OnInvocationCompletedSuccessfully(GetSystemSettingsForCompanyQuery parameters, Result<Option<SystemSettingsForCompany>, Exception> result, TimeSpan timeElapsed)
    {
        // perhaps post performance timings to Hosted Graphite
        // perhaps log the occurence of a faulted Result<TSuccess, TFailure> object
    }

    public void OnInvocationException(GetSystemSettingsForCompanyQuery parameters, Exception exception, TimeSpan timeElapsed)
    {
        // perhaps post performance timings to Hosted Graphite
        // perhaps log exceptions to loggly / Serilog
    }
}
```

Once the metrics-capturing strategy implementation has been defined, [ensure that the assembly containing the strategy is fed into the CQS handler installation method](installation.md).

The process for defining metrics-capturing strategy implementations for command handlers is largely similar to the above.  Just use the `IMetricsCapturingStrategyForCommand<TCommand, TError>` contract instead.
