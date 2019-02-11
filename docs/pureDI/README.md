# Integrating `Functional.CQS.AOP` Into Your Application When Using [Pure DI](http://blog.ploeh.dk/2014/06/10/pure-di/) (No Dependency Injection Container)

Perhaps you have decided that [using a DI container is more trouble than it's worth for your simple application](http://blog.ploeh.dk/2012/11/06/WhentouseaDIContainer/)?

A series of NuGet packages are provided that enable manual decoration of CQS handlers, all prefixed with `Functional.CQS.AOP.IoC.PureDI`.  Those packages are listed below.  As a matter of best practice, all `Functional.CQS.AOP.IoC.PureDI`-prefixed packages should only be installed in your application's [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/), as the decorators are an infrastructure concern and not a business domain concern.
- `Functional.CQs.AOP.IoC.PureDI.MetricsCapturing` (decorators for posting performance metrics and unhandled errors)

Decorators are applied to CQS handlers by fulfilling all decorator dependencies manually.  First, define a query parameter object and associated query handler, like the following:

```
// a Functional.CQS query parameter object
// assume SystemSettingsForCompany is a POCO (plain old C# object)
// query returns Option<SystemSettingsForCompany> in success case because it is possible that a company ID is invalid and no result would be returned in that case
// constructor boilerplate has been left out for brevity
public class GetSystemSettingsForCompanyQuery : IQueryParameters<Result<Option<SystemSettingsForCompany>, Exception>>
{
    public int CompanyID { get; }
}
```

```
// a Functional query handler implementation
// actual implementation has not been included as it is not relevant to the example
public class GetSystemSettingsForCompanyQueryHandler : IQueryHandler<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>
{
    public Result<Option<SystemSettingsForCompany>, Exception> Handle(GetSystemSettingsForCompanyQuery query) => ...;
}
```

Then, in your application's startup code (i.e. composition root), instantiate all your components via `new`:

```
// instantiate handler and all decorator dependencies
var queryHandler = new GetSystemSettingsForCompanyQueryHandler();
var metricsCapturingStrategy = new GetSystemSettingsForCompanyQueryMetricsCapturingStrategy();

// decorate the original query handler
var decoratedQueryHandler = new QueryHandlerMetricsCapturingDecorator<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>(
    queryHandler,
    metricsCapturingStrategy);
```

The `decoratedQueryHandler` is then supplied to any other objects requiring a `IQueryHandler<GetSystemSettingsForCompanyQuery, Result<Option<SystemSettingsForCompany>, Exception>>` dependency.