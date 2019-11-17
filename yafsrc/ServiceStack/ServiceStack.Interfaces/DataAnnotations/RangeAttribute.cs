using System;

namespace ServiceStack.DataAnnotations
{
    public class RangeAttribute : AttributeBase
    {
        public object Minimum { get; }
        public object Maximum { get; }
        public Type OperandType { get; }

        public RangeAttribute(int minimum, int maximum)
        {
            OperandType = typeof(int);
            Minimum = minimum;
            Maximum = maximum;
        }

        public RangeAttribute(double minimum, double maximum)
        {
            OperandType = typeof(double);
            Minimum = minimum;
            Maximum = maximum;
        }

        public RangeAttribute(Type type, string minimum, string maximum)
        {
            OperandType = type;
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
