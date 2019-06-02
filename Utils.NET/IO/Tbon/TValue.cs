using System;
namespace Utils.NET.IO.Tbon
{
    public enum TValueType
    {
        String,
        Number
    }

    public class TValue : TToken
    {
        private readonly static Type stringType = typeof(string);

        public override bool IsValue => true;

        public override bool IsObject => false;

        /// <summary>
        /// The type of the value
        /// </summary>
        public TValueType valueType;

        /// <summary>
        /// String representation of the value
        /// </summary>
        private readonly string strValue;

        /// <summary>
        /// Parsed value from data
        /// </summary>
        private object value;

        public TValue(string value)
        {
            strValue = value.Trim();
            ParseStringValue();
        }

        private void ParseStringValue()
        {
            if (double.TryParse(strValue, out double numberVal))
            {
                valueType = TValueType.Number;
                value = numberVal;
            }
            else
            {
                valueType = TValueType.String;
                if (strValue.Length >= 2 && strValue[0] == '\"' && strValue[strValue.Length - 1] == '\"')
                {
                    value = strValue.Substring(1, strValue.Length - 2);
                }
                else
                {
                    value = strValue;
                }
            }
        }

        public override T Value<T>()
        {
            var tType = typeof(T);
            var tIsString = tType.IsEquivalentTo(stringType);
            if (valueType == TValueType.Number && tIsString)
            {
                return (T)(object)strValue;
            }
            return (T)value;
        }
    }
}
