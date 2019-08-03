using System;

namespace ServiceStack.Metadata
{
    using ServiceStack.Text;

    public class JsvMetadataHandler : BaseMetadataHandler
    {
        public override Format Format => Format.Jsv;

        protected override string CreateMessage(Type dtoType)
        {
            var requestObj = AutoMappingUtils.PopulateWith(Activator.CreateInstance(dtoType));
            return requestObj.SerializeAndFormat();
        }
    }
}