using System;

namespace Simple.Container.Tests
{
	public class CompositeDependecy: IDisposable
	{
		public CompositeDependecy(ITestDependency internalDependecy)
		{
			this.InnerDependency = internalDependecy;
		}

		public ITestDependency InnerDependency { get; private set; }

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (this.InnerDependency != null)
			{
				var dep = this.InnerDependency as IDisposable;
				if (dep != null)
				{
					dep.Dispose();
				}

				this.InnerDependency = null;
			}
		}

		#endregion
	}
}