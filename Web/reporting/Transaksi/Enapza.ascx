<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Enapza.ascx.cs" Inherits="reporting_history_Enapza" %>
<ext:Panel ID="Panel1" runat="server">
  <Items>
    <ext:Hidden ID="hidWndDown" runat="server" />
    <ext:FormPanel ID="frmReportKriteria" runat="server" Padding="5" Frame="True" 
      Layout="Form">
      <Items>
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode">
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
        <ext:RadioGroup ID="rdgBentukReport" runat="server" ColumnsNumber="1" FieldLabel="Bentuk Laporan">
          <Items>
            <ext:Radio ID="rdgSC" runat="server" BoxLabel="Soft Copy" Checked="true" InputValue="01" />
            <ext:Radio ID="rdgHC" runat="server" BoxLabel="Hard Copy" InputValue="02" Hidden="true" />
          </Items>
        </ext:RadioGroup>
        <ext:RadioGroup ID="rdgTipeReport" runat="server" ColumnsNumber="1" FieldLabel="Tipe Laporan">
          <Items>
            <ext:Radio ID="rdgPrekursor" runat="server" BoxLabel="Prekursor" Checked="true" InputValue="01" />
            <ext:Radio ID="rdgPsikotropika" runat="server" BoxLabel="Psikotropika" InputValue="02" />
            <ext:Radio ID="rdgOOT" runat="server" BoxLabel="OOT" InputValue="03" /> 
          </Items>
        </ext:RadioGroup>             
        <ext:SelectBox ID="cbRptTypeOutput" runat="server" FieldLabel="Output" SelectedIndex="1"
          AllowBlank="false">
          <Items>
            <%--<ext:ListItem Value="01" Text="PDF" /> indra 20170822--%>
            <ext:ListItem Value="02" Text="Excel Data Only" />
            <ext:ListItem Value="03" Text="Excel" />
            <%--<ext:ListItem Value="05" Text="CSV" /> indra 20170822--%>
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
