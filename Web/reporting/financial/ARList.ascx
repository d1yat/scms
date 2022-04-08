<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ARList.ascx.cs" 
Inherits="reporting_financial_ARList" %>

<ext:Panel runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportARList" runat="server" Padding="5" Frame="True" 
      Layout="Form">
      <Items>
        <ext:CompositeField  runat="server" FieldLabel="Periode">
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
        <ext:RadioGroup ID="rdgTipe" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdSummary" runat="server" BoxLabel="Ringkasan" Checked="true" InputValue="ringkasan" />
            <ext:Radio ID="rdDetil" runat="server" BoxLabel="Detil PerFaktur" InputValue="detilFak" />
            <ext:Radio ID="Radio1" runat="server" BoxLabel="Detil PerTransaksi" InputValue="detilTrans" />
          </Items>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgTipeFak" runat="server" ColumnsNumber="1" FieldLabel="Tipe Faktur" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdFak" runat="server" BoxLabel="Faktur" InputValue="01" Checked="true" />
            <ext:Radio ID="rdFakRet" runat="server" BoxLabel="Faktur Retur" InputValue="02" />
          </Items>
        </ext:RadioGroup>
        <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Cabang" DisplayField="v_cunam"
          ValueField="c_cusno" Width="350" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
          MinChars="3" AllowBlank="true" ForceSelection="false">
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
                <ext:Parameter Name="model" Value="2011" />
                <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean'],
                               ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]"
                  Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_cunam" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_cusno" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_cusno" />
                    <ext:RecordField Name="v_cunam" />
                    <ext:RecordField Name="c_cab" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="1" style="width: 400px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
                <tpl for="."><tr class="search-item">
                    <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
                </tr></tpl>
                </table>
            </Html>
          </Template>
          <Triggers>
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0"
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
            <ext:Label ID="Label2" runat="server" Text="&nbsp;" />
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
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportARList});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReportARList}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportARList}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
  </Listeners>
</ext:Panel>
