using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoCodigoVerificacionNotificadorPruebas
    {
        private const string CodigoVerificacionPrueba = "123456";
        private const string NombreUsuarioDestinoPrueba = "UsuarioTest";
        private const string CodigoIdiomaEspanol = "es";
        private const string CodigoIdiomaIngles = "en";
        private const string CodigoIdiomaEspanolVariante = "es-MX";
        private const string CodigoIdiomaInglesVariante = "en-US";
        private const string CodigoIdiomaFrances = "fr";
        private const string TextoSaludoEspanol = "Hola";
        private const string TextoSaludoIngles = "Hello";
        private const string TextoMensajeCodigoEspanol = "Tu codigo de verificacion es:";
        private const string TextoMensajeCodigoIngles = "Your verification code is:";
        private const string TextoMensajeIgnorarEspanol = "Si no solicitaste este codigo";
        private const string TextoMensajeIgnorarIngles = "If you did not request this code";
        private const string EtiquetaHtmlApertura = "<html>";
        private const string EtiquetaHtmlCierre = "</html>";
        private const string EtiquetaBodyApertura = "<body";
        private const string EtiquetaBodyCierre = "</body>";
        private const string EstiloFontFamily = "font-family";
        private const string EstiloColor = "color";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaEspanolContieneTextoEspanol()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeCodigoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeIgnorarEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaInglesContieneTextoIngles()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaIngles);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeCodigoIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeIgnorarIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneCodigoVerificacion()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(CodigoVerificacionPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneNombreUsuario()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(NombreUsuarioDestinoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsuarioNuloNoIncluyeNombre()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                null,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsFalse(cuerpoMensaje.Contains(NombreUsuarioDestinoPrueba));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsuarioVacioNoIncluyeNombre()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                string.Empty,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(CodigoVerificacionPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstructuraHtmlValida()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlCierre));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaBodyApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaBodyCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaNuloUsaEspanol()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                null);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeCodigoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVacioUsaEspanol()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                string.Empty);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoMensajeCodigoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteEspanolUsaEspanol()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanolVariante);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteInglesUsaIngles()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaInglesVariante);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaDesconocidoUsaEspanol()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaFrances);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstilosCss()
        {
            string cuerpoMensaje = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                NombreUsuarioDestinoPrueba,
                CodigoVerificacionPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EstiloFontFamily));
            Assert.IsTrue(cuerpoMensaje.Contains(EstiloColor));
        }
    }
}
