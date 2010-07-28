#region License

/*
 * Copyright 2002-2010 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using GemStone.GemFire.Cache;
using Spring.Dao;

namespace Spring.Data.GemFire
{
    /// <summary>
    ///  
    /// </summary>
    /// <author>Mark Pollack</author>
    public abstract class GemFireCacheUtils
    {
        public static DataAccessException ConvertGemFireAccessException(GemFireException ex)
        {
            if (ex is CacheExistsException)
            {
                return new DataIntegrityViolationException(ex.Message, ex);
            }
            if (ex is EntryExistsException)
            {
                return new DuplicateKeyException(ex.Message, ex);
            }
            if (ex is EntryNotFoundException)
            {
                return new DataRetrievalFailureException(ex.Message, ex);
            }
            if (ex is RegionExistsException)
            {
                return new DataIntegrityViolationException(ex.Message, ex);
            }
            if (ex is CqClosedException)
            {
                return new InvalidDataAccessApiUsageException(ex.Message, ex);
            }
            if (ex is DiskCorruptException)
            {
                return new DataAccessResourceFailureException(ex.Message, ex);
            }
            if (ex is DiskFailureException)
            {
                return new DataAccessResourceFailureException(ex.Message, ex);
            }
            if (ex is EntryDestroyedException)
            {
                return new InvalidDataAccessApiUsageException(ex.Message, ex);
            }
            if (ex is RegionDestroyedException)
            {
                return new InvalidDataAccessResourceUsageException(ex.Message, ex);
            }
            if (ex is RegionExistsException)
            {
                return new InvalidDataAccessResourceUsageException(ex.Message, ex);
            }
            if (ex is EntryNotFoundException) 
            {
                return new DataRetrievalFailureException(ex.Message, ex);
            }
            if (ex is FunctionExecutionException)
            {
                return new InvalidDataAccessApiUsageException(ex.Message, ex);
            }
           
            if (ex is CacheExistsException)
            {
                return new DataIntegrityViolationException(ex.Message, ex);
            }
            if (ex is CqClosedException)
            {
                return new InvalidDataAccessApiUsageException(ex.Message, ex);
            }
            if (ex is GemFireIOException)
            {
                return new DataAccessResourceFailureException(ex.Message, ex);
            }
            return new GemFireSystemException(ex);
        }
    }

}