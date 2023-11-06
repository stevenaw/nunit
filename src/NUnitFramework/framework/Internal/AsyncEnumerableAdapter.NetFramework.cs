// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
#if NETFRAMEWORK

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace NUnit.Framework.Internal
{
    internal static partial class AsyncEnumerableAdapter
    {
        private static partial bool TryGetAsyncBlockingEnumerable(object enumerable, out IEnumerable<object>? result)
        {
            var asyncEnumerable = enumerable.GetType().GetInterface("System.Collections.Generic.IAsyncEnumerable`1");
            if (asyncEnumerable is null)
            {
                result = default;
                return default;
            }

            var getEnumeratorMethod = asyncEnumerable.GetMethod("GetAsyncEnumerator")!;
            var enumeratorType = getEnumeratorMethod.ReturnType;
            var asyncDisposableType = enumeratorType.GetInterface("System.IAsyncDisposable");

            var shape = new AsyncEnumerableShapeInfo
            {
                GetAsyncEnumerator = getEnumeratorMethod,
                Current = enumeratorType.GetProperty("Current")!,
                MoveNextAsync = enumeratorType.GetMethod("MoveNextAsync")!,
                DisposeAsync =  asyncDisposableType.GetMethod("DisposeAsync")!
            };

            result = new AsyncEnumerableWrapper(shape, enumerable);
            return true;
        }


        private class AsyncEnumerableWrapper : IEnumerable<object>
        {
            private readonly AsyncEnumerableShapeInfo _shape;
            private readonly object _asyncEnumerable;

            public AsyncEnumerableWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerable)
            {
                _shape = shape;
                _asyncEnumerable = asyncEnumerable;
            }

            public IEnumerator<object> GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _shape.GetAsyncEnumerator.Invoke(_asyncEnumerable, new object[] { CancellationToken.None }));

            IEnumerator IEnumerable.GetEnumerator()
                => new AsyncEnumeratorWrapper(_shape, _shape.GetAsyncEnumerator.Invoke(_asyncEnumerable, new object[] { CancellationToken.None }));
        }

        private class AsyncEnumeratorWrapper : IEnumerator<object>
        {
            private readonly AsyncEnumerableShapeInfo _shape;
            private readonly object _asyncEnumerator;

            public AsyncEnumeratorWrapper(AsyncEnumerableShapeInfo shape, object asyncEnumerator)
            {
                _shape = shape;
                _asyncEnumerator = asyncEnumerator;
            }

            public object Current => _shape.Current.GetValue(_asyncEnumerator);

            public void Dispose()
                => AsyncToSyncAdapter.Await(() => _shape.DisposeAsync.Invoke(_asyncEnumerator, null));

            public bool MoveNext()
                => (bool)AsyncToSyncAdapter.Await(() => _shape.MoveNextAsync.Invoke(_asyncEnumerator, null));

            public void Reset()
                => throw new InvalidOperationException("Can not reset an async enumerable.");
        }

        private record AsyncEnumerableShapeInfo
        {
            public MethodInfo GetAsyncEnumerator { get; init; }
            public PropertyInfo Current { get; init; }
            public MethodInfo MoveNextAsync { get; init; }
            public MethodInfo DisposeAsync { get; init; }
        }
    }
}
#endif
