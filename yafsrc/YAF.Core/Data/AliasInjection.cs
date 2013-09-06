namespace YAF.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Omu.ValueInjecter;

    using ServiceStack.DataAnnotations;

    public class AliasInjection : LoopValueInjection
    {
        private Dictionary<string, string> aliasMapping = null;

        private Func<string, string> nameMap = null;

        protected override void Inject(object source, object target)
        {
            var props = target.GetProps();

            this.aliasMapping = props.OfType<PropertyDescriptor>()
                .Where(p => p.Attributes.OfType<AliasAttribute>().Any())
                .ToDictionary(k => k.Attributes.OfType<AliasAttribute>().FirstOrDefault().Name, v => v.Name);

            this.nameMap = new Func<string, string>(inputName => this.aliasMapping.ContainsKey(inputName) ? this.aliasMapping[inputName] : inputName);

            base.Inject(source, target);
        }

        protected override string TargetPropName(string sourcePropName)
        {
            if (this.nameMap != null)
            {
                return base.TargetPropName(this.nameMap(sourcePropName));
            }

            return base.TargetPropName(sourcePropName);
        }
    }
}