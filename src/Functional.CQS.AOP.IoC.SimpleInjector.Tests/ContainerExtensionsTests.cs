using System;
using System.Collections.Generic;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using SimpleInjector;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Tests
{
	public class ContainerExtensionsTests
	{
		[Fact]
		public void ShouldSuccessfullyRegisterAllFunctionalCQSHandlersAsSingleton()
		{
			var container = new Container();
			container.RegisterAllFunctionalCQSHandlers(Lifestyle.Singleton, typeof(CommonTestInfrastructureAssemblyMarker).Assembly);
			container.Verify();

			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeSameAs(container.GetInstance(type));
		}

		[Fact]
		public void ShouldSuccessfullyRegisterAllFunctionalCQSHandlersAsTransient()
		{
			var container = new Container();
			container.RegisterAllFunctionalCQSHandlers(Lifestyle.Transient, typeof(CommonTestInfrastructureAssemblyMarker).Assembly);
			container.Verify();

			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().NotBeSameAs(container.GetInstance(type));
		}
	}
}
