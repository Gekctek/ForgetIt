using System.Reflection;

namespace ForgetIt.Core
{
	public class ReflectionUtil
	{
		public static Dictionary<string, PropertyInfo> GetClassProperties<T>()
		{
			return GetClassProperties(typeof(T));
		}

		public static Dictionary<string, PropertyInfo> GetClassProperties(Type type)
		{
			return type
			   .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
			   .ToDictionary(p => p.Name, p => p);
		}
	}
}