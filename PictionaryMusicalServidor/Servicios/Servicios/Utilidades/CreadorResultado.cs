using PictionaryMusicalServidor.Servicios.Contratos.DTOs;

namespace Servicios.Servicios.Utilidades
{
    public static class CreadorResultado
    {
        public static ResultadoOperacionDTO CrearResultadoFallo(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = false,
                Mensaje = mensaje
            };
        }

        public static ResultadoOperacionDTO CrearResultadoOperacion(bool operacion, string mensaje = null)
        {
            return new ResultadoOperacionDTO
            {
                OperacionExitosa = operacion,
                Mensaje = mensaje
            };
        }
    }
}
