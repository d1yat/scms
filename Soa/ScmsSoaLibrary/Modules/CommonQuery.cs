using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel.Core;
using ScmsModel;
using Ext.Net;
using System.Globalization;
using System.Data.Linq.SqlClient;
using ScmsSoaLibraryInterface.Commons;
using ScmsSoaLibraryInterface.Core.Crypto;
using System.Data.SqlClient;

namespace ScmsSoaLibrary.Modules
{
  partial class CommonQuery
  {
    #region Internal Class
    
    internal class ListReportDetail
    {
      public int Idx { get; set; }
      public string c_entry { get; set; }
      public string c_type { get; set; }
      public DateTime d_entry { get; set; }
      public bool l_compress { get; set; }
      public byte l_download { get; set; }
      public bool l_share { get; set; }
      public string v_filetype { get; set; }
      public string v_report { get; set; }
      public string v_reportname { get; set; }
      public string v_reportusername { get; set; }
      public string v_size { get; set; }
      public DateTime d_entry_date { get; set; }
      public TimeSpan d_entry_time { get; set; }
    }

    internal class SCMS_USERWP
    {
        public string c_nip { get; set; }
        public string v_username { get; set; }
        public string c_type { get; set; }

        //public string v_password { get; set; }
        //public string x_hash { get; set; }
    }

    internal class PickerDetail
    {
        public string c_iteno { get; set; }
        public string v_itnam { get; set; }
        public string v_cunam { get; set; }
        public string c_spno { get; set; }
        public string c_batch { get; set; }
        public decimal? n_qty { get; set; }
    }
    #endregion

