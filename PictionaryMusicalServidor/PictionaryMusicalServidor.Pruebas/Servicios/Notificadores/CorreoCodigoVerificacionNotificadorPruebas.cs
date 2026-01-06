using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Servicios.Notificadores;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Notificadores
{
    [TestClass]
    public class CorreoCodigoVerificacionNotificadorPruebas
    {
        private const string CodigoPrueba = "123456";
        private const string UsuarioDestinoPrueba = "UsuarioPrueba";
        private const string IdiomaEspanol = "es";
        private const string IdiomaIngles = "en";
        private const string IdiomaDesconocido = "fr";

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeCodigoEnHtml()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(CodigoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeNombreUsuario()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(UsuarioDestinoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_GeneraHtmlValido()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.StartsWith("<html>"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_TerminaConEtiquetaCierre()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.EndsWith("</html>"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaTextoEspanolParaEs()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaTextoInglesParaEn()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaIngles);

            Assert.IsTrue(resultado.Contains("Hello"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaEspanolPorDefecto()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaDesconocido);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_UsaEspanolSiIdiomaEsNulo()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                null);

            Assert.IsTrue(resultado.Contains("Hola"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ManejaUsuarioVacio()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                string.Empty, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(CodigoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_ManejaUsuarioNulo()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                null, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains(CodigoPrueba));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeMensajeIgnorarEspanol()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaEspanol);

            Assert.IsTrue(resultado.Contains("ignorar"));
        }

        [TestMethod]
        public void Prueba_ConstruirCuerpoMensaje_IncluyeMensajeIgnorarIngles()
        {
            string resultado = CorreoCodigoVerificacionNotificador.ConstruirCuerpoMensaje(
                UsuarioDestinoPrueba, 
                CodigoPrueba, 
                IdiomaIngles);

            Assert.IsTrue(resultado.Contains("ignore"));
        }
    }
}
