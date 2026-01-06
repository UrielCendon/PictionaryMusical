using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Autenticacion;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Autenticacion
{
    [TestClass]
    public class VerificacionCodigoUtilidadesPruebas
    {
        private const string TokenValidoPrueba = "abcd1234-efgh-5678-ijkl-9012mnop3456";
        private const string TokenInvalidoCorto = "abc";
        private const string CodigoValidoPrueba = "123456";
        private const string CodigoInvalidoCorto = "12";
        private const string CodigoInvalidoLetras = "abcdef";

        [TestMethod]
        public void Prueba_ValidarDatosConfirmacion_RetornaFalsoConfirmacionNula()
        {
            bool resultado = VerificacionCodigoUtilidades.ValidarDatosConfirmacion(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarDatosConfirmacion_RetornaFalsoTokenNulo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = null,
                CodigoIngresado = CodigoValidoPrueba
            };

            bool resultado = VerificacionCodigoUtilidades.ValidarDatosConfirmacion(confirmacion);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarDatosConfirmacion_RetornaFalsoTokenVacio()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = string.Empty,
                CodigoIngresado = CodigoValidoPrueba
            };

            bool resultado = VerificacionCodigoUtilidades.ValidarDatosConfirmacion(confirmacion);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarDatosConfirmacion_RetornaFalsoCodigoNulo()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValidoPrueba,
                CodigoIngresado = null
            };

            bool resultado = VerificacionCodigoUtilidades.ValidarDatosConfirmacion(confirmacion);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarDatosConfirmacion_RetornaFalsoCodigoVacio()
        {
            var confirmacion = new ConfirmacionCodigoDTO
            {
                TokenCodigo = TokenValidoPrueba,
                CodigoIngresado = string.Empty
            };

            bool resultado = VerificacionCodigoUtilidades.ValidarDatosConfirmacion(confirmacion);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarToken_RetornaFalsoTokenNulo()
        {
            bool resultado = VerificacionCodigoUtilidades.ValidarToken(null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarToken_RetornaFalsoTokenVacio()
        {
            bool resultado = VerificacionCodigoUtilidades.ValidarToken(string.Empty);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarToken_RetornaFalsoTokenSoloEspacios()
        {
            bool resultado = VerificacionCodigoUtilidades.ValidarToken("   ");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_CrearFalloReenvio_RetornaDtoConCodigoEnviadoFalso()
        {
            string mensajePrueba = "Error de prueba";

            var resultado = VerificacionCodigoUtilidades.CrearFalloReenvio(mensajePrueba);

            Assert.IsFalse(resultado.CodigoEnviado);
        }

        [TestMethod]
        public void Prueba_CrearFalloReenvio_RetornaDtoConMensajeCorrecto()
        {
            string mensajePrueba = "Error de prueba";

            var resultado = VerificacionCodigoUtilidades.CrearFalloReenvio(mensajePrueba);

            Assert.AreEqual(mensajePrueba, resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_CrearFalloOperacion_RetornaDtoConOperacionFallida()
        {
            string mensajePrueba = "Error de operacion";

            var resultado = VerificacionCodigoUtilidades.CrearFalloOperacion(mensajePrueba);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_CrearFalloOperacion_RetornaDtoConMensajeCorrecto()
        {
            string mensajePrueba = "Error de operacion";

            var resultado = VerificacionCodigoUtilidades.CrearFalloOperacion(mensajePrueba);

            Assert.AreEqual(mensajePrueba, resultado.Mensaje);
        }
    }
}
