@setlocal enableextensions
@set scriptdir=%~dp0
@set gf=%scriptdir:\bin\=%
@if exist "%gf%\bin\gfcpp.bat" @goto gfok
@echo Could not determine GFCPP product location
@verify other 2>nul
@goto done
:gfok
@set GFCPP=%gf%
@"%GFCPP%\bin\gfcpp.exe" %*
:done
