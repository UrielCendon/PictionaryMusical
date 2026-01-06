using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoInvitacionNotificadorPruebas
    {
        private const string CodigoSalaPrueba = "SALA01";
        private const string CreadorPrueba = "Creador";
        private const string IdiomaEspanol = "es";
        private const string IdiomaIngles = "en";
        private const string IdiomaDesconocido = "fr";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeCodigoSala()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(CodigoSalaPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeNombreCreador()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(CreadorPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_GeneraHtmlValido()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.StartsWith("<html>"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_TerminaConEtiquetaCierre()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.EndsWith("</html>"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaTextoEspanolParaEs()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaTextoInglesParaEn()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaIngles);

            Assert.IsTrue(resultado.Contains("Hello"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaEspanolPorDefecto()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaDesconocido);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaEspanolSiIdiomaEsNulo()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                null);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeMensajeInvitacionEspanol()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains("invitado"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeMensajeInvitacionIngles()
        {
            string resultado = CorreoInvitacionNotificador.ConstruirCuerpoMensaje(
                CodigoSalaPrueba, 
                CreadorPrueba, 
                IdiomaIngles);

            Assert.IsTrue(resultado.Contains("invited"));
        }
    }
}
