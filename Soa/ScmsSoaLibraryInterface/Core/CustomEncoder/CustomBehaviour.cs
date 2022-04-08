using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using ScmsSoaLibraryInterface.Core.CustomMessageEncoder;

namespace ScmsSoaLibraryInterface.Core.CustomEncoder
{
  public class CustomBehaviour : Attribute, IOperationBehavior
  {
    #region IOperationBehavior Members

    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    { return; }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
      clientOperation.ParameterInspectors.Add(new Inspector(this.ReformatStringResult));
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
      dispatchOperation.ParameterInspectors.Add(new Inspector(this.ReformatStringResult));
    }

    public void Validate(OperationDescription operationDescription) { return; }

    #endregion

    internal CustomBehaviour()
    {
      ;
    }

    public bool ReformatStringResult
    { get; set; }
    
    //Parameter inspector
    class Inspector : IParameterInspector
    {
      bool reformatStringResult;

      public Inspector(bool reformatStringResult)
      {
        this.reformatStringResult = reformatStringResult;
      }

      public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
      {
      }

      public object BeforeCall(string operationName, object[] inputs)
      {
        CustomMessageProperty property = new CustomMessageProperty()
        {
          MethodLogic = CustomMessageProperty.MethodLogical.StringResultFormatter,
          ReformatStringResult = this.reformatStringResult
        };
        OperationContext.Current.OutgoingMessageProperties.Add(CustomMessageProperty.Name, property);

        return null;
      }
    }
  }
}
