using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalServidor.Datos;
using PictionaryMusicalServidor.Datos.Entidades;
using PictionaryMusicalServidor.Datos.Excepciones;

namespace PictionaryMusicalServidor.Pruebas
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase CatalogoCanciones.
    /// Verifica flujos normales, alternos y de excepcion para la gestion del catalogo de canciones.
    /// </summary>
    [TestClass]
    public class CatalogoCancionesPruebas
    {
        private CatalogoCanciones _catalogo;

        /// <summary>
        /// Inicializa el catalogo antes de cada prueba.
        /// </summary>
        [TestInitialize]
        public void Inicializar()
        {
            _catalogo = new CatalogoCanciones();
        }

        /// <summary>
        /// Limpia los recursos despues de cada prueba.
        /// </summary>
        [TestCleanup]
        public void Limpiar()
        {
            _catalogo = null;
        }

        #region ObtenerCancionPorId - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdUno_RetornaGasolina()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            // Assert
            Assert.AreEqual("Gasolina", resultado.Nombre);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_CancionEspanol_TieneIdiomaEspanol()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            // Assert
            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_CancionIngles_TieneIdiomaIngles()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionPorId(21);

            // Assert
            Assert.AreEqual("Ingles", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_RetornaCancionConNombreNormalizadoNoVacio()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(resultado.NombreNormalizado));
        }

        #endregion

        #region ObtenerCancionPorId - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdNoExistente_LanzaKeyNotFoundException()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                _catalogo.ObtenerCancionPorId(9999);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdCero_LanzaKeyNotFoundException()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                _catalogo.ObtenerCancionPorId(0);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdNegativo_LanzaKeyNotFoundException()
        {
            Assert.ThrowsException<KeyNotFoundException>(() =>
            {
                _catalogo.ObtenerCancionPorId(-1);
            });
        }

        #endregion

        #region ObtenerCancionAleatoria - Flujos Normales

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaEspanol_RetornaCancionIdiomaEspanol()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", null);

            // Assert
            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaIngles_RetornaCancionIdiomaIngles()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Ingles", null);

            // Assert
            Assert.AreEqual("Ingles", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaEs_RetornaCancionIdiomaEspanol()
        {
            // Act
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("es", null);

            // Assert
            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_ConExclusiones_NoRetornaCancionExcluida()
        {
            // Arrange
            var excluidos = new HashSet<int> { 1, 2, 3 };

            // Act
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", excluidos);

            // Assert
            Assert.IsFalse(excluidos.Contains(resultado.Id));
        }

        #endregion

        #region ObtenerCancionAleatoria - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MuchasExclusionesEspanol_NoRetornaExcluidas()
        {
            // Arrange
            var excluidos = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", excluidos);

            // Assert
            Assert.IsFalse(excluidos.Contains(resultado.Id));
        }

        #endregion

        #region ObtenerCancionAleatoria - Flujos de Excepcion

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaNulo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _catalogo.ObtenerCancionAleatoria(null, null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaVacio_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _catalogo.ObtenerCancionAleatoria("", null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaSoloEspacios_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                _catalogo.ObtenerCancionAleatoria("   ", null);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_TodasExcluidasEspanol_LanzaCancionNoDisponibleExcepcion()
        {
            var excluidos = new HashSet<int>();
            for (int i = 1; i <= 20; i++)
            {
                excluidos.Add(i);
            }

            Assert.ThrowsException<CancionNoDisponibleExcepcion>(() =>
            {
                _catalogo.ObtenerCancionAleatoria("Espanol", excluidos);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_TodasExcluidasIngles_LanzaCancionNoDisponibleExcepcion()
        {
            var excluidos = new HashSet<int>();
            for (int i = 21; i <= 40; i++)
            {
                excluidos.Add(i);
            }

            Assert.ThrowsException<CancionNoDisponibleExcepcion>(() =>
            {
                _catalogo.ObtenerCancionAleatoria("Ingles", excluidos);
            });
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaInexistente_LanzaCancionNoDisponibleExcepcion()
        {
            Assert.ThrowsException<CancionNoDisponibleExcepcion>(() =>
            {
                _catalogo.ObtenerCancionAleatoria("Frances", null);
            });
        }

        #endregion

        #region ValidarRespuesta - Flujos Normales

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaExacta_RetornaTrue()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(1, "Gasolina");

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaMinusculas_RetornaTrue()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(1, "gasolina");

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConEspaciosExtras_RetornaTrue()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(3, "  La  Nave  Del  Olvido  ");

            // Assert
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConAcentos_RetornaTrue()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(4, "Tiburón");

            // Assert
            Assert.IsTrue(resultado);
        }

        #endregion

        #region ValidarRespuesta - Flujos Alternos

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaIncorrecta_RetornaFalse()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(1, "Otra Cancion");

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaNula_RetornaFalse()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(1, null);

            // Assert
            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IdNoExistente_RetornaFalse()
        {
            // Act
            bool resultado = _catalogo.ValidarRespuesta(9999, "Cualquier Cosa");

            // Assert
            Assert.IsFalse(resultado);
        }

        #endregion

        #region Consistencia del Catalogo

        [TestMethod]
        public void Prueba_Catalogo_CancionesConsistentes_IdCorrespondeACancion()
        {
            // Arrange & Act & Assert
            for (int id = 1; id <= 5; id++)
            {
                Cancion cancion = _catalogo.ObtenerCancionPorId(id);
                Assert.AreEqual(id, cancion.Id);
            }
        }

        #endregion
    }
}
