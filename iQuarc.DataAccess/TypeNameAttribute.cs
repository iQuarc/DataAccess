using System;

namespace iQuarc.Finance.DataAccess
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