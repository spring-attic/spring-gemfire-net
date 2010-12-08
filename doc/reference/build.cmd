@ECHO OFF
cls
@echo .
@echo ..
@echo ...
@echo Generating documention...
..\..\tools\nant\nant.exe %1 -f:docbook.build
