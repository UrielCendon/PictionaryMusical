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
        public void Prueba_ObtenerCancionPorId_IdExistente_RetornaCancion()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_IdUno_RetornaGasolina()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            Assert.AreEqual("Gasolina", resultado.Nombre);
            Assert.AreEqual("Daddy Yankee", resultado.Artista);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_CancionEspanol_TieneIdiomaEspanol()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_CancionIngles_TieneIdiomaIngles()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(21);

            Assert.AreEqual("Ingles", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_RetornaCancionConTodosLosCampos()
        {
            Cancion resultado = _catalogo.ObtenerCancionPorId(1);

            Assert.IsTrue(resultado.Id > 0);
            Assert.IsFalse(string.IsNullOrEmpty(resultado.Nombre));
            Assert.IsFalse(string.IsNullOrEmpty(resultado.NombreNormalizado));
            Assert.IsFalse(string.IsNullOrEmpty(resultado.Artista));
            Assert.IsFalse(string.IsNullOrEmpty(resultado.Genero));
            Assert.IsFalse(string.IsNullOrEmpty(resultado.Idioma));
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_VariosIds_RetornaCancionesCorrectas()
        {
            Cancion cancion1 = _catalogo.ObtenerCancionPorId(1);
            Cancion cancion2 = _catalogo.ObtenerCancionPorId(2);
            Cancion cancion21 = _catalogo.ObtenerCancionPorId(21);

            Assert.AreEqual("Gasolina", cancion1.Nombre);
            Assert.AreEqual("Bocanada", cancion2.Nombre);
            Assert.AreEqual("Black Or White", cancion21.Nombre);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionPorId_MismoIdMultiplesLlamadas_RetornaMismaCancion()
        {
            Cancion resultado1 = _catalogo.ObtenerCancionPorId(5);
            Cancion resultado2 = _catalogo.ObtenerCancionPorId(5);

            Assert.AreEqual(resultado1.Id, resultado2.Id);
            Assert.AreEqual(resultado1.Nombre, resultado2.Nombre);
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
        public void Prueba_ObtenerCancionAleatoria_IdiomaEspanol_RetornaCancionEspanol()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", null);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaIngles_RetornaCancionIngles()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Ingles", null);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Ingles", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaEs_RetornaCancionEspanol()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("es", null);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Espanol", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaEn_RetornaCancionIngles()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("en", null);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Ingles", resultado.Idioma);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaMixto_RetornaCancion()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("mixto", null);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_SinExclusiones_RetornaCancion()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", new HashSet<int>());

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_ConExclusiones_NoRetornaExcluida()
        {
            var excluidos = new HashSet<int> { 1, 2, 3 };

            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", excluidos);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(excluidos.Contains(resultado.Id), 
                "La cancion retornada no debe estar en la lista de excluidos");
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MultiplesLlamadas_RetornaCancionesValidas()
        {
            for (int i = 0; i < 20; i++)
            {
                Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", null);
                
                Assert.IsNotNull(resultado);
                Assert.AreEqual("Espanol", resultado.Idioma);
            }
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_ConExclusionesNulo_FuncionaCorrectamente()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Ingles", null);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaConMayusculas_RetornaCancion()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("ESPANOL", null);

            Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_IdiomaConMinusculas_RetornaCancion()
        {
            Cancion resultado = _catalogo.ObtenerCancionAleatoria("espanol", null);

            Assert.IsNotNull(resultado);
        }

        #endregion

        #region ObtenerCancionAleatoria - Flujos Alternos

        [TestMethod]
        public void Prueba_ObtenerCancionAleatoria_MuchasExclusionesEspanol_RetornaCancionDisponible()
        {
            var excluidos = new HashSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Cancion resultado = _catalogo.ObtenerCancionAleatoria("Espanol", excluidos);

            Assert.IsNotNull(resultado);
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
            bool resultado = _catalogo.ValidarRespuesta(1, "Gasolina");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaMinusculas_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "gasolina");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaMayusculas_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "GASOLINA");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConEspacios_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(3, "La Nave Del Olvido");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConEspaciosExtras_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(3, "  La  Nave  Del  Olvido  ");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaSinAcentos_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(4, "Tiburon");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConAcentos_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(4, "Tiburón");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_CancionIngles_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(21, "Black Or White");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_CancionInglesMinusculas_RetornaTrue()
        {
            bool resultado = _catalogo.ValidarRespuesta(21, "black or white");

            Assert.IsTrue(resultado);
        }

        #endregion

        #region ValidarRespuesta - Flujos Alternos

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaIncorrecta_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "Otra Cancion");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaParcial_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "Gaso");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaVacia_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaNula_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, null);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaSoloEspacios_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "   ");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IdNoExistente_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(9999, "Cualquier Cosa");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IdCero_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(0, "Gasolina");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_IdNegativo_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(-1, "Gasolina");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaDeOtraCancion_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "Bocanada");

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_RespuestaConTextoExtra_RetornaFalse()
        {
            bool resultado = _catalogo.ValidarRespuesta(1, "Gasolina Remix");

            Assert.IsFalse(resultado);
        }

        #endregion

        #region Validacion de Normalizacion

        [TestMethod]
        public void Prueba_ValidarRespuesta_NombreConApostrofe_ValidaCorrectamente()
        {
            bool resultado = _catalogo.ValidarRespuesta(22, "Don't Stop The Music");

            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public void Prueba_ValidarRespuesta_NombreConSignos_ValidaCorrectamente()
        {
            bool resultado = _catalogo.ValidarRespuesta(23, "Man In The Mirror");

            Assert.IsTrue(resultado);
        }

        #endregion

        #region Consistencia del Catalogo

        [TestMethod]
        public void Prueba_Catalogo_TieneCancionesEspanol()
        {
            bool tieneEspanol = false;

            try
            {
                Cancion cancion = _catalogo.ObtenerCancionAleatoria("Espanol", null);
                tieneEspanol = cancion != null;
            }
            catch
            {
                tieneEspanol = false;
            }

            Assert.IsTrue(tieneEspanol, "El catalogo debe tener canciones en espanol");
        }

        [TestMethod]
        public void Prueba_Catalogo_TieneCancionesIngles()
        {
            bool tieneIngles = false;

            try
            {
                Cancion cancion = _catalogo.ObtenerCancionAleatoria("Ingles", null);
                tieneIngles = cancion != null;
            }
            catch
            {
                tieneIngles = false;
            }

            Assert.IsTrue(tieneIngles, "El catalogo debe tener canciones en ingles");
        }

        [TestMethod]
        public void Prueba_Catalogo_CancionesConsistentes_IdCorrespondeACancion()
        {
            for (int id = 1; id <= 10; id++)
            {
                Cancion cancion = _catalogo.ObtenerCancionPorId(id);
                Assert.AreEqual(id, cancion.Id, 
                    $"El ID de la cancion {cancion.Nombre} debe ser {id}");
            }
        }

        [TestMethod]
        public void Prueba_Catalogo_CancionesConNombreNormalizado()
        {
            Cancion cancion = _catalogo.ObtenerCancionPorId(1);

            Assert.IsNotNull(cancion.NombreNormalizado);
            Assert.IsFalse(string.IsNullOrWhiteSpace(cancion.NombreNormalizado));
        }

        #endregion
    }
}
