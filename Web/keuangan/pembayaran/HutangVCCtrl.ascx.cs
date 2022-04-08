using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;

public partial class keuangan_pembayaran_HutangVCCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    txTanggalHdr.Clear();

    cbTipeBayarHdr.Clear();

    cbSuplierHdr.Clear();

    cbTipeHdr.Clear();

    cbBankHdr.Clear();

    cbRekeningHdr.Clear();

    txGiroHdr.Clear();
    txGiroHdr.Disabled = false;

    txTempoGiroHdr.Clear();
    txTempoGiroHdr.Disabled = false;

    cbKursHdr.Clear();
    txKursValueHdr.Clear();

    txJumlahHdr.Clear();

    lbSisaHdr.Text = "";

    txBiayaAdminHdr.Clear();
    chkDPHdr.Checked = false;

    txKeteranganHdr.Clear();

    Ext.Net.Store store = gridDetailBayarVC.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }

    X.AddScript(string.Format("if(!Ext.isEmpty({0})) {{ {0}.removeAll(); }}; if(!Ext.isEmpty(valuSisa)) {{ valuSisa = 0; }}; if(!Ext.isEmpty(valuSisaAwal)) {{ valuSisaAwal = 0; }};", bufferStore.ClientID));
  }

  private PostDataParser.StructureResponse SaveParser(string numberID, Dictionary<string, string>[] dics, string tanggalNote, string jenisBayar, string suplierId, string tipeBayar, string bankID, string rekNo, string giroID, string giroDate, string kursID, string kursValue, string jumlahTransaksi, string sisaTransaksi, string biayaAdmin, string isDP, string ketHeader, out string jumlahTransaksiOut)
  {
    jumlahTransaksiOut = string.Empty;

    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = numberID;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    decimal jmlBayar = 0,
      sumBayar = 0;
    string tmp = null,
      fakturID = null,
      tipeId = null,
      varData = null;

    DateTime date = DateTime.Today;

    dic.Add("ID", pair);

    pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
    pair.DicAttributeValues.Add("PaymentType", jenisBayar);
    pair.DicAttributeValues.Add("Suplier", suplierId);
    pair.DicAttributeValues.Add("PaymentModeType", tipeBayar);
    pair.DicAttributeValues.Add("Bank", bankID);
    pair.DicAttributeValues.Add("Account", rekNo);
    pair.DicAttributeValues.Add("GiroNumber", giroID);

    if (!Functional.DateParser(tanggalNote, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("NoteDate", date.ToString("yyyyMMddHHmmssfff"));
    
    if (!Functional.DateParser(giroDate, "dd-MM-yyyy", out date))
    {
      date = DateTime.MinValue;
    }
    pair.DicAttributeValues.Add("GiroDate", date.ToString("yyyyMMddHHmmssfff"));

    pair.DicAttributeValues.Add("Kurs", kursID);
    pair.DicAttributeValues.Add("KursValue", kursValue);
    pair.DicAttributeValues.Add("AdminTax", biayaAdmin);
    pair.DicAttributeValues.Add("IsDownPayment", isDP.ToLower());
    pair.DicAttributeValues.Add("Keterangan", ketHeader);
    
    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      dicAttr.Add("New", "true");
      dicAttr.Add("Delete", "false");
      dicAttr.Add("Modified", "false");

      fakturID = dicData.GetValueParser<string>("Faktur");
      tipeId = dicData.GetValueParser<string>("v_type");
      jmlBayar = dicData.GetValueParser<decimal>("Pembayaran", 0);

      if ((!string.IsNullOrEmpty(fakturID)) &&
        ((tipeId.Equals("01") && jmlBayar > 0) ||
          (tipeId.Equals("02") && jmlBayar < 0)))
      {
        dicAttr.Add("Faktur", fakturID);
        dicAttr.Add("TipeFaktur", tipeId);
        dicAttr.Add("Jumlah", jmlBayar.ToString());
        
        if (tipeBayar.Equals("02") && tipeId.Equals("02"))
        {
          sumBayar -= jmlBayar;
        }

        pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
        {
          IsSet = true,
          DicAttributeValues = dicAttr
        });
      }

      dicData.Clear();
    }

    if (tipeBayar.Equals("01"))
    {
      jumlahTransaksiOut = jumlahTransaksi;

      pair.DicAttributeValues.Add("ValuePay", jumlahTransaksi);
    }
    else if (tipeBayar.Equals("02"))
    {
      jumlahTransaksiOut = sumBayar.ToString();

      pair.DicAttributeValues.Add("ValuePay", sumBayar.ToString());
    }

    try
    {
      varData = parser.ParserData("VoucherDebit", "Add", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_HutangVCCtrl SaveParser : {0} ", ex.Message);
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

  public void Initialize(string storeIDGridMain)
  {
    hfStoreID.Text = storeIDGridMain;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      DateTime date = DateTime.Now;

      this.ClearEntrys();

      txTanggalHdr.Text = date.ToString("dd-MM-yyyy");
      txJumlahHdr.Text = "0";
      txBiayaAdminHdr.Text = "0";
      lbSisaHdr.Text = "0";

      winDetail.Title = "Debit";

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
  }
  
  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string numberId = (e.ExtraParams["NumberID"] ?? string.Empty);
    string jsonStoreValues = (e.ExtraParams["storeValues"] ?? string.Empty);
    string tanggalNote = (e.ExtraParams["TanggalNote"] ?? string.Empty);
    string jenisBayar = (e.ExtraParams["JenisBayar"] ?? string.Empty);
    string suplierId = (e.ExtraParams["SuplierID"] ?? string.Empty);
    string suplierName = (e.ExtraParams["SuplierName"] ?? string.Empty);
    string tipeBayar = (e.ExtraParams["TipeBayar"] ?? string.Empty);
    string bankID = (e.ExtraParams["BankID"] ?? string.Empty);
    string rekNo = (e.ExtraParams["RekNo"] ?? string.Empty);
    string giroID = (e.ExtraParams["GiroID"] ?? string.Empty);
    string giroDate = (e.ExtraParams["GiroDate"] ?? string.Empty);
    string kursID = (e.ExtraParams["KursID"] ?? string.Empty);
    string kursValue = (e.ExtraParams["KursValue"] ?? string.Empty);
    string jumlahTransaksi = (e.ExtraParams["JumlahTransaksi"] ?? string.Empty);
    string sisaTransaksi = (e.ExtraParams["SisaTransaksi"] ?? string.Empty);
    string biayaAdmin = (e.ExtraParams["BiayaAdmin"] ?? string.Empty);
    string isDP = (e.ExtraParams["IsDP"] ?? string.Empty);
    string ketHeader = (e.ExtraParams["Keterangan"] ?? string.Empty);

    Dictionary<string, string>[] gridDataPL = JSON.Deserialize<Dictionary<string, string>[]>(jsonStoreValues);
    
    string jumlahTransaksiOut = null;

    PostDataParser.StructureResponse respon = SaveParser(numberId, gridDataPL, tanggalNote, jenisBayar,
            suplierId, tipeBayar, bankID, rekNo, giroID, giroDate, kursID, kursValue, jumlahTransaksi, sisaTransaksi,
            biayaAdmin, isDP, ketHeader, out jumlahTransaksiOut);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        string scrpt = null;
        string storeId = hfStoreID.Text;

        string dateJs = null;
        DateTime date = DateTime.Today;
        
        if (respon.Values != null)
        {
          if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
          {
            dateJs = Functional.DateToJson(date);
          }

          if (!string.IsNullOrEmpty(storeId))
          {
            scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
                                          'c_noteno': '{1}',
                                          'd_notedate': {2},
                                          'c_refcode': '{3}',
                                          'v_refcode_desc': '{4}',
                                          'n_bilva': {5},
                                          'n_sisa': 0,
                                          'c_vcno': '{6}',
                                          'v_ket': '{7}'
                          }}));{0}.commitChanges();", storeId,
                                      respon.Values.GetValueParser<string>("Note", string.Empty),
                                      dateJs,
                                      suplierId,
                                      suplierName,
                                      jumlahTransaksiOut,
                                      respon.Values.GetValueParser<string>("Voucher", string.Empty),
                                      ketHeader);

            X.AddScript(scrpt);
          }
        }

        this.ClearEntrys();

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
  
  [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string gdgId = pag.ActiveGudang;
    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string pl1 = (e.ExtraParams["PLID1"] ?? string.Empty);
    string pl2 = (e.ExtraParams["PLID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(gdgId) && string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(pl1) && string.IsNullOrEmpty(pl2))
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();
    
    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10101";
    
    #region Report Parameter
    
    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(gdgId) ? string.Empty : gdgId)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_cusno",
      ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    });

    if (!string.IsNullOrEmpty(pl1))
    {
      if (string.IsNullOrEmpty(pl2))
      {
        lstRptParam.Add(new ReportParameter()
        {
          DataType = typeof(string).FullName,
          ParameterName = "LG_PLH.c_plno",
          ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)
        });
      }
      else
      {
        if (pl1.CompareTo(pl2) <= 0)
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = "LG_PLH.c_plno",
            ParameterValue = (string.IsNullOrEmpty(pl1) ? string.Empty : pl1)

          });
        }
        else
        {
          lstRptParam.Add(new ReportParameter()
          {
            DataType = typeof(string).FullName,
            ParameterName = string.Format("({{LG_PLH.c_plno}} IN ({0} TO {1}))", pl1, pl2),
            IsReportDirectValue = true
          });
        }
      }
    }

    #endregion

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    //string result = soa.GeneratorReport(isAsync, xmlFiles);
  }
}