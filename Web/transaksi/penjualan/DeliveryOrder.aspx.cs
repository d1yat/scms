using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;

public partial class transaksi_penjualan_DeliveryOrder : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      string qryString = (this.Request.QueryString["mode"] ?? string.Empty);

      if (qryString.Equals("stt", StringComparison.OrdinalIgnoreCase))
      {
        DOSTT.Visible = true;
        DOPL.Visible = false;
      }
      else
      {
        DOSTT.Visible = false;
        DOPL.Visible = true;
      }
    }
  }

  #region old

  //private void LoadDataGridHeader(string Model)
  //{
  //  switch (Model)
  //  {
  //    case "PL":
  //      {

  //        #region Column
  //        ColumnCollection cc = new ColumnCollection();
  //        Column cnDO = new Column();
  //        cnDO.ColumnID = "c_dono";
  //        cnDO.Header = "No DO";
  //        cnDO.DataIndex = "c_dono";
  //        cc.Add(cnDO);
  //        Column cnDate = new Column();
  //        cnDate.ColumnID = "d_dodate";
  //        cnDate.Header = "Tanngal DO";
  //        cnDate.DataIndex = "d_dodate";
  //        cc.Add(cnDate);
  //        Column cnGd = new Column();
  //        cnGd.ColumnID = "v_gdgdesc";
  //        cnGd.Header = "Gudang";
  //        cnGd.DataIndex = "v_gdgdesc";
  //        cc.Add(cnGd);
  //        Column cnCus = new Column();
  //        cnCus.ColumnID = "v_cunam";
  //        cnCus.Header = "Customer";
  //        cnCus.DataIndex = "v_cunam";
  //        cc.Add(cnCus);
  //        Column cnVia = new Column();
  //        cnVia.ColumnID = "v_ket";
  //        cnVia.Header = "VIA";
  //        cnVia.DataIndex = "v_ket";
  //        cc.Add(cnVia);
  //        Column cnPL = new Column();
  //        cnPL.ColumnID = "c_plno";
  //        cnPL.Header = "No PL";
  //        cnPL.DataIndex = "c_plno";
  //        cc.Add(cnPL);
  //        Column cnPin = new Column();
  //        cnPin.ColumnID = "c_pin";
  //        cnPin.Header = "PIN";
  //        cnPin.DataIndex = "c_pin";
  //        cc.Add(cnPin);
  //        Column cnExp = new Column();
  //        cnExp.ColumnID = "c_expno";
  //        cnExp.Header = "Exp";
  //        cnExp.DataIndex = "c_expno";
  //        cc.Add(cnExp);
  //        CheckColumn chCon = new CheckColumn();
  //        chCon.ColumnID = "l_confirm";
  //        chCon.Header = "Submit";
  //        chCon.DataIndex = "l_confirm";
  //        cc.Add(chCon);
  //        #endregion

  //        #region addcolumn

  //        JsonReader arrRead1 = new JsonReader();

  //        arrRead1.Fields.Add("", RecordFieldType.String);
  //        arrRead1.Fields.Add("c_dono", RecordFieldType.String);
  //        arrRead1.Fields.Add("d_dodate", RecordFieldType.Date);
  //        arrRead1.Fields.Add("v_gdgdesc", RecordFieldType.String);
  //        arrRead1.Fields.Add("v_cunam", RecordFieldType.String);
  //        arrRead1.Fields.Add("v_ket", RecordFieldType.String);
  //        arrRead1.Fields.Add("c_plno", RecordFieldType.String);
  //        arrRead1.Fields.Add("c_pin", RecordFieldType.String);
  //        arrRead1.Fields.Add("c_expno", RecordFieldType.String);
  //        arrRead1.Fields.Add("l_confirm", RecordFieldType.Boolean);
  //        arrRead1.TotalProperty = "d.totalRows";
  //        arrRead1.Root = "d.records";
  //        arrRead1.SuccessProperty = "d.success";
  //        arrRead1.IDProperty = "c_dono";

  //        gridMain.ColumnModel.Columns.AddRange(cc);
  //        gridMain.Border = false;
  //        gridMain.StripeRows = true;


  //        #endregion

  //        #region AddFilter

  //        #region btnClear

  //        Ext.Net.Button btnClear = new Ext.Net.Button();
  //        btnClear.ID = "ClearFilterButton";
  //        btnClear.Icon = Icon.Cancel;
  //        btnClear.ToolTip = "Clear filter";
  //        btnClear.Listeners.Click.Handler = "clearFilterGridHeader(#{GridMain}, #{txEXPFltr});";

  //        #endregion

  //        #region txDOFltr
  //        Ext.Net.TextField txtDOFil = new Ext.Net.TextField();
  //        txtDOFil.ID = "txDOFltr";
  //        txtDOFil.AllowBlank = true;
  //        txtDOFil.EnableKeyEvents = true;

  //        Ext.Net.DateField txDateDO = new Ext.Net.DateField();
  //        txDateDO.ID = "txDateDO";
  //        txDateDO.Format="dd-MM-Y";
  //        txDateDO.EnableKeyEvents = true;

  //        #endregion

  //        #region combo Cusmas

  //        Ext.Net.ComboBox cbCusmas = new Ext.Net.ComboBox();
  //        cbCusmas.ID = "cbCusmas";
  //        //cbCusmas.Template.Html = "<table><tr><td>Test</td></tr></table>";
  //        //Table tab = new Table();
  //        //TableRow tr = new TableRow();
  //        //TableCell td1 = new TableCell();
  //        //Ext.Net.Label lab = new Ext.Net.Label();
  //        //tab.Rows.Add(tr);
  //        //Ext.Net.TemplateColumn tem = new Ext.Net.TemplateColumn();
  //        //cbCusmas.Template.Controls.Add(tab);

  //        #endregion

  //        #region via
  //        string viaName = "c_type";
  //        //Ext.Net.Store store = gridMain.GetStore();
  //        Ext.Net.ComboBox cbVia = new ComboBox();
  //        Ext.Net.Store strVia = cbVia.GetStore();
  //        strVia.BaseParams["model"] = "2001";
  //        strVia.BaseParams["parameters"] = string.Format("c_portal = @0, 3, System.Char");
  //        strVia.BaseParams["parameters"] = string.Format("l_hide = @0, false, System.Boolean");

  //        Ext.Net.ScriptTagProxy prox = new Ext.Net.ScriptTagProxy();
  //        //prox.Url = "http://localhost:1234/scms/WebJsonP/GlobalQueryJson";
  //        //prox.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase);
  //        strVia.Proxy.Add(prox);

  //        Ext.Net.JsonReader jReadVia = new Ext.Net.JsonReader();
  //        Ext.Net.RecordField rec1 = new Ext.Net.RecordField();
  //        Ext.Net.RecordField rec2 = new Ext.Net.RecordField();
  //        jReadVia.Fields.Add(rec1);
  //        jReadVia.Fields.Add(rec2);
  //        rec1.Name = "c_type";
  //        rec2.Name = "v_ket";
  //        strVia.Reader.Add(jReadVia);
  //        #endregion


  //        Ext.Net.HeaderColumn EHeadColBtnClear = new Ext.Net.HeaderColumn();
  //        EHeadColBtnClear.Component.Add(btnClear);

  //        Ext.Net.HeaderColumn EHeadColDO = new Ext.Net.HeaderColumn();
  //        EHeadColDO.Component.Add(txtDOFil);

  //        Ext.Net.HeaderColumn EHeadColDateDO = new Ext.Net.HeaderColumn();
  //        EHeadColDateDO.Component.Add(txDateDO);

  //        Ext.Net.HeaderColumn EHeadColCus= new Ext.Net.HeaderColumn();
  //        EHeadColCus.Component.Add(cbCusmas);

  //        Ext.Net.HeaderColumn EHeadColGud = new Ext.Net.HeaderColumn();

  //        Ext.Net.HeaderColumn EHeadColVia = new Ext.Net.HeaderColumn();
  //        EHeadColVia.Component.Add(cbVia);

  //        Ext.Net.HeaderRow rowClear = new HeaderRow();
  //        rowClear.Columns.Add(EHeadColBtnClear);
  //        rowClear.Columns.Add(EHeadColDO);
  //        rowClear.Columns.Add(EHeadColDateDO);
  //        rowClear.Columns.Add(EHeadColGud);
  //        rowClear.Columns.Add(EHeadColCus);
  //        rowClear.Columns.Add(EHeadColVia);
  //        GridView1.HeaderRows.Add(rowClear);




  //        #region DoText

  //        //Ext.Net.TextField txtDOFil = new Ext.Net.TextField();
  //        //txtDOFil.ID = "txDOFltr";
  //        //txtDOFil.AllowBlank = true;
  //        //txtDOFil.EnableKeyEvents = true;

  //        //Ext.Net.HeaderColumn EHeadColDO = new Ext.Net.HeaderColumn();
  //        //EHeadColDO.Component.Add(txtDOFil);

  //        //Ext.Net.HeaderRow row = new HeaderRow();
  //        //row.Columns.Add(EHeadColDO);

  //        //GridView1.HeaderRows.Add(row);

  //        #endregion

  //        #endregion

  //        #region old
  //        string pName = "c_dono";
  //        string pID = "";

  //        //Ext.Net.Store store = gridMain.GetStore();
  //        //Ext.Net.ScriptTagProxy stp = new Ext.Net.ScriptTagProxy();
  //        //stp.Url = "http://localhost:1234/scms/WebJsonP/GlobalQueryJson";
  //        //stp.CallbackParam.Equals(Functional.NAME_SOA_SCMS_CALLBACK, StringComparison.OrdinalIgnoreCase);
  //        //storeGridPL.Proxy.Add(stp);
  //        storeGridPL.BaseParams["model"] = "0007";
  //        storeGridPL.BaseParams["parameters"] = string.Format("{0},%,", pName);
  //        storeGridPL.Sort("d_dodate", Ext.Net.SortDirection.ASC);
  //        storeGridPL.Reader.Add(arrRead1);
  //        #endregion

  //      };
  //      break;
  //  };
  //}

  #endregion
}