using System;
using System.Collections.Generic;
using System.Text;

namespace ScmsSoaLibrary.Core.Threading
{
  class Running
  {
    #region Structures

    public struct RunningThreadFJ
    {
      public string connectionString;
      public string Customer;
      public string DoNo1;
      public string DoNo2;
      public string User;
      public bool IsSet;
      public decimal SisaFJNO; //Indra 20170613
    }

    public struct RunningThreadRS
    {
      public string connectionString;
      public ScmsSoaLibrary.Modules.CommonUploadedQuery.Temporary_ImportRS[] importRSes;
      public string User;
      public bool CreateFakturRetur;
      public bool IsSet;
    }

    public struct RunningThreadSendDO
    {
      public string connectionString;
      public string DoNo;
      public bool IsStt;
      public bool IsSet;
    }

    public struct RunningThreadSendMasterDivisiPrinsipal
    {
      public bool IsSet;
      public string SenderMode;
      public ScmsModel.FA_MsDivPri DivPri;
      public string connectionString;
    }

    public struct RunningThreadReplySPStructure
    {
      public string connectionString;
      public string RawData;
      public string spId;
      public bool IsSet;
    }

    public struct RunningThreadReplyItmStructure
    {
        public string connectionString;
        public string RawData;
        public string Iteno;
        public bool IsSet;
    }

    #endregion

    public static void RunningThreadRunningThreadReplySP(object state)
    {
      RunningThreadReplySPStructure rtRSP = (RunningThreadReplySPStructure)state;

      ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;

      if (rtRSP.IsSet)
      {
        spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

        spRun.PostDataReplySP(rtRSP.connectionString, rtRSP.RawData, rtRSP.spId);
      }
    }

    public static void RunningThreadRunningThreadReplySPM(object state)
    {
        RunningThreadReplySPStructure rtRSP = (RunningThreadReplySPStructure)state;

        ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;

        if (rtRSP.IsSet)
        {
            spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

            spRun.PostDataReplySPM(rtRSP.connectionString, rtRSP.RawData, rtRSP.spId);
        }
    }

    public static void RunningThreadRunningThreadReplySPETA(object state)
    {
        RunningThreadReplySPStructure rtRSP = (RunningThreadReplySPStructure)state;

        ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;

        if (rtRSP.IsSet)
        {
            spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

            spRun.PostDataReplySPETA(rtRSP.connectionString, rtRSP.RawData, rtRSP.spId);
        }
    }

    public static void RunningThreadGenerateFJ(object state)
    {
      RunningThreadFJ rtFJ = (RunningThreadFJ)state;

      IDictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> param = null;
      ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
      string[] res = null;

      if (rtFJ.IsSet)
      {
        param = new Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser>();
        
        param.Add("customer", new ScmsSoaLibrary.Commons.Functionals.ParameterParser()
        {
          IsSet = true,
          Value = rtFJ.Customer
        });
        param.Add("do_from", new ScmsSoaLibrary.Commons.Functionals.ParameterParser()
        {
          IsSet = true,
          Value = rtFJ.DoNo1
        });
        param.Add("do_to", new ScmsSoaLibrary.Commons.Functionals.ParameterParser()
        {
          IsSet = true,
          Value = rtFJ.DoNo2
        });
        param.Add("user", new ScmsSoaLibrary.Commons.Functionals.ParameterParser()
        {
          IsSet = true,
          Value = rtFJ.User
        });
        //Indra 20170613
        param.Add("SisaFJ", new ScmsSoaLibrary.Commons.Functionals.ParameterParser()
        {
            IsSet = true,
            Value = rtFJ.SisaFJNO
        });
        //End
        spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

        res = spRun.SP_LG_CalcFJ(rtFJ.connectionString, param);
      }
    }

    public static void RunningThreadGenerateRS(object state)
    {
      RunningThreadRS rtRS = (RunningThreadRS)state;

      ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
      string[] res = null;

      if (rtRS.IsSet)
      {
        spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

        res = spRun.SP_LG_CalcRS(rtRS.connectionString, rtRS.User, rtRS.CreateFakturRetur, rtRS.importRSes);
      }
    }
    
    public static void RunningThreadGenerateSendDO(object state)
    {
      RunningThreadSendDO rtSendDO = (RunningThreadSendDO)state;

      ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
      bool bOk = false;

      if (rtSendDO.IsSet)
      {
        spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

        bOk = spRun.PostDataDO(rtSendDO.connectionString, rtSendDO.DoNo, rtSendDO.IsStt);
      }
    }

    public static void RunningThreadSPAdmin(object state)
    {
      ScmsSoaLibrary.Parser.Class.SuratPesananStructure strt = state as ScmsSoaLibrary.Parser.Class.SuratPesananStructure;

      if (strt == null)
      {
        ScmsSoaLibraryInterface.Commons.Logger.WriteLine("Invalid state on 'RunningThreadSPAdmin'");

        return;
      }

      ScmsSoaLibrary.Bussiness.Pembelian pbln = new ScmsSoaLibrary.Bussiness.Pembelian();      

      pbln.SuratPesananAdmin(strt);
    }

    public static void RunningThreadSendMasterDivPrinsipal(object state)
    {
      RunningThreadSendMasterDivisiPrinsipal rtSendDivPri = (RunningThreadSendMasterDivisiPrinsipal)state;

      ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;
      bool bOk = false;

      if (rtSendDivPri.IsSet)
      {
        spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

        bOk = spRun.PostDataMasterDivisiPrinsipal(rtSendDivPri.connectionString, rtSendDivPri.SenderMode, rtSendDivPri.DivPri);
      }
    }

    public static void RunningThreadRunningThreadReplyItm(object state)
    {
        RunningThreadReplyItmStructure rtRItm = (RunningThreadReplyItmStructure)state;

        ScmsSoaLibrary.Modules.CommonQuerySP spRun = null;

        if (rtRItm.IsSet)
        {
            spRun = new ScmsSoaLibrary.Modules.CommonQuerySP();

            spRun.PostDataReplyItm(rtRItm.connectionString, rtRItm.RawData, rtRItm.Iteno);
        }
    }
  }
}
