using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iQuarc.DataAccess.UnitTests
{
    // TODO move this to a package with extensions to MS Tests
    public static class AssertEx
    {
        /// <summary>
        ///     Assertion that fails if the specified Action does not throw any exception.
        /// </summary>
        public static void ShouldThrow(this Action act)
        {
            ShouldThrow<Exception>(act);
        }

        /// <summary>
        ///     Assertion that fails if the specified Action does not throw the specified type of exception (TException).
        ///     If other types of exceptions are thrown or no exception is thrown, the assert succeeds.
        /// </summary>
        public static void ShouldThrow<TException>(this Action act)
            where TException : Exception
        {
            string message = String.Empty;
            ShouldThrow<TException>(act, message);
        }

        public static void ShouldThrow<TException>(this Action act, string message)
            where TException : Exception
        {
            ShouldThrow<TException>(act, assert => true, message);
        }

        public static void ShouldThrow<TException>(this Action act, Func<TException, bool> assert, string message = null)
            where TException : Exception
        {
            try
            {
                act();
            }
            catch (TException expected)
            {
                Assert.IsTrue(assert(expected), message);
                return;
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(message))
                    message = "Exception was thrown but it was not of the expected type: " + ex.GetType().Name + " - " + ex.Message;
                Assert.Fail(message);
            }

            if (string.IsNullOrEmpty(message))
                message = "No exception was thrown.";
            Assert.Fail(message);
        }

        /// <summary>
        ///     Assertion fails if any type of exception is thrown
        /// </summary>
        public static void ShouldNotThrow(this Action act)
        {
            Exception exception = TryExec(act);
            if (exception != null)
                Assert.IsTrue(false, string.Format("Exception of type {0} was thrown when it shouldn't have been.", exception.GetType()));
        }

        /// <summary>
        ///     Assertion that fails if the specified Action throws the specified type of exception (TException).
        ///     If other types of exceptions are thrown or no exception is thrown, the assert succeeds.
        /// </summary>
        public static void ShouldNotThrow<TException>(this Action act)
            where TException : Exception
        {
            string message = "Exception of type " + typeof (TException) + " was thrown when it shouldn't have been.";
            ShouldNotThrow<TException>(act, message);
        }

        /// <summary>
        ///     Assertion that fails with the specified message if the specified Action throws the specified type of exception
        ///     (TException) or a derived type.
        ///     If other types of exceptions are thrown or no exception is thrown, the assert succeeds.
        /// </summary>
        public static void ShouldNotThrow<TException>(this Action act, string message)
            where TException : Exception
        {
            if (TryExec(act) is TException)
                Assert.Fail(message);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static Exception TryExec(Action act)
        {
            try
            {
                act();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        /// <summary>
        ///     Verifies that the specified collections are equivalent.
        ///     Two collections are equivalent if they have the same elements in the same quantity, but in any order.
        ///     Elements are equal if their values are equal, not if they refer to the same object.
        /// </summary>
        public static void AreEquivalent<T>(IEnumerable<T> actual, params T[] expected)
        {
            Func<T, T, bool> equality = EqualityComparer<T>.Default.Equals;
            AreEquivalent(actual, equality, expected);
        }

        /// <summary>
        ///     Verifies that the specified collections are equivalent.
        ///     Two collections are equivalent if they have the same elements in the same quantity, but in any order.
        /// </summary>
        public static void AreEquivalent<T>(IEnumerable<T> actual, Func<T, T, bool> equality, params T[] expected)
        {
            Assert.IsTrue(actual.Count() == expected.Length,
                string.Format("Collections do not have same number of elements. Expected: {0}; Actual: {1}", expected.Length, actual.Count()));

            if (expected.Length == 0)
                return;


            int expectedCount;
            int actualCount;
            T mismatchedElement;
            bool isMismatch = FindMismatchedElement(actual, expected, equality, out expectedCount, out actualCount, out mismatchedElement);

            Assert.IsFalse(isMismatch,
                string.Format("Collections are not equivalent. Mismatch element {0}. Expected count: {1}; Actual: {2}",
                    mismatchedElement, expectedCount, actualCount));
        }

        private static bool FindMismatchedElement<T>(IEnumerable<T> actual, T[] expected, Func<T, T, bool> equality, out int expectedCount, out int actualCount,
            out T mismatchedElement)
        {
            List<ElementCount<T>> expectedElementCounts = GetElementCounts(expected, equality);
            List<ElementCount<T>> actualElementCounts = GetElementCounts(actual, equality);
            foreach (ElementCount<T> expectedElement in expectedElementCounts)
            {
                ElementCount<T> actualElement = actualElementCounts.Find(e => equality(e.Element, expectedElement.Element));

                if (actualElement == null)
                {
                    expectedCount = expectedElement.Count;
                    actualCount = 0;
                    mismatchedElement = expectedElement.Element;
                    return true;
                }

                if (expectedElement.Count != actualElement.Count)
                {
                    expectedCount = expectedElement.Count;
                    actualCount = actualElement.Count;
                    mismatchedElement = expectedElement.Element;
                    return true;
                }
            }

            expectedCount = 0;
            actualCount = 0;
            mismatchedElement = default(T);
            return false;
        }

        private static List<ElementCount<T>> GetElementCounts<T>(IEnumerable<T> collection, Func<T, T, bool> equality)
        {
            List<ElementCount<T>> elementCounts = new List<ElementCount<T>>();
            foreach (T element in collection)
            {
                ElementCount<T> count = elementCounts.Find(p => equality(p.Element, element));
                if (count == null)
                    elementCounts.Add(new ElementCount<T>(element));
                else
                    count.Increment();
            }
            return elementCounts;
        }

        private class ElementCount<T>
        {
            public ElementCount(T element)
            {
                Element = element;
                Count = 1;
            }

            public T Element { get; private set; }
            public int Count { get; private set; }

            public void Increment()
            {
                Count++;
            }
        }
    }
}