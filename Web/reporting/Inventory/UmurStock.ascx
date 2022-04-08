<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UmurStock.ascx.cs" Inherits="reporting_Inventory_UmurStock" %>
<ext:Panel ID="Panel1" runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    
    <ext:FormPanel ID="frmStokGudang" runat="server" Padding="5" Frame="True"
      Layout="Form">
      <Items>
        <ext:DateField ID="txPeriode1" FieldLabel="Periode" runat="server"
          Format="dd-MM-yyyy" AllowBlank="false" />
        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" ValueField="c_gdg"
          DisplayField="v_gdgdesc" Width="175" AllowBlank="false">
          <Store>
            <ext:Store ID="Store1" runat="server">
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
        <ext:ComboBox ID="cbDivAms" runat="server" FieldLabel="Divisi Ams" ValueField="c_kddivams"
          DisplayField="v_nmdivams" Width="500" ListWidth="500" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" >
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
                <ext:Parameter Name="model" Value="2041" />
                <ext:Parameter Name="parameters" Value="[['@contains.c_kddivams.Contains(@0) || @contains.v_nmdivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
                  Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_nmdivams" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_kddivams" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_kddivams" />
                    <ext:RecordField Name="v_nmdivams" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_kddivams}</td><td>{v_nmdivams}</td>
              </tr>
            </tpl>
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
        <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
          DisplayField="v_nama" Width="500" ListWidth="500"  PageSize="10"
          ItemSelector="tr.search-item" AllowBlank="true" >
          <Store>
            <ext:Store ID="Store3" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_nama" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_nosup" />
                    <ext:RecordField Name="v_nama" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template2" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_nosup}</td><td>{v_nama}</td>
              </tr>
            </tpl>
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
        <ext:ComboBox ID="cbDivPrinsipal" runat="server" FieldLabel="Divisi Pemasok" ValueField="c_kddivpri"
          DisplayField="v_nmdivpri" Width="500" ListWidth="500" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" >
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
          <Store>
            <ext:Store ID="Store4" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2051" />
                <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                  ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_nmdivpri" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_kddivpri" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_kddivpri" />
                    <ext:RecordField Name="v_nmdivpri" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template3" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_kddivpri}</td><td>{v_nmdivpri}</td>
              </tr>
            </tpl>
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
        <ext:ComboBox ID="cbItems" runat="server" FieldLabel="Produk" ValueField="c_iteno"
          DisplayField="v_itnam" Width="500" ListWidth="500" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" >
                            <CustomConfig>
                              <ext:ConfigItem Name="allowBlank" Value="true" />
                            </CustomConfig>
          <Store>
            <ext:Store ID="Store5" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2141" />
                <ext:Parameter Name="parameters" Value="[['@in.c_kddivams', paramValueMultiGetter(#{cbDivAms}), 'System.String[]'],
                  ['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                  ['@in.c_kddivpri', paramValueMultiGetter(#{cbDivPrinsipal}), 'System.String[]'],
                  ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItems}), '']]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_itnam" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_iteno" />
                    <ext:RecordField Name="c_itenopri" />
                    <ext:RecordField Name="v_itnam" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template4" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Kode Pemasok</td><td class="body-panel">Nama</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_iteno}</td><td>{c_itenopri}</td><td>{v_itnam}</td>
              </tr>
            </tpl>
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
        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Umur (Hari)">
          <Items>
            <ext:NumberField ID="txUmur1" runat="server" AllowDecimals="false" AllowNegative="false" Width="50" MinValue="0" MaxValue="730" />
            <ext:Label runat="server" Text="-" />
            <ext:NumberField ID="txUmur2" runat="server" AllowDecimals="false" AllowNegative="false" Width="50" MinValue="0" MaxValue="730" />
          </Items>
        </ext:CompositeField>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Report" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdgDetil" runat="server" BoxLabel="Detil" InputValue="01" />
            <ext:Radio ID="rdgPerItm" runat="server" BoxLabel="Per Item" InputValue="02" />
            <ext:Radio ID="rdgDivPrin" runat="server" BoxLabel="Divisi Pemasok" InputValue="03" />
            <ext:Radio ID="rdgPrin" runat="server" BoxLabel="Pemasok" InputValue="04" Checked="true" />
          </Items>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgTipeProduk" runat="server" ColumnsNumber="1" FieldLabel="Tipe Produk">
          <Items>
            <ext:Radio ID="rdgTPSemua" runat="server" BoxLabel="Semua" InputValue="01" Checked="true" />
            <ext:Radio ID="rdgTPGood" runat="server" BoxLabel="Baik" InputValue="02" />
            <ext:Radio ID="rdgTPBad" runat="server" BoxLabel="Buruk" InputValue="03" />
          </Items>
        </ext:RadioGroup>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="0"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <ext:CompositeField ID="CompositeField1" runat="server">
          <Items>
            <ext:TextField ID="txRptUName" runat="server" FieldLabel="Nama" MaxLength="100" Width="200" />
            <ext:Label ID="Label1" runat="server" Text="&nbsp;" />
            <ext:Checkbox ID="chkShare" runat="server" FieldLabel="Berbagi" />
          </Items>
        </ext:CompositeField>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="true" />
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button runat="server" Icon="Report" Text="Generate">
      <DirectEvents>
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmStokGudang});">
          <EventMask ShowMask="true" />
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmStokGudang}, #{chkAsync}, #{txPeriode1});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmStokGudang}, #{chkAsync}, #{txPeriode1});" />
  </Listeners>
</ext:Panel>