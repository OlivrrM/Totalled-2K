using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totalled
{
    public enum DamageType
    {
        Fall,
        Bullet,
        Melee,
        Explosion,
        FireTick,
        Environment,
        Force,
        Unknown
    }
    public enum HealType
    {
        Consumable,
        Unknown
    }
    public enum RobotActionState
    {
        Idle,
        Chasing,
        Attacking,
        Jumping,
        Custom // Used for custom states for external logic
    }
    public enum KeybindType
    {
        Down,
        Key,
        Up,
        None
    }
    public enum InputKeyState //Used for Terminal commands. Unessesary use of 2 enums
    {
        Down,
        Held,
        Up
    }
    public enum sv_PacketType : byte
    {
        sv_Unknown,
        sv_ServerInfo,
        sv_MapLoadedResult,
        sv_PlayerInfo,
        sv_PlayerDisconnected,
        sv_Movement,
        sv_EnteredGameValidation,
        sv_Orientation
    }
    public enum OnlineState
    {
        Unknown,
        OutGame,
        InGame,
        Disconnected
    }
    public class CommandExecutionBind
    {
        public KeyCode key;
        public InputKeyState inputKeyState;
        public string command;
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NonSerializedPacketField : Attribute { }

    namespace Numerics
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

            public static explicit operator Vector3f16(Vector3 v)
            {
                return new Vector3f16(v.x, v.y, v.z);
            }

            public static explicit operator Vector3(Vector3f16 v)
            {
                return new Vector3(v.x.ToFloat(), v.y.ToFloat(), v.z.ToFloat());
            }
        }
        public class Vector3si8
        {
            private sbyte _x, _y, _z;

            public sbyte Magnitude => (sbyte)Math.Sqrt((float)x * (float)x + (float)y * (float)y + (float)z * (float)z);

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
    [Serializable]
    public struct CamDamageVisualValues //Used for scripts to easily implement values for tweaking in inspector
    {
        public float camDirectionAmount;
        public float camDirectionRandomnessAmount;
        public float camDirectionReturnSpeed;
        public float camDirectionSpeed;
        public float postFxAmount;
        public float postFxDisableTimeMultiplier;
    }
}