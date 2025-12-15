@echo off
setlocal EnableExtensions

REM === CONFIGURA ESTO ===
set "SONAR_HOST_URL=http://localhost:9000"
set "PROJECT_KEY=PictionaryMusical"
set "SOLUTION=PictionaryMusicalServidor.sln"
set "SCANNER=C:\Users\Usuario\Downloads\SonarNetframework\SonarScanner.MSBuild.exe"

REM === TOKEN (NO LO PEGUES AQUI) ===
REM Opcion A: setear variable de entorno antes de correr:
REM   set "SONAR_TOKEN=tu_token"
REM Opcion B: pedirlo por consola (se vera mientras escribes):
if "%SONAR_TOKEN%"=="" (
  echo SONAR_TOKEN no esta seteado.
  set /p SONAR_TOKEN=Escribe tu token (se vera al teclear) y presiona Enter:
)

REM === IR A LA CARPETA DEL SCRIPT ===
cd /d "%~dp0"

REM === VALIDAR SONARQUBE ARRIBA ===
echo.
echo Validando SonarQube...
curl "%SONAR_HOST_URL%/api/system/status"
echo.

REM === VALIDAR TOKEN ===
echo Validando token...
curl -u "%SONAR_TOKEN%:" "%SONAR_HOST_URL%/api/authentication/validate"
echo.

REM === LIMPIAR CONFIG ANTERIOR ===
if exist ".sonarqube" (
  rmdir /s /q ".sonarqube"
)

REM === BEGIN ===
"%SCANNER%" begin /k:"%PROJECT_KEY%" ^
  /d:sonar.host.url="%SONAR_HOST_URL%" ^
  /d:sonar.token="%SONAR_TOKEN%" ^
  /d:sonar.scm.disabled=true

if errorlevel 1 (
  echo ERROR: fallo el begin.
  exit /b 1
)

REM === BUILD ===
msbuild "%SOLUTION%" /t:Rebuild
if errorlevel 1 (
  echo ERROR: fallo el build.
  exit /b 1
)

REM === END ===
"%SCANNER%" end /d:sonar.token="%SONAR_TOKEN%"
if errorlevel 1 (
  echo ERROR: fallo el end.
  exit /b 1
)

echo.
echo Listo. Revisa el proyecto en: %SONAR_HOST_URL%
exit /b 0
