# Investigación: Validación de Mensajes del Chat

## Resumen Ejecutivo

Este documento describe cómo se validan los mensajes del chat en el sistema Pictionary Musical. La validación se realiza en múltiples capas: interfaz de usuario (cliente), lógica de negocio del cliente, y servidor.

---

## 1. Arquitectura del Chat

El sistema de chat utiliza una arquitectura cliente-servidor basada en WCF (Windows Communication Foundation) con el patrón duplex para comunicación bidireccional.

### Componentes Principales:

| Componente | Ubicación | Responsabilidad |
|------------|-----------|-----------------|
| `ChatManejador.cs` | Servidor | Gestiona la comunicación en tiempo real entre jugadores |
| `IChatManejador.cs` | Servidor | Contrato del servicio de chat |
| `IChatManejadorCallback.cs` | Servidor | Contrato de callback para notificaciones |
| `ChatVistaModelo.cs` | Cliente | Lógica del chat y validación de respuestas |
| `SalaVistaModelo.cs` | Cliente | Integración del chat con la sala de juego |
| `Sala.xaml` | Cliente | Vista del chat con validaciones de UI |

---

## 2. Validaciones del Lado del Cliente

### 2.1 Validaciones en la Vista (Sala.xaml)

**Archivo:** `PictionaryMusicalCliente/Vista/Sala.xaml` (líneas 361-365)

```xml
<TextBox x:Name="campoTextoChat" 
         Grid.Row="2" 
         Background="LightYellow" 
         BorderBrush="Black" 
         BorderThickness="1"
         Text="{Binding MensajeChat, UpdateSourceTrigger=PropertyChanged}"
         IsEnabled="{Binding PuedeEscribir}" 
         KeyDown="CampoTextoChat_KeyDown"
         MaxLength="150"/>
```

**Validaciones aplicadas:**
- **MaxLength="150"**: Limita el mensaje a 150 caracteres máximo
- **IsEnabled="{Binding PuedeEscribir}"**: Controla si el usuario puede escribir (deshabilita el campo cuando el dibujante está activo o después de adivinar)

### 2.2 Validaciones en SalaVistaModelo.cs

**Archivo:** `PictionaryMusicalCliente/VistaModelo/Salas/SalaVistaModelo.cs`

#### 2.2.1 Límite de Palabras (líneas 71, 903-918)

```csharp
private const int LimitePalabrasChat = 150;

private static string LimitarMensajePorPalabras(string mensaje)
{
    if (string.IsNullOrWhiteSpace(mensaje))
    {
        return mensaje;
    }

    var palabras = mensaje.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

    if (palabras.Length <= LimitePalabrasChat)
    {
        return mensaje;
    }

    return string.Join(" ", palabras.Take(LimitePalabrasChat));
}
```

**Qué valida:** Limita el número de palabras a un máximo de 150

#### 2.2.2 Validación de Mensaje Vacío (líneas 920-929)

```csharp
private void EjecutarEnviarMensajeChat()
{
    if (string.IsNullOrWhiteSpace(MensajeChat))
    {
        return; // No envía mensajes vacíos
    }

    _ = _chatVistaModelo.EnviarMensaje(MensajeChat);
    MensajeChat = string.Empty;
}
```

**Qué valida:** No permite enviar mensajes vacíos o que solo contengan espacios

### 2.3 Validaciones en ChatVistaModelo.cs

**Archivo:** `PictionaryMusicalCliente/VistaModelo/Salas/ChatVistaModelo.cs`

#### 2.3.1 Validación Principal de Envío (líneas 114-143)

```csharp
public async Task EnviarMensaje(string mensaje)
{
    // 1. Validación de mensaje vacío
    if (string.IsNullOrWhiteSpace(mensaje))
    {
        return;
    }

    // 2. Si la partida no ha iniciado, envía normalmente
    if (!EsPartidaIniciada)
    {
        EnviarMensajeAlServidor?.Invoke(mensaje);
        return;
    }

    // 3. El dibujante no puede enviar mensajes durante su turno
    if (EsDibujante)
    {
        return;
    }

    // 4. Verifica si es una respuesta correcta
    if (EsRespuestaCorrecta(mensaje))
    {
        await ProcesarAciertoAsync().ConfigureAwait(false);
    }
    else
    {
        EnviarMensajeAlServidor?.Invoke(mensaje);
    }
}
```

