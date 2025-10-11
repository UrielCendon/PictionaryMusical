using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using PictionaryMusicalCliente.Modelo;

// alias de tus connected services
using SrvAv = PictionaryMusicalCliente.PictionaryServidorServicioAvatares;
using SrvCod = PictionaryMusicalCliente.PictionaryServidorServicioCodigoVerificacion;
using SrvCta = PictionaryMusicalCliente.PictionaryServidorServicioCuenta;
using SrvReenv = PictionaryMusicalCliente.PictionaryServidorServicioReenvioCodigoVerificacion;

namespace PictionaryMusicalCliente.Servicios
{
    public class ServidorProxy : IDisposable
    {
        private readonly SrvAv.CatalogoAvataresClient _avatares;
        private readonly SrvCta.CuentaManejadorClient _cuentas;
        private readonly SrvCod.CodigoVerificacionManejadorClient _codigoVerificacion;
        private readonly SrvReenv.ReenviarCodigoVerificacionManejadorClient _reenviarCodigo;

        private const string BaseImagenesRemotas = "http://localhost:8086/";

        public ServidorProxy()
        {
            _avatares = new SrvAv.CatalogoAvataresClient("BasicHttpBinding_ICatalogoAvatares");
            _cuentas = new SrvCta.CuentaManejadorClient("BasicHttpBinding_ICuentaManejador");
            _codigoVerificacion = new SrvCod.CodigoVerificacionManejadorClient("BasicHttpBinding_ICodigoVerificacionManejador");
            _reenviarCodigo = new SrvReenv.ReenviarCodigoVerificacionManejadorClient("BasicHttpBinding_IReenviarCodigoVerificacionManejador");
        }

        public async Task<List<ObjetoAvatar>> ObtenerAvataresAsync()
        {
            var dtos = await _avatares.ObtenerAvataresDisponiblesAsync();
            return dtos.Select(d => new ObjetoAvatar
            {
                Id = d.Id,
                Nombre = d.Nombre,
                RutaRelativa = d.RutaRelativa,
                ImagenUriAbsoluta = ObtenerRutaAbsoluta(d.RutaRelativa)
            }).ToList();
        }

        public async Task<ResultadoSolicitudCodigo> SolicitarCodigoVerificacionAsync(SolicitudRegistrarUsuario solicitud)
        {
            var dto = CrearNuevaCuentaDtoVerificacion(solicitud);
            SrvCod.ResultadoSolicitudCodigoDTO resultadoDto = await _codigoVerificacion.SolicitarCodigoVerificacionAsync(dto);
            return ConvertirResultadoSolicitudCodigo(resultadoDto);
        }

        public async Task<ResultadoSolicitudCodigo> ReenviarCodigoVerificacionAsync(SolicitudReenviarCodigo solicitud)
        {
            var dto = new SrvReenv.ReenviarCodigoVerificacionDTO
            {
                TokenVerificacion = solicitud.TokenVerificacion
            };

            SrvReenv.ResultadoSolicitudCodigoDTO resultadoDto = await _reenviarCodigo.ReenviarCodigoVerificacionAsync(dto);
            return ConvertirResultadoSolicitudCodigo(resultadoDto);
        }

        public async Task<ResultadoRegistroCuenta> ConfirmarCodigoVerificacionAsync(SolicitudConfirmarCodigo solicitud)
        {
            var dto = new SrvCod.ConfirmarCodigoVerificacionDTO
            {
                TokenVerificacion = solicitud.TokenVerificacion,
                CodigoIngresado = solicitud.Codigo
            };

            SrvCod.ResultadoRegistroCuentaDTO resultadoDto = await _codigoVerificacion.ConfirmarCodigoVerificacionAsync(dto);
            return ConvertirResultadoRegistroCuenta(resultadoDto);
        }

        public async Task<ResultadoRegistroCuenta> RegistrarCuentaAsync(SolicitudRegistrarUsuario solicitud)
        {
            var dto = CrearNuevaCuentaDtoCuenta(solicitud);
            SrvCta.ResultadoRegistroCuentaDTO resultadoDto = await _cuentas.RegistrarCuentaAsync(dto);
            return ConvertirResultadoRegistroCuenta(resultadoDto);
        }

        private static string ObtenerRutaAbsoluta(string rutaRelativa)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
            {
                return null;
            }

            if (Uri.TryCreate(rutaRelativa, UriKind.Absolute, out Uri uriAbsoluta))
            {
                return uriAbsoluta.ToString();
            }

            string rutaNormalizada = rutaRelativa.TrimStart('/');

            if (!string.IsNullOrEmpty(BaseImagenesRemotas)
                && Uri.TryCreate(BaseImagenesRemotas, UriKind.Absolute, out Uri baseUri))
            {
                return new Uri(baseUri, rutaNormalizada).ToString();
            }

            return null;
        }

        public void Dispose()
        {
            CerrarCliente(_avatares);
            CerrarCliente(_cuentas);
            CerrarCliente(_codigoVerificacion);
            CerrarCliente(_reenviarCodigo);
        }

        private static SrvCod.NuevaCuentaDTO CrearNuevaCuentaDtoVerificacion(SolicitudRegistrarUsuario solicitud)
        {
            if (solicitud == null)
            {
                return null;
            }

            return new SrvCod.NuevaCuentaDTO
            {
                Correo = solicitud.Correo,
                Contrasena = solicitud.ContrasenaPlano,
                Usuario = solicitud.Usuario,
                Nombre = solicitud.Nombre,
                Apellido = solicitud.Apellido,
                AvatarId = solicitud.AvatarId
            };
        }

        private static SrvCta.NuevaCuentaDTO CrearNuevaCuentaDtoCuenta(SolicitudRegistrarUsuario solicitud)
        {
            if (solicitud == null)
            {
                return null;
            }

            return new SrvCta.NuevaCuentaDTO
            {
                Correo = solicitud.Correo,
                Contrasena = solicitud.ContrasenaPlano,
                Usuario = solicitud.Usuario,
                Nombre = solicitud.Nombre,
                Apellido = solicitud.Apellido,
                AvatarId = solicitud.AvatarId
            };
        }

        private static ResultadoSolicitudCodigo ConvertirResultadoSolicitudCodigo(SrvCod.ResultadoSolicitudCodigoDTO resultadoDto)
        {
            return resultadoDto == null
                ? null
                : CrearResultadoSolicitudCodigo(
                    resultadoDto.CodigoEnviado,
                    resultadoDto.Mensaje,
                    resultadoDto.TokenVerificacion,
                    resultadoDto.CorreoYaRegistrado,
                    resultadoDto.UsuarioYaRegistrado);
        }

        private static ResultadoSolicitudCodigo ConvertirResultadoSolicitudCodigo(SrvReenv.ResultadoSolicitudCodigoDTO resultadoDto)
        {
            return resultadoDto == null
                ? null
                : CrearResultadoSolicitudCodigo(
                    resultadoDto.CodigoEnviado,
                    resultadoDto.Mensaje,
                    resultadoDto.TokenVerificacion,
                    resultadoDto.CorreoYaRegistrado,
                    resultadoDto.UsuarioYaRegistrado);
        }

        private static ResultadoSolicitudCodigo CrearResultadoSolicitudCodigo(bool codigoEnviado, string mensaje, string token, bool correoYaRegistrado, bool usuarioYaRegistrado)
        {
            return new ResultadoSolicitudCodigo
            {
                CodigoEnviado = codigoEnviado,
                Mensaje = mensaje,
                TokenVerificacion = token,
                CorreoYaRegistrado = correoYaRegistrado,
                UsuarioYaRegistrado = usuarioYaRegistrado
            };
        }

        private static ResultadoRegistroCuenta ConvertirResultadoRegistroCuenta(SrvCod.ResultadoRegistroCuentaDTO resultadoDto)
        {
            return resultadoDto == null
                ? null
                : CrearResultadoRegistroCuenta(
                    resultadoDto.RegistroExitoso,
                    resultadoDto.Mensaje,
                    resultadoDto.CorreoYaRegistrado,
                    resultadoDto.UsuarioYaRegistrado);
        }

        private static ResultadoRegistroCuenta ConvertirResultadoRegistroCuenta(SrvCta.ResultadoRegistroCuentaDTO resultadoDto)
        {
            return resultadoDto == null
                ? null
                : CrearResultadoRegistroCuenta(
                    resultadoDto.RegistroExitoso,
                    resultadoDto.Mensaje,
                    resultadoDto.CorreoYaRegistrado,
                    resultadoDto.UsuarioYaRegistrado);
        }

        private static ResultadoRegistroCuenta CrearResultadoRegistroCuenta(bool registroExitoso, string mensaje, bool correoYaRegistrado, bool usuarioYaRegistrado)
        {
            return new ResultadoRegistroCuenta
            {
                RegistroExitoso = registroExitoso,
                Mensaje = mensaje,
                CorreoYaRegistrado = correoYaRegistrado,
                UsuarioYaRegistrado = usuarioYaRegistrado
            };
        }

        private static void CerrarCliente(ICommunicationObject cliente)
        {
            if (cliente == null)
            {
                return;
            }

            try
            {
                if (cliente.State == CommunicationState.Opened)
                {
                    cliente.Close();
                }
                else
                {
                    cliente.Abort();
                }
            }
            catch
            {
                cliente.Abort();
            }
        }
    }
}
