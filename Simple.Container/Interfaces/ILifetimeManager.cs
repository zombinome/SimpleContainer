using System;

namespace Simple.Container
{
	/// <summary>
	/// Lifetime manager interface
	/// </summary>
	public interface ILifetimeManager: IDisposable
	{
		/// <summary>
		/// Resolves dependency
		/// </summary>
		/// <param name="container">IoC container, used to resolve autowiring</param>
		/// <param name="keepTrackObject">if true, container should keep track over dependecy lifetime, and release them before its own destruction</param>
		/// <returns>Resolved dependency</returns>
		object Resolve(SimpleContainer container, out bool keepTrackObject);

		/// <summary>
		/// Releases dependency, performing all needed internal work
		/// </summary>
		/// <param name="dependency">Dependecy to release</param>
		void Release(object dependency);
	}
}