<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LaporanPBF.ascx.cs" Inherits="reporting_Transaksi_LaporanPBF" %>



<ext:Panel runat="server">
  <Items>
     <%--hafizh--%>
     
    <ext:Hidden ID="hfTipe" runat="server" />
    
     <%-- end hafzih--%>
    
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportSJ" runat="server" Padding="5" Frame="True" Layout="Form">
      <Items>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode">
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
        <ext:ComboBox ID="cbGudang" runat="server" FieldLabel="Gudang" ValueField="c_gdg"
          DisplayField="v_gdgdesc" AllowBlank="false">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="false" />
          </CustomConfig>
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
        </ext:ComboBox>
        <%--<ext:CompositeField runat="server" FieldLabel="Periode">
          <Items>
            <ext:SelectBox ID="cbTahun" runat="server" Width="75" AllowBlank="false" />
            <ext:ComboBox ID="cbPeriode" runat="server" AllowBlank="false" DisplayField="v_ket"
              ValueField="c_type">
              <CustomConfig>
                <ext:ConfigItem Name="allowBlank" Value="false" />
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
                    <ext:Parameter Name="model" Value="2001" />
                    <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '002', 'System.String'],
                                    ['c_portal = @0', '9', 'System.Char']]" Mode="Raw" />
                    <ext:Parameter Name="sort" Value="c_notrans" />
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
          </Items>
        </ext:CompositeField>--%>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan">
          <Items>
            <ext:Radio ID="rdgNonRetur" runat="server" BoxLabel="Non Retur" Checked="true" InputValue="01" />
            <ext:Radio ID="rdgRetur" runat="server" BoxLabel="Retur" InputValue="02" />
           
          </Items>
        </ext:RadioGroup>
        <ext:CompositeField ID="CompositeField2" runat="server" FieldLabel="Periode">
          <Items>
            <ext:SelectBox ID="SBPeriode" runat="server" FieldLabel="Periode" SelectedIndex="0"
              AllowBlank="false">
              <Items>
                <ext:ListItem Value="01" Text="Triwulan I(Jan-Mar)" />
                <ext:ListItem Value="02" Text="Triwulan II(Apr-Jun)" />
                <ext:ListItem Value="03" Text="Triwulan III(Jul-Sep)" />
                <ext:ListItem Value="04" Text="Triwulan IV(Okt-Des)" />
              </Items>
            </ext:SelectBox>
            <ext:SelectBox ID="sbTahun" runat="server" FieldLabel="Year" SelectedIndex="0" Width="60px"
              AllowBlank="false">
              <Items>
                <ext:ListItem Value="2017" Text="2017" />
                <ext:ListItem Value="2018" Text="2018" />
                <ext:ListItem Value="2019" Text="2019" />
                <ext:ListItem Value="2020" Text="2020" />
                <ext:ListItem Value="2021" Text="2021" />
              </Items>
            </ext:SelectBox>            
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
            <ext:Label ID="Label3" runat="server" Text="&nbsp;" />
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
        <Click OnEvent="Report_OnGenerate" Before="return isValidForm(#{frmReportSJ});">
          <EventMask ShowMask="true" />
          
          <%--hafizh--%>
          
          <ExtraParams>
            <ext:Parameter Name="TipeID" Value="#{hfTipe}.getValue()" Mode="Raw" />
          </ExtraParams>
          
          <%--end hafzih--%>
          
        </Click>
      </DirectEvents>
    </ext:Button>
    <ext:Button runat="server" Icon="Reload" Text="Bersihkan">
      <Listeners>
        <Click Handler="initializePanel(#{frmReportSJ}, #{chkAsync});" />
      </Listeners>
    </ext:Button>
  </Buttons>
  <Listeners>
    <AfterRender Handler="initializePanel(#{frmReportSJ}, #{chkAsync});" />
  </Listeners>
</ext:Panel>
