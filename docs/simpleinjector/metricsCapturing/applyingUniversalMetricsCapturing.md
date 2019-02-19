# Applying Universal Metrics-Capturing Concerns to All CQS Handler Implementations

The universal metrics-capturing decorator requires an implementation of `IUniversalMetricsCapturingStrategy` as a dependency.  The universal metrics-capturing decorator is useful for applying consistent metrics-capturing logic across all CQS handler implementations.  For example, you may wish to log all unhandled exceptions to a third-party logging service.  Rather than duplicating that logic across individual handler-specific metrics-capturing decorators, you can define it once as a **universal metrics-capturing strategy**.  The universal metrics-capturing decorator is agnostic to input and output types for individual CQS handlers.

The universal metrics-capturing decorator works by wrapping invocations of handler implementations with the following method calls:
- `OnInvocationStart` (before invoking the handler)
- `OnInvocationCompletedSuccessfully` (after invoking the handler and having it complete successfully)
- `OnInvocationException` (after invoking the handler and having it fail due to an unhandled exception) 

An example implementation of `IUniversalMetricsCapturingStrategy` follows:

```
// note the lack of generics
// for handler-specific logic, [define a handler-specific metrics-capturing strategy](applyingMetricsCapturing.md)
public class UniversalMetricsCapturingStrategy : IUniversalMetricsCapturingStrategy
{
	public void OnInvocationStart() => Console.WriteLine("invocation started");
	public void OnInvocationCompletedSuccessfully(TimeSpan timeElapsed) => Console.WriteLine($"invocation completed successfully in {timeElapsed.TotalMilliseconds} ms" + Environment.NewLine);
	public void OnInvocationException(Exception exception, TimeSpan timeElapsed) => Console.WriteLine($"invocation aborted due to exception in {timeElapsed.TotalMilliseconds} ms" + Environment.NewLine + $"{exception}" + Environment.NewLine);
}

```

Now that the strategy has been defined, it can be supplied as part of the [installation code](installation.md).