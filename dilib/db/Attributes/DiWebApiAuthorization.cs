using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace dataislandcommon.Attributes
{
	public class DiWebApiAuthorization : AuthorizeAttribute
	{
		private List<string> _role = new List<string>();
		public string Role
		{
			get { return String.Join(",", _role.ToArray()); }
			set { _role = value.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries).ToList();
			_role.Add("all");
			}
		}

		protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			if (actionContext.RequestContext.Principal.Identity.AuthenticationType == "OAuth 2 Bearer")
			{
				if (actionContext.RequestContext.Principal.Identity.IsAuthenticated)
				{
					if (_role.Count == 0)
					{
						return true;
					}
                    if(actionContext.RequestContext.Principal.IsInRole("all"))
                    {
                        return true;
                    }
					foreach (string role in _role)
					{
						if (actionContext.RequestContext.Principal.IsInRole(role))
						{
							return true;
						}
					}
				}
			}
			actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
			return false;
		}

		protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var request = actionContext.Request;
			var response = actionContext.Response;
			var user = actionContext.RequestContext.Principal.Identity;

			if (user.IsAuthenticated == false)
				response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
			else
				response.StatusCode = System.Net.HttpStatusCode.Forbidden;

			base.HandleUnauthorizedRequest(actionContext);
		}

		
	}
}
