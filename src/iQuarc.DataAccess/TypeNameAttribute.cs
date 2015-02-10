using System;

namespace iQuarc.DataAccess
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TypeNameAttribute : Attribute
	{
		public TypeNameAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}
}