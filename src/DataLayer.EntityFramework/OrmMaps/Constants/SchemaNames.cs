namespace MyCompany.MyExamples.EfPlaygroundOne.DomainDataLayer.EntityFramework.OrmMaps.Constants
{
    public static class SchemaNames
    {
#if (NETCOREAPP2_1 || NETSTANDARD2_0)
       public const string DefaultSchemaName = "MYORACLESCHEMAONE"; /* Oracle (??) It seems to require ALL-CAPS.  In Oracle, Users and Schemas are "the same". */


#endif

#if (NETCOREAPP3_1 || NETSTANDARD2_1)
        ///public const string DefaultSchemaName = "dbo"; /* Sql Server */
        
        ////public const string DefaultSchemaName = "public"; /* PostGres */
      
         public const string DefaultSchemaName = null; /* MySql   MySQL does not support the EF Core concept of schemas. Any schema property of any "MigrationOperation" must be null. */
#endif
    }
}
