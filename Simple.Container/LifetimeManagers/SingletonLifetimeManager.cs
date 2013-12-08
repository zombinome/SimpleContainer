namespace Simple.Container
{
	public class SingletonLifetimeManager: ILifetimeManager
	{
		private object instance;

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

		public object Resolve(out bool keepTrackObject)
		{
			keepTrackObject = false;
			return this.instance;
		}

		public void Release(object dependency)
		{
		}

		/// <summary>
		/// Owner of this lifetime manager
		/// </summary>
		public SimpleContainer Container { get; internal set; }

		#endregion
	}
}