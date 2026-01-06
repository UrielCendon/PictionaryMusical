using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.LogicaNegocio;

namespace PictionaryMusicalServidor.Pruebas.Servicios.LogicaNegocio
{
    [TestClass]
    public class JugadorPartidaPruebas
    {
        private const string IdConexionPrueba = "conexion-original";
        private const string NombreUsuarioPrueba = "JugadorOriginal";
        private const int PuntajeInicialCero = 0;
        private const int PuntajeTotal = 100;

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaIdConexion()
        {
            var origen = CrearJugadorOrigenPrueba();

            var copia = origen.CopiarDatosBasicos();

            Assert.AreEqual(IdConexionPrueba, copia.IdConexion);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaNombreUsuario()
        {
            var origen = CrearJugadorOrigenPrueba();

            var copia = origen.CopiarDatosBasicos();

            Assert.AreEqual(NombreUsuarioPrueba, copia.NombreUsuario);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaEsHost()
        {
            var origen = CrearJugadorOrigenPrueba();
            origen.EsHost = true;

            var copia = origen.CopiarDatosBasicos();

            Assert.IsTrue(copia.EsHost);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaEsDibujante()
        {
            var origen = CrearJugadorOrigenPrueba();
            origen.EsDibujante = true;

            var copia = origen.CopiarDatosBasicos();

            Assert.IsTrue(copia.EsDibujante);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaYaAdivino()
        {
            var origen = CrearJugadorOrigenPrueba();
            origen.YaAdivino = true;

            var copia = origen.CopiarDatosBasicos();

            Assert.IsTrue(copia.YaAdivino);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CopiaPuntajeTotal()
        {
            var origen = CrearJugadorOrigenPrueba();
            origen.PuntajeTotal = PuntajeTotal;

            var copia = origen.CopiarDatosBasicos();

            Assert.AreEqual(PuntajeTotal, copia.PuntajeTotal);
        }

        [TestMethod]
        public void Prueba_CopiarDatosBasicos_CreaInstanciaDistinta()
        {
            var origen = CrearJugadorOrigenPrueba();

            var copia = origen.CopiarDatosBasicos();

            Assert.AreNotSame(origen, copia);
        }        
        
        [TestMethod]
        public void Prueba_ToString_ContieneNombreUsuario()
        {
            var jugador = CrearJugadorOrigenPrueba();

            string resultado = jugador.ToString();

            Assert.IsTrue(resultado.Contains(NombreUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_ToString_ContienePuntaje()
        {
            var jugador = CrearJugadorOrigenPrueba();
            jugador.PuntajeTotal = PuntajeTotal;

            string resultado = jugador.ToString();

            Assert.IsTrue(resultado.Contains(PuntajeTotal.ToString()));
        }

        [TestMethod]
        public void Prueba_ToString_ContieneIdConexion()
        {
            var jugador = CrearJugadorOrigenPrueba();

            string resultado = jugador.ToString();

            Assert.IsTrue(resultado.Contains(IdConexionPrueba));
        }

        private JugadorPartida CrearJugadorOrigenPrueba()
        {
            return new JugadorPartida
            {
                IdConexion = IdConexionPrueba,
                NombreUsuario = NombreUsuarioPrueba,
                EsHost = false,
                EsDibujante = false,
                YaAdivino = false,
                PuntajeTotal = PuntajeInicialCero
            };
        }    
    }
}
