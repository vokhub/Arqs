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

namespace Largs.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    public class Tests
    {
        [Test]
        public void Test1()
        {
            var args =
                from foo in Arg.Value("foo", -1, Parser.Int32()).List()
                join bar in Arg.Flag("bar")  on 1 equals 1
                join baz in Arg.Value("baz", Parser.Int32().Nullable())  on 1 equals 1
                join qux in Arg.Value("qux", "?", Parser.String()) on 1 equals 1
                join xs  in Arg.Value("x", Parser.String()).List() on 1 equals 1
                select new { Foo = foo, Bar = bar, Baz = baz, Qux = qux, X = string.Join(",", xs) };

            var commandLine = "--bar --foo 4 hello --foo 2 -x one -x two world -x three".Split();
            var (result, tail) = args.Bind(commandLine);

            Assert.That(result.Foo, Is.EqualTo(new[] { 4, 2 }));
            Assert.That(result.Bar, Is.True);
            Assert.That(result.Baz, Is.Null);
            Assert.That(result.Qux, Is.EqualTo("?"));
            Assert.That(result.X, Is.EqualTo("one,two,three"));
            Assert.That(tail, Is.EqualTo(new[] { "hello", "world" }));

            var infos = new Queue<IArg>(args.Inspect());
            Assert.That(infos.Dequeue().Name, Is.EqualTo("foo"));
            Assert.That(infos.Dequeue().Name, Is.EqualTo("bar"));
            Assert.That(infos.Dequeue().Name, Is.EqualTo("baz"));
            Assert.That(infos.Dequeue().Name, Is.EqualTo("qux"));
            Assert.That(infos.Dequeue().Name, Is.EqualTo("x"));
        }
    }
}
