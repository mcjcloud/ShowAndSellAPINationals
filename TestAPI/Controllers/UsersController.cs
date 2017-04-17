using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ShowAndSellAPI.Models.Database;
using ShowAndSellAPI.Models;
using System.IO;
using ShowAndSellAPI.Models.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowAndSellAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        private const string ADMIN_PASSWORD = "donut";
        private readonly SSDbContext _context;

        public UsersController(SSDbContext context)
        {
            _context = context;
        }

        // /api/users/allusers?devPassword={dev password}
        // GET all Users if the devPassword is correct.
        [HttpGet]
        public IActionResult AllUsers([FromQuery]string devPassword)
        {
            if(devPassword == ADMIN_PASSWORD)
            {
                IEnumerable<SSUser> users = _context.Users.ToArray();
                if (users != null) return new ObjectResult(users);
                else return NotFound("No Users found.");

            }
            else
            {
                return Unauthorized();
            }
        }
        // /api/users/search?name={name}
        // GET all Users (excluding passwords) matching the given name.
        [HttpGet]
        public IActionResult Search([FromQuery]string name)
        {
            IEnumerable<SSUser> matches = _context.Users.Where(e => (e.FirstName + e.LastName).Contains(name)).ToArray();

            if (matches != null && matches.Count() > 0)
            {
                foreach (var match in matches)
                {
                    match.Password = "";
                }

                // return the array of users
                return new ObjectResult(matches);
            }
            else    // matches is null (404)
            {
                return NotFound("No users containing the name " + name);
            }
        }
        // /api/users/userbyemail?email={email}&password={user password}
        // GET a User with the given email and password.
        [HttpGet]
        public IActionResult UserByEmail([FromQuery]string email, [FromQuery]string password)
        {
            SSUser match = _context.Users.Where(e => e.Email.Equals(email)).FirstOrDefault();
            // check if null
            if(match != null)
            {
                // authenticate
                if(match.Password.Equals(password))
                {
                    // return the user
                    return new ObjectResult(match);
                }
                else    // unauthorized
                {
                    return Unauthorized();
                }
            }
            else    // the user was not found.
            {
                return NotFound("User with email " + email + " not found.");
            }
        }
        // /api/users/googleuser?email={gmail email address}&userId={userId of user}&firstName={
        // POST user information for someone signing in from Google
        [HttpPost]
        public IActionResult GoogleUser([FromBody]GoogleUserModel user)
        {
            var existingUser = _context.Users.Where(e => e.Email.Equals(user.Email)).FirstOrDefault();
            if(existingUser == null)
            {
                // create the user
                SSUser newUser = new SSUser
                {
                    SSUserId = Guid.NewGuid().ToString(),
                    GroupId = "",
                    Email = user.Email,
                    Password = user.UserID,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };
                
                // add the user to the database
                _context.Users.Add(newUser);
                _context.SaveChanges();

                // return the user
                return new ObjectResult(newUser);
            }
            else
            {
                if (existingUser.Password.Equals(user.UserID))
                {
                    return new ObjectResult(existingUser);
                }
                else
                {
                    return StatusCode(409, "Account with email already exists.");
                }
            }
        }
        // /api/users/userbyuserid?id={user id}&password={user password}
        // GET a User with the given User ID and password.
        [HttpGet]
        public IActionResult UserByUserId([FromQuery]string id, [FromQuery]string password)
        {
            SSUser match = _context.Users.Where(e => e.SSUserId.Equals(id)).FirstOrDefault();
            // check if null
            if (match != null)
            {
                // authenticate
                if (match.Password.Equals(password))
                {
                    // return the user
                    return new ObjectResult(match);
                }
                else    // unauthorized
                {
                    return Unauthorized();
                }
            }
            else    // the user was not found.
            {
                return NotFound("User with User Id " + id + " not found.");
            }
        }

        // /api/users/create
        // POST a User to the server.
        [HttpPost]
        public IActionResult Create([FromBody]SSUser user)
        {
            bool fieldsFilled = (user.Password.Count() > 0 && user.FirstName.Count() > 0 && user.LastName.Count() > 0 && user.Email.Count() > 0);
            if (!fieldsFilled) return StatusCode(449, "Some fields are missing or invalid.");

            // check if email already exists
            foreach (var _user in _context.Users.ToArray())
            {
                if (_user.Email == user.Email) return StatusCode(449, "Email already in use."); 
            }

            // check if string has special characters
            var regex = new Regex("[a-zA-Z0-9]");
            if (!regex.IsMatch(user.Password)) return StatusCode(449, "Password cannot have special characters.");

            // check groupId
            if (user.GroupId != null && user.GroupId.Count() > 0)
            {
                // try to find the group
                SSGroup group = _context.Groups.Where(e => e.SSGroupId.Equals(user.GroupId)).FirstOrDefault();
                if (group == null) return NotFound("Group not found.");
            }

            user.SSUserId = Guid.NewGuid().ToString();
            _context.Users.Add(user);
            _context.SaveChanges();

            return new ObjectResult(user);
        }

        // /api/users/update?id={user id}
        // PUT a User with the updated data.
        [HttpPut]
        public IActionResult Update([FromQuery]string id, [FromBody]UpdateUserRequest updateRequest)
        {
            SSUser user = _context.Users.Where(e => e.SSUserId == id).FirstOrDefault();
            if (user == null) return NotFound("User with User ID " + id + " not found.");

            bool fieldsFilled = updateRequest.OldPassword.Count() > 0
                && updateRequest.NewFirstName.Count() > 0
                && updateRequest.NewLastName.Count() > 0
                && updateRequest.NewEmail.Count() > 0;

            // if any of the fields aren't filled
            if (!fieldsFilled)
            {
                return StatusCode(449, "All fields must be filled (see documentation)");
            }
            // check if email already exists
            foreach (var _user in _context.Users.ToArray())
            {
                if (_user.Email.Equals(updateRequest.NewEmail) && !_user.Email.Equals(user.Email)) return StatusCode(449, "Email already in use.");
            }

            // check password
            if (!updateRequest.OldPassword.Equals(user.Password)) return Unauthorized();

            // check if string has special characters
            var regex = new Regex("[a-zA-Z0-9]");
            if (!regex.IsMatch(updateRequest.NewPassword)) return StatusCode(449, "Password cannot have special characters.");

            // check groupId
            if (updateRequest.NewGroupId != null && updateRequest.NewGroupId.Count() > 0)
            {
                // try to find the group
                SSGroup group = _context.Groups.Where(e => e.SSGroupId.Equals(updateRequest.NewGroupId)).FirstOrDefault();
                if (group == null) return NotFound("Group not found.");
            }

            // update the userdata
            user.Password = (updateRequest.NewPassword.Count() > 0) ? updateRequest.NewPassword : updateRequest.OldPassword;       // only update the password if there is a new one
            user.FirstName = updateRequest.NewFirstName;
            user.LastName = updateRequest.NewLastName;
            user.Email = updateRequest.NewEmail;
            user.GroupId = updateRequest.NewGroupId;

            // save changes
            _context.Update(user);
            _context.SaveChanges();

            return new ObjectResult(user);
        }
        
        // /api/users/delete?id={user id}&password={user password}
        // DELETE a User from the server (deletes Group and Items in that group).
        [HttpDelete]
        public IActionResult Delete([FromQuery]string id, [FromQuery]string password)
        {
            SSUser userToDelete = _context.Users.Where(e => e.SSUserId == id).FirstOrDefault();
            if (userToDelete == null) return NotFound("The user could not be found.");
            if (userToDelete.Password != password) return Unauthorized();

            // check and delete a group that the user is admin of.
            SSGroup groupToDelete = _context.Groups.Where(e => e.AdminId == userToDelete.SSUserId).FirstOrDefault();
            if (groupToDelete != null)
            {
                // request group delete.
                DeleteGroup(groupToDelete);
            }

            _context.Remove(userToDelete);
            _context.SaveChanges();

            // return the deleted user
            return new ObjectResult(userToDelete);
        }

        /*
         * Helper methods
        */
        private void DeleteGroup(SSGroup group)
        {
            // first, remove all items in group.
            string groupId = group.SSGroupId;
            IEnumerable<SSItem> itemsToDelete = _context.Items.Where(e => e.GroupId.Equals(groupId)).ToArray();
            foreach(var item in itemsToDelete)
            {
                // remove bookmarks.
                foreach (var bookmark in _context.Bookmarks.Where(e => e.ItemId.Equals(item.SSItemId)).ToArray())
                {
                    _context.Remove(bookmark);
                }
                // remove chat
                foreach (var message in _context.Messages.Where(e => e.ItemId.Equals(item.SSItemId)).ToArray())
                {
                    _context.Remove(message);
                }

                // remove the full size image
                var imageToRemove = _context.Images.Where(e => e.ItemId.Equals(item.SSItemId)).FirstOrDefault();
                imageToRemove.Thumbnail = "";
                _context.Remove(imageToRemove);

                _context.Remove(item);
            }

            // delete the group.
            _context.Remove(group);
        }
    }
}
