using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ShowAndSellAPI.Models.Database;
using ShowAndSellAPI.Models;
using ShowAndSellAPI.Models.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowAndSellAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ChatController : Controller
    {
        public readonly SSDbContext _context;

        public ChatController(SSDbContext context)
        {
            _context = context;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Messages([FromQuery]string itemId)
        {
            SSItem item = _context.Items.Where(e => e.SSItemId.Equals(itemId)).FirstOrDefault();
            if (item == null) return NotFound("Item with ID " + itemId + " not found.");

            // get group the item is in (for AdminId/name)
            SSGroup group = _context.Groups.Where(e => e.SSGroupId.Equals(item.GroupId)).FirstOrDefault();
            if (group == null) return NotFound("Item's group not found.");

            // get Admin
            SSUser admin = _context.Users.Where(e => e.SSUserId == group.AdminId).FirstOrDefault();

            IEnumerable<SSMessage> messages = _context.Messages.Where(e => e.ItemId.Equals(item.SSItemId)).ToArray();
            if (messages == null || messages.Count() <= 0) return NotFound("No Messages associated with the given Item.");

            GetMessageResponse[] responses = new GetMessageResponse[messages.Count()];
            for(int i = 0; i < responses.Count(); i++)
            {
                SSMessage message = messages.ElementAt(i);
                SSUser poster = _context.Users.Where(e => e.SSUserId.Equals(message.PosterId)).FirstOrDefault();
                responses[i] = new GetMessageResponse
                {
                    SSMessageId = message.SSMessageId,
                    ItemId = message.ItemId,
                    PosterId = message.PosterId,
                    PosterName = poster.FirstName + " " + poster.LastName,
                    AdminId = admin.SSUserId,
                    AdminName = admin.FirstName + " " + admin.LastName,
                    DatePosted = message.DatePosted,
                    Body = message.Body
                };
            }

            // return the array of messages.
            return new ObjectResult(responses);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Create([FromQuery]string posterId, [FromQuery]string password, [FromBody]AddMessageRequest request)
        {
            SSUser poster = _context.Users.Where(e => e.SSUserId.Equals(posterId)).FirstOrDefault();
            if (poster == null) return NotFound("User with ID " + posterId + " not found.");

            // Authenticate
            if (!poster.Password.Equals(password)) return Unauthorized();

            SSItem item = _context.Items.Where(e => e.SSItemId.Equals(request.ItemId)).FirstOrDefault();
            if (item == null) return NotFound("Item not found.");

            // check body size
            string body = request.Body;
            if (body.Length <= 0) return StatusCode(449, "Body must not be empty.");

            SSMessage message = new SSMessage
            {
                SSMessageId = Guid.NewGuid().ToString(),
                ItemId = request.ItemId,
                PosterId = poster.SSUserId,
                Body = body,
                DatePosted = DateTime.Now.ToString()
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            // return the added message
            return new ObjectResult(message);
        }

        // DELETE api/values/5
        [HttpDelete]
        public IActionResult Delete([FromQuery]string id)
        {
            SSMessage messageToDelete = _context.Messages.Where(e => e.SSMessageId.Equals(id)).FirstOrDefault();
            if (messageToDelete == null) return NotFound("Message with ID " + id + " not found.");

            _context.Remove(messageToDelete);
            _context.SaveChanges();

            // return the deleted message
            return new ObjectResult(messageToDelete);
        }
    }
}
