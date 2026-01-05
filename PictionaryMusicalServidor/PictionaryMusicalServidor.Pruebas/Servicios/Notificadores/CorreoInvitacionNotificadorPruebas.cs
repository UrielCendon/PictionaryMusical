using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoInvitacionNotificadorPruebas
    {
        private const string CodigoSalaPrueba = "ABC123";
        private const string CreadorPrueba = "CreadorTest";
        private const string IdiomaEspanol = "es";
        private const string IdiomaIngles = "en";
        private const string IdiomaEspanolVariante = "es-MX";
        private const string IdiomaInglesVariante = "en-US";
        private const string SaludoEspanol = "Hola";
        private const string SaludoIngles = "Hello";
        private const string MensajeBienvenidaEspanol = "Has sido invitado";
        private const string MensajeBienvenidaIngles = "You have been invited";
        private const string MensajeInstruccionEspanol = "Utiliza el siguiente codigo";
        private const string MensajeInstruccionIngles = "Use the following code";
        private const string MensajeDespedidaEspanol = "Nos vemos en el juego";
        private const string MensajeDespedidaIngles = "See you in the game";
        private const string EtiquetaHtmlApertura = "<html>";
        private const string EtiquetaHtmlCierre = "</html>";
        private const string EtiquetaBodyApertura = "<body";
        private const string EtiquetaBodyCierre = "</body>";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaEspanolContieneTextoEspanol()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeBienvenidaEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeInstruccionEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeDespedidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaInglesContieneTextoIngles()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaIngles);

            Assert.IsTrue(cuerpo.Contains(SaludoIngles));
            Assert.IsTrue(cuerpo.Contains(MensajeBienvenidaIngles));
            Assert.IsTrue(cuerpo.Contains(MensajeInstruccionIngles));
            Assert.IsTrue(cuerpo.Contains(MensajeDespedidaIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneCodigoSala()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(CodigoSalaPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneNombreCreador()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(CreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstructuraHtmlValida()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlCierre));
            Assert.IsTrue(cuerpo.Contains(EtiquetaBodyApertura));
            Assert.IsTrue(cuerpo.Contains(EtiquetaBodyCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaNuloUsaEspanol()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                null);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeBienvenidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVacioUsaEspanol()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                string.Empty);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
            Assert.IsTrue(cuerpo.Contains(MensajeBienvenidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteEspanolUsaEspanol()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanolVariante);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteInglesUsaIngles()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaInglesVariante);

            Assert.IsTrue(cuerpo.Contains(SaludoIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaDesconocidoUsaEspanol()
        {
            string idiomaDesconocido = "fr";

            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                idiomaDesconocido);

            Assert.IsTrue(cuerpo.Contains(SaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_MensajeInvitacionEspanolContieneCreador()
        {
            string mensajeInvitacionEspanol = "te ha invitado a su sala";

            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(mensajeInvitacionEspanol));
            Assert.IsTrue(cuerpo.Contains(CreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_MensajeInvitacionInglesContieneCreador()
        {
            string mensajeInvitacionIngles = "has invited you to their room";

            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaIngles);

            Assert.IsTrue(cuerpo.Contains(mensajeInvitacionIngles));
            Assert.IsTrue(cuerpo.Contains(CreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstilosCss()
        {
            string estiloFontFamily = "font-family";
            string estiloColor = "color";

            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(estiloFontFamily));
            Assert.IsTrue(cuerpo.Contains(estiloColor));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_CodigoSalaVacioGeneraHtmlValido()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                string.Empty,
                CreadorPrueba,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_CreadorVacioGeneraHtmlValido()
        {
            string cuerpo = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                string.Empty,
                IdiomaEspanol);

            Assert.IsTrue(cuerpo.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpo.Contains(CodigoSalaPrueba));
        }
    }
}
