﻿using System;
using MyCompany.MyExamples.EfPlaygroundOne.Domain.Enums;

namespace MyCompany.MyExamples.EfPlaygroundOne.Domain.Entities
{
    /// <summary>
    /// scalar properties
    /// </summary>
    [Serializable]
    public partial class MyChildEntity
    {
        public Guid MyChildKey { get; set; } /* PK */

        public Guid MyParentUuidFk { get; set; }

        public string MyChildName { get; set; }

        public int MyChildMagicStatus { get; set; }

        public DateTime UpdateDateStamp { get; set; }

        public FavoriteColorEnum FavoriteColor { get; set; }
    }
}
