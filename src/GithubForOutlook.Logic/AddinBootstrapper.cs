﻿using System;
using Autofac;
using GithubForOutlook.Logic.Repositories;
using GithubForOutlook.Logic.Repositories.Interfaces;
using GithubForOutlook.Logic.Ribbons.Email;
using GithubForOutlook.Logic.Ribbons.MainExplorer;
using Microsoft.Office.Interop.Outlook;
using NGitHub;
using NGitHub.Authentication;
using VSTOContrib.Core.RibbonFactory.Interfaces;

namespace GithubForOutlook.Logic
{
	using GithubForOutlook.Logic.Ribbons.Tasks;

	public class AddinBootstrapper : IDisposable
    {
        private readonly IContainer container;

        public AddinBootstrapper(NameSpace nameSpace)
        {
            var containerBuilder = new ContainerBuilder();

            RegisterComponents(containerBuilder, nameSpace);

            container = containerBuilder.Build();
        }

        private static void RegisterComponents(ContainerBuilder containerBuilder, NameSpace nameSpace)
        {
            var assembly = typeof (GithubTask).Assembly;

            containerBuilder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf();

            containerBuilder.RegisterType<GitHubOAuthAuthorizer>()
                            .AsImplementedInterfaces();
            containerBuilder.RegisterType<GitHubClient>()
                            .AsImplementedInterfaces();

            containerBuilder.RegisterType<OutlookDispatchingRepository>()
                .As<IOutlookRepository>();

            containerBuilder.RegisterType<GithubRepository>()
                .As<IGithubRepository>();

            containerBuilder.Register(c => nameSpace);

            containerBuilder.RegisterType<GithubMailItem>()
                .As<IRibbonViewModel>()
                .AsSelf();

            containerBuilder.RegisterType<GithubTask>()
                .As<IRibbonViewModel>()
                .AsSelf();

            containerBuilder.RegisterType<GithubExplorerRibbon>()
                .As<IRibbonViewModel>()
                .AsSelf();
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}