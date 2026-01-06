using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class SesionUsuarioManejadorPruebas
    {
        private const int IdUsuarioPrueba = 1;
        private const int IdUsuarioDosPrueba = 2;
        private const string NombreUsuarioPrueba = "UsuarioPrueba";
        private const string NombreUsuarioDosPrueba = "UsuarioDosPrueba";

        private SesionUsuarioManejador _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _manejador = new SesionUsuarioManejador();
        }

        [TestMethod]
        public void Prueba_TieneSesionActiva_RetornaFalsoSinSesiones()
        {
            bool resultado = _manejador.TieneSesionActiva(IdUsuarioPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_IntentarRegistrarSesion_RetornaVerdaderoSesionNueva()
        {
            bool resultado = _manejador.IntentarRegistrarSesion(
                IdUsuarioPrueba, 
                NombreUsuarioPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_IntentarRegistrarSesion_RetornaFalsoSesionDuplicada()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            bool resultado = _manejador.IntentarRegistrarSesion(
                IdUsuarioPrueba, 
                NombreUsuarioPrueba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_TieneSesionActiva_RetornaVerdaderoConSesionRegistrada()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            bool resultado = _manejador.TieneSesionActiva(IdUsuarioPrueba);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EliminarSesion_EliminaSesionExistente()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesion(IdUsuarioPrueba);

            Assert.IsFalse(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesion_NoFallaConSesionInexistente()
        {
            _manejador.EliminarSesion(IdUsuarioPrueba);

            Assert.IsFalse(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesionPorNombre_EliminaSesionExistente()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesionPorNombre(NombreUsuarioPrueba);

            Assert.IsFalse(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesionPorNombre_IgnoraNombreNulo()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesionPorNombre(null);

            Assert.IsTrue(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesionPorNombre_IgnoraNombreVacio()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesionPorNombre(string.Empty);

            Assert.IsTrue(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesionPorNombre_IgnoraNombreSoloEspacios()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesionPorNombre("   ");

            Assert.IsTrue(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_EliminarSesionPorNombre_NoDistingueMayusculasMinusculas()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);

            _manejador.EliminarSesionPorNombre(NombreUsuarioPrueba.ToUpper());

            Assert.IsFalse(_manejador.TieneSesionActiva(IdUsuarioPrueba));
        }

        [TestMethod]
        public void Prueba_ObtenerConteoSesiones_RetornaCeroSinSesiones()
        {
            int conteo = _manejador.ObtenerConteoSesiones();

            Assert.AreEqual(0, conteo);
        }

        [TestMethod]
        public void Prueba_ObtenerConteoSesiones_RetornaNumeroCorrectoConSesiones()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);
            _manejador.IntentarRegistrarSesion(IdUsuarioDosPrueba, NombreUsuarioDosPrueba);

            int conteo = _manejador.ObtenerConteoSesiones();

            Assert.AreEqual(2, conteo);
        }

        [TestMethod]
        public void Prueba_ObtenerConteoSesiones_ActualizaDespuesDeEliminar()
        {
            _manejador.IntentarRegistrarSesion(IdUsuarioPrueba, NombreUsuarioPrueba);
            _manejador.IntentarRegistrarSesion(IdUsuarioDosPrueba, NombreUsuarioDosPrueba);
            _manejador.EliminarSesion(IdUsuarioPrueba);

            int conteo = _manejador.ObtenerConteoSesiones();

            Assert.AreEqual(1, conteo);
        }

        [TestMethod]
        public void Prueba_Instancia_RetornaMismaInstancia()
        {
            var instancia1 = SesionUsuarioManejador.Instancia;
            var instancia2 = SesionUsuarioManejador.Instancia;

            Assert.AreSame(instancia1, instancia2);
        }
    }
}
