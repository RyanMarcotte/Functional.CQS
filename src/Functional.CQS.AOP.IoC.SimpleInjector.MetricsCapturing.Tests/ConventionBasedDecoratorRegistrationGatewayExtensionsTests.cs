using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Configuration;
using SimpleInjector;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Tests
{
	public class ConventionBasedDecoratorRegistrationGatewayExtensionsTests
	{
		[Fact]
		public void ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled()
		{
			var container = new Container();
			container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(CommonTestInfrastructureAssemblyMarker).Assembly)
				.WithMetricsCapturingDecorator(new MetricsCapturingModuleConfigurationParameters(false, false, false));

			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.ImplementationTypeLookupByCQSHandlerContractType[type]);
		}
	}
}
