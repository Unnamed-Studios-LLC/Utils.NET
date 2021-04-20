using System;
using System.Data;
using System.Text;

namespace Utils.NET.Database.Queries.Conditions
{
    internal class WhereCondition<T> : IQueryCondition<T> where T : DbModel
    {
        /// <summary>
        /// The function to specify which fields to check
        /// </summary>
        private Func<T, Condition[]> conditionsGetter;

        public WhereCondition(Func<T, Condition[]> conditionsGetter)
        {
            this.conditionsGetter = conditionsGetter ?? throw new ArgumentNullException("conditionsGetter");
        }

        public void AppendToQuery(ref T model, StringBuilder builder, IDbCommand command)
        {
            builder.Append(" WHERE ");

            AddFields(ref model, builder, command);
        }

        /// <summary>
        /// Adds the condition fields to the command
        /// </summary>
        /// <param name="model"></param>
        /// <param name="builder"></param>
        /// <param name="command"></param>
        private void AddFields(ref T model, StringBuilder builder, IDbCommand command)
        {
            var conditions = conditionsGetter.Invoke(model);
            if (conditions == null || conditions.Length == 0)
            {
                throw new NullReferenceException("Fields returned from the fieldGetter cannot be null or empty.");
            }

            for (int i = 0; i < conditions.Length; i++)
            {
                var condition = conditions[i];
                var fieldName = condition.field.GetFieldName();

                // append AND if not the first
                if (i != 0)
                {
                    builder.Append(" AND");
                }

                // append to command text
                builder.Append(' ');
                builder.Append(fieldName);
                builder.Append(GetComparisonOperator(condition.comparisonType));
                builder.Append("?w");
                builder.Append(fieldName);

                // add parameter to command
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"?w{fieldName}";
                parameter.Value = condition.comparisonValue;
                command.Parameters.Add(parameter);
            }
        }

        private string GetComparisonOperator(ConditionComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ConditionComparisonType.NotEquals:
                    return "!=";
                case ConditionComparisonType.GreaterThan:
                    return ">";
                case ConditionComparisonType.GreaterThanOrEquals:
                    return ">=";
                case ConditionComparisonType.LessThan:
                    return "<";
                case ConditionComparisonType.LessThanOrEquals:
                    return "<=";
                default:
                    return "=";
            }
        }
    }
}
