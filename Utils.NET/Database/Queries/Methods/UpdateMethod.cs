using System;
using System.Data;
using System.Text;

namespace Utils.NET.Database.Queries.Methods
{
    internal class UpdateMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        private T model;

        public UpdateMethod(T model)
        {
            this.model = model;
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            model = this.model;
            AddMethod(builder);
            AddFields(ref model, builder, command);
        }

        /// <summary>
        /// Adds the update method to the command text
        /// </summary>
        /// <param name="builder"></param>
        private void AddMethod(StringBuilder builder)
        {
            builder.Append("UPDATE ");
            builder.Append(model.tableName);
        }

        /// <summary>
        /// Adds fields parameters to the select command
        /// </summary>
        /// <param name="model"></param>
        /// <param name="builder"></param>
        private void AddFields(ref T model, StringBuilder builder, IDbCommand command)
        {
            builder.Append(" SET ");

            bool first = true;
            foreach (var field in model.GetAllFields())
            {
                if (!field.IsUpdated()) continue; // ignore non updated fields
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;
                var fieldName = field.GetFieldName();
                builder.Append(fieldName);
                builder.Append("=?");
                builder.Append(fieldName);

                var parameter = command.CreateParameter();
                parameter.ParameterName = $"?{fieldName}";
                parameter.Value = field.Get();
                command.Parameters.Add(parameter);
            }
        }
    }
}
