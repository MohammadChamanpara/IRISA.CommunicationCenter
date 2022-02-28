using IRISA.CommunicationCenter.Components;
using System.ComponentModel;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public partial class MainForm
    {

        private IContainer components = null;
        private TabControl tabControl;
        private TabPage settingsTabPage;
        private TabPage TransfersTabPage;
        private IrisaDataGrid transfersDataGrid;
        private TabPage eventsTabPage;
        private IrisaDataGrid eventsDataGrid;
        private FlowLayoutPanel settingsPanel;
        private SplitContainer settingsSplitContainer;
        private PropertyGrid settingsPropertyGrid;
        private TabPage mainTabPage;
        private ToolStrip eventsToolStrip;
        private ToolStrip transfersToolStrip;
        private ImageList imageList;
        private ToolStrip bottomToolStrip;
        private NotifyIcon notifyIcon;
        private ToolStripButton eventsMoreButton;
        private ToolStripButton transfersRefreshButton;
        private ToolStripButton transfersMoreButton;
        private ToolStripButton closeApplicationButton;
        private ToolStripButton restartApplicationButton;
        private ToolStripButton stopStartApplicationButton;
        private ToolStripButton minimizeApplicationButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStrip settingsToolStrip;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton toolStripButton1;
        private ToolStripLabel toolStripLabel2;
        private FlowLayoutPanel adaptersPanel;
        private SplitContainer adaptersSplitContainer;
        private ToolStrip adaptersToolStrip;
        private ToolStripLabel toolStripLabel3;
        private ToolStripButton toolStripButton2;
        private ToolStripLabel toolStripLabel4;
        private ToolStripLabel toolStripLabel5;
        private ToolStripLabel toolStripLabel6;
        private ToolStripButton eventsRefreshButton;
        private ToolStripButton TelegramDetailsButton;
        private GroupBox telegramSearchGroupbox;
        private Label label1;
        private FlowLayoutPanel searchFlowLayoutPanel;
        private Label label2;
        private CheckBox sentCheckBox;
        private CheckBox droppedCheckBox;
        private Label label5;
        private TextBox dropReasonTextBox;
        private ToolStrip searchToolStrip;
        private ToolStripButton clearSearchButton;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private FlowLayoutPanel flowLayoutPanel3;
        private FlowLayoutPanel flowLayoutPanel5;
        private FlowLayoutPanel flowLayoutPanel4;
        private MaskedTextBox sendHourTextBox;
        private Label label10;
        private MaskedTextBox sendMinuteTextBox;
        private Label label9;
        private MaskedTextBox sendSecondTextBox;
        private Label label7;
        private MaskedTextBox sendDateTextBox;
        private Label label8;
        private FlowLayoutPanel flowLayoutPanel6;
        private Label label4;
        private MaskedTextBox telegramIdTextBox;
        private FlowLayoutPanel flowLayoutPanel7;
        private Label label3;
        private TextBox destinationTextbox;
        private FlowLayoutPanel flowLayoutPanel8;
        private TextBox sourceTextBox;
        private FlowLayoutPanel flowLayoutPanel9;
        private MaskedTextBox idTextBox;
        private FlowLayoutPanel flowLayoutPanel10;
        private Label label6;
        private FlowLayoutPanel flowLayoutPanel11;
        private MaskedTextBox receiveSecondTextBox;
        private Label label11;
        private MaskedTextBox receiveMinuteTextBox;
        private Label label12;
        private MaskedTextBox receiveHourTextBox;
        private Label label13;
        private MaskedTextBox receiveDateTextBox;
        private Label label14;
        private Label label15;
        private Label label16;
        private Splitter splitter1;
        private Splitter splitter3;
        private Splitter splitter2;
        private Splitter splitter4;
        private Splitter splitter5;
        private Splitter splitter6;
        private Splitter splitter7;
        private Splitter splitter8;
        private FlowLayoutPanel flowLayoutPanel12;
        private Label label17;
        private Splitter splitter9;
        private Splitter splitter10;
        private Splitter splitter11;
        private FlowLayoutPanel flowLayoutPanel13;
        private Label TransfersResultsCountLabel;
        private Label label20;
        private Label TransfersPageSizeLabel;
        private ToolStripButton searchTelegramButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.mainTabPage = new System.Windows.Forms.TabPage();
            this.adaptersSplitContainer = new System.Windows.Forms.SplitContainer();
            this.adaptersPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.adaptersToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.settingsTabPage = new System.Windows.Forms.TabPage();
            this.settingsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.settingsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.settingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.settingsToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.TransfersTabPage = new System.Windows.Forms.TabPage();
            this.transfersDataGrid = new IRISA.CommunicationCenter.Components.IrisaDataGrid();
            this.transfersIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sourceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DestinationColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TelegramIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BodyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReceiveTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SentColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DroppedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DropReasonColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.telegramSearchGroupbox = new System.Windows.Forms.GroupBox();
            this.searchFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.searchToolStrip = new System.Windows.Forms.ToolStrip();
            this.clearSearchButton = new System.Windows.Forms.ToolStripButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.MaskedTextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.sourceTextBox = new System.Windows.Forms.TextBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.destinationTextbox = new System.Windows.Forms.TextBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.telegramIdTextBox = new System.Windows.Forms.MaskedTextBox();
            this.splitter5 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.sendSecondTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.sendMinuteTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.sendHourTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.sendDateTextBox = new System.Windows.Forms.MaskedTextBox();
            this.splitter6 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel10 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.flowLayoutPanel11 = new System.Windows.Forms.FlowLayoutPanel();
            this.receiveSecondTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.receiveMinuteTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.receiveHourTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.receiveDateTextBox = new System.Windows.Forms.MaskedTextBox();
            this.splitter7 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label15 = new System.Windows.Forms.Label();
            this.sentCheckBox = new System.Windows.Forms.CheckBox();
            this.splitter8 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.droppedCheckBox = new System.Windows.Forms.CheckBox();
            this.splitter9 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.dropReasonTextBox = new System.Windows.Forms.TextBox();
            this.splitter10 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel12 = new System.Windows.Forms.FlowLayoutPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.flowLayoutPanel13 = new System.Windows.Forms.FlowLayoutPanel();
            this.TransfersResultsCountLabel = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.TransfersPageSizeLabel = new System.Windows.Forms.Label();
            this.splitter11 = new System.Windows.Forms.Splitter();
            this.DoTelegramSearch = new System.Windows.Forms.Button();
            this.transfersToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.transfersRefreshButton = new System.Windows.Forms.ToolStripButton();
            this.transfersMoreButton = new System.Windows.Forms.ToolStripButton();
            this.TelegramDetailsButton = new System.Windows.Forms.ToolStripButton();
            this.searchTelegramButton = new System.Windows.Forms.ToolStripButton();
            this.eventsTabPage = new System.Windows.Forms.TabPage();
            this.eventsDataGrid = new IRISA.CommunicationCenter.Components.IrisaDataGrid();
            this.IdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogLevelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TextColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogsSearchGroupBox = new System.Windows.Forms.GroupBox();
            this.eventsSearchflowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.ClearFiltersToolStrip = new System.Windows.Forms.ToolStrip();
            this.clearSearchEventsPanel = new System.Windows.Forms.ToolStripButton();
            this.splitter12 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel15 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel16 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel17 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel18 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel22 = new System.Windows.Forms.FlowLayoutPanel();
            this.label19 = new System.Windows.Forms.Label();
            this.flowLayoutPanel30 = new System.Windows.Forms.FlowLayoutPanel();
            this.SearchKeywordTextBox = new System.Windows.Forms.MaskedTextBox();
            this.flowLayoutPanel19 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel20 = new System.Windows.Forms.FlowLayoutPanel();
            this.splitter17 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel21 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel23 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel14 = new System.Windows.Forms.FlowLayoutPanel();
            this.label34 = new System.Windows.Forms.Label();
            this.flowLayoutPanel27 = new System.Windows.Forms.FlowLayoutPanel();
            this.LogLevelComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel24 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel25 = new System.Windows.Forms.FlowLayoutPanel();
            this.splitter21 = new System.Windows.Forms.Splitter();
            this.flowLayoutPanel26 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel28 = new System.Windows.Forms.FlowLayoutPanel();
            this.label18 = new System.Windows.Forms.Label();
            this.flowLayoutPanel29 = new System.Windows.Forms.FlowLayoutPanel();
            this.LogsResultsCountLabel = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.LogsPageSizeLabel = new System.Windows.Forms.Label();
            this.splitter13 = new System.Windows.Forms.Splitter();
            this.DoIccEventSearch = new System.Windows.Forms.Button();
            this.eventsToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.eventsRefreshButton = new System.Windows.Forms.ToolStripButton();
            this.eventsMoreButton = new System.Windows.Forms.ToolStripButton();
            this.iccEventSearchButton = new System.Windows.Forms.ToolStripButton();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.bottomToolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeApplicationButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.restartApplicationButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.stopStartApplicationButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.minimizeApplicationButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adaptersSplitContainer)).BeginInit();
            this.adaptersSplitContainer.Panel1.SuspendLayout();
            this.adaptersSplitContainer.SuspendLayout();
            this.adaptersToolStrip.SuspendLayout();
            this.settingsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.settingsSplitContainer)).BeginInit();
            this.settingsSplitContainer.Panel1.SuspendLayout();
            this.settingsSplitContainer.Panel2.SuspendLayout();
            this.settingsSplitContainer.SuspendLayout();
            this.settingsToolStrip.SuspendLayout();
            this.TransfersTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transfersDataGrid)).BeginInit();
            this.telegramSearchGroupbox.SuspendLayout();
            this.searchFlowLayoutPanel.SuspendLayout();
            this.searchToolStrip.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.flowLayoutPanel8.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel10.SuspendLayout();
            this.flowLayoutPanel11.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel12.SuspendLayout();
            this.flowLayoutPanel13.SuspendLayout();
            this.transfersToolStrip.SuspendLayout();
            this.eventsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGrid)).BeginInit();
            this.LogsSearchGroupBox.SuspendLayout();
            this.eventsSearchflowLayout.SuspendLayout();
            this.ClearFiltersToolStrip.SuspendLayout();
            this.flowLayoutPanel22.SuspendLayout();
            this.flowLayoutPanel30.SuspendLayout();
            this.flowLayoutPanel19.SuspendLayout();
            this.flowLayoutPanel14.SuspendLayout();
            this.flowLayoutPanel27.SuspendLayout();
            this.flowLayoutPanel28.SuspendLayout();
            this.flowLayoutPanel29.SuspendLayout();
            this.eventsToolStrip.SuspendLayout();
            this.bottomToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.mainTabPage);
            this.tabControl.Controls.Add(this.settingsTabPage);
            this.tabControl.Controls.Add(this.TransfersTabPage);
            this.tabControl.Controls.Add(this.eventsTabPage);
            this.tabControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.imageList;
            this.tabControl.ItemSize = new System.Drawing.Size(10, 0);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabControl.RightToLeftLayout = true;
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1094, 606);
            this.tabControl.TabIndex = 0;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // mainTabPage
            // 
            this.mainTabPage.BackColor = System.Drawing.Color.White;
            this.mainTabPage.Controls.Add(this.adaptersSplitContainer);
            this.mainTabPage.Controls.Add(this.adaptersToolStrip);
            this.mainTabPage.ImageIndex = 0;
            this.mainTabPage.Location = new System.Drawing.Point(4, 31);
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.mainTabPage.Size = new System.Drawing.Size(1086, 571);
            this.mainTabPage.TabIndex = 3;
            this.mainTabPage.Text = "کلاینت ها";
            // 
            // adaptersSplitContainer
            // 
            this.adaptersSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.adaptersSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adaptersSplitContainer.Location = new System.Drawing.Point(3, 47);
            this.adaptersSplitContainer.Name = "adaptersSplitContainer";
            // 
            // adaptersSplitContainer.Panel1
            // 
            this.adaptersSplitContainer.Panel1.Controls.Add(this.adaptersPanel);
            this.adaptersSplitContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // adaptersSplitContainer.Panel2
            // 
            this.adaptersSplitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.adaptersSplitContainer.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.adaptersSplitContainer.Size = new System.Drawing.Size(1080, 521);
            this.adaptersSplitContainer.SplitterDistance = 381;
            this.adaptersSplitContainer.SplitterWidth = 10;
            this.adaptersSplitContainer.TabIndex = 1;
            // 
            // adaptersPanel
            // 
            this.adaptersPanel.AutoScroll = true;
            this.adaptersPanel.BackColor = System.Drawing.Color.White;
            this.adaptersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.adaptersPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.adaptersPanel.Location = new System.Drawing.Point(0, 0);
            this.adaptersPanel.Name = "adaptersPanel";
            this.adaptersPanel.Size = new System.Drawing.Size(379, 519);
            this.adaptersPanel.TabIndex = 0;
            // 
            // adaptersToolStrip
            // 
            this.adaptersToolStrip.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.adaptersToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.adaptersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel3,
            this.toolStripButton2,
            this.toolStripLabel4});
            this.adaptersToolStrip.Location = new System.Drawing.Point(3, 3);
            this.adaptersToolStrip.Name = "adaptersToolStrip";
            this.adaptersToolStrip.Size = new System.Drawing.Size(1080, 44);
            this.adaptersToolStrip.TabIndex = 2;
            this.adaptersToolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(442, 41);
            this.toolStripLabel3.Text = "در این بخش کلاینت های متصل به سیستم را مشاهده می فرمایید . کلاینت های فعال به صور" +
    "ت ";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::IRISA.CommunicationCenter.Properties.Resources.connected;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(110, 41);
            this.toolStripLabel4.Text = "نمایش داده می شوند.";
            // 
            // settingsTabPage
            // 
            this.settingsTabPage.BackColor = System.Drawing.Color.White;
            this.settingsTabPage.Controls.Add(this.settingsSplitContainer);
            this.settingsTabPage.Controls.Add(this.settingsToolStrip);
            this.settingsTabPage.ImageIndex = 1;
            this.settingsTabPage.Location = new System.Drawing.Point(4, 31);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.settingsTabPage.Size = new System.Drawing.Size(1086, 571);
            this.settingsTabPage.TabIndex = 0;
            this.settingsTabPage.Text = "تنظیمات";
            // 
            // settingsSplitContainer
            // 
            this.settingsSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.settingsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsSplitContainer.Location = new System.Drawing.Point(3, 50);
            this.settingsSplitContainer.Name = "settingsSplitContainer";
            // 
            // settingsSplitContainer.Panel1
            // 
            this.settingsSplitContainer.Panel1.Controls.Add(this.settingsPanel);
            this.settingsSplitContainer.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // settingsSplitContainer.Panel2
            // 
            this.settingsSplitContainer.Panel2.Controls.Add(this.settingsPropertyGrid);
            this.settingsSplitContainer.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.settingsSplitContainer.Size = new System.Drawing.Size(1080, 518);
            this.settingsSplitContainer.SplitterDistance = 329;
            this.settingsSplitContainer.SplitterWidth = 10;
            this.settingsSplitContainer.TabIndex = 1;
            // 
            // settingsPanel
            // 
            this.settingsPanel.AutoScroll = true;
            this.settingsPanel.AutoSize = true;
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.settingsPanel.Location = new System.Drawing.Point(0, 0);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(327, 516);
            this.settingsPanel.TabIndex = 0;
            // 
            // settingsPropertyGrid
            // 
            this.settingsPropertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.settingsPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.settingsPropertyGrid.Name = "settingsPropertyGrid";
            this.settingsPropertyGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.settingsPropertyGrid.Size = new System.Drawing.Size(739, 516);
            this.settingsPropertyGrid.TabIndex = 0;
            this.settingsPropertyGrid.UseCompatibleTextRendering = true;
            // 
            // settingsToolStrip
            // 
            this.settingsToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.settingsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripButton1,
            this.toolStripLabel2});
            this.settingsToolStrip.Location = new System.Drawing.Point(3, 3);
            this.settingsToolStrip.Name = "settingsToolStrip";
            this.settingsToolStrip.Size = new System.Drawing.Size(1080, 47);
            this.settingsToolStrip.TabIndex = 2;
            this.settingsToolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(214, 44);
            this.toolStripLabel1.Text = "برای ایجاد تغییر در تنظیمات ، توسط  دکمه";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::IRISA.CommunicationCenter.Properties.Resources.stop;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(44, 44);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(117, 44);
            this.toolStripLabel2.Text = "برنامه را متوقف نمایید.";
            // 
            // TransfersTabPage
            // 
            this.TransfersTabPage.BackColor = System.Drawing.Color.White;
            this.TransfersTabPage.Controls.Add(this.transfersDataGrid);
            this.TransfersTabPage.Controls.Add(this.telegramSearchGroupbox);
            this.TransfersTabPage.Controls.Add(this.transfersToolStrip);
            this.TransfersTabPage.ImageIndex = 2;
            this.TransfersTabPage.Location = new System.Drawing.Point(4, 31);
            this.TransfersTabPage.Name = "TransfersTabPage";
            this.TransfersTabPage.Size = new System.Drawing.Size(1086, 571);
            this.TransfersTabPage.TabIndex = 2;
            this.TransfersTabPage.Text = "تلگرام ها";
            // 
            // transfersDataGrid
            // 
            this.transfersDataGrid.AllowUserToAddRows = false;
            this.transfersDataGrid.AllowUserToDeleteRows = false;
            this.transfersDataGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.AliceBlue;
            this.transfersDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.transfersDataGrid.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.transfersDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.transfersDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.transfersIdColumn,
            this.sourceColumn,
            this.DestinationColumn,
            this.TelegramIdColumn,
            this.BodyColumn,
            this.SendTimeColumn,
            this.ReceiveTimeColumn,
            this.SentColumn,
            this.DroppedColumn,
            this.DropReasonColumn});
            this.transfersDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transfersDataGrid.Location = new System.Drawing.Point(0, 116);
            this.transfersDataGrid.MultiSelect = false;
            this.transfersDataGrid.Name = "transfersDataGrid";
            this.transfersDataGrid.ReadOnly = true;
            this.transfersDataGrid.Size = new System.Drawing.Size(1086, 455);
            this.transfersDataGrid.TabIndex = 1;
            // 
            // transfersIdColumn
            // 
            this.transfersIdColumn.DataPropertyName = "TransferId";
            this.transfersIdColumn.HeaderText = "شناسه";
            this.transfersIdColumn.Name = "transfersIdColumn";
            this.transfersIdColumn.ReadOnly = true;
            this.transfersIdColumn.Width = 70;
            // 
            // sourceColumn
            // 
            this.sourceColumn.DataPropertyName = "Source";
            this.sourceColumn.HeaderText = "مبدا";
            this.sourceColumn.Name = "sourceColumn";
            this.sourceColumn.ReadOnly = true;
            this.sourceColumn.Width = 60;
            // 
            // DestinationColumn
            // 
            this.DestinationColumn.DataPropertyName = "Destination";
            this.DestinationColumn.HeaderText = "مقصد";
            this.DestinationColumn.Name = "DestinationColumn";
            this.DestinationColumn.ReadOnly = true;
            this.DestinationColumn.Width = 60;
            // 
            // TelegramIdColumn
            // 
            this.TelegramIdColumn.DataPropertyName = "TelegramId";
            this.TelegramIdColumn.HeaderText = "شناسه تلگرام";
            this.TelegramIdColumn.Name = "TelegramIdColumn";
            this.TelegramIdColumn.ReadOnly = true;
            // 
            // BodyColumn
            // 
            this.BodyColumn.DataPropertyName = "BodyString";
            this.BodyColumn.HeaderText = "محتوای تلگرام";
            this.BodyColumn.Name = "BodyColumn";
            this.BodyColumn.ReadOnly = true;
            // 
            // SendTimeColumn
            // 
            this.SendTimeColumn.DataPropertyName = "PersianSendTime";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "G";
            dataGridViewCellStyle2.NullValue = null;
            this.SendTimeColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.SendTimeColumn.HeaderText = "زمان ارسال";
            this.SendTimeColumn.Name = "SendTimeColumn";
            this.SendTimeColumn.ReadOnly = true;
            this.SendTimeColumn.Width = 130;
            // 
            // ReceiveTimeColumn
            // 
            this.ReceiveTimeColumn.DataPropertyName = "PersianReceiveTime";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "G";
            dataGridViewCellStyle3.NullValue = null;
            this.ReceiveTimeColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.ReceiveTimeColumn.HeaderText = "زمان دریافت";
            this.ReceiveTimeColumn.Name = "ReceiveTimeColumn";
            this.ReceiveTimeColumn.ReadOnly = true;
            this.ReceiveTimeColumn.Width = 130;
            // 
            // SentColumn
            // 
            this.SentColumn.DataPropertyName = "Sent";
            this.SentColumn.HeaderText = "ارسال شده";
            this.SentColumn.Name = "SentColumn";
            this.SentColumn.ReadOnly = true;
            this.SentColumn.Width = 85;
            // 
            // DroppedColumn
            // 
            this.DroppedColumn.DataPropertyName = "Dropped";
            this.DroppedColumn.HeaderText = "حذف شده";
            this.DroppedColumn.Name = "DroppedColumn";
            this.DroppedColumn.ReadOnly = true;
            this.DroppedColumn.Width = 80;
            // 
            // DropReasonColumn
            // 
            this.DropReasonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DropReasonColumn.DataPropertyName = "DropReason";
            this.DropReasonColumn.HeaderText = "دلیل حذف";
            this.DropReasonColumn.MinimumWidth = 400;
            this.DropReasonColumn.Name = "DropReasonColumn";
            this.DropReasonColumn.ReadOnly = true;
            // 
            // telegramSearchGroupbox
            // 
            this.telegramSearchGroupbox.Controls.Add(this.searchFlowLayoutPanel);
            this.telegramSearchGroupbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.telegramSearchGroupbox.Location = new System.Drawing.Point(0, 31);
            this.telegramSearchGroupbox.Name = "telegramSearchGroupbox";
            this.telegramSearchGroupbox.Size = new System.Drawing.Size(1086, 85);
            this.telegramSearchGroupbox.TabIndex = 3;
            this.telegramSearchGroupbox.TabStop = false;
            this.telegramSearchGroupbox.Text = "جستجو";
            this.telegramSearchGroupbox.Visible = false;
            // 
            // searchFlowLayoutPanel
            // 
            this.searchFlowLayoutPanel.AutoScroll = true;
            this.searchFlowLayoutPanel.AutoSize = true;
            this.searchFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.searchFlowLayoutPanel.Controls.Add(this.searchToolStrip);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter1);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel9);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter2);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel8);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter3);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel7);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter4);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel6);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter5);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel5);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter6);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel10);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter7);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel3);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter8);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel2);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter9);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel1);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter10);
            this.searchFlowLayoutPanel.Controls.Add(this.flowLayoutPanel12);
            this.searchFlowLayoutPanel.Controls.Add(this.splitter11);
            this.searchFlowLayoutPanel.Controls.Add(this.DoTelegramSearch);
            this.searchFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchFlowLayoutPanel.Location = new System.Drawing.Point(3, 17);
            this.searchFlowLayoutPanel.Name = "searchFlowLayoutPanel";
            this.searchFlowLayoutPanel.Size = new System.Drawing.Size(1080, 65);
            this.searchFlowLayoutPanel.TabIndex = 2;
            this.searchFlowLayoutPanel.WrapContents = false;
            // 
            // searchToolStrip
            // 
            this.searchToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.searchToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.searchToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.searchToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearSearchButton});
            this.searchToolStrip.Location = new System.Drawing.Point(1090, 0);
            this.searchToolStrip.Name = "searchToolStrip";
            this.searchToolStrip.Size = new System.Drawing.Size(56, 47);
            this.searchToolStrip.TabIndex = 37;
            this.searchToolStrip.Text = "toolStrip1";
            // 
            // clearSearchButton
            // 
            this.clearSearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearSearchButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.close;
            this.clearSearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearSearchButton.Name = "clearSearchButton";
            this.clearSearchButton.Size = new System.Drawing.Size(44, 44);
            this.clearSearchButton.Text = "toolStripButton3";
            this.clearSearchButton.ToolTipText = "حذف فیلترینگ";
            this.clearSearchButton.Click += new System.EventHandler(this.ClearSearchButton_Click);
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Location = new System.Drawing.Point(1089, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1, 52);
            this.splitter1.TabIndex = 50;
            this.splitter1.TabStop = false;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.AutoSize = true;
            this.flowLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel9.Controls.Add(this.label1);
            this.flowLayoutPanel9.Controls.Add(this.idTextBox);
            this.flowLayoutPanel9.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel9.Location = new System.Drawing.Point(1019, 3);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Size = new System.Drawing.Size(67, 46);
            this.flowLayoutPanel9.TabIndex = 0;
            this.flowLayoutPanel9.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "شناسه";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // idTextBox
            // 
            this.idTextBox.Location = new System.Drawing.Point(3, 22);
            this.idTextBox.Mask = "0000000";
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.PromptChar = ' ';
            this.idTextBox.Size = new System.Drawing.Size(61, 21);
            this.idTextBox.TabIndex = 2;
            this.idTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter2.Location = new System.Drawing.Point(1015, 0);
            this.splitter2.Margin = new System.Windows.Forms.Padding(0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(1, 52);
            this.splitter2.TabIndex = 53;
            this.splitter2.TabStop = false;
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.AutoSize = true;
            this.flowLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel8.Controls.Add(this.label2);
            this.flowLayoutPanel8.Controls.Add(this.sourceTextBox);
            this.flowLayoutPanel8.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel8.Location = new System.Drawing.Point(959, 3);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(53, 46);
            this.flowLayoutPanel8.TabIndex = 1;
            this.flowLayoutPanel8.WrapContents = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "مبدأ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sourceTextBox
            // 
            this.sourceTextBox.Location = new System.Drawing.Point(3, 22);
            this.sourceTextBox.Name = "sourceTextBox";
            this.sourceTextBox.Size = new System.Drawing.Size(47, 21);
            this.sourceTextBox.TabIndex = 10;
            // 
            // splitter3
            // 
            this.splitter3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter3.Location = new System.Drawing.Point(955, 0);
            this.splitter3.Margin = new System.Windows.Forms.Padding(0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(1, 52);
            this.splitter3.TabIndex = 52;
            this.splitter3.TabStop = false;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel7.Controls.Add(this.label3);
            this.flowLayoutPanel7.Controls.Add(this.destinationTextbox);
            this.flowLayoutPanel7.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(899, 3);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(53, 46);
            this.flowLayoutPanel7.TabIndex = 2;
            this.flowLayoutPanel7.WrapContents = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 3);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "مقصد";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // destinationTextbox
            // 
            this.destinationTextbox.Location = new System.Drawing.Point(3, 22);
            this.destinationTextbox.Name = "destinationTextbox";
            this.destinationTextbox.Size = new System.Drawing.Size(47, 21);
            this.destinationTextbox.TabIndex = 39;
            // 
            // splitter4
            // 
            this.splitter4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter4.Location = new System.Drawing.Point(895, 0);
            this.splitter4.Margin = new System.Windows.Forms.Padding(0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(1, 52);
            this.splitter4.TabIndex = 54;
            this.splitter4.TabStop = false;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel6.Controls.Add(this.label4);
            this.flowLayoutPanel6.Controls.Add(this.telegramIdTextBox);
            this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(813, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(79, 46);
            this.flowLayoutPanel6.TabIndex = 3;
            this.flowLayoutPanel6.WrapContents = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "شناسه تلگرام";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // telegramIdTextBox
            // 
            this.telegramIdTextBox.Location = new System.Drawing.Point(5, 22);
            this.telegramIdTextBox.Mask = "000000000";
            this.telegramIdTextBox.Name = "telegramIdTextBox";
            this.telegramIdTextBox.PromptChar = ' ';
            this.telegramIdTextBox.Size = new System.Drawing.Size(71, 21);
            this.telegramIdTextBox.TabIndex = 37;
            this.telegramIdTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // splitter5
            // 
            this.splitter5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter5.Location = new System.Drawing.Point(809, 0);
            this.splitter5.Margin = new System.Windows.Forms.Padding(0);
            this.splitter5.Name = "splitter5";
            this.splitter5.Size = new System.Drawing.Size(1, 52);
            this.splitter5.TabIndex = 55;
            this.splitter5.TabStop = false;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel5.Controls.Add(this.label8);
            this.flowLayoutPanel5.Controls.Add(this.flowLayoutPanel4);
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(625, 3);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(181, 46);
            this.flowLayoutPanel5.TabIndex = 4;
            this.flowLayoutPanel5.WrapContents = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(3, 3);
            this.label8.Margin = new System.Windows.Forms.Padding(3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(175, 13);
            this.label8.TabIndex = 45;
            this.label8.Text = "زمان ارسال";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.sendSecondTextBox);
            this.flowLayoutPanel4.Controls.Add(this.label10);
            this.flowLayoutPanel4.Controls.Add(this.sendMinuteTextBox);
            this.flowLayoutPanel4.Controls.Add(this.label9);
            this.flowLayoutPanel4.Controls.Add(this.sendHourTextBox);
            this.flowLayoutPanel4.Controls.Add(this.label7);
            this.flowLayoutPanel4.Controls.Add(this.label14);
            this.flowLayoutPanel4.Controls.Add(this.sendDateTextBox);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 19);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(181, 27);
            this.flowLayoutPanel4.TabIndex = 44;
            this.flowLayoutPanel4.WrapContents = false;
            // 
            // sendSecondTextBox
            // 
            this.sendSecondTextBox.Location = new System.Drawing.Point(157, 3);
            this.sendSecondTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sendSecondTextBox.Mask = "00";
            this.sendSecondTextBox.Name = "sendSecondTextBox";
            this.sendSecondTextBox.PromptChar = ' ';
            this.sendSecondTextBox.Size = new System.Drawing.Size(24, 21);
            this.sendSecondTextBox.TabIndex = 3;
            this.sendSecondTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(146, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(11, 27);
            this.label10.TabIndex = 42;
            this.label10.Text = ":";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sendMinuteTextBox
            // 
            this.sendMinuteTextBox.Location = new System.Drawing.Point(122, 3);
            this.sendMinuteTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sendMinuteTextBox.Mask = "00";
            this.sendMinuteTextBox.Name = "sendMinuteTextBox";
            this.sendMinuteTextBox.PromptChar = ' ';
            this.sendMinuteTextBox.Size = new System.Drawing.Size(24, 21);
            this.sendMinuteTextBox.TabIndex = 2;
            this.sendMinuteTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(111, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(11, 27);
            this.label9.TabIndex = 41;
            this.label9.Text = ":";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sendHourTextBox
            // 
            this.sendHourTextBox.Location = new System.Drawing.Point(87, 3);
            this.sendHourTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sendHourTextBox.Mask = "00";
            this.sendHourTextBox.Name = "sendHourTextBox";
            this.sendHourTextBox.PromptChar = ' ';
            this.sendHourTextBox.Size = new System.Drawing.Size(24, 21);
            this.sendHourTextBox.TabIndex = 1;
            this.sendHourTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(87, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 27);
            this.label7.TabIndex = 43;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(77, 0);
            this.label14.Margin = new System.Windows.Forms.Padding(0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(10, 27);
            this.label14.TabIndex = 44;
            this.label14.Text = " ";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sendDateTextBox
            // 
            this.sendDateTextBox.Location = new System.Drawing.Point(0, 3);
            this.sendDateTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.sendDateTextBox.Mask = "1400/00/00";
            this.sendDateTextBox.Name = "sendDateTextBox";
            this.sendDateTextBox.PromptChar = ' ';
            this.sendDateTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sendDateTextBox.Size = new System.Drawing.Size(77, 21);
            this.sendDateTextBox.TabIndex = 0;
            this.sendDateTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // splitter6
            // 
            this.splitter6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter6.Location = new System.Drawing.Point(621, 0);
            this.splitter6.Margin = new System.Windows.Forms.Padding(0);
            this.splitter6.Name = "splitter6";
            this.splitter6.Size = new System.Drawing.Size(1, 52);
            this.splitter6.TabIndex = 56;
            this.splitter6.TabStop = false;
            // 
            // flowLayoutPanel10
            // 
            this.flowLayoutPanel10.AutoSize = true;
            this.flowLayoutPanel10.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel10.Controls.Add(this.label6);
            this.flowLayoutPanel10.Controls.Add(this.flowLayoutPanel11);
            this.flowLayoutPanel10.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel10.Location = new System.Drawing.Point(437, 3);
            this.flowLayoutPanel10.Name = "flowLayoutPanel10";
            this.flowLayoutPanel10.Size = new System.Drawing.Size(181, 46);
            this.flowLayoutPanel10.TabIndex = 5;
            this.flowLayoutPanel10.WrapContents = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Margin = new System.Windows.Forms.Padding(3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(175, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "زمان دریافت";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel11
            // 
            this.flowLayoutPanel11.AutoSize = true;
            this.flowLayoutPanel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel11.Controls.Add(this.receiveSecondTextBox);
            this.flowLayoutPanel11.Controls.Add(this.label11);
            this.flowLayoutPanel11.Controls.Add(this.receiveMinuteTextBox);
            this.flowLayoutPanel11.Controls.Add(this.label12);
            this.flowLayoutPanel11.Controls.Add(this.receiveHourTextBox);
            this.flowLayoutPanel11.Controls.Add(this.label13);
            this.flowLayoutPanel11.Controls.Add(this.receiveDateTextBox);
            this.flowLayoutPanel11.Location = new System.Drawing.Point(0, 19);
            this.flowLayoutPanel11.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel11.Name = "flowLayoutPanel11";
            this.flowLayoutPanel11.Size = new System.Drawing.Size(181, 27);
            this.flowLayoutPanel11.TabIndex = 44;
            this.flowLayoutPanel11.WrapContents = false;
            // 
            // receiveSecondTextBox
            // 
            this.receiveSecondTextBox.Location = new System.Drawing.Point(157, 3);
            this.receiveSecondTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.receiveSecondTextBox.Mask = "00";
            this.receiveSecondTextBox.Name = "receiveSecondTextBox";
            this.receiveSecondTextBox.PromptChar = ' ';
            this.receiveSecondTextBox.Size = new System.Drawing.Size(24, 21);
            this.receiveSecondTextBox.TabIndex = 3;
            this.receiveSecondTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(146, 0);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(11, 27);
            this.label11.TabIndex = 42;
            this.label11.Text = ":";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // receiveMinuteTextBox
            // 
            this.receiveMinuteTextBox.Location = new System.Drawing.Point(122, 3);
            this.receiveMinuteTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.receiveMinuteTextBox.Mask = "00";
            this.receiveMinuteTextBox.Name = "receiveMinuteTextBox";
            this.receiveMinuteTextBox.PromptChar = ' ';
            this.receiveMinuteTextBox.Size = new System.Drawing.Size(24, 21);
            this.receiveMinuteTextBox.TabIndex = 2;
            this.receiveMinuteTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(111, 0);
            this.label12.Margin = new System.Windows.Forms.Padding(0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(11, 27);
            this.label12.TabIndex = 41;
            this.label12.Text = ":";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // receiveHourTextBox
            // 
            this.receiveHourTextBox.Location = new System.Drawing.Point(87, 3);
            this.receiveHourTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.receiveHourTextBox.Mask = "00";
            this.receiveHourTextBox.Name = "receiveHourTextBox";
            this.receiveHourTextBox.PromptChar = ' ';
            this.receiveHourTextBox.Size = new System.Drawing.Size(24, 21);
            this.receiveHourTextBox.TabIndex = 1;
            this.receiveHourTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(77, 0);
            this.label13.Margin = new System.Windows.Forms.Padding(0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(10, 27);
            this.label13.TabIndex = 43;
            this.label13.Text = " ";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // receiveDateTextBox
            // 
            this.receiveDateTextBox.Location = new System.Drawing.Point(0, 3);
            this.receiveDateTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.receiveDateTextBox.Mask = "1400/00/00";
            this.receiveDateTextBox.Name = "receiveDateTextBox";
            this.receiveDateTextBox.PromptChar = ' ';
            this.receiveDateTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.receiveDateTextBox.Size = new System.Drawing.Size(77, 21);
            this.receiveDateTextBox.TabIndex = 0;
            this.receiveDateTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // splitter7
            // 
            this.splitter7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter7.Location = new System.Drawing.Point(433, 0);
            this.splitter7.Margin = new System.Windows.Forms.Padding(0);
            this.splitter7.Name = "splitter7";
            this.splitter7.Size = new System.Drawing.Size(1, 52);
            this.splitter7.TabIndex = 57;
            this.splitter7.TabStop = false;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.label15);
            this.flowLayoutPanel3.Controls.Add(this.sentCheckBox);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(355, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(75, 39);
            this.flowLayoutPanel3.TabIndex = 6;
            this.flowLayoutPanel3.WrapContents = false;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(3, 3);
            this.label15.Margin = new System.Windows.Forms.Padding(3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "ارسال شده";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sentCheckBox
            // 
            this.sentCheckBox.AutoSize = true;
            this.sentCheckBox.Checked = true;
            this.sentCheckBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.sentCheckBox.Location = new System.Drawing.Point(57, 22);
            this.sentCheckBox.Name = "sentCheckBox";
            this.sentCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sentCheckBox.Size = new System.Drawing.Size(15, 14);
            this.sentCheckBox.TabIndex = 9;
            this.sentCheckBox.ThreeState = true;
            this.sentCheckBox.UseVisualStyleBackColor = true;
            // 
            // splitter8
            // 
            this.splitter8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter8.Location = new System.Drawing.Point(351, 0);
            this.splitter8.Margin = new System.Windows.Forms.Padding(0);
            this.splitter8.Name = "splitter8";
            this.splitter8.Size = new System.Drawing.Size(1, 52);
            this.splitter8.TabIndex = 58;
            this.splitter8.TabStop = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.label16);
            this.flowLayoutPanel2.Controls.Add(this.droppedCheckBox);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(274, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(74, 39);
            this.flowLayoutPanel2.TabIndex = 7;
            this.flowLayoutPanel2.WrapContents = false;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(3, 3);
            this.label16.Margin = new System.Windows.Forms.Padding(3);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(68, 13);
            this.label16.TabIndex = 37;
            this.label16.Text = "حذف شده";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // droppedCheckBox
            // 
            this.droppedCheckBox.AutoSize = true;
            this.droppedCheckBox.Checked = true;
            this.droppedCheckBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.droppedCheckBox.Location = new System.Drawing.Point(56, 22);
            this.droppedCheckBox.Name = "droppedCheckBox";
            this.droppedCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.droppedCheckBox.Size = new System.Drawing.Size(15, 14);
            this.droppedCheckBox.TabIndex = 10;
            this.droppedCheckBox.ThreeState = true;
            this.droppedCheckBox.UseVisualStyleBackColor = true;
            // 
            // splitter9
            // 
            this.splitter9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter9.Location = new System.Drawing.Point(270, 0);
            this.splitter9.Margin = new System.Windows.Forms.Padding(0);
            this.splitter9.Name = "splitter9";
            this.splitter9.Size = new System.Drawing.Size(1, 52);
            this.splitter9.TabIndex = 61;
            this.splitter9.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.dropReasonTextBox);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(168, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(99, 46);
            this.flowLayoutPanel1.TabIndex = 8;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(3, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 35;
            this.label5.Text = "دلیل حذف";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dropReasonTextBox
            // 
            this.dropReasonTextBox.Location = new System.Drawing.Point(3, 22);
            this.dropReasonTextBox.Name = "dropReasonTextBox";
            this.dropReasonTextBox.Size = new System.Drawing.Size(93, 21);
            this.dropReasonTextBox.TabIndex = 11;
            // 
            // splitter10
            // 
            this.splitter10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter10.Location = new System.Drawing.Point(164, 0);
            this.splitter10.Margin = new System.Windows.Forms.Padding(0);
            this.splitter10.Name = "splitter10";
            this.splitter10.Size = new System.Drawing.Size(1, 52);
            this.splitter10.TabIndex = 62;
            this.splitter10.TabStop = false;
            // 
            // flowLayoutPanel12
            // 
            this.flowLayoutPanel12.AutoSize = true;
            this.flowLayoutPanel12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel12.Controls.Add(this.label17);
            this.flowLayoutPanel12.Controls.Add(this.flowLayoutPanel13);
            this.flowLayoutPanel12.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel12.Location = new System.Drawing.Point(85, 3);
            this.flowLayoutPanel12.Name = "flowLayoutPanel12";
            this.flowLayoutPanel12.Size = new System.Drawing.Size(76, 46);
            this.flowLayoutPanel12.TabIndex = 60;
            this.flowLayoutPanel12.WrapContents = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(3, 3);
            this.label17.Margin = new System.Windows.Forms.Padding(3);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 60;
            this.label17.Text = "تعداد رکورد ها";
            // 
            // flowLayoutPanel13
            // 
            this.flowLayoutPanel13.AutoSize = true;
            this.flowLayoutPanel13.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel13.Controls.Add(this.TransfersResultsCountLabel);
            this.flowLayoutPanel13.Controls.Add(this.label20);
            this.flowLayoutPanel13.Controls.Add(this.TransfersPageSizeLabel);
            this.flowLayoutPanel13.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel13.Location = new System.Drawing.Point(11, 22);
            this.flowLayoutPanel13.Name = "flowLayoutPanel13";
            this.flowLayoutPanel13.Size = new System.Drawing.Size(62, 21);
            this.flowLayoutPanel13.TabIndex = 64;
            this.flowLayoutPanel13.WrapContents = false;
            // 
            // TransfersResultsCountLabel
            // 
            this.TransfersResultsCountLabel.AutoSize = true;
            this.TransfersResultsCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TransfersResultsCountLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.TransfersResultsCountLabel.Location = new System.Drawing.Point(3, 3);
            this.TransfersResultsCountLabel.Margin = new System.Windows.Forms.Padding(3);
            this.TransfersResultsCountLabel.Name = "TransfersResultsCountLabel";
            this.TransfersResultsCountLabel.Size = new System.Drawing.Size(15, 15);
            this.TransfersResultsCountLabel.TabIndex = 61;
            this.TransfersResultsCountLabel.Text = "0";
            this.TransfersResultsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(24, 3);
            this.label20.Margin = new System.Windows.Forms.Padding(3);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(14, 13);
            this.label20.TabIndex = 60;
            this.label20.Text = "از";
            // 
            // TransfersPageSizeLabel
            // 
            this.TransfersPageSizeLabel.AutoSize = true;
            this.TransfersPageSizeLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TransfersPageSizeLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.TransfersPageSizeLabel.Location = new System.Drawing.Point(44, 3);
            this.TransfersPageSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.TransfersPageSizeLabel.Name = "TransfersPageSizeLabel";
            this.TransfersPageSizeLabel.Size = new System.Drawing.Size(15, 15);
            this.TransfersPageSizeLabel.TabIndex = 62;
            this.TransfersPageSizeLabel.Text = "0";
            this.TransfersPageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter11
            // 
            this.splitter11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter11.Location = new System.Drawing.Point(81, 0);
            this.splitter11.Margin = new System.Windows.Forms.Padding(0);
            this.splitter11.Name = "splitter11";
            this.splitter11.Size = new System.Drawing.Size(1, 52);
            this.splitter11.TabIndex = 63;
            this.splitter11.TabStop = false;
            // 
            // DoTelegramSearch
            // 
            this.DoTelegramSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DoTelegramSearch.Location = new System.Drawing.Point(3, 14);
            this.DoTelegramSearch.Name = "DoTelegramSearch";
            this.DoTelegramSearch.Size = new System.Drawing.Size(75, 23);
            this.DoTelegramSearch.TabIndex = 64;
            this.DoTelegramSearch.Text = "جستجو";
            this.DoTelegramSearch.UseVisualStyleBackColor = true;
            this.DoTelegramSearch.Click += new System.EventHandler(this.DoTelegramSearch_Click);
            // 
            // transfersToolStrip
            // 
            this.transfersToolStrip.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.transfersToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.transfersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.transfersRefreshButton,
            this.transfersMoreButton,
            this.TelegramDetailsButton,
            this.searchTelegramButton});
            this.transfersToolStrip.Location = new System.Drawing.Point(0, 0);
            this.transfersToolStrip.Name = "transfersToolStrip";
            this.transfersToolStrip.Size = new System.Drawing.Size(1086, 31);
            this.transfersToolStrip.TabIndex = 0;
            this.transfersToolStrip.Text = "toolStrip2";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(313, 28);
            this.toolStripLabel5.Text = "در این جدول کلیه تلگرام های مورد انتقال مشاهده می شوند.";
            // 
            // transfersRefreshButton
            // 
            this.transfersRefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.transfersRefreshButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.refresh;
            this.transfersRefreshButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.transfersRefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.transfersRefreshButton.Name = "transfersRefreshButton";
            this.transfersRefreshButton.Size = new System.Drawing.Size(28, 28);
            this.transfersRefreshButton.Text = "toolStripButton3";
            this.transfersRefreshButton.ToolTipText = "بازیابی مجدد رکورد ها";
            this.transfersRefreshButton.Click += new System.EventHandler(this.RefreshRecordsButton_Click);
            // 
            // transfersMoreButton
            // 
            this.transfersMoreButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.transfersMoreButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.more;
            this.transfersMoreButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.transfersMoreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.transfersMoreButton.Name = "transfersMoreButton";
            this.transfersMoreButton.Size = new System.Drawing.Size(28, 28);
            this.transfersMoreButton.Text = "toolStripButton4";
            this.transfersMoreButton.ToolTipText = "بازیابی رکورد های بیشتر";
            this.transfersMoreButton.Click += new System.EventHandler(this.MoreRecordsButton_Click);
            // 
            // TelegramDetailsButton
            // 
            this.TelegramDetailsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.TelegramDetailsButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.tree;
            this.TelegramDetailsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TelegramDetailsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TelegramDetailsButton.Name = "TelegramDetailsButton";
            this.TelegramDetailsButton.Size = new System.Drawing.Size(26, 28);
            this.TelegramDetailsButton.Text = "toolStripButton4";
            this.TelegramDetailsButton.ToolTipText = "جزئیات تلگرام";
            this.TelegramDetailsButton.Click += new System.EventHandler(this.TelegramDetails_Click);
            // 
            // searchTelegramButton
            // 
            this.searchTelegramButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchTelegramButton.Image = ((System.Drawing.Image)(resources.GetObject("searchTelegramButton.Image")));
            this.searchTelegramButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.searchTelegramButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchTelegramButton.Name = "searchTelegramButton";
            this.searchTelegramButton.Size = new System.Drawing.Size(28, 28);
            this.searchTelegramButton.Text = "جستجو";
            this.searchTelegramButton.Click += new System.EventHandler(this.TelegramSearchShowHideButton_Click);
            // 
            // eventsTabPage
            // 
            this.eventsTabPage.BackColor = System.Drawing.Color.White;
            this.eventsTabPage.Controls.Add(this.eventsDataGrid);
            this.eventsTabPage.Controls.Add(this.LogsSearchGroupBox);
            this.eventsTabPage.Controls.Add(this.eventsToolStrip);
            this.eventsTabPage.ImageIndex = 3;
            this.eventsTabPage.Location = new System.Drawing.Point(4, 31);
            this.eventsTabPage.Name = "eventsTabPage";
            this.eventsTabPage.Size = new System.Drawing.Size(1086, 571);
            this.eventsTabPage.TabIndex = 1;
            this.eventsTabPage.Text = "رویداد ها";
            // 
            // eventsDataGrid
            // 
            this.eventsDataGrid.AllowUserToAddRows = false;
            this.eventsDataGrid.AllowUserToDeleteRows = false;
            this.eventsDataGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.AliceBlue;
            this.eventsDataGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.eventsDataGrid.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.eventsDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdColumn,
            this.TimeColumn,
            this.LogLevelColumn,
            this.TextColumn});
            this.eventsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventsDataGrid.Location = new System.Drawing.Point(0, 116);
            this.eventsDataGrid.MultiSelect = false;
            this.eventsDataGrid.Name = "eventsDataGrid";
            this.eventsDataGrid.ReadOnly = true;
            this.eventsDataGrid.Size = new System.Drawing.Size(1086, 455);
            this.eventsDataGrid.TabIndex = 0;
            // 
            // IdColumn
            // 
            this.IdColumn.DataPropertyName = "Id";
            this.IdColumn.HeaderText = "شناسه";
            this.IdColumn.Name = "IdColumn";
            this.IdColumn.ReadOnly = true;
            this.IdColumn.Width = 67;
            // 
            // TimeColumn
            // 
            this.TimeColumn.DataPropertyName = "PersianTime";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.NullValue = null;
            this.TimeColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.TimeColumn.HeaderText = "زمان رویداد";
            this.TimeColumn.Name = "TimeColumn";
            this.TimeColumn.ReadOnly = true;
            this.TimeColumn.Width = 130;
            // 
            // LogLevelColumn
            // 
            this.LogLevelColumn.DataPropertyName = "PersianLogLevel";
            this.LogLevelColumn.HeaderText = "نوع رویداد";
            this.LogLevelColumn.Name = "LogLevelColumn";
            this.LogLevelColumn.ReadOnly = true;
            this.LogLevelColumn.Width = 76;
            // 
            // TextColumn
            // 
            this.TextColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TextColumn.DataPropertyName = "Text";
            this.TextColumn.HeaderText = "متن رویداد";
            this.TextColumn.MinimumWidth = 150;
            this.TextColumn.Name = "TextColumn";
            this.TextColumn.ReadOnly = true;
            // 
            // LogsSearchGroupBox
            // 
            this.LogsSearchGroupBox.Controls.Add(this.eventsSearchflowLayout);
            this.LogsSearchGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.LogsSearchGroupBox.Location = new System.Drawing.Point(0, 31);
            this.LogsSearchGroupBox.Name = "LogsSearchGroupBox";
            this.LogsSearchGroupBox.Size = new System.Drawing.Size(1086, 85);
            this.LogsSearchGroupBox.TabIndex = 15;
            this.LogsSearchGroupBox.TabStop = false;
            this.LogsSearchGroupBox.Text = "جستجو";
            this.LogsSearchGroupBox.Visible = false;
            // 
            // eventsSearchflowLayout
            // 
            this.eventsSearchflowLayout.AutoScroll = true;
            this.eventsSearchflowLayout.AutoSize = true;
            this.eventsSearchflowLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.eventsSearchflowLayout.Controls.Add(this.ClearFiltersToolStrip);
            this.eventsSearchflowLayout.Controls.Add(this.splitter12);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel15);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel16);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel17);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel18);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel22);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel19);
            this.eventsSearchflowLayout.Controls.Add(this.splitter17);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel21);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel23);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel14);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel24);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel25);
            this.eventsSearchflowLayout.Controls.Add(this.splitter21);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel26);
            this.eventsSearchflowLayout.Controls.Add(this.flowLayoutPanel28);
            this.eventsSearchflowLayout.Controls.Add(this.splitter13);
            this.eventsSearchflowLayout.Controls.Add(this.DoIccEventSearch);
            this.eventsSearchflowLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eventsSearchflowLayout.Location = new System.Drawing.Point(3, 17);
            this.eventsSearchflowLayout.Name = "eventsSearchflowLayout";
            this.eventsSearchflowLayout.Size = new System.Drawing.Size(1080, 65);
            this.eventsSearchflowLayout.TabIndex = 2;
            this.eventsSearchflowLayout.WrapContents = false;
            // 
            // ClearFiltersToolStrip
            // 
            this.ClearFiltersToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.ClearFiltersToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ClearFiltersToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.ClearFiltersToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearSearchEventsPanel});
            this.ClearFiltersToolStrip.Location = new System.Drawing.Point(1024, 0);
            this.ClearFiltersToolStrip.Name = "ClearFiltersToolStrip";
            this.ClearFiltersToolStrip.Size = new System.Drawing.Size(56, 65);
            this.ClearFiltersToolStrip.TabIndex = 37;
            this.ClearFiltersToolStrip.Text = "ClearFiltersToolStrip";
            // 
            // clearSearchEventsPanel
            // 
            this.clearSearchEventsPanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearSearchEventsPanel.Image = global::IRISA.CommunicationCenter.Properties.Resources.close;
            this.clearSearchEventsPanel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearSearchEventsPanel.Name = "clearSearchEventsPanel";
            this.clearSearchEventsPanel.Size = new System.Drawing.Size(44, 62);
            this.clearSearchEventsPanel.Text = "toolStripButton3";
            this.clearSearchEventsPanel.ToolTipText = "حذف فیلترینگ";
            this.clearSearchEventsPanel.Click += new System.EventHandler(this.ClearSearchEventsPanel_Click);
            // 
            // splitter12
            // 
            this.splitter12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter12.Location = new System.Drawing.Point(1023, 0);
            this.splitter12.Margin = new System.Windows.Forms.Padding(0);
            this.splitter12.Name = "splitter12";
            this.splitter12.Size = new System.Drawing.Size(1, 65);
            this.splitter12.TabIndex = 50;
            this.splitter12.TabStop = false;
            // 
            // flowLayoutPanel15
            // 
            this.flowLayoutPanel15.AutoSize = true;
            this.flowLayoutPanel15.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel15.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel15.Location = new System.Drawing.Point(1020, 3);
            this.flowLayoutPanel15.Name = "flowLayoutPanel15";
            this.flowLayoutPanel15.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel15.TabIndex = 0;
            this.flowLayoutPanel15.WrapContents = false;
            // 
            // flowLayoutPanel16
            // 
            this.flowLayoutPanel16.AutoSize = true;
            this.flowLayoutPanel16.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel16.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel16.Location = new System.Drawing.Point(1014, 3);
            this.flowLayoutPanel16.Name = "flowLayoutPanel16";
            this.flowLayoutPanel16.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel16.TabIndex = 1;
            this.flowLayoutPanel16.WrapContents = false;
            // 
            // flowLayoutPanel17
            // 
            this.flowLayoutPanel17.AutoSize = true;
            this.flowLayoutPanel17.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel17.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel17.Location = new System.Drawing.Point(1008, 3);
            this.flowLayoutPanel17.Name = "flowLayoutPanel17";
            this.flowLayoutPanel17.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel17.TabIndex = 2;
            this.flowLayoutPanel17.WrapContents = false;
            // 
            // flowLayoutPanel18
            // 
            this.flowLayoutPanel18.AutoSize = true;
            this.flowLayoutPanel18.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel18.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel18.Location = new System.Drawing.Point(1002, 3);
            this.flowLayoutPanel18.Name = "flowLayoutPanel18";
            this.flowLayoutPanel18.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel18.TabIndex = 3;
            this.flowLayoutPanel18.WrapContents = false;
            // 
            // flowLayoutPanel22
            // 
            this.flowLayoutPanel22.Controls.Add(this.label19);
            this.flowLayoutPanel22.Controls.Add(this.flowLayoutPanel30);
            this.flowLayoutPanel22.Location = new System.Drawing.Point(817, 3);
            this.flowLayoutPanel22.Name = "flowLayoutPanel22";
            this.flowLayoutPanel22.Size = new System.Drawing.Size(179, 59);
            this.flowLayoutPanel22.TabIndex = 73;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.Location = new System.Drawing.Point(109, 3);
            this.label19.Margin = new System.Windows.Forms.Padding(3);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(67, 13);
            this.label19.TabIndex = 71;
            this.label19.Text = "کلمه جستجو";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // flowLayoutPanel30
            // 
            this.flowLayoutPanel30.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.flowLayoutPanel30.Controls.Add(this.SearchKeywordTextBox);
            this.flowLayoutPanel30.Location = new System.Drawing.Point(6, 22);
            this.flowLayoutPanel30.Name = "flowLayoutPanel30";
            this.flowLayoutPanel30.Size = new System.Drawing.Size(170, 24);
            this.flowLayoutPanel30.TabIndex = 72;
            // 
            // SearchKeywordTextBox
            // 
            this.SearchKeywordTextBox.Location = new System.Drawing.Point(0, 3);
            this.SearchKeywordTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.SearchKeywordTextBox.Name = "SearchKeywordTextBox";
            this.SearchKeywordTextBox.PromptChar = ' ';
            this.SearchKeywordTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SearchKeywordTextBox.Size = new System.Drawing.Size(170, 21);
            this.SearchKeywordTextBox.TabIndex = 0;
            this.SearchKeywordTextBox.Click += new System.EventHandler(this.MaskedTextBox_Click);
            // 
            // flowLayoutPanel19
            // 
            this.flowLayoutPanel19.AutoSize = true;
            this.flowLayoutPanel19.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel19.Controls.Add(this.flowLayoutPanel20);
            this.flowLayoutPanel19.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel19.Location = new System.Drawing.Point(811, 3);
            this.flowLayoutPanel19.Name = "flowLayoutPanel19";
            this.flowLayoutPanel19.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel19.TabIndex = 4;
            this.flowLayoutPanel19.WrapContents = false;
            // 
            // flowLayoutPanel20
            // 
            this.flowLayoutPanel20.AutoSize = true;
            this.flowLayoutPanel20.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel20.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel20.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel20.Name = "flowLayoutPanel20";
            this.flowLayoutPanel20.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel20.TabIndex = 44;
            this.flowLayoutPanel20.WrapContents = false;
            // 
            // splitter17
            // 
            this.splitter17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter17.Location = new System.Drawing.Point(807, 0);
            this.splitter17.Margin = new System.Windows.Forms.Padding(0);
            this.splitter17.Name = "splitter17";
            this.splitter17.Size = new System.Drawing.Size(1, 65);
            this.splitter17.TabIndex = 56;
            this.splitter17.TabStop = false;
            // 
            // flowLayoutPanel21
            // 
            this.flowLayoutPanel21.AutoSize = true;
            this.flowLayoutPanel21.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel21.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel21.Location = new System.Drawing.Point(804, 3);
            this.flowLayoutPanel21.Name = "flowLayoutPanel21";
            this.flowLayoutPanel21.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel21.TabIndex = 5;
            this.flowLayoutPanel21.WrapContents = false;
            // 
            // flowLayoutPanel23
            // 
            this.flowLayoutPanel23.AutoSize = true;
            this.flowLayoutPanel23.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel23.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel23.Location = new System.Drawing.Point(798, 3);
            this.flowLayoutPanel23.Name = "flowLayoutPanel23";
            this.flowLayoutPanel23.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel23.TabIndex = 6;
            this.flowLayoutPanel23.WrapContents = false;
            // 
            // flowLayoutPanel14
            // 
            this.flowLayoutPanel14.Controls.Add(this.label34);
            this.flowLayoutPanel14.Controls.Add(this.flowLayoutPanel27);
            this.flowLayoutPanel14.Location = new System.Drawing.Point(655, 3);
            this.flowLayoutPanel14.Name = "flowLayoutPanel14";
            this.flowLayoutPanel14.Size = new System.Drawing.Size(137, 59);
            this.flowLayoutPanel14.TabIndex = 65;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label34.Location = new System.Drawing.Point(83, 3);
            this.label34.Margin = new System.Windows.Forms.Padding(3);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(51, 13);
            this.label34.TabIndex = 71;
            this.label34.Text = "نوع رویداد";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // flowLayoutPanel27
            // 
            this.flowLayoutPanel27.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.flowLayoutPanel27.Controls.Add(this.LogLevelComboBox);
            this.flowLayoutPanel27.Location = new System.Drawing.Point(-22, 22);
            this.flowLayoutPanel27.Name = "flowLayoutPanel27";
            this.flowLayoutPanel27.Size = new System.Drawing.Size(156, 24);
            this.flowLayoutPanel27.TabIndex = 72;
            // 
            // LogLevelComboBox
            // 
            this.LogLevelComboBox.FormattingEnabled = true;
            this.LogLevelComboBox.Location = new System.Drawing.Point(32, 3);
            this.LogLevelComboBox.Name = "LogLevelComboBox";
            this.LogLevelComboBox.Size = new System.Drawing.Size(121, 21);
            this.LogLevelComboBox.TabIndex = 73;
            // 
            // flowLayoutPanel24
            // 
            this.flowLayoutPanel24.AutoSize = true;
            this.flowLayoutPanel24.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel24.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel24.Location = new System.Drawing.Point(649, 3);
            this.flowLayoutPanel24.Name = "flowLayoutPanel24";
            this.flowLayoutPanel24.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel24.TabIndex = 7;
            this.flowLayoutPanel24.WrapContents = false;
            // 
            // flowLayoutPanel25
            // 
            this.flowLayoutPanel25.AutoSize = true;
            this.flowLayoutPanel25.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel25.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel25.Location = new System.Drawing.Point(643, 3);
            this.flowLayoutPanel25.Name = "flowLayoutPanel25";
            this.flowLayoutPanel25.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel25.TabIndex = 8;
            this.flowLayoutPanel25.WrapContents = false;
            // 
            // splitter21
            // 
            this.splitter21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter21.Location = new System.Drawing.Point(639, 0);
            this.splitter21.Margin = new System.Windows.Forms.Padding(0);
            this.splitter21.Name = "splitter21";
            this.splitter21.Size = new System.Drawing.Size(1, 65);
            this.splitter21.TabIndex = 62;
            this.splitter21.TabStop = false;
            // 
            // flowLayoutPanel26
            // 
            this.flowLayoutPanel26.AutoSize = true;
            this.flowLayoutPanel26.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel26.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel26.Location = new System.Drawing.Point(636, 3);
            this.flowLayoutPanel26.Name = "flowLayoutPanel26";
            this.flowLayoutPanel26.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel26.TabIndex = 60;
            this.flowLayoutPanel26.WrapContents = false;
            // 
            // flowLayoutPanel28
            // 
            this.flowLayoutPanel28.AutoSize = true;
            this.flowLayoutPanel28.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel28.Controls.Add(this.label18);
            this.flowLayoutPanel28.Controls.Add(this.flowLayoutPanel29);
            this.flowLayoutPanel28.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel28.Location = new System.Drawing.Point(554, 3);
            this.flowLayoutPanel28.Name = "flowLayoutPanel28";
            this.flowLayoutPanel28.Size = new System.Drawing.Size(76, 46);
            this.flowLayoutPanel28.TabIndex = 66;
            this.flowLayoutPanel28.WrapContents = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(3, 3);
            this.label18.Margin = new System.Windows.Forms.Padding(3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(70, 13);
            this.label18.TabIndex = 60;
            this.label18.Text = "تعداد رکورد ها";
            // 
            // flowLayoutPanel29
            // 
            this.flowLayoutPanel29.AutoSize = true;
            this.flowLayoutPanel29.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel29.Controls.Add(this.LogsResultsCountLabel);
            this.flowLayoutPanel29.Controls.Add(this.label21);
            this.flowLayoutPanel29.Controls.Add(this.LogsPageSizeLabel);
            this.flowLayoutPanel29.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel29.Location = new System.Drawing.Point(11, 22);
            this.flowLayoutPanel29.Name = "flowLayoutPanel29";
            this.flowLayoutPanel29.Size = new System.Drawing.Size(62, 21);
            this.flowLayoutPanel29.TabIndex = 64;
            this.flowLayoutPanel29.WrapContents = false;
            // 
            // LogsResultsCountLabel
            // 
            this.LogsResultsCountLabel.AutoSize = true;
            this.LogsResultsCountLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogsResultsCountLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.LogsResultsCountLabel.Location = new System.Drawing.Point(3, 3);
            this.LogsResultsCountLabel.Margin = new System.Windows.Forms.Padding(3);
            this.LogsResultsCountLabel.Name = "LogsResultsCountLabel";
            this.LogsResultsCountLabel.Size = new System.Drawing.Size(15, 15);
            this.LogsResultsCountLabel.TabIndex = 61;
            this.LogsResultsCountLabel.Text = "0";
            this.LogsResultsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(24, 3);
            this.label21.Margin = new System.Windows.Forms.Padding(3);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(14, 13);
            this.label21.TabIndex = 60;
            this.label21.Text = "از";
            // 
            // LogsPageSizeLabel
            // 
            this.LogsPageSizeLabel.AutoSize = true;
            this.LogsPageSizeLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogsPageSizeLabel.ForeColor = System.Drawing.Color.RoyalBlue;
            this.LogsPageSizeLabel.Location = new System.Drawing.Point(44, 3);
            this.LogsPageSizeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.LogsPageSizeLabel.Name = "LogsPageSizeLabel";
            this.LogsPageSizeLabel.Size = new System.Drawing.Size(15, 15);
            this.LogsPageSizeLabel.TabIndex = 62;
            this.LogsPageSizeLabel.Text = "0";
            this.LogsPageSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter13
            // 
            this.splitter13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter13.Location = new System.Drawing.Point(550, 0);
            this.splitter13.Margin = new System.Windows.Forms.Padding(0);
            this.splitter13.Name = "splitter13";
            this.splitter13.Size = new System.Drawing.Size(1, 65);
            this.splitter13.TabIndex = 67;
            this.splitter13.TabStop = false;
            // 
            // DoIccEventSearch
            // 
            this.DoIccEventSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DoIccEventSearch.Location = new System.Drawing.Point(472, 21);
            this.DoIccEventSearch.Name = "DoIccEventSearch";
            this.DoIccEventSearch.Size = new System.Drawing.Size(75, 23);
            this.DoIccEventSearch.TabIndex = 64;
            this.DoIccEventSearch.Text = "جستجو";
            this.DoIccEventSearch.UseVisualStyleBackColor = true;
            this.DoIccEventSearch.Click += new System.EventHandler(this.DoIccEventSearch_Click);
            // 
            // eventsToolStrip
            // 
            this.eventsToolStrip.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.eventsToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.eventsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel6,
            this.eventsRefreshButton,
            this.eventsMoreButton,
            this.iccEventSearchButton});
            this.eventsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.eventsToolStrip.Name = "eventsToolStrip";
            this.eventsToolStrip.Size = new System.Drawing.Size(1086, 31);
            this.eventsToolStrip.TabIndex = 14;
            this.eventsToolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(297, 28);
            this.toolStripLabel6.Text = "در این جدول کلیه رویدادهای سیستم مشاهده می شوند.";
            // 
            // eventsRefreshButton
            // 
            this.eventsRefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eventsRefreshButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.refresh;
            this.eventsRefreshButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.eventsRefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eventsRefreshButton.Name = "eventsRefreshButton";
            this.eventsRefreshButton.Size = new System.Drawing.Size(28, 28);
            this.eventsRefreshButton.Text = "toolStripButton1";
            this.eventsRefreshButton.ToolTipText = "بازیابی مجدد رکورد ها";
            this.eventsRefreshButton.Click += new System.EventHandler(this.RefreshRecordsButton_Click);
            // 
            // eventsMoreButton
            // 
            this.eventsMoreButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eventsMoreButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.more;
            this.eventsMoreButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.eventsMoreButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eventsMoreButton.Name = "eventsMoreButton";
            this.eventsMoreButton.Size = new System.Drawing.Size(28, 28);
            this.eventsMoreButton.Text = "toolStripButton2";
            this.eventsMoreButton.ToolTipText = "بازیابی رکورد های بیشتر";
            this.eventsMoreButton.Click += new System.EventHandler(this.MoreRecordsButton_Click);
            // 
            // iccEventSearchButton
            // 
            this.iccEventSearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.iccEventSearchButton.Image = ((System.Drawing.Image)(resources.GetObject("iccEventSearchButton.Image")));
            this.iccEventSearchButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.iccEventSearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.iccEventSearchButton.Name = "iccEventSearchButton";
            this.iccEventSearchButton.Size = new System.Drawing.Size(28, 28);
            this.iccEventSearchButton.Text = "جستجو";
            this.iccEventSearchButton.Click += new System.EventHandler(this.LogSearchShowHideButton_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "adapter.png");
            this.imageList.Images.SetKeyName(1, "1331643511_setting.png");
            this.imageList.Images.SetKeyName(2, "transfers.png");
            this.imageList.Images.SetKeyName(3, "events (5).png");
            // 
            // bottomToolStrip
            // 
            this.bottomToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomToolStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.bottomToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.closeApplicationButton,
            this.toolStripSeparator2,
            this.restartApplicationButton,
            this.toolStripSeparator3,
            this.stopStartApplicationButton,
            this.toolStripSeparator4,
            this.minimizeApplicationButton,
            this.toolStripSeparator5});
            this.bottomToolStrip.Location = new System.Drawing.Point(0, 606);
            this.bottomToolStrip.Name = "bottomToolStrip";
            this.bottomToolStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.bottomToolStrip.Size = new System.Drawing.Size(1094, 39);
            this.bottomToolStrip.TabIndex = 16;
            this.bottomToolStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // closeApplicationButton
            // 
            this.closeApplicationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeApplicationButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.close;
            this.closeApplicationButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeApplicationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeApplicationButton.Name = "closeApplicationButton";
            this.closeApplicationButton.Size = new System.Drawing.Size(36, 36);
            this.closeApplicationButton.Text = "toolStripButton1";
            this.closeApplicationButton.ToolTipText = "خروج از برنامه";
            this.closeApplicationButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // restartApplicationButton
            // 
            this.restartApplicationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restartApplicationButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.restart;
            this.restartApplicationButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restartApplicationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restartApplicationButton.Name = "restartApplicationButton";
            this.restartApplicationButton.Size = new System.Drawing.Size(36, 36);
            this.restartApplicationButton.Text = "toolStripButton2";
            this.restartApplicationButton.ToolTipText = "توقف و اجرای مجدد برنامه";
            this.restartApplicationButton.Click += new System.EventHandler(this.RestartButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // stopStartApplicationButton
            // 
            this.stopStartApplicationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopStartApplicationButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.start;
            this.stopStartApplicationButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.stopStartApplicationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopStartApplicationButton.Name = "stopStartApplicationButton";
            this.stopStartApplicationButton.Size = new System.Drawing.Size(36, 36);
            this.stopStartApplicationButton.Text = "toolStripButton3";
            this.stopStartApplicationButton.ToolTipText = "اجرای برنامه";
            this.stopStartApplicationButton.Click += new System.EventHandler(this.StopStartButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // minimizeApplicationButton
            // 
            this.minimizeApplicationButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.minimizeApplicationButton.Image = global::IRISA.CommunicationCenter.Properties.Resources.minimize;
            this.minimizeApplicationButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.minimizeApplicationButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.minimizeApplicationButton.Name = "minimizeApplicationButton";
            this.minimizeApplicationButton.Size = new System.Drawing.Size(36, 36);
            this.minimizeApplicationButton.Text = "toolStripButton4";
            this.minimizeApplicationButton.ToolTipText = "پنهان سازی برنامه";
            this.minimizeApplicationButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 39);
            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 645);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.bottomToolStrip);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.mainTabPage.PerformLayout();
            this.adaptersSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.adaptersSplitContainer)).EndInit();
            this.adaptersSplitContainer.ResumeLayout(false);
            this.adaptersToolStrip.ResumeLayout(false);
            this.adaptersToolStrip.PerformLayout();
            this.settingsTabPage.ResumeLayout(false);
            this.settingsTabPage.PerformLayout();
            this.settingsSplitContainer.Panel1.ResumeLayout(false);
            this.settingsSplitContainer.Panel1.PerformLayout();
            this.settingsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.settingsSplitContainer)).EndInit();
            this.settingsSplitContainer.ResumeLayout(false);
            this.settingsToolStrip.ResumeLayout(false);
            this.settingsToolStrip.PerformLayout();
            this.TransfersTabPage.ResumeLayout(false);
            this.TransfersTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transfersDataGrid)).EndInit();
            this.telegramSearchGroupbox.ResumeLayout(false);
            this.telegramSearchGroupbox.PerformLayout();
            this.searchFlowLayoutPanel.ResumeLayout(false);
            this.searchFlowLayoutPanel.PerformLayout();
            this.searchToolStrip.ResumeLayout(false);
            this.searchToolStrip.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.flowLayoutPanel10.ResumeLayout(false);
            this.flowLayoutPanel10.PerformLayout();
            this.flowLayoutPanel11.ResumeLayout(false);
            this.flowLayoutPanel11.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel12.ResumeLayout(false);
            this.flowLayoutPanel12.PerformLayout();
            this.flowLayoutPanel13.ResumeLayout(false);
            this.flowLayoutPanel13.PerformLayout();
            this.transfersToolStrip.ResumeLayout(false);
            this.transfersToolStrip.PerformLayout();
            this.eventsTabPage.ResumeLayout(false);
            this.eventsTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGrid)).EndInit();
            this.LogsSearchGroupBox.ResumeLayout(false);
            this.LogsSearchGroupBox.PerformLayout();
            this.eventsSearchflowLayout.ResumeLayout(false);
            this.eventsSearchflowLayout.PerformLayout();
            this.ClearFiltersToolStrip.ResumeLayout(false);
            this.ClearFiltersToolStrip.PerformLayout();
            this.flowLayoutPanel22.ResumeLayout(false);
            this.flowLayoutPanel22.PerformLayout();
            this.flowLayoutPanel30.ResumeLayout(false);
            this.flowLayoutPanel30.PerformLayout();
            this.flowLayoutPanel19.ResumeLayout(false);
            this.flowLayoutPanel19.PerformLayout();
            this.flowLayoutPanel14.ResumeLayout(false);
            this.flowLayoutPanel14.PerformLayout();
            this.flowLayoutPanel27.ResumeLayout(false);
            this.flowLayoutPanel28.ResumeLayout(false);
            this.flowLayoutPanel28.PerformLayout();
            this.flowLayoutPanel29.ResumeLayout(false);
            this.flowLayoutPanel29.PerformLayout();
            this.eventsToolStrip.ResumeLayout(false);
            this.eventsToolStrip.PerformLayout();
            this.bottomToolStrip.ResumeLayout(false);
            this.bottomToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private Button DoTelegramSearch;
        private ToolStripButton iccEventSearchButton;
        private GroupBox LogsSearchGroupBox;
        private FlowLayoutPanel eventsSearchflowLayout;
        private ToolStrip ClearFiltersToolStrip;
        private ToolStripButton clearSearchEventsPanel;
        private Splitter splitter12;
        private FlowLayoutPanel flowLayoutPanel15;
        private FlowLayoutPanel flowLayoutPanel16;
        private FlowLayoutPanel flowLayoutPanel17;
        private FlowLayoutPanel flowLayoutPanel18;
        private FlowLayoutPanel flowLayoutPanel19;
        private FlowLayoutPanel flowLayoutPanel20;
        private MaskedTextBox SearchKeywordTextBox;
        private Splitter splitter17;
        private FlowLayoutPanel flowLayoutPanel21;
        private FlowLayoutPanel flowLayoutPanel23;
        private FlowLayoutPanel flowLayoutPanel24;
        private FlowLayoutPanel flowLayoutPanel25;
        private Splitter splitter21;
        private FlowLayoutPanel flowLayoutPanel26;
        private Button DoIccEventSearch;
        private FlowLayoutPanel flowLayoutPanel14;
        private Label label34;
        private FlowLayoutPanel flowLayoutPanel27;
        private ComboBox LogLevelComboBox;
        private DataGridViewTextBoxColumn transfersIdColumn;
        private DataGridViewTextBoxColumn sourceColumn;
        private DataGridViewTextBoxColumn DestinationColumn;
        private DataGridViewTextBoxColumn TelegramIdColumn;
        private DataGridViewTextBoxColumn BodyColumn;
        private DataGridViewTextBoxColumn SendTimeColumn;
        private DataGridViewTextBoxColumn ReceiveTimeColumn;
        private DataGridViewTextBoxColumn SentColumn;
        private DataGridViewTextBoxColumn DroppedColumn;
        private DataGridViewTextBoxColumn DropReasonColumn;
        private FlowLayoutPanel flowLayoutPanel28;
        private Label label18;
        private FlowLayoutPanel flowLayoutPanel29;
        private Label LogsResultsCountLabel;
        private Label label21;
        private Label LogsPageSizeLabel;
        private Splitter splitter13;
        private FlowLayoutPanel flowLayoutPanel22;
        private Label label19;
        private FlowLayoutPanel flowLayoutPanel30;
        private DataGridViewTextBoxColumn IdColumn;
        private DataGridViewTextBoxColumn TimeColumn;
        private DataGridViewTextBoxColumn LogLevelColumn;
        private DataGridViewTextBoxColumn TextColumn;
    }
}
