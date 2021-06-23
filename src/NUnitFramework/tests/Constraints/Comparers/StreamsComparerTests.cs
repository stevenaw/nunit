// ***********************************************************************
// Copyright (c) 2021 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NUnit.Framework.Constraints.Comparers
{
    [TestFixture]
    public class StreamsComparerTests
    {
        [Test]
        public void RunsQuickOnHappyPath()
        {
            var a = new byte[8092 * 4];

            using var streamA = new MemoryStream(a);
            using var streamB = new MemoryStream(a);

            var comparer = new StreamsComparer(new NUnitEqualityComparer());
            var tolerance = Tolerance.Default;

            var result = comparer.Equal(streamA, streamB, ref tolerance, new ComparisonState());

            Assert.That(result, Is.True);
        }

#if !(NET35 || NET40)
        [Test]
        public void RunsQuickOnHugeStream()
        {
            const int streamSize = 1 << 30;

            using var streamA = new FakeReadOnlyStream(streamSize);
            using var streamB = new FakeReadOnlyStream(streamSize);

            var comparer = new StreamsComparer(new NUnitEqualityComparer());
            var tolerance = Tolerance.Default;

            var result = comparer.Equal(streamA, streamB, ref tolerance, new ComparisonState());

            Assert.That(result, Is.True);
        }
#endif

        private class FakeReadOnlyStream : Stream
        {
            public FakeReadOnlyStream(long length)
            {
                Length = length;
            }

            public override bool CanRead => true;

            public override bool CanSeek => true;

            public override bool CanWrite => false;

            public override long Length { get; }

            public override long Position { get; set; }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                var maxRead = (int)Math.Min(Position + offset + count, Length);

                Array.Clear(buffer, offset, maxRead);
                return maxRead;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                if (origin == SeekOrigin.Begin)
                    Position = offset;
                else if (origin == SeekOrigin.End)
                    Position = -offset;
                else if (origin == SeekOrigin.Current)
                    Position += offset;

                return Position;
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
