using DatabaseLayer.NoteModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLayer.Interface
{
    public interface INoteBL
    {
        Task AddNote(int UserId, NotePostModel notePostModel);
        Task<List<NoteResponseModel>> GetAllNote(int UserId);
    }
}
