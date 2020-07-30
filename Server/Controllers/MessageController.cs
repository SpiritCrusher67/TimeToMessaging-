using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class MessageController : ControllerBase
    {
        MessageService _messageService;
        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage([ModelBinder(BinderType = typeof(JsonModelBinder))] MessageModelView message, IFormFile attachedFile = null)
        {
            if (message.SenderLogin != HttpContext.User.Identity.Name)
                return BadRequest();

            var builder = attachedFile == null ? new TextMessageBuilder() : new FileMessageBuilder();
            message = await _messageService.CreateMessage(message, builder, attachedFile);

            if (message != null)
                return Ok(message);

            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> GetMessage(Guid messageId)
        {
            var message = await _messageService.GetMessage(messageId, new TextMessageBuilder());

            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpGet("api/[controller]/GetAttachedFile")]
        public async Task<ActionResult> GetAttachedFile(Guid messageId)
        {
            var (stream, contentType) = await _messageService.GetAttachedFile(messageId);

            if (stream == null)
                return NotFound();

            return File(stream, contentType);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(Guid messageId)
        {
            if (await _messageService.DeleteMessage(messageId, HttpContext.User.Identity.Name))
                return Ok();

            return NotFound();
        }

    }
}