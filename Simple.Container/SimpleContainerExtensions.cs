using System;

namespace Simple.Container
{
	public static class SimpleContainerExtensions
	{
		public static SimpleContainer Register<TObject>(this SimpleContainer container, TObject instance, string name = null)
		{
			ILifetimeManager lifetimeManager = new SingletonLifetimeManager(instance);
			Type instanceType = instance.GetType();

			container.Register(instanceType, instanceType, lifetimeManager, name);
			return container;
		}

		public static SimpleContainer Register<TMappedType, TActualType>(this SimpleContainer container, string name = null)
		{
			ILifetimeManager lifetimeManager = new InstanceLifetimeManager(typeof(TActualType));

			container.Register(typeof (TMappedType), typeof (TActualType), lifetimeManager, name);
			return container;
		}
	}
}