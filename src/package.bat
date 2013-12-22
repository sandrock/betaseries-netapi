@echo off
echo:
echo This is a PACKAGING script that may work properly.
echo There are a few dependencies though.
echo    You must have winrar installed for this to work :/
echo Check this script to make sure the paths are right for you.
echo:
echo:

set /p version=<version
set workdir=tmp
set libpath=%workdir%\Srk.BetaseriesApi.%version%
set rar="%ProgramFiles%\winrar\winrar.exe"
set librarname=Srk.BetaseriesApi.%version%.rar
set msbuild4=%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild

echo -
echo Version number is:    %version%
echo Lib destination path: %libpath%
echo Lib archive name:     %librarname%
echo -
echo:

echo:
echo Checks
echo ================
echo:
echo Verifying paths...

echo msbuild4...
if not exist %msbuild4% (
 echo ERROR: msbuild could not be found, verify path. exiting.
 echo Configured as: %msbuild4%
 pause
 goto end
)

echo rar...
if not exist %rar% (
 echo ERROR: rar.exe could not be found, verify path. exiting.
 echo Configured as: %rar%
 pause
 goto end
)

echo Everything is fine.


echo:
echo Build
echo ================
echo: 
echo Let's build the sources...
%msbuild4% Srk.BetaseriesApi.sln /nologo /property:Configuration=Release /verbosity:q
if not %ERRORLEVEL% == 0 (
 echo ERROR: build failed. exiting.
 pause
 goto end
)
echo Let's build more sources...
%msbuild4% Srk.BetaseriesApi.FX45.VS11.sln /nologo /property:Configuration=Release /verbosity:q
if not %ERRORLEVEL% == 0 (
 echo ERROR: build failed. exiting.
 pause
 goto end
)
echo:
echo Build OK.


echo:
echo Copy files
echo ================
echo: 
echo Let's sort all things...

@mkdir %libpath% 2>NUL

echo Copying .NET binaries...
xcopy /Y FX35.Srk.BetaseriesApi\bin\Release\Srk* %libpath%\NET35\ >NUL
xcopy /Y FX35.Srk.BetaseriesApi\bin\Release\fr-fr %libpath%\NET35\fr-fr\ >NUL
xcopy /Y FX45.Srk.BetaseriesApi\bin\Release\Srk* %libpath%\NET45\ >NUL
xcopy /Y FX45.Srk.BetaseriesApi\bin\Release\fr-fr %libpath%\NET45\fr-fr\ >NUL

echo Copying Silverlight 4 binaries...
xcopy /Y SL4.Srk.BetaseriesApi\Bin\Release\Srk* %libpath%\SL4\ >NUL
xcopy /Y SL4.Srk.BetaseriesApi\Bin\Release\fr-fr %libpath%\SL4\fr-fr\ >NUL

echo Copying WP7 binaries...
xcopy /Y WP70.BetaseriesApi\Bin\Release\Srk* %libpath%\WP70\ >NUL
xcopy /Y WP70.BetaseriesApi\Bin\Release\fr-fr %libpath%\WP70\fr-fr\ >NUL

echo Copying WPF app...
xcopy /Y FX35.Srk.BetaseriesApiApp\bin\Release\* "%libpath%\Test App\" >NUL

echo Copying license file...
xcopy /Y ..\LICENSE.txt %libpath%\ >NUL
copy /Y package-Homepage.URL %libpath%\Homepage.URL >NUL
copy /Y package-LibHomepage.URL %libpath%\LibHomepage.URL >NUL
copy /Y package-LibGuide.URL %libpath%\LibGuide.URL >NUL
copy /Y package-Codeplex.URL %libpath%\LibGuide.URL >NUL
copy /Y ApiImplementation.png %libpath%\ClassDiagram.png >NUL
copy /Y package-comment %libpath%\Srk.BetaseriesApi.nfo >NUL


echo:
echo Archive
echo ================
echo: 
echo Make a nice present with a knot.

echo -
echo Creating library archive...
cd %workdir%
%rar% a -z..\package-comment %librarname% Srk.BetaseriesApi.%version%
if not %ERRORLEVEL% == 0 (
 echo ERROR: packaging failed. exiting.
 pause
 goto end
)
cd ..

echo -
echo Done.
pause
@echo on


:end
