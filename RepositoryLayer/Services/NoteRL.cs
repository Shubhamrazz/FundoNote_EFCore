using DatabaseLayer.NoteModels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Interface;
using RepositoryLayer.Services.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace RepositoryLayer.Services
{
    public class NoteRL : INoteRL
    {
        private readonly FundoContext fundoContext;
        private readonly IConfiguration configuration;

        public NoteRL(FundoContext fundooContext, IConfiguration configuration)
        {
            this.fundoContext = fundooContext;
            this.configuration = configuration;
        }

        public async Task AddNote(int UserId, NotePostModel notePostModel)
        {
            try
            {
                Note note = new Note();
                note.UserId = UserId;
                note.Title = notePostModel.Title;
                note.Description = notePostModel.Description;
                note.Bgcolor = notePostModel.Bgcolor;
                note.RegisteredDate = DateTime.Now;
                note.ModifiedDate = DateTime.Now;
                this.fundoContext.Notes.Add(note);
                await this.fundoContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NoteResponseModel>> GetAllNote(int UserId)
        {
            try
            {
                return await fundoContext.Users
               .Where(u => u.UserId == UserId)
               .Join(fundoContext.Notes,
               u => u.UserId,
               n => n.UserId,
               (u, n) => new NoteResponseModel
               {
                   NoteId = n.NoteId,
                   UserId = u.UserId,
                   Title = n.Title,
                   Description = n.Description,
                   Bgcolor = n.Bgcolor,
                   Firstname = u.Firstname,
                   Lasttname = u.Lastname,
                   Email = u.Email,
                   CreatedDate = u.CreateDate,
               }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
