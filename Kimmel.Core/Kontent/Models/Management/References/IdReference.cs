﻿namespace Kimmel.Core.Kontent.Models.Management.References
{
    public class IdReference : Reference
    {
        public IdReference(string id) : base(id)
        {
        }

        protected override string ResolveValue() => Value;
    }
}