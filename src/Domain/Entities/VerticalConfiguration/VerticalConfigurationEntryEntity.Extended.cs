namespace MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.VerticalConfiguration
{
    /// <summary>
    /// Navigation and Other properties
    /// </summary>
    public partial class VerticalConfigurationEntryEntity
    {
        public VerticalConfigurationCategoryEntity ParentVerticalConfigurationCategoryEntity { get; set; }
    }
}
