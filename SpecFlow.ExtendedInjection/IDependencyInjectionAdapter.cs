using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace betterbinding
{
    public interface IDependencyInjectionAdapter
    {
        object Resolve(Type type);
        void Initialize();
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class DependencyAdapterAttribute : Attribute
    {
        public DependencyAdapterAttribute(Type type)
        {
            Type = type;
        }
        public Type Type { get; set; }

    }
}
