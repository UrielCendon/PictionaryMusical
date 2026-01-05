using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoInvitacionNotificadorPruebas
    {
        private const string CodigoSalaPrueba = "ABC123";
        private const string NombreCreadorPrueba = "CreadorTest";
        private const string CodigoIdiomaEspanol = "es";
        private const string CodigoIdiomaIngles = "en";
        private const string CodigoIdiomaEspanolVariante = "es-MX";
        private const string CodigoIdiomaInglesVariante = "en-US";
        private const string CodigoIdiomaFrances = "fr";
        private const string TextoSaludoEspanol = "Hola";
        private const string TextoSaludoIngles = "Hello";
        private const string TextoBienvenidaEspanol = "Has sido invitado";
        private const string TextoBienvenidaIngles = "You have been invited";
        private const string TextoInstruccionEspanol = "Utiliza el siguiente codigo";
        private const string TextoInstruccionIngles = "Use the following code";
        private const string TextoDespedidaEspanol = "Nos vemos en el juego";
        private const string TextoDespedidaIngles = "See you in the game";
        private const string TextoInvitacionEspanol = "te ha invitado a su sala";
        private const string TextoInvitacionIngles = "has invited you to their room";
        private const string EtiquetaHtmlApertura = "<html>";
        private const string EtiquetaHtmlCierre = "</html>";
        private const string EtiquetaBodyApertura = "<body";
        private const string EtiquetaBodyCierre = "</body>";
        private const string EstiloFontFamily = "font-family";
        private const string EstiloColor = "color";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaEspanolContieneTextoEspanol()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoBienvenidaEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoInstruccionEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoDespedidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaInglesContieneTextoIngles()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaIngles);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoBienvenidaIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoInstruccionIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoDespedidaIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneCodigoSala()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(CodigoSalaPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneNombreCreador()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(NombreCreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstructuraHtmlValida()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlCierre));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaBodyApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaBodyCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaNuloUsaEspanolPorDefecto()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                null);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoBienvenidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVacioUsaEspanolPorDefecto()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                string.Empty);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(TextoBienvenidaEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteEspanolUsaEspanol()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanolVariante);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaVarianteInglesUsaIngles()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaInglesVariante);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoIngles));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IdiomaDesconocidoUsaEspanolPorDefecto()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaFrances);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoSaludoEspanol));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_EspanolContieneTextoInvitacion()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoInvitacionEspanol));
            Assert.IsTrue(cuerpoMensaje.Contains(NombreCreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_InglesContieneTextoInvitacion()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaIngles);

            Assert.IsTrue(cuerpoMensaje.Contains(TextoInvitacionIngles));
            Assert.IsTrue(cuerpoMensaje.Contains(NombreCreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ContieneEstilosCss()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EstiloFontFamily));
            Assert.IsTrue(cuerpoMensaje.Contains(EstiloColor));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_CodigoSalaVacioGeneraHtmlValido()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                string.Empty,
                NombreCreadorPrueba,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlCierre));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_CreadorVacioGeneraHtmlValido()
        {
            string cuerpoMensaje = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba,
                string.Empty,
                CodigoIdiomaEspanol);

            Assert.IsTrue(cuerpoMensaje.Contains(EtiquetaHtmlApertura));
            Assert.IsTrue(cuerpoMensaje.Contains(CodigoSalaPrueba));
        }
    }
}
