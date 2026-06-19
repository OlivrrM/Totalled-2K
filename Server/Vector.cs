using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace t2kCore
{
    class Vector3
    {
        private float _x, _y, _z;
        public float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);
        public float SqrMagnitude => x * x + y * y + z * z;

        public static readonly Vector3 One = new Vector3(1f, 1f, 1f);
        public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);
        public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);
        public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);
        public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);
        public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);
        public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);
        public static readonly Vector3 Backward = new Vector3(0f, 0f, -1f);
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3() { }
        public float x
        {
            get => _x;
            set
            {
                _x = value;
                OnValueChanged();
            }
        }

        public float y
        {
            get => _y;
            set
            {
                _y = value;
                OnValueChanged();
            }
        }

        public float z
        {
            get => _z;
            set
            {
                _z = value;
                OnValueChanged();
            }
        }
        public Vector3 Normalized
        {
            get
            {
                float mag = Magnitude;
                return mag > 0 ? new Vector3(x / mag, y / mag, z / mag) : Zero;
            }
        }

        public void Normalize()
        {
            float mag = Magnitude;
            if (mag > 0)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }
        public static float Distance(Vector3 a, Vector3 b)
        {
            return (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
        }
        private void OnValueChanged()
        {
            
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
    }
    public class Vector3f16
    {
        private Half _x, _y, _z;

        public Half Magnitude => (Half)Math.Sqrt(x.ToFloat() * x.ToFloat() + y.ToFloat() * y.ToFloat() + z.ToFloat() * z.ToFloat());
        public Half SqrMagnitude => x * x + y * y + z * z;

        public static readonly Vector3f16 One = new Vector3f16(1f, 1f, 1f);
        public static readonly Vector3f16 Zero = new Vector3f16(0f, 0f, 0f);
        public static readonly Vector3f16 Up = new Vector3f16(0f, 1f, 0f);
        public static readonly Vector3f16 Down = new Vector3f16(0f, -1f, 0f);
        public static readonly Vector3f16 Left = new Vector3f16(-1f, 0f, 0f);
        public static readonly Vector3f16 Right = new Vector3f16(1f, 0f, 0f);
        public static readonly Vector3f16 Forward = new Vector3f16(0f, 0f, 1f);
        public static readonly Vector3f16 Backward = new Vector3f16(0f, 0f, -1f);

        public Vector3f16(Half x, Half y, Half z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3f16(float x, float y, float z)
            : this(new Half(x), new Half(y), new Half(z)) { }

        public Vector3f16() { }

        public Half x
        {
            get => _x;
            set
            {
                _x = value;
                OnValueChanged();
            }
        }

        public Half y
        {
            get => _y;
            set
            {
                _y = value;
                OnValueChanged();
            }
        }

        public Half z
        {
            get => _z;
            set
            {
                _z = value;
                OnValueChanged();
            }
        }

        public Vector3f16 Normalized
        {
            get
            {
                Half mag = Magnitude;
                return mag.ToFloat() > 0 ? new Vector3f16(x / mag, y / mag, z / mag) : Zero;
            }
        }

        public void Normalize()
        {
            Half mag = Magnitude;
            if (mag.ToFloat() > 0)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
        }

        public static Half Dot(Vector3f16 a, Vector3f16 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3f16 Cross(Vector3f16 a, Vector3f16 b)
        {
            return new Vector3f16(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static Half Distance(Vector3f16 a, Vector3f16 b)
        {
            return (Half)Math.Sqrt((a.x.ToFloat() - b.x.ToFloat()) * (a.x.ToFloat() - b.x.ToFloat()) +
                                   (a.y.ToFloat() - b.y.ToFloat()) * (a.y.ToFloat() - b.y.ToFloat()) +
                                   (a.z.ToFloat() - b.z.ToFloat()) * (a.z.ToFloat() - b.z.ToFloat()));
        }

        private void OnValueChanged() { }

        public static Vector3f16 operator +(Vector3f16 a, Vector3f16 b)
        {
            return new Vector3f16(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3f16 operator -(Vector3f16 a, Vector3f16 b)
        {
            return new Vector3f16(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
    public class Vector3si8
    {
        private sbyte _x, _y, _z;

        public sbyte Magnitude => (sbyte)Math.Sqrt((float)x * (float)x + (float)y * (float)y + (float)z * (float)z);
        //public sbyte SqrMagnitude => x * x + y * y + z * z;

        public static readonly Vector3si8 One = new Vector3si8(1f, 1f, 1f);
        public static readonly Vector3si8 Zero = new Vector3si8(0f, 0f, 0f);
        public static readonly Vector3si8 Up = new Vector3si8(0f, 1f, 0f);
        public static readonly Vector3si8 Down = new Vector3si8(0f, -1f, 0f);
        public static readonly Vector3si8 Left = new Vector3si8(-1f, 0f, 0f);
        public static readonly Vector3si8 Right = new Vector3si8(1f, 0f, 0f);
        public static readonly Vector3si8 Forward = new Vector3si8(0f, 0f, 1f);
        public static readonly Vector3si8 Backward = new Vector3si8(0f, 0f, -1f);

        public Vector3si8(sbyte x, sbyte y, sbyte z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3si8(float x, float y, float z)
            : this((sbyte)x, (sbyte)y, (sbyte)z) { }

        public Vector3si8() { }

        public sbyte x
        {
            get => _x;
            set
            {
                _x = value;
                OnValueChanged();
            }
        }

        public sbyte y
        {
            get => _y;
            set
            {
                _y = value;
                OnValueChanged();
            }
        }

        public sbyte z
        {
            get => _z;
            set
            {
                _z = value;
                OnValueChanged();
            }
        }

        public Vector3si8 Normalized
        {
            get
            {
                sbyte mag = Magnitude;
                return ((float)mag) > 0 ? new Vector3si8(x / mag, y / mag, z / mag) : Zero;
            }
        }

        public void Normalize()
        {
            sbyte mag = Magnitude;
            if (((float)mag) > 0)
            {
                x /= mag;
                y /= mag;
                z /= mag;
            }
        }

        /*
        public static sbyte Dot(Vector3si8 a, Vector3si8 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3si8 Cross(Vector3si8 a, Vector3si8 b)
        {
            return new VeVector3sf8ctor3f16(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );
        }

        public static sbyte Distance(Vector3si8 a, Vector3si8 b)
        {
            return (sbyte)Math.Sqrt((a.x.ToFloat() - b.x.ToFloat()) * (a.x.ToFloat() - b.x.ToFloat()) +
                                   (a.y.ToFloat() - b.y.ToFloat()) * (a.y.ToFloat() - b.y.ToFloat()) +
                                   (a.z.ToFloat() - b.z.ToFloat()) * (a.z.ToFloat() - b.z.ToFloat()));
        }
        */

        private void OnValueChanged() { }

        public static Vector3si8 operator +(Vector3si8 a, Vector3si8 b)
        {
            return new Vector3si8(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3si8 operator -(Vector3si8 a, Vector3si8 b)
        {
            return new Vector3si8(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
