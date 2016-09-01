using betterbinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;

[assembly: RuntimePlugin(typeof(ArgumentInjectionPlugin))]
[assembly: GeneratorPlugin(typeof(DummyGeneratorPlugin))]

public class DummyGeneratorPlugin : IGeneratorPlugin
{
    public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters)
    {

    }
}

namespace betterbinding
{
    public class ArgumentInjectionPlugin : IRuntimePlugin
    {
        private Type adapter;
        private List<string> additionalAssemblies;

        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<ArgumentInjectionBindingInstanceResolver, IBindingInstanceResolver>();
                args.ObjectContainer.RegisterTypeAs<ArgumentInjectionBindingInvoker, IBindingInvoker>();
                additionalAssemblies = args.RuntimeConfiguration.AdditionalStepAssemblies;
            };



            runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) =>
            {
                var assemblies = args.ObjectContainer.Resolve<IBindingRegistry>().GetStepDefinitions().Select(m => m.Method).OfType<RuntimeBindingMethod>().Select(t => t.MethodInfo.DeclaringType.Assembly).Distinct();
                var primaryAssembly = assemblies.Where(a => !additionalAssemblies.Contains(a.FullName)).First();

                adapter = primaryAssembly.GetTypes().Where(t => typeof(IDependencyInjectionAdapter).IsAssignableFrom(t)).SingleOrDefault();

                if (adapter == null)
                    adapter = primaryAssembly.GetCustomAttributes(true).OfType<DependencyAdapterAttribute>().FirstOrDefault()?.Type;

                if (adapter == null)
                    adapter = assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(IDependencyInjectionAdapter).IsAssignableFrom(t)).Single();

                args.ObjectContainer.RegisterFactoryAs(() =>
                {
                    var a = (IDependencyInjectionAdapter)Activator.CreateInstance(adapter);
                    a.Initialize();
                    return a;
                });
            };
        }
    }
}
