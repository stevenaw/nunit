// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using System.Text;
#if !NETFRAMEWORK
using System.Threading;
#endif
using System.Threading.Tasks;

namespace NUnit.Framework.Internal
{
    internal class NUnitWriter : TextWriter
    {
        private readonly StringBuilder _buffer;
        private readonly TextWriter _out;

        public override Encoding Encoding => _out.Encoding;
        public override IFormatProvider FormatProvider => _out.FormatProvider;

        public NUnitWriter()
        {
            _buffer = new StringBuilder();
            _out = TextWriter.Synchronized(new StringWriter(_buffer));
        }
        public string GetOutput()
        {
            lock (_out)
            {
                return _buffer.ToString();
            }
        }

        #region TextWriter overrides

        public override void Close() => _out.Close();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _out.Dispose();
            }
        }

        public override void Flush() => _out.Flush();

        public override void Write(char value) => _out.Write(value);

        public override void Write(char[]? buffer) => _out.Write(buffer);

        public override void Write(char[] buffer, int index, int count) => _out.Write(buffer, index, count);

#if !NETFRAMEWORK
        public override void Write(ReadOnlySpan<char> buffer) => _out.Write(buffer);
#endif

        public override void Write(bool value) => _out.Write(value);

        public override void Write(int value) => _out.Write(value);

        public override void Write(uint value) => _out.Write(value);

        public override void Write(long value) => _out.Write(value);

        public override void Write(ulong value) => _out.Write(value);

        public override void Write(float value) => _out.Write(value);

        public override void Write(double value) => _out.Write(value);

        public override void Write(decimal value) => _out.Write(value);

        public override void Write(string? value) => _out.Write(value);

#if !NETFRAMEWORK
        public override void Write(StringBuilder? value) => _out.Write(value);
#endif

        public override void Write(object? value) => _out.Write(value);

        public override void Write(string format, object? arg0) => _out.Write(format, arg0);

        public override void Write(string format, object? arg0, object? arg1) => _out.Write(format, arg0, arg1);

        public override void Write(string format, object? arg0, object? arg1, object? arg2) => _out.Write(format, arg0, arg1, arg2);

        public override void Write(string format, object?[] arg) => _out.Write(format, arg);

        public override void WriteLine() => _out.WriteLine();

        public override void WriteLine(char value) => _out.WriteLine(value);

        public override void WriteLine(decimal value) => _out.WriteLine(value);

        public override void WriteLine(char[]? buffer) => _out.WriteLine(buffer);

        public override void WriteLine(char[] buffer, int index, int count) => _out.WriteLine(buffer, index, count);

#if !NETFRAMEWORK
        public override void WriteLine(ReadOnlySpan<char> buffer) => _out.WriteLine(buffer);
#endif

        public override void WriteLine(bool value) => _out.WriteLine(value);

        public override void WriteLine(int value) => _out.WriteLine(value);

        public override void WriteLine(uint value) => _out.WriteLine(value);

        public override void WriteLine(long value) => _out.WriteLine(value);

        public override void WriteLine(ulong value) => _out.WriteLine(value);

        public override void WriteLine(float value) => _out.WriteLine(value);

        public override void WriteLine(double value) => _out.WriteLine(value);

        public override void WriteLine(string? value) => _out.WriteLine(value);

#if !NETFRAMEWORK
        public override void WriteLine(StringBuilder? value) => _out.WriteLine(value);
#endif

        public override void WriteLine(object? value) => _out.WriteLine(value);

        public override void WriteLine(string format, object? arg0) => _out.WriteLine(format, arg0);

        public override void WriteLine(string format, object? arg0, object? arg1) => _out.WriteLine(format, arg0, arg1);

        public override void WriteLine(string format, object? arg0, object? arg1, object? arg2) => _out.WriteLine(format, arg0, arg1, arg2);

        public override void WriteLine(string format, object?[] arg) => _out.WriteLine(format, arg);

#if !NETFRAMEWORK
        public override ValueTask DisposeAsync() => _out.DisposeAsync();
#endif

        public override Task WriteAsync(char value) => _out.WriteAsync(value);

        public override Task WriteAsync(string? value) => _out.WriteAsync(value);

#if !NETFRAMEWORK
        public override Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default)
             => _out.WriteAsync(value, cancellationToken);
#endif
        public override Task WriteAsync(char[] buffer, int index, int count)
            => _out.WriteAsync(buffer, index, count);

#if !NETFRAMEWORK
        public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
             => _out.WriteAsync(buffer, cancellationToken);

        public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
            => _out.WriteLineAsync(buffer, cancellationToken);
#endif

        public override Task WriteLineAsync(char value) => _out.WriteLineAsync(value);

        public override Task WriteLineAsync() => _out.WriteLineAsync();

        public override Task WriteLineAsync(string? value) => _out.WriteLineAsync(value);

#if !NETFRAMEWORK
        public override Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default)
            => _out.WriteLineAsync(value, cancellationToken);
#endif
        public override Task WriteLineAsync(char[] buffer, int index, int count)
            => _out.WriteLineAsync(buffer, index, count);

        public override Task FlushAsync() => FlushAsync();
#endregion
    }
}
