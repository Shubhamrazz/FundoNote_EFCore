using DatabaseLayer.NoteModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NLogger.Interface;
using RepositoryLayer;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BussinessLayer.Interface;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using Newtonsoft.Json;
using RepositoryLayer.Services.Entities;

namespace FundoNote_EFCore.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class NoteController: ControllerBase
    {
        private readonly FundoContext fundocontext;
        private readonly INoteBL noteBL;
        private readonly ILoggerManager logger;
        private readonly IDistributedCache distributedCache;
        private readonly IMemoryCache memoryCache;

        public NoteController(FundoContext fundoContext,INoteBL noteBL,ILoggerManager logger, IDistributedCache distributedCache, IMemoryCache memoryCache)
        {
            this.fundocontext = fundoContext;
            this.noteBL = noteBL;
            this.logger = logger;
            this.distributedCache = distributedCache;
            this.memoryCache = memoryCache;

        }

        [HttpPost("AddNote")]
        public async Task<IActionResult>AddNote(NotePostModel notePostModel)
        {
            try
            {
                if (notePostModel.Title !="string" && notePostModel.Description != "string" && notePostModel.Bgcolor != "string")
                {
                    var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                    int UserId = int.Parse(userId.Value);
                    await this.noteBL.AddNote(UserId, notePostModel);
                    this.logger.LogInfo($"Note Created Successfully UserId={userId}");
                    return this.Ok(new { success = true, Message = "Note Created Successfully.." });
                }
                return this.BadRequest(new { success = false, message = "Entered Details are similar to Default one" });
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return this.BadRequest(new { success = false, message = "Entered Detail are same as Default one" });
        }

        [HttpGet("GetALlNotes")]
        public async Task<IActionResult> GetAllNotes()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var NoteData = await this.noteBL.GetAllNote(UserId);
                if (NoteData.Count == 0)
                {
                    this.logger.LogInfo($"No Notes Exists At Moment!! UserId = {UserId}");
                    return this.BadRequest(new { sucess = false, Message = "Currently You Don't Have Any Notes!!" });
                }

                this.logger.LogInfo($"All Notes Retrieved Successfully UserId = {UserId}");
                return this.Ok(new { sucess = true, Message = "Notes Data Retrieved successfully...", data = NoteData });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("UpdateNote")]
        public async Task<IActionResult> UpdateNote(int NoteId, NoteUpdateModel updateModel)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var check = this.fundocontext.Notes.Where(x => x.NoteId == NoteId).FirstOrDefault();
                if (check == null || check.IsTrash == true)
                {
                    return this.BadRequest(new { sucess = false, Message = $"No Note Found for NodeId : {NoteId}" });
                }

                if ((updateModel.Title == string.Empty) || (updateModel.Title == "string" && updateModel.Description == "string" && updateModel.Bgcolor == "string") && (updateModel.IsTrash == true))
                {
                    return this.BadRequest(new { sucess = false, Message = "Enter Valid Data" });
                }

                bool result = await this.noteBL.UpdateNote(UserId, NoteId, updateModel);
                if (result == true)
                {
                    return this.Ok(new { sucess = true, Message = "Note Updated Success Fully!!" });
                }

                return this.BadRequest(new { sucess = false, Message = $"No Note Found for NodeId : {NoteId}" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("TrashNote/{noteId}")]
        public async Task<IActionResult> TrashNode(int noteId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var check = this.fundocontext.Notes.Where(x => x.NoteId == noteId && x.UserId == UserId).FirstOrDefault();
                if (check == null)
                    return this.BadRequest(new { sucess = false, Message = $"Note Not Found" });

                bool result = await this.noteBL.DeleteNote(UserId, noteId);
                if (result)
                {
                    return this.Ok(new { sucess = true, Message = "Notes Deleted successfully..." });
                }

                return this.BadRequest(new { sucess = false, Message = $"Note not found for NoteId : {noteId}" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("IsArchive/{NoteId}")]
        public async Task<IActionResult> IsArchive(int NoteId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var res = this.fundocontext.Notes.Where(x => x.NoteId == NoteId).FirstOrDefault();
                if (res == null)
                {
                    return this.BadRequest(new { sucess = false, Message = "Note not Found" });

                }

                bool result = await this.noteBL.ArchiveNote(UserId, NoteId);
                if (result == true)
                {
                    return this.Ok(new { sucess = true, Message = "Note Archive SuccessFully !!" });
                }
                return this.Ok(new { sucess = true, Message = "Note UnArchive SuccessFully !!" });

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpPut("PinNote/{NoteId}")]
        public async Task<IActionResult> PinNote(int NoteId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var res = this.fundocontext.Notes.Where(x => x.NoteId == NoteId).FirstOrDefault();
                if (res == null)
                {
                    return this.BadRequest(new { sucess = false, Message = "Note not Found" });

                }

                bool result = await this.noteBL.PinNote(UserId, NoteId);
                if (result == true)
                {
                    return this.Ok(new { sucess = true, Message = "Note Pin SuccessFully !!" });
                }
                return this.Ok(new { sucess = true, Message = "Note UnPin SuccessFully !!" });

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpPut("Remainder")]
        public async Task<IActionResult> Remainder(int NoteId, NoteRemainderModel remainderModel)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                int UserId = int.Parse(userId.Value);
                var remainder = Convert.ToDateTime(remainderModel.Remainder);
                var res = this.fundocontext.Notes.Where(x => x.NoteId == NoteId).FirstOrDefault();
                if (res == null)
                {
                    return this.BadRequest(new { sucess = false, Message = "Note not Found" });

                }

                string result = await this.noteBL.Remainder(UserId, NoteId, remainder);
                if (result != null)
                {
                    return this.Ok(new { sucess = true, Message = "Remainder set SuccessFully !! ", data = result });
                }
                return this.Ok(new { sucess = true, Message = "Remainder Deleted SuccessFully !!" });

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                throw ex;
            }
        }

        [HttpGet("GetALlNotesUsingRedis")]
        public async Task<IActionResult> GetAllNotesUsingRedis()
        {
            try
            {
                var CacheKey = "NoteList";
                string SerializeNoteList;
                var notelist = new List<NoteResponseModel>();
                var redisnotelist = await distributedCache.GetAsync(CacheKey);
                if (redisnotelist != null)
                {
                    SerializeNoteList = Encoding.UTF8.GetString(redisnotelist);
                    notelist = JsonConvert.DeserializeObject<List<NoteResponseModel>>(SerializeNoteList);
                }
                else
                {
                    var userid = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("userId", StringComparison.InvariantCultureIgnoreCase));
                    int userId = Int32.Parse(userid.Value);
                     notelist = await this.noteBL.GetAllNote(userId);
                    SerializeNoteList = JsonConvert.SerializeObject(notelist);
                    redisnotelist = Encoding.UTF8.GetBytes(SerializeNoteList);
                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20)).SetAbsoluteExpiration(TimeSpan.FromHours(6));
                    await distributedCache.SetAsync(CacheKey, redisnotelist, option);
                }
                return this.Ok(new { success = true, message = $"Get Note Successful", data = notelist });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
