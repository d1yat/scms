using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Core;
using ScmsSoaLibrary.Commons;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using System.Data.Common;
using ScmsSoaLibrary.Bussiness;
using ScmsSoaLibraryInterface.Commons;


namespace ScmsSoaLibrary.Core.Response
{
  class DiscoreResponse
  {
    //public string resu;

    void PostKirim(object sID)
    {
      ScmsSoaLibrary.Parser.Class.ReturCustomerResponse strt = new ScmsSoaLibrary.Parser.Class.ReturCustomerResponse();

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      SCMS_RESPONSE_OBJECT res = new SCMS_RESPONSE_OBJECT();

      List<SCMS_RESPONSE_OBJECT> lisRes = new List<SCMS_RESPONSE_OBJECT>();

      IDictionary<string, string> dic = new Dictionary<string, string>();

      lisRes = (from q in db.SCMS_RESPONSE_OBJECTs
                where q.l_status == false
                select q).Distinct().ToList();

      //var arr = (from q in db.SCMS_RESPONSE_OBJECTs
      //           where q.l_status == false
      //           select q).Distinct().ToArray();

      int nLoop = 0;

      Uri uri = null;

      string result = null;

      Config cfg = new Config();

      ScmsSoaLibrary.Parser.ParserDisCore pdc;

      Dictionary<string, string> dHeader = null,
        dParam = null;
      Encoding utf8 = Encoding.UTF8;
      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      for (nLoop = 0; nLoop < lisRes.Count; nLoop++)
      {
        System.Threading.Thread.Sleep(1000);

        uri = Functionals.DistCoreUrlBuilder(cfg, lisRes[nLoop].v_url);

        pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, lisRes[nLoop].v_referer);
        pdc.ContentType = lisRes[nLoop].v_contentType;

        dHeader = ParserDisCore.HeaderParserDec(lisRes[nLoop].v_header);
        dParam = ParserDisCore.ParameterParserDec(lisRes[nLoop].v_param);

        if (pdc.PostGetData(uri, dParam, dHeader))
        {
          result = utf8.GetString(pdc.Result);

          Logger.WriteLine(result);

          dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCoreSwitch(result);

          result = Parser.ResponseParser.ResponseGenerator(rpe, dic, "sukses");

          ScmsSoaLibraryInterface.Components.PostDataParser.ParserDataNext(result);
        }
        else
        {
          Logger.WriteLine(pdc.ErrorMessage);
        }

        dHeader.Clear();
        dParam.Clear();
      }
    }

    public void MultiPostKirim(ORMDataContext db, string sID)
    {
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(PostKirim), sID);
    }
  }
}
