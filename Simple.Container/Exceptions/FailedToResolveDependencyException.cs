using System;

namespace Simple.Container
{
	public class FailedToResolveDependencyException: Exception
	{
		public FailedToResolveDependencyException(Type type, string message, Exception innerException = null)
			: base(message, innerException)
		{
			this.TypeToResolve = type;
		}

		public Type TypeToResolve { get; set; }
	}
}