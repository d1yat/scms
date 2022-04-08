using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using ScmsSoaLibraryInterface.Core.CustomMessageEncoder;

namespace ScmsSoaLibraryInterface.Core.JSONP
{
  public class JSONPBehavior : Attribute, IOperationBehavior
  {
    public string Callback { get; set; }

    public bool ReturnJsonArray { get; set; }

    #region IOperationBehavior Members

    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    { return; }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
      clientOperation.ParameterInspectors.Add(new Inspector(this.Callback, this.ReturnJsonArray));
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
      dispatchOperation.ParameterInspectors.Add(new Inspector(this.Callback, this.ReturnJsonArray));
    }

    public void Validate(OperationDescription operationDescription) { return; }

    #endregion

    //Parameter inspector
    class Inspector : IParameterInspector
    {
      string callback;
      bool returnJsonArray;

      public Inspector(string callback)
      {
        this.callback = callback;
      }

      public Inspector(string callback, bool returnJsonArray)
      {
        this.callback = callback;
        this.returnJsonArray = returnJsonArray;
      }

      public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
      {
      }

      public object BeforeCall(string operationName, object[] inputs)
      {
        string methodName = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters[callback];        
        if (methodName != null)
        {
          //System.ServiceModel.OperationContext.Current.OutgoingMessageProperties["wrapper"] = inputs[0];
          CustomMessageProperty property = new CustomMessageProperty()
          {
            MethodLogic = CustomMessageProperty.MethodLogical.JsonPadding,
            MethodName = methodName,
            ReturnJsonArray = returnJsonArray
          };
          OperationContext.Current.OutgoingMessageProperties.Add(CustomMessageProperty.Name, property);
        }
        return null;
      }
    }
  }
}
