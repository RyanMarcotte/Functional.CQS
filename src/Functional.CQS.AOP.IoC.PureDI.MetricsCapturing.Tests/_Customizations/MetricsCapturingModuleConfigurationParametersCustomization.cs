using System;
using AutoFixture;
using Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Configuration;

namespace Functional.CQS.AOP.IoC.PureDI.MetricsCapturing.Tests._Customizations
{
    public class MetricsCapturingModuleConfigurationParametersCustomization : ICustomization
    {
		private readonly MetricsCapturingModuleConfigurationParameters _configurationParameters;

		public MetricsCapturingModuleConfigurationParametersCustomization(MetricsCapturingModuleConfigurationParameters configurationParameters)
		{
			_configurationParameters = configurationParameters ?? throw new ArgumentNullException(nameof(configurationParameters));
		}

		public void Customize(IFixture fixture)
		{
			fixture.Inject(_configurationParameters);
		}
	}
}
