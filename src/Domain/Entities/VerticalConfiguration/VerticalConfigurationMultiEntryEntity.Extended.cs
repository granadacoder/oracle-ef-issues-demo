﻿namespace MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities.VerticalConfiguration
{
    /// <summary>
    /// Navigation and Other properties
    /// </summary>
    public partial class VerticalConfigurationMultiEntryEntity
    {
        public VerticalConfigurationCategoryEntity ParentVerticalConfigurationCategoryEntity { get; set; }
    }
}
