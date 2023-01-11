using System;
using System.Net;
using System.Linq;

namespace GTA5_Private_Public_Lobby
{
    public class ComparableIPAddress : IPAddress, IComparable
    {
        public ComparableIPAddress(byte[] source)
            : base(source)
        {
        }

        public static ComparableIPAddress From(IPAddress other)
        {
            return other is ComparableIPAddress comparable
                ? comparable
                : new ComparableIPAddress(other.GetAddressBytes());
        }

        public static new ComparableIPAddress Parse(string ipString)
        {
            return From(IPAddress.Parse(ipString));
        }

        public static ComparableIPAddress operator +(ComparableIPAddress obj, byte increment)
        {
            var originalBytes = obj.GetAddressBytes();
            var bytes = obj.GetAddressBytes();

            bytes[^1] += increment; // Increment the last byte

            // If the last byte overflowed, increment the necessary bytes
            for (var i = bytes.Length - 1; i > 0 && bytes[i] < originalBytes[i]; i--)
            {
                bytes[i - 1]++;
            }

            return new ComparableIPAddress(bytes);
        }

        public static ComparableIPAddress operator -(ComparableIPAddress obj, byte decrement)
        {
            var originalBytes = obj.GetAddressBytes();
            var bytes = obj.GetAddressBytes();

            bytes[^1] -= decrement; // Decrement the last byte

            // If the last byte overflowed, increment the necessary bytes
            for (var i = bytes.Length - 1; i > 0 && bytes[i] > originalBytes[i]; i--)
            {
                bytes[i - 1]--;
            }

            return new ComparableIPAddress(bytes);
        }

        #region Comparable

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is not IPAddress other)
            {
                throw new ArgumentException("Object is not a IPAddress");
            }

            var a = GetAddressBytes();
            var b = other.GetAddressBytes();

            if (a.Length != b.Length)
            {
                return a.Length - b.Length;
            }

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return a[i] - b[i];
                }
            }

            return 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) ||
                (obj is IPAddress other && GetAddressBytes().SequenceEqual(other.GetAddressBytes()));
        }

        public static bool operator ==(ComparableIPAddress left, ComparableIPAddress right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(ComparableIPAddress left, ComparableIPAddress right)
        {
            return !(left == right);
        }

        public static bool operator <(ComparableIPAddress left, ComparableIPAddress right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ComparableIPAddress left, ComparableIPAddress right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ComparableIPAddress left, ComparableIPAddress right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ComparableIPAddress left, ComparableIPAddress right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        #endregion
    }
}
