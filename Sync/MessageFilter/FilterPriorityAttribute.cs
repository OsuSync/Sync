using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.MessageFilter
{
    public enum FilterPriority
    {
        Lowest=-2,
        Low=-1,
        Normal=0,
        High=1,
        Highest=2
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FilterPriorityAttribute:Attribute
    {
        public FilterPriority Priority { get; set; } = FilterPriority.Normal;
    }

    public static class FilterPriorityAttributeExtension
    {
        public static FilterPriority GetFilterPriority(this IFilter filter)
        {
            var attr=filter.GetType().GetCustomAttributes(typeof(FilterPriorityAttribute),false).FirstOrDefault() as FilterPriorityAttribute;

            return attr?.Priority ?? FilterPriority.Normal;
        }
    }
}
