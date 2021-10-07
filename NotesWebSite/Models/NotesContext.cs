using Microsoft.EntityFrameworkCore;

namespace NotesWebSite.Models
{
    public class NotesContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<NoteBase> Notes { get; set; }
        public DbSet<UserNote> UserNotes { get; set; }
        public DbSet<LinkNote> LinkNotes { get; set; }

        public NotesContext(DbContextOptions<NotesContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
