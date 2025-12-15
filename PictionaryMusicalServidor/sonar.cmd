@echo off
setlocal EnableExtensions EnableDelayedExpansion

REM ===============================
REM  SONARQUBE ANALYSIS SCRIPT
REM ===============================

REM === CONFIGURACION ===
set "SONAR_HOST_URL=http://localhost:9000"
set "SONAR_TOKEN=sqp_94f2779d6d37aad73a67d5f913d8614100227a90"
set "PROJECT_KEY=PictionaryMusical"
set "SOLUTION=PictionaryMusicalServidor.sln"
set "SCANNER=C:\Users\Usuario\Downloads\SonarNetframework\SonarScanner.MSBuild.exe"

REM === IR A LA CARPETA DEL SCRIPT ===
cd /d "%~dp0"

echo.
echo ===== SonarQube =====
echo Host   : %SONAR_HOST_URL%
echo Key    : %PROJECT_KEY%
echo Sol    : %SOLUTION%
echo Scanner: %SCANNER%
echo.

REM === VALIDAR QUE SONAR ESTE ARRIBA ===
curl "%SONAR_HOST_URL%/api/system/status" || (
  echo ERROR: No se pudo conectar a SonarQube.
  exit /b 1
)

REM === VALIDAR TOKEN (SIN FOR /F PROBLEMATICO) ===
curl -u "%SONAR_TOKEN%:" "%SONAR_HOST_URL%/api/authentication/validate"
echo.

REM === LIMPIAR ANALISIS PREVIO ===
if exist ".sonarqube" (
  rmdir /s /q ".sonarqube"
)

REM === BEGIN ===
"%SCANNER%" begin /k:"%PROJECT_KEY%" ^
  /d:sonar.host.url="%SONAR_HOST_URL%" ^
  /d:sonar.token="%SONAR_TOKEN%" ^
  /d:sonar.scm.disabled=true

if errorlevel 1 (
  echo ERROR: Fallo el begin de Sonar.
  exit /b 1
)

REM === BUILD ===
msbuild "%SOLUTION%" /t:Rebuild
if errorlevel 1 (
  echo ERROR: Fallo la compilacion.
  exit /b 1
)

REM === END ===
"%SCANNER%" end /d:sonar.token="%SONAR_TOKEN%"
if errorlevel 1 (
  echo ERROR: Fallo el end de Sonar.
  exit /b 1
)

echo.
echo Analisis SonarQube completado correctamente.
echo Revisa resultados en: %SONAR_HOST_URL%
exit /b 0
