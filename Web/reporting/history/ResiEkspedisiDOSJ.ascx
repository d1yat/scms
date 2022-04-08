<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ResiEkspedisiDOSJ.ascx.cs" Inherits="reporting_history_ResiEkspedisiDOSJ" %>

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
        <ext:RadioGroup ID="rdgTipe" runat="server" ColumnsNumber="1" FieldLabel="Tipe Report">
          <Items>
            <ext:Radio ID="rdDO" runat="server" BoxLabel="DO" Checked="true" />
            <ext:Radio ID="rdSJ" runat="server" BoxLabel="SJ" />
          </Items>
        </ext:RadioGroup>
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
                <ext:Parameter Name="parameters" Value="[['c_exp != @0', '00', 'System.String'],
                  ['@contains.v_ket.Contains(@0) || @contains.c_exp.Contains(@0)', paramTextGetter(#{cbExpedisi}), '']]"
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
                    <ext:RecordField Name="l_darat" Type="Boolean" />
                    <ext:RecordField Name="l_import" Type="Boolean" />
                    <ext:RecordField Name="l_laut" Type="Boolean" />
                    <ext:RecordField Name="l_udara" Type="Boolean" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="1" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td><td class="body-panel">Udara</td><td class="body-panel">Darat</td><td class="body-panel">Laut</td><td class="body-panel">Import</td></tr>
            <tpl for="."><tr class="search-item">
            <td>{c_exp}</td><td>{v_ket}</td>
            <td>{l_udara}</td>
            <td>{l_darat}</td>
            <td>{l_laut}</td>
            <td>{l_import}</td>
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
        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="No. Resi">
          <Items>
            <ext:TextField ID="txNoResi1" runat="server" />
            <ext:Label ID="Label2" runat="server" Text="-" />
            <ext:TextField ID="txNoResi2" runat="server" />
          </Items>
        </ext:CompositeField>
        <ext:CompositeField ID="cfDono" runat="server" FieldLabel="No. DO/SJ">
          <Items>
            <ext:TextField ID="txNoDO1" runat="server" />
            <ext:Label ID="Label1" runat="server" Text="-" />
            <ext:TextField ID="txNoDO2" runat="server" />
          </Items>
        </ext:CompositeField>
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
