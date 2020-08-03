using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Server.Models;
using Server.Services;
using Server.Services.Builders;
using TTMLibrary.ModelViews;

namespace Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        IEntityService<Group, GroupModelView> _groupService;

        public GroupController(IEntityService<Group,GroupModelView> entityService)
        {
            _groupService = entityService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateGroup([ModelBinder(BinderType = typeof(JsonModelBinder))] GroupModelView modelView, IFormFile avatar = null)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            var group = await _groupService.Create(modelView, new GroupBuilder());

            if (group == null)
                return BadRequest();

            if (avatar != null)
                await ((IEntityFilesHandler)_groupService).SaveFile(avatar, newFileName: group.Id.ToString());

            return Ok(group);
        }

        [HttpGet]
        public async Task<ActionResult> GetGroup(Guid groupId)
        {
            var group = await _groupService.Get(groupId, new GroupBuilder());

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpGet("/api/[controller]/GetAvatar")]
        public async Task<ActionResult> GetAttachedFile(Guid id)
        {
            var (stream, contentType) = await ((IEntityFilesHandler)_groupService).GetFile(id.ToString());

            if (stream == null)
                return NotFound();

            return File(stream, contentType);
        }

        [HttpPost("api/[controller]/AddUser")]
        public async Task<ActionResult> AddUser() //inviteMV
        {
            return Ok();
        }
        
        [HttpPut]
        public async Task<ActionResult> UpdateGroup(GroupModelView modelview)
        {
            var group = await _groupService.Update(modelview);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteGroup(Guid id)
        {
            if (await _groupService.Delete(id, HttpContext.User.Identity.Name))
                return Ok();

            return NotFound();
        }

    }
}