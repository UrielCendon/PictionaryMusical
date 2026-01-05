using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class ManejadorCallbackPruebas
    {
        private const string NombreUsuarioValido = "UsuarioTest";
        private const string NombreUsuarioVacio = "";
        private const string NombreUsuarioNulo = null;
        private const string NombreUsuarioSoloEspacios = "   ";
        private const string NombreUsuarioAlternativo = "UsuarioAlternativo";
        private const string NombreUsuarioMinusculas = "usuariotest";
        private const string NombreUsuarioMayusculas = "USUARIOTEST";
        private const string NombreUsuarioExcedeLongitud =
            "UsuarioConNombreDemasiadoLargoQueExcedeLosLimitesPermitidosEnElSistema" +
            "YDeberiaSerRechazadoPorElValidador";

        private ManejadorCallback<ICallbackPrueba> _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _manejador = new ManejadorCallback<ICallbackPrueba>();
        }

        #region Pruebas Constructor

        [TestMethod]
        public void Prueba_Constructor_SinParametros_CreaInstancia()
        {
            var manejador = new ManejadorCallback<ICallbackPrueba>();
            var callback = new CallbackPrueba();
            manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = manejador.ObtenerCallback(NombreUsuarioValido);

            Assert.AreEqual(callback, resultado);
        }

        [TestMethod]
        public void Prueba_Constructor_ConComparerNulo_UsaComparerPorDefecto()
        {
            var manejador = new ManejadorCallback<ICallbackPrueba>(null);
            var callback = new CallbackPrueba();
            manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = manejador.ObtenerCallback(NombreUsuarioMinusculas);

            Assert.AreEqual(callback, resultado);
        }

        [TestMethod]
        public void Prueba_Constructor_ConOrdinalIgnoreCase_ComparaIgnorandoMayusculas()
        {
            var manejador = new ManejadorCallback<ICallbackPrueba>(StringComparer.OrdinalIgnoreCase);
            var callback = new CallbackPrueba();
            manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = manejador.ObtenerCallback(NombreUsuarioMayusculas);

            Assert.AreEqual(callback, resultado);
        }

        #endregion

        #region Pruebas Suscribir

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioNulo_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioNulo, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioNulo);
            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioVacio_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioVacio, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioVacio);
            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioSoloEspacios_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioSoloEspacios, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioSoloEspacios);
            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_CallbackNulo_NoRegistraSuscripcion()
        {
            _manejador.Suscribir(NombreUsuarioValido, null);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_DatosValidos_RegistraCallbackCorrectamente()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(callback, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_MismoUsuarioDosVeces_SobrescribeCallback()
        {
            var callback1 = new CallbackPrueba();
            var callback2 = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback1);

            _manejador.Suscribir(NombreUsuarioValido, callback2);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(callback2, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_UsuariosDiferentes_RegistraAmbos()
        {
            var callback1 = new CallbackPrueba();
            var callback2 = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioValido, callback1);
            _manejador.Suscribir(NombreUsuarioAlternativo, callback2);

            ICallbackPrueba resultado1 = _manejador.ObtenerCallback(NombreUsuarioValido);
            ICallbackPrueba resultado2 = _manejador.ObtenerCallback(NombreUsuarioAlternativo);
            Assert.AreEqual(callback1, resultado1);
            Assert.AreEqual(callback2, resultado2);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreConDiferentesMayusculas_ConsideraMismoUsuario()
        {
            var callback1 = new CallbackPrueba();
            var callback2 = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioMinusculas, callback1);
            _manejador.Suscribir(NombreUsuarioMayusculas, callback2);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(callback2, resultado);
        }

        #endregion

        #region Pruebas Desuscribir

        [TestMethod]
        public void Prueba_Desuscribir_NombreUsuarioNulo_NoLanzaExcepcion()
        {
            _manejador.Desuscribir(NombreUsuarioNulo);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Prueba_Desuscribir_NombreUsuarioVacio_NoLanzaExcepcion()
        {
            _manejador.Desuscribir(NombreUsuarioVacio);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Prueba_Desuscribir_UsuarioNoExistente_NoLanzaExcepcion()
        {
            _manejador.Desuscribir(NombreUsuarioValido);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Prueba_Desuscribir_UsuarioExistente_EliminaSuscripcion()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback);

            _manejador.Desuscribir(NombreUsuarioValido);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_Desuscribir_SoloEliminaUsuarioEspecifico_OtrosPermanecen()
        {
            var callback1 = new CallbackPrueba();
            var callback2 = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback1);
            _manejador.Suscribir(NombreUsuarioAlternativo, callback2);

            _manejador.Desuscribir(NombreUsuarioValido);

            ICallbackPrueba resultado1 = _manejador.ObtenerCallback(NombreUsuarioValido);
            ICallbackPrueba resultado2 = _manejador.ObtenerCallback(NombreUsuarioAlternativo);
            Assert.AreEqual(null, resultado1);
            Assert.AreEqual(callback2, resultado2);
        }

        [TestMethod]
        public void Prueba_Desuscribir_ConDiferentesMayusculas_EliminaSuscripcion()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioMinusculas, callback);

            _manejador.Desuscribir(NombreUsuarioMayusculas);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(null, resultado);
        }

        #endregion

        #region Pruebas ObtenerCallback

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioNoRegistrado_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);

            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioNulo_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioNulo);

            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioVacio_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioVacio);

            Assert.AreEqual(null, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioRegistrado_RetornaCallback()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);

            Assert.AreEqual(callback, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_ConMayusculasDiferentes_RetornaCallback()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioMinusculas, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioMayusculas);

            Assert.AreEqual(callback, resultado);
        }

        #endregion

        #region Interfaz y Clase de Prueba

        public interface ICallbackPrueba
        {
            void NotificarEvento();
        }

        private class CallbackPrueba : ICallbackPrueba
        {
            public void NotificarEvento()
            {
            }
        }

        #endregion
    }
}
