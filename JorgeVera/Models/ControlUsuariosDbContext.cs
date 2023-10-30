using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JorgeVera.Models
{
    public class ControlUsuariosDbContext : DbContext
    {

        public ControlUsuariosDbContext() : base("CadenaDeConexion")
        {

            // Agrega código de diagnóstico para registrar la conexión
            Database.Connection.StateChange += (sender, e) =>
            {
                Console.WriteLine($"Estado de la conexión a SQL Server: {e.CurrentState}");
            };
        }


        public DbSet<User> Users { get; set; }
        


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Configuraciones adicionales de modelos si es necesario
    }

    }

}