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
using System.Data.Odbc;

public partial class transaksi_pembelian_ProsesPharmanetCtrl : System.Web.UI.UserControl
{
  #region Private

  private void ClearEntrys()
  {
    winDetail.Title = "Proses Pharmanet";

    //hfSpNo.Clear();
    hfPoOutlet.Clear();

               
    cbCustomerHdr.Clear();
    cbCustomerHdr.Disabled = false;

    Ext.Net.Store cbCustomerHdrstr = cbCustomerHdr.GetStore();
    if (cbCustomerHdrstr != null)
    {
        cbCustomerHdrstr.RemoveAll();
    }

    cbPrincipalHdr.Clear();
    cbPrincipalHdr.Disabled = false;

    Ext.Net.Store cbPrincipalHdrstr = cbPrincipalHdr.GetStore();
    if (cbPrincipalHdrstr != null)
    {
        cbPrincipalHdrstr.RemoveAll();
    }



    txTanggal.Clear();
    txTanggal.Disabled = false;

    txSpCabang.Clear();
    txSpCabang.Disabled = false;

    txNoPL.Clear();
    txNoPL.Disabled = false;

   
    //chkCheck.Clear();
    //chkCheck.Disabled = false;

    

    Ext.Net.Store store = gridDetail.GetStore();
    if (store != null)
    {
      store.RemoveAll();
    }
  }

  private void PopulateDetail(string pName, string pID)
  {
    ClearEntrys();

    Dictionary<string, object> dicSP = null;
    Dictionary<string, string> dicSPInfo = null;
    Dictionary<string, string> dicResultInfo = null;
    Newtonsoft.Json.Linq.JArray jarr = null;

    Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

    string[][] paramX = new string[][]{
        new string[] { string.Format("{0} = @0", pName), pID, "System.String"}
      };

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    string tmp = null;

    #region Parser Header

    string res = soa.GlobalQueryService(0, 1, false, string.Empty, string.Empty, "0024", paramX); //"2500" // "0024"
    bool isCheck = false;
    bool isComplete = false;

    try
    {
      dicSP = JSON.Deserialize<Dictionary<string, object>>(res);
      if (dicSP.ContainsKey("records") && (dicSP.ContainsKey("totalRows") && (((long)dicSP["totalRows"]) > 0)))
      {
        jarr = new Newtonsoft.Json.Linq.JArray(dicSP["records"]);

        dicSPInfo = JSON.Deserialize<Dictionary<string, string>>(jarr.First.First.ToString());

        winDetail.Title = string.Format("Proses Pharmanet - {0}", pID);

        cbCustomerHdr.ToBuilder().AddItem(
          (dicSPInfo.ContainsKey("v_cunam") ? dicSPInfo["v_cunam"] : string.Empty),
          (dicSPInfo.ContainsKey("c_cusno") ? dicSPInfo["c_cusno"] : string.Empty)
        );
        if (cbCustomerHdr.GetStore() != null)
        {
            cbCustomerHdr.GetStore().CommitChanges();
        }
        cbCustomerHdr.SetValueAndFireSelect((dicSPInfo.ContainsKey("c_cusno") ? dicSPInfo["c_cusno"] : string.Empty));

        Functional.SetComboData(cbCustomerHdr, "c_cusno", dicSPInfo.GetValueParser<string>("v_cunam", string.Empty), dicSPInfo.GetValueParser<string>("c_cusno", string.Empty));
        cbCustomerHdr.Disabled = true;

       

        Functional.SetComboData(cbPrincipalHdr, "c_nosup", dicSPInfo.GetValueParser<string>("v_nama", string.Empty), dicSPInfo.GetValueParser<string>("c_nosup", string.Empty));
        cbPrincipalHdr.Disabled = true;



        tmp = dicSPInfo.GetValueParser<string>("d_dodate", string.Empty);

        isCheck = dicSPInfo.GetValueParser<bool>("l_cek");
        isComplete = dicSPInfo.GetValueParser<bool>("spComplete");

        //chkCheck.Checked = isCheck;

        DateTime date = Functional.JsonDateToDate(tmp);

        txTanggal.Text = date.ToString("dd-MM-yyyy");
        txTanggal.Disabled = true;

        txSpCabang.Text = dicSPInfo.GetValueParser<string>("c_dono", string.Empty);
        txSpCabang.Disabled = true;

       
        txNoPL.Text = dicSPInfo.GetValueParser<string>("c_plphar", string.Empty);
        txNoPL.Disabled = true;

       // txKeterangan.Text = dicSPInfo.GetValueParser<string>("v_ket", string.Empty);
        
        jarr.Clear();

        //btnPrint.Hidden = false;
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Header - ", ex.Message));
    }
    finally
    {
      SetEditor(!isComplete);

      if (jarr != null)
      {
        jarr.Clear();
      }
      if (dicSPInfo != null)
      {
        dicSPInfo.Clear();
      }
      if (dicSP != null)
      {
        dicSP.Clear();
      }
    }

    #endregion

    #region Parser Detail

    try
    {
      Ext.Net.Store store = gridDetail.GetStore();
      if (store.Proxy.Count > 0)
      {
        Ext.Net.ScriptTagProxy stp = store.Proxy[0] as Ext.Net.ScriptTagProxy;
        if ((stp != null) && stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase))
        {
          string param = (store.BaseParams["parameters"] ?? string.Empty);
          if (string.IsNullOrEmpty(param))
          {
            store.BaseParams.Add(new Ext.Net.Parameter("parameters", string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID), ParameterMode.Raw));
          }
          else
          {
            store.BaseParams["parameters"] = string.Format("[['{0} = @0', '{1}', 'System.String']]", pName, pID);
          }
        }
      }

      hfPoOutlet.Text = pID;
      X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine(
        string.Concat("transaksi_pembelian_SuratPesananCtrl:PopulateDetail Detail - ", ex.Message));
    }

    #endregion

    winDetail.Hidden = false;
    winDetail.ShowModal();

    GC.Collect();
  }

  private PostDataParser.StructureResponse CancelParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
  { 
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);
    PostDataParser parser = new PostDataParser();

    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    Dictionary<string, string> dicAttr = null;
    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    string result = null, email = null, tmp = null, item = null, ket = null, ketedit = null, nbatch = null, nbatchditerima = null, keterangan = null, vitnam = null;
    decimal nQty = 0, nAcc = 0;
    bool isNew = false, isVoid = false, isModify = false;
    string varData = null;
    if (txKeterangan.Text == null || txKeterangan.Text == "")
    {
        responseResult = new PostDataParser.StructureResponse()
        {
            IsSet = true,
            Message = "Mohon di isi keterangannya.",
            Response = PostDataParser.ResponseStatus.Failed
        };
        goto endlogic;
    }
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Supplier", cbPrincipalHdr.Text);
    pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
    pair.DicAttributeValues.Add("NoPl", txNoPL.Text.Trim());
    pair.DicAttributeValues.Add("Tipe", "03");
    pair.DicAttributeValues.Add("Keterangan",txKeterangan.Text.Trim());
    try
    {
        varData = parser.ParserData("CancelPharmanet", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
        Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_Proses_Pharmanet VerifikasiParser : {0} ", ex.Message);
    }

    if (!string.IsNullOrEmpty(varData))
    {
        Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

        result = soa.PostData(varData);

        responseResult = parser.ResponseParser(result);
    }
      endlogic:
    return responseResult;
  }

  private PostDataParser.StructureResponse VerifikasiParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
  {
      PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

      PostDataParser parser = new PostDataParser();
      IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";

      Dictionary<string, string> dicData = null;

      PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

      Dictionary<string, string> dicAttr = null;

      pair.IsSet = true;
      pair.IsList = true;
      pair.Value = plNumber;
      pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
      pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      string result = null;
      string email = null;
      string tmp = null, item = null, ket = null, ketEdit = null, nbatch = null, nbatchditerima = null, keterangan = null, vitnam = null;
      decimal nQty = 0, nAcc = 0;
      bool isNew = false, isVoid = false, isModify = false;
      string varData = null;

      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      DateTime date = DateTime.Today;

      if (!Functional.DateParser(txTanggal.RawText, "dd-MM-yyyy", out date))
      {
          responseResult = new PostDataParser.StructureResponse()
          {
              IsSet = true,
              Message = "Format penulisan tanggal salah.",
              Response = PostDataParser.ResponseStatus.Failed
          };

          return responseResult;
      }

      dic.Add("ID", pair);
      pair.DicAttributeValues.Add("Entry", pag.Nip);
      pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
      pair.DicAttributeValues.Add("Supplier", cbPrincipalHdr.Text);
      pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
      pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
      pair.DicAttributeValues.Add("NoPl", txNoPL.Text.Trim());
      pair.DicAttributeValues.Add("Tipe", "03");
      //pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());

      for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
      {
          tmp = nLoop.ToString();

          dicData = dics[nLoop];
          item = dicData.GetValueParser<string>("c_iteno");
          nQty = dicData.GetValueParser<decimal>("n_qty");
          nAcc = dicData.GetValueParser<decimal>("n_qtyterima");
          nbatch = dicData.GetValueParser<string>("c_batch");
          nbatchditerima = dicData.GetValueParser<string>("nv_batchterima");
          keterangan = dicData.GetValueParser<string>("v_ket");
          vitnam = dicData.GetValueParser<string>("v_itnam");
          email = email + vitnam + " - " + "Item data = "+ nQty + ",item diterima = " + nAcc + ",Batch data = " + nbatch + ",batch diterima = " + nbatchditerima + " - " + keterangan + "\r\n";

          using (OdbcConnection con = new OdbcConnection(connectionStringSql))
          {
              con.Open();
              OdbcCommand cmd = new OdbcCommand();
              cmd.Connection = con;
              string sSql = "update TEMP_LG_DOPD set d_expiredterima = '" + date + "', n_qtyterima = '" + nAcc + "', nv_batchterima = '" + nbatchditerima + "', v_ket = '" + keterangan + "' where v_itnam = '" + dicData.GetValueParser<string>("v_itnam") + "' and c_batch = '" + nbatch + "' and c_po_outlet = '" + pair.Value + "'";
              cmd.CommandText = sSql;
              cmd.ExecuteNonQuery();
          }

          if (nAcc == null)
          {
              responseResult = new PostDataParser.StructureResponse()
              {
                  IsSet = true,
                  Message = "Qty diterima tidak boleh kosong.",
                  Response = PostDataParser.ResponseStatus.Failed
              };
              goto endlogic;
          }

          if (nbatchditerima == null)
          {
              responseResult = new PostDataParser.StructureResponse()
              {
                  IsSet = true,
                  Message = "Batch diterima tidak boleh kosong.",
                  Response = PostDataParser.ResponseStatus.Failed
              };
              goto endlogic;
          }
          dicData.Clear();
      }
      pair.DicAttributeValues.Add("Keterangan", email);

      try
      {
          varData = parser.ParserData("VerifikasiPharmanet", (isAdd ? "Add" : "Modify"), dic);
      }
      catch (Exception ex)
      {
          Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_Proses_Pharmanet VerifikasiParser : {0} ", ex.Message);
      }
      endlogic:
      if (!string.IsNullOrEmpty(varData))
      {
          Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

          result = soa.PostData(varData);

          responseResult = parser.ResponseParser(result);
      }

      return responseResult;
  }

  private PostDataParser.StructureResponse SaveParser(bool isAdd, string plNumber, Dictionary<string, string>[] dics)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    Dictionary<string, string> dicData = null;

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();
    var connectionStringSql = "Driver={SQL Server};Server=10.100.41.29;Database=AMS;Uid=sa;Pwd=4M5M1s2015";

    Dictionary<string, string> dicAttr = null;

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = plNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    string result = null;

    string tmp = null,
      item = null,
      ket = null,
      ketEdit = null,
      nbatch = null,
      nbatchditerima = null;
    decimal nQty = 0, nJml = 0,
      nAcc = 0;
    bool isNew = false,
      isVoid = false,
      isModify = false;
    string varData = null;
    string email = null, vitnam = null;
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    DateTime date = DateTime.Today;
    DateTime dateawal = DateTime.Today;
    if (!Functional.DateParser(txTanggal.RawText, "dd-MM-yyyy", out date))
    {
      responseResult = new PostDataParser.StructureResponse()
      {
        IsSet = true,
        Message = "Format penulisan tanggal salah.",
        Response = PostDataParser.ResponseStatus.Failed
      };

      return responseResult;
    }

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", pag.Nip);
    pair.DicAttributeValues.Add("Customer", cbCustomerHdr.Text);
    pair.DicAttributeValues.Add("Supplier", cbPrincipalHdr.Text);
    pair.DicAttributeValues.Add("Tanggal", date.ToString("yyyyMMddHHmmssfff"));
    pair.DicAttributeValues.Add("SPCabang", txSpCabang.Text.Trim());
    pair.DicAttributeValues.Add("NoPl", txNoPL.Text.Trim());
    //pair.DicAttributeValues.Add("Keterangan", txKeterangan.Text.Trim());
    pair.DicAttributeValues.Add("Tipe", "03");
    //pair.DicAttributeValues.Add("Cek", chkCheck.Checked.ToString().ToLower());
    try
    {
        using (OdbcConnection con = new OdbcConnection(connectionStringSql))
        {
            con.Open();
            for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
            {
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;
                tmp = nLoop.ToString();
                dicData = dics[nLoop];
                nQty = dicData.GetValueParser<decimal>("n_qty");
                nAcc = dicData.GetValueParser<decimal>("n_qtyterima");
                nbatch = dicData.GetValueParser<string>("c_batch");
                nbatchditerima = dicData.GetValueParser<string>("nv_batchterima");
                date = dicData.GetValueParser<DateTime>("d_expiredterima");
                nJml = nJml + nAcc;
                if (date < DateTime.Now)
                {
                    responseResult = new PostDataParser.StructureResponse()
                    {
                        IsSet = true,
                        Message = "Tanggal ED tidak boleh di bawa hari ini.",
                        Response = PostDataParser.ResponseStatus.Error
                    };
                    goto endlogic;
                }

                if (nbatchditerima == null || nbatchditerima == "" || nbatchditerima == "-")
                { 
                    responseResult = new PostDataParser.StructureResponse()
                    {
                        IsSet = true,
                        Message = "Batch tidak boleh kosong.",
                        Response = PostDataParser.ResponseStatus.Error
                    };
                    goto endlogic;
                }
                string sSql = "update TEMP_LG_DOPD set d_expiredterima = '" + date + "', n_qtyterima = '" + nAcc + "', nv_batchterima = '" + nbatchditerima + "' where v_itnam = '" + dicData.GetValueParser<string>("v_itnam") + "' and c_batch = '" + nbatch + "' and c_po_outlet = '" + pair.Value + "'";
                cmd.CommandText = sSql;
                cmd.ExecuteNonQuery();
            }
            con.Dispose();
        }
    }
    catch (Exception ex)
    {
        responseResult = new PostDataParser.StructureResponse()
        {
            IsSet = true,
            Message = "Ada Kesalahan pada batch mohon di cek kembali.",
            Response = PostDataParser.ResponseStatus.Failed
        };
        goto endlogic;
    }
    if (nJml == 0)
    {
        responseResult = new PostDataParser.StructureResponse()
        {
            IsSet = true,
            Message = "Jumlah QTY terima tidak boleh kosong",
            Response = PostDataParser.ResponseStatus.Failed
        };
        goto endlogic;
    }
    for (int nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicData = dics[nLoop];
      nQty = dicData.GetValueParser<decimal>("n_qty");
      nAcc = dicData.GetValueParser<decimal>("n_qtyterima");
      nbatch = dicData.GetValueParser<string>("c_batch");
      nbatchditerima = dicData.GetValueParser<string>("nv_batchterima");
      dateawal = dicData.GetValueParser<DateTime>("d_expired");
      date = dicData.GetValueParser<DateTime>("d_expiredterima");
      vitnam = dicData.GetValueParser<string>("v_itnam");
      email = email + vitnam + " - " + "Item data = " + nQty + ",item diterima = " + nAcc + ",Batch data = " + nbatch + ",batch diterima = " + nbatchditerima + ", Data Expired = " + dateawal + ", Fisik Expired" + date + "\r\n";
      //if (nQty != nAcc)
      //{
      //    responseResult = new PostDataParser.StructureResponse()
      //    {
      //        IsSet = true,
      //        Message = "Qty data PL dan Qty diterima tidak sama. Mohon cek kembali.",
      //        Response = PostDataParser.ResponseStatus.Failed
      //    };
      //    goto endlogic;
      //}

      if (nbatch == null)
      {
          responseResult = new PostDataParser.StructureResponse()
          {
              IsSet = true,
              Message = "Data batch tidak boleh kosong. Mohon cek kembali.",
              Response = PostDataParser.ResponseStatus.Failed
          };
          goto endlogic;
      } 


      dicData.Clear();
    }
      pair.DicAttributeValues.Add("Keterangan", email);

    try
    {
        varData = parser.ParserData("ProsesPharmanet", (isAdd ? "Add" : "Modify"), dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("transaksi_pembelian_Proses_Pharmanet SaveParser : {0} ", ex.Message);
    }

    endlogic:
    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }
  
  private void SetEditor(bool isEditing)
  {
    if (isEditing)
    {
      /*
      Properties of 'e' include:
      e.grid - This grid
      e.record - The record being edited
      e.field - The field name being edited
      e.value - The value being set
      e.originalValue - The original value for the field, before the edit.
      e.row - The grid row index
      e.column - The grid column index
      e.cancel - Cancel new data
      */
      X.AddScript(@"var verifyRowEditConfirm = function(e) {
                      var accQty = e.record.get('n_QtyApprove');
                      var sisa = e.record.get('n_sisa');
                      var totAccQty = e.record.get('n_totalAcc');
                      var lMod = e.record.get('l_modified');
                      var oAccQty = e.record.get('n_QtyOriginal');
                      var qtyReq = e.record.get('n_QtyRequest');
                      if((!lMod) && ((accQty == 0) || (sisa <= 0))) {
                        e.cancel = true;
                      }
                      else if (e.value > qtyReq) {
                        e.cancel = true;
                        ShowWarning('Maaf, jumlah acc tidak boleh lebih dari quantity.');
                      }
                      else if (accQty != sisa) {
                        e.cancel = true;
                        ShowWarning('Maaf, barang yang telah terproses tidak dapat diubah.');
                      }
                      else if((e.value == oAccQty) && (e.value != 0)) {
                        e.record.reject();
                      }
                      else {
                        e.record.set('n_QtyApprove', e.value);
                        e.record.set('n_sisa', e.value);
                        e.record.set('l_modified', true);
                        if(e.value == 0) {
                          e.record.set('n_totalAcc', 0);
                        }
                        else {
                          e.record.set('n_totalAcc', ((sisa > e.value) ? (totAccQty - (sisa - e.value)) : (totAccQty + (e.value - sisa))));
                        }
                        if(!lMod) {
                          e.record.set('n_QtyOriginal', accQty);
                        }
                      }
                    };");

      X.AddScript(
        string.Format(@"var tryFindColIndex = function () {{
                          var idx = {0}.findColumnIndex('n_QtyApprove');
                          if(idx != -1) {{
                            {0}.setEditable(idx, true);
                            {0}.setEditor(idx, new Ext.form.NumberField({{
                              allowBlank: false,
                              allowDecimals: true,
                              allowNegative: false,
                              minValue: 0.00
                            }}));
                          }}
                        }};
                        tryFindColIndex();", gridDetail.ColumnModel.ClientID));

      gridDetail.AddListener("ValidateEdit", "verifyRowEditConfirm");
    }
    else
    {
      X.AddScript(
        string.Format(@"var idx = {0}.findColumnIndex('n_QtyRequest');
              if(idx != -1) {{
                {0}.setEditable(idx, false);
              }}", gridDetail.ColumnModel.ClientID));
    }
  }

  #endregion

  public void Initialize(string storeIDGridMain, string typeName)
  {
    hfStoreID.Text = storeIDGridMain;
    hfTypeNameCtrl.Text = typeName;
  }

  public void CommandPopulate(bool isAdd, string pID)
  {
    if (isAdd)
    {
      SetEditor(false);

      ClearEntrys();

      txTanggal.Text = DateTime.Now.ToString("dd-MM-yyyy");

      winDetail.Hidden = false;
      winDetail.ShowModal();
    }
    else
    {
        PopulateDetail("c_po_outlet", pID);
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
      string numberId = (e.ExtraParams["POoutlet"] ?? string.Empty);
      string Qty = (e.ExtraParams["QtyPenerima"] ?? string.Empty);
      string ket = (e.ExtraParams["Keterangan"] ?? string.Empty);
      string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);

      bool isAdd = (string.IsNullOrEmpty(numberId) ? false : true);

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
         

      Dictionary<string, string>[] gridDataLim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

     
      PostDataParser.StructureResponse respon = SaveParser(isAdd, numberId, gridDataLim);
      if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              string storeId = hfStoreID.Text;

              PopulateDetail("c_po_outlet", numberId);

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

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;
    

    //string gdgId = (e.ExtraParams["CustomerID"] ?? string.Empty); ;
    //string idSup = (e.ExtraParams["NomorPL"] ?? string.Empty);
    string idPoOutlet = (e.ExtraParams["POoutlet"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    Dictionary<string, string>[] gridDataLim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

    bool isAdd = (string.IsNullOrEmpty(idPoOutlet) ? false : true);
    PostDataParser.StructureResponse respon = VerifikasiParser(isAdd, idPoOutlet, gridDataLim);

    int nloop = 0;
    if (respon.IsSet)
    {
        if (respon.Response == PostDataParser.ResponseStatus.Success)
        {
            string storeId = hfStoreID.Text;

            PopulateDetail("c_po_outlet", idPoOutlet);

            Functional.ShowMsgInformation("Data berhasil Terkirim.");
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

      #region Old
      ////if (!pag.IsAllowPrinting)
    ////{
    ////  Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
    ////  return;
    ////}


    //if (!Functional.CanCreateGenerateReport(pag))
    //{
    //    return;
    //}

    //string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    //string numberID = (e.ExtraParams["NumberID"] ?? string.Empty);

    //if (string.IsNullOrEmpty(custId) && string.IsNullOrEmpty(numberID))
    //{
    //    Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
    //    return;
    //}

    //ReportParser rptParse = new ReportParser();

    //List<ReportParameter> lstRptParam = new List<ReportParameter>();

    //List<string> lstData = new List<string>();

    //rptParse.ReportingID = "10113";

    //#region Report Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SPH.c_cusno",
    //    ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(string).FullName,
    //    ParameterName = "LG_SPH.c_spno",
    //    ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID)
    //});

    //#endregion

    //#region Linq Filter Parameter

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(char).FullName,
    //    ParameterName = "c_cusno = @0",
    //    ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
    //    IsLinqFilterParameter = true
    //});

    //lstRptParam.Add(new ReportParameter()
    //{
    //    DataType = typeof(string).FullName,
    //    ParameterName = "c_spno = @0",
    //    ParameterValue = (string.IsNullOrEmpty(numberID) ? string.Empty : numberID),
    //    IsLinqFilterParameter = true
    //});

    //#endregion

    //rptParse.PaperID = "Letter";
    //rptParse.ReportParameter = lstRptParam.ToArray();
    //rptParse.User = pag.Nip;
    //rptParse.UserDefinedName = numberID;

    //string xmlFiles = ReportParser.Deserialize(rptParse);

    //SoaReportCaller soa = new SoaReportCaller();

    //string result = soa.GeneratorReport(xmlFiles);

    //ReportingResult rptResult = ReportingResult.Serialize(result);

    //if (rptResult == null)
    //{
    //    Functional.ShowMsgError("Pembuatan report gagal.");
    //}
    //else
    //{
    //    if (rptResult.IsSuccess)
    //    {
    //        string tmpUri = Functional.UriDownloadGenerator(pag,
    //          rptResult.OutputFile, "Surat Pesanan", rptResult.Extension);

    //        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
    //    }
    //    else
    //    {
    //        Functional.ShowMsgWarning(rptResult.MessageResponse);
    //    }
    //}

      //GC.Collect();
      #endregion
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void ReloadBtn_Click(object sender, DirectEventArgs e)
  {
    if (!((Scms.Web.Core.PageHandler)(this.Page)).IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;


    //string gdgId = (e.ExtraParams["CustomerID"] ?? string.Empty); ;
    //string idSup = (e.ExtraParams["NomorPL"] ?? string.Empty);
    string idPoOutlet = (e.ExtraParams["POoutlet"] ?? string.Empty);
    string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
    Dictionary<string, string>[] gridDataLim = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);
    bool isAdd = (string.IsNullOrEmpty(idPoOutlet) ? false : true);

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
    PostDataParser.StructureResponse respon = CancelParser(isAdd, idPoOutlet, gridDataLim);

  }
}