using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class JugadorPartidaPruebas
    {
        private const string NombreUsuarioPrueba = "TestUser";
        private const string IdConexionPrueba = "conexion-123";
        private const int PuntajeAlto = 500;
        private const int PuntajeInicial = 100;
        private const int PuntajeModificado = 999;
        private const int PuntajeVisualizacion = 250;

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaPropiedades()
        {

            var jugadorOriginal = new JugadorPartida
            {
                NombreUsuario = NombreUsuarioPrueba,
                IdConexion = IdConexionPrueba,
                EsHost = true,
                EsDibujante = true,
                YaAdivino = true,
                PuntajeTotal = PuntajeAlto
            };


            var copia = jugadorOriginal.CopiarDatosBasicos();


            Assert.AreEqual(NombreUsuarioPrueba, copia.NombreUsuario);
            Assert.AreEqual(IdConexionPrueba, copia.IdConexion);
            Assert.IsTrue(copia.EsHost);
            Assert.IsTrue(copia.EsDibujante);
            Assert.IsTrue(copia.YaAdivino);
            Assert.AreEqual(PuntajeAlto, copia.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicosModificado_NoAfectaOriginal()
        {
            const string NombreOriginal = "Original";
            const string NombreModificado = "Modificado";

            var jugadorOriginal = new JugadorPartida
            {
                NombreUsuario = NombreOriginal,
                PuntajeTotal = PuntajeInicial
            };


            var copia = jugadorOriginal.CopiarDatosBasicos();
            copia.NombreUsuario = NombreModificado;
            copia.PuntajeTotal = PuntajeModificado;


            Assert.AreEqual(NombreOriginal, jugadorOriginal.NombreUsuario);
            Assert.AreEqual(PuntajeInicial, jugadorOriginal.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_ToString_ContieneInformacionJugador()
        {
            const string NombreJugadorTest = "JugadorTest";
            const string IdConexion = "id-001";
            const string PuntajeEsperadoTexto = "250";

            var jugador = new JugadorPartida
            {
                NombreUsuario = NombreJugadorTest,
                IdConexion = IdConexion,
                EsHost = true,
                EsDibujante = false,
                PuntajeTotal = PuntajeVisualizacion
            };


            var resultado = jugador.ToString();


            StringAssert.Contains(resultado, NombreJugadorTest);
            StringAssert.Contains(resultado, PuntajeEsperadoTexto);
        }
    }
}
