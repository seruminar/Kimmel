using System;
using System.Collections.Generic;

using Kimmel.Core.Kontent.Models.Management.References;

namespace Kimmel.Core.Kontent.Models.Management.Items
{
    public class Component
    {
        public Guid Id { get; set; }

        public Reference? Type { get; set; }

        public IList<dynamic>? Elements { get; set; }
    }
}