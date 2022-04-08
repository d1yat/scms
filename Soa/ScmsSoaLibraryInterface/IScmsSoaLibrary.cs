using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using ScmsSoaLibraryInterface.Core.JSONP;
using ScmsSoaLibraryInterface.Core.CustomEncoder;
using System.Xml.Linq;

namespace ScmsSoaLibraryInterface
{
  [ServiceContract(Namespace = "http://scms.ams.org", Name = "RestService")]  
  public interface IScmsSoaLibrary
  {
    [OperationContract]
    //[WebInvoke(Method = "GET",
    //  BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    [JSONPBehavior(Callback = "soaScmsCallback", ReturnJsonArray = true)]
    string GlobalQueryJson(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters);

    [OperationContract]
    //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
    //  BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest ,
      ResponseFormat = WebMessageFormat.Json)]
    string GlobalQueryService(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters);

    [OperationContract]
    //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
    //  BodyStyle = WebMessageBodyStyle.Bare)]
    [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
      ResponseFormat = WebMessageFormat.Json)]
    [CustomBehaviour(ReformatStringResult = true)]
    string GlobalQueryServiceClient(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters);

    [OperationContract]
    //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
    //  BodyStyle = WebMessageBodyStyle.Bare)]
    [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
      ResponseFormat = WebMessageFormat.Json)]
    [CustomBehaviour(ReformatStringResult = true)]
    string CheckConnectionDatabase();

    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
      //RequestFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    string PostData(string data);

    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
      //RequestFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    string GlobalPostUploadedData(string model, string originalName, byte[] data, string[][] parameters);

    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
      //RequestFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    string TestPostData(string data);

    [OperationContract]
    [WebGet]
    string GetData();

    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare,
    RequestFormat = WebMessageFormat.Xml,
    ResponseFormat = WebMessageFormat.Xml,
      Method = "POST")]
    [CustomBehaviour(ReformatStringResult = true)]
    string PostDataDiscore(XElement data);
  }
}
