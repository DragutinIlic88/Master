using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBankingWebApi.Helpers.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
	public class ApiPrefixRouteAttribute : Attribute, IRouteTemplateProvider
	{
		public string Name { get; set; }

		public int? Order => 2;

		public string Template { get; }

		public ApiPrefixRouteAttribute() {
			Template = "api/[controller]";
		}

		public ApiPrefixRouteAttribute(string routeSuffix)
		{
			Template = string.Format("api/{0}",routeSuffix);
		}

		public ApiPrefixRouteAttribute(string routeSuffix, string name) : this(routeSuffix)
		{
			Name = name;
		}
	}
}
