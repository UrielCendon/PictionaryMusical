using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoCodigoVerificacionNotificadorPruebas
    {
        private const string CodigoPrueba = "123456";
        private const string UsuarioDestinoPrueba = "UsuarioTest";
        private const string IdiomaEspanol = "es";
        private const string IdiomaIngles = "en";
        private const string IdiomaEspanolVariante = "es-MX";
        private const string IdiomaInglesVariante = "en-US";
        private const string SaludoEspanol = "Hola";
        private const string SaludoIngles = "Hello";
        private const string MensajeCodigoEspanol = "Tu codigo de verificacion es:";
        private const string MensajeCodigoIngles = "Your verification code is:";
        private const string MensajeIgnorarEspanol = "Si no solicitaste este codigo";
        private const string MensajeIgnorarIngles = "If you did not request this code";
        private const string EtiquetaHtmlApertura = "<html>";
        private const string EtiquetaHtmlCierre = "</html>";
        private const string EtiquetaBodyApertura = "<body";
        private const string EtiquetaBodyCierre = "</body>";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaEspanolContieneTextoEspanol()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeCodigoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeIgnorarEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaInglesContieneTextoIngles()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaIngles);

            Assert.IsTrue(cuerpo.Contains(SaludoIngles));
            Assert.IsTrue(cuerpo.Contains(MensajeCodigoIngles));
            Assert.IsTrue(cuerpo.Contains(MensajeIgnorarIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneCodigoVerificacion()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(CodigoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneNombreUsuario()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(UsuarioDestinoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsuarioNuloNoIncluyeNombre()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                null,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsFalse(cuerpo.Contains(UsuarioDestinoPrueba));
            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsuarioVacioNoIncluyeNombre()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                string.Empty,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(CodigoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstructuraHtmlValida()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlCierre));
            Assert.IsTrue(cuerpo.Contains(EtiquetaBodyApertura));
            Assert.IsTrue(cuerpo.Contains(EtiquetaBodyCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaNuloUsaEspanol()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                null);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeCodigoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVacioUsaEspanol()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                string.Empty);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeCodigoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteEspanolUsaEspanol()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanolVariante);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteInglesUsaIngles()
        {
            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaInglesVariante);

            Assert.IsTrue(cuerpo.Contains(SaludoIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaDesconocidoUsaEspanol()
        {
            string idiomaDesconocido = "fr";

            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                idiomaDesconocido);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstilosCss()
        {
            string estiloFontFamily = "font-family";
            string estiloColor = "color";

            string cuerpo = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba,
                CodigoPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(estiloFontFamily));
            Assert.IsTrue(cuerpo.Contains(estiloColor));
        }
    }
}
