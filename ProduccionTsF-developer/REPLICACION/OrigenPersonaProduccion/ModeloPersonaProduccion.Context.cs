﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrigenPersonaProduccion
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBContextoPersonaProduccion : DbContext
    {
        public DBContextoPersonaProduccion()
            : base("name=DBContextoPersonaProduccion")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<IdentificationType> IdentificationType { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonType> PersonType { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
    }
}
