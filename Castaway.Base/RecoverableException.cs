using System;
using System.Runtime.Serialization;

namespace Castaway.Base;

public class RecoverableException : Exception
{
	public RecoverableException()
	{
	}

	protected RecoverableException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}

	public RecoverableException(string? message) : base(message)
	{
	}

	public RecoverableException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}