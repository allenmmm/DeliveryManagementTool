using DMT.GeneratingOrderBooks.Data;
using DMT.ManagingNotifications.Data;
using DMT.SharedKernel.DependencyResolution;
using DMT.SharedKernel.Interface;
using DMT.Web.DependencyResolution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using StructureMap;

namespace GeneratingOrderbooks.given_an_orderbook.Integration.Test
{
    public abstract class TestBase  : IDisposable
    {
        protected readonly GeneratingOrderbooksContext GeneratingOrderbooksContext;
        protected readonly ManagingNotificationsContext ManagingNotificationsContext;
        protected readonly PlannedAndOverdueFileParser FileParser;

        public TestBase()
        {
            GeneratingOrderbooksContext = new GeneratingOrderbooksContext();
            ManagingNotificationsContext = new ManagingNotificationsContext();

            IoC.ClearRegistries();
            IoC.AddRegistry<TestBaseRegistry>();
        }

        protected virtual void SetUp()
        {
            GeneratingOrderbooksContext.Database.EnsureDeleted();
            GeneratingOrderbooksContext.Database.EnsureCreated();
            RelationalDatabaseCreator databaseCreator =
                (RelationalDatabaseCreator)ManagingNotificationsContext.Database.GetService<IDatabaseCreator>();
                        databaseCreator.CreateTables();
        }

        protected void PopulateTable(string table,
                                     List<List<SqlParameter>> values)
        {
            values.ForEach(value =>
            {
                var commandText = "INSERT into " + table + " Values (";
                var parameters = SqlSeeder.CreateSQLParameters(value);
                parameters.ForEach(p => commandText += p + ",");
                commandText = commandText.Remove(commandText.Length - 1) + ")";
                SqlParameter[] parameters_in = value.ToArray();
                GeneratingOrderbooksContext.Database
                 .ExecuteSqlCommand(commandText, parameters_in);
            });
        }

        protected void Seed(DateTime dateTime)
        {
            PlannedAndOverdueFileParser FileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(FileParser);
            using (var repo = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                repo.UpdatePlannedAndOverdues(FileParser.PlannedAndOverdueOrders);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(GeneratingOrderbooksContext!= null)
                GeneratingOrderbooksContext.Dispose();
            if(ManagingNotificationsContext!=null)
            ManagingNotificationsContext.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class TestBaseRegistry : Registry
    {
        public TestBaseRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
                scan.With(new ControllerConvention());
                scan.Assembly("DMT.ManagingNotifications.EventService");
                scan.Assembly("DMT.Web");
                scan.ConnectImplementationsToTypesClosing(typeof(IHandle<>));
            });
        }
    }

}
