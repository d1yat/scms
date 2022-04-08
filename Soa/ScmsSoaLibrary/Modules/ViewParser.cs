using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScmsSoaLibrary.Modules
{
  class LG_vwStock
  {
    public string c_table
    { get; set; }
    public char c_gdg
    { get; set; }
    public string c_no
    { get; set; }
    public string c_iteno
    { get; set; }
    public DateTime? d_date
    { get; set; }
    public string c_type
    { get; set; }
    public string c_batch
    { get; set; }
    public decimal? n_gsisa
    { get; set; }
    public decimal? n_bsisa
    { get; set; }
  }

  class LG_vwStockLite
  {
    public string c_table
    { get; set; }
    public char c_gdg
    { get; set; }
    public string c_no
    { get; set; }
    public string c_iteno
    { get; set; }
    public DateTime d_date
    { get; set; }
    public string c_type
    { get; set; }
    public string c_batch
    { get; set; }
    public decimal n_gsisa
    { get; set; }
    public decimal n_bsisa
    { get; set; }
  }


  class LG_vwStockBatch
  {
    public char c_gdg
    { get; set; }
    public string c_iteno
    { get; set; }
    public string c_batch
    { get; set; }
    public decimal? N_GSISA
    { get; set; }
    public decimal? N_BSISA
    { get; set; }
    public decimal? N_GSISATOTAL
    { get; set; }
    public decimal? N_BSISATOTAL
    { get; set; }
  }

  class LG_vwSPPending
  {
    public char c_gdg
    { get; set; }
    public string c_cusno
    { get; set; }
    public string c_iteno
    { get; set; }
    public string c_spno
    { get; set; }
    public DateTime? d_spdate
    { get; set; }
    public string v_cunam
    { get; set; }
    public string c_sp
    { get; set; }
    public string c_type
    { get; set; }
    public decimal? n_sisaSP
    { get; set; }
    public decimal? n_sisaType
    { get; set; }
    public decimal? n_sisa
    { get; set; }
    public decimal? n_pending
    { get; set; }
    public decimal? n_sisaGudang
    { get; set; }
  }

  class LG_vwSPPending_New
  {
    public char c_gdg
    { get; set; }
    public string c_cusno
    { get; set; }
    public string c_iteno
    { get; set; }
    public string c_spno
    { get; set; }
    public DateTime d_spdate
    { get; set; }
    public string v_cunam
    { get; set; }
    public string c_sp
    { get; set; }
    public string c_type
    { get; set; }
    public decimal n_pending
    { get; set; }
    public DateTime d_entry
    { get; set; }
  }

  class LG_vwPOPending_New
  {
    public char c_gdg
    { get; set; }
    public string c_pono
    { get; set; }
    public string c_nosup
    { get; set; }
    public string c_iteno
    { get; set; }
    public DateTime? d_podate
    { get; set; }
    public decimal n_sisa
    { get; set; }
    public bool l_import
    { get; set; }
  }

  class LG_vwAvgSales
  {
    public short? n_Tahun
    { get; set; }
    public string c_iteno
    { get; set; }
    public sbyte? n_Bulan
    { get; set; }
    public decimal? n_sales
    { get; set; }
    public decimal? n_retur
    { get; set; }
    public string c_cusno
    { get; set; }
  }
}
