<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BiayaEkspedisi.ascx.cs" Inherits="reporting_history_BiayaEkspedisi" %>

<script type="text/javascript" language="javascript">
  var changeTipeReport = function(itm, comField) {
  var idTarget = '<%= rdByResi.ClientID %>';
    var isCek = itm.getValue();

    if (itm.getId() == idTarget) {
      if (isCek) {
        comField.show();
      }
      else {
        comField.hide();
      }
    }
    else {
      comField.hide();
    }
  }
</script>

<ext:Panel runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" 
      Layout="Form">
      <Items>
        <ext:CompositeField runat="server" FieldLabel="Periode">
          <Items>
            <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false">
              <CustomConfig>
                <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
            <ext:Label runat="server" Text="-" />
            <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false">
              <CustomConfig>
                <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
          </Items>
        </ext:CompositeField>
        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" ValueField="c_gdg"
          DisplayField="v_gdgdesc" Width="300" AllowBlank="true">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store ID="Store2" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2031" />
                <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="c_gdg" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_gdg" Root="d.records" SuccessProperty="d.success" TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_gdg" />
                    <ext:RecordField Name="v_gdgdesc" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Triggers>
            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
          </Triggers>
          <Listeners>
            <Select Handler="this.triggers[0].show();" />
            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
            <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:ComboBox ID="cbExpedisi" runat="server" DisplayField="v_ket" ValueField="c_exp"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" MinChars="3"
          FieldLabel="Expedisi" AllowBlank="true">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store ID="Store1" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2081" />
                <ext:Parameter Name="parameters" Value="[['@contains.v_ket.Contains(@0) || @contains.c_exp.Contains(@0)', paramTextGetter(#{cbExpedisi}), '']]"
                  Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_ket" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_exp" />
                    <ext:RecordField Name="v_ket" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="1" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for="."><tr class="search-item">
            <td>{c_exp}</td><td>{v_ket}</td>
            <%--<td><input type="checkbox" disabled="disable" value="{l_udara}" /></td>
            <td><input type="checkbox" disabled="disable" value="{l_darat}" /></td>
            <td><input type="checkbox" disabled="disabled" value="{l_laut}" /></td>
            <td><input type="checkbox" disabled="disabled" value="{l_import}" /></td>--%>
            </tr></tpl>
            </table>
            </Html>
          </Template>
          <Triggers>
            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <Select Handler="this.triggers[0].show();" />
            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
            <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="No. IE">
          <Items>
            <ext:TextField ID="txIENo1" runat="server" />
            <ext:Label ID="Label3" runat="server" Text="-" />
            <ext:TextField ID="txIENo2" runat="server" />
          </Items>
        </ext:CompositeField>
        <ext:CompositeField ID="CompositeField3" runat="server" FieldLabel="No. Resi">
          <Items>
            <ext:TextField ID="txNoResi1" runat="server" />
            <ext:Label ID="Label1" runat="server" Text="-" />
            <ext:TextField ID="txNoResi2" runat="server" />
          </Items>
        </ext:CompositeField>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="No. EP">
          <Items>
            <ext:TextField ID="txEPNo1" runat="server" />
            <ext:Label ID="Label2" runat="server" Text="-" />
            <ext:TextField ID="txEPNo2" runat="server" />
          </Items>
        </ext:CompositeField>
        <%--<ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="No. Resi">
          <Items>
            <ext:TextField ID="txNoResi1" runat="server" />
            <ext:Label ID="Label3" runat="server" Text="-" />
            <ext:TextField ID="txNoResi2" runat="server" />
          </Items>
        </ext:CompositeField>--%>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan">
          <Items>
            <ext:Radio ID="rdByResi" runat="server" BoxLabel="By Resi" Checked="true" InputValue="01" />
            <ext:Radio ID="rdByDO" runat="server" BoxLabel="By DO" InputValue="02" />
            <ext:Radio ID="rdByItem" runat="server" BoxLabel="By Item" InputValue="03" />
          </Items>
            <Listeners>
              <Change Handler="changeTipeReport(checked, #{chkOutstanding});" />
            </Listeners>
        </ext:RadioGroup>
        <ext:Checkbox ID="chkOutstanding" runat="server" FieldLabel="Outstanding" />
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="2"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <ext:CompositeField runat="server">
          <Items>
            <ext:TextField ID="txRptUName" runat="server" FieldLabel="Nama" MaxLength="100" Width="200" />
            <ext:Label runat="server" Text="&nbsp;" />
            <ext:Checkbox ID="chkShare" runat="server" FieldLabel="Berbagi" />
          </Items>
        </ext:CompositeField>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="ReportGo" Text="Generate">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportKriteria});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReportKriteria}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportKriteria}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
  </Listeners>
</ext:Panel>
