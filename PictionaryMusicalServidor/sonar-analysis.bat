@echo off
echo ========================================
echo  Analisis SonarQube - PictionaryMusical
echo ========================================
echo.

set SONAR_TOKEN=sqp_94f2779d6d37aad73a67d5f913d8614100227a90
set SONAR_HOST=http://localhost:9000
set PROJECT_KEY=PictionaryMusical
set VSTEST_PATH=D:\visual studio\Nueva carpeta (3)\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe

echo [1/4] Iniciando SonarScanner...
dotnet sonarscanner begin /k:"%PROJECT_KEY%" /n:"%PROJECT_KEY%" /d:sonar.host.url="%SONAR_HOST%" /d:sonar.token="%SONAR_TOKEN%" /d:sonar.cs.opencover.reportsPaths="coverage.xml"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Fallo al iniciar SonarScanner
    pause
    exit /b 1
)
echo.

echo [2/4] Compilando solucion...
msbuild "PictionaryMusicalServidor.sln" /t:Rebuild /p:Configuration=Debug
if %ERRORLEVEL% neq 0 (
    echo ERROR: Fallo la compilacion
    pause
    exit /b 1
)
echo.

echo [3/4] Ejecutando pruebas con OpenCover...
"packages\OpenCover.4.7.1221\tools\OpenCover.Console.exe" -target:"%VSTEST_PATH%" -targetargs:"PictionaryMusicalServidor.Pruebas\bin\Debug\PictionaryMusicalServidor.Pruebas.dll" -filter:"+[Datos*]* +[Servicios*]* -[PictionaryMusicalServidor.Pruebas*]*" -register:user -output:"coverage.xml"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Fallo la ejecucion de pruebas
    pause
    exit /b 1
)
echo.

echo [4/4] Finalizando analisis SonarQube...
dotnet sonarscanner end /d:sonar.token="%SONAR_TOKEN%"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Fallo al finalizar SonarScanner
    pause
    exit /b 1
)
echo.

echo ========================================
echo  Analisis completado exitosamente!
echo  Ver resultados en: %SONAR_HOST%/dashboard?id=%PROJECT_KEY%
echo ========================================
pause
