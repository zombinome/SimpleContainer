using System;

namespace Simple.Container
{
	public static class SimpleContainerExtensions
	{
		/// <summary>
		/// Registers new singleton dependecy
		/// </summary>
		/// <typeparam name="TObject">Dependecy object type</typeparam>
		/// <param name="container">Container to register dependency</param>
		/// <param name="instance">Dependecy object</param>
		/// <param name="name">Dependecy name</param>
		/// <returns></returns>
		public static SimpleContainer Register<TObject>(this SimpleContainer container, TObject instance, string name = null)
		{
			ILifetimeManager lifetimeManager = new SingletonLifetimeManager(instance);
			Type instanceType = instance.GetType();

			container.Register(instanceType, instanceType, lifetimeManager, name);
			return container;
		}

		/// <summary>
		/// Registers new dependecy with per-resolve lifetime
		/// </summary>
		/// <typeparam name="TMappedType">Type to which current dependency should be resolved</typeparam>
		/// <typeparam name="TActualType">Actual dependency type</typeparam>
		/// <param name="container">Container to register dependency</param>
		/// <param name="name">Depdency name</param>
		/// <returns></returns>
		public static SimpleContainer Register<TMappedType, TActualType>(this SimpleContainer container, string name = null)
		{
			ILifetimeManager lifetimeManager = new InstanceLifetimeManager(typeof(TActualType));

			container.Register(typeof (TMappedType), typeof (TActualType), lifetimeManager, name);
			return container;
		}

		public static SimpleContainer RegisterType<TDepenedcy>(this SimpleContainer container, string name = null)
		{
			ILifetimeManager lifetimeManager = new InstanceLifetimeManager(typeof(TDepenedcy));

			container.Register(typeof(TDepenedcy), typeof(TDepenedcy), lifetimeManager, name);
			return container;
		}

		public static TDependency Resolve<TDependency>(this SimpleContainer container)
		{
			return (TDependency)container.Resolve(typeof (TDependency));
		}
	}
}