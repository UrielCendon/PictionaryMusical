using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using PictionaryMusicalCliente.Modelo;

// alias de tus connected services
using SrvAv = PictionaryMusicalCliente.PictionaryServidorServicioAvatares;
using SrvCta = PictionaryMusicalCliente.PictionaryServidorServicioCuenta;

namespace PictionaryMusicalCliente.Servicios
{
    public class ServidorProxy : IDisposable
    {
        private readonly SrvAv.CatalogoAvataresClient _avatares;
        private readonly SrvCta.CuentaManejadorClient _cuentas;

        // ⚠️ Ajusta a tu ruta base real (o deja "" y no combines URIs)
        private const string BASE_IMG = "http://localhost:8086/";

        public ServidorProxy()
        {
            _avatares = new SrvAv.CatalogoAvataresClient("BasicHttpBinding_ICatalogoAvatares");
            _cuentas = new SrvCta.CuentaManejadorClient("BasicHttpBinding_ICuentaManejador");
        }

        public async Task<List<ObjetoAvatar>> ObtenerAvataresAsync()
        {
            var dtos = await _avatares.ObtenerAvataresDisponiblesAsync(); // List<AvatarDTO>
            return dtos.Select(d => new ObjetoAvatar
            {
                Id = d.Id,
                Nombre = d.Nombre,
                RutaRelativa = d.RutaRelativa,
                ImagenUriAbsoluta = string.IsNullOrWhiteSpace(d.RutaRelativa)
                    ? null
                    : new Uri(new Uri(BASE_IMG), d.RutaRelativa).ToString()
            }).ToList();
        }

        public async Task<bool> RegistrarCuentaAsync(SolicitudRegistrarUsuario req)
        {
            var dto = new SrvCta.NuevaCuentaDTO
            {
                Correo = req.Correo,
                Contrasena = req.ContrasenaPlano, // el servidor la hashea
                Usuario = req.Usuario,
                Nombre = req.Nombre,
                Apellido = req.Apellido,
                AvatarId = req.AvatarId
            };

            return await _cuentas.RegistrarCuentaAsync(dto);
        }

        public void Dispose()
        {
            try { if (_avatares.State == CommunicationState.Opened) _avatares.Close(); else _avatares.Abort(); } catch { _avatares.Abort(); }
            try { if (_cuentas.State == CommunicationState.Opened) _cuentas.Close(); else _cuentas.Abort(); } catch { _cuentas.Abort(); }
        }
    }
}
