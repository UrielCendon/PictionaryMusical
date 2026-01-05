using System.ServiceModel;
using log4net;
using PictionaryMusicalServidor.Servicios.Servicios.Constantes;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Servicios.Servicios.Salas
{
    /// <summary>
    /// Generador de codigos unicos para salas.
    /// </summary>
    public class GeneradorCodigoSala : IGeneradorCodigoSala
    {
        private const int LongitudCodigo = 6;
        private const int MaximoIntentos = 1000;

        private static readonly ILog _logger = LogManager.GetLogger(typeof(GeneradorCodigoSala));

        private readonly IAlmacenSalas _almacenSalas;

        /// <summary>
        /// Constructor con inyeccion del almacen de salas.
        /// </summary>
        /// <param name="almacenSalas">Almacen para verificar codigos existentes.</param>
        public GeneradorCodigoSala(IAlmacenSalas almacenSalas)
        {
            _almacenSalas = almacenSalas;
        }

        /// <inheritdoc/>
        public string GenerarCodigo()
        {
            for (int i = 0; i < MaximoIntentos; i++)
            {
                string codigo = GeneradorAleatorio.GenerarCodigoSala(LongitudCodigo);
                if (!_almacenSalas.ContieneCodigo(codigo))
                {
                    return codigo;
                }
            }

            _logger.Error(MensajesError.Bitacora.ErrorGenerarCodigoSala);
            throw new FaultException(MensajesError.Cliente.ErrorGenerarCodigo);
        }
    }
}
