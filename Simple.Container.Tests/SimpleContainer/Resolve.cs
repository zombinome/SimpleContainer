using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Simple.Container.Tests
{
	[TestClass]
	public class Resolve
	{
		[TestMethod]
		public void TestDerivedTypeResolution()
		{
			var container = new SimpleContainer();
			container.RegisterType<SimpleDependency>();

			var dependency = container.Resolve<ITestDependency>();
			Assert.IsNotNull(dependency);
			Assert.IsInstanceOfType(dependency, typeof(SimpleDependency));

			container.Release(dependency);
		}

		[TestMethod]
		public void TestResolitionWithAutowiring()
		{
			var container = new SimpleContainer();
			container.RegisterType<SimpleDependency>()
			         .RegisterType<CompositeDependecy>();

			var dependency = container.Resolve<CompositeDependecy>();

			Assert.IsNotNull(dependency);
			Assert.IsInstanceOfType(dependency, typeof(CompositeDependecy));

			Assert.IsNotNull(dependency.InnerDependency);
			Assert.IsInstanceOfType(dependency.InnerDependency, typeof(SimpleDependency));

			container.Release(dependency);

			Assert.IsNull(dependency.InnerDependency);
		}
	}
}
