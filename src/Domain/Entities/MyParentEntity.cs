using System;

namespace MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities
{
    /// <summary>
    /// scalar properties
    /// </summary>
    [Serializable]
    public partial class MyParentEntity
    {
        public Guid MyParentKey { get; set; } /* PK */

        public string MyParentName { get; set; }

        public DateTime UpdateDateStamp { get; set; }
    }
}
