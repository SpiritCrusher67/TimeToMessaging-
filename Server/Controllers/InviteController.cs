using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
    public class InviteController : ControllerBase
    {
        IEntityService<Invite, InviteModelView> _inviteService;
        public InviteController(IEntityService<Invite, InviteModelView> service)
        {
            _inviteService = service;
        }

        [HttpGet]
        public async Task<ActionResult> Get(Guid id)
        {
            var invite = await _inviteService.Get(id, new InviteBuilder());

            if (invite == null)
                return NotFound();

            return Ok(invite);
        }

        [HttpPost]
        public async Task<ActionResult> Create(InviteModelView modelView)
        {
            if (!ModelState.IsValid)
                return ValidationProblem();

            var invite = await _inviteService.Create(modelView, new InviteBuilder());

            if (invite == null)
                return BadRequest();

            return Ok(invite);
        }

        [HttpPost("/api/[controller]/Accept")]
        public async Task<ActionResult> Accept(Guid id)
        {
            if (!await (_inviteService as InviteService)?.Accept(id, HttpContext.User.Identity.Name))
                return BadRequest();

            await _inviteService.Delete(id, HttpContext.User.Identity.Name);

            return Ok();
        }

        [HttpPost("/api/[controller]/Reject")]
        public async Task<ActionResult> Reject(Guid id)
        {
            if (!await _inviteService.Delete(id, HttpContext.User.Identity.Name))
                return BadRequest();

            return Ok();
        }
}
}