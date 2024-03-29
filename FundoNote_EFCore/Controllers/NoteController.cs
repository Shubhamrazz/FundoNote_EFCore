﻿using DatabaseLayer.NoteModels;
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

        public NoteController(FundoContext fundoContext,INoteBL noteBL,ILoggerManager logger)
        {
            this.fundocontext = fundoContext;
            this.noteBL = noteBL;
            this.logger = logger;
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
    }
}
