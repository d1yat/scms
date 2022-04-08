<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BDP.ascx.cs" Inherits="reporting_konsolidasi_BDP" %>
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
            <ext:Label ID="Label1" runat="server" Text="-" />
            <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false">
              <CustomConfig>
                <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
          </Items>
        </ext:CompositeField>
        <%--<ext:MultiCombo ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" MinChars="3"
          FieldLabel="Cabang" WrapBySquareBrackets="true" Delimiter=";">
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
                <ext:Parameter Name="parameters" Value="[['l_cabang = @0', true, 'System.Boolean']]" Mode="Raw" />
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
        </ext:MultiCombo>--%>
        <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" MinChars="3"
          FieldLabel="Cabang" ForceSelection="false" Hidden="true">
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
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2011" />
                <ext:Parameter Name="parameters" Value="[['@contains.c_cusno.Contains(@0) || @contains.v_cunam.Contains(@0) || @contains.c_cab.Contains(@0)', paramTextGetter(#{cbCustomer}), ''],
                                                        ['l_cabang = @0', true, 'System.Boolean']]"
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
        <%--<ext:MultiCombo ID="cbDivAms" runat="server" DisplayField="v_nmdivams" ValueField="c_kddivams"
          Width="500" ItemSelector="tr.search-item" PageSize="40" ListWidth="500" MinChars="3"
          FieldLabel="Divisi AMS" WrapBySquareBrackets="true" Delimiter=";">
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
                <ext:Parameter Name="model" Value="2041" />
                <ext:Parameter Name="parameters" Value="[['l_aktif = @0', true, 'System.Boolean']]" Mode="Raw" />
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
          <Template runat="server">
            <Html>
            <table cellpading="0" cellspacing="1" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
            <tpl for="."><tr class="search-item">
            <td>{c_kddivams}</td><td>{v_nmdivams}</td>
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
        </ext:MultiCombo>--%>
        <ext:ComboBox ID="cbDivAms" runat="server" FieldLabel="Divisi Ams" ValueField="c_kddivams"
          DisplayField="v_nmdivams" Width="500" ListWidth="500" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" Hidden="true">
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
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2041" />
                <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                  ['@contains.c_kddivams.Contains(@0) || @contains.v_nmdivams.Contains(@0) || @contains.c_kddivams.Contains(@0)', paramTextGetter(#{cbDivAms}), '']]"
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
                    <ext:RecordField Name="v_divams_desc" />
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
        <%--<ext:MultiCombo ID="cbItems" runat="server" FieldLabel="Produk" ValueField="c_iteno"
          DisplayField="v_itnam" Width="500" ListWidth="500" WrapBySquareBrackets="true"
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" Delimiter=";">
          <Store>
            <ext:Store runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2141" />
                <ext:Parameter Name="parameters" Value="[['@in.c_kddivams', paramValueMultiGetter(#{cbDivAms}), 'System.String[]']]" Mode="Raw" />
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
          <Template runat="server">
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
        </ext:MultiCombo>--%>
        <ext:ComboBox ID="cbItems" runat="server" FieldLabel="Produk" ValueField="c_iteno"
              DisplayField="v_itnam" Width="500" ListWidth="500" 
              PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false" Hidden="true">
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
                                                            ['@contains.c_iteno.Contains(@0) || @contains.c_itenopri.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItems}), '']
                                                            ]" Mode="Raw" />
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
        <ext:RadioGroup ID="rbgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan" AllowBlank="false">
          <Items>
            <ext:Radio ID="rbgTRBDP" runat="server" BoxLabel="B D P" Checked="true" Tag="01" />
            <ext:Radio ID="rbgTRRBDP" runat="server" BoxLabel="Ril B D P" Tag="02" Hidden="true" />
            <ext:Radio ID="rbgTRPLL" runat="server" BoxLabel="Penerimaan Lain-lain" Tag="03" Hidden="true" />
            <ext:Radio ID="rbgTRSELI" runat="server" BoxLabel="R D P" Tag="04" />
            <ext:Radio ID="rbgTRRNC" runat="server" BoxLabel="RN Cabang" Tag="05" Hidden="true" />
            <ext:Radio ID="rbgTRRNCS" runat="server" BoxLabel="RN Cabang Selisih Quantity" Tag="06" Hidden="true" />
            <ext:Radio ID="rbgTRPEND" runat="server" BoxLabel="Pending" Tag="07" Hidden="true" />
          </Items>
        </ext:RadioGroup>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="1"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <%--<ext:CompositeField ID="CompositeField2" runat="server">
          <Items>
            <ext:TextField ID="txRptUName" runat="server" FieldLabel="Nama" MaxLength="100" Width="200" />
            <ext:Label ID="Label2" runat="server" Text="&nbsp;" />
            <ext:Checkbox ID="chkShare" runat="server" FieldLabel="Berbagi" />
          </Items>
        </ext:CompositeField>--%>
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
