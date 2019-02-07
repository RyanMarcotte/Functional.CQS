namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// 
	/// </summary>
	public class DummyCommandThatSucceeds : ICommandParameters<DummyCommandError>
	{

	}

	/// <summary>
	/// 
	/// </summary>
	public class DummyCommandThatFails : ICommandParameters<DummyCommandError>
	{

	}
}