using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.UnitTests
{
    [TestClass]
    public class DataValidationExceptionTests
    {
        [TestMethod]
        public void Serialize_MoreEntitiesWithMoreErrors_DeserializedIsSame()
        {
            var errors1 = new[]
            {
                new ValidationError("UserName", "error1"),
                new ValidationError("UserEmail", "error2")
            };
            var entityErrors1 = new DataValidationResult(new EntryDouble(new User(1)), errors1);
            var errors2 = new[]
            {
                new ValidationError("RoleName", "error3"),
                new ValidationError("AccessRights", "error4")
            };
            var entityErrors2 = new DataValidationResult(new EntryDouble(new Role(2)), errors2);

            DataValidationException e = new DataValidationException(string.Empty, new[] {entityErrors1, entityErrors2});

            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, e);
                s.Position = 0; // Reset stream position
                e = (DataValidationException) formatter.Deserialize(s);
            }

            DataValidationResult[] actual = e.ValidationErrors.ToArray();
            AssertAreEqual(entityErrors1, actual[0]);
            AssertAreEqual(entityErrors2, actual[1]);
        }

        private void AssertAreEqual(DataValidationResult expected, DataValidationResult actual)
        {
            Func<ValidationError, ValidationError, bool> equalityFunc = (e1, e2) => e1.ErrorMessage == e2.ErrorMessage && e1.PropertyName == e2.PropertyName;

            Assert.AreEqual(expected.Entry, actual.Entry);
            AssertEx.AreEquivalent(actual.Errors, equalityFunc, expected.Errors.ToArray());
        }

        [Serializable]
        private class EntryDouble : IEntityEntry
        {
            public EntryDouble(object entity)
            {
                Entity = entity;
            }

            public object Entity { get; set; }

            public EntityEntryState State
            {
                get { throw new NotImplementedException(); }
            }

            public object GetOriginalValue(string propertyName)
            {
                throw new NotImplementedException();
            }

            public IEntityEntry<T> Convert<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public void SetOriginalValue(string propertyName, object value)
            {
                throw new NotImplementedException();
            }

            protected bool Equals(EntryDouble other)
            {
                return Equals(Entity, other.Entity);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((EntryDouble) obj);
            }

            public override int GetHashCode()
            {
                return (Entity != null ? Entity.GetHashCode() : 0);
            }
        }

        [Serializable]
        private class User
        {
            public User(int id)
            {
                Id = id;
            }

            public int Id { get; set; }

            protected bool Equals(User other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((User) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        [Serializable]
        private class Role
        {
            public Role(int id)
            {
                Id = id;
            }

            public int Id { get; set; }

            protected bool Equals(Role other)
            {
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Role) obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }
    }
}