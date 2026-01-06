using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalCliente.ClienteServicios;
using System;
using System.Linq;

namespace PictionaryMusicalCliente.Pruebas.ClienteServicios
{
    [TestClass]
    public class ServicioExcepcionPruebas
    {
        [TestMethod]
        public void Prueba_Constructor_ConTipoYMensaje_CreaInstancia()
        {
            var tipo = TipoErrorServicio.FallaServicio;
            string mensaje = "Error de servicio";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.AreEqual(tipo, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Constructor_ConCausaInterna_IncluyeCausa()
        {
            var tipo = TipoErrorServicio.Comunicacion;
            string mensaje = "Error de comunicacion";
            var causaInterna = new InvalidOperationException("Operacion invalida");

            var excepcion = new ServicioExcepcion(tipo, mensaje, causaInterna);

            Assert.AreEqual(causaInterna, excepcion.InnerException);
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeNulo_CreaInstanciaSinMensaje()
        {
            var tipo = TipoErrorServicio.TiempoAgotado;

            var excepcion = new ServicioExcepcion(tipo, null);

            Assert.AreEqual(tipo, excepcion.Tipo);
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeVacio_CreaInstancia()
        {
            var tipo = TipoErrorServicio.OperacionInvalida;
            string mensaje = "";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsInstanceOfType(excepcion, typeof(ServicioExcepcion));
        }

        [TestMethod]
        public void Prueba_Constructor_MensajeEspaciosBlanco_CreaInstancia()
        {
            var tipo = TipoErrorServicio.Desconocido;
            string mensaje = "   ";

            var excepcion = new ServicioExcepcion(tipo, mensaje);

            Assert.IsInstanceOfType(excepcion, typeof(ServicioExcepcion));
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
            var valores = Enum.GetValues(typeof(TipoErrorServicio)).Cast<TipoErrorServicio>().ToList();
            
            valores.Should().HaveCount(6);
        }
    }
}
