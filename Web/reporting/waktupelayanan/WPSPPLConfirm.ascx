<%@ Control Language="C#" AutoEventWireup="true" CodeFile="WPSPPLConfirm.ascx.cs" Inherits="Report_Analisa_WPCabang" %>
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
         <ext:SelectBox ID="cbGudang" FieldLabel="Gudang" runat="server" AllowBlank="false" SelectedIndex="0"
          Width="250">
          <Items>
            <ext:ListItem Text="Gudang 1" Value="1" />
            <ext:ListItem Text="Gudang 2" Value="2" />
          </Items>
        </ext:SelectBox>
        <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" MinChars="3"
          FieldLabel="Cabang">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2011" />
                <ext:Parameter Name="parameters" Value="[['l_cabang = @0', true, 'System.Boolean'],
                                                        ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]" Mode="Raw" />
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
            <table cellpading="0" cellspacing="1" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Short</td><td class="body-panel">Cabang</td></tr>
            <tpl for="."><tr class="search-item">
            <td>{c_cusno}</td><td>{c_cab}</td><td>{v_cunam}</td>
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
        <%--<ext:SelectBox ID="cbTipeTransaksi" runat="server" AllowBlank="false" SelectedIndex="0"
          Width="250">
          <Items>
            <ext:ListItem Text="Surat Pesanan vs Packing List" Value="01" />
            <ext:ListItem Text="Surat Pesanan vs Packing List Confirm" Value="02" />
            <ext:ListItem Text="Surat Pesanan vs Delivery Order" Value="03" />
            <ext:ListItem Text="Surat Pesanan vs Expeditur" Value="04" />
            <ext:ListItem Text="Packing List vs Packing List Confirm" Value="05" />
            <ext:ListItem Text="Packing List vs Delivery Order" Value="06" />
            <ext:ListItem Text="Packing List vs Expeditur" Value="07" />
            <ext:ListItem Text="Packing List Confirm vs Delivery Order" Value="08" />
            <ext:ListItem Text="Packing List Confirm vs Expeditur" Value="09" />
            <ext:ListItem Text="Delivery Order vs Expeditur" Value="10" />
            <ext:ListItem Text="Surat Pesanan Gudang vs Surat Jalan" Value="11" />
            <ext:ListItem Text="Surat Pesanan Gudang vs Expeditur" Value="12" />
            <ext:ListItem Text="Surat Jalan vs Expeditur" Value="13" />
            <ext:ListItem Text="Purchase Order vs Receive Note" Value="14" />
          </Items>
        </ext:SelectBox>--%>
        <ext:RadioGroup ID="rbgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan" AllowBlank="false">
          <Items>
            <ext:Radio ID="rbgTRDtl" runat="server" BoxLabel="Detail" Checked="true" Tag="01" />
            <ext:Radio ID="rbgTRSmry" runat="server" BoxLabel="Summary" Tag="02" />
          </Items>
        </ext:RadioGroup>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="2"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <ext:CompositeField ID="CompositeField2" runat="server">
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
    <ext:Button ID="Button1" runat="server" Icon="ReportGo" Text="Generate">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportKriteria});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReportKriteria}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportKriteria}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
  </Listeners>
</ext:Panel>
