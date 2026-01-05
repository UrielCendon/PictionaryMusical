using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Servicios.Contratos.DTOs;
using PictionaryMusicalServidor.Servicios.Servicios.Utilidades;
using System;
using System.ServiceModel;

namespace PictionaryMusicalServidor.Pruebas.Servicios.Utilidades
{
    [TestClass]
    public class EntradaComunValidadorPruebas
    {
        private const string TextoVacio = "";
        private const string TextoNulo = null;
        private const string TextoConEspacios = "   ";
        private const string TextoValido = "TextoValido";
        private const string TextoConEspaciosAlrededores = "   TextoValido   ";
        private const string TextoExcedeLongitud = 
            "TextoQueExcedeLaLongitudMaximaPermitidaDe50CaracteresEnElSistema";
        private const int LongitudMaximaTextoEsperada = 50;
        private const int LongitudMaximaReporteEsperada = 100;
        private const int LongitudCodigoVerificacionEsperada = 6;
        private const int LongitudCodigoSalaEsperada = 6;
        private const int LongitudMaximaMensajeChatEsperada = 150;

        private const string CorreoValido = "usuario@ejemplo.com";
        private const string CorreoInvalidoSinArroba = "usuarioejemplo.com";
        private const string CorreoInvalidoSinDominio = "usuario@";
        private const string CorreoInvalidoEspacios = "usuario @ejemplo.com";
        private const string CorreoInvalidoCaracteres = "usuario<>@ejemplo.com";

        private const string ContrasenaValida = "Password1!";
        private const string ContrasenaSinMayuscula = "password1!";
        private const string ContrasenaSinNumero = "Password!!";
        private const string ContrasenaSinEspecial = "Password12";
        private const string ContrasenaCorta = "Pass1!";
        private const string ContrasenaLarga = "PasswordMuyLarga1!";

        private const string TokenValido = "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6";
        private const string TokenInvalidoCorto = "a1b2c3d4";
        private const string TokenInvalidoLargo = "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6e7f8";
        private const string TokenInvalidoCaracteres = "g1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6";

        private const string CodigoVerificacionValido = "123456";
        private const string CodigoVerificacionInvalidoCorto = "12345";
        private const string CodigoVerificacionInvalidoLargo = "1234567";
        private const string CodigoVerificacionInvalidoLetras = "12345a";

        private const string CodigoSalaValido = "123456";
        private const string CodigoSalaInvalidoCorto = "12345";
        private const string CodigoSalaInvalidoLargo = "1234567";
        private const string CodigoSalaInvalidoLetras = "12345a";

        private const string MensajeValido = "Mensaje de prueba";
        private const string MensajeExcedeLimite = 
            "Este es un mensaje de prueba que excede el limite maximo de caracteres " +
            "permitido en el chat del sistema y por lo tanto deberia fallar la validacion " +
            "cuando se intente enviar este mensaje tan largo";

        private const string IdiomaValido = "espanol";
        private const string DificultadValida = "media";
        private const string NombreUsuarioValido = "Usuario123";
        private const string NombreParametro = "nombreUsuario";

        private const int NumeroRondasValido = 3;
        private const int NumeroRondasInvalidoCero = 0;
        private const int NumeroRondasInvalidoNegativo = -1;
        private const int NumeroRondasInvalidoExcede = 5;
        private const int TiempoRondaValido = 60;
        private const int TiempoRondaInvalidoCero = 0;
        private const int TiempoRondaInvalidoNegativo = -1;
        private const int TiempoRondaInvalidoExcede = 150;

        private const string IdSalaValido = "sala123";
        private const string IdJugadorValido = "jugador123";
        private const int IdUsuarioValido = 1;
        private const int IdUsuarioInvalido = 0;
        private const int IdUsuarioNegativo = -1;
        private const int AvatarIdValido = 1;
        private const int AvatarIdInvalido = 0;

        private const string MotivoReporteValido = "Comportamiento inapropiado";
        private const string MotivoReporteExcedeLongitud =
            "Este es un motivo de reporte extremadamente largo que excede el limite maximo " +
            "de caracteres permitido para un motivo de reporte en el sistema";

        private const string NombreValido = "Juan";
        private const string ApellidoValido = "Perez";

        #region Pruebas NormalizarTexto

        [TestMethod]
        public void Prueba_NormalizarTexto_TextoNulo_RetornaNulo()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoNulo);

            Assert.AreEqual(TextoNulo, resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_TextoVacio_RetornaNulo()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoVacio);

            Assert.AreEqual(TextoNulo, resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_TextoSoloEspacios_RetornaNulo()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoConEspacios);

            Assert.AreEqual(TextoNulo, resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_TextoConEspaciosAlrededor_RetornaTextoSinEspacios()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoConEspaciosAlrededores);

            Assert.AreEqual(TextoValido, resultado);
        }

        [TestMethod]
        public void Prueba_NormalizarTexto_TextoValidoSinEspacios_RetornaMismoTexto()
        {
            string resultado = EntradaComunValidador.NormalizarTexto(TextoValido);

            Assert.AreEqual(TextoValido, resultado);
        }

        #endregion

        #region Pruebas EsLongitudValida

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoSoloEspacios_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoConEspacios);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoExcedeLongitudMaxima_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoExcedeLongitud);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsLongitudValida(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValida_TextoExactamente50Caracteres_RetornaVerdadero()
        {
            string textoExacto = new string('a', LongitudMaximaTextoEsperada);

            bool resultado = EntradaComunValidador.EsLongitudValida(textoExacto);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsLongitudValidaReporte

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_TextoNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_TextoVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_TextoExcedeLongitud_RetornaFalso()
        {
            string textoLargo = new string('a', LongitudMaximaReporteEsperada + 1);

            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(textoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_TextoValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(TextoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsLongitudValidaReporte_TextoExactamente100Caracteres_RetornaVerdadero()
        {
            string textoExacto = new string('a', LongitudMaximaReporteEsperada);

            bool resultado = EntradaComunValidador.EsLongitudValidaReporte(textoExacto);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsCorreoValido

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoSinArroba_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(CorreoInvalidoSinArroba);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoSinDominio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(CorreoInvalidoSinDominio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsCorreoValido(CorreoValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsCorreoValido_CorreoExcedeLongitud_RetornaFalso()
        {
            string correoLargo = new string('a', LongitudMaximaTextoEsperada) + "@test.com";

            bool resultado = EntradaComunValidador.EsCorreoValido(correoLargo);

            Assert.IsFalse(resultado);
        }

        #endregion

        #region Pruebas EsContrasenaValida

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaNula_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaVacia_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaSoloEspacios_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(TextoConEspacios);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaSinMayuscula_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinMayuscula);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaSinNumero_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinNumero);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaSinCaracterEspecial_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaSinEspecial);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaDemasiadoCorta_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaCorta);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaDemasiadoLarga_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaLarga);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsContrasenaValida_ContrasenaValida_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsContrasenaValida(ContrasenaValida);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsTokenValido

        [TestMethod]
        public void Prueba_EsTokenValido_TokenNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_TokenVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_TokenDemasiadoCorto_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TokenInvalidoCorto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_TokenDemasiadoLargo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TokenInvalidoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsTokenValido_TokenValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsTokenValido(TokenValido);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsCodigoVerificacionValido

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoDemasiadoCorto_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionInvalidoCorto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoDemasiadoLargo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionInvalidoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoConLetras_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionInvalidoLetras);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoVerificacionValido_CodigoValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsCodigoVerificacionValido(
                CodigoVerificacionValido);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsCodigoSalaValido

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoDemasiadoCorto_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaInvalidoCorto);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoDemasiadoLargo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaInvalidoLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoConLetras_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaInvalidoLetras);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsCodigoSalaValido_CodigoValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsCodigoSalaValido(CodigoSalaValido);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsMensajeValido

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeSoloEspacios_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(TextoConEspacios);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeExcedeLimite_RetornaFalso()
        {
            string mensajeLargo = new string('a', LongitudMaximaMensajeChatEsperada + 1);

            bool resultado = EntradaComunValidador.EsMensajeValido(mensajeLargo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsMensajeValido(MensajeValido);

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_EsMensajeValido_MensajeExactamente150Caracteres_RetornaVerdadero()
        {
            string mensajeExacto = new string('a', LongitudMaximaMensajeChatEsperada);

            bool resultado = EntradaComunValidador.EsMensajeValido(mensajeExacto);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsIdiomaValido

        [TestMethod]
        public void Prueba_EsIdiomaValido_IdiomaNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsIdiomaValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsIdiomaValido_IdiomaVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsIdiomaValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsIdiomaValido_IdiomaValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsIdiomaValido(IdiomaValido);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsDificultadValida

        [TestMethod]
        public void Prueba_EsDificultadValida_DificultadNula_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsDificultadValida(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsDificultadValida_DificultadVacia_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsDificultadValida(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsDificultadValida_DificultadValida_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsDificultadValida(DificultadValida);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas EsNombreUsuarioValido

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_NombreNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_NombreVacio_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(TextoVacio);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_NombreExcedeLongitud_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(TextoExcedeLongitud);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_EsNombreUsuarioValido_NombreValido_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.EsNombreUsuarioValido(NombreUsuarioValido);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas ValidarNombreUsuario

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_NombreNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarNombreUsuario(TextoNulo, NombreParametro));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_NombreVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarNombreUsuario(TextoVacio, NombreParametro));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_NombreExcedeLongitud_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarNombreUsuario(TextoExcedeLongitud, NombreParametro));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuario_NombreValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarNombreUsuario(NombreUsuarioValido, NombreParametro);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ObtenerNombreUsuarioNormalizado

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_AmbosNulos_RetornaNulo()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                TextoNulo, TextoNulo);

            Assert.AreEqual(TextoNulo, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_PrincipalValido_RetornaPrincipal()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                NombreUsuarioValido, TextoValido);

            Assert.AreEqual(NombreUsuarioValido, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_PrincipalNulo_RetornaAlterno()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                TextoNulo, NombreUsuarioValido);

            Assert.AreEqual(NombreUsuarioValido, resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerNombreUsuarioNormalizado_PrincipalConEspacios_RetornaNormalizado()
        {
            string resultado = EntradaComunValidador.ObtenerNombreUsuarioNormalizado(
                TextoConEspaciosAlrededores, TextoValido);

            Assert.AreEqual(TextoValido, resultado);
        }

        #endregion

        #region Pruebas ValidarNuevaCuenta

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_CuentaNula_RetornaFallo()
        {
            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_UsuarioVacio_RetornaFallo()
        {
            var cuenta = CrearNuevaCuentaValida();
            cuenta.Usuario = TextoVacio;

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_NombreVacio_RetornaFallo()
        {
            var cuenta = CrearNuevaCuentaValida();
            cuenta.Nombre = TextoVacio;

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_ApellidoVacio_RetornaFallo()
        {
            var cuenta = CrearNuevaCuentaValida();
            cuenta.Apellido = TextoVacio;

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_CorreoInvalido_RetornaFallo()
        {
            var cuenta = CrearNuevaCuentaValida();
            cuenta.Correo = CorreoInvalidoSinArroba;

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_ContrasenaInvalida_RetornaFallo()
        {
            var cuenta = CrearNuevaCuentaValida();
            cuenta.Contrasena = ContrasenaSinMayuscula;

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNuevaCuenta_DatosValidos_RetornaExito()
        {
            var cuenta = CrearNuevaCuentaValida();

            ResultadoOperacionDTO resultado = EntradaComunValidador.ValidarNuevaCuenta(cuenta);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        #endregion

        #region Pruebas ValidarActualizacionPerfil

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_SolicitudNula_RetornaFallo()
        {
            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(null);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_UsuarioIdCero_RetornaFallo()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.UsuarioId = IdUsuarioInvalido;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_AvatarIdInvalido_RetornaFallo()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.AvatarId = AvatarIdInvalido;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_NombreVacio_RetornaFallo()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.Nombre = TextoVacio;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_ApellidoVacio_RetornaFallo()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.Apellido = TextoVacio;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_InstagramExcedeLongitud_RetornaFallo()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.Instagram = TextoExcedeLongitud;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_DatosValidos_RetornaExito()
        {
            var solicitud = CrearActualizacionPerfilValida();

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarActualizacionPerfil_RedesSocialesNulas_RetornaExito()
        {
            var solicitud = CrearActualizacionPerfilValida();
            solicitud.Instagram = null;
            solicitud.Facebook = null;
            solicitud.X = null;
            solicitud.Discord = null;

            ResultadoOperacionDTO resultado = EntradaComunValidador
                .ValidarActualizacionPerfil(solicitud);

            Assert.IsTrue(resultado.OperacionExitosa);
        }

        #endregion

        #region Pruebas ValidarConfiguracionPartida

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_ConfiguracionNula_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(null));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_NumeroRondasCero_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.NumeroRondas = NumeroRondasInvalidoCero;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_NumeroRondasNegativo_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.NumeroRondas = NumeroRondasInvalidoNegativo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_NumeroRondasExcede_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.NumeroRondas = NumeroRondasInvalidoExcede;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_TiempoRondaCero_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.TiempoPorRondaSegundos = TiempoRondaInvalidoCero;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_TiempoRondaNegativo_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.TiempoPorRondaSegundos = TiempoRondaInvalidoNegativo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_TiempoRondaExcede_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.TiempoPorRondaSegundos = TiempoRondaInvalidoExcede;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_IdiomaVacio_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.IdiomaCanciones = TextoVacio;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_DificultadVacia_LanzaFaultException()
        {
            var configuracion = CrearConfiguracionPartidaValida();
            configuracion.Dificultad = TextoVacio;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarConfiguracionPartida(configuracion));
        }

        [TestMethod]
        public void Prueba_ValidarConfiguracionPartida_ConfiguracionValida_NoLanzaExcepcion()
        {
            var configuracion = CrearConfiguracionPartidaValida();

            EntradaComunValidador.ValidarConfiguracionPartida(configuracion);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarEntradaSalaChat

        [TestMethod]
        public void Prueba_ValidarEntradaSalaChat_NombreJugadorNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarEntradaSalaChat(CodigoSalaValido, TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarEntradaSalaChat_CodigoSalaInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarEntradaSalaChat(
                    CodigoSalaInvalidoCorto, NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ValidarEntradaSalaChat_DatosValidos_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarEntradaSalaChat(CodigoSalaValido, NombreUsuarioValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarCodigoSala

        [TestMethod]
        public void Prueba_ValidarCodigoSala_CodigoInvalido_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarCodigoSala(CodigoSalaInvalidoCorto));
        }

        [TestMethod]
        public void Prueba_ValidarCodigoSala_CodigoValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarCodigoSala(CodigoSalaValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarIdSala

        [TestMethod]
        public void Prueba_ValidarIdSala_IdNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarIdSala(TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarIdSala_IdVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarIdSala(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarIdSala_IdValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarIdSala(IdSalaValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarIdJugador

        [TestMethod]
        public void Prueba_ValidarIdJugador_IdNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarIdJugador(TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarIdJugador_IdVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarIdJugador(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarIdJugador_IdValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarIdJugador(IdJugadorValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarSuscripcionJugador

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_SuscripcionNula_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarSuscripcionJugador(null));
        }

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_IdSalaNulo_LanzaFaultException()
        {
            var suscripcion = CrearSuscripcionJugadorValida();
            suscripcion.IdSala = TextoNulo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarSuscripcionJugador(suscripcion));
        }

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_IdJugadorNulo_LanzaFaultException()
        {
            var suscripcion = CrearSuscripcionJugadorValida();
            suscripcion.IdJugador = TextoNulo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarSuscripcionJugador(suscripcion));
        }

        [TestMethod]
        public void Prueba_ValidarSuscripcionJugador_SuscripcionValida_NoLanzaExcepcion()
        {
            var suscripcion = CrearSuscripcionJugadorValida();

            EntradaComunValidador.ValidarSuscripcionJugador(suscripcion);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas SuperaLimiteCaracteresMensaje

        [TestMethod]
        public void Prueba_SuperaLimiteCaracteresMensaje_MensajeNulo_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.SuperaLimiteCaracteresMensaje(TextoNulo);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_SuperaLimiteCaracteresMensaje_MensajeCorto_RetornaFalso()
        {
            bool resultado = EntradaComunValidador.SuperaLimiteCaracteresMensaje(MensajeValido);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_SuperaLimiteCaracteresMensaje_MensajeExcedeLimite_RetornaVerdadero()
        {
            bool resultado = EntradaComunValidador.SuperaLimiteCaracteresMensaje(MensajeExcedeLimite);

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Pruebas ValidarMensajeJuego

        [TestMethod]
        public void Prueba_ValidarMensajeJuego_IdSalaNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarMensajeJuego(MensajeValido, TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarMensajeJuego_MensajeExcedeLimite_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarMensajeJuego(MensajeExcedeLimite, IdSalaValido));
        }

        [TestMethod]
        public void Prueba_ValidarMensajeJuego_DatosValidos_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarMensajeJuego(MensajeValido, IdSalaValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarReporteJugador

        [TestMethod]
        public void Prueba_ValidarReporteJugador_ReporteNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarReporteJugador(null));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_NombreReportanteNulo_LanzaFaultException()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.NombreUsuarioReportante = TextoNulo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_NombreReportadoNulo_LanzaFaultException()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.NombreUsuarioReportado = TextoNulo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_MotivoNulo_LanzaFaultException()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.Motivo = TextoNulo;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_MotivoExcedeLongitud_LanzaFaultException()
        {
            var reporte = CrearReporteJugadorValido();
            reporte.Motivo = MotivoReporteExcedeLongitud;

            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarReporteJugador(reporte));
        }

        [TestMethod]
        public void Prueba_ValidarReporteJugador_ReporteValido_NoLanzaExcepcion()
        {
            var reporte = CrearReporteJugadorValido();

            EntradaComunValidador.ValidarReporteJugador(reporte);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarInvitacionSala

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_InvitacionNula_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarInvitacionSala(null));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_CodigoSalaInvalido_LanzaArgumentException()
        {
            var invitacion = CrearInvitacionSalaValida();
            invitacion.CodigoSala = CodigoSalaInvalidoCorto;

            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarInvitacionSala(invitacion));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_CorreoVacio_LanzaArgumentException()
        {
            var invitacion = CrearInvitacionSalaValida();
            invitacion.Correo = TextoVacio;

            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarInvitacionSala(invitacion));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_CorreoInvalido_LanzaArgumentException()
        {
            var invitacion = CrearInvitacionSalaValida();
            invitacion.Correo = CorreoInvalidoSinArroba;

            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarInvitacionSala(invitacion));
        }

        [TestMethod]
        public void Prueba_ValidarInvitacionSala_InvitacionValida_NoLanzaExcepcion()
        {
            var invitacion = CrearInvitacionSalaValida();

            EntradaComunValidador.ValidarInvitacionSala(invitacion);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarIdUsuario

        [TestMethod]
        public void Prueba_ValidarIdUsuario_IdCero_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarIdUsuario(IdUsuarioInvalido));
        }

        [TestMethod]
        public void Prueba_ValidarIdUsuario_IdNegativo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                EntradaComunValidador.ValidarIdUsuario(IdUsuarioNegativo));
        }

        [TestMethod]
        public void Prueba_ValidarIdUsuario_IdValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarIdUsuario(IdUsuarioValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarNombreUsuarioSuscripcion

        [TestMethod]
        public void Prueba_ValidarNombreUsuarioSuscripcion_NombreNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarNombreUsuarioSuscripcion(TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuarioSuscripcion_NombreVacio_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarNombreUsuarioSuscripcion(TextoVacio));
        }

        [TestMethod]
        public void Prueba_ValidarNombreUsuarioSuscripcion_NombreValido_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarNombreUsuarioSuscripcion(NombreUsuarioValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Pruebas ValidarUsuariosInteraccion

        [TestMethod]
        public void Prueba_ValidarUsuariosInteraccion_UsuarioANulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarUsuariosInteraccion(TextoNulo, NombreUsuarioValido));
        }

        [TestMethod]
        public void Prueba_ValidarUsuariosInteraccion_UsuarioBNulo_LanzaFaultException()
        {
            Assert.ThrowsException<FaultException>(() =>
                EntradaComunValidador.ValidarUsuariosInteraccion(NombreUsuarioValido, TextoNulo));
        }

        [TestMethod]
        public void Prueba_ValidarUsuariosInteraccion_AmbosValidos_NoLanzaExcepcion()
        {
            EntradaComunValidador.ValidarUsuariosInteraccion(
                NombreUsuarioValido, NombreUsuarioValido);

            Assert.IsTrue(true);
        }

        #endregion

        #region Metodos Auxiliares

        private NuevaCuentaDTO CrearNuevaCuentaValida()
        {
            return new NuevaCuentaDTO
            {
                Usuario = NombreUsuarioValido,
                Nombre = NombreValido,
                Apellido = ApellidoValido,
                Correo = CorreoValido,
                Contrasena = ContrasenaValida,
                AvatarId = AvatarIdValido
            };
        }

        private ActualizacionPerfilDTO CrearActualizacionPerfilValida()
        {
            return new ActualizacionPerfilDTO
            {
                UsuarioId = IdUsuarioValido,
                Nombre = NombreValido,
                Apellido = ApellidoValido,
                AvatarId = AvatarIdValido,
                Instagram = TextoValido,
                Facebook = TextoValido,
                X = TextoValido,
                Discord = TextoValido
            };
        }

        private ConfiguracionPartidaDTO CrearConfiguracionPartidaValida()
        {
            return new ConfiguracionPartidaDTO
            {
                NumeroRondas = NumeroRondasValido,
                TiempoPorRondaSegundos = TiempoRondaValido,
                IdiomaCanciones = IdiomaValido,
                Dificultad = DificultadValida
            };
        }

        private SuscripcionJugadorDTO CrearSuscripcionJugadorValida()
        {
            return new SuscripcionJugadorDTO
            {
                IdSala = IdSalaValido,
                IdJugador = IdJugadorValido,
                NombreUsuario = NombreUsuarioValido,
                EsHost = false
            };
        }

        private ReporteJugadorDTO CrearReporteJugadorValido()
        {
            return new ReporteJugadorDTO
            {
                NombreUsuarioReportante = NombreUsuarioValido,
                NombreUsuarioReportado = NombreValido,
                Motivo = MotivoReporteValido
            };
        }

        private InvitacionSalaDTO CrearInvitacionSalaValida()
        {
            return new InvitacionSalaDTO
            {
                CodigoSala = CodigoSalaValido,
                Correo = CorreoValido,
                Idioma = IdiomaValido
            };
        }

        #endregion
    }
}