    public static IDictionary<string, object> ModelGridQuery(string connectionString, int start, int limit, bool allQuery, string sort, string dir, string model, IDictionary<string, Functionals.ParameterParser> parameters)
    {
      IDictionary<string, object> dic = new Dictionary<string, object>();

      ScmsModel.ORMDataContext db = new ScmsModel.ORMDataContext(connectionString);
      //db.CommandTimeout = 1000;

      int nCount = 0;
      string paternLike = null;

      try
      {
        switch (model)
        {
          #region MODEL_COMMON_QUERY_SINGLE_MSTRANSD

          case Constant.MODEL_COMMON_QUERY_SINGLE_MSTRANSD:
            {
              var qry = (from q in db.MsTransHes
                         join q1 in db.MsTransDs on new { q.c_portal, q.c_notrans } equals new { q1.c_portal, q1.c_notrans }
                         select new
                         {
                           v_ket_category = q.v_ket,
                           q1.c_notrans,
                           q1.c_portal,
                           q1.c_type,
                           q1.v_ket
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsIn)
                    {
                      #region In Clause

                      qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                      #endregion
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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_CUSTOMER

          case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMER:
            {
              var qry = (from q in db.LG_Cusmas
                         select q).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_CABANG

          case Constant.MODEL_COMMON_QUERY_SINGLE_CABANG:
            {
                var qry = (from q in db.LG_CusmasCabs
                           select q).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SEARCH_CABANG

          case Constant.MODEL_COMMON_QUERY_SEARCH_CABANG:
            {
                string cab = (parameters.ContainsKey("c_cab_dcore") ? (string)((Functionals.ParameterParser)parameters["c_cab_dcore"]).Value : string.Empty);

                var qry = (from q in db.LG_CusmasCabs
                           where q.c_cab_dcore == cab
                           select q.c_cusno).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_CUSTOMER_AND_GUDANG

          case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMER_AND_GUDANG:
            {
                var cabang = (from q in db.LG_Cusmas
                              //where q.l_cabang == true
                              select new
                              {
                                  q.c_cusno,
                                  q.v_cunam,
                                  q.c_cab
                              }).AsQueryable();

                var gudang = (from qq in db.LG_MsGudangs
                               where qq.l_aktif == true
                               select new
                               {
                                   c_cusno = qq.c_gdg.ToString(),
                                   v_cunam = qq.v_gdgdesc,
                                   c_cab = "HO" +qq.c_gdg
                               }
                               ).AsQueryable();

                var qry = cabang.Union(gudang).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_CUSTOMERDCORE Indra Monitoring Process 20180523FM

          case Constant.MODEL_COMMON_QUERY_SINGLE_CUSTOMERDCORE:
            {
                var qry = (from q in db.LG_CusmasCabs
                           where q.c_cab_dcore != null
                           select q).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_DUS

          case Constant.MODEL_COMMON_QUERY_DUS:
            {
                var dus = (from q in db.LG_MSDUS
                           select new { q.v_nama_dus, q.n_volume
                           }).AsQueryable();

                var cabang = (from q in db.LG_Cusmas
                              //where q.l_cabang == true
                              select new
                              {
                                  q.c_cusno,
                                  q.v_cunam,
                                  q.c_cab
                              }).AsQueryable();

                var gudang = (from qq in db.LG_MsGudangs
                              where qq.l_aktif == true
                              select new
                              {
                                  c_cusno = qq.c_gdg.ToString(),
                                  v_cunam = qq.v_gdgdesc,
                                  c_cab = "HO" + qq.c_gdg
                              }
                               ).AsQueryable();

                var qry = dus.AsQueryable();

                //if ((parameters != null) && (parameters.Count > 0))
                //{
                //    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                //    {
                //        if (kvp.Value.IsCondition)
                //        {
                //            if (kvp.Value.IsLike)
                //            {
                //                paternLike = kvp.Value.Value.ToString();
                //                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                //            }
                //            else if (kvp.Value.IsBetween)
                //            {

                //            }
                //            else
                //            {
                //                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                //            }
                //        }
                //    }
                //}

                Logger.WriteLine(qry.Provider.ToString());

                nCount = qry.Count();

                if (nCount > 0)
                {
                    //if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    //{
                    //    qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    //}

                    if ((limit == -1) || allQuery)
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    }
                    else
                    {
                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                    }
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_SINGLE_SUPLIER

          case Constant.MODEL_COMMON_QUERY_SINGLE_SUPLIER:
            {
              var qry = (from q in db.LG_DatSups
                         select q).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_SUPLIER_AUTO

          case Constant.MODEL_COMMON_QUERY_SINGLE_SUPLIER_AUTO:
            {
              string TipePL = (parameters.ContainsKey("TipePL") ? (string)((Functionals.ParameterParser)parameters["TipePL"]).Value : string.Empty);

              if (TipePL.Equals("06"))
              {

                var qry = (from q in db.LG_DatSups
                           join q1 in db.LG_DatSupCrossDockings on q.c_nosup equals q1.c_nosup into q_1
                           from qDSA in q_1.DefaultIfEmpty()
                           select new
                           {
                             q,
                             qDSA
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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
              else
              {
                var qry = (from q in db.LG_DatSups
                           join q1 in db.LG_DatSupAutos on q.c_nosup equals q1.c_nosup into q_1
                           from qDSA in q_1.DefaultIfEmpty()
                           select new
                           {
                             q,
                             qDSA
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_SINGLE_GUDANG

          case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG:
            {
              var qry = (from Gud in db.LG_MsGudangs
                         select Gud);

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_GUDANG_FULL

          case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG_FULL:
            {
              var qry = (from Gud in db.LG_MsGudangs
                         join q1 in db.HR_MsKotas on Gud.c_kota equals q1.c_kota into q_1
                         from qKota in q_1.DefaultIfEmpty()
                         select new
                         {
                           Gud.c_gdg,
                           Gud.v_gdgdesc,
                           Gud.v_nama,
                           Gud.v_alamat,
                           Gud.c_rt,
                           Gud.c_rw,
                           Gud.c_kodepos,
                           Gud.v_lurah,
                           Gud.v_camat,
                           Gud.v_telp,
                           Gud.l_aktif,
                           Gud.c_kota,
                           v_kota_desc = qKota.v_desc
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_GUDANG_CS

          case Constant.MODEL_COMMON_QUERY_SINGLE_GUDANG_CS:
            {
                var qry = (from Gud in db.LG_vwGudangs
                           select Gud);

                if ((parameters != null) && (parameters.Count > 0))
                {


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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_MSDIVAMS

          case Constant.MODEL_COMMON_QUERY_SINGLE_MSDIVAMS:
            {
              var qry = (from q in db.FA_MsDivAMs
                         select new
                         {
                           q.c_kddivams,
                           q.v_nmdivams,
                           q.l_aktif,
                           q.l_hide,
                           v_divams_desc = string.Concat(q.c_kddivams.Trim(), " - ", q.v_nmdivams.Trim())
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_MSDIVPRI

          case Constant.MODEL_COMMON_QUERY_SINGLE_MSDIVPRI:
            {
              var qry = (from q in db.FA_MsDivPris
                         select new
                         {
                           q.c_kddivpri,
                           q.c_nosup,
                           q.l_aktif,
                           q.l_delete,
                           q.l_hide,
                           q.n_het,
                           q.n_idxnp,
                           q.n_idxp,
                           q.v_nmdivpri
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsIn)
                    {
                      #region In Clause

                      qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                      #endregion
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_ITEM

          case Constant.MODEL_COMMON_QUERY_SINGLE_ITEM:
            {
              bool activateSuplier = (parameters.ContainsKey("activateSuplier") ? (bool)((Functionals.ParameterParser)parameters["activateSuplier"]).Value : false);

              var qry = (from q in db.FA_MasItms
                         join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup into q_1
                         from qDS in q_1.DefaultIfEmpty()
                         select new
                         {
                           q.c_alkes,
                           //q.c_entry,
                           q.c_iteno,
                           q.c_itenopri,
                           q.c_nosup,
                           q.c_type,
                           //q.c_update,
                           q.c_via,
                           //q.d_entry,
                           //q.d_update,
                           q.l_aktif,
                           q.l_berat,
                           q.l_combo,
                           q.l_dinkes,
                           q.l_gdg,
                           q.l_hide,
                           q.l_import,
                           q.l_mprice,
                           q.l_opname,
                           q.n_beli,
                           q.n_bonus,
                           q.n_box,
                           q.n_disc,
                           q.n_estimasi,
                           q.n_PBF,
                           q.n_pminord,
                           q.n_qminord,
                           q.n_qtykons,
                           q.n_salpri,
                           q.v_acronim,
                           q.v_itnam,
                           q.v_undes,
                           v_nama_suplier = (activateSuplier ? qDS.v_nama : null)
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_ITEM_NONPSIKOTROPIKA

          case Constant.MODEL_COMMON_QUERY_SINGLE_ITEM_NONPSIKOTROPIKA:
            {
                var qry = (from q in db.lg_vwitm_nonpsikos
                           select new
                           {
                               q.c_iteno,
                               q.v_itnam
                           });

                if ((parameters != null) && (parameters.Count > 0))
                {
                    foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                    {
                        if (kvp.Value.IsCondition)
                        {
                            if (kvp.Value.IsLike)
                            {
                                paternLike = kvp.Value.Value.ToString();
                                qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                            }
                            else
                            {
                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                            }
                        }
                    }
                }

                Logger.WriteLine(qry.Provider.ToString());

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_GROUP

          case Constant.MODEL_COMMON_QUERY_SINGLE_GROUP:
            {
              var qry = (from q in db.SCMS_GROUPs
                         select new
                         {
                           q.c_entry,
                           q.c_group,
                           q.c_update,
                           q.d_entry,
                           q.d_update,
                           q.l_aktif,
                           q.v_group_desc
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_REPORT

          case Constant.MODEL_COMMON_QUERY_SINGLE_REPORT:
            {
              string nipUser = (parameters.ContainsKey("nipUser") ? (string)((Functionals.ParameterParser)parameters["nipUser"]).Value : string.Empty);

              IQueryable<ListReportDetail> iqRptUser = null,
                iqRptShared = null;

              iqRptUser = (from q in db.SCMS_Reports
                           where (q.c_entry == nipUser)
                           select new ListReportDetail()
                           {
                             Idx = q.Idx,
                             c_entry = q.c_entry,
                             c_type = q.c_type,
                             d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                             l_compress = (q.l_compress.HasValue ? q.l_compress.Value : false),
                             l_download = (q.l_download.HasValue ? q.l_download.Value : (byte)0),
                             l_share = (q.l_share.HasValue ? q.l_share.Value : false),
                             v_filetype = q.v_filetype,
                             v_report = q.v_report,
                             v_reportname = q.v_reportname,
                             v_reportusername = q.v_reportusername,
                             v_size = q.v_size,
                             d_entry_date = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).Date,
                             d_entry_time = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).TimeOfDay,
                           }).Distinct().AsQueryable();

              if (string.IsNullOrEmpty(nipUser))
              {
                iqRptShared = (from q in db.SCMS_Reports
                               select new ListReportDetail()
                               {
                                 Idx = q.Idx,
                                 c_entry = q.c_entry,
                                 c_type = q.c_type,
                                 d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                 l_compress = (q.l_compress.HasValue ? q.l_compress.Value : false),
                                 l_download = (q.l_download.HasValue ? q.l_download.Value : (byte)0),
                                 l_share = (q.l_share.HasValue ? q.l_share.Value : false),
                                 v_filetype = q.v_filetype,
                                 v_report = q.v_report,
                                 v_reportname = q.v_reportname,
                                 v_reportusername = q.v_reportusername,
                                 v_size = q.v_size,
                                 d_entry_date = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).Date,
                                 d_entry_time = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).TimeOfDay,
                               }).Take(0);
              }
              else
              {
                iqRptShared = (from q in db.SCMS_Reports
                               where (q.c_entry != nipUser)
                                && ((q.l_share.HasValue ? q.l_share.Value : false) == true)
                               select new ListReportDetail()
                               {
                                 Idx = q.Idx,
                                 c_entry = q.c_entry,
                                 c_type = q.c_type,
                                 d_entry = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime),
                                 l_compress = (q.l_compress.HasValue ? q.l_compress.Value : false),
                                 l_download = (q.l_download.HasValue ? q.l_download.Value : (byte)0),
                                 l_share = (q.l_share.HasValue ? q.l_share.Value : false),
                                 v_filetype = q.v_filetype,
                                 v_report = q.v_report,
                                 v_reportname = q.v_reportname,
                                 v_reportusername = q.v_reportusername,
                                 v_size = q.v_size,
                                 d_entry_date = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).Date,
                                 d_entry_time = (q.d_entry.HasValue ? q.d_entry.Value : Functionals.StandardSqlDateTime).TimeOfDay,
                               }).Distinct().AsQueryable();
              }

              //var qry = (from q in db.SCMS_Reports
              
              var qry = iqRptUser.Union(iqRptShared).AsQueryable();
              //           select q);

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_KURS

          case Constant.MODEL_COMMON_QUERY_SINGLE_KURS:
            {
              var qry = (from q in db.FA_Kurs
                         select new
                         {
                           q.c_desc,
                           q.c_kurs,
                           //c_symbol_normal = q.c_symbol,
                           c_symbol =
                           (
                            (q.c_desc == "EUR" ? "&#8364;" :
                              (q.c_desc == "FFR" ? "&#8355;" :
                              (q.c_desc == "JPY" ? "&#165;" :
                              (q.c_desc == "GBP" ? "&#163;" :
                              (q.c_desc == "USD" || q.c_desc == "SGD" || q.c_desc == "AUD" || q.c_desc == "HKD" ? "&#36;" :
                              q.c_symbol)))))
                           ),
                           q.n_currency,
                           q.v_desc,
                           q.l_aktif,
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_KURS_NONSYMBOL

          case Constant.MODEL_COMMON_QUERY_SINGLE_KURS_NONSYMBOL:
            {
              var qry = (from kurs in db.FA_Kurs
                         select new
                         {
                           kurs.c_desc,
                           kurs.c_kurs,
                           kurs.n_currency,
                           kurs.v_desc
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_EKSPEDISI

          case Constant.MODEL_COMMON_QUERY_SINGLE_EKSPEDISI:
            {
              var qry = (from kurs in db.LG_MsExps
                         select new
                         {
                           kurs.c_exp,
                           kurs.l_aktif,
                           kurs.l_darat,
                           kurs.l_import,
                           kurs.l_laut,
                           kurs.l_udara,
                           kurs.l_npwp,
                           kurs.v_ket
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_BANK

          case Constant.MODEL_COMMON_QUERY_SINGLE_BANK:
            {
              var qry = (from q in db.FA_MsBanks
                         select new
                         {
                           q.c_bank,
                           q.c_cab1,
                           q.c_cab2,
                           q.c_cab3,
                           q.c_cab4,
                           q.v_bank,
                           q.v_bankcab,
                           q.l_delete,
                           q.l_aktif,
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_REKENING

          case Constant.MODEL_COMMON_QUERY_SINGLE_REKENING:
            {
              var qry = (from q in db.FA_MsBankReks
                         select new
                         {
                           q.c_bank,
                           q.c_glno,
                           q.c_rekno,
                           q.c_type,
                           q.v_pemilk
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_BATCH

          case Constant.MODEL_COMMON_QUERY_SINGLE_BATCH:
            {
              var qry = (from q in db.LG_MsBatches
                         select new
                         {
                           q.c_batch,
                           q.c_iteno,
                           q.d_expired
                         }).Distinct().AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_KARYAWAN

          case Constant.MODEL_COMMON_QUERY_SINGLE_KARYAWAN:
            {
              var qry = (from q in db.HR_MsKries
                         select new
                         {
                           q.l_aktif,
                           q.v_nama,
                           q.c_nip
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                

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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_KOTA

          case Constant.MODEL_COMMON_QUERY_SINGLE_KOTA:
            {
              var qry = (from q in db.HR_MsKotas
                         select q);

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_FULLITEM

          case Constant.MODEL_COMMON_QUERY_SINGLE_FULLITEM:
            {
              var qry = (from q in db.FA_MasItms
                         join q1 in db.FA_DivAMs on q.c_iteno equals q1.c_iteno into q_1
                         from qDA in q_1.DefaultIfEmpty()
                         join q2 in db.FA_Divpris on q.c_iteno equals q2.c_iteno into q_2
                         from qDP in q_2.DefaultIfEmpty()
                         select new
                         {
                           q.c_alkes,
                           //q.c_entry,
                           q.c_iteno,
                           q.c_itenopri,
                           q.c_nosup,
                           q.c_type,
                           //q.c_update,
                           q.c_via,
                           //q.d_entry,
                           //q.d_update,
                           q.l_aktif,
                           q.l_berat,
                           q.l_combo,
                           q.l_dinkes,
                           q.l_gdg,
                           q.l_hide,
                           q.l_import,
                           q.l_mprice,
                           q.l_opname,
                           q.n_beli,
                           q.n_bonus,
                           q.n_box,
                           q.n_disc,
                           q.n_estimasi,
                           q.n_PBF,
                           q.n_pminord,
                           q.n_qminord,
                           q.n_qtykons,
                           q.n_salpri,
                           q.v_acronim,
                           q.v_itnam,
                           q.v_undes,
                           qDA.c_kddivams,
                           qDP.c_kddivpri,
                         });

              if ((parameters != null) && (parameters.Count > 0))
              {
                

                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsIn)
                    {
                      #region In Clause

                      qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                      #endregion
                    }
                    else
                    {
                      qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                    }
                  }
                }
              }

              Logger.WriteLine(qry.Provider.ToString());

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_USERWP

          case Constant.MODEL_COMMON_QUERY_SINGLE_USERWP:
            {
                string c_nip = (parameters.ContainsKey("c_nip") ? (string)((Functionals.ParameterParser)parameters["c_nip"]).Value : string.Empty);
                //string c_grouped = (parameters.ContainsKey("c_grouped") ? (string)((Functionals.ParameterParser)parameters["c_grouped"]).Value : string.Empty);
                
                //var qry = (from q in db.SCMS_GROUPEDs
                //           join q1 in db.SCMS_USERs on q.c_nip equals q1.c_nip
                //           where q.c_nip == c_nip && q.c_grouped == c_grouped && q1.l_aktif == true
                //           select new SCMS_USERWP
                //           {
                //               c_nip = q.c_nip,
                //               v_username = q1.v_username,
                //               v_password = q1.v_password,
                //               x_hash = q1.x_hash
                //           }).AsQueryable();

                var qry = (from q in db.SCMS_USER_STs
                           where q.c_nip == c_nip
                           select new SCMS_USERWP
                           {
                               c_nip = q.c_nip,
                               v_username = q.v_nama,
                               c_type = q.c_type
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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

                nCount = qry.Count();

                if (nCount > 0)
                {
                    List<SCMS_USERWP> lists = null;

                    if ((!string.IsNullOrEmpty(sort)) && (!string.IsNullOrEmpty(dir)))
                    {
                        qry = qry.OrderBy(string.Format("{0} {1}", sort, dir).Trim());
                    }

                    if ((limit == -1) || allQuery)
                    {
                        //dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.ToList());
                        lists = qry.ToList();

                        //for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                        //{
                        //    lists[nLoop].v_password = GlobalCrypto.Crypt1WayMD5String(string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                        //}

                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                    }
                    else
                    {
                        //dic.Add(Constant.DEFAULT_NAMING_RECORDS, qry.Skip(start).Take(limit).ToList());

                        lists = qry.Skip(start).Take(limit).ToList();

                        //for (int nLoop = 0; nLoop < lists.Count; nLoop++)
                        //{
                        //    lists[nLoop].v_password = GlobalCrypto.Crypt1WayMD5String(string.IsNullOrEmpty(lists[nLoop].v_password) ? "" : Functionals.DecryptHashRjdnl(lists[nLoop].x_hash, lists[nLoop].v_password));
                        //}

                        dic.Add(Constant.DEFAULT_NAMING_RECORDS, lists);
                    }
                }

                dic.Add(Constant.DEFAULT_NAMING_TOTAL_ROWS, nCount);

                dic.Add(Constant.DEFAULT_NAMING_SUCCESS, true);
            }
            break;

          #endregion

          #region MODEL_COMMON_QUERY_SINGLE_WP_PICKER_DETAIL

          case Constant.MODEL_COMMON_QUERY_SINGLE_WP_PICKER_DETAIL:
            {
                string c_no = (parameters.ContainsKey("c_no") ? (string)((Functionals.ParameterParser)parameters["c_no"]).Value : string.Empty);

                var qryPL = (from q in db.LG_PLD2s
                             join q1 in db.LG_PLHs on q.c_plno equals q1.c_plno
                             join q2 in db.LG_Cusmas on q1.c_cusno equals q2.c_cusno
                           where q.c_plno == c_no
                           group q by new { q.c_iteno, q.c_batch, q.c_spno, q2.v_cunam } into g
                           select new PickerDetail
                           {
                               c_iteno = g.Key.c_iteno,
                               c_spno = g.Key.c_spno,
                               v_cunam = g.Key.v_cunam,
                               c_batch = g.Key.c_batch,
                               n_qty = g.Sum(t => t.n_qty)
                           }).AsQueryable();

                var qrySJ = (from q in db.LG_SJD2s
                             join q1 in db.LG_SJHs on q.c_sjno equals q1.c_sjno
                             join q2 in db.LG_MsGudangs on q1.c_gdg2 equals q2.c_gdg
                             where q.c_sjno == c_no
                             group q by new { q.c_iteno, q.c_batch, q.c_spgno, q2.v_gdgdesc} into g
                             select new PickerDetail
                             {
                                 c_iteno = g.Key.c_iteno,
                                 c_spno =  g.Key.c_spgno,
                                 v_cunam = g.Key.v_gdgdesc,
                                 c_batch = g.Key.c_batch,
                                 n_qty = g.Sum(t=> t.n_gqty)
                             }).AsQueryable();

                var qryAll = qrySJ.Union(qryPL).Distinct().AsQueryable();

                var qry = (from q in qryAll
                           join q1 in db.FA_MasItms on q.c_iteno equals q1.c_iteno
                           select new PickerDetail
                           {
                               c_iteno = q.c_iteno,
                               v_itnam = q1.v_itnam,
                               v_cunam = q.v_cunam,
                               c_spno = q.c_spno,
                               c_batch = q.c_batch,
                               n_qty = q.n_qty
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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
                            {}
                            else
                            {
                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                            }
                        }
                    }
                }

                Logger.WriteLine(qry.Provider.ToString());

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_WP_PACKER_DETAIL

          case Constant.MODEL_COMMON_QUERY_SINGLE_WP_PACKER_DETAIL:
            {
                string c_no = (parameters.ContainsKey("c_no") ? (string)((Functionals.ParameterParser)parameters["c_no"]).Value : string.Empty);

                var qry = (from q in db.LG_DOHs
                           join q1 in db.LG_DOD2s on q.c_dono equals q1.c_dono
                           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                           join q3 in db.LG_Cusmas on q.c_cusno equals q3.c_cusno
                           where q.c_dono == c_no
                           select new PickerDetail
                           {
                               c_iteno = q1.c_iteno,
                               v_itnam = q2.v_itnam,
                               v_cunam = q3.v_cunam,
                               c_spno = q.c_plno,
                               c_batch = q1.c_batch,
                               n_qty = q1.n_qty
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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
                            { }
                            else
                            {
                                qry = qry.Where(kvp.Key, kvp.Value.Value).AsQueryable();
                            }
                        }
                    }
                }

                Logger.WriteLine(qry.Provider.ToString());

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_USER_ST

          case Constant.MODEL_COMMON_QUERY_SINGLE_USER_ST:
            {
              var qry = (from q in db.SCMS_USER_STs
                         select new
                         {
                           q.c_nip,
                           q.v_nama,
                           q.c_type,
                         }).AsQueryable();

              if ((parameters != null) && (parameters.Count > 0))
              {
                foreach (KeyValuePair<string, Functionals.ParameterParser> kvp in parameters)
                {
                  if (kvp.Value.IsCondition)
                  {
                    if (kvp.Value.IsLike)
                    {
                      paternLike = kvp.Value.Value.ToString();
                      qry = qry.Like(kvp.Key, paternLike).AsQueryable();
                    }
                    else if (kvp.Value.IsIn)
                    {
                      #region In Clause

                      qry = qry.In(kvp.Key, (object[])kvp.Value.Value).AsQueryable();

                      #endregion
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

              nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_OUTLET

          case Constant.MODEL_COMMON_QUERY_SINGLE_OUTLET:
            {
                var qry = (from q in db.SCMS_MSOUTLET_CABANGs
                           select q).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_REASON_RETUR

          case Constant.MODEL_COMMON_QUERY_SINGLE_REASON_RETUR:
            {
                var qry = (from q in db.SCMS_MSRETUR_REASONs
                           select q).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {
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

                nCount = qry.Count();

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

          //Indra 20181019FM
          #region MODEL_COMMON_QUERY_SINGLE_MSBATCH 

          case Constant.MODEL_COMMON_QUERY_SINGLE_MSBATCH:
            {
                string itemCode = (parameters.ContainsKey("itemCode") ? (string)((Functionals.ParameterParser)parameters["itemCode"]).Value : string.Empty);

                using (SqlConnection cn = new SqlConnection(Functionals.ActiveConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = cn.CreateCommand();
                    cmd.CommandText = "exec LG_VW_MSBATCH_GRID '" + itemCode + "'";
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }

                var qry = (from q in db.LG_MSBATCH_GRIDs
                           select new
                           {
                               q.KOLOM_1,
                               q.KOLOM1,
                               q.KOLOM_2,
                               q.KOLOM2,
                               q.KOLOM_3,
                               q.KOLOM3,
                               q.KOLOM_4,
                               q.KOLOM4
                           }).AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {

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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_TRANS_RECALL

          case Constant.MODEL_COMMON_QUERY_SINGLE_TRANS_RECALL:
            {
                string recallno = (parameters.ContainsKey("recallno") ? (string)((Functionals.ParameterParser)parameters["recallno"]).Value : string.Empty);

                var qry = (from q in db.LG_RECALLHs
                           join q1 in db.LG_RECALLDs on q.c_recalno equals q1.c_recallno
                           join q2 in db.FA_MasItms on q1.c_iteno equals q2.c_iteno
                           where q.c_recalno.Contains(recallno)
                           select new
                           {
                               q.c_recalno,
                               q.d_recall_from,
                               q.d_recall_start,
                               q.d_recall_to,
                               q1.c_iteno,
                               q2.v_itnam
                           }).Distinct().AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {

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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_RECEIVE_NOTE

          case Constant.MODEL_COMMON_QUERY_SINGLE_RECEIVE_NOTE:
            {
                char gdg = (parameters.ContainsKey("gdg") ? (char)((Functionals.ParameterParser)parameters["gdg"]).Value : char.MinValue);
                string receiveId = (parameters.ContainsKey("receiveId") ? (string)((Functionals.ParameterParser)parameters["receiveId"]).Value : string.Empty);
                var qry = (from a in db.LG_RNHs where a.c_gdg == gdg && a.c_rnno == receiveId select a).Distinct().AsQueryable();

                if ((parameters != null) && (parameters.Count > 0))
                {

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

                nCount = qry.Count();

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

          #region MODEL_COMMON_QUERY_SINGLE_HARD_TAX

          case Constant.MODEL_COMMON_QUERY_SINGLE_HARD_TAX:
            {
                string handler = (parameters.ContainsKey("handler") ? (string)((Functionals.ParameterParser)parameters["handler"]).Value : string.Empty);
                string tglBerlaku = (parameters.ContainsKey("tglBerlaku") ? (string)((Functionals.ParameterParser)parameters["tglBerlaku"]).Value : string.Empty);
                var qry = (from a in db.hard_taxes where a.handler == handler && a.d_berlaku <= DateTime.Parse(tglBerlaku ?? "1990-01-01") select a).OrderByDescending(b => b.d_berlaku).Take(1);

                if ((parameters != null) && (parameters.Count > 0))
                {

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

                nCount = qry.Count();

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
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaLibrary.Modules.CommonQuery:ModelGridQuery <-> Switch {0} - {1}", model, ex.Message);
        Logger.WriteLine(ex.StackTrace);

        dic.Add(Constant.DEFAULT_NAMING_EXCEPTION, ex.Message);

        dic.Add(Constant.DEFAULT_NAMING_SUCCESS, false);

        Logger.WriteLine(ex.Message);
      }

      db.Dispose();

      return dic;
    }
  }
}