using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;

namespace ScmsSoaLibraryInterface.Core.CustomMessageEncoder
{
  public class CustomBindingExtension : BindingElementExtensionElement
  {
    public override Type BindingElementType
    {
      get { return typeof(CustomMessageBindingElement); }
    }

    protected override System.ServiceModel.Channels.BindingElement CreateBindingElement()
    {
      return new CustomMessageBindingElement();
    }
  }
}
