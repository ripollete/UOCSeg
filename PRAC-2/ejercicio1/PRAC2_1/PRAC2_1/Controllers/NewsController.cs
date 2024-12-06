using PRAC2_1;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace YourNamespace.Controllers
{
    public class NewsController : Controller
    {
        private readonly string connectionString = "Server=localhost;Database=PRAC2_DB;User Id=servicioWEB;Password=StrongPassword@123;";

        [Route("news-blind-sql-injection/{id}")] // Ruta explícita para pasar el ID
        public ActionResult NewsBlindSqlInjection(string id)
        {
            var newsContent = new System.Text.StringBuilder();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT Title, Body FROM News WHERE id = {id}";
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ViewBag.Title = reader["Title"];
                            newsContent.Append($"<p>{reader["Body"]}</p>");
                        }
                        else
                        {
                            newsContent.Append("<p>No se encontró ninguna noticia con el ID proporcionado.</p>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                newsContent.Append($"<p>Error: {ex.Message}</p>");
            }

            ViewBag.NewsContent = newsContent.ToString();
            return View();
        }

        [Route("NewsBlindSqlInjectionSol1/{id}")]
        public ActionResult NewsBlindSqlInjectionSol1(string id)
        {
            var newsContent = new System.Text.StringBuilder();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Title, Body FROM News WHERE id = @Id";
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Validamos manualmente que el parámetro `id` sea convertible a entero
                        if (int.TryParse(id, out int idValue))
                        {
                            command.Parameters.AddWithValue("@Id", idValue);

                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    ViewBag.Title = reader["Title"];
                                    newsContent.Append($"<p>{reader["Body"]}</p>");
                                }
                                else
                                {
                                    newsContent.Append("<p>No se encontró ninguna noticia con el ID proporcionado.</p>");
                                }
                            }
                        }
                        else
                        {
                            newsContent.Append("<p>Formato de ID inválido.</p>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                newsContent.Append($"<p>Error: {ex.Message}</p>");
            }

            ViewBag.NewsContent = newsContent.ToString();
            return View();
        }

        [Route("NewsBlindSqlInjectionSol2/{id}")]
        public ActionResult NewsBlindSqlInjectionSol2(int id)
        {
            using (var context = new PRAC2_DBEntities())
            {
                var news = context.News.Where(n => n.id == id).FirstOrDefault();

                if (news != null)
                {
                    ViewBag.Title = news.Title;
                    ViewBag.NewsContent = $"<p>{news.Body}</p>";
                }
                else
                {
                    ViewBag.Title = "No encontrado";
                    ViewBag.NewsContent = "<p>No se encontró ninguna noticia con el ID proporcionado.</p>";
                }
            }

            return View();
        }


    }
}
