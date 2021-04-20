using System;
namespace Utils.NET.Database.Queries.Conditions
{
    public class Condition
    {
        public static Condition Equals(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.Equals);

        public static Condition NotEquals(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.NotEquals);

        public static Condition GreaterThan(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.GreaterThan);

        public static Condition GreaterThanOrEqual(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.GreaterThanOrEquals);

        public static Condition LessThan(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.LessThan);

        public static Condition LessThanOrEqual(DbFieldValue field, object comparisonValue) => new Condition(field, comparisonValue, ConditionComparisonType.LessThanOrEquals);

        public DbFieldValue field;

        public object comparisonValue;

        public ConditionComparisonType comparisonType;

        internal Condition(DbFieldValue field, object comparisonValue, ConditionComparisonType comparisonType)
        {
            this.field = field;
            this.comparisonValue = comparisonValue;
            this.comparisonType = comparisonType;
        }
    }
}
