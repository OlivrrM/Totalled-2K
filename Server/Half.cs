using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    public struct Half
    {
        private readonly short value;
        private const float ScaleFactor = 16.0f;

        public Half(float number)
        {
            if (number > 2047.94f) number = 2047.94f;
            if (number < -2047.94f) number = -2047.94f;
            value = (short)(number * ScaleFactor);
        }

        public float ToFloat() => value / ScaleFactor;

        public short ToShort() => value;

        public static Half FromShort(short shortValue) => new Half(shortValue / ScaleFactor);

        public static implicit operator Half(float number) => new Half(number);

        public static implicit operator float(Half half) => half.ToFloat();

        public static explicit operator short(Half half) => half.value;

        public static Half operator +(Half a, Half b) => new Half(a.ToFloat() + b.ToFloat());
        public static Half operator -(Half a, Half b) => new Half(a.ToFloat() - b.ToFloat());
        public static Half operator *(Half a, Half b) => new Half(a.ToFloat() * b.ToFloat());
        public static Half operator /(Half a, Half b) => new Half(a.ToFloat() / b.ToFloat());

        public override string ToString() => ToFloat().ToString("0.00");
    }
}
