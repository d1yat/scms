<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SuratJalan.ascx.cs" Inherits="reporting_pending_SuratJalan" %>

<%--Indra 20170828--%>
<script type="text/javascript">

    var validateRadio = function(rdgKAll, rdgConf, Status, dest) {
        if (rdgConf) {
            dest.setDisabled(false);
            Status.setValue('01');
        } else {
            dest.setDisabled(true);
            Status.setValue('02');
        };
    };
</script>

<ext:Panel runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:Hidden ID="hfStatus" runat="server" /><%--Indra 20170828--%>
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" Layout="Form">
      <Items>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Tgl. Buat SJ">
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
        <ext:ComboBox ID="cbGudangFrom" runat="server" FieldLabel="Gudang Asal" ValueField="c_gdg"
          DisplayField="v_gdgdesc" Width="175">
          <Store>
            <ext:Store ID="Store4" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2031" />
                <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
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
        <ext:ComboBox ID="cbGudangTo" runat="server" FieldLabel="Gudang Tujuan" ValueField="c_gdg"
          DisplayField="v_gdgdesc" Width="175">
          <Store>
            <ext:Store ID="Store2" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="allQuery" Value="true" />
                <ext:Parameter Name="model" Value="2031" />
                <ext:Parameter Name="parameters" Value="[['c_gdg != @0', '3', 'System.Char']]" Mode="Raw" />
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
        <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
          DisplayField="v_nama" Width="500" ListWidth="500" PageSize="10"
          ItemSelector="tr.search-item" AllowBlank="true">
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
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                    ['l_aktif == @0', true, 'System.Boolean'],
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
          <Template ID="Template1" runat="server">
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
        <%--<ext:RadioGroup ID="rdgTipeReport" runat="server" FieldLabel="Tipe Report" ColumnsNumber="1" AllowBlank="false">--%>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" FieldLabel="Konfirmasi" ColumnsNumber="1" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdgAll" runat="server" BoxLabel="Semua" Tag="01" Checked="true">
            </ext:Radio>
            <ext:Radio ID="rdgConf" runat="server" BoxLabel="Confirm" Tag="02" >
            </ext:Radio>
            <ext:Radio ID="rdgUnConf" runat="server" BoxLabel="Unconfirm" Tag="03">
            </ext:Radio>
          </Items>
          <%--Indra 20170828--%>
          <Listeners>
            <Change Handler="validateRadio(#{rdgAll}.getValue(), #{rdgConf}.getValue(), #{hfStatus}, #{rdgStatusSJ});" />
          </Listeners>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgStatusSJ" runat="server" FieldLabel="Status" ColumnsNumber="1" AllowBlank="false">
          <Items>
            <ext:Radio ID="rdgSelectAll" runat="server" BoxLabel="Semua" Tag="01" Checked="true" />
            <ext:Radio ID="rdgNonEP" runat="server" BoxLabel="SJ Belum Jadi EP" Tag="02" />
            <ext:Radio ID="rdgEP" runat="server" BoxLabel="SJ Sudah Jadi EP" Tag="03" /> 
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
