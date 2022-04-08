<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Claim.ascx.cs" Inherits="reporting_Claim" %>

<script type="text/javascript">

    var validateRadio = function(rdgDetail, rdgTotal, hfStatus, cbTahun, CFClaimDetail, cbSuplier, cbDivPrinsipal, CFPeriode, cbType, cbTipeDetail) {
    if (rdgTotal) {
        cbTahun.setVisible(true);
        CFClaimDetail.setVisible(false);
        cbSuplier.setVisible(true);
        cbDivPrinsipal.setVisible(true);
        CFPeriode.setVisible(true);
        cbType.setVisible(true);
        cbTipeDetail.setVisible(true);
        hfStatus.setValue('01');
        } else {
        cbTahun.setVisible(false);
        CFClaimDetail.setVisible(true);
        cbSuplier.setVisible(true);
        cbDivPrinsipal.setVisible(false);
        CFPeriode.setVisible(false);
        cbType.setVisible(false);
        cbTipeDetail.setVisible(false);
        hfStatus.setValue('02');
        };
    };
</script>

<ext:Panel runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:Hidden ID="hfStatus" runat="server" />
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" Layout="Form">
      <Items>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" FieldLabel="Tipe" ColumnsNumber="1" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdgTotal" runat="server" BoxLabel="Total" Tag="01" Checked="true">
            </ext:Radio>
            <ext:Radio ID="rdgDetail" runat="server" BoxLabel="Detail" Tag="02" >
            </ext:Radio>
          </Items>
            <Listeners>
               <Change Handler="validateRadio(#{rdgDetail}.getValue(), #{rdgTotal}.getValue(), #{hfStatus}, #{cbTahun}, #{CFClaimDetail}, #{cbSuplier}, #{cbDivPrinsipal}, #{CFPeriode}, #{cbType}, #{cbTipeDetail});" />
            </Listeners>
        </ext:RadioGroup>
        <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
        <ext:CompositeField ID="CFClaimDetail" runat="server" FieldLabel="No. Claim" Hidden="true">
          <Items>
            <ext:TextField ID="txClaimDetail1" runat="server" />
            <ext:Label ID="Label3" runat="server" Text="-" />
            <ext:TextField ID="txClaimDetail2" runat="server" />
          </Items>
        </ext:CompositeField>
        <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
          DisplayField="v_nama" Width="500" ListWidth="500" PageSize="10" ItemSelector="tr.search-item"
          AllowBlank="true">
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
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                    ['l_aktif == @0', true, 'System.Boolean'],
                    ['@contains.v_nama.Contains(@0) || @contains.c_nosup.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
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
          <Template ID="Template12" runat="server">
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
          AllowBlank="true">
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
                <ext:Parameter Name="model" Value="2051" />
                <ext:Parameter Name="parameters" Value="[['c_nosup = @0', paramValueGetter(#{cbSuplier}), 'System.String'],
                ['@contains.v_nmdivpri.Contains(@0) || @contains.c_kddivpri.Contains(@0)', paramTextGetter(#{cbDivPrinsipal}), '']]" Mode="Raw" />
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
          <Template ID="Template13" runat="server">
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
        <ext:CompositeField ID="CFPeriode" runat="server" FieldLabel="Periode">
          <Items>
            <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false" hidden="true">
              <CustomConfig>
                <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
            <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false">
              <CustomConfig>
                <ext:ConfigItem Name="startDateField" Value="#{txPeriode2}" Mode="Value" />
              </CustomConfig>
            </ext:DateField>
          </Items>
        </ext:CompositeField>
        <ext:ComboBox ID="cbType" runat="server" FieldLabel="Type" DisplayField="v_ket"
          ValueField="c_type" Width="220" AllowBlank="true" ForceSelection="false">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store ID="Store6" runat="server" RemotePaging="false">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2001" />
                <ext:Parameter Name="parameters" Value="[['c_portal = @0', '3', 'System.Char'],
                                  ['c_notrans = @0', '39', '']]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="c_type" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_type" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_type" />
                    <ext:RecordField Name="v_ket" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
        </ext:ComboBox>
        <ext:SelectBox ID="cbTipeDetail" runat="server" FieldLabel="Base On" AllowBlank="false" SelectedIndex="0"
          Width="250">
          <Items>
            <ext:ListItem Text="RN Date" Value="01" />
            <ext:ListItem Text="DO Date" Value="02" />
          </Items>
        </ext:SelectBox>
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="1"
          AllowBlank="false">
          <Items>
            <ext:ListItem Value="01" Text="PDF" />
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
          </Items>
        </ext:SelectBox>
        <%--<ext:CompositeField runat="server">
          <Items>
            <ext:TextField ID="txRptUName" runat="server" FieldLabel="Nama" MaxLength="100" Width="200" />
            <ext:Label runat="server" Text="&nbsp;" />
            <ext:Checkbox ID="chkShare" runat="server" FieldLabel="Berbagi" />
          </Items>
        </ext:CompositeField>--%>
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="false" />
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
