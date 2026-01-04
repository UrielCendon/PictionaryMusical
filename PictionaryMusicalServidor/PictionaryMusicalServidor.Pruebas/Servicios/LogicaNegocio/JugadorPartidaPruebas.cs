using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios
{
    /// <summary>
    /// Contiene pruebas unitarias para la clase <see cref="JugadorPartida"/>.
    /// Valida la copia de datos y la representacion en cadena del jugador.
    /// </summary>
    [TestClass]
    public class JugadorPartidaPruebas
    {
        private const string NombreUsuarioPrueba = "UsuarioTest";
        private const string IdConexionPrueba = "Conexion123";
        private const bool EsHostVerdadero = true;
        private const bool EsDibujanteFalso = false;
        private const bool YaAdivinoVerdadero = true;
        private const int PuntajeInicial = 100;

        private const string FormatoToStringEsperado = "Nombre: {0}, Conexion: {1}, " +
            "Host: {2}, Dibujante: {3}, Puntaje: {4}";

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CreaNuevaInstanciaConMismosValores()
        {
            var jugadorOriginal = new JugadorPartida
            {
                NombreUsuario = NombreUsuarioPrueba,
                IdConexion = IdConexionPrueba,
                EsHost = EsHostVerdadero,
                EsDibujante = EsDibujanteFalso,
                YaAdivino = YaAdivinoVerdadero,
                PuntajeTotal = PuntajeInicial
            };

            var copiaJugador = jugadorOriginal.CopiarDatosBasicos();

            Assert.AreNotSame(jugadorOriginal, copiaJugador);
            Assert.AreEqual(jugadorOriginal.NombreUsuario, copiaJugador.NombreUsuario);
            Assert.AreEqual(jugadorOriginal.IdConexion, copiaJugador.IdConexion);
            Assert.AreEqual(jugadorOriginal.EsHost, copiaJugador.EsHost);
            Assert.AreEqual(jugadorOriginal.EsDibujante, copiaJugador.EsDibujante);
            Assert.AreEqual(jugadorOriginal.YaAdivino, copiaJugador.YaAdivino);
            Assert.AreEqual(jugadorOriginal.PuntajeTotal, copiaJugador.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_ModificarCopiaNoAfectaOriginal()
        {
            var jugadorOriginal = new JugadorPartida
            {
                PuntajeTotal = PuntajeInicial
            };

            var copiaJugador = jugadorOriginal.CopiarDatosBasicos();
            copiaJugador.PuntajeTotal = 0;

            Assert.AreEqual(PuntajeInicial, jugadorOriginal.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_ToString_DevuelveFormatoCorrecto()
        {
            var jugador = new JugadorPartida
            {
                NombreUsuario = NombreUsuarioPrueba,
                IdConexion = IdConexionPrueba,
                EsHost = EsHostVerdadero,
                EsDibujante = EsDibujanteFalso,
                PuntajeTotal = PuntajeInicial
            };

            string resultadoEsperado = string.Format(
                FormatoToStringEsperado,
                NombreUsuarioPrueba,
                IdConexionPrueba,
                EsHostVerdadero,
                EsDibujanteFalso,
                PuntajeInicial);

            var resultadoObtenido = jugador.ToString();

            Assert.AreEqual(resultadoEsperado, resultadoObtenido);
        }
    }
}