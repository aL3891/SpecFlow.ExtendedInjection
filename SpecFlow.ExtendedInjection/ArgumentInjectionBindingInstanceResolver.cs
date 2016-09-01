using System;
using BoDi;
using TechTalk.SpecFlow.Infrastructure;

namespace betterbinding
{
    public class ArgumentInjectionBindingInstanceResolver : IBindingInstanceResolver
    {
        public object ResolveBindingInstance(Type bindingType, IObjectContainer scenarioContainer)
        {
            return scenarioContainer.Resolve<IDependencyInjectionAdapter>().Resolve(bindingType);
        }
    }
}