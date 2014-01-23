namespace Simple.Container.Tests.Core
{
	public class TestContainer: SimpleContainer
	{
		public override SimpleContainer CreateChildContainer()
		{
			return new TestContainer { parentContainer = this };
		}
	}
}