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

        private ManejadorCallback<ICallbackPrueba> _manejador;

        [TestInitialize]
        public void Inicializar()
        {
            _manejador = new ManejadorCallback<ICallbackPrueba>();
        }

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
            var manejador = new ManejadorCallback<ICallbackPrueba>(
                StringComparer.OrdinalIgnoreCase);
            var callback = new CallbackPrueba();
            manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = manejador.ObtenerCallback(NombreUsuarioMayusculas);

            Assert.AreEqual(callback, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioNulo_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioNulo, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioNulo);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioVacio_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioVacio, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioVacio);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreUsuarioSoloEspacios_NoRegistraCallback()
        {
            var callback = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioSoloEspacios, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioSoloEspacios);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_CallbackNulo_NoRegistraSuscripcion()
        {
            _manejador.Suscribir(NombreUsuarioValido, null);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.IsNull(resultado);
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
            var callbackPrimero = new CallbackPrueba();
            var callbackSegundo = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callbackPrimero);

            _manejador.Suscribir(NombreUsuarioValido, callbackSegundo);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(callbackSegundo, resultado);
        }

        [TestMethod]
        public void Prueba_Suscribir_UsuariosDiferentes_RegistraAmbos()
        {
            var callbackPrimero = new CallbackPrueba();
            var callbackSegundo = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioValido, callbackPrimero);
            _manejador.Suscribir(NombreUsuarioAlternativo, callbackSegundo);

            ICallbackPrueba resultadoPrimero = _manejador.ObtenerCallback(NombreUsuarioValido);
            ICallbackPrueba resultadoSegundo = _manejador.ObtenerCallback(NombreUsuarioAlternativo);
            Assert.AreEqual(callbackPrimero, resultadoPrimero);
            Assert.AreEqual(callbackSegundo, resultadoSegundo);
        }

        [TestMethod]
        public void Prueba_Suscribir_NombreConDiferentesMayusculas_ConsideraMismoUsuario()
        {
            var callbackPrimero = new CallbackPrueba();
            var callbackSegundo = new CallbackPrueba();

            _manejador.Suscribir(NombreUsuarioMinusculas, callbackPrimero);
            _manejador.Suscribir(NombreUsuarioMayusculas, callbackSegundo);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.AreEqual(callbackSegundo, resultado);
        }

        [TestMethod]
        public void Prueba_Desuscribir_NombreUsuarioNulo_NoLanzaExcepcion()
        {
            Exception excepcion = CapturarExcepcion(() => _manejador.Desuscribir(NombreUsuarioNulo));

            Assert.IsNull(excepcion);
        }

        [TestMethod]
        public void Prueba_Desuscribir_NombreUsuarioVacio_NoLanzaExcepcion()
        {
            Exception excepcion = CapturarExcepcion(() => _manejador.Desuscribir(NombreUsuarioVacio));

            Assert.IsNull(excepcion);
        }

        [TestMethod]
        public void Prueba_Desuscribir_UsuarioNoExistente_NoLanzaExcepcion()
        {
            Exception excepcion = CapturarExcepcion(() => _manejador.Desuscribir(NombreUsuarioValido));

            Assert.IsNull(excepcion);
        }

        [TestMethod]
        public void Prueba_Desuscribir_UsuarioExistente_EliminaSuscripcion()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback);

            _manejador.Desuscribir(NombreUsuarioValido);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_Desuscribir_SoloEliminaUsuarioEspecifico_OtrosPermanecen()
        {
            var callbackPrimero = new CallbackPrueba();
            var callbackSegundo = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callbackPrimero);
            _manejador.Suscribir(NombreUsuarioAlternativo, callbackSegundo);

            _manejador.Desuscribir(NombreUsuarioValido);

            ICallbackPrueba resultadoPrimero = _manejador.ObtenerCallback(NombreUsuarioValido);
            ICallbackPrueba resultadoSegundo = _manejador.ObtenerCallback(NombreUsuarioAlternativo);
            Assert.IsNull(resultadoPrimero);
            Assert.AreEqual(callbackSegundo, resultadoSegundo);
        }

        [TestMethod]
        public void Prueba_Desuscribir_ConDiferentesMayusculas_EliminaSuscripcion()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioMinusculas, callback);

            _manejador.Desuscribir(NombreUsuarioMayusculas);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioNoRegistrado_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioNulo_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioNulo);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioVacio_RetornaNulo()
        {
            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioVacio);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_UsuarioRegistrado_RetornaCallback()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioValido, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioValido);

            Assert.IsNotNull(resultado);
            Assert.AreSame(callback, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCallback_ConMayusculasDiferentes_RetornaCallback()
        {
            var callback = new CallbackPrueba();
            _manejador.Suscribir(NombreUsuarioMinusculas, callback);

            ICallbackPrueba resultado = _manejador.ObtenerCallback(NombreUsuarioMayusculas);

            Assert.AreEqual(callback, resultado);
        }

        private static Exception CapturarExcepcion(Action accion)
        {
            try
            {
                accion();
                return null;
            }
            catch (Exception excepcion)
            {
                return excepcion;
            }
        }

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
    }
}
