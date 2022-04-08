<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HutangVCVwCtrl.ascx.cs"
  Inherits="keuangan_pembayaran_HutangVCVwCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="480" Width="800" Hidden="true"
  Maximizable="true" MinHeight="480" MinWidth="800" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfDebit" runat="server" />
    <ext:Hidden ID="hfVoucher" runat="server" />
    <ext:Store runat="server" ID="bufferStore">
      <Reader>
        <ext:ArrayReader>
          <Fields>
            <ext:RecordField Name="Faktur" />
            <ext:RecordField Name="FakturBeli" />
            <ext:RecordField Name="DebitNote" />
            <ext:RecordField Name="SuplierID" />
            <ext:RecordField Name="FakturDate" Type="Date" DateFormat="M$" />
            <ext:RecordField Name="Value" Type="Float" />
            <ext:RecordField Name="SisaTagihan" Type="Float" />
            <ext:RecordField Name="Pembayaran" Type="Float" />
            <ext:RecordField Name="l_bayar" Type="Boolean" />
            <ext:RecordField Name="l_void" Type="Boolean" />
            <ext:RecordField Name="l_modified" Type="Boolean" />
            <ext:RecordField Name="v_type" />
            <ext:RecordField Name="v_ket" />
          </Fields>
        </ext:ArrayReader>
      </Reader>
    </ext:Store>
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <North MinHeight="220" Collapsible="false">
        <ext:FormPanel ID="frmHeaders" runat="server" Title="Header" Height="220" AutoScroll="true"
          Layout="Fit">
          <Items>
            <ext:Panel runat="server" Padding="5" AutoScroll="true" Layout="Column">
              <Items>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:DateField ID="txTanggalHdr" runat="server" AllowBlank="false" FieldLabel="Tanggal"
                      Width="100" Format="dd-MM-yyyy" />
                    <ext:ComboBox ID="cbTipeHdr" runat="server" FieldLabel="Jenis Bayar" DisplayField="v_ket"
                      ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '48', 'System.String'],
                                    ['c_portal = @0', '0', 'System.Char']]" Mode="Raw" />
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <%--<Listeners>
                        <Change Handler="changeJenisBayar(newValue, #{txGiroHdr}, #{txTempoGiroHdr});" />
                      </Listeners>--%>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbSuplierHdr" runat="server" DisplayField="v_nama" ValueField="c_nosup"
                      FieldLabel="Pemasok" Width="255" ItemSelector="tr.search-item" PageSize="10" ListWidth="400"
                      MinChars="3">
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
                            <ext:Parameter Name="parameters" Value="[['l_hide = @0', false, 'System.Boolean']]"
                              Mode="Raw" />
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
                        <table cellpading="0" cellspacing="1" style="width: 400px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td></tr>
                        <tpl for="."><tr class="search-item">
                            <td>{c_nosup}</td><td>{v_nama}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <%--<Listeners>
                        <Change Handler="refreshGrid(#{gridDetailBayarVC});resetDefaultValue(#{lbSisaHdr}, #{gridDetailBayarVC});" />
                      </Listeners>--%>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbTipeBayarHdr" runat="server" FieldLabel="Tipe Bayar" DisplayField="v_ket"
                      ValueField="c_type" ItemSelector="tr.search-item" ListWidth="200" MinChars="3"
                      Width="100">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="-1" />
                            <ext:Parameter Name="allQuery" Value="true" />
                            <ext:Parameter Name="model" Value="2001" />
                            <ext:Parameter Name="parameters" Value="[['c_notrans = @0', '49', 'System.String'],
                                    ['c_portal = @0', '3', 'System.Char']]" Mode="Raw" />
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
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 200px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Deskripsi</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_type}</td><td>{v_ket}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbBankHdr" runat="server" DisplayField="v_bank" ValueField="c_bank"
                      FieldLabel="Bank" Width="200" ItemSelector="tr.search-item" PageSize="10" ListWidth="300"
                      MinChars="3">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2091" />
                            <ext:Parameter Name="parameters" Value="[['c_cab1 = @0', 'X9', 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="v_bank" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_bank" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_bank" />
                                <ext:RecordField Name="c_cab1" />
                                <ext:RecordField Name="v_bank" />
                                <ext:RecordField Name="v_bankcab" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Template runat="server">
                        <Html>
                        <table cellpading="0" cellspacing="1" style="width: 300px">
                        <tr><td class="body-panel">Kode</td><td class="body-panel">Pemasok</td>
                        <td class="body-panel">Cabang</td></tr>
                        <tpl for="."><tr class="search-item">
                        <td>{c_bank}</td><td>{v_bank}</td><td>{v_bankcab}</td>
                        </tr></tpl>
                        </table>
                        </Html>
                      </Template>
                      <%--<Listeners>
                        <Change Handler="clearRelatedComboRecursive(true, #{cbRekeningHdr});" />
                      </Listeners>--%>
                    </ext:ComboBox>
                    <ext:ComboBox ID="cbRekeningHdr" runat="server" DisplayField="c_rekno" ValueField="c_rekno"
                      FieldLabel="Rekening" Width="225" MinChars="3" Mode="Local">
                      <Store>
                        <ext:Store runat="server">
                          <Proxy>
                            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                              CallbackParam="soaScmsCallback" />
                          </Proxy>
                          <BaseParams>
                            <ext:Parameter Name="start" Value="={0}" />
                            <ext:Parameter Name="limit" Value="={10}" />
                            <ext:Parameter Name="model" Value="2101" />
                            <ext:Parameter Name="parameters" Value="[['c_bank = @0', paramValueGetter(#{cbBankHdr}), 'System.String']]"
                              Mode="Raw" />
                            <ext:Parameter Name="sort" Value="c_rekno" />
                            <ext:Parameter Name="dir" Value="ASC" />
                          </BaseParams>
                          <Reader>
                            <ext:JsonReader IDProperty="c_rekno" Root="d.records" SuccessProperty="d.success"
                              TotalProperty="d.totalRows">
                              <Fields>
                                <ext:RecordField Name="c_bank" />
                                <ext:RecordField Name="c_rekno" />
                              </Fields>
                            </ext:JsonReader>
                          </Reader>
                        </ext:Store>
                      </Store>
                      <Triggers>
                        <ext:FieldTrigger Icon="Search" Qtip="Reload" />
                      </Triggers>
                      <%--<Listeners>
                        <TriggerClick Handler="if(index == 0) { reloadData(this); }" />
                      </Listeners>--%>
                    </ext:ComboBox>
                  </Items>
                </ext:Panel>
                <ext:Panel runat="server" Border="false" Header="false" ColumnWidth=".5" Layout="Form">
                  <Items>
                    <ext:TextField ID="txGiroHdr" runat="server" FieldLabel="Nomor Giro" MaxLength="50"
                      AllowBlank="true" />
                    <ext:DateField ID="txTempoGiroHdr" runat="server" FieldLabel="Tanggal" Width="100"
                      Format="dd-MM-yyyy" AllowBlank="true" />
                    <ext:CompositeField runat="server" FieldLabel="Kurs">
                      <Items>
                        <ext:ComboBox ID="cbKursHdr" runat="server" DisplayField="v_desc" ValueField="c_kurs"
                          Width="100" ItemSelector="tr.search-item" ListWidth="275" MinChars="3">
                          <Store>
                            <ext:Store runat="server">
                              <Proxy>
                                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                                  CallbackParam="soaScmsCallback" />
                              </Proxy>
                              <BaseParams>
                                <ext:Parameter Name="start" Value="={0}" />
                                <ext:Parameter Name="limit" Value="-1" />
                                <ext:Parameter Name="allQuery" Value="true" />
                                <ext:Parameter Name="model" Value="2071" />
                                <ext:Parameter Name="parameters" Value="[[]]" Mode="Raw" />
                                <ext:Parameter Name="sort" Value="v_desc" />
                                <ext:Parameter Name="dir" Value="ASC" />
                              </BaseParams>
                              <Reader>
                                <ext:JsonReader IDProperty="c_kurs" Root="d.records" SuccessProperty="d.success"
                                  TotalProperty="d.totalRows">
                                  <Fields>
                                    <ext:RecordField Name="c_kurs" />
                                    <ext:RecordField Name="v_desc" />
                                    <ext:RecordField Name="c_symbol" />
                                    <ext:RecordField Name="n_currency" Type="Float" />
                                  </Fields>
                                </ext:JsonReader>
                              </Reader>
                            </ext:Store>
                          </Store>
                          <Template runat="server">
                            <Html>
                            <table cellpading="0" cellspacing="1" style="width: 275px">
                            <tr><td class="body-panel">Simbol</td>
                            <td class="body-panel">Nama</td><td class="body-panel">Nilai</td></tr>
                            <tpl for="."><tr class="search-item">
                            <td>{c_symbol}</td><td>{v_desc}</td><td>{n_currency:this.formatNumber}</td>
                            </tr></tpl>
                            </table>
                            </Html>
                            <Functions>
                              <ext:JFunction Name="formatNumber" Fn="myFormatNumber" />
                            </Functions>
                          </Template>
                          <%--<Listeners>
                            <Change Handler="changeKursAct(this, newValue, #{txKursValueHdr});" />
                          </Listeners>--%>
                        </ext:ComboBox>
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:NumberField ID="txKursValueHdr" runat="server" AllowBlank="false" AllowDecimals="true"
                          DecimalPrecision="2" AllowNegative="false" Width="75" />
                      </Items>
                    </ext:CompositeField>
                    <ext:NumberField ID="txJumlahHdr" runat="server" AllowBlank="false" FieldLabel="Jumlah"
                      AllowDecimals="true" DecimalPrecision="2" AllowNegative="false">
                      <%--<Listeners>
                        <Change Handler="changesJumlah(newValue, #{lbSisaHdr}, #{hfDebit});resetDefaultValue(#{lbSisaHdr}, #{gridDetailBayarVC});" />
                      </Listeners>--%>
                    </ext:NumberField>
                    <ext:Label ID="lbSisaHdr" runat="server" FieldLabel="Sisa" />
                    <ext:CompositeField runat="server" FieldLabel="Biaya Admin">
                      <Items>
                        <ext:NumberField ID="txBiayaAdminHdr" runat="server" AllowDecimals="true" DecimalPrecision="2"
                          AllowNegative="false" />
                        <ext:Label runat="server" Text="&nbsp;" />
                        <ext:Checkbox ID="chkDPHdr" runat="server" BoxLabel="Uang muka" />
                      </Items>
                    </ext:CompositeField>
                    <ext:TextField ID="txKeteranganHdr" runat="server" FieldLabel="Keterangan" MaxLength="100"
                      Width="255" />
                  </Items>
                </ext:Panel>
              </Items>
            </ext:Panel>
          </Items>
        </ext:FormPanel>
      </North>
      <Center MinHeight="150">
        <ext:GridPanel ID="gridDetailBayarVC" runat="server">
          <LoadMask ShowMask="true" />
          <%--<Listeners>
            <BeforeEdit Handler="checkBayar(e, #{cbSuplierHdr}.getValue(), #{lbSisaHdr});" />
          </Listeners>--%>
          <SelectionModel>
            <ext:RowSelectionModel SingleSelect="true" />
          </SelectionModel>
          <Store>
            <ext:Store runat="server" RemoteSort="true" SkinID="OriginalExtStore">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <AutoLoadParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={20}" />
              </AutoLoadParams>
              <BaseParams>
                <ext:Parameter Name="start" Value="0" />
                <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
                <ext:Parameter Name="model" Value="0081" />
                <ext:Parameter Name="sort" Value="FakturDate" />
                <ext:Parameter Name="dir" Value="ASC" />
                <ext:Parameter Name="parameters" Value="[['c_noteno = @0', paramValueGetter(#{hfDebit}), 'System.String']]"
                  Mode="Raw" />
              </BaseParams>
              <Reader>
                <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                  <Fields>
                    <ext:RecordField Name="Faktur" />
                    <ext:RecordField Name="FakturBeli" />
                    <ext:RecordField Name="Kurs" />
                    <ext:RecordField Name="SuplierID" />
                    <ext:RecordField Name="Tipe" />
                    <ext:RecordField Name="FakturDate" Type="Date" DateFormat="M$" />
                    <ext:RecordField Name="Value" Type="Float" />
                    <ext:RecordField Name="SisaTagihan" Type="Float" />
                    <ext:RecordField Name="Pembayaran" Type="Float" />
                    <ext:RecordField Name="l_bayar" Type="Boolean" />
                    <ext:RecordField Name="l_new" Type="Boolean" />
                    <ext:RecordField Name="l_void" Type="Boolean" />
                    <ext:RecordField Name="l_modified" Type="Boolean" />
                    <ext:RecordField Name="v_ket" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
              <SortInfo Field="FakturDate" Direction="ASC" />
              <%--<Listeners>
                <Load Handler="loadPopulateMergeBuffer(store, records);" />
              </Listeners>--%>
            </ext:Store>
          </Store>
          <ColumnModel>
            <Columns>
              <%--<ext:CommandColumn Width="25">
                <Commands>
                  <ext:GridCommand CommandName="Delete" Icon="Decline" ToolTip-Title="Command" ToolTip-Text="Hapus entry" />
                  <ext:GridCommand CommandName="Void" Icon="Cross" ToolTip-Title="Command" ToolTip-Text="Void Entry" />
                </Commands>
                <PrepareToolbar Handler="prepareCommands(record, toolbar, #{hfSpNo}.getValue());" />
              </ext:CommandColumn>--%>
              <ext:CheckColumn DataIndex="l_bayar" ColumnID="cekBayar" Width="25" Editable="true" />
              <ext:Column DataIndex="Faktur" Header="Nomor" />
              <ext:Column DataIndex="FakturBeli" Header="Faktur Pembelian" Width="175" />
              <ext:DateColumn DataIndex="FakturDate" Header="Tanggal" Width="150" Format="dd-MM-yyyy" />
              <ext:NumberColumn DataIndex="Value" Header="Tagihan" Format="0.000,00/i" />
              <ext:NumberColumn DataIndex="SisaTagihan" Header="Sisa Tagihan" Format="0.000,00/i" />
              <ext:NumberColumn DataIndex="Pembayaran" ColumnID="jmlBayar" Header="Pembayaran"
                Format="0.000,00/i" Editable="false">
                <%--<Editor>
                  <ext:NumberField runat="server" AllowBlank="false" AllowDecimals="true" AllowNegative="true" DecimalPrecision="2">
                    <Listeners>
                      <Focus Handler="this.selectText();" />
                    </Listeners>
                  </ext:NumberField>
                </Editor>--%>
              </ext:NumberColumn>
            </Columns>
          </ColumnModel>
          <BottomBar>
            <ext:PagingToolbar ID="gmPagingBB" runat="server" PageSize="20">
              <Items>
                <ext:Label runat="server" Text="Page size:" />
                <ext:ToolbarSpacer runat="server" Width="10" />
                <ext:ComboBox runat="server" Width="80">
                  <Items>
                    <ext:ListItem Text="5" />
                    <ext:ListItem Text="10" />
                    <ext:ListItem Text="20" />
                    <ext:ListItem Text="50" />
                    <ext:ListItem Text="100" />
                  </Items>
                  <SelectedItem Value="20" />
                  <Listeners>
                    <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
                  </Listeners>
                </ext:ComboBox>
              </Items>
            </ext:PagingToolbar>
          </BottomBar>
        </ext:GridPanel>
      </Center>
    </ext:BorderLayout>
  </Items>
  <Buttons>
    <%--<ext:Button runat="server" Icon="Disk" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndCustomStore(#{frmHeaders},#{bufferStore});">
          <Confirmation BeforeConfirm="return verifyHeaderAndCustomStore(#{frmHeaders},#{bufferStore});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <EventMask ShowMask="true" />
          <ExtraParams>
            <ext:Parameter Name="NumberID" Value="" />
            <ext:Parameter Name="storeValues" Value="saveStoreToServer(#{bufferStore})"
              Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>--%>
    <ext:Button runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
