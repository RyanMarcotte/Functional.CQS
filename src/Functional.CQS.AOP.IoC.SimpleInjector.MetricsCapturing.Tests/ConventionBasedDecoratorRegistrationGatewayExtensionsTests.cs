using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Configuration;
using SimpleInjector;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Tests
{
	public class ConventionBasedDecoratorRegistrationGatewayExtensionsTests
	{
		[Theory, SingletonRegistrationWithNoDecorationsEnabledArrangement]
		public void ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled_SingletonRegistration(Container container) => ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled(container);
		
		[Theory, TransientRegistrationWithNoDecorationsEnabledArrangement]
		public void ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled_TransientRegistration(Container container) => ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled(container);
		
		private static void ShouldNotApplyDecoratorIfConfigurationHasAllDecorationsDisabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.ImplementationTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasCommandAndQuerySpecificDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasCommandAndQuerySpecificDecorationsEnabled(container);

		[Theory, TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasCommandAndQuerySpecificDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasCommandAndQuerySpecificDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasCommandAndQuerySpecificDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.MetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithUniversalDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(container);

		[Theory, TransientRegistrationWithUniversalDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithAllDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(container);

		[Theory, TransientRegistrationWithAllDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		#region Arrangements

		private abstract class ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase : AutoDataAttribute
		{
			protected ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration)
				: base(() => new Fixture().Customize(new ContainerCustomization(lifestyle, configuration)))
			{

			}
		}

		private class SingletonRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(false, false, false))
			{
			}
		}

		private class TransientRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(false, false, false))
			{
			}
		}

		private class SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(false, true, true))
			{
			}
		}

		private class TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(false, true, true))
			{
			}
		}

		private class SingletonRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, false, false))
			{
			}
		}

		private class TransientRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, false, false))
			{
			}
		}

		private class SingletonRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, true, true))
			{
			}
		}

		private class TransientRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, true, true))
			{
			}
		}

		#endregion

		#region Customizations

		private class ContainerCustomization : ICustomization
		{
			private readonly MetricsCapturingModuleConfigurationParameters _configuration;
			private readonly Lifestyle _lifestyle;

			public ContainerCustomization(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration)
			{
				_lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
				_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			}

			public void Customize(IFixture fixture)
			{
				var container = new Container();
				container.RegisterAllFunctionalCQSHandlers(_lifestyle, typeof(CommonTestInfrastructureAssemblyMarker).Assembly)
					.WithMetricsCapturingDecorator(_configuration);

				fixture.Inject(container);
			}
		}

		#endregion
	}
}
