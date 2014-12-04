using System;
using System.Data.Entity.Core;
using System.Linq;
using System.Runtime.Serialization;

namespace iQuarc.DataAccess
{
	[Serializable]
	public class ConcurrencyRepositoryViolationException : RepositoryViolationException
	{
		private const string RepositoryEntityKey = "RepositoryEntity";
		public object RepositoryEntity { get; set; }

		public ConcurrencyRepositoryViolationException()
		{
		}

		public ConcurrencyRepositoryViolationException(string errorMessage)
			: base(errorMessage)
		{
		}

		public ConcurrencyRepositoryViolationException(UpdateException exception)
			: base(exception)
		{
			this.RepositoryEntity = exception.StateEntries.FirstOrDefault();
		}

		public ConcurrencyRepositoryViolationException(string message, Exception exception)
			: base(message, exception)
		{
		}


		protected ConcurrencyRepositoryViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.RepositoryEntity = info.GetValue(RepositoryEntityKey, typeof(object));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(RepositoryEntityKey, this.RepositoryEntity, typeof(string));
		}
	}
}