**Qué valida:**
1. **Mensaje vacío**: No permite mensajes vacíos o con solo espacios
2. **Estado de partida**: Comportamiento diferente según si la partida está iniciada
3. **Rol de dibujante**: El dibujante no puede enviar mensajes durante su turno
4. **Respuesta correcta**: Intercepta si el mensaje es la respuesta correcta para procesar el acierto

#### 2.3.2 Validación de Respuesta Correcta (líneas 191-202)

```csharp
private bool EsRespuestaCorrecta(string mensaje)
{
    if (string.IsNullOrWhiteSpace(NombreCancionCorrecta))
    {
        return false;
    }

    return string.Equals(
        mensaje.Trim(),
        NombreCancionCorrecta.Trim(),
        StringComparison.OrdinalIgnoreCase);
}
```

**Qué valida:**
- Compara el mensaje (sin espacios al inicio/fin) con el nombre de la canción correcta
- La comparación es **case-insensitive** (ignora mayúsculas/minúsculas)

---

## 3. Validaciones del Lado del Servidor

### 3.1 ChatManejador.cs

**Archivo:** `PictionaryMusicalServidor/Servicios/Servicios/ChatManejador.cs`

#### 3.1.1 Validación de Nombre de Usuario (línea 107)

```csharp
public void EnviarMensaje(string idSala, string mensaje, string nombreJugador)
{
    try
    {
        _validadorUsuario.Validar(nombreJugador, nameof(nombreJugador));
        // ...
    }
}
```

**Usa:** `ValidadorNombreUsuario.Validar()`

#### 3.1.2 Validación de ID de Sala (líneas 109-112)

```csharp
if (string.IsNullOrWhiteSpace(idSala))
{
    throw new FaultException(MensajesError.Cliente.CodigoSalaObligatorio);
}
```

**Qué valida:** El código de sala es obligatorio y no puede estar vacío

#### 3.1.3 Validación de Mensaje Vacío (líneas 114-117)

```csharp
if (string.IsNullOrWhiteSpace(mensaje))
{
    return; // Simplemente retorna, no lanza excepción
}
```

**Qué valida:** Ignora silenciosamente mensajes vacíos (no genera error)

#### 3.1.4 Normalización de Datos (líneas 119-121)

```csharp
var idSalaNormalizado = idSala.Trim();
var nombreNormalizado = nombreJugador.Trim();
var mensajeNormalizado = mensaje.Trim();
```

**Qué hace:** Elimina espacios al inicio y fin de todos los campos antes de procesar

### 3.2 ValidadorNombreUsuario.cs

**Archivo:** `PictionaryMusicalServidor/Servicios/Servicios/Utilidades/ValidadorNombreUsuario.cs`

```csharp
public void Validar(string nombreUsuario, string parametro)
{
    string normalizado = nombreUsuario?.Trim();

    // 1. Validación de vacío
    if (string.IsNullOrWhiteSpace(normalizado))
    {
        string mensaje = string.Format(CultureInfo.CurrentCulture, 
            MensajesError.Cliente.ParametroObligatorio, parametro);
        throw new FaultException(mensaje);
    }

    // 2. Validación de longitud máxima
    if (normalizado.Length > EntradaComunValidador.LongitudMaximaTexto)
    {
        throw new FaultException(MensajesError.Cliente.UsuarioRegistroInvalido);
    }
}
```

**Qué valida:**
1. **Campo obligatorio**: El nombre de usuario no puede ser vacío o nulo
2. **Longitud máxima**: No puede exceder 50 caracteres (`LongitudMaximaTexto = 50`)

---

## 4. Constantes de Validación

### EntradaComunValidador.cs

```csharp
internal const int LongitudMaximaTexto = 50;
internal const int LongitudMaximaReporte = 100;
internal const int LongitudMaximaContrasena = 15;
internal const int LongitudCodigoVerificacion = 6;
```

---

## 5. Tabla Resumen de Validaciones

