using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShowAndSellAPI.Models.Database;
using ShowAndSellAPI.Models;
using ShowAndSellAPI.Models.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowAndSellAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BookmarksController : Controller
    {
        // Properties
        private readonly SSDbContext _context;

        // CONSTRUCTOR
        public BookmarksController(SSDbContext context)
        {
            _context = context;
        }

        // /api/bookmarks/bookmarks?userId={user id}&password={user password}
        // GET Bookmarks with the given User ID and password.
        [HttpGet]
        public IActionResult Bookmarks([FromQuery]string userId, [FromQuery]string password)
        {
            // get bookmarks
            IEnumerable<SSBookmark> bookmarks = _context.Bookmarks.Where(e => e.UserId == userId).ToArray();
            if (bookmarks == null || bookmarks.Count() < 1) return NotFound("Bookmarks for User with ID " + userId + " not found.");

            // get the bookmarked items
            GetBookmarkResponse[] responses = new GetBookmarkResponse[bookmarks.Count()];
            for (int i = 0; i < bookmarks.Count(); i++)
            {
                SSItem item = _context.Items.Where(e => e.SSItemId == bookmarks.ElementAt(i).ItemId).FirstOrDefault();
                //return new ObjectResult(item);
                responses[i] = new GetBookmarkResponse
                {
                    BookmarkId = bookmarks.ElementAt(i).SSBookmarkId,
                    Item = item
                };
            }

            // return the responses
            return new ObjectResult(responses);
        }

        // /api/bookmarks/create?userId={user id}&itemId={id of item to bookmark}
        // POST a new bookmark.
        [HttpPost]
        public IActionResult Create([FromQuery]string userId, [FromQuery]string itemId)
        {
            // check user
            SSUser user = _context.Users.Where(e => e.SSUserId.Equals(userId)).FirstOrDefault();
            if (user == null) return NotFound("User with ID " + userId + " not found.");

            // check if user has already bookmarked item
            foreach (var bookmark in _context.Bookmarks.ToArray())
            {
                if (bookmark.ItemId.Equals(itemId) && bookmark.UserId.Equals(userId))
                {
                    return StatusCode(409, "Bookmark already exists for this user.");
                }
            }

            SSItem bookmarkedItem = _context.Items.Where(e => e.SSItemId == itemId).FirstOrDefault();
            SSBookmark bookmarkToAdd;
            if (bookmarkedItem != null)
            {
                bookmarkToAdd = new SSBookmark
                {
                    SSBookmarkId = Guid.NewGuid().ToString(),
                    ItemId = bookmarkedItem.SSItemId,
                    UserId = userId
                };
            }
            else
            {
                return NotFound("Item with id " + itemId + " not found.");
            }
            _context.Bookmarks.Add(bookmarkToAdd);
            _context.SaveChanges();

            // return the bookmarked item.
            return new ObjectResult(bookmarkToAdd);
        }

        // /api/bookmarks/delete?id={bookmark id}
        // DELETE a Bookmark.
        [HttpDelete]
        public IActionResult Delete([FromQuery]string id)
        {
            SSBookmark bookmark = _context.Bookmarks.Where(e => e.SSBookmarkId.Equals(id)).FirstOrDefault();
            if (bookmark == null) return NotFound("Bookmark with ID " + id + " not found.");

            _context.Remove(bookmark);
            _context.SaveChanges();

            // return the deleted bookmark.
            return new ObjectResult(bookmark);
        }
    }
}
