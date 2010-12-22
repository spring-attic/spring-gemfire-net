THE Spring Gemfire project for .NET, Release 1.0.0 M2 (December 10, 2010)
--------------------------------------------------------------------
http://www.springsource.org/spring-gemfire


1. INTRODUCTION

The 1.0.0 M2 release of Spring Gemfire for .NET contains

     * IoC configuration of Cache, Regions, and associated CacheLoaders, CacheListener, and CacheWriters
     * Exception translation of GemFire exceptions into Spring's DAO exception hierarchy for use with Spring's
       PersistenceExceptionTranslationPostProcessor.
     * Namespace support for configuration of Cache and Regions
     * Support for Spring.NET Cache advice

2. SUPPORTED .NET FRAMEWORK VERSIONS

Spring Gemfire for .NET 1.0 supports .NET 2.0, 3.0 and 3.5.  

3. KNOWN ISSUES

None

4. RELEASE INFO

Release contents:

* "src" contains the C# source files for the framework
* "test" contains the C# source files for the test suite
* "bin" contains the distribution dll files
* "lib/net" contains common libraries needed for building and running the framework
* "lib/Gemfire" contains the Gemfire dlls
* "doc" contains reference documentation and MSDN-style API help.
* "examples" contains sample applications.

debug build is done using /DEBUG:full and release build using /DEBUG:pdbonly flags.

The VS.NET solution for the framework and examples are provided.

Latest info is available at the public website: http://www.springsource.org/spring-gemfire

Spring Gemfire for .NET is released under the terms of the Apache Software License (see license.txt).


5. DISTRIBUTION DLLs

The "bin" directory contains the following distinct dll files for use in applications. Dependencies are those other than on the .NET BCL.

- "Spring.Data.Gemfire" (~60 KB)
- Contents: IoC configuration of Cache, Regions, and associated CacheLoaders, CacheListener, and CacheWriters
- Dependencies: Spring.Core, Spring.Aop, Spring.Data,  Common.Logging

6. WHERE TO START?

Documentation can be found in the "docs" directory:
* The Spring Gemfire for .NET reference documentation

Documented sample applications can be found in "examples":

7. How to build

VS.NET
------
The is a solution file for different version of VS.NET

* Spring.Data.Gemfire.sln for use with VS.NET 2008

8. Support

The user forums at http://forum.springframework.net/ are available for you to submit questions, support requests,
and interact with other Spring.NET users.

Bug and issue tracking can be found at https://jira.springframework.org/browse/SGFNET

A Fisheye repository browser is located at https://fisheye.springsource.org/browse/spring-gemfire-net

To get the sources, check them out at the git repository at git://git.springsource.org/spring-gemfire/spring-gemfire-net.git

We are always happy to receive your feedback on the forums. If you think you found a bug, have an improvement suggestion
or feature request, please submit a ticket in JIRA (see link above).

9. Acknowledgements

InnovaSys Document X!
---------------------
InnovSys has kindly provided a license to generate the SDK documentation and supporting utilities for
integration with Visual Studio.






