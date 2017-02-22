using System.Web.Http;
using System.Collections.Generic;
using Wask.Lib.Model;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using Wask.Lib.Sessions;

namespace Wask.Lib.SignalR
{
    /// <summary>
    /// Session OWIN Controller
    /// </summary>
    [RoutePrefix(Constants.SessionChannel)]
    public class SessionController : ApiController
    {
        private List<Session> _sessions = new List<Session>();

        public SessionController()
        {
        }

        /// <summary>
        /// Get all sessions
        /// </summary>
        [Route("")]
        [HttpGet]
        public IEnumerable<Session> Get()
        {
            return SessionDal.Sessions;
        }

        /// <summary>
        /// Add a session
        /// </summary>
        [Route("{name}")]
        [HttpPost]
        public IHttpActionResult Post(string name)
        {
            SessionDal.Sessions.Add(new Session() { name = name });
            SessionDal.Save();
            return Ok();
        }

        /// <summary>
        /// Delete a session
        /// </summary>
        [Route("{name}")]
        [HttpDelete]
        public IHttpActionResult Delete(string name)
        {
            int index = SessionDal.Sessions.FindIndex(o => o.name == name);
            SessionDal.Sessions.RemoveAt(index);
            SessionDal.Save();
            return Ok();
        }
    }
}
