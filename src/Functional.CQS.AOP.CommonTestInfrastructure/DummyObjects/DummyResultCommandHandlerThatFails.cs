﻿using System.Reflection;
using Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects.Metadata;

namespace Functional.CQS.AOP.CommonTestInfrastructure.DummyObjects
{
	/// <summary>
	/// Sample <see cref="ICommandHandler{TCommand, TError}"/> implementation.  Returns a failure result.
	/// </summary>
	public class DummyResultCommandHandlerThatFails : ICommandHandler<DummyCommand, DummyCommandError>, IProvideInformationAboutCQSHandlerDummyImplementation
	{
		private static readonly DummyCommand _command = new DummyCommand();

		/// <summary>
		/// Gets the return value for this handler.
		/// </summary>
		private static readonly Result<Unit, DummyCommandError> _result = Result.Failure<Unit, DummyCommandError>(new DummyCommandError());

		/// <summary>
		/// Handle the command.
		/// </summary>
		/// <param name="command">The command parameters.</param>
		/// <returns></returns>
		public Result<Unit, DummyCommandError> Handle(DummyCommand command)
		{
			return _result;
		}

		MethodInfo IProvideInformationAboutCQSHandlerDummyImplementation.GetHandleMethodInfo()
		{
			return GetType().GetMethod(nameof(Handle));
		}

		object[] IProvideInformationAboutCQSHandlerDummyImplementation.GetArgumentsThatWillBePassedIntoDummyImplementationHandleMethod()
		{
			return new object[] { _command };
		}

		object IProvideInformationAboutCQSHandlerDummyImplementation.GetValueThatWillBeReturnedFromDummyImplementationHandleMethod()
		{
			return _result;
		}
	}
}