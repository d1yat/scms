﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SuratPesanan.ascx.cs"
  Inherits="reporting_history_SuratPesanan" %>
<ext:Panel runat="server">
  <Content>
    <ext:Hidden ID="hidCab" runat="server" />
    <ext:Hidden ID="hidSupl" runat="server" />
    <ext:Hidden ID="hidDivSupl" runat="server" />
  </Content>
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" Layout="Form">
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
        <ext:CompositeField runat="server" FieldLabel="Batasan SP">
          <Items>
            <ext:TextField ID="txSP1" runat="server" />
            <ext:Label ID="Label1" runat="server" Text="-" />
            <ext:TextField ID="txSP2" runat="server" />
          </Items>
        </ext:CompositeField>
        <ext:ComboBox ID="cbCustomer" runat="server" DisplayField="v_cunam" ValueField="c_cusno"
          Width="500" ItemSelector="tr.search-item" PageSize="10" ListWidth="500" FieldLabel="Cabang"
          ForceSelection="false">
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
                <ext:Parameter Name="parameters" Value="[['c_cusno = @0', paramValueGetter(#{hidCab}), ''],
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
        <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
          DisplayField="v_nama" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
          AllowBlank="true" ForceSelection="false">
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
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['c_nosup = @0', paramValueGetter(#{hidSupl}), ''],
                      ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
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
          <Template runat="server">
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
          DisplayField="v_nmdivpri" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
          AllowBlank="true" ForceSelection="false">
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
                <ext:Parameter Name="model" Value="2051" />
                <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                  ['c_kddivpri = @0', paramValueGetter(#{hidDivSupl}), ''],
                      ['@contains.c_kddivpri.Contains(@0) || @contains.v_nmdivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]"
                  Mode="Raw" />
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
          <Template runat="server">
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
          DisplayField="v_itnam" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
          AllowBlank="true" ForceSelection="false">
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
                <ext:Parameter Name="model" Value="2141" />
                <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{cbSuplier}), 'System.String[]'],
                  ['@in.c_kddivpri', paramValueMultiGetter(#{cbDivPrinsipal}), 'System.String[]'],
                  ['c_kddivpri = @0', paramValueGetter(#{hidDivSupl}), ''],
                  ['c_nosup = @0', paramValueGetter(#{hidSupl}), ''],
                      ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItems}), '']]"
                  Mode="Raw" />
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
