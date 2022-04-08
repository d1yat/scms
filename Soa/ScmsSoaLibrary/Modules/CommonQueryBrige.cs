using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Core;
using ScmsSoaLibrary.Services;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Core;
using ScmsSoaLibraryInterface.Core.Converter;
using ScmsSoaLibraryInterface.Core.CustomMessageEncoder;
using System.Data.Linq.SqlClient;
using System.ServiceModel;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using ScmsModel.Core;
using ScmsModel;
using Ext.Net;
using System.Globalization;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Modules
{
  partial class CommonQueryBrige
  {
    public static IDictionary<string, object> ModelGridQuery(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();
      
      Config cfg = Functionals.Configuration;

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
      //db.CommandTimeout = 1000;

      Encoding utf8 = Encoding.UTF8;

      try
      {
        switch (model)
        {
            #region MODEL_COMMON_QUERY_BRIGE_RC

            case Constant.MODEL_COMMON_QUERY_BRIGE_RC:
                {
                    //Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.52/dist_core/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb");
                    Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb");

                    Dictionary<string, string> param = new Dictionary<string, string>();

                    string c_pbno = (parameters.ContainsKey("C_PBNO") ? (string)((Functionals.ParameterParser)parameters["C_PBNO"]).Value : string.Empty);
                    string cusmas = (parameters.ContainsKey("cusmas") ? (string)((Functionals.ParameterParser)parameters["cusmas"]).Value : string.Empty);
                    string cab = c_pbno.Substring(3, 3);
                    string result = null;
                    #region old code
                    //if (string.IsNullOrEmpty(cusmas) && !string.IsNullOrEmpty(c_pbno))
                    //{
                    //    if (c_pbno.Length > 6)
                    //    {
                    //        string cabDcore = c_pbno.Substring(3, 3);
                    //        cusmas = (from q in db.LG_CusmasCabs
                    //                  where q.c_cab_dcore == cabDcore
                    //                  select q.c_cab).Take(1).SingleOrDefault();
                    //    }
                    //}
                    #endregion
                    int i = 0;
                    for (i = 0; i < 3; i++)
                    {
                        #region multi ship ho1 & ho2
                        if (i == 0)
                        {
                            param.Add("C_PBNO", c_pbno);
                            param.Add("C_KODECAB", cab);
                            //param.Add("C_STATUS", "P"); //Suwandi 22 mei 2018 Old Code
                            param.Add("C_STATUS", "X"); //Suwandi 22 mei 2018
                            param.Add("C_MODE", "T");
                            param.Add("multi_C_SHPTO", "HO1,HO2");
                        }

                        #endregion

                        #region shipto kosong
                        if (i == 1)
                        {
                            param.Add("C_PBNO", c_pbno);
                            param.Add("C_KODECAB", cab);
                            //param.Add("C_STATUS", "P"); //Suwandi 22 mei 2018 Old Code
                            param.Add("C_STATUS", "X"); //Suwandi 22 mei 2018
                            param.Add("C_MODE", "T");
                            param.Add("C_SHPTO", "");

                        }
                        #endregion

                        #region mode regular
                        if (i == 2)
                        {
                            param.Add("C_PBNO", c_pbno);
                            param.Add("C_KODECAB", cab);
                            //param.Add("C_STATUS", "P"); //Suwandi 22 mei 2018 Old Code
                            param.Add("C_STATUS", "X"); //Suwandi 22 mei 2018
                            param.Add("multi_C_MODE", "R,L,N,A");

                        }
                        #endregion

                        #region relokasi lama
                        //if (i == 3)
                        //{
                        //    param.Add("C_PBNO", c_pbno);
                        //    param.Add("C_KODECAB", cab);
                        //    //param.Add("C_STATUS", "P"); //Suwandi 22 mei 2018 Old Code
                        //    param.Add("C_STATUS", "X"); //Suwandi 22 mei 2018
                        //    param.Add("C_MODE", "L");
                        //}
                        #endregion

                        Dictionary<string, string> header = new Dictionary<string, string>();
                        header.Add("X-Requested-With", "XMLHttpRequest");
                        bool getSuccess = false;


                        Dictionary<string, object> dicHeader = null;
                        //Dictionary<string, object> dicDetail = null;
                        List<Dictionary<string, string>> list = null;
                        Dictionary<string, string> dataRow = null;

                        ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

                        pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
                        pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        if (pdc.PostGetData(uri, param, header))
                        {
                            result = utf8.GetString(pdc.Result);

                            Logger.WriteLine(result);

                            dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                            if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
                            {
                                getSuccess = (bool)dicHeader[Constant.DEFAULT_NAMING_SUCCESS];

                                #region detail

                                if (getSuccess)
                                {
                                    param.Remove("C_KODECABOLD");

                                    list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                                    if (list.Count > 0)
                                    {
                                        dataRow = list[0];

                                        if (dataRow.ContainsKey("C_KODECAB"))
                                        {
                                            param.Add("C_KODECAB", dataRow["C_KODECAB"]);
                                            param.Add("C_PBNO", dataRow["C_PBNO"]);
                                            param.Add("limit", "1000");

                                            uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb_dt");
                                            //uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&f=AutoLookup&_q=trx_pb_dt_coba");
                                            if (pdc.PostGetData(uri, param, header))
                                            {
                                                result = utf8.GetString(pdc.Result);

                                                Logger.WriteLine(result);

                                                dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                                                if ((dic == null) || (!dic.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS)))
                                                {
                                                    result = utf8.GetString(pdc.Result);

                                                    dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                                                }
                                            }
                                            else
                                            {
                                                result = pdc.ErrorMessage;

                                                Logger.WriteLine(result);
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            result = pdc.ErrorMessage;

                            Logger.WriteLine(result);

                        }
                    }
                    #region oldcode
                    //param.Add("C_PBNO", c_pbno);
                    //param.Add("C_KODECAB", cab);
                    ////param.Add("C_STATUS", "P"); //Suwandi 22 mei 2018 Old Code
                    //param.Add("C_STATUS", "X"); //Suwandi 22 mei 2018
                    //param.Add("C_MODE", "T");
                    //param.Add("C_SHPTO", ""); //penambahan Parameter untuk membedakan relokasi ke cabang by suwandi 28 November 2018
                    //param.Add("multi_C_SHPTO", "HO1,HO2");
                    //param.Remove("C_SHPTO");
                    //Dictionary<string, string> header = new Dictionary<string, string>();
                    //header.Add("X-Requested-With", "XMLHttpRequest");

                    //bool getSuccess = false;

                    //string result = null;
                    //Dictionary<string, object> dicHeader = null;
                    ////Dictionary<string, object> dicDetail = null;
                    //List<Dictionary<string, string>> list = null;
                    //Dictionary<string, string> dataRow = null;

                    //ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

                    //pdc.Referer = Functionals.DistCoreUrlBuilderString(cfg, "http://10.100.10.40/dcore/?m=com.ams.trx.pbbpbr");
                    //pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    //if (pdc.PostGetData(uri, param, header))
                    //{
                    //  result = utf8.GetString(pdc.Result);

                    //  Logger.WriteLine(result);

                    //  dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                    //  if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
                    //  {
                    //    getSuccess = (bool)dicHeader[Constant.DEFAULT_NAMING_SUCCESS];

                    //      #region detail

                    //    if (getSuccess)
                    //    {
                    //      param.Remove("C_KODECABOLD");

                    //      list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

                    //      if (list.Count > 0)
                    //      {
                    //        dataRow = list[0];

                    //        if (dataRow.ContainsKey("C_KODECAB"))
                    //        {
                    //          param.Add("C_KODECAB", dataRow["C_KODECAB"]);
                    //          param.Add("C_PBNO", dataRow["C_PBNO"]);
                    //          param.Add("limit", "1000");

                    //          uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb_dt");
                    //          //uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.28/dcore/?m=com.ams.json.ds&f=AutoLookup&_q=trx_pb_dt_coba");
                    //          if (pdc.PostGetData(uri, param, header))
                    //          {
                    //            result = utf8.GetString(pdc.Result);

                    //            Logger.WriteLine(result);

                    //            dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                    //            if ((dic == null) || (!dic.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS)))
                    //            {
                    //              result = utf8.GetString(pdc.Result);

                    //              dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);
                    //                goto EndLogic;
                    //            }
                    //          }
                    //          else
                    //          {
                    //            result = pdc.ErrorMessage;

                    //            Logger.WriteLine(result);
                    //          }
                    //        }
                    //      }
                    //    }
                    //      #endregion
                    //  }
                    //}
                    //else
                    //{
                    //  result = pdc.ErrorMessage;

                    //  Logger.WriteLine(result);
                    //}
                    #endregion
                }
                break;

            #endregion

          #region MODEL_COMMON_QUERY_BRIGE_EKSPEDISI

          case Constant.MODEL_COMMON_QUERY_BRIGE_EKSPEDISI:
            {
              var qry = (from expH in db.LG_ExpHs
                         join expD in db.LG_ExpDs on expH.c_expno equals expD.c_expno
                         join msexp in db.LG_MsExps on expH.c_exp equals msexp.c_exp
                         select new
                         {
                           expD.c_expno,
                           c_dono = expD.c_dono,
                           expH.n_koli,
                           expH.n_berat,
                           msexp.v_ket
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                string paternLike = null;

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsBetween)
                    {

                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              int nCount = qry.Count();

              if (nCount > 0)
              {
                if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                {
                  qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                }

                if ((limit == -1) || allQuery)
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                }
                else
                {
                  dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());
                }
              }

              dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

              dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_BRIGE_BASPB

          case Constant.MODEL_COMMON_QUERY_BRIGE_BASPB:
            {
                Uri uri = Functionals.DistCoreUrlBuilder(cfg, "http://10.100.10.40/dcore/?m=com.ams.json.ds&action=form&f=Business&open=trx_baspb_dt");

                Dictionary<string, string> param = new Dictionary<string, string>();

                string baspbNo = (parameters.ContainsKey("C_BASPBNO") ? (string)((Functionals.ParameterParser)parameters["C_BASPBNO"]).Value : string.Empty);
                string cabDcore = null,
                    result = null;

                if (baspbNo.Length > 7)
                {
                    cabDcore = baspbNo.Substring(4, 3);
                }

                param.Add("C_BASPBNO", baspbNo);
                param.Add("C_KODECAB", cabDcore);

                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("X-Requested-With", "XMLHttpRequest");

                ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

                pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                if (pdc.PostGetData(uri, param, header))
                {
                    result = utf8.GetString(pdc.Result);
                    Logger.WriteLine(result);
                    dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                    if ((dic == null) || (!dic.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS)))
                    {
                        result = utf8.GetString(pdc.Result);
                        dic = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);
                    }
                    else
                    {
                        result = pdc.ErrorMessage;
                        Logger.WriteLine(result);
                    }
                }
                else
                {
                    result = pdc.ErrorMessage;
                    Logger.WriteLine(result);
                }
            }
            break;

          #endregion

          }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Modules.CommonQueryBrige:ModelGridQuery (First) <-> Switch {0} - {1}", model, ex.Message);
        Logger.WriteLine(ex.StackTrace);

        dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);
      }

      db.Dispose();

      return dic;
    }
  }
}
