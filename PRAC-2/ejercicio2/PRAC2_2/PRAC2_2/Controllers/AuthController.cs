using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MongoDB.Driver;

namespace NoSQLInjectionDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMongoCollection<dynamic> _usersCollection;

        public AuthController()
        {
            // Configuración de la conexión a MongoDB
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("NoSQLInjectionDB");
            _usersCollection = database.GetCollection<dynamic>("users");
        }
        [HttpGet]
        public ActionResult TestConnection()
        {
            try
            {
                // Probar conexión obteniendo las bases de datos del servidor
                var client = new MongoClient("mongodb://localhost:27017");
                var databases = client.ListDatabaseNames().ToList();

                // Obtener los usuarios de la colección "users"
                var users = _usersCollection.Find(FilterDefinition<dynamic>.Empty).ToList();

                // Construir el mensaje de respuesta
                var userList = string.Join("<br>", users.Select(u => $"Username: {u.username}, Password: {u.password}"));
                var response = $"Conexión exitosa. Bases de datos: {string.Join(", ", databases)}.<br>" +
                               $"Usuarios en 'users': {users.Count}.<br>" +
                               $"Contenido:<br>{userList}";

                return Content(response, "text/html");
            }
            catch (Exception ex)
            {
                return Content($"Error al conectar con MongoDB: {ex.Message}");
            }
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Convertir el valor de la contraseña a una cadena explícitamente
            var passwordString = password.ToString();

            // Crear el filtro como un documento BSON dinámico
            var filter = new MongoDB.Bson.BsonDocument
    {
        { "username", username },
        { "password", MongoDB.Bson.BsonValue.Create(passwordString) }
    };

            // Ejecutar la consulta en MongoDB
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Message = $"¡Bienvenido, {username}!";
            }
            else
            {
                ViewBag.Message = "Usuario o contraseña incorrectos.";
            }

            return View();
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Login2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login2(string username, string password)
        {
            // Convertir el valor de la contraseña a una cadena explícitamente
            var passwordString = password.ToString();

            // Crear el filtro como un documento BSON dinámico
            var filter = new MongoDB.Bson.BsonDocument
    {
        { "username", username },
        { "password", MongoDB.Bson.BsonValue.Create(passwordString) }
    };

            // Ejecutar la consulta en MongoDB
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                return Content($"¡Bienvenido, {username}!");
            }
            else
            {
                return Content("Usuario o contraseña incorrectos.");
            }
        }


        [HttpGet]
        public ActionResult Login3()
        {
            return View(); // Muestra la vista Login3.cshtml
        }

        [HttpPost]
        [ActionName("Login3")] // Hacer que esta acción responda a la misma URL que el método HttpGet Login3
        [ValidateInput(false)]
        public ActionResult Login3Post()
        {
            // Leer el cuerpo de la solicitud sin procesar
            string rawBody;
            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {
                rawBody = reader.ReadToEnd();
            }

            Console.WriteLine($"Raw Body: {rawBody}");

            // Analizar los datos enviados en formato URL-encoded
            var formData = System.Web.HttpUtility.ParseQueryString(rawBody);

            // Decodificar los valores para eliminar codificaciones URL-encoded
            var username = System.Web.HttpUtility.UrlDecode(formData["username"]);
            var password = System.Web.HttpUtility.UrlDecode(formData["password"]);


            Console.WriteLine($"Processed Username: {username}");
            Console.WriteLine($"Processed Password: {password}");

            // Crear el filtro para MongoDB
            var filter = new MongoDB.Bson.BsonDocument
    {
        { "username", username },
        { "password", password }
    };

            // Buscar el usuario en la base de datos
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Message = $"¡Bienvenido, {username}!";
                return View("Login3"); // Devuelve la vista Login3 con el mensaje de éxito
            }
            else
            {
                ViewBag.Message = "Usuario o contraseña incorrectos.";
                return View("Login3"); // Devuelve la vista Login3 con el mensaje de error
            }
        }


        [HttpGet]
        public ActionResult Login4()
        {
            return View(); // Devuelve la vista Login4.cshtml
        }

        [HttpPost]
        public ActionResult Login4(string username, string password)
        {
            MongoDB.Bson.BsonValue passwordValue;

            try
            {
                // Intentar parsear el password como un documento BSON
                passwordValue = MongoDB.Bson.BsonDocument.Parse(password);
            }
            catch (FormatException)
            {
                // Si falla, tratar el password como una cadena literal
                passwordValue = password;
            }

            // Construir el filtro
            var filter = new MongoDB.Bson.BsonDocument
    {
        { "username", username },
        { "password", passwordValue }
    };

            Console.WriteLine($"Filter: {filter}");

            // Ejecutar la consulta
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Message = $"¡Bienvenido, {username}!";
            }
            else
            {
                ViewBag.Message = "Usuario o contraseña incorrectos.";
            }

            return View("Login4"); // Devuelve la vista Login4 con el mensaje
        }

        [HttpGet]
        public ActionResult Login5()
        {
            return View(); // Devuelve la vista Login4.cshtml
        }

        [HttpPost]
        public ActionResult Login5(string username, string password)
        {

            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
            {
                ViewBag.Message = "La contraseña contiene caracteres inválidos.";
                return View("Login5");
            }

            MongoDB.Bson.BsonValue passwordValue;

            try
            {
                // Intentar parsear el password como un documento BSON
                passwordValue = MongoDB.Bson.BsonDocument.Parse(password);
            }
            catch (FormatException)
            {
                // Si falla, tratar el password como una cadena literal
                passwordValue = password;
            }

            // Construir el filtro
            var filter = new MongoDB.Bson.BsonDocument
    {
        { "username", username },
        { "password", passwordValue }
    };

            Console.WriteLine($"Filter: {filter}");

            // Ejecutar la consulta
            var user = _usersCollection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Message = $"¡Bienvenido, {username}!";
            }
            else
            {
                ViewBag.Message = "Usuario o contraseña incorrectos.";
            }

            return View("Login5"); // Devuelve la vista Login4 con el mensaje
        }

    }
}