| Validación | Ubicación | Valor | Comportamiento |
|------------|-----------|-------|----------------|
| Longitud máxima mensaje | Sala.xaml | 150 caracteres | UI: No permite escribir más |
| Palabras máximas | SalaVistaModelo.cs | 150 palabras | Trunca automáticamente |
| Mensaje vacío (cliente) | ChatVistaModelo.cs | - | Ignora el envío |
| Mensaje vacío (servidor) | ChatManejador.cs | - | Ignora silenciosamente |
| Dibujante no puede escribir | ChatVistaModelo.cs | - | Ignora el mensaje |
| Nombre usuario obligatorio | ValidadorNombreUsuario.cs | - | Lanza FaultException |
| Nombre usuario longitud | ValidadorNombreUsuario.cs | 50 caracteres | Lanza FaultException |
| ID sala obligatorio | ChatManejador.cs | - | Lanza FaultException |
| Respuesta correcta | ChatVistaModelo.cs | Case-insensitive | Procesa como acierto |

---

## 6. Flujo de Validación

```
┌─────────────────────────────────────────────────────────────────────┐
│                        USUARIO ESCRIBE MENSAJE                       │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│  1. Sala.xaml                                                        │
│     - MaxLength=150 (límite de caracteres en UI)                     │
│     - IsEnabled binding (dibujante/adivinó = deshabilitado)          │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│  2. SalaVistaModelo.cs                                               │
│     - LimitarMensajePorPalabras (máx 150 palabras)                   │
│     - Validación string.IsNullOrWhiteSpace                           │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│  3. ChatVistaModelo.cs                                               │
│     - Validación mensaje vacío                                       │
│     - Validación si partida iniciada                                 │
│     - Validación si es dibujante (no puede enviar)                   │
│     - Validación de respuesta correcta (case-insensitive)            │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│  4. ChatManejador.cs (SERVIDOR)                                      │
│     - ValidadorNombreUsuario.Validar (obligatorio, máx 50 chars)     │
│     - Validación idSala obligatorio                                  │
│     - Validación mensaje vacío (ignora)                              │
│     - Normalización (Trim)                                           │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    MENSAJE DISTRIBUIDO A CLIENTES                    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 7. Observaciones y Recomendaciones

### Fortalezas Actuales:
1. ✅ Validación en múltiples capas (cliente y servidor)
2. ✅ Normalización de datos (Trim)
3. ✅ Manejo de errores con mensajes descriptivos
4. ✅ Validación case-insensitive para respuestas correctas
5. ✅ Control de acceso basado en rol (dibujante no puede chatear)

### Posibles Mejoras:
1. ⚠️ **Sanitización XSS**: No se observa sanitización explícita de caracteres HTML/JavaScript en los mensajes
2. ⚠️ **Rate limiting**: No hay control de velocidad de envío de mensajes (podría permitir spam)
3. ⚠️ **Filtro de palabras**: No hay filtro de lenguaje inapropiado
4. ⚠️ **Longitud máxima en servidor**: El servidor no valida explícitamente la longitud del mensaje
5. ℹ️ **Validación duplicada**: La validación de mensaje vacío se hace tanto en cliente como servidor (es correcto para defensa en profundidad)

---

## 8. Archivos Analizados

| Archivo | Ruta |
|---------|------|
| ChatManejador.cs | `PictionaryMusicalServidor/Servicios/Servicios/ChatManejador.cs` |
| IChatManejador.cs | `PictionaryMusicalServidor/Servicios/Contratos/IChatManejador.cs` |
| IChatManejadorCallback.cs | `PictionaryMusicalServidor/Servicios/Contratos/IChatManejadorCallback.cs` |
| ValidadorNombreUsuario.cs | `PictionaryMusicalServidor/Servicios/Servicios/Utilidades/ValidadorNombreUsuario.cs` |
| IValidadorNombreUsuario.cs | `PictionaryMusicalServidor/Servicios/Servicios/Utilidades/IValidadorNombreUsuario.cs` |
| EntradaComunValidador.cs | `PictionaryMusicalServidor/Servicios/Servicios/Utilidades/EntradaComunValidador.cs` |
| MensajesError.cs | `PictionaryMusicalServidor/Servicios/Servicios/Constantes/MensajesError.cs` |
| ChatVistaModelo.cs | `PictionaryMusicalCliente/VistaModelo/Salas/ChatVistaModelo.cs` |
| SalaVistaModelo.cs | `PictionaryMusicalCliente/VistaModelo/Salas/SalaVistaModelo.cs` |
| Sala.xaml | `PictionaryMusicalCliente/Vista/Sala.xaml` |
| Sala.xaml.cs | `PictionaryMusicalCliente/Vista/Sala.xaml.cs` |

---

*Documento generado como resultado de la investigación solicitada sobre validación de mensajes del chat.*
