using System.Linq;
using System.Reflection;

namespace Simple.Container
{
	internal static class TypeHelper
	{
		public static object CreateInstance(SimpleContainer container, ConstructorInfo constructor)
		{
			var parameters = constructor.GetParameters();
			object[] args = parameters.Select(p => container.Resolve(p.ParameterType)).ToArray();

			return constructor.Invoke(args);
		}
	}
}