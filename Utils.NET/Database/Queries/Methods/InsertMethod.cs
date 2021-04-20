using System;
using System.Data;
using System.Text;

namespace Utils.NET.Database.Queries.Methods
{
    public class InsertMethod<T> : IQueryMethod<T> where T : DbModel, new()
    {
        private T model;

        public InsertMethod(T model)
        {
            this.model = model;
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            model = this.model;
            AddMethod(ref model, builder);
            AddFields(ref model, builder);
            AddValues(ref model, builder, command);
        }

        private void AddMethod(ref T model, StringBuilder builder)
        {
            builder.Append("INSERT INTO ");
            builder.Append(model.tableName);
        }

        private void AddFields(ref T model, StringBuilder builder)
        {
            builder.Append(" (");
            bool first = true;
            foreach (var field in model.GetAllFields())
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;
                builder.Append(field.GetFieldName());
            }
            builder.Append(")");
        }

        private void AddValues(ref T model, StringBuilder builder, IDbCommand command)
        {
            builder.Append(" VALUES(");
            bool first = true;
            foreach (var field in model.GetAllFields())
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                first = false;
                builder.Append('?');
                builder.Append(field.GetFieldName());

                var parameter = command.CreateParameter();
                parameter.ParameterName = $"?{field.GetFieldName()}";
                parameter.Value = field.Get();
                command.Parameters.Add(parameter);
            }
            builder.Append(")");
        }
    }
}
