using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extras.NLog;

namespace CQRS.Tests
{
	public static class Container
	{
		public static ContainerBuilder CreateBuilder()
		{
			var builder = new ContainerBuilder();

			builder.RegisterModule<NLogModule>();
			builder.RegisterModule(new CQRSModule(Assembly.GetExecutingAssembly()));

			return builder;
		}
	}
}