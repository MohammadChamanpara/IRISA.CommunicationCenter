using IRISA.CommunicationCenter.Components;
using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Library.Tasks;
using IRISA.CommunicationCenter.Properties;
using IRISA.CommunicationCenter.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter.Forms
{
    public partial class MainForm : Form
    {
        private const string ApplicationPassword = "iccAdmin";

        private readonly IIccCore _iccCore;
        private UiSettings _uiSettings;
        private readonly ILogger _logger;
        private BackgroundTimer _refreshTimer;

        public MainForm(ILogger logger, IIccCore iccCore)
        {
            InitializeComponent();

            _logger = logger;
            _iccCore = iccCore;
        }

        private void StartApplication()
        {
            try
            {
                InitialUiSettings();
                _iccCore.Start();
                LoadAdapters();
                LoadSettings();
                LoadLogLevelComboBox();
                transfersDataGrid.Click += DataGrid_Click;
                eventsDataGrid.Click += DataGrid_Click;
                stopStartApplicationButton.ToolTipText = "متوقف نمودن برنامه";
                stopStartApplicationButton.Image = Resources.stop;
                settingsPropertyGrid.Enabled = false;
                Text = _uiSettings.ProgramTitle;
                notifyIcon.Text = _uiSettings.ProgramTitle;
                Application.DoEvents();
                if (!_iccCore.Started)
                {
                    StopApplication();
                }
                InitializeRefreshTimer();
                _refreshTimer.Start();
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام راه اندازی برنامه.");
            }
            finally
            {
                if (!_iccCore.Started)
                    StopApplication();
            }
        }

        private void InitializeRefreshTimer()
        {
            _refreshTimer = new BackgroundTimer(_logger)
            {
                Interval = _uiSettings.RecordsRefreshInterval,
                AliveTime = _uiSettings.RecordsRefreshAliveTime,
                PersianDescription = "پروسه نمایش رویداد ها و تلگرام ها در فرم"
            };
            _refreshTimer.DoWork += LoadRecords;
            _refreshTimer.Started += RefreshTimer_Started;
            _refreshTimer.Stopped += RefreshTimer_Stopped;
        }

        private void RefreshTimer_Stopped()
        {
            SetRefreshStatus(false);
        }
        private void RefreshTimer_Started()
        {
            SetRefreshStatus(true);
        }

        private void StopApplication()
        {
            try
            {
                _iccCore?.Stop();
                stopStartApplicationButton.Image = Resources.start;
                stopStartApplicationButton.ToolTipText = "اجرای برنامه";
                settingsPropertyGrid.Enabled = true;
                _refreshTimer?.Stop();
                Application.DoEvents();
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام متوقف سازی برنامه.");
            }
        }

        private void RestartApplication()
        {
            StopApplication();
            StartApplication();
        }

        private void DataGrid_Click(object sender, EventArgs e)
        {
            _refreshTimer.Stop();
        }

        private void SetRefreshStatus(bool refreshingRecordsEnabled)
        {
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

        private void InitialUiSettings()
        {
            _uiSettings = new UiSettings();
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
                _logger.LogException(exception, "بروز خطا هنگام بارگذاری انواع رویداد");
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
            try
            {
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
            }
            catch (Exception exception)
            {
                searchModel.SendTime = DateTime.MinValue;
                _logger.LogException(exception, "بروز خطا هنگام جستجوی رکورد ها");
            }

            /*.................... Receive Time....................*/
            try
            {
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
            }
            catch (Exception exception)
            {
                searchModel.ReceiveTime= DateTime.MinValue;
                _logger.LogException(exception, "بروز خطا هنگام جستجوی رکورد ها");
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
            _refreshTimer.Stop();
        }

        private void LoadTransfers()
        {
            LoadTransfers(_uiSettings.RecordsLoadCount);
        }

        private void LoadTransfers(int pageSize)
        {
            try
            {
                if (!Visible)
                    return;

                if (GetSelectedTab() != TransfersTabPage)
                    return;

                if (!_iccCore.IccQueue.Connected)
                    return;

                IccTelegramSearchModel searchModel = new IccTelegramSearchModel();

                if (telegramSearchGroupbox.Visible)
                    Invoke(new Action(() => { CopySearchControlsToSearchModel(searchModel); }));

                var telegrams = _iccCore.IccQueue.GetTelegrams(searchModel, pageSize, out int resultsCount);

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
                _logger.LogException(exception, "بروز خطا هنگام نمایش تلگرام ها");
            }
        }

        private void LoadMoreTransfers()
        {
            LoadTransfers(transfersDataGrid.Rows.Count + _uiSettings.RecordsIncrementCount);
        }

        private void LoadEvents()
        {
            LoadEvents(_uiSettings.RecordsLoadCount);
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

                var query = _logger.GetLogs(searchModel, pageSize, out int resultsCount);

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
                _logger.LogException(exception, "بروز خطا هنگام نمایش رویداد ها");
            }
        }

        private void LoadMoreEvents()
        {
            LoadEvents(eventsDataGrid.Rows.Count + _uiSettings.RecordsIncrementCount);
        }

        private void LoadSettings()
        {
            try
            {
                List<Control> settingControls = new List<Control>
                {
                    new RadioButton
                    {
                        Text = _iccCore.PersianDescription,
                        Tag = _iccCore
                    },
                    new RadioButton
                    {
                        Text = "واسط کاربری",
                        Tag = _uiSettings
                    },
                    new RadioButton
                    {
                        Text = "صف تلگرام ها",
                        Tag = _iccCore.IccQueue
                    }
                };
                if (_iccCore.ConnectedAdapters != null)
                {
                    foreach (IIccAdapter adapter in _iccCore.ConnectedAdapters)
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
                    RadioButton radioButton = settingControls.First() as RadioButton;
                    radioButton.Checked = true;
                    SettingControl_Click(radioButton, null);
                }
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام لود تنظیمات.");
            }
        }

        private void LoadAdapters()
        {
            try
            {
                adaptersPanel.Controls.Clear();
                if (_iccCore.ConnectedAdapters != null)
                {
                    foreach (IIccAdapter adapter in _iccCore.ConnectedAdapters)
                    {
                        AdapterUserControl adapterUserControl = new AdapterUserControl(adapter, _logger);
                        adaptersPanel.Controls.Add(adapterUserControl);
                        adapter.ConnectionChanged += Adapter_ConnectionChanged;
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogException(exception, "بروز خطا هنگام بارگذاری آداپتور ها.");
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
            if (ShowPasswordDialog(_uiSettings.ProgramTitle) != ApplicationPassword)
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
            _logger.LogWarning("اجرای برنامه خاتمه یافت");
        }
        private void SettingControl_Click(object sender, EventArgs e)
        {

        }
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _refreshTimer.Start();
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
            if (_refreshTimer.IsRunning)
                _refreshTimer.Stop();
            else
                _refreshTimer.Start();
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
            if (_iccCore.Started)
            {
                StopApplication();
            }
            else
            {
                StartApplication();
            }
        }
        private void Adapter_ConnectionChanged(IIccAdapter iccAdapter)
        {
            if (_uiSettings.NotifyIconShowAdapterConnected)
            {
                string tipText;
                if (_uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("کلاینت {0} {1} شد", iccAdapter.PersianDescription, iccAdapter.Connected ? "متصل" : "متوقف");
                }
                else
                {
                    tipText = string.Format("{0} Client {1}.", iccAdapter.Name, iccAdapter.Connected ? "Connected" : "Disconnected");
                }
                notifyIcon.ShowBalloonTip(_uiSettings.NotifyIconShowTime, "Irisa Communication Center", tipText, ToolTipIcon.Info);
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
            telegramSearchGroupbox.Visible = false;
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
            LoadEvents();
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

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            _refreshTimer.Start();
        }
    }
}
