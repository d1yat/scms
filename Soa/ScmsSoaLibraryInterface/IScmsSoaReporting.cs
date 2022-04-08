using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using ScmsSoaLibraryInterface.Components;

namespace ScmsSoaLibraryInterface
{
  [ServiceContract(Namespace = "http://reporting.soa.scms.ams.org/", Name = "scms")] 
  public interface IScmsSoaReporting
  {
    [OperationContract]
    [WebInvoke(Method = "GET",
      BodyStyle = WebMessageBodyStyle.WrappedRequest)]
    string GeneratorReport(bool async, string config);
  }
}
