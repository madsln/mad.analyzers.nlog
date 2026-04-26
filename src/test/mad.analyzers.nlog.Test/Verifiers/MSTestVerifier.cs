using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace mad.analyzers.nlog.Test.Verifiers
{
    /// <summary>
    /// MSTest-basierte Implementierung von <see cref="IVerifier"/>.
    /// </summary>
    public class MSTestVerifier : IVerifier
    {
        public MSTestVerifier()
            : this(ImmutableStack<string>.Empty)
        {
        }

        private MSTestVerifier(ImmutableStack<string> context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected ImmutableStack<string> Context { get; }

        public virtual void Empty<T>(string collectionName, IEnumerable<T> collection)
        {
            var count = collection?.Count() ?? 0;

            if (count != 0)
            {
                Assert.Fail(CreateMessage(
                    $"Expected '{collectionName}' to be empty, but it contains '{count}' element(s)."));
            }
        }

        public virtual void Equal<T>(T expected, T actual, string? message = null)
        {
            if (message is null && Context.IsEmpty)
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected, actual, CreateMessage(message));
            }
        }

        public virtual void True([DoesNotReturnIf(false)] bool assert, string? message = null)
        {
            if (message is null && Context.IsEmpty)
            {
                Assert.IsTrue(assert);
            }
            else
            {
                Assert.IsTrue(assert, CreateMessage(message));
            }
        }

        public virtual void False([DoesNotReturnIf(true)] bool assert, string? message = null)
        {
            if (message is null && Context.IsEmpty)
            {
                Assert.IsFalse(assert);
            }
            else
            {
                Assert.IsFalse(assert, CreateMessage(message));
            }
        }

        [DoesNotReturn]
        public virtual void Fail(string? message = null)
        {
            if (message is null && Context.IsEmpty)
            {
                Assert.Fail();
            }
            else
            {
                Assert.Fail(CreateMessage(message));
            }

            // Wird nie erreicht, hilft aber dem Static Analyzer
            throw new InvalidOperationException("This program location is thought to be unreachable.");
        }

        public virtual void LanguageIsSupported(string language)
        {
            var supported =
                string.Equals(language, LanguageNames.CSharp, StringComparison.Ordinal) ||
                string.Equals(language, LanguageNames.VisualBasic, StringComparison.Ordinal);

            if (!supported)
            {
                Assert.Fail(CreateMessage($"Unsupported language: '{language}'"));
            }
        }

        public virtual void NotEmpty<T>(string collectionName, IEnumerable<T> collection)
        {
            var count = collection?.Count() ?? 0;

            if (count == 0)
            {
                Assert.Fail(CreateMessage(
                    $"Expected '{collectionName}' to be non-empty."));
            }
        }

        public virtual void SequenceEqual<T>(
            IEnumerable<T> expected,
            IEnumerable<T> actual,
            IEqualityComparer<T>? equalityComparer = null,
            string? message = null)
        {
            equalityComparer ??= EqualityComparer<T>.Default;

            var comparer = new SequenceEqualEnumerableEqualityComparer<T>(equalityComparer);
            var areEqual = comparer.Equals(expected, actual);

            if (!areEqual)
            {
                Assert.Fail(CreateMessage(message ?? "Sequences are not equal."));
            }
        }

        public virtual IVerifier PushContext(string context)
        {
            // Gleicher Pattern wie bei den offiziellen Verifiern:
            // sicherstellen, dass abgeleitete Klassen PushContext sinnvoll �berschreiben.
            Assert.AreEqual(
                typeof(MSTestVerifier),
                GetType(),
                "PushContext should be overridden in derived verifiers to create the correct type.");

            return new MSTestVerifier(Context.Push(context));
        }

        protected virtual string CreateMessage(string? message)
        {
            var result = message ?? string.Empty;

            foreach (var frame in Context)
            {
                result = "Context: " + frame + Environment.NewLine + result;
            }

            return result;
        }

        private sealed class SequenceEqualEnumerableEqualityComparer<TItem>
            : IEqualityComparer<IEnumerable<TItem>>
        {
            private readonly IEqualityComparer<TItem> _itemEqualityComparer;

            public SequenceEqualEnumerableEqualityComparer(IEqualityComparer<TItem> itemEqualityComparer)
            {
                _itemEqualityComparer = itemEqualityComparer ?? EqualityComparer<TItem>.Default;
            }

            public bool Equals(IEnumerable<TItem>? x, IEnumerable<TItem>? y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (x is null || y is null)
                    return false;

                return x.SequenceEqual(y, _itemEqualityComparer);
            }

            public int GetHashCode(IEnumerable<TItem> obj)
            {
                if (obj is null)
                    return 0;

                var hash = 0;

                foreach (var item in obj)
                {
                    var itemHash = _itemEqualityComparer.GetHashCode(item!);
                    hash = (hash << 5) + hash ^ itemHash;
                }

                return hash;
            }
        }
    }
}
