#region License
//
// Command Line Library: BaseOptionAttribute.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2011 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
//using System.Diagnostics;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides base properties for creating an attribute, used to define rules for command line parsing.
    /// </summary>
    public abstract class BaseOptionAttribute : Attribute
    {
        private string _shortName;

        /// <summary>
        /// Short name of this command line option. You can use only one character.
        /// </summary>
        public string ShortName
        {
            get { return _shortName; }
            internal set
            {
                if (value != null && value.Length > 1)
                    throw new ArgumentException("shortName");

                _shortName = value;
            }
        }

        /// <summary>
        /// Long name of this command line option. This name is usually a single english word.
        /// </summary>
        public string LongName { get; internal set; }

        /// <summary>
        /// True if this command line option is required.
        /// </summary>
        public virtual bool Required { get; set; }

        internal bool HasShortName
        {
            get { return !string.IsNullOrEmpty(_shortName); }
        }

        internal bool HasLongName
        {
            get { return !string.IsNullOrEmpty(LongName); }
        }

        /// <summary>
        /// A short description of this command line option. Usually a sentence summary. 
        /// </summary>
        public string HelpText { get; set; }
    }
}
