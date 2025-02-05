﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation.Language;
using System.Text;

namespace Microsoft.PowerShell.EditorServices.Utility
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Extension to evaluate an object's ToString() method in an exception safe way. This will
        /// extension method will not throw.
        /// </summary>
        /// <param name="obj">The object on which to call ToString()</param>
        /// <returns>The ToString() return value or a suitable error message is that throws.</returns>
        public static string SafeToString(this object obj)
        {
            string str;

            try
            {
                str = obj.ToString();
            }
            catch (Exception ex)
            {
                str = $"<Error converting property value to string - {ex.Message}>";
            }

            return str;
        }

        /// <summary>
        /// Get the maximum of the elements from the given enumerable.
        /// </summary>
        /// <typeparam name="T">Type of object for which the enumerable is defined.</typeparam>
        /// <param name="elements">An enumerable object of type T</param>
        /// <param name="comparer">A comparer for ordering elements of type T. The comparer should handle null values.</param>
        /// <returns>An object of type T. If the enumerable is empty or has all null elements, then the method returns null.</returns>
        public static T MaxElement<T>(this IEnumerable<T> elements, Func<T, T, int> comparer) where T : class
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            if (!elements.Any())
            {
                return null;
            }

            T maxElement = elements.First();
            foreach (T element in elements.Skip(1))
            {
                if (element != null && comparer(element, maxElement) > 0)
                {
                    maxElement = element;
                }
            }

            return maxElement;
        }

        /// <summary>
        /// Get the minimum of the elements from the given enumerable.
        /// </summary>
        /// <typeparam name="T">Type of object for which the enumerable is defined.</typeparam>
        /// <param name="elements">An enumerable object of type T</param>
        /// <param name="comparer">A comparer for ordering elements of type T. The comparer should handle null values.</param>
        /// <returns>An object of type T. If the enumerable is empty or has all null elements, then the method returns null.</returns>
        public static T MinElement<T>(this IEnumerable<T> elements, Func<T, T, int> comparer) where T : class => MaxElement(elements, (elementX, elementY) => -1 * comparer(elementX, elementY));

        /// <summary>
        /// Compare extents with respect to their widths.
        ///
        /// Width of an extent is defined as the difference between its EndOffset and StartOffest properties.
        /// </summary>
        /// <param name="extentX">Extent of type IScriptExtent.</param>
        /// <param name="extentY">Extent of type IScriptExtent.</param>
        /// <returns>0 if extentX and extentY are equal in width. 1 if width of extent X is greater than that of extent Y. Otherwise, -1.</returns>
        public static int ExtentWidthComparer(this IScriptExtent extentX, IScriptExtent extentY)
        {
            if (extentX == null && extentY == null)
            {
                return 0;
            }

            if (extentX != null && extentY == null)
            {
                return 1;
            }

            if (extentX == null)
            {
                return -1;
            }

            int extentWidthX = extentX.EndOffset - extentX.StartOffset;
            int extentWidthY = extentY.EndOffset - extentY.StartOffset;
            if (extentWidthX > extentWidthY)
            {
                return 1;
            }
            else if (extentWidthX < extentWidthY)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Check if the given coordinates are wholly contained in the instance's extent.
        /// </summary>
        /// <param name="scriptExtent">Extent of type IScriptExtent.</param>
        /// <param name="line">1-based line number.</param>
        /// <param name="column">1-based column number</param>
        /// <returns>True if the coordinates are wholly contained in the instance's extent, otherwise, false.</returns>
        public static bool Contains(this IScriptExtent scriptExtent, int line, int column)
        {
            if (scriptExtent.StartLineNumber > line || scriptExtent.EndLineNumber < line)
            {
                return false;
            }

            if (scriptExtent.StartLineNumber == line)
            {
                return scriptExtent.StartColumnNumber <= column;
            }

            if (scriptExtent.EndLineNumber == line)
            {
                return scriptExtent.EndColumnNumber >= column;
            }

            return true;
        }

        /// <summary>
        /// Same as <see cref="StringBuilder.AppendLine()" /> but never CRLF. Use this when building
        /// formatting for clients that may not render CRLF correctly.
        /// </summary>
        /// <param name="self"></param>
        public static StringBuilder AppendLineLF(this StringBuilder self) => self.Append('\n');

        /// <summary>
        /// Same as <see cref="StringBuilder.AppendLine(string)" /> but never CRLF. Use this when building
        /// formatting for clients that may not render CRLF correctly.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="value"></param>
        public static StringBuilder AppendLineLF(this StringBuilder self, string value)
            => self.Append(value).Append('\n');
    }
}
