﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShareYourView.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class shareYourView_DBEntities : DbContext
    {
        public shareYourView_DBEntities()
            : base("name=shareYourView_DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ImageDetail> ImageDetails { get; set; }
        public virtual DbSet<ImageMetadata> ImageMetadatas { get; set; }
        public virtual DbSet<ImageShared> ImageShareds { get; set; }
        public virtual DbSet<ImageTag> ImageTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
    }
}
