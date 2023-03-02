// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DMT.Web.DependencyResolution
{
    using DMT.GeneratingOrderbooks.Service;
    using DMT.GeneratingOrderBooks.Data;
    using DMT.GeneratingOrderBooks.Domain.Interfaces;
    using DMT.ManagingNotifications.Data;
    using DMT.ManagingNotifications.Domain.Interfaces;
    using DMT.ManagingNotifications.Service;
    using DMT.ManagingNotifications.Service.Interfaces;
    using DMT.SharedKernel.Interface;
    using GeneratingOrderbooks.Service.Interfaces;
    using StructureMap;

    public class DefaultRegistry : Registry
    {
        #region Constructors and Destructors

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                    scan.Assembly("DMT.ManagingNotifications.EventService");
                    scan.Assembly("DMT.Web");
                    scan.ConnectImplementationsToTypesClosing(typeof(IHandle<>));
                });

            For<IGeneratingOrderbooksService>().Use<GeneratingOrderbooksService>();
            For<IGeneratingOrderbooksRepo>().Use<GeneratingOrderbooksRepository>().
                                                Ctor<GeneratingOrderbooksContext>("context").
                                                    Is(new GeneratingOrderbooksContext());
            For<IManagingNotificationsService>().Use<ManagingNotificationsService>();
            For<IManagingNotificationsRepo>().Use<ManagingNotificationsRepository>().
                                                Ctor<ManagingNotificationsContext>("context").
                                                    Is(new ManagingNotificationsContext());
        }

        #endregion
    }
}