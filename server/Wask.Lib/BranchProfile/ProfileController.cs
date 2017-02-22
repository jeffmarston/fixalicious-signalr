using System.Web.Http;
using System.Collections.Generic;
using Wask.Lib.Model;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using Wask.Lib.BranchProfile;

namespace Wask.Lib.SignalR
{
    /// <summary>
    /// Profile OWIN Controller
    /// </summary>
    [RoutePrefix(Constants.ServiceInfoChannel)]
    public class ProfileController : ApiController
    {
        private List<Profile> _profiles = new List<Profile>();

        public ProfileController()
        {
        }

        /// <summary>
        /// Get all profiles
        /// </summary>
        [Route("profile")]
        [HttpGet]
        public IEnumerable<Profile> GetProfiles()
        {
            return ProfileDal.Profiles;
        }

        /// <summary>
        /// Add a profile
        /// </summary>
        [Route("profile/{name}")]
        [HttpPost]
        public IHttpActionResult PostProfile(string name, string path)
        {
            ProfileDal.Profiles.Add(new Profile() { name = name, path = path });
            ProfileDal.Save();
            return Ok();
        }
    }
}
