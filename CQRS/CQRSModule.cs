using System;
using System.Reflection;
using Autofac;
using CQRS.Bus.Command;
using CQRS.Bus.Event;
using CQRS.Bus.Query;
using CQRS.Command;
using CQRS.Event;
using CQRS.Query;
using Module = Autofac.Module;

namespace CQRS
{
	public class CQRSModule : Module
	{
		private Assembly assembly;

		public CQRSModule(Assembly assembly = null)
		{
			this.assembly = assembly == null ? ThisAssembly : assembly;
		}

		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			RegisterCommands(builder);
			RegisterQueries(builder);
			RegisterEvents(builder);
		}

		private void RegisterEvents(ContainerBuilder builder)
		{
			builder.RegisterType<EventBus>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterAssemblyTypes(assembly)
				.AsClosedTypesOf(typeof(IEventHandler<>)).PropertiesAutowired();
		}


		private void RegisterQueries(ContainerBuilder builder)
		{
			builder.RegisterType<QueryBus>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterAssemblyTypes(assembly)
				.AsClosedTypesOf(typeof(IQueryHandler<,>))
				.SingleInstance().PropertiesAutowired();
		}

		private void RegisterCommands(ContainerBuilder builder)
		{
			builder.RegisterType<CommandBus>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterAssemblyTypes(assembly)
				.Where(x => x.IsAssignableTo<ICommandHandler>())
				.AsImplementedInterfaces();

			builder.Register<Func<Type, ICommandHandler>>(c =>
			{
				var context = c.Resolve<IComponentContext>();

				return type =>
				{
					var handlerType = typeof(ICommandHandler<>).MakeGenericType(type);
					return (ICommandHandler)context.Resolve(handlerType);
				};
			});
		}
	}
}
