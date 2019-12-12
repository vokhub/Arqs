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

namespace Arqs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Linq;

    partial class Parser
    {
        static class Parsers
        {
            public static readonly IParser<string> Id = Create(ParseResult.Success);

            public static readonly IParser<int, NumberParseOptions> Int32 =
                Create(new NumberParseOptions(NumberStyles.Integer, CultureInfo.InvariantCulture),
                       (s, options) => int.TryParse(s, options.Styles, options.FormatProvider, out var v) ? ParseResult.Success(v) : default);

            public static readonly IParser<double, NumberParseOptions> Double =
                Create(new NumberParseOptions(NumberStyles.Float, CultureInfo.InvariantCulture),
                       (s, options) => double.TryParse(s, options.Styles, options.FormatProvider, out var v) ? ParseResult.Success(v) : default);

            public static readonly IParser<DateTime, DateTimeParseOptions> DateTime =
                Create(new DateTimeParseOptions(ImmutableArray<string>.Empty, DateTimeStyles.None, CultureInfo.InvariantCulture),
                       (s, options) => options.Formats.IsDefaultOrEmpty
                                     ? options.ParseFormatted(s)
                                     : System.DateTime.TryParse(s, options.FormatProvider, options.Styles, out var v) ? ParseResult.Success(v) : default);
        }

        public static IParser<string> String() => Parsers.Id;

        public static IParser<T?> Nullable<T>(this IParser<T> parser) where T : struct =>
            parser.Cast<T?>();

        public static IParser<int, NumberParseOptions> Int32() => Parsers.Int32;
        public static IParser<int, NumberParseOptions> Int32(NumberStyles styles) => Parsers.Int32.WithOptions(Parsers.Int32.Options.WithStyles(styles));

        public static IParser<double, NumberParseOptions> Double() => Parsers.Double;
        public static IParser<double, NumberParseOptions> Double(NumberStyles styles) => Parsers.Double.WithOptions(Parsers.Int32.Options.WithStyles(styles));

        public static IParser<T> FormatProvider<T>(this IParser<T, NumberParseOptions> parser, IFormatProvider value) =>
            parser.WithOptions(parser.Options.WithFormatProvider(value));

        public static IParser<T> Styles<T>(this IParser<T, NumberParseOptions> parser, NumberStyles value) =>
            parser.WithOptions(parser.Options.WithStyles(value));

        public static IParser<DateTime, DateTimeParseOptions> DateTime() => Parsers.DateTime;
        public static IParser<DateTime, DateTimeParseOptions> DateTime(string format) => Parsers.DateTime.WithOptions(Parsers.DateTime.Options.WithFormats(ImmutableArray.Create(format)));

        public static IParser<T> FormatProvider<T>(this IParser<T, DateTimeParseOptions> parser, IFormatProvider value) =>
            parser.WithOptions(parser.Options.WithFormatProvider(value));

        public static IParser<T> Styles<T>(this IParser<T, DateTimeParseOptions> parser, DateTimeStyles value) =>
            parser.WithOptions(parser.Options.WithStyles(value));

        public static IParser<T> Format<T>(this IParser<T, DateTimeParseOptions> parser, string[] value) =>
            parser.WithOptions(parser.Options.WithFormats(value.ToImmutableArray()));

        public static IParser<string> Choose(params string[] choices) =>
            String().Choose(StringComparer.Ordinal, choices);

        public static IParser<string> Choose(CaseSensitivity caseSensitivity, params string[] choices) =>
            String().Choose(caseSensitivity.ToStringComparer(), choices);

        public static IParser<T> Choose<T>(this IParser<T> parser, params T[] choices) =>
            parser.Choose(EqualityComparer<T>.Default, choices);

        public static IParser<T> Choose<T>(this IParser<T> parser, IEqualityComparer<T> comparer, params T[] choices) =>
            from v in parser
            where choices.Contains(v, comparer)
            select v;

        internal static readonly IParser<bool> BooleanPlusMinus = Boolean("+", "-");

        public static IParser<bool> Boolean(string trueString, string falseString) =>
            Boolean(trueString, falseString, StringComparison.Ordinal);

        public static IParser<bool> Boolean(string trueString, string falseString, StringComparison comparison) =>
            Create(s => string.Equals(s, trueString, comparison) ? ParseResult.Success(true)
                      : string.Equals(s, falseString, comparison) ? ParseResult.Success(false)
                      : default);

        public static IParser<string> Literal(string value) =>
            Literal(value, StringComparison.Ordinal);

        public static IParser<string> Literal(string value, StringComparison comparison) =>
            Create(s => string.Equals(s, value, comparison) ? ParseResult.Success(s) : default);

        public static IParser<T> Range<T>(this IParser<T> parser, T min, T max) where T : IComparable<T> =>
            from v in parser
            where v.CompareTo(min) >= 0 && v.CompareTo(max) <= 0
            select v;
    }
}
