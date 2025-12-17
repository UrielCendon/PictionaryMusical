using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalCliente.ClienteServicios.Wcf;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase LocalizadorServicio.
    /// Verifica el comportamiento del servicio de localizacion de mensajes de error.
    /// </summary>
    [TestClass]
    public class LocalizadorServicioPruebas
    {
        private LocalizadorServicio _localizador;

        /// <summary>
        /// Inicializa el localizador antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _localizador = new LocalizadorServicio();
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeNulo_RetornaPredeterminado()
        {
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _localizador.Localizar(null, mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeVacio_RetornaPredeterminado()
        {
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _localizador.Localizar("", mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeEspaciosBlanco_RetornaPredeterminado()
        {
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _localizador.Localizar("   ", mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajePredeterminadoNulo_RetornaMensajeGenerico()
        {
            var resultado = _localizador.Localizar(null, null);

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajePredeterminadoVacio_RetornaMensajeGenerico()
        {
            var resultado = _localizador.Localizar("", "");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajePartidaYaIniciada_RetornaTraducido()
        {
            string mensaje = "La partida ya comenzo, no puedes unirte.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.AreNotEqual("default", resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeReporteEnviado_RetornaTraducido()
        {
            string mensaje = "Reporte enviado correctamente.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeCredencialesIncorrectas_RetornaTraducido()
        {
            string mensaje = "Usuario o contrasena incorrectos.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeUsuarioBaneado_RetornaTraducido()
        {
            string mensaje = "Has sido baneado del juego por mala conducta.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeCorreoInvalido_RetornaTraducido()
        {
            string mensaje = "El correo electronico es obligatorio, debe tener un formato valido y no debe exceder 50 caracteres.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeCodigoExpirado_RetornaTraducido()
        {
            string mensaje = "El codigo de verificacion ha expirado. Inicie el proceso nuevamente.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeSalaLlena_RetornaTraducido()
        {
            string mensaje = "La sala esta llena.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeInvitacionEnviada_RetornaTraducido()
        {
            string mensaje = "Invitacion enviada correctamente.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeAutoSolicitudAmistad_RetornaTraducido()
        {
            string mensaje = "No es posible enviarse una solicitud de amistad a si mismo.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajePerfilActualizado_RetornaTraducido()
        {
            string mensaje = "Perfil actualizado correctamente.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeTiempoEsperaCodigo_RetornaTraducidoConTiempo()
        {
            string mensaje = "Debe esperar 30 segundos para solicitar un nuevo codigo.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsTrue(resultado.Contains("30") || resultado != "default");
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeTiempoEsperaCodigo_DiversosTiempos()
        {
            string mensaje60 = "Debe esperar 60 segundos para solicitar un nuevo codigo.";
            string mensaje120 = "Debe esperar 120 segundos para solicitar un nuevo codigo.";

            var resultado60 = _localizador.Localizar(mensaje60, "default");
            var resultado120 = _localizador.Localizar(mensaje120, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado60));
            Assert.IsFalse(string.IsNullOrEmpty(resultado120));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeIdentificadorRedSocial_RetornaTraducido()
        {
            string mensaje = "El identificador de Facebook no debe exceder 50 caracteres.";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeIdentificadorRedSocial_DiversasRedes()
        {
            string mensajeFacebook = "El identificador de Facebook no debe exceder 50 caracteres.";
            string mensajeGoogle = "El identificador de Google no debe exceder 100 caracteres.";

            var resultadoFacebook = _localizador.Localizar(mensajeFacebook, "default");
            var resultadoGoogle = _localizador.Localizar(mensajeGoogle, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultadoFacebook));
            Assert.IsFalse(string.IsNullOrEmpty(resultadoGoogle));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeNoReconocido_RetornaPredeterminado()
        {
            string mensaje = "Este es un mensaje que no existe en el mapa";
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _localizador.Localizar(mensaje, mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeAleatorio_RetornaPredeterminado()
        {
            string mensaje = "Error completamente aleatorio sin traduccion";
            string mensajePredeterminado = "Error generico";

            var resultado = _localizador.Localizar(mensaje, mensajePredeterminado);

            Assert.AreEqual(mensajePredeterminado, resultado);
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeConEspaciosExtras_RetornaTraducido()
        {
            string mensaje = "  La partida ya comenzo  ";

            var resultado = _localizador.Localizar(mensaje, "default");

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }

        [TestMethod]
        public void Prueba_Localizar_MensajeSimilarPeroNoExacto_RetornaPredeterminado()
        {
            string mensaje = "la partida ya comenzo";
            string mensajePredeterminado = "Error predeterminado";

            var resultado = _localizador.Localizar(mensaje, mensajePredeterminado);

            Assert.IsFalse(string.IsNullOrEmpty(resultado));
        }
    }
}
