﻿using DAL.Configurations;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public partial class BlogDbContext : DbContext
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public BlogDbContext()
        {

        }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
            Database.EnsureCreated();
            //_httpContextAccessor = httpContextAccessor;
        }


        public DbSet<UserEntity>? Users { get; set; }

        public DbSet<PostEntity>? Posts { get; set; }

        public DbSet<CommentEntity>? Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var assembly = typeof(ConfigurationsAssemblyMarker).Assembly;

            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
