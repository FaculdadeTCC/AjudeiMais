using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AjudeiMais.Data.Context
{
    internal class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options){ }

        public DbSet<Usuario> Usuario { get; set; }
    }
}
