#region License

/*
 * Copyright 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region

using System;
using System.Runtime.Serialization;
using GemStone.GemFire.Cache;
using Spring.Dao;

#endregion

namespace Spring.Data.GemFire
{
    /// <summary>
    /// GemFire-specific subclass of UncategorizedDataAccessException, for GemFire system errors 
    /// that do not match any concrete <code>Spring.Data.Dao</code> exceptions.
    /// </summary>
    /// <author>Costin Leau</author>
    /// <author>Mark Pollack (.NET)</author>
    public class GemFireSystemException : UncategorizedDataAccessException
    {
        #region Constructor (s)

        /// <summary>
        /// Initializes a new instance of the <see cref="GemFireSystemException"/> class.
        /// </summary>
        public GemFireSystemException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GemFireSystemException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GemFireSystemException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of the GemFireSystemException class with the specified message
        /// and root cause.
        /// </summary>
        /// <param name="message">
        /// A message about the exception.
        /// </param>
        /// <param name="rootCause">
        /// The root exception that is being wrapped.
        /// </param>
        public GemFireSystemException(string message, Exception rootCause) : base(message, rootCause)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GemFireSystemException"/> class.
        /// </summary>
        /// <param name="cause">The cause.</param>
        public GemFireSystemException(GemFireException cause) : base(cause != null ? cause.Message : null, cause)
        {
        }

        /// <summary>
        /// Creates a new instance of the
        /// <see cref="GemFireSystemException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected GemFireSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion
    }
}