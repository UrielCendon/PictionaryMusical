using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalCliente.ClienteServicios;
using System;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase ServicioExcepcion.
    /// Verifica el comportamiento de la excepcion personalizada para servicios WCF.
    /// </summary>
    [TestClass]
    public class ServicioExcepcionPruebas
    {
        [TestMethod]
        public void Prueba_Constructor_ConTipoYMensaje_CreaInstancia()
        {
            var tipo = TipoErrorServicio.FallaServicio;
            string mensaje = "Error de servicio";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsNotNull(excepcion);
            Assert.AreEqual(tipo, excepcion.Tipo);
            Assert.AreEqual(mensaje, excepcion.Message);
        }

        [TestMethod]
        public void Prueba_Constructor_ConCausaInterna_IncluyeCausa()
        {
            var tipo = TipoErrorServicio.Comunicacion;
            string mensaje = "Error de comunicacion";
            var causaInterna = new InvalidOperationException("Operacion invalida");

            var excepcion = new ServicioExcepcion(tipo, mensaje, causaInterna);

            Assert.IsNotNull(excepcion);
            Assert.AreEqual(causaInterna, excepcion.InnerException);
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeNulo_CreaInstanciaSinMensaje()
        {
            var tipo = TipoErrorServicio.TiempoAgotado;

            var excepcion = new ServicioExcepcion(tipo, null);

            Assert.IsNotNull(excepcion);
            Assert.AreEqual(tipo, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeVacio_CreaInstancia()
        {
            var tipo = TipoErrorServicio.OperacionInvalida;
            string mensaje = "";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsNotNull(excepcion);
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeEspaciosBlanco_CreaInstancia()
        {
            var tipo = TipoErrorServicio.Desconocido;
            string mensaje = "   ";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsNotNull(excepcion);
        }

        [TestMethod]
        public void Prueba_Constructor_SinCausaInterna_InnerExceptionNulo()
        {
            var tipo = TipoErrorServicio.FallaServicio;
            string mensaje = "Error";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsNull(excepcion.InnerException);
        }

        [TestMethod]
        public void Prueba_Tipo_FallaServicio_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.FallaServicio, 
                "Error de servicio");

            Assert.AreEqual(TipoErrorServicio.FallaServicio, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Tipo_Comunicacion_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.Comunicacion, 
                "Error de red");

            Assert.AreEqual(TipoErrorServicio.Comunicacion, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Tipo_TiempoAgotado_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.TiempoAgotado, 
                "Tiempo agotado");

            Assert.AreEqual(TipoErrorServicio.TiempoAgotado, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Tipo_OperacionInvalida_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.OperacionInvalida, 
                "Operacion invalida");

            Assert.AreEqual(TipoErrorServicio.OperacionInvalida, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Tipo_Desconocido_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.Desconocido, 
                "Error desconocido");

            Assert.AreEqual(TipoErrorServicio.Desconocido, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Tipo_Ninguno_RetornaTipoCorrecto()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.Ninguno, 
                "Sin error");

            Assert.AreEqual(TipoErrorServicio.Ninguno, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_EsException_HeredaDeException()
        {
            var excepcion = new ServicioExcepcion(
                TipoErrorServicio.FallaServicio, 
                "Error");

            Assert.IsInstanceOfType(excepcion, typeof(Exception));
        }

        [TestMethod]
        public void Prueba_PuedeSerLanzada_YCapturada()
        {
            bool capturada = false;

            try
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.FallaServicio, 
                    "Error de prueba");
            }
            catch (ServicioExcepcion ex)
            {
                capturada = true;
                Assert.AreEqual(TipoErrorServicio.FallaServicio, ex.Tipo);
            }

            Assert.IsTrue(capturada);
        }

        [TestMethod]
        public void Prueba_PuedeSerCapturadaComoException()
        {
            bool capturada = false;

            try
            {
                throw new ServicioExcepcion(
                    TipoErrorServicio.Comunicacion, 
                    "Error de comunicacion");
            }
            catch (Exception ex)
            {
                capturada = true;
                Assert.IsInstanceOfType(ex, typeof(ServicioExcepcion));
            }

            Assert.IsTrue(capturada);
        }

        [TestMethod]
        public void Prueba_TipoErrorServicio_ContieneTodosLosValores()
        {
            Assert.AreEqual(0, (int)TipoErrorServicio.Ninguno);
            Assert.AreEqual(1, (int)TipoErrorServicio.FallaServicio);
            Assert.AreEqual(2, (int)TipoErrorServicio.Comunicacion);
            Assert.AreEqual(3, (int)TipoErrorServicio.TiempoAgotado);
            Assert.AreEqual(4, (int)TipoErrorServicio.OperacionInvalida);
            Assert.AreEqual(5, (int)TipoErrorServicio.Desconocido);
        }
    }
}
