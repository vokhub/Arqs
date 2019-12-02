#region Copyright (c) 2019 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace Largs
{
    using System;

    public sealed class ShortOptionName :
        IEquatable<ShortOptionName>,
        IComparable<ShortOptionName>, IComparable
    {
        readonly string _ld;

        static readonly ShortOptionName[] Cache;

        static ShortOptionName()
        {
            Cache = new ShortOptionName['z' + 1];
            InitCacheRange('0', '9');
            InitCacheRange('A', 'Z');
            InitCacheRange('a', 'z');

            static void InitCacheRange(char first, char last)
            {
                for (var ch = first; ch <= last; ch++)
                    Cache[ch] = new ShortOptionName(ch);
            }
        }

        static ArgumentOutOfRangeException InvalidNameError(char ch) =>
            new ArgumentOutOfRangeException(nameof(ch), ch, "Short name must be a digit or a lowercase or uppercase ASCII letter.");

        public static ShortOptionName From(char ch) =>
            IsLetterOrDigit(ch) ? Cache[ch] : throw InvalidNameError(ch);

        ShortOptionName(char ch) =>
            _ld = IsLetterOrDigit(ch)
                ? ch.ToString()
                : throw InvalidNameError(ch);

        static bool IsLetterOrDigit(char ch) =>
            ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '9';

        public bool Equals(ShortOptionName other) =>
            !ReferenceEquals(null, other) &&
            (ReferenceEquals(this, other) || _ld == other._ld);

        public override bool Equals(object obj) =>
            Equals(obj as ShortOptionName);

        public override int GetHashCode() => _ld.GetHashCode();

        public static bool operator ==(ShortOptionName left, ShortOptionName right) =>
            Equals(left, right);

        public static bool operator !=(ShortOptionName left, ShortOptionName right) =>
            !Equals(left, right);

        public static implicit operator char(ShortOptionName name) => name._ld[0];

        public int CompareTo(ShortOptionName other) =>
            ((char)this).CompareTo(other);

        int IComparable.CompareTo(object obj)
            => ReferenceEquals(null, obj) ? 1
             : ReferenceEquals(this, obj) ? 0
             : obj is ShortOptionName other ? CompareTo(other)
             : throw new ArgumentException($"Object must be of type {nameof(ShortOptionName)}");

        public static bool operator < (ShortOptionName left, ShortOptionName right) => left.CompareTo(right) < 0;
        public static bool operator > (ShortOptionName left, ShortOptionName right) => left.CompareTo(right) > 0;
        public static bool operator <=(ShortOptionName left, ShortOptionName right) => left.CompareTo(right) <= 0;
        public static bool operator >=(ShortOptionName left, ShortOptionName right) => left.CompareTo(right) >= 0;

        public override string ToString() => _ld;
    }
}