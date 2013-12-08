﻿using System;
using System.Linq;
using System.Reflection;

namespace Simple.Container
{
	public class InstanceLifetimeManager: ILifetimeManager
	{
		protected readonly Type type;

		protected readonly bool isDisposable;

		protected readonly ConstructorInfo constructor = null;

		public InstanceLifetimeManager(Type actualType)
		{
			this.type = actualType;
			this.isDisposable = typeof (IDisposable).IsAssignableFrom(this.type);
			this.constructor = this.type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)[0];
		}

		public InstanceLifetimeManager(ConstructorInfo constructorInfo)
		{
			this.type = constructorInfo.DeclaringType;
			this.isDisposable = typeof(IDisposable).IsAssignableFrom(this.type);
			this.constructor = constructorInfo;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion

		#region Implementation of ILifetimeManager

		public object Resolve(SimpleContainer container, out bool keepTrackObject)
		{
			keepTrackObject = this.isDisposable;
			return TypeHelper.CreateInstance(container, this.constructor);
		}

		public virtual void Release(object dependency)
		{
			if (dependency.GetType() == this.type && this.isDisposable)
			{
				((IDisposable)dependency).Dispose();
			}
		}
		#endregion
	}
}