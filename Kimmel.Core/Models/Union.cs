using System.ComponentModel;

namespace Kimmel.Core.Models
{
    [TypeConverter(typeof(StringConverter))]
    public class Union<T>
    {
        private readonly T value;

        protected Union(T value) => this.value = value;

        public override string ToString() => value?.ToString() ?? string.Empty;
    }
}