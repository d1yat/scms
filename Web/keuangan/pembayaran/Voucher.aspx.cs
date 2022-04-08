using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;

public partial class keuangan_pembayaran_Voucher : Scms.Web.Core.PageHandler
{
  private const string VC_DEBIT = "DEBIT";
  private const string VC_KREDIT = "KREDIT";

  private const string TYPE_VC_DEBIT = "0080";
  private const string TYPE_VC_KREDIT = "0082";

  private const string CONT_CREATED = "x1x1x1x1";
  private const string CONT_VIEW = "x1x1x1";

  private Control MyLoadControl(bool isDebit)
  {
    Control c = null;

    if (isDebit)
    {
      c = phCtrl.FindControl(CONT_CREATED);
      if (c == null)
      {
        c = this.LoadControl("HutangVCCtrl.ascx");

        c.ID = CONT_CREATED;

        phCtrl.Controls.Add(c);
      }

      c = phCtrl.FindControl(CONT_VIEW);
      if (c == null)
      {
        c = this.LoadControl("HutangVCVwCtrl.ascx");

        c.ID = CONT_VIEW;

        phCtrl.Controls.Add(c);
      }
    }
    else
    {
      c = phCtrl.FindControl(CONT_CREATED);
      if (c == null)
      {
        c = this.LoadControl("PiutangVCCtrl.ascx");

        c.ID = CONT_CREATED;

        phCtrl.Controls.Add(c);
      }

      c = phCtrl.FindControl(CONT_VIEW);
      if (c == null)
      {
        c = this.LoadControl("PiutangVCVwCtrl.ascx");

        c.ID = CONT_VIEW;

        phCtrl.Controls.Add(c);
      }
    }

    return c;
  }

  private PostDataParser.StructureResponse DeleteParser(string noteNumber, string ket, string mode)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    pair.IsSet = true;
    pair.IsList = true;
    pair.Value = noteNumber;
    pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
    pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    string varData = null;

    dic.Add("ID", pair);
    pair.DicAttributeValues.Add("Entry", this.Nip);

    pair.DicAttributeValues.Add("Keterangan", ket.Trim());

    try
    {
      if (mode.Equals(VC_DEBIT, StringComparison.OrdinalIgnoreCase))
      {
        varData = parser.ParserData("VoucherDebit", "Delete", dic);
      }
      else
      {
        varData = parser.ParserData("VoucherCredit", "Delete", dic);
      }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("keuangan_pembayaran_Voucher DeleteParser : {0} ", ex.Message);
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

  [DirectMethod(ShowMask = true)]
  public void DeleteMethod(string noteNumber, string keterangan)
  {
    if (string.IsNullOrEmpty(noteNumber))
    {
      Functional.ShowMsgWarning("Nomor Note tidak terbaca.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Nomor SP tidak terbaca.";

      return;
    }
    else if (string.IsNullOrEmpty(keterangan))
    {
      Functional.ShowMsgWarning("Keterangan tidak boleh kosong.");
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = "Keterangan tidak boleh kosong.";

      return;
    }

    PostDataParser.StructureResponse respon = DeleteParser(noteNumber, keterangan, hfMode.Text);

    if (respon.Response == PostDataParser.ResponseStatus.Success)
    {
      X.AddScript(
        string.Format("var r = {0}.getById('{1}');if(!Ext.isEmpty(r)) {{ {0}.remove(r);{0}.commitChanges(); }}",
          storeGridMain.ClientID, noteNumber));

      Functional.ShowMsgInformation(string.Format("Nomor Note '{0}' telah terhapus.", noteNumber));
    }
    else
    {
      Functional.ShowMsgWarning(respon.Message);
      //Ext.Net.ResourceManager.AjaxSuccess = false;
      //Ext.Net.ResourceManager.AjaxErrorMessage = respon.Message;
    }
  }

  protected void Page_Init(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("kredit", StringComparison.OrdinalIgnoreCase))
      {
        hfMode.Text = VC_KREDIT;
        hfType.Text = TYPE_VC_KREDIT;
      }
      else
      {
        hfMode.Text = VC_DEBIT;
        hfType.Text = TYPE_VC_DEBIT;
      }
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!this.IsPostBack)
    //{
    //  HutangVCCtrl1.Initialize(storeGridMain.ClientID);
    //  HutangVCVwCtrl1.Initialize();
    //}

    if (hfMode.Text.Equals(VC_KREDIT, StringComparison.OrdinalIgnoreCase))
    {
      MyLoadControl(false);

      keuangan_pembayaran_PiutangVCCtrl c1 = phCtrl.FindControl(CONT_CREATED) as keuangan_pembayaran_PiutangVCCtrl;
      keuangan_pembayaran_PiutangVCVwCtrl c2 = phCtrl.FindControl(CONT_VIEW) as keuangan_pembayaran_PiutangVCVwCtrl;

      if (c1 != null)
      {
        c1.Initialize(storeGridMain.ClientID);
      }
      if (c2 != null)
      {
        c2.Initialize();
      }

      #region Modify Grid Settings

      // Edit Grid Column
      string s = string.Format(@"Ext.onReady(function() {{
  var col = {0}.getColumnModel().getColumnById('v_refcode_desc');
  //alert(col);
  if(!Ext.isEmpty(col)) {{ 
    col.header = 'Cabang';
  }}
}});",
      gridMain.ClientID);

      //gridMain.ColumnModel.Columns.
      X.AddScript(s);

      cbSuplierFltr.DisplayField = "v_cunam";
      cbSuplierFltr.ValueField = "c_cusno";
      
      Ext.Net.Store store = cbSuplierFltr.GetStore();

      store.Proxy.Clear();
      Ext.Net.ScriptTagProxy proxy = new Ext.Net.ScriptTagProxy(new Ext.Net.ScriptTagProxy.Config()
      {
        CallbackParam = "soaScmsCallback",
        Url = "http://localhost:1234/scms/WebJsonP/GlobalQueryJson",
        Timeout = 10000000
      });
      store.Proxy.Add(proxy);

      store.BaseParams.Clear();
      store.BaseParams.Add(new Ext.Net.Parameter("start", "={0}"));
      store.BaseParams.Add(new Ext.Net.Parameter("limit", "={10}"));
      store.BaseParams.Add(new Ext.Net.Parameter("model", "2011"));
      store.BaseParams.Add(new Ext.Net.Parameter("parameters", @"[['l_hide = @0', false, 'System.Boolean'],['l_stscus = @0', true, 'System.Boolean'],['l_cabang = @0', true, 'System.Boolean']]", ParameterMode.Raw));
      store.BaseParams.Add(new Ext.Net.Parameter("sort", "v_cunam"));
      store.BaseParams.Add(new Ext.Net.Parameter("dir", "ASC"));

      store.Reader.Clear();
      Ext.Net.JsonReader jRead = new JsonReader(new Ext.Net.JsonReader.Config()
      {
        IDProperty = "c_cusno",
        Root = "d.records",
        SuccessProperty = "d.success",
        TotalProperty = "d.totalRows"
      });
      jRead.Fields.Add("c_cusno");
      jRead.Fields.Add("c_cab");
      jRead.Fields.Add("v_cunam");
      store.Reader.Add(jRead);

      cbSuplierFltr.Template.Html = @"<table cellpading=""0"" cellspacing=""1"" style=""width: 400px"">
                              <tr><td class=""body-panel"">Kode</td><td class=""body-panel"">Short</td><td class=""body-panel"">Cabang</td></tr>
                              <tpl for="".""><tr class=""search-item"">
                              <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                              </tr></tpl>
                              </table>";

      #endregion
    }
    else
    {
      MyLoadControl(true);

      keuangan_pembayaran_HutangVCCtrl c1 = phCtrl.FindControl(CONT_CREATED) as keuangan_pembayaran_HutangVCCtrl;
      keuangan_pembayaran_HutangVCVwCtrl c2 = phCtrl.FindControl(CONT_VIEW) as keuangan_pembayaran_HutangVCVwCtrl;

      if (c1 != null)
      {
        c1.Initialize(storeGridMain.ClientID);
      }
      if (c2 != null)
      {
        c2.Initialize();
      }
    }
  }

  protected void btnAddNew_OnClick(object sender, DirectEventArgs e)
  {
    if (!this.IsAllowAdd)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk menambah data.");
      return;
    }

    //HutangVCCtrl1.CommandPopulate(true, null);

    if (hfMode.Text.Equals(VC_KREDIT, StringComparison.OrdinalIgnoreCase))
    {
      MyLoadControl(false);

      keuangan_pembayaran_PiutangVCCtrl c = phCtrl.FindControl(CONT_CREATED) as keuangan_pembayaran_PiutangVCCtrl;

      c.CommandPopulate(true, null);
    }
    else
    {
      MyLoadControl(true);

      keuangan_pembayaran_HutangVCCtrl c = phCtrl.FindControl(CONT_CREATED) as keuangan_pembayaran_HutangVCCtrl;

      c.CommandPopulate(true, null);
    }
  }

  protected void gridMainCommand(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string pName = (e.ExtraParams["Parameter"] ?? string.Empty);
    string pID = (e.ExtraParams["PrimaryID"] ?? string.Empty);

    if (cmd.Equals("Select", StringComparison.OrdinalIgnoreCase))
    {
      if (hfMode.Text.Equals(VC_KREDIT, StringComparison.OrdinalIgnoreCase))
      {
        MyLoadControl(false);

        keuangan_pembayaran_PiutangVCVwCtrl c = phCtrl.FindControl(CONT_VIEW) as keuangan_pembayaran_PiutangVCVwCtrl;

        c.CommandPopulate(pID);
      }
      else
      {
        MyLoadControl(true);

        keuangan_pembayaran_HutangVCVwCtrl c = phCtrl.FindControl(CONT_VIEW) as keuangan_pembayaran_HutangVCVwCtrl;

        c.CommandPopulate(pID);
      }
    }

    GC.Collect();
  }
}
