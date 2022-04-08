<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaGoodPrint.ascx.cs" Inherits="transaksi_retur_SerahTerimaGoodPrint" %>


<ext:Window ID="winDetail" runat="server" Height="110" Width="360" Hidden="true" Resizable="false"
  Maximizable="false" MinHeight="110" MinWidth="360" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfNoSup" runat="server" />
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfTypeDO" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="210" MaxHeight="210" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Height="110" Width="360" Padding="10" Layout="FitLayout">
          <Items>
            <ext:Panel ID="Panel1" runat="server" Border="false" Header="false" Layout="Form" LabelAlign="Left">
              <Items>
                <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Periode">
                  <Items>
                    <ext:DateField ID="txPeriode1" runat="server" Vtype="daterange" Format="dd-MM-yyyy" AllowBlank="true">
                        <CustomConfig>
                            <ext:ConfigItem Name="endDateField" Value="#{txPeriode2}" Mode="Value" />
                        </CustomConfig>
                    </ext:DateField>
                    <ext:Label ID="Label1" runat="server" Text="-" />
                    <ext:DateField ID="txPeriode2" runat="server" Vtype="daterange" Format="dd-MM-yyyy" AllowBlank="true">
                        <CustomConfig>
                            <ext:ConfigItem Name="startDateField" Value="#{txPeriode1}" Mode="Value" />
                        </CustomConfig>
                    </ext:DateField>
                  </Items>
                </ext:CompositeField>            
              </Items>
            </ext:Panel>              
          </Items>
        </ext:FormPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <ext:Button runat="server" ID="btnReport" Icon="Printer" Text="Cetak Report">
      <DirectEvents>
        <Click OnEvent="Report_onGenerate">
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="#{hfNoSup}.getValue()" Mode="Raw" />
            <ext:Parameter Name="StoreID" Value="01" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />