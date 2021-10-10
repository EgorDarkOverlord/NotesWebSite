using NotesWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebSite.ViewModels
{
    public class NotesViewModel
    {
        public List<NoteBase> Notes { get; set; }
        
        public void CheckDefaultValues()
        {
            Notes ??= new List<NoteBase>();

            foreach (var note in Notes)
            {
                note.Title ??= "Без названия";
            }
        }
    }
}
