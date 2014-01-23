using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Simple.Container.Tests
{
	[TestClass]
	public class SingletonLifetimeManager
	{
		[TestMethod]
		public void TestLifetimeManagerRegisterResolve()
		{
			var dependency = new SimpleDependency();

			var lifetimeManager = new Container.SingletonLifetimeManager(dependency);

			bool keepTrackObject;
			var resolvedObject1 = lifetimeManager.Resolve(null, out keepTrackObject);
			Assert.IsNotNull(resolvedObject1);

			var resolvedObject2 = lifetimeManager.Resolve(null, out keepTrackObject);
			Assert.IsNotNull(resolvedObject2);

			Assert.AreSame(resolvedObject1, resolvedObject2);
			Assert.AreEqual(SimpleDependency.ActiveDependecies, 1);

			dependency.Dispose();
		}

		[TestMethod]
		public void TestContainerResolveSingleton()
		{
			var container = new SimpleContainer();

			string str = "TEST";

			container.Register(str);

			string result1 = container.Resolve<string>();
			string result2 = container.Resolve<string>();

			Assert.AreSame(result1, result2);
			Assert.AreSame(result1, str);
		}
	}
}