using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictionaryMusicalCliente.Utilidades;

namespace PictionaryMusicalCliente.Pruebas.Utilidades
{
    /// <summary>
    /// Contiene las pruebas unitarias para la clase ValidadorEntrada.
    /// Verifica el comportamiento de los metodos de validacion de entrada de usuario.
    /// </summary>
    [TestClass]
    public class ValidadorEntradaPruebas
    {
        [TestMethod]
        public void Prueba_ValidarUsuario_ValorValido_RetornaExito()
        {
            string usuario = "UsuarioValido";

            var resultado = ValidadorEntrada.ValidarUsuario(usuario);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarUsuario_ValorNulo_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarUsuario(null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
            Assert.IsNotNull(resultado.Mensaje);
        }

        [TestMethod]
        public void Prueba_ValidarUsuario_ValorVacio_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarUsuario("");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarUsuario_SoloEspacios_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarUsuario("   ");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNombre_ValorValido_RetornaExito()
        {
            string nombre = "Juan";

            var resultado = ValidadorEntrada.ValidarNombre(nombre);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNombre_ValorNulo_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarNombre(null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNombre_ValorVacio_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarNombre("");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarNombre_SoloEspacios_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarNombre("   ");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarApellido_ValorValido_RetornaExito()
        {
            string apellido = "Perez";

            var resultado = ValidadorEntrada.ValidarApellido(apellido);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarApellido_ValorNulo_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarApellido(null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarApellido_ValorVacio_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarApellido("");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarApellido_SoloEspacios_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarApellido("   ");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_FormatoValido_RetornaExito()
        {
            string correo = "usuario@ejemplo.com";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_FormatoValidoConSubdominio_RetornaExito()
        {
            string correo = "usuario@mail.ejemplo.com";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_ValorNulo_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarCorreo(null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_ValorVacio_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarCorreo("");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_SinArroba_RetornaFallo()
        {
            string correo = "usuarioejemplo.com";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_SinDominio_RetornaFallo()
        {
            string correo = "usuario@";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_SinExtension_RetornaFallo()
        {
            string correo = "usuario@ejemplo";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_ConEspacios_RetornaFallo()
        {
            string correo = "usuario @ejemplo.com";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarCorreo_MultipleArrobas_RetornaFallo()
        {
            string correo = "usuario@@ejemplo.com";

            var resultado = ValidadorEntrada.ValidarCorreo(correo);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_FormatoValido_RetornaExito()
        {
            string contrasena = "Contrasena1!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_ConVariosCaracteresEspeciales_RetornaExito()
        {
            string contrasena = "Abc123!@#";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_ValorNulo_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarContrasena(null);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_ValorVacio_RetornaFallo()
        {
            var resultado = ValidadorEntrada.ValidarContrasena("");

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_SinMayuscula_RetornaFallo()
        {
            string contrasena = "contrasena1!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_SinNumero_RetornaFallo()
        {
            string contrasena = "Contrasena!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_SinCaracterEspecial_RetornaFallo()
        {
            string contrasena = "Contrasena1";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_MuyCorta_RetornaFallo()
        {
            string contrasena = "Abc1!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_MuyLarga_RetornaFallo()
        {
            string contrasena = "ContrasenaMuyLarga123!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_ExactamenteOchoCaracteres_RetornaExito()
        {
            string contrasena = "Abcdef1!";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_ExactamenteQuinceCaracteres_RetornaExito()
        {
            string contrasena = "Abcdefghij123!@";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.OperacionExitosa);
        }

        [TestMethod]
        public void Prueba_ValidarContrasena_SoloEspacios_RetornaFallo()
        {
            string contrasena = "        ";

            var resultado = ValidadorEntrada.ValidarContrasena(contrasena);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.OperacionExitosa);
        }
    }
}
