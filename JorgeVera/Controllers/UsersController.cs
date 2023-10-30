using JorgeVera.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JorgeVera.Controllers
{
    public class UsersController : Controller
    {

        private ControlUsuariosDbContext db = new ControlUsuariosDbContext();

        //// GET: Usuarios
        //public ActionResult Index()
        //{
        //    var usuarios = db.Users.ToList();
        //    return View(usuarios);
        //}

        public async Task<ActionResult> Index()
        {
            // URL de la API que deseas consultar
            string apiUrl = "https://localhost:7166/api/Users";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Realiza una solicitud GET a la API
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        // Lee el contenido de la respuesta en forma de cadena
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        // Deserializa la respuesta JSON en una lista de objetos
                        List<User> usuarios = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                        // Puedes ahora usar la lista de usuarios en tu vista
                        return View(usuarios);
                    }
                    else
                    {
                        // Manejo de errores en caso de que la solicitud a la API no sea exitosa
                        // Puedes mostrar un mensaje de error o realizar otras acciones apropiadas
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones en caso de problemas de conexión, etc.
                    // Puedes registrar el error, mostrar un mensaje de error, etc.
                }
            }

            // En caso de errores, puedes devolver una vista vacía o redirigir a otra página de error
            return View(new List<User>());
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Username,Rol, TipoUsuario, Area, Nombres, Apellido_Paterno, Apellido_Materno, Edad, Direccion")] User usuario)
        {
            if (ModelState.IsValid)
            {

                // Crear el hash de la contraseña
                //CreatePasswordHash(usuario.Password, out byte[] passwordHash, out byte[] passwordSalt);
                //usuario.passwordHash = passwordHash;
                //usuario.passwordSalt = passwordSalt;
                
                usuario.passwordHash = Encoding.UTF8.GetBytes("passwordHash");
                usuario.passwordSalt = Encoding.UTF8.GetBytes("passwordSalt");
                usuario.Activo = true;

                usuario.Usuario = usuario.Username;
                usuario.tokenCreated = DateTime.Now;
                usuario.tokenExpires = DateTime.Now.AddMinutes(60);
                db.Users.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }




    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User usuario = db.Users.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Username,Rol")] User usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User usuario = db.Users.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User usuario = db.Users.Find(id);
            db.Users.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Obtén el usuario con el ID especificado desde tu base de datos
            var usuario = ObtenerUsuarioPorId(id.Value); // Reemplaza esto con tu lógica real

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        private User ObtenerUsuarioPorId(int userId)
        {
            using (var context = new ControlUsuariosDbContext())
            {
                // Buscar el usuario por su ID en la base de datos
                var usuario = context.Users.Find(userId);

                // Si el usuario no se encuentra, retornar null
                return usuario;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}