using System;
using System.Linq;
using System.Reflection;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow;
using System.Diagnostics;

namespace betterbinding
{
    public class ArgumentInjectionBindingInvoker : IBindingInvoker
    {
        public object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            var method = ((RuntimeBindingMethod)binding.Method).MethodInfo;
            object instance = null;

            var diAdapter = contextManager.ScenarioContext.ScenarioContainer.Resolve<IDependencyInjectionAdapter>();
            var args = arguments;

            if (arguments.Length < method.GetParameters().Length)
            {
                var extraParameters = method.GetParameters().Skip(arguments.Length).Select(p => GetExtraParameter(p, contextManager.ScenarioContext, diAdapter));
                args = args.Concat(extraParameters).ToArray();
            }

            if (!method.IsStatic)
                instance = contextManager.ScenarioContext.GetBindingInstance(method.ReflectedType);

            Stopwatch s = new Stopwatch();
            s.Start();
            var res = method.Invoke(instance, args);
            duration = s.Elapsed;

            if (res != null)
                contextManager.ScenarioContext.Set(res, method.GetCustomAttribute<ReturnKeyAttribute>()?.Key ?? method.ReturnType.FullName);



            return null;

        }

        private object GetExtraParameter(ParameterInfo p, ScenarioContext scenarioContainer, IDependencyInjectionAdapter diAdapter)
        {
            object res = null;
            var name = p.GetCustomAttribute<KeyAttribute>()?.Key ?? p.ParameterType.FullName;
            if (scenarioContainer.TryGetValue(name, out res))
                return res;
            else
                try
                {
                    return scenarioContainer.ScenarioContainer.Resolve(p.ParameterType);
                }
                catch (Exception)
                {
                    return diAdapter.Resolve(p.ParameterType);
                }
        }
    }
}