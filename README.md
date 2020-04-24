# oracle-ef-issues-demo
Demo showing issues with Oracle Entity Framework package/driver



Developers (Visual Studio)

Open sln
\src\Solutions\MyCompany.MyExamples.EfPlaygroundOne.sln

Set up start up project to:
\src\ConsoleOne\MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne.csproj

Set MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne.csproj to run under DotNetCore 2.1 (netcoreapp2.1)

Fix the ORACLE connection string here:

\src\ConsoleOne\appsettings.Development.json

Run the code.

Find the log file named

MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne*.log  where the * is a datetime.

............................

Points of interest:

In Program.cs (\src\ConsoleOne\Program.cs)

Set breakpoints at:

await demo.PerformDemoIQueryableWithReusedPocoObject();

await demo.PerformDemoIQueryableWithPrivateClassHolderObject();

await demo.PerformDemoIQueryableWithAnonymousClass();

............................

Logging/Tracing "setup".

The below file:

\src\DataLayer.EntityFramework\Contexts\EfPlaygroundDbContext.cs

has this code:

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Allow null in case you are using an IDesignTimeDbContextFactory
            if (this.loggerFactory != null)
            {
                ////if (System.Diagnostics.Debugger.IsAttached)
                ////{
                ////    //// Probably shouldn't log sql statements in production reminder
                optionsBuilder.UseLoggerFactory(this.loggerFactory); /* where this.loggerFactory is Microsoft.Extensions.Logging.ILoggerFactory */
                ////}
            }
        }

The below file:

\src\ConsoleOne\Program.cs

has this code:

            /* need trace to see Oracle.EF statements */
            servColl.AddLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Trace));

            servColl.AddLogging(blder =>
            {
                blder.SetMinimumLevel(LogLevel.Trace); /* need trace to see Oracle.EF statements */
                blder.AddSerilog(logger: lgr, dispose: true);
            });

The sample is also using a very simple to setup txt/log file.

            /* easy concrete logger that uses a file for demos */
            Serilog.ILogger lgr = new Serilog.LoggerConfiguration()
                .WriteTo.File("MyCompany.MyExamples.EfPlaygroundOne.ConsoleOne.log.txt", rollingInterval: Serilog.RollingInterval.Day)
                .CreateLogger();

The combination of those things is what allows this simple example to write a log/txt file with the EF generated SQL statements.







CURRENT ISSUES:

(1) (This is an actual run time bug)

Oracle.ManagedDataAccess.Client.OracleException (0x80004005): ORA-00904: "t"."MY_PARENT_UUID": invalid identifier


                 * Oracle.ManagedDataAccess.Client.OracleException (0x80004005): ORA-00904: "t"."MY_PARENT_UUID": invalid identifier
                 * 
                 * in the sql about 8 lines down:
                    The "t" table produces an alias "MyParentUuidFk" ( from the sql SELECT "i"."MY_PARENT_UUID" "MyParentUuidFk" )
                        But it does not use the alias on the JOIN.
                            (sql =         "chd"."MY_PARENT_UUID" = "t"."MY_PARENT_UUID" )
                    The join sql should be:
                            "chd"."MY_PARENT_UUID" = "t"."MyParentUuidFk"
                 */


The Generated Sql:


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


(2) (Performance Killer)

[WRN] The LINQ expression 'where ([anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
[WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
[WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.

(3) (Performance Killer)

[WRN] The LINQ expression 'where ({from MyChildEntity chd in value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.OneTimeRunnerExample.Domain.Entities.MyChildEntity]) join <>f__AnonymousType2`2 anonymousPerParentMaxUpdateDate in {from IGrouping`2 g in {value(Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[MyCompany.MyExamples.OneTimeRunnerExample.Domain.Entities.MyChildEntity]) => GroupBy([i].MyParentUuidFk, [i])} select new <>f__AnonymousType2`2(MyAnonymousMyParentUUID = [g].Key, MyAnonymousMaxPerParentUpdateDateStamp = {from MyChildEntity row in [g] select [row].UpdateDateStamp => Max()})} on new <>f__AnonymousType1`2(a = [chd].UpdateDateStamp, b = [chd].MyParentUuidFk) equals new <>f__AnonymousType1`2(a = [anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp, b = [anonymousPerParentMaxUpdateDate].MyAnonymousMyParentUUID) where (Not({__magicStatusValues_0 => Contains([chd].MyChildMagicStatu...
[WRN] The LINQ expression 'where ([anonymousPerParentMaxUpdateDate].MyAnonymousMaxPerParentUpdateDateStamp < __cutOffDate_1)' could not be translated and will be evaluated locally.
[WRN] The LINQ expression 'where ([chd].MyParentUuidFk == [par].MyParentKey)' could not be translated and will be evaluated locally.
[WRN] The LINQ expression 'Any()' could not be translated and will be evaluated locally.


