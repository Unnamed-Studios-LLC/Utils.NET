using System;
using System.Data;
using System.Text;

namespace Utils.NET.Database.Queries.Methods
{
    internal class SelectMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        private Func<T, DbFieldValue[]> fieldGetter;

        public SelectMethod(Func<T, DbFieldValue[]> fieldGetter = null)
        {
            this.fieldGetter = fieldGetter;
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            AddMethod(builder);
            AddFields(ref model, builder);
            AddFrom(model, builder);
        }

        /// <summary>
        /// Adds the select method to the command text
        /// </summary>
        /// <param name="builder"></param>
        private void AddMethod(StringBuilder builder)
        {
            builder.Append("SELECT");
        }

        /// <summary>
        /// Gets selected fields from the model using a specified getter function
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DbFieldValue[] GetFields(T model)
        {
            return fieldGetter?.Invoke(model);
        }

        /// <summary>
        /// Adds fields parameters to the select command
        /// </summary>
        /// <param name="model"></param>
        /// <param name="command"></param>
        private void AddFields(ref T model, StringBuilder command)
        {
            model = new T();
            var fields = GetFields(model);
            if (fields == null || fields.Length == 0)
            {
                // get all fields
                command.Append(" *");
                model.InitializeAllFields();
                return;
            }

            bool first = true;
            foreach (var field in fields)
            {
                command.Append(first ? " " : ", ");
                first = false;
                command.Append(field.GetFieldName());
            }
        }

        /// <summary>
        /// Adds the FROM database parameter to the select command
        /// </summary>
        /// <param name="command"></param>
        private void AddFrom(T model, StringBuilder command)
        {
            command.Append(" FROM ");
            command.Append(model.tableName);
        }
    }
}
