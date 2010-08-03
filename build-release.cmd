REM other targets are:
REM 'build'

@ECHO OFF
cls

.\tools\nant\nant.exe -f:Spring.Data.GemFire.build package-zip -D:project.sign=true -D:project.releasetype=release
