//Indra 20190411FM Penambahan modul produk kosong

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class ProsesEmailProdukKosong : Scms.Web.Core.PageHandler
{
  private void ClearEntrys()
  {
      hidWndDown.Text           = storeGridEPK.ClientID;
      winTambahProduk.Title     = "";
      hfPKNo.Clear();

      cbSuplier.Text            = "";
      cbSuplier.ReadOnly        = false;
      cbDivPrinsipal.Text       = "";
      cbDivPrinsipal.ReadOnly   = false;
      cbItems.Text              = "";
      cbItems.ReadOnly          = false;
      rdgJenisSP.Disabled       = false;
      dtABE.MinDate             = DateTime.Now.AddDays(1);
      dtABE.Value               = DateTime.Now.AddDays(1);
      dtExpired.MinDate         = DateTime.Now.AddDays(1);
      dtExpired.Value           = DateTime.Now.AddDays(1);
      chkSendEmail.Checked      = true;
      chkAktif.Checked          = true;
      chkAktif.ReadOnly         = true;
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
        ClearEntrys();
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("EditDataProduk", StringComparison.OrdinalIgnoreCase))
    {
        if (!this.IsAllowEdit)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mengubah data.");
            return;
        }

        PopulateDetail("", pID);
    }
    else
    {
        if (!this.IsAllowAdd)
        {
            Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
            return;
        }
    }
    GC.Collect();
  }

  private void PopulateDetail(string pName, string pID)
  {
      Dictionary<string, object> dicResult = null;
      Dictionary<string, string> dicResultInfo = null;
      Newtonsoft.Json.Linq.JArray jarr = null;

      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      string[][] paramX = new string[][]{
        new string[] { "c_pkno = @0", pID, "System.String"}
      };

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      #region Parser Header

      string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0231", paramX);

      System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

      try
      {
          dicResult = JSON.Deserialize<Dictionary<string, object>>(res);
          if (dicResult.ContainsKey("records") && (dicResult.ContainsKey("totalRows") && (((long)dicResult["totalRows"]) > 0)))
          {
              winTambahProduk.Hidden = false;
              ClearEntrys();

              jarr = new Newtonsoft.Json.Linq.JArray(dicResult["records"]);

              string tgl = null, tipe = null;
              DateTime date = DateTime.Now;

              dicResultInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

              tipe = ((dicResultInfo.ContainsKey("c_tipe") ? dicResultInfo["c_tipe"] : string.Empty));
              if (tipe == "PK")
              {
                  winTambahProduk.Title = string.Format("Produk Kosong - {0}", pID);
                  tgl = ((dicResultInfo.ContainsKey("pkdt") ? dicResultInfo["pkdt"] : string.Empty));
                  rdProdukKosong.Checked = true;
              }
              else
              {
                  winTambahProduk.Title = string.Format("Nearly Expired - {0}", pID);
                  tgl = ((dicResultInfo.ContainsKey("nedt") ? dicResultInfo["nedt"] : string.Empty));
                  rdProdukExpired.Checked = true;
              }
              rdgJenisSP.Disabled = true;

              cbSuplier.Text = ((dicResultInfo.ContainsKey("v_nama") ? dicResultInfo["v_nama"] : string.Empty));
              cbSuplier.ReadOnly = true;

              cbDivPrinsipal.Text = ((dicResultInfo.ContainsKey("v_nmdivpri") ? dicResultInfo["v_nmdivpri"] : string.Empty));
              cbDivPrinsipal.ReadOnly = true;

              cbItems.Text = ((dicResultInfo.ContainsKey("v_itnam") ? dicResultInfo["v_itnam"] : string.Empty));
              cbItems.ReadOnly = true;
              
              date = Functional.JsonDateToDate(tgl);
              dtABE.Text = date.ToString("dd-MM-yyyy");
              dtExpired.Text = date.ToString("dd-MM-yyyy");

              chkSendEmail.Value = (dicResultInfo.GetValueParser<bool>("l_sent") ? chkSendEmail.Checked = true : chkSendEmail.Checked = false);
              chkSendEmail.ReadOnly = false;

              chkAktif.Value = (dicResultInfo.GetValueParser<bool>("l_aktif") ? chkAktif.Checked = true : chkAktif.Checked = false);
              chkAktif.ReadOnly = false;

              jarr.Clear();
              hfPKNo.Text = pID;
          }
      }
      catch (Exception ex)
      {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("ProsesEmailProdukKosong:PopulateDetail Header - ", ex.Message));
      }
      finally
      {
          if (jarr != null)
          {
              jarr.Clear();
          }
          if (dicResultInfo != null)
          {
              dicResultInfo.Clear();
          }
          if (dicResult != null)
          {
              dicResult.Clear();
          }
      }

      #endregion

      GC.Collect();
  }

  #region Tambah produk kosong

  protected void BtnAdd_Click(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      winTambahProduk.Hidden = false;
      ClearEntrys();
  }

  protected void btnbtnSimpan_OnClick(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

      string strProduk, Tipeinput;
      bool bSendEmail, bAktif;

      if (isAdd)
      {
          if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
          {
              Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
              return;
          }
      }
      else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
          return;
      }

      if (cbItems.SelectedItem.Text == "")
      {
          Functional.ShowMsgError("Produk belum diisi, harap isi produk terlebih dahulu");
          return;
      }
      strProduk = cbItems.SelectedItem.Value;

      bSendEmail    = chkSendEmail.Checked;
      bAktif        = chkAktif.Checked;
      if (rdProdukKosong.Checked)
      {
          Tipeinput = "01";
      }
      else
      {
          Tipeinput = "02";
      }
      PostDataParser.StructureResponse respon = SaveParserAddProdukKosong(isAdd, numberId, strProduk, bSendEmail, bAktif, Tipeinput);

      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              string scrpt = null;
              string storeId = hidWndDown.Text;

              string dateJs = null, NmPrincipal, DivPrincipal, KdItem, NmItm, NmPenginput;
              DateTime ABE = DateTime.Today;
              DateTime inputdate = DateTime.Today;
              bool Send = false;

              if (isAdd)
              {
                  if (respon.Values != null)
                  {
                      numberId = respon.Values.GetValueParser<string>("PKno", string.Empty);

                      if (!string.IsNullOrEmpty(storeId))
                      {
                          inputdate     = DateTime.Now;
                          NmPrincipal   = respon.Values.GetValueParser<string>("Pemasok", string.Empty);
                          DivPrincipal  = respon.Values.GetValueParser<string>("DivPemasok", string.Empty);
                          KdItem        = cbItems.SelectedItem.Value;
                          NmItm         = cbItems.SelectedItem.Text;

                          if (!Functional.DateParser(dtABE.RawText, "dd-MM-yyyy", out inputdate))
                          {
                              ABE = DateTime.Now;
                          }

                          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                            'c_pkno': '{1}',
                            'd_pkdate': {2},
                            'v_nama': '{3}',
                            'v_nmdivpri': '{4}',
                            'c_iteno': '{5}',
                            'v_itnam': '{6}',
                            'd_abe': {7}
                          }}));{0}.commitChanges();", storeId, numberId, 
                                                               Functional.DateToJson(inputdate),
                                                               NmPrincipal,
                                                               DivPrincipal,
                                                               KdItem,
                                                               NmItm,
                                                               Functional.DateToJson(ABE));

                          X.AddScript(scrpt);
                      }
                  }
              }

              PopulateDetail("c_pkno", numberId);
              Functional.ShowMsgInformation("Data berhasil tersimpan.");
          }
          else
          {
              e.ErrorMessage = respon.Message;

              e.Success = false;
          }
      }
      else
      {
          e.ErrorMessage = "Unknown response";

          e.Success = false;
      }
  }

  private PostDataParser.StructureResponse SaveParserAddProdukKosong(bool isAdd, string numberId, string strProduk, bool bSendEmail, bool bAktif, string Tipeinput)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = numberId;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      DateTime TimeABE = DateTime.MinValue;

      dic.Add("Pkno", pair);
      pair.DicAttributeValues.Add("Produk", strProduk);
      pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
      pair.DicAttributeValues.Add("SendEmail", bSendEmail == true ? "1" : "0");
      TimeABE = DateTime.Parse(Tipeinput == "01" ? dtABE.Text : dtExpired.Text);
      pair.DicAttributeValues.Add("strABE", TimeABE.ToString("yyyyMMdd"));
      pair.DicAttributeValues.Add("Aktif", bAktif == true ? "1" : "0");
      pair.DicAttributeValues.Add("Tipeinput", Tipeinput);

      try
      {
          varData = parser.ParserData("ProsesEmailProdukKosong", (isAdd ? "Add" : "Modify"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("ProsesEmailProdukKosong SaveParser : {0} ", ex.Message);
      }

      string result = null;

      if (!string.IsNullOrEmpty(varData))
      {
          Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

          result = soa.PostData(varData);

          responseResult = parser.ResponseParser(result);
      }

      return responseResult;
  }

  #endregion

  #region Send email

  protected void BtnSend_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? true : false);

      if (isAdd)
      {
          if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
          {
              Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
              return;
          }
      }
      else if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowEdit)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menyimpan data.");
          return;
      }

      PostDataParser.StructureResponse respon = SaveParserSendProdukKosong(isAdd, numberId);

      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              string scrpt = null;
              string storeId = hidWndDown.Text;

              string dateJs = null, NmPrincipal, DivPrincipal, KdItem, NmItm, NmPenginput;
              DateTime ABE = DateTime.Today;
              DateTime inputdate = DateTime.Today;
              bool Send = false;

              if (isAdd)
              {
                  if (respon.Values != null)
                  {
                      numberId = respon.Values.GetValueParser<string>("PKno", string.Empty);

                      if (!string.IsNullOrEmpty(storeId))
                      {
                          inputdate = DateTime.Now;
                          NmPrincipal = respon.Values.GetValueParser<string>("Pemasok", string.Empty);
                          DivPrincipal = respon.Values.GetValueParser<string>("DivPemasok", string.Empty);
                          KdItem = cbItems.SelectedItem.Value;
                          NmItm = cbItems.SelectedItem.Text;

                          if (!Functional.DateParser(dtABE.RawText, "dd-MM-yyyy", out inputdate))
                          {
                              ABE = DateTime.Now;
                          }

                          scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                            'c_pkno': '{1}',
                            'd_pkdate': {2},
                            'v_nama': {3},
                          }}));{0}.commitChanges();", storeId, numberId,
                                                               Functional.DateToJson(inputdate),
                                                               NmPrincipal);

                          X.AddScript(scrpt);
                      }
                  }
              }

              Functional.ShowMsgInformation("Proses email data berhasil.");
          }
          else
          {
              e.ErrorMessage = respon.Message;

              e.Success = false;
          }
      }
      else
      {
          e.ErrorMessage = "Unknown response";

          e.Success = false;
      }
  }

  private PostDataParser.StructureResponse SaveParserSendProdukKosong(bool isAdd, string numberId)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("id-ID");

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = numberId;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      dic.Add("Pkno", pair);
      try
      {
          varData = parser.ParserData("ProsesEmailProdukKosong", (isAdd ? "Email" : "Modify"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("ProsesEmailProdukKosong SaveParser : {0} ", ex.Message);
      }

      string result = null;

      if (!string.IsNullOrEmpty(varData))
      {
          Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

          result = soa.PostData(varData);

          responseResult = parser.ResponseParser(result);
      }

      return responseResult;
  }

  #endregion

  #region Delete data

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string PkNumber, string keterangan)
  {
      if (!((Scms.Web.Core.PageHandler)this.Page).IsAllowDelete)
      {
          Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menghapus data.");
          return;
      }

      if (string.IsNullOrEmpty(PkNumber))
      {
          Functional.ShowMsgWarning("No. Dokumen produk kosong tidak terbaca.");
          //Ext.Net.ResourceManager.AjaxSuccess = false;
          //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor SP tidak terbaca.";

          return;
      }
      else if (string.IsNullOrEmpty(keterangan))
      {
          Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");

          return;
      }

      PostDataParser.StructureResponse respon = DeleteParser(PkNumber, keterangan);

      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
          //X.AddScript(
          //  string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
          //  gridMain.ClientID, PkNumber));

          Functional.ShowMsgInformation(string.Format("No. Dokumen produk kosong '{0}' telah terhapus.", PkNumber));
      }
      else
      {
          Functional.ShowMsgWarning(respon.Message);
          //Ext.Net.ResourceManager.AjaxSuccess = false;
          //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
      }
  }

  private PostDataParser.StructureResponse DeleteParser(string PkNumber, string ket)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = PkNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      string varData = null;

      dic.Add("Pkno", pair);
      pair.DicAttributeValues.Add("Entry", this.Nip);

      pair.DicAttributeValues.Add("Keterangan", ket.Trim());

      try
      {
          varData = parser.ParserData("ProsesEmailProdukKosong", "Delete", dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("ProsesEmailProdukKosong DeleteParser : {0} ", ex.Message);
      }

      string result = null;

      if (!string.IsNullOrEmpty(varData))
      {
          Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

          result = soa.PostData(varData);

          responseResult = parser.ResponseParser(result);
      }

      return responseResult;
  }

  #endregion

  #region History Produk kosong

  protected void BtnHistory_Click(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      winHistoryProduk.Hidden = false;

  }

  #endregion

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      if (!Functional.CanCreateGenerateReport(pag))
      {
          return;
      }

      ReportParser rptParse = new ReportParser();

      List<ReportParameter> lstRptParam = new List<ReportParameter>();
      List<ReportCustomizeText> lstCustTxt = new List<ReportCustomizeText>();

      DateTime date1 = DateTime.Today,
        date2 = DateTime.Today;
      List<string> lstData = new List<string>();
      //string tmp = null;
      bool isAsync = false;

      rptParse.ReportingID = "0231-c";

      #region SQL Parameter

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(char).FullName,
          ParameterName = "Entry",
          IsSqlParameter = true,
          ParameterValue = pag.Nip
      });

      lstRptParam.Add(new ReportParameter()
      {
          DataType = typeof(string).FullName,
          ParameterName = "LG_TEMPRPTPRODUKKOSONG.NIP",
          ParameterValue = pag.Nip
      });

      #endregion

      rptParse.PaperID = "A3";
      rptParse.ReportCustomizeText = lstCustTxt.ToArray();
      rptParse.ReportParameter = lstRptParam.ToArray();
      rptParse.IsLandscape = false;
      rptParse.User = pag.Nip;

      rptParse.OutputReport = ReportParser.ParsingOutputReport("02");
      rptParse.IsShared = false;
      rptParse.UserDefinedName = "";

      string xmlFiles = ReportParser.Deserialize(rptParse);

      SoaReportCaller soa = new SoaReportCaller();

      string result = soa.GeneratorReport(isAsync, xmlFiles);

      ReportingResult rptResult = ReportingResult.Serialize(result);

      if (rptResult == null)
      {
          Functional.ShowMsgError("Pembuatan report gagal.");
      }
      else
      {
          if (rptResult.IsSuccess)
          {
              if (isAsync)
              {
                  Functional.ShowMsgInformation("Report sedang dibuat, silahkan cek di halaman report beberapa saat lagi.");
              }
              else
              {
                  string rptName = string.Concat("ProsesEmailProdukKosong_", pag.Nip, ".", rptResult.Extension);

                  string tmpUri = Functional.UriDownloadGenerator(pag, rptResult.OutputFile, rptName, rptResult.Extension);

                  wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
              }
          }
          else
          {
              Functional.ShowMsgWarning(rptResult.MessageResponse);
          }
      }

      GC.Collect();
  }
}
