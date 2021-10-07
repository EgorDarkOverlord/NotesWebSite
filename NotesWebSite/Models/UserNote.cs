using System;

namespace NotesWebSite.Models
{
    public class UserNote : NoteBase
    {
        public Guid UserId { get; set; }
    }
}
