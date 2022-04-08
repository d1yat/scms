<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReturnDO.ascx.cs" 
Inherits="reporting_Transaksi_ReturnDO" %>

<ext:Viewport ID="vwReturnDO" runat="server" Layout="Fit">
<Items>
<ext:Panel ID="Panel1" runat="server" Layout="Fit">
  <Items>
  <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReturnDO" runat="server" Padding="5" Frame="True" Layout="Form">
      <Items>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tgl. Buat DO">
          <Items>
            <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="true">
              <CustomConfig>
                <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
            <ext:Label ID="Label1" runat="server" Text="-" />
            <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="true">
              <CustomConfig>
                <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
          </Items>
        </ext:CompositeField>
        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang Asal" ValueField="c_gdg"
                      DisplayField="v_gdgdesc" Width="175" AllowBlank="false">
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
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" FieldLabel="Cabang Tujuan"
          ForceSelection="false">
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
                <ext:Parameter Name="parameters" Value="[
                ['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0)', paramTextGetter(#{cbCustomer}), '']]" Mode="Raw" />
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
          <Template ID="Template1" runat="server">
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
        <ext:RadioGroup ID="rdgKategoriRpt" runat="server" ColumnsNumber="1" FieldLabel="Kategori Report">
          <Items>
            <ext:Radio ID="rdALL" runat="server" Checked="true" BoxLabel="Semua" />
            <ext:Radio ID="rdReturnDO" runat="server" BoxLabel="Do Kembali" />
            <ext:Radio ID="rdNotReturnDO" runat="server" BoxLabel="Do Belum Kembali" />
          </Items>
        </ext:RadioGroup>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0" AllowBlank="false">
          <Items>
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <ext:CompositeField ID="CompositeField2" runat="server">
          <Items>
            <ext:TextField ID="txRptUName" runat="server" FieldLabel="Nama" MaxLength="100" Width="200" />
            <ext:Label ID="Label3" runat="server" Text="&nbsp;" />
            <ext:Checkbox ID="chkShare" runat="server" FieldLabel="Berbagi" />
          </Items>
        </ext:CompositeField>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button ID="Button1" runat="server" Icon="Report" Text="Generate">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReturnDO});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button ID="Button2" runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReturnDO}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Panel>
</Items>
<Listeners>
    <AfterRender Handler="initializePanel(#{frmReturnDO}, #{chkAsync}, #{txPeriode1}, #{txPeriode2});" />
</Listeners>
</ext:Viewport>