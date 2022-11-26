using System;
using Serilog;

namespace Castaway.Base;

public abstract class MessageException : Exception
{
	public readonly bool IsFatal;

	protected MessageException(string? message, bool isFatal) : base(message)
	{
		IsFatal = isFatal;
	}

	protected MessageException(string? message, Exception? innerException, bool isFatal) : base(message,
		innerException)
	{
		IsFatal = isFatal;
	}

	public abstract void Log(ILogger logger);
	public abstract void Repair(ILogger logger);
}