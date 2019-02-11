using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing;
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
				container.GetInstance(type).Should().BeOfType(MetricsCapturingTestUtility.MetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithUniversalDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(container);

		[Theory, TransientRegistrationWithUniversalDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasUniversalDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(MetricsCapturingTestUtility.UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithAllDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(container);

		[Theory, TransientRegistrationWithAllDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasAllDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(MetricsCapturingTestUtility.UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldNotApplyDecoratorWhenNoStrategyImplementationExists_SingletonRegistration(Container container) => ShouldNotApplyDecoratorWhenNoStrategyImplementationExists(container);

		[Theory, TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldNotApplyDecoratorWhenNoStrategyImplementationExists_TransientRegistration(Container container) => ShouldNotApplyDecoratorWhenNoStrategyImplementationExists(container);

		private static void ShouldNotApplyDecoratorWhenNoStrategyImplementationExists(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.ImplementationTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldApplyUniversalDecoratorIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist_SingletonRegistration(Container container) => ShouldApplyUniversalDecoratorIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(container);

		[Theory, TransientRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldApplyUniversalDecoratorIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist_TransientRegistration(Container container) => ShouldApplyUniversalDecoratorIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(container);

		private static void ShouldApplyUniversalDecoratorIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(MetricsCapturingTestUtility.UniversalMetricsCapturingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		#region Arrangements

		private static readonly Assembly[] _assemblyCollectionWithHandlersOnly = { typeof(CommonTestInfrastructureAssemblyMarker).Assembly };
		private static readonly Assembly[] _assemblyCollectionWithHandlersAndStrategies = { typeof(CommonTestInfrastructureAssemblyMarker).Assembly, typeof(CommonTestInfrastructureMetricsCapturingAssemblyMarker).Assembly };

		private abstract class ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase : AutoDataAttribute
		{
			protected ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
				: base(() => new Fixture().Customize(new ContainerCustomization(lifestyle, configuration, assemblyCollection)))
			{

			}
		}

		private class SingletonRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(false, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(false, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(false, true, true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(false, true, true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(false, true, true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		private class TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithCommandAndQuerySpecificDecorationsEnabledButNoStrategyImplementationsArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(false, true, true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		private class SingletonRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		private class TransientRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		#endregion

		#region Customizations

		private class ContainerCustomization : ICustomization
		{
			private readonly Lifestyle _lifestyle;
			private readonly Assembly[] _assemblyCollection;
			private readonly MetricsCapturingModuleConfigurationParameters _configuration;

			public ContainerCustomization(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
			{
				_lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
				_assemblyCollection = assemblyCollection ?? throw new ArgumentNullException(nameof(assemblyCollection));
				_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			}

			public void Customize(IFixture fixture)
			{
				var container = new Container();
				container.RegisterAllFunctionalCQSHandlers(_lifestyle, _assemblyCollection)
					.WithMetricsCapturingDecorator(_configuration);

				fixture.Inject(container);
			}
		}

		#endregion
	}
}
