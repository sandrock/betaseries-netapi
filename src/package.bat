@echo off

set /p version=<version
set workdir=tmp
set libpath=%workdir%\Srk.BetaseriesApi.%version%
set rar="%ProgramFiles%\winrar\winrar.exe"
set librarname=Srk.BetaseriesApi.%version%.rar


echo Version number is:    %version%
echo Lib destination path: %libpath%
echo Lib archive name:     %librarname%
echo -


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
xcopy /Y FX35.Srk.BetaseriesApi\LICENSE %libpath%\ >NUL
copy /Y package-Homepage.URL %libpath%\Homepage.URL >NUL
copy /Y package-LibHomepage.URL %libpath%\LibHomepage.URL >NUL
copy /Y package-LibGuide.URL %libpath%\LibGuide.URL >NUL
copy /Y package-Codeplex.URL %libpath%\LibGuide.URL >NUL
copy /Y ApiImplementation.png %libpath%\ClassDiagram.png >NUL
copy /Y package-comment %libpath%\Srk.BetaseriesApi.nfo >NUL


echo -
echo Creating library archive...
cd %workdir%
%rar% a -z..\package-comment %librarname% Srk.BetaseriesApi.%version%
cd ..

echo -
echo Done.
pause
@echo on
