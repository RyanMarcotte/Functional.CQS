# Contributing

`Functional.CQS.AOP` is intended to be application-agnostic.  As such, library enhancements and modifications must be discussed first.  Please [log an issue](https://github.com/RyanMarcotte/Functional.CQS/issues) prior to writing any code.

Feature branches are created off of `master` and merged in after code review.

All code submitted for code review must possess sufficient unit test coverage to demonstrate the correctness of the new code and demonstrate that the new code does not break existing systems.  The meaning of "sufficient" is dependent on the code being submitted for review.  When in doubt, submit a pull request.

NuGet package releases are [semantically versioned](https://semver.org/).  No automatic package deployment process exists yet.

## Structure of the Code

`Functional.CQS.AOP` is a set of .NET Standard 2.0 libraries.

Individual projects adhere to one of three naming conventions:
- if the code is agnostic to any specific DI container and is not an infrastructural concern for decoration, then prefix your project `Functional.CQS.AOP`
- if the code is specific to a DI container, then prefix your project `Functional.CQS.AOP.IoC.ContainerName`

These conventions are meant to properly document the scope of projects and effectively manage dependencies.  If you have particular questions about how to architecturally structure a new feature, discuss within the issue ticket that the feature is being driven from.
