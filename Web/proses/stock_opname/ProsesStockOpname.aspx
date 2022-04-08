<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProsesStockOpname.aspx.cs" 
Inherits="proses_stock_opname_ProsesStockOpname" MasterPageFile="~/Master.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" AutoScroll="true" Layout="FormLayout"
        Padding="5" ButtonAlign="Center">
        <Items>
          <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
          <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />
        </Items>
        <Buttons>
          <ext:Button ID="Button1" runat="server" Icon="CogStart" Text="Frezee Stock">
              <DirectEvents>
                <Click OnEvent="ProsesSO_OnGenerate">
                  <Confirmation ConfirmRequest="true" Title="Proses ?" Message="Anda yakin ingin memproses data ini?" />
                </Click>
              </DirectEvents>
            </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
   </ext:Viewport>
</asp:Content>