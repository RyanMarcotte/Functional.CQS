﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.MetricsCapturing;
using Functional.CQS.AOP.IoC.SimpleInjector.MetricsCapturing.Configuration;
using Functional.CQS.AOP.MetricsCapturing;
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

		[Theory, SingletonRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldNotApplyAnyDecoratorsIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist_SingletonRegistration(Container container) => ShouldNotApplyAnyDecoratorsIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(container);

		[Theory, TransientRegistrationWithAllDecorationsEnabledButNoStrategyImplementationsArrangement]
		public void ShouldNotApplyAnyDecoratorsIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist_TransientRegistration(Container container) => ShouldNotApplyAnyDecoratorsIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(container);

		private static void ShouldNotApplyAnyDecoratorsIfConfigurationHasAllDecorationsEnabledButNoStrategyImplementationsExist(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.ImplementationTypeLookupByCQSHandlerContractType[type]);
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

		private abstract class ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined : AutoDataAttribute
		{
			protected ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
				: base(() => new Fixture().Customize(new ContainerWithUniversalMetricsCapturingStrategyDefinedCustomization(lifestyle, configuration, assemblyCollection)))
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

		private class SingletonRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined
		{
			public SingletonRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithUniversalDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined
		{
			public TransientRegistrationWithUniversalDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, false, false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined
		{
			public SingletonRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithAllDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBaseWithUniversalMetricsCapturingStrategyDefined
		{
			public TransientRegistrationWithAllDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new MetricsCapturingModuleConfigurationParameters(true, true, true), _assemblyCollectionWithHandlersAndStrategies)
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

		private class ContainerWithUniversalMetricsCapturingStrategyDefinedCustomization : ICustomization
		{
			private readonly Lifestyle _lifestyle;
			private readonly Assembly[] _assemblyCollection;
			private readonly MetricsCapturingModuleConfigurationParameters _configuration;

			public ContainerWithUniversalMetricsCapturingStrategyDefinedCustomization(Lifestyle lifestyle, MetricsCapturingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
			{
				_lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
				_assemblyCollection = assemblyCollection ?? throw new ArgumentNullException(nameof(assemblyCollection));
				_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			}

			public void Customize(IFixture fixture)
			{
				var container = new Container();
				container.RegisterAllFunctionalCQSHandlers(_lifestyle, _assemblyCollection)
					.WithMetricsCapturingDecorator<UniversalMetricsCapturingStrategy>(_configuration);

				fixture.Inject(container);
			}
		}

		#endregion

		#region Mocks

		// ReSharper disable once ClassNeverInstantiated.Local
		private class UniversalMetricsCapturingStrategy : IUniversalMetricsCapturingStrategy
		{
			public void OnInvocationStart() { }
			public void OnInvocationCompletedSuccessfully(TimeSpan timeElapsed) { }
			public void OnInvocationException(Exception exception, TimeSpan timeElapsed) { }
		}

		#endregion
	}
}
