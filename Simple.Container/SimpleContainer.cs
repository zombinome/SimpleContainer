using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Simple.Container
{
	/// <summary>
	/// Simpple IoC container
	/// </summary>
	public class SimpleContainer: IDisposable
	{
		/// <summary>
		/// List of dependecy resolution rules for types
		/// </summary>
		protected Dictionary<Type, Dictionary<string, ILifetimeManager>> registeredLifetimeManagers;

		/// <summary>
		/// List of dependecies that are needed to be tracked to invoke Release method on container dispose
		/// </summary>
		protected Dictionary<object, ILifetimeManager> dependenciesToRelease;

		/// <summary>
		/// Reference to parent container
		/// </summary>
		protected SimpleContainer parentContainer;

		public SimpleContainer()
		{
			this.registeredLifetimeManagers = new Dictionary<Type, Dictionary<string, ILifetimeManager>>();
			this.dependenciesToRelease = new Dictionary<object, ILifetimeManager>();

			this.parentContainer = null;
		}

		protected SimpleContainer(SimpleContainer parent)
			: this()
		{
			this.parentContainer = parent;
		}

		/// <summary>
		/// Registers new dependecy resolution rule
		/// </summary>
		/// <param name="mappedType">Type, which would be requested by resolution</param>
		/// <param name="actualType">Actual type of dependecy</param>
		/// <param name="lifetimeManager">Lifetime manager</param>
		/// <param name="name">Rule name, if needed</param>
		public void Register(Type mappedType, Type actualType, ILifetimeManager lifetimeManager = null, string name = "")
		{
			if (lifetimeManager == null)
			{
				lifetimeManager = new InstanceLifetimeManager(actualType);
			}

			name = name ?? string.Empty;
			if (!this.registeredLifetimeManagers.ContainsKey(mappedType))
			{
				this.registeredLifetimeManagers[mappedType] = new Dictionary<string, ILifetimeManager> { {name, lifetimeManager } };
			}
			else
			{
				this.registeredLifetimeManagers[mappedType][name] = lifetimeManager;
			}
		}

		/// <summary>
		/// Resolves dependency
		/// </summary>
		/// <param name="dependecyType">Dependecy type</param>
		/// <param name="name">Dependecy resolution name, if provided</param>
		/// <returns></returns>
		public object Resolve(Type dependecyType, string name = null)
		{
			try
			{
				object result;
				if (ResolveExact(this, this.dependenciesToRelease, dependecyType, name, out result))
				{
					return result;
				}

				if (ResolveDerived(this, this.dependenciesToRelease, dependecyType, name, out result))
				{
					return result;
				}

				if (this.parentContainer != null)
				{
					if (ResolveExact(this.parentContainer, this.dependenciesToRelease, dependecyType, name, out result))
					{
						return result;
					}

					if (ResolveDerived(this.parentContainer, this.parentContainer.dependenciesToRelease, dependecyType, name, out result))
					{
						return result;
					}
				}

				ConstructorInfo[] constructorInfos = dependecyType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
				var ci = constructorInfos[0];
				return TypeHelper.CreateInstance(this, ci);
			}
			catch (Exception ex)
			{
				throw new FailedToResolveDependencyException(dependecyType, "Error occured during dependecy resolution.", ex);
			}
		}

		/// <summary>
		/// Release resolved dependency
		/// </summary>
		/// <param name="dependency">Dependency to be released</param>
		public void Release(object dependency)
		{
			if (dependenciesToRelease.ContainsKey(dependency))
			{
				ILifetimeManager lifetimeManager = dependenciesToRelease[dependency];
				dependenciesToRelease.Remove(dependency);

				lifetimeManager.Release(dependency);
			}
		}

		/// <summary>
		/// Creates child IoC container. Child container inherits all parent rules, but all object resolved through child container should
		/// </summary>
		/// <returns></returns>
		public virtual SimpleContainer CreateChildContainer()
		{
			return new SimpleContainer(this);
		}
		
		/// <summary>
		/// Implementation of <see cref="IDisposable"/> interface
		/// </summary>
		public virtual void Dispose()
		{
			if (this.dependenciesToRelease != null)
			{
				foreach (var dependency in this.dependenciesToRelease)
				{
					var ltm = dependency.Value;
					ltm.Release(dependency.Key);
				}

				this.dependenciesToRelease = null;
			}

			if (this.registeredLifetimeManagers != null)
			{
				foreach (var dependencyLifetimeManagers in registeredLifetimeManagers)
				{
					foreach (var lifetimeManager in dependencyLifetimeManagers.Value)
					{
						lifetimeManager.Value.Dispose();
					}
				}

				this.registeredLifetimeManagers = null;
			}
		}

		protected static bool ResolveExact(
			SimpleContainer container,
			//Dictionary<Type, Dictionary<string, ILifetimeManager>> registeredLifetimeManagers, 
			Dictionary<object, ILifetimeManager> dependenciesToRelease, 
			Type dependencyType, 
			string name, 
			out object result)
		{
			result = null;

			var registeredLifetimeManagers = container.registeredLifetimeManagers;

			// trying exact match
			if (registeredLifetimeManagers.ContainsKey(dependencyType))
			{
				ILifetimeManager lifetimeManager = null;

				Dictionary<string, ILifetimeManager> registeredLifetimes = registeredLifetimeManagers[dependencyType];
				if (name == null)
				{
					lifetimeManager = registeredLifetimes.Values.First();
				}
				else if (registeredLifetimes.ContainsKey(name))
				{
					lifetimeManager = registeredLifetimes[name];
				}

				if (lifetimeManager != null)
				{
					bool keepTrack;
					result = lifetimeManager.Resolve(container, out keepTrack);
					if (keepTrack)
					{
						dependenciesToRelease[result] = lifetimeManager;
					}

					return true;
				}
			}

			return false;
		}

		protected static bool ResolveDerived(
			SimpleContainer container,
			//Dictionary<Type, Dictionary<string, ILifetimeManager>> registeredLifetimeManagers,
			Dictionary<object, ILifetimeManager> dependenciesToRelease,
			Type dependencyType,
			string name,
			out object result)
		{
			result = null;
			var registeredLifetimeManagers = container.registeredLifetimeManagers;

			// trying to resolve nearest derived type
			foreach (var registeredLifetimeManager in registeredLifetimeManagers)
			{
				Type registeredType = registeredLifetimeManager.Key;
				if (dependencyType.IsAssignableFrom(registeredType))
				{
					return ResolveExact(container, dependenciesToRelease, registeredType, name, out result);
				}
			}

			return false;
		}
	}
}