<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ARFakturPending.ascx.cs"
  Inherits="reporting_financial_ARFakturPending" %>
<ext:Panel runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportARFakPend" runat="server" Padding="5" Frame="True" Layout="Form">
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
        <ext:RadioGroup ID="rdgTipe" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan">
          <Items>
            <ext:Radio ID="rdSummary" runat="server" BoxLabel="Ringkasan" Checked="true" InputValue="ringkasan" />
            <ext:Radio ID="rdDetil" runat="server" BoxLabel="Detil" InputValue="detilFak" />
          </Items>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgBase" runat="server" ColumnsNumber="1" FieldLabel="Base Tanngal">
          <Items>
            <ext:Radio ID="rdFaktur" runat="server" BoxLabel="Tanggal Faktur" Checked="true"
              InputValue="01" />
            <ext:Radio ID="rdJthTempo" runat="server" BoxLabel="Tanggal Jatuh Tempo" InputValue="02" />
          </Items>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgTipeFak" runat="server" ColumnsNumber="1" FieldLabel="Tipe Faktur">
          <Items>
            <ext:Radio ID="rdFak" runat="server" BoxLabel="Faktur" Checked="true" InputValue="01" />
            <ext:Radio ID="rdFakRet" runat="server" BoxLabel="Faktur Retur" InputValue="02" />
          </Items>
        </ext:RadioGroup>
        <ext:ComboBox ID="cbCustomer" runat="server" FieldLabel="Customer" DisplayField="v_cunam"
          ValueField="c_cusno" Width="300" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
          MinChars="3" ForceSelection="false" AllowBlank="true">
          <Store>
            <ext:Store runat="server">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="true" />
              </CustomConfig>
              <Proxy>
                <ext:ScriptTagProxy CallbackParam="soaScmsCallback" Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson"
                  Timeout="10000000" />
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
                    <ext:RecordField Name="c_cab" />
                    <ext:RecordField Name="v_cunam" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template runat="server">
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
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportARFakPend});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReportARFakPend}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportARFakPend}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
  </Listeners>
</ext:Panel>
