using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Demos.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers;
using MyCompany.MyExamples.EfPlaygroundOne.BusinessLayer.Managers.Interfaces;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.EntityFramework;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.EntityFramework.Contexts;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.EntityFramework.DataGenerators;
using MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.Interfaces;

#if (NETCOREAPP2_1 || NETSTANDARD2_0)
////using MySql.Data.EntityFrameworkCore;
#endif
#if (NETCOREAPP3_1 || NETSTANDARD2_1)
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
#endif

using Serilog;

namespace MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            /* easy concrete logger that uses a file for demos */
            Serilog.ILogger lgr = new Serilog.LoggerConfiguration()
                .WriteTo.File("MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne.log.txt", rollingInterval: Serilog.RollingInterval.Day)
                .CreateLogger();

            try
            {
                /* look at the Project-Properties/Debug(Tab) for this environment variable */
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Console.WriteLine(string.Format("ASPNETCORE_ENVIRONMENT='{0}'", environmentName));
                Console.WriteLine(string.Empty);

                IConfigurationBuilder builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables();

                IConfigurationRoot configuration = builder.Build();

                IServiceProvider servProv = BuildDi(configuration, lgr);

                VerticalConfigurationDataGenerator.Initialize(servProv);
                await RunMyParentDemo(servProv, lgr);
                ////await RunMyChildDemo(servProv);
                ////await RunBoardGameDemo(servProv);
            }
            catch (Exception ex)
            {
                string flattenMsg = GenerateFullFlatMessage(ex, true);
                Console.WriteLine(flattenMsg);
            }

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();

            return 0;
        }

        private static async Task RunMyParentDemo(IServiceProvider servProv, Serilog.ILogger lgr)
        {
            /* Create seed data for the EF Database.  NOT for Production type of code */
            MyParentDataGenerator.Initialize(servProv);

            IParentDemos demo = servProv.GetService<IParentDemos>();
            ////await demo.PerformBasicCrudDemo();

            ICollection<Exception> allExceptions = new List<Exception>();

            try
            {
                await demo.PerformDemoIQueryableWithReusedPocoObject();
            }
            catch (Exception ex)
            {
                /* this one is actually broken.         
                 * Oracle.ManagedDataAccess.Client.OracleException (0x80004005): ORA-00904: "t"."MY_PARENT_UUID": invalid identifier
                 * 
                 * in the sql about 8 lines down:
                    The "t" table produces an alias "MyParentUuidFk" ( from the sql SELECT "i"."MY_PARENT_UUID" "MyParentUuidFk" )
                        But it does not use the alias on the JOIN.
                            (sql =         "chd"."MY_PARENT_UUID" = "t"."MY_PARENT_UUID" )
                    The join sql should be:
                            "chd"."MY_PARENT_UUID" = "t"."MyParentUuidFk"
                 */

                /*
                SELECT "par"."MY_PARENT_UUID", "par"."MY_PARENT_NAME", "par"."UPDATE_DATE_TS"
                FROM "MYORACLESCHEMAONE"."MY_PARENT" "par"
                WHERE EXISTS (
                    SELECT 1
                    FROM "MYORACLESCHEMAONE"."MY_CHILD" "chd"
                    INNER JOIN (
                        SELECT "i"."MY_PARENT_UUID" "MyParentUuidFk", MAX("i"."UPDATE_DATE_TS") "UpdateDateStamp"
                        FROM "MYORACLESCHEMAONE"."MY_CHILD" "i"
                        GROUP BY "i"."MY_PARENT_UUID"
                    ) "t" 
                        ON ("chd"."UPDATE_DATE_TS" = "t"."UPDATE_DATE_TS") AND ("chd"."MY_PARENT_UUID" = "t"."MY_PARENT_UUID")
                    WHERE ("chd"."MY_CHILD_MAGIC_STATUS" NOT IN (98, 99) AND ("t"."UPDATE_DATE_TS" < :cutOffDate_1)) AND ("chd"."MY_PARENT_UUID" = "par"."MY_PARENT_UUID")) OR NOT EXISTS (
                    SELECT 1
                    FROM "MYORACLESCHEMAONE"."MY_CHILD" "m"
                    WHERE "par"."MY_PARENT_UUID" = "m"."MY_PARENT_UUID")
                        ;                 
                 
                 */

                lgr.Warning(ex, "PerformDemoIQueryableWithReusedPocoObject FAILED");
                Console.WriteLine(GenerateFullFlatMessage(ex));
                allExceptions.Add(ex);
            }

            try
            {
                await demo.PerformDemoIQueryableWithPrivateClassHolderObject();
            }
            catch (Exception ex)
            {
                /* no error here, BUT "not be translated */
                /* 
                2020-04-21 04:43:55.834 -04:00 [WRN] The LINQ expression 'where ([perParentMaxUpdateDate].MyPrivateClassMaxChildUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.841 -04:00 [WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.851 -04:00 [WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.896 -04:00 [WRN] The LINQ expression 'where ({from MyChildEntity chd in value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) join ChildMaxHolder perParentMaxUpdateDate in {from IGrouping`2 g in {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) => GroupBy([i].MyParentUuidFk, [i])} select new ChildMaxHolder() {MyPrivateClassParentUuid = [g].Key, MyPrivateClassMaxChildUpdateDateStamp = {from MyChildEntity row in [g] select [row].UpdateDateStamp => Max()}}} on new <>f__AnonymousType1`2(a = [chd].UpdateDateStamp, b = [chd].MyParentUuidFk) equals new <>f__AnonymousType1`2(a = [perParentMaxUpdateDate].MyPrivateClassMaxChildUpdateDateStamp, b = [perParentMaxUpdateDate].MyPrivateClassParentUuid) where (Not({__magicStatusValues_0 => Contains([chd].MyChildMagicStatus)}) AndAlso ([perParentMaxUpdateDate].MyPrivateClassMaxChildUpdateDateStamp < __cutOffDate_1)) where ([chd].MyParentUuidFk == [par].MyParentKey) select [chd] => Any()} OrElse Not({from MyChildEntity <generated>_2 in value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) where  ?= (Property([par], "MyParentKey") == Property([<generated>_2], "MyParentUuidFk")) =? select [<generated>_2] => Any()}))' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.903 -04:00 [WRN] The LINQ expression 'where ([perParentMaxUpdateDate].MyPrivateClassMaxChildUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.912 -04:00 [WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
                2020-04-21 04:43:55.912 -04:00 [WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.
                */

                lgr.Warning(ex, "PerformDemoIQueryableWithPrivateClassHolderObject FAILED");
                Console.WriteLine(GenerateFullFlatMessage(ex));
                allExceptions.Add(ex);
            }

            try
            {
                await demo.PerformDemoIQueryableWithAnonymousClass();
            }
            catch (Exception ex)
            {
                /* no error here, BUT "not be translated and will be evaluated locally" warnings in the log file */

                /*
                2020-04-21 04:45:51.833 -04:00 [WRN] The LINQ expression 'where ([anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.851 -04:00 [WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.863 -04:00 [WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.902 -04:00 [WRN] The LINQ expression 'where ({from MyChildEntity chd in value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) join <>f__AnonymousType2`2 anonymousPerParentMaxUpdateDate in {from IGrouping`2 g in {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) => GroupBy([i].MyParentUuidFk, [i])} select new <>f__AnonymousType2`2(MyAnonymousMyParentUUID = [g].Key, MyAnonymousMaxPerParentUpdateDateStamp = {from MyChildEntity row in [g] select [row].UpdateDateStamp => Max()})} on new <>f__AnonymousType1`2(a = [chd].UpdateDateStamp, b = [chd].MyParentUuidFk) equals new <>f__AnonymousType1`2(a = [anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp, b = [anonymousPerParentMaxUpdateDate].MyAnonymousMyParentUUID) where (Not({__magicStatusValues_0 => Contains([chd].MyChildMagicStatus)}) AndAlso ([anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp < __cutOffDate_1)) where ([chd].MyParentUuidFk == [par].MyParentKey) select [chd] => Any()} OrElse Not({from MyChildEntity <generated>_2 in value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.MyChildEntity]) where  ?= (Property([par], "MyParentKey") == Property([<generated>_2], "MyParentUuidFk")) =? select [<generated>_2] => Any()}))' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.906 -04:00 [WRN] The LINQ expression 'where ([anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.914 -04:00 [WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
                2020-04-21 04:45:51.914 -04:00 [WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.                 
                */

                lgr.Warning(ex, "PerformDemoIQueryableWithAnonymousClass FAILED");
                Console.WriteLine(GenerateFullFlatMessage(ex));
                allExceptions.Add(ex);
            }

            if (allExceptions.Any())
            {
                AggregateException aggex = new AggregateException("All Oracle.EF Exceptions", allExceptions);
                lgr.Error(aggex, aggex.Message);
                throw aggex;
            }
        }

        private static async Task RunMyChildDemo(IServiceProvider servProv)
        {
            /* Create seed data for the In Memory Database.  NOT for Production type of code */
            EfPlaygroundDbContext context = servProv.GetRequiredService<EfPlaygroundDbContext>();
            ////MyChildDataGenerator.Initialize(servProv);

            IChildDemos demo = servProv.GetService<IChildDemos>();
            await demo.PerformBasicCrudDemo();
        }

        private static async Task RunBoardGameDemo(IServiceProvider servProv)
        {
            /* Create seed data for the In Memory Database.  NOT for Production type of code */
            EfPlaygroundDbContext context = servProv.GetRequiredService<EfPlaygroundDbContext>();
            BoardGameDataGenerator.Initialize(servProv);

            IBoardGameDemos demo = servProv.GetService<IBoardGameDemos>();
            await demo.PerformBasicCrudDemo();
        }

        private static string GenerateFullFlatMessage(Exception ex)
        {
            return GenerateFullFlatMessage(ex, false);
        }

        private static IServiceProvider BuildDi(IConfiguration configuration, Serilog.ILogger lgr)
        {
            string defaultOracleConnectionString = string.Empty;
            string defaultMySqlConnectionStringValue = string.Empty;

#if (NETCOREAPP2_1 || NETSTANDARD2_0)
            defaultOracleConnectionString = configuration.GetConnectionString("DefaultOracleConnectionString");
            Console.WriteLine(string.Format("defaultOracleConnectionString='{0}'", defaultOracleConnectionString));
            Console.WriteLine(string.Empty);
#endif

            string defaultPostGresConnectionStringValue = string.Empty;
            string defaultSqlServerConnectionStringValue = string.Empty;

#if (NETCOREAPP3_1 || NETSTANDARD2_1)
            defaultPostGresConnectionStringValue = configuration.GetConnectionString("DefaultPostGresConnectionString");
            Console.WriteLine(string.Format("defaultPostGresConnectionStringValue='{0}'", defaultPostGresConnectionStringValue));
            Console.WriteLine(string.Empty);

            defaultSqlServerConnectionStringValue = configuration.GetConnectionString("DefaultSqlServerConnectionString");
            Console.WriteLine(string.Format("defaultSqlServerConnectionStringValue='{0}'", defaultSqlServerConnectionStringValue));
            Console.WriteLine(string.Empty);

            defaultMySqlConnectionStringValue = configuration.GetConnectionString("DefaultMySqlConnectionString");
            Console.WriteLine(string.Format("defaultMySqlConnectionStringValue='{0}'", defaultMySqlConnectionStringValue));
            Console.WriteLine(string.Empty);
#endif

            bool realRdbmsWasTriggered = false;
            string didYouChangeReminder = "Did you change?\n\\src\\DataLayer.EntityFramework\\OrmMaps\\Constants\\SchemaNames.cs and\n\\src\\DataLayer.EntityFramework\\OrmMaps\\Constants\\SqlKeyWords.cs\nto match the RDBMS?";

            ////setup our DI
            IServiceCollection servColl = new ServiceCollection()
                .AddSingleton(lgr)
                .AddLogging()
                .AddSingleton<IBoardGameManager, BoardGameManager>()
                .AddSingleton<IBoardGameDataLayer, BoardGameEntityFrameworkDataLayer>()
                .AddSingleton<IMyParentManager, MyParentManager>()
                .AddSingleton<IMyParentDomainData, MyParentEntityFrameworkDomainDataLayer>()
                .AddSingleton<IParentDemos, ParentDemos>()
                .AddSingleton<IMyChildManager, MyChildManager>()
                .AddSingleton<IMyChildDomainData, MyChildEntityFrameworkDomainDataLayer>();

            /* need trace to see Oracle.EF statements */
            servColl.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            servColl.AddLogging(blder =>
            {
                blder.SetMinimumLevel(LogLevel.Trace); /* need trace to see Oracle.EF statements */
                blder.AddSerilog(logger: lgr, dispose: true);
            });

            /* give preference to oracle or mysql ..... over sql-server and postgres */
            if (!string.IsNullOrEmpty(defaultOracleConnectionString))
            {
#if (NETCOREAPP2_1 || NETSTANDARD2_0)
                Console.WriteLine("Using Oracle EF");
                Console.WriteLine(didYouChangeReminder);
                servColl.AddDbContext<EfPlaygroundDbContext>(options => options.UseOracle(defaultOracleConnectionString));
                realRdbmsWasTriggered = true;
#endif
            }
            else
            {
                if (!string.IsNullOrEmpty(defaultMySqlConnectionStringValue))
                {
#if (NETCOREAPP3_1 || NETSTANDARD2_1)
                    ////throw new NotImplementedException("Pomelo.EntityFrameworkCore.MySql not implemented yet");
                    Console.WriteLine("Using Pomelo.EntityFrameworkCore.MySql EF");
                    Console.WriteLine(didYouChangeReminder);
                    servColl.AddDbContext<EfPlaygroundDbContext>(options => options.UseMySql(defaultMySqlConnectionStringValue));
                    realRdbmsWasTriggered = true;
#endif
                }
                else
                {
                    /* give preference to sql-server over postgres */
                    if (!string.IsNullOrEmpty(defaultSqlServerConnectionStringValue))
                    {
                        Console.WriteLine("Using Sql Server EF");
                        Console.WriteLine(didYouChangeReminder);
                        servColl.AddDbContext<EfPlaygroundDbContext>(options => options.UseSqlServer(defaultSqlServerConnectionStringValue));
                        realRdbmsWasTriggered = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(defaultPostGresConnectionStringValue))
                        {
#if (NETCOREAPP3_1 || NETSTANDARD2_1)
                            ////throw new NotImplementedException("PostGres not added yet");
                            Console.WriteLine("Using PostGres EF");
                            Console.WriteLine(didYouChangeReminder);
                            servColl.AddDbContext<EfPlaygroundDbContext>(options => options.UseNpgsql(defaultPostGresConnectionStringValue));
                            realRdbmsWasTriggered = true;
#endif
                        }
                    }
                }
            }

            if (!realRdbmsWasTriggered)
            {
                Console.WriteLine("Using UseInMemoryDatabase");
                servColl.AddDbContext<EfPlaygroundDbContext>(options => options.UseInMemoryDatabase(databaseName: "BoardGamesInMemoryDatabase"));
            }

            ServiceProvider servProv = servColl.BuildServiceProvider();

            return servProv;
        }

        private static string GenerateFullFlatMessage(Exception ex, bool showStackTrace)
        {
            string returnValue;

            StringBuilder sb = new StringBuilder();
            Exception nestedEx = ex;

            while (nestedEx != null)
            {
                if (!string.IsNullOrEmpty(nestedEx.Message))
                {
                    sb.Append(nestedEx.Message + System.Environment.NewLine);
                }

                if (showStackTrace && !string.IsNullOrEmpty(nestedEx.StackTrace))
                {
                    sb.Append(nestedEx.StackTrace + System.Environment.NewLine);
                }

                if (ex is AggregateException)
                {
                    AggregateException ae = ex as AggregateException;

                    foreach (Exception aeflatEx in ae.Flatten().InnerExceptions)
                    {
                        if (!string.IsNullOrEmpty(aeflatEx.Message))
                        {
                            sb.Append(aeflatEx.Message + System.Environment.NewLine);
                        }

                        if (showStackTrace && !string.IsNullOrEmpty(aeflatEx.StackTrace))
                        {
                            sb.Append(aeflatEx.StackTrace + System.Environment.NewLine);
                        }
                    }
                }

                nestedEx = nestedEx.InnerException;
            }

            returnValue = sb.ToString();

            return returnValue;
        }
    }
}