namespace Simple.Container
{
	public class SingletonLifetimeManager: ILifetimeManager
	{
		protected object instance;

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			instance = null;
		}

		#endregion

		#region Implementation of ILifetimeManager

		public SingletonLifetimeManager(object instanceToKeep)
		{
			this.instance = instanceToKeep;
		}

		/// <summary>
		/// Resolves dependency
		/// </summary>
		/// <param name="container">IoC container, used to resolve autowiring</param>
		/// <param name="keepTrackObject">if true, container should keep track over dependecy lifetime, and release them before its own destruction</param>
		/// <returns>Resolved dependency</returns>
		public object Resolve(SimpleContainer container, out bool keepTrackObject)
		{
			keepTrackObject = false;
			return this.instance;
		}

		public void Release(object dependency)
		{
		}

		#endregion
	}
}