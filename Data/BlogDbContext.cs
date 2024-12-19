using DAL.Configurations;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public partial class BlogDbContext : DbContext
    {
        public BlogDbContext()
        {
        }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<PostReaction> PostReactions { get; set; }
        public DbSet<Passkey> Passkeys { get; init; }
        public DbSet<UserWarning> UserWarnings { get; init; }
        public DbSet<UserBanLog> UserBans { get; init; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var assembly = typeof(ConfigurationsAssemblyMarker).Assembly;

            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}