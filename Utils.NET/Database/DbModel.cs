using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Utils.NET.Database
{
    public abstract class DbModel
    {
        private static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> propertyCache = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        private static Dictionary<string, PropertyInfo> GetProperties(Type type)
        {
            if (propertyCache.TryGetValue(type, out var properties))
                return properties;

            var fieldBaseType = typeof(DbFieldValue);
            properties = type.GetProperties()
                .Where(_ => _.GetGetMethod().ReturnType.IsSubclassOf(fieldBaseType))
                .ToDictionary(_ => _.Name);
            propertyCache.TryAdd(type, properties);
            return properties;
        }

        /// <summary>
        /// The name of the database table that this model resides in
        /// </summary>
        public abstract string tableName { get; }

        /// <summary>
        /// The field values of this model
        /// </summary>
        private readonly Dictionary<string, DbFieldValue> fieldValues = new Dictionary<string, DbFieldValue>();

        /// <summary>
        /// Gets the value of a field
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected DbFieldValue<T> GetField<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentException("PropertyName argument cannot be null");
            }

            if (!fieldValues.TryGetValue(propertyName, out var field))
            {
                field = new DbFieldValue<T>(propertyName);
                fieldValues.Add(propertyName, field);
            }

            return (DbFieldValue<T>)field;
        }

        /// <summary>
        /// Initializes all property fields
        /// </summary>
        public void InitializeAllFields()
        {
            foreach (var property in GetProperties(GetType()).Values)
            {
                property.GetValue(this);
            }
        }

        /// <summary>
        /// Returns all assigned fields within the model
        /// </summary>
        public IEnumerable<DbFieldValue> GetAllFields()
        {
            return fieldValues.Values;
        }

        /// <summary>
        /// Clones fields and values into the given object
        /// </summary>
        public void CloneFieldsInto(DbModel other)
        {
            var properties = GetProperties(GetType());
            foreach (var keyPair in fieldValues)
            {
                var property = properties[keyPair.Key];
                var fieldValue = (DbFieldValue)property.GetValue(other);
                fieldValue.Set(keyPair.Value.Get());
            }
        }

        /// <summary>
        /// Reads model data
        /// </summary>
        public void ReadFromDb(DbDataReader reader)
        {
            foreach (var field in fieldValues.Values)
            {
                field.Set(reader.GetValue(reader.GetOrdinal(field.GetFieldName())));
                field.Save();
            }
        }
    }
}
