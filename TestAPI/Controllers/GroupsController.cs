using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ShowAndSellAPI.Models.Database;
using ShowAndSellAPI.Models;
using ShowAndSellAPI.Models.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowAndSellAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class GroupsController : Controller
    {
        // Properties
        private readonly SSDbContext _context;

        // CONSTRUCTOR
        public GroupsController(SSDbContext context)
        {
            _context = context;
        }

        // /api/groups/allgroups
        // GET all Groups on the server.
        [HttpGet]
        public IActionResult AllGroups()
        {
            IEnumerable<SSGroup> groups = _context.Groups.ToArray();
            if (groups != null && groups.Count() > 0)
            {
                foreach(var group in groups)
                {
                    group.Routing = "";
                }
                return new ObjectResult(groups);
            }
            else return NotFound("No Groups found.");
        }
        // /api/groups/group?id={group id}
        // GET a group with the given Group ID.
        [HttpGet]
        public IActionResult Group([FromQuery]string id)
        {
            SSGroup group = _context.Groups.Where(e => e.SSGroupId.Equals(id)).FirstOrDefault();
            if(group != null)
            {
                group.Routing = "";
                return new ObjectResult(group);
            }
            else    // group not found
            {
                return NotFound("Group with id " + id + " not found.");
            }
        }
        // /api/groups/groupwithadmin?adminId={id of group admin}
        // GET the group with the given user as Admin
        [HttpGet]
        public IActionResult GroupWithAdmin([FromQuery]string adminId)
        {
            SSGroup group = _context.Groups.Where(e => e.AdminId.Equals(adminId)).FirstOrDefault();
            if (group == null) return NotFound("Group not found with Admin ID " + adminId);
            else {
                group.Routing = "";
                return new ObjectResult(group);
            }
        }
        // /api/groups/search
        // GET an array of Groups whose names contain the given string.
        [HttpGet]
        public IActionResult Search([FromQuery]string name)
        {
            IEnumerable<SSGroup> groups = _context.Groups.Where(e => e.Name.Contains(name)).ToArray();
            if (groups != null && groups.Count() > 0) {
                foreach(var group in groups) {
                    group.Routing = "";
                }
                return new ObjectResult(groups);
            }
            else return NotFound("No groups containing the name " + name + " found.");
        }

        // /api/groups/groupsinradius?radius={(float) radius in miles)}&latitude={(double) latitude of source}&longitude={(double) longitude of source}
        // GET all of the groups within a certain radius.
        [HttpGet]
        public IActionResult GroupsInRadius([FromQuery]float radius, [FromQuery]double latitude, [FromQuery]double longitude)
        {
            // convert to radians.
            latitude = latitude * (Math.PI / 180);          // 32.8984211013027 ->  0.5741857669
            longitude = longitude * (Math.PI / 180);        // -97.2801185120803 -> -1.697858365

            // conversion
            double latConst = 69.172;
            double latMiles = latitude * latConst;                          // 39.71757787
            double longMiles = Math.Cos(latitude) * latConst * longitude;   // -98.61029058

            Debug.WriteLine("cos(lat): " + Math.Cos(latitude));
            Debug.WriteLine("latMiles: " + latMiles + " longMiles: " + longMiles);

            // assemble a list of the groups
            Dictionary<double, SSGroup> groupMap = new Dictionary<double, SSGroup>();
            foreach(var group in _context.Groups.ToArray())
            {
                // convert to radians.
                var groupLat = group.Latitude * (Math.PI / 180);    // 34.6081320802352     ->  0.6040258528
                var groupLong = group.Longitude * (Math.PI / 180);  // -98.0928189586848    ->  -1.712042663

                // lat/long to miles
                double groupLatMiles = groupLat * latConst;                             // 41.78167629
                double groupLongMiles = Math.Cos(groupLat) * latConst * groupLong;      // -97.47072064

                Debug.WriteLine("cos(groupLat): " + Math.Cos(groupLat));
                Debug.WriteLine("groupLat: " + groupLat);
                Debug.WriteLine("lat: " + latitude);
                Debug.WriteLine("groupLat: " + groupLatMiles + " groupLong: " + groupLongMiles);

                // pythagorean therom for distance between.
                double distance = Math.Abs(Math.Sqrt(Math.Pow(Math.Abs(groupLatMiles - latMiles), 2) + Math.Pow(Math.Abs(groupLongMiles - longMiles), 2)));
                Debug.WriteLine("distance from (" + latitude + ", " + longitude + ") to (" + groupLat + ", " + groupLong + "): " + distance);
                // if the distance is <= the radius, append to the list.
                if(distance <= radius)
                {
                    groupMap.Add(distance, group);
                }
            }

            // if the list is empty, return NotFound (404)
            if (groupMap.Values.Count() <= 0) return NotFound("No groups in given radius found.");

            // sort groups
            List<double> keysSorted = groupMap.Keys.OrderBy(dist => dist).ToList();

            // order groups
            List<SSGroup> groupsInRadius = new List<SSGroup>();
            foreach(var key in keysSorted)
            {
                SSGroup value;
                groupMap.TryGetValue(key, out value);
                if (value != null) {
                    value.Routing = "";
                    groupsInRadius.Add(value);
                }
            }

            // return the list
            return new ObjectResult(groupsInRadius);
        }

        // /api/groups/closestgroups?n={number of groups to return}&latitude={latitude of source}&longitude={longitude of source}
        // GET n closest groups to coordinates
        [HttpGet]
        public IActionResult ClosestGroups([FromQuery]int n, [FromQuery]double latitude, [FromQuery]double longitude)
        {
            // conversion
            double latConst = 69.172;
            double latMiles = latitude * latConst;
            double longMiles = (Math.Cos(latitude) * latConst) * longitude;

            IEnumerable<SSGroup> groups = _context.Groups.ToArray();
            if(n >= groups.Count())
            {
                return new ObjectResult(groups);
            }
            else
            {
                // Hash groups with keys by distance.
                Dictionary<double, SSGroup> groupMap = new Dictionary<double, SSGroup>();
                foreach(var group in _context.Groups.ToArray())
                {
                    // lat/long to miles
                    double groupLatMiles = group.Latitude * latConst;
                    double groupLongMiles = (Math.Cos(group.Latitude) * latConst) * group.Longitude;

                    // pythagorean therom for distance between.
                    double distance = Math.Abs(Math.Sqrt(Math.Pow((groupLatMiles - latMiles), 2) + Math.Pow((groupLongMiles - longMiles), 2)));

                    // add the group to the dic
                    groupMap.Add(distance, group);
                }
                List<SSGroup> closestGroups = new List<SSGroup>();
                List<double> keysSorted = groupMap.Keys.OrderBy(dist => dist).ToList();
                for(int i = 0; i < n; i++)
                {
                    SSGroup group;
                    groupMap.TryGetValue(keysSorted.ElementAt(i), out group);
                    if(group != null) {
                        group.Routing = "";
                        closestGroups.Add(group);
                    }
                }

                return new ObjectResult(closestGroups);
            }
        }

        // /api/groups/create
        // POST a new Group to the server.
        [HttpPost]
        public IActionResult Create([FromBody]AddGroupRequest groupRequest)
        {
            if (groupRequest == null) return BadRequest("Missing or invalid body");

            // if no admin was specified.
            // bool is if there is insufficient data entered.
            bool invalidRequest = groupRequest.Group.AdminId == null ||
                groupRequest.Group.AdminId == "" ||
                groupRequest.Group.Routing == null ||
                groupRequest.Group.Routing == "" ||
                groupRequest.Group.Name == null ||
                groupRequest.Group.Name == "" ||
                groupRequest.Password == null ||
                groupRequest.Group.LocationDetail == "" ||
                groupRequest.Group.LocationDetail == null;

            if (invalidRequest) return StatusCode(449, "Some fields missing or invalid.");

            // check if user exists
            SSUser admin = _context.Users.Where(e => e.SSUserId == groupRequest.Group.AdminId).FirstOrDefault();
            if (admin == null) return NotFound("Error creating group. Admin with ID " + groupRequest.Group.AdminId + " not found.");

            // check password
            string realPassword = admin.Password;
            if (groupRequest.Password != realPassword) return Unauthorized();

            // check if group name is taken, or if admin already has a group, or if lat/long is taken.
            foreach (var group in _context.Groups.ToArray())
            {
                if (group.AdminId == admin.SSUserId) return BadRequest("Group under admin " + admin.Email + " already exists.");
                if (group.Name == groupRequest.Group.Name) return BadRequest("Group with name " + groupRequest.Group.Name + " already exists.");
            }

            // add the group to the database.
            groupRequest.Group.SSGroupId = Guid.NewGuid().ToString();
            groupRequest.Group.DateCreated = DateTime.Now;
            groupRequest.Group.Rating = 0.0f;
            _context.Groups.Add(groupRequest.Group);
            _context.SaveChanges();

            return new ObjectResult(groupRequest.Group);
        }

        [HttpPost]
        public IActionResult RateGroup([FromQuery]string id, [FromQuery]int rating, [FromQuery]string userId, [FromQuery]string password)
        {
            SSGroup group = _context.Groups.Where(e => e.SSGroupId.Equals(id)).FirstOrDefault();
            if (group == null) return NotFound("Group not found.");

            SSUser user = _context.Users.Where(e => e.SSUserId.Equals(userId)).FirstOrDefault();
            if (user == null) return NotFound("User not found");
            if (!user.Password.Equals(password)) return Unauthorized();

            float sum = 0.0f;
            bool exists = false;
            foreach(var rat in _context.Ratings)
            {
                if (rat.GroupId.Equals(id))
                {
                    if (rat.UserId.Equals(user.SSUserId))
                    {
                        // change the user's rating
                        rat.Rating = rating;
                        exists = true;
                    }
                    sum += rat.Rating;
                }
            }

            if(!exists)
            {
                // create new Ratings
                _context.Ratings.Add(new SSRating {
                    SSRatingId = Guid.NewGuid().ToString(),
                    UserId = userId,
                    GroupId = id,
                    Rating = rating
                });
                _context.SaveChanges();
                sum += rating;
            }

            Debug.WriteLine("rating: " + rating + " sum: " + sum + " Max(1, count): " + Math.Max(1, _context.Ratings.Count()) + " as array: " + Math.Max(1, _context.Ratings.ToArray().Count()) + " new rating: " + (sum / Math.Max(1, _context.Ratings.Count())));
            float newRating = sum / Math.Max(1, _context.Ratings.Count());
            group.Rating = newRating;

            // save and return
            _context.SaveChanges();
            return new ObjectResult(group.Rating);
        }

        // /api/groups/update?id={group id}
        // PUT a Group in the server with the new data.
        [HttpPut]
        public IActionResult Update([FromQuery]string id, [FromBody]UpdateGroupRequest groupRequest)
        {
            SSGroup groupToUpdate = _context.Groups.Where(e => e.SSGroupId == id).FirstOrDefault();
            SSUser admin = _context.Users.Where(e => e.SSUserId == groupToUpdate.AdminId).FirstOrDefault();
            if (groupToUpdate == null) return NotFound("Group not found.");
            if (admin == null) return NotFound("Admin not found.");

            // authenticate/authorize
            if (admin.Password != groupRequest.Password) return Unauthorized();

            // check if group name is taken, or if admin already has a group.
            foreach (SSGroup group in _context.Groups.ToArray())
            {
                if (group.Name == groupRequest.NewName && groupRequest.NewName != groupToUpdate.Name) return BadRequest("Group with name " + groupRequest.NewName + " already exists.");
            }

            // check if fields filled
            bool valid = groupRequest.NewLocationDetail.Count() > 0 && groupRequest.NewName.Count() > 0;

            if (!valid) return StatusCode(449, "Some fields missing or invalid.");

            groupToUpdate.Name = groupRequest.NewName;
            groupToUpdate.LocationDetail = groupRequest.NewLocationDetail;
            groupToUpdate.Address = groupRequest.NewAddress;
            groupToUpdate.Routing = groupRequest.NewRouting;
            groupToUpdate.Latitude = groupRequest.NewLatitude;
            groupToUpdate.Longitude = groupRequest.NewLongitude;
            _context.Update(groupToUpdate);
            _context.SaveChanges();
            return new ObjectResult(groupToUpdate);
        }

        // /api/groups/delete?id={group id}&password={admin password}
        // DELETE a Group from the server.
        [HttpDelete]
        public IActionResult Delete([FromQuery]string id, [FromQuery]string password)
        {
            SSGroup groupToDelete = _context.Groups.Where(e => e.SSGroupId == id).FirstOrDefault();
            if (groupToDelete == null) return NotFound("Group not found.");

            SSUser admin = _context.Users.Where(e => e.SSUserId == groupToDelete.AdminId).FirstOrDefault();
            if (admin == null) return NotFound("Admin not found.");

            // if not authorized, return 401
            if (admin.Password != password) return Unauthorized();

            // check for items to delete.
            IList<SSItem> items = _context.Items.Where(e => e.GroupId == groupToDelete.SSGroupId).ToList();
            foreach (SSItem item in items)
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
                // remove groupId from user
                foreach (var user in _context.Users.Where(e => e.GroupId.Equals(groupToDelete.SSGroupId)))
                {
                    user.GroupId = "";
                }

                // remove the full size image
                var imageToRemove = _context.Images.Where(e => e.ItemId.Equals(item.SSItemId)).FirstOrDefault();
                imageToRemove.Thumbnail = "";
                _context.Remove(imageToRemove);

                _context.Remove(item);
            }

            // remove the group
            _context.Remove(groupToDelete);
            _context.SaveChanges();
            // returned the removed group
            return new ObjectResult(groupToDelete);
        }

    }
}
