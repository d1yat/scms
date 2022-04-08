using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibraryInterface;
using ScmsSoaLibrary.Core.Reports;
using System.ServiceModel.Activation;
using System.ServiceModel;

namespace ScmsSoaLibrary.Services
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
    InstanceContextMode = InstanceContextMode.PerCall)]
  public class Reporting : IScmsSoaReporting
  {
    #region IScmsSoaReporting Members

    public string GeneratorReport(bool async, string config)
    {
      return (async ? ReportCaller.GenerateAsync(config) : ReportCaller.Generate(async, config));
    }

    #endregion
  }
}
