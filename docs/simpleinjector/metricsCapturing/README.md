# Adding Metrics and Exception Logging With `Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing`

`Functional.CQS.AOP.IoC.PureDI.MetricsCapturing` supplies metrics-capturing decorator implementations, while `Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing` supplies a set of extension methods that make it easy to register those decorators with the `SimpleInjector` container.

Metrics-capturing decorators can be applied to any `Functional.CQS` handler implementation.

Consult the following documentation for usage instructions and suggestions for best practice:
- [Installation](installation.md)
- [Applying Metrics-Capturing Concerns to Specific CQS Handler Implementations](applyingMetricsCapturing.md)
- [Applying Universal Metrics-Capturing Concerns to All CQS Handler Implementations](applyingUniversalMetricsCapturing.md)