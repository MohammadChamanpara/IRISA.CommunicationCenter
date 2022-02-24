using IRISA.CommunicationCenter.Components;
using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Properties;
using IRISA.CommunicationCenter.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public partial class MainForm : Form
    {
        private const string ApplicationPassword = "iccAdmin";

        #region Properties
        private IccCore iccCore;
        private UiSettings uiSettings;
        private bool refreshingRecordsEnabled = false;
        public ILogger Logger { get; set; }

        public MainForm(ILogger logger)
        {
            Logger = logger;

            InitializeComponent();
        }
        #endregion

        private void StartApplication()
        {
            try
            {
                InitialIccCore();
                InitialUiSettings();
                iccCore.Start();
                LoadAdapters();
                LoadSettings();
                LoadLogLevelComboBox();
                transfersDataGrid.Click += DataGrid_Click;
                eventsDataGrid.Click += DataGrid_Click;
                stopStartApplicationButton.ToolTipText = "متوقف نمودن برنامه";
                stopStartApplicationButton.Image = Resources.stop;
                settingsPropertyGrid.Enabled = false;
                Text = uiSettings.ProgramTitle;
                notifyIcon.Text = uiSettings.ProgramTitle;
                Application.DoEvents();
                if (!iccCore.Started)
                {
                    StopApplication();
                }
                StartRefreshingRecords();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام راه اندازی برنامه.");
            }
            finally
            {
                if (!iccCore.Started)
                    StopApplication();
            }
        }

        private void StopApplication()
        {
            try
            {
                iccCore?.Stop();
                stopStartApplicationButton.Image = Resources.start;
                stopStartApplicationButton.ToolTipText = "اجرای برنامه";
                settingsPropertyGrid.Enabled = true;
                Application.DoEvents();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام متوقف سازی برنامه.");
            }
        }

        private void RestartApplication()
        {
            StopApplication();
            StartApplication();
        }

        private void DataGrid_Click(object sender, EventArgs e)
        {
            StopRefreshingRecords();
        }

        private void StartRefreshingRecords()
        {
            if (!refreshingRecordsEnabled)
                _ = KeepRefreshingRecords();
        }

        private async Task KeepRefreshingRecords()
        {
            SetRefreshStatus(true);
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds < 20 && refreshingRecordsEnabled == true)
            {
                LoadRecords();
                await Task.Delay(1000);
            }
            StopRefreshingRecords();
        }

        private void StopRefreshingRecords()
        {
            SetRefreshStatus(false);
        }

        private void SetRefreshStatus(bool newStatus)
        {
            refreshingRecordsEnabled = newStatus;
            if (refreshingRecordsEnabled)
            {
                transfersRefreshButton.Image = Resources.refresh;
                eventsRefreshButton.Image = Resources.refresh;
            }
            else
            {
                transfersRefreshButton.Image = Resources.refresh_disable;
                eventsRefreshButton.Image = Resources.refresh_disable;
            }
            Application.DoEvents();
        }



        private void InitialIccCore()
        {
            iccCore = new IccCore(new InProcessTelegrams(), new LoggerInMemory(), new IccQueueInMemory());
        }

        private void InitialUiSettings()
        {
            uiSettings = new UiSettings();
        }

        private void LoadLogLevelComboBox()
        {
            try
            {
                var logLevels = new List<string>();
                foreach (int logLevel in Enum.GetValues(typeof(LogLevel)))
                {
                    logLevels.Add(((LogLevel)logLevel).ToPersian());
                }

                LogLevelComboBox.DataSource = logLevels;
                LogLevelComboBox.SelectedIndex = -1;
            }
            catch (Exception exception)
            {
                iccCore.Logger.LogException(exception, "بروز خطا هنگام بارگذاری انواع رویداد");
            }
        }

        private void CopySearchControlsToSearchModel(IccTelegramSearchModel searchModel)
        {
            if (long.TryParse(idTextBox.Text, out long transferId))
                searchModel.TransferId = transferId;

            if (int.TryParse(telegramIdTextBox.Text, out int telegramId))
                searchModel.TelegramId = telegramId;

            searchModel.Source = sourceTextBox.Text;
            searchModel.Destination = destinationTextbox.Text;

            /*.................... Send Time....................*/
            if (!sendDateTextBox.Text.Contains(' ') && sendDateTextBox.Text.Length == 10)
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                int persianYear = int.Parse(sendDateTextBox.Text.Substring(0, 4));
                int persianMonth = int.Parse(sendDateTextBox.Text.Substring(5, 2));
                int persianDay = int.Parse(sendDateTextBox.Text.Substring(8, 2));
                int hour = sendHourTextBox.Text.HasValue() ? int.Parse(sendHourTextBox.Text) : 0;
                int minute = sendMinuteTextBox.Text.HasValue() ? int.Parse(sendMinuteTextBox.Text) : 0;
                int second = sendSecondTextBox.Text.HasValue() ? int.Parse(sendSecondTextBox.Text) : 0;

                searchModel.SendTime = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, hour, minute, second, 0, 0);
            }

            /*.................... Receive Time....................*/
            if (!receiveDateTextBox.Text.Contains(' ') && receiveDateTextBox.Text.Length == 10)
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                int persianYear = int.Parse(receiveDateTextBox.Text.Substring(0, 4));
                int persianMonth = int.Parse(receiveDateTextBox.Text.Substring(5, 2));
                int persianDay = int.Parse(receiveDateTextBox.Text.Substring(8, 2));
                int hour = receiveHourTextBox.Text.HasValue() ? int.Parse(receiveHourTextBox.Text) : 0;
                int minute = receiveMinuteTextBox.Text.HasValue() ? int.Parse(receiveMinuteTextBox.Text) : 0;
                int second = receiveSecondTextBox.Text.HasValue() ? int.Parse(receiveSecondTextBox.Text) : 0;

                searchModel.ReceiveTime = persianCalendar.ToDateTime(persianYear, persianMonth, persianDay, hour, minute, second, 0, 0);
            }

            searchModel.DropReason = dropReasonTextBox.Text;

            if (sentCheckBox.CheckState != CheckState.Indeterminate)
                searchModel.Sent = sentCheckBox.Checked;

            if (droppedCheckBox.CheckState != CheckState.Indeterminate)
                searchModel.Dropped = droppedCheckBox.Checked;
        }

        private void CopyEventSearchControlsToSearchModel(LogSearchModel searchModel)
        {
            if (LogTimeTextBox.Text.HasValue())
                searchModel.PersianTime = LogTimeTextBox.Text;

            if (LogLevelComboBox.SelectedIndex > -1)
                searchModel.LogLevel = (LogLevel)LogLevelComboBox.SelectedIndex;
        }

        private void LoadRecords()
        {
            LoadEvents();
            LoadTransfers();
        }

        private void LoadMoreRecords()
        {
            LoadMoreEvents();
            LoadMoreTransfers();
            StopRefreshingRecords();
        }

        private void LoadTransfers()
        {
            LoadTransfers(uiSettings.RecordsLoadCount);
        }

        private void LoadTransfers(int pageSize)
        {
            try
            {
                if (!Visible)
                    return;

                if (GetSelectedTab() != TransfersTabPage)
                    return;

                if (!iccCore.IccQueue.Connected)
                    return;

                IccTelegramSearchModel searchModel = new IccTelegramSearchModel();

                if (telegramSearchGroupbox.Visible)
                    Invoke(new Action(() => { CopySearchControlsToSearchModel(searchModel); }));

                var telegrams = iccCore.IccQueue.GetTelegrams(searchModel, pageSize, out int resultsCount);

                pageSize = Math.Min(pageSize, resultsCount);

                SortableBindingList<IccTelegram> source = new SortableBindingList<IccTelegram>(telegrams);

                base.Invoke(new Action(() =>
                {
                    transfersDataGrid.DataSource = source;
                    TransfersResultsCountLabel.Text = GroupDigits(resultsCount);
                    TransfersPageSizeLabel.Text = GroupDigits(pageSize);
                }));

            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام نمایش تلگرام ها");
            }
        }

        private void LoadMoreTransfers()
        {
            LoadTransfers(transfersDataGrid.Rows.Count + uiSettings.RecordsIncrementCount);
        }

        private void LoadEvents()
        {
            LoadEvents(uiSettings.RecordsLoadCount);
        }

        private void LoadEvents(int pageSize)
        {
            try
            {
                if (!base.Visible)
                    return;

                if (GetSelectedTab() != eventsTabPage)
                    return;

                LogSearchModel searchModel = new LogSearchModel();

                if (LogsSearchGroupBox.Visible)
                    Invoke(new Action(() => { CopyEventSearchControlsToSearchModel(searchModel); }));

                var query = Logger.GetLogs(searchModel, pageSize, out int resultsCount);

                pageSize = Math.Min(pageSize, resultsCount);

                SortableBindingList<LogEvent> source = new SortableBindingList<LogEvent>(query);
                base.Invoke(new Action(() =>
                {
                    eventsDataGrid.DataSource = source;
                    LogsResultsCountLabel.Text = GroupDigits(resultsCount);
                    LogsPageSizeLabel.Text = GroupDigits(pageSize);
                }));
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام نمایش رویداد ها");
            }
        }

        private void LoadMoreEvents()
        {
            LoadEvents(eventsDataGrid.Rows.Count + uiSettings.RecordsIncrementCount);
        }

        private void LoadSettings()
        {
            try
            {
                List<Control> settingControls = new List<Control>
                {
                    new RadioButton
                    {
                        Text = iccCore.PersianDescription,
                        Tag = iccCore
                    },
                    new RadioButton
                    {
                        Text = uiSettings.UiInterfacePersianDescription,
                        Tag = uiSettings
                    },
                    new RadioButton
                    {
                        Text = "صف تلگرام ها",
                        Tag = iccCore.IccQueue
                    }
                };
                if (iccCore.connectedAdapters != null)
                {
                    foreach (IIccAdapter adapter in iccCore.connectedAdapters)
                    {
                        settingControls.Add(new RadioButton
                        {
                            Text = adapter.Name + " - " + adapter.PersianDescription,
                            Tag = adapter
                        });
                    }
                }
                settingsPanel.Controls.Clear();
                foreach (Control settingControl in settingControls)
                {
                    settingControl.Cursor = Cursors.Hand;
                    settingControl.AutoSize = true;
                    settingControl.Click += new EventHandler(SettingControl_Click);
                    settingsPanel.Controls.Add(settingControl);
                }
                if (settingControls.Count > 0)
                {
                    RadioButton radioButton = settingControls.First<Control>() as RadioButton;
                    radioButton.Checked = true;
                    SettingControl_Click(radioButton, null);
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام لود تنظیمات.");
            }
        }

        private void LoadAdapters()
        {
            try
            {
                adaptersPanel.Controls.Clear();
                if (iccCore.connectedAdapters != null)
                {
                    foreach (IIccAdapter adapter in iccCore.connectedAdapters)
                    {
                        AdapterUserControl adapterUserControl = new AdapterUserControl(adapter, Logger);
                        adaptersPanel.Controls.Add(adapterUserControl);
                        adapter.ConnectionChanged += Adapter_ConnectionChanged;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception, "بروز خطا هنگام بارگذاری آداپتور ها.");
            }
        }

        private TabPage GetSelectedTab()
        {
            TabPage selectedTab = null;
            base.Invoke(new Action(() =>
            {
                selectedTab = tabControl.SelectedTab;
            }));
            return selectedTab;
        }

        private bool CheckPassword()
        {
            if (ShowPasswordDialog(uiSettings.ProgramTitle) != ApplicationPassword)
            {
                MessageForm.ShowErrorMessage("کلمه عبور صحیح نمی باشد");
                return false;
            }
            return true;
        }

        private void ClearControls(Control control)
        {
            if (control is CheckBox)
            {
                var checkBox = (control as CheckBox);
                if (checkBox.ThreeState == true)
                    checkBox.CheckState = CheckState.Indeterminate;
                else
                    checkBox.Checked = false;
            }
            else if (control is TextBox)
                (control as TextBox).Clear();
            else if (control is MaskedTextBox)
                (control as MaskedTextBox).Clear();
            else if (control is ComboBox)
                (control as ComboBox).SelectedIndex = -1;
            else
                foreach (Control childControl in control.Controls)
                    ClearControls(childControl);
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            StartApplication();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopApplication();
            Logger.LogWarning("اجرای برنامه خاتمه یافت");
        }
        private void SettingControl_Click(object sender, EventArgs e)
        {
            settingsPropertyGrid.SelectedObject = (sender as Control).Tag;
        }
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            StartRefreshingRecords();
        }
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!base.Visible)
            {
                if (CheckPassword())
                {
                    base.Show();
                }
            }
        }
        private void RefreshRecordsButton_Click(object sender, EventArgs e)
        {
            if (refreshingRecordsEnabled)
                StopRefreshingRecords();
            else
                StartRefreshingRecords();
        }
        private void MoreRecordsButton_Click(object sender, EventArgs e)
        {
            LoadMoreRecords();
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }
        private void RestartButton_Click(object sender, EventArgs e)
        {
            RestartApplication();
        }
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            base.Hide();
        }
        private void StopStartButton_Click(object sender, EventArgs e)
        {
            if (iccCore.Started)
            {
                StopApplication();
            }
            else
            {
                StartApplication();
            }
        }
        private void Adapter_ConnectionChanged(object sender, AdapterConnectionChangedEventArgs e)
        {
            if (uiSettings.NotifyIconShowAdapterConnected)
            {
                string tipText;
                if (uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("کلاینت {0} {1} شد", e.Adapter.PersianDescription, e.Adapter.Connected ? "متصل" : "متوقف");
                }
                else
                {
                    tipText = string.Format("{0} Client {1}.", e.Adapter.Name, e.Adapter.Connected ? "Connected" : "Disconnected");
                }
                notifyIcon.ShowBalloonTip(uiSettings.NotifyIconShowTime, uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void TelegramDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (transfersDataGrid.RowCount != 0)
                {
                    IccTelegram iccTelegram = transfersDataGrid.Rows[transfersDataGrid.SelectedCells[0].RowIndex].DataBoundItem as IccTelegram;
                    new TelegramViewerForm(iccTelegram).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageForm.ShowErrorMessage(ex.Message);
            }
        }

        private void ClearSearchButton_Click(object sender, EventArgs e)
        {
            ClearControls(searchFlowLayoutPanel);
            StartRefreshingRecords();
        }
        private void MaskedTextBox_Click(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox)
            {
                (sender as MaskedTextBox).SelectAll();
            }
        }
        private void SearchTelegramButton_Click(object sender, EventArgs e)
        {
            telegramSearchGroupbox.Visible = !telegramSearchGroupbox.Visible;
            if (!telegramSearchGroupbox.Visible)
            {
                ClearSearchButton_Click(null, null);
            }
        }

        private void DoTelegramSearch_Click(object sender, EventArgs e)
        {
            LoadTransfers();
        }

        private void IccEventSearchButton_Click(object sender, EventArgs e)
        {
            LogsSearchGroupBox.Visible = !LogsSearchGroupBox.Visible;
            if (!LogsSearchGroupBox.Visible)
            {
                ClearSearchButton_Click(null, null);
            }
        }

        private void DoIccEventSearch_Click(object sender, EventArgs e)
        {
            LoadEvents();
        }

        private void ClearSearchEventsPanel_Click(object sender, EventArgs e)
        {
            ClearControls(eventsSearchflowLayout);
            StartRefreshingRecords();
        }

        public static string ShowPasswordDialog(string caption)
        {
            PasswordDialogForm passwordDialogForm = new PasswordDialogForm
            {
                Text = caption
            };
            passwordDialogForm.ShowDialog();
            return passwordDialogForm.passwordTextBox.Text;
        }

        private void SearchControl_Enter(object sender, EventArgs e)
        {
            StartRefreshingRecords();
        }

        public string GroupDigits(int number)
        {
            string text = number.ToString();
            if (text.Length > 3)
            {
                text = text.Insert(text.Length - 3, ",");
            }
            if (text.Length > 7)
            {
                text = text.Insert(text.Length - 7, ",");
            }
            if (text.Length > 11)
            {
                text = text.Insert(text.Length - 11, ",");
            }
            return text;
        }
    }
}
