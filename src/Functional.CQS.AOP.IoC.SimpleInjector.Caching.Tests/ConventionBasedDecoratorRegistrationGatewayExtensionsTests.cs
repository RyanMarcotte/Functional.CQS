using System;
using System.Collections.Generic;
using System.Reflection;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Functional.CQS.AOP.Caching.Infrastructure.MemoryCache;
using Functional.CQS.AOP.CommonTestInfrastructure;
using Functional.CQS.AOP.CommonTestInfrastructure.Caching;
using Functional.CQS.AOP.IoC.SimpleInjector.Caching.Configuration;
using SimpleInjector;
using Xunit;

namespace Functional.CQS.AOP.IoC.SimpleInjector.Caching.Tests
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

		[Theory, SingletonRegistrationWithDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasDecorationsEnabled_SingletonRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasDecorationsEnabled(container);

		[Theory, TransientRegistrationWithDecorationsEnabledArrangement]
		public void ShouldApplyDecoratorIfConfigurationHasDecorationsEnabled_TransientRegistration(Container container) => ShouldApplyDecoratorIfConfigurationHasDecorationsEnabled(container);

		private static void ShouldApplyDecoratorIfConfigurationHasDecorationsEnabled(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(CachingTestUtility.CachingDecoratorTypeLookupByCQSHandlerContractType[type]);
		}

		[Theory, SingletonRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement]
		public void ShouldNotApplyDecoratorIfConfigurationHasDecorationsEnabledButNoStrategyImplementationsExist_SingletonRegistration(Container container) => ShouldNotApplyDecoratorIfConfigurationHasDecorationsEnabledButNoStrategyImplementationsExist(container);

		[Theory, TransientRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement]
		public void ShouldNotApplyDecoratorIfConfigurationHasDecorationsEnabledButNoStrategyImplementationsExist_TransientRegistration(Container container) => ShouldNotApplyDecoratorIfConfigurationHasDecorationsEnabledButNoStrategyImplementationsExist(container);

		private static void ShouldNotApplyDecoratorIfConfigurationHasDecorationsEnabledButNoStrategyImplementationsExist(Container container)
		{
			foreach (var type in TestUtility.CQSHandlerContractTypes)
				container.GetInstance(type).Should().BeOfType(TestUtility.ImplementationTypeLookupByCQSHandlerContractType[type]);
		}

		#region Arrangements

		private static readonly Assembly[] _assemblyCollectionWithHandlersOnly = { typeof(CommonTestInfrastructureAssemblyMarker).Assembly };
		private static readonly Assembly[] _assemblyCollectionWithHandlersAndStrategies = { typeof(CommonTestInfrastructureAssemblyMarker).Assembly, typeof(CommonTestInfrastructureCachingAssemblyMarker).Assembly };

		private abstract class ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase : AutoDataAttribute
		{
			protected ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase(Lifestyle lifestyle, CachingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
				: base(() => new Fixture().Customize(new ContainerCustomization(lifestyle, configuration, assemblyCollection)))
			{

			}
		}

		private class SingletonRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new CachingModuleConfigurationParameters(false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithNoDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithNoDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new CachingModuleConfigurationParameters(false), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithDecorationsEnabledArrangement()
				: base(Lifestyle.Singleton, new CachingModuleConfigurationParameters(true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class TransientRegistrationWithDecorationsEnabledArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithDecorationsEnabledArrangement()
				: base(Lifestyle.Transient, new CachingModuleConfigurationParameters(true), _assemblyCollectionWithHandlersAndStrategies)
			{
			}
		}

		private class SingletonRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public SingletonRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement()
				: base(Lifestyle.Singleton, new CachingModuleConfigurationParameters(true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		private class TransientRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement : ConventionBasedDecoratorRegistrationGatewayExtensionsTestsArrangementBase
		{
			public TransientRegistrationWithDecorationsEnabledButNoStrategiesDefinedArrangement()
				: base(Lifestyle.Transient, new CachingModuleConfigurationParameters(true), _assemblyCollectionWithHandlersOnly)
			{
			}
		}

		#endregion

		#region Customizations

		private class ContainerCustomization : ICustomization
		{
			private readonly Lifestyle _lifestyle;
			private readonly Assembly[] _assemblyCollection;
			private readonly CachingModuleConfigurationParameters _configuration;

			public ContainerCustomization(Lifestyle lifestyle, CachingModuleConfigurationParameters configuration, params Assembly[] assemblyCollection)
			{
				_lifestyle = lifestyle ?? throw new ArgumentNullException(nameof(lifestyle));
				_assemblyCollection = assemblyCollection ?? throw new ArgumentNullException(nameof(assemblyCollection));
				_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			}

			public void Customize(IFixture fixture)
			{
				var container = new Container();
				container.RegisterAllFunctionalCQSHandlers(_lifestyle, _assemblyCollection)
					.WithCachingDecorator(() => new FunctionalMemoryCache(), _configuration);

				fixture.Inject(container);
			}
		}

		#endregion
	}
}
