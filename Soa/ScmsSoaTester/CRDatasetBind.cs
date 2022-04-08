using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ScmsModel;
using ScmsModel.Core;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaTester
{
  class CRDatasetBind
  {
    internal class DSNG_SCMS_QuerySales
    {
      public string c_fjno { get; set; }
      public string d_fjdate { get; set; }
      public string c_sektor { get; set; }
      public string c_cusno { get; set; }
      public string v_cunam { get; set; }
      public string c_iteno { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public decimal n_disc { get; set; }
      public string c_kddivams { get; set; }
      public string v_nmdivams { get; set; }
      public bool l_clinic { get; set; }
      public bool l_cabang { get; set; }
      public string v_ket { get; set; }

    }
    
    public System.Data.DataSet ReportSales(ORMDataContext db, DateTime date1, DateTime date2)
    {
      //DateTime date1 = Convert.ToDateTime("2010-11-01");
      //DateTime date2 = Convert.ToDateTime("2010-11-10");

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      var ListFJ = (from q in db.LG_FJHs
                    join q1 in db.LG_FJD1s on q.c_fjno equals q1.c_fjno
                    where ((q.d_fjdate >= date1) && (q.d_fjdate.Value <= date2))
                    select new
                    {
                      q.c_fjno,
                      q.c_cusno,
                      q.d_fjdate,
                      q1.c_iteno,
                      q1.n_qty,
                      q1.n_salpri,
                      q1.n_disc
                    }).Distinct().ToList();


      var ListTran = (from q in db.MsTransDs
                      join q1 in db.LG_Cusmas on q.c_type equals q1.c_sektor
                      where (q.c_notrans == "59") && (q.c_portal == '3')
                      select new
                      {
                        q1.c_cusno,
                        q1.v_cunam,
                        q1.c_sektor,
                        q.c_type,
                        q.v_ket
                      }).Distinct().ToList();

      var ListTranAMS = (from q in db.FA_DivAMs
                         join q1 in db.FA_MsDivAMs on q.c_kddivams equals q1.c_kddivams
                         select new
                         {
                           q.c_iteno,
                           q1.c_kddivams,
                           q1.v_nmdivams
                         }).Distinct().ToList();

      var ListQuery = (from q in ListFJ
                       join q2 in ListTran on q.c_cusno equals q2.c_cusno
                       join q3 in ListTranAMS on q.c_iteno equals q3.c_iteno
                       select new DSNG_SCMS_QuerySales()
                       {
                         c_fjno = q.c_fjno,
                         c_cusno = q.c_cusno,
                         d_fjdate = (q.d_fjdate.HasValue ? q.d_fjdate.Value.ToString("yyyy-MM-dd") : Functionals.StandardSqlDateTime.ToString("yyyy-MM-dd")),
                         c_iteno = q.c_iteno,
                         n_qty = (q.n_qty.HasValue ? q.n_qty.Value : 0),
                         n_salpri = (q.n_salpri.HasValue ? q.n_salpri.Value : 0),
                         n_disc = (q.n_disc.HasValue ? q.n_disc.Value : 0),
                         c_sektor = q2.c_sektor,
                         v_cunam = q2.v_cunam,
                         v_ket = q2.v_ket,
                         c_kddivams = q3.c_kddivams,
                         v_nmdivams = q3.v_nmdivams,
                         l_cabang = (q2.c_sektor == "01" ? true : false),
                         l_clinic = (q2.c_sektor == "03" ? true : false),
                       }).ToList();


      try
      {
        //table = ListQuery.CopyToDataTableObject();

        if (table != null)
        {
          dataSet = new System.Data.DataSet();

          dataSet.Tables.Add(table);
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine(
          "ScmsSoaTester.CRDatasetBind:ReportSales - {0}", ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      ListFJ.Clear();
      ListTran.Clear();
      ListTranAMS.Clear();
      ListQuery.Clear();

      return dataSet;
    }
  }
}