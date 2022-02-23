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
        #region Properties
        private IccCore iccCore;
        private UiSettings uiSettings;
        private bool refreshingRecordsEnabled = false;
        public IccEventSearchModel IccEventSearchModel { get; set; }
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }
        private void StartApplication()
        {
            InitialIccCore();
            InitialUiSettings();
            iccCore.Start();
            LoadAdapters();
            LoadSettings();
            LoadStatusTypeComboBox();
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
            if (IccEventSearchModel == null)
            {
                IccEventSearchModel = new IccEventSearchModel();
            }
            if (iccEventSearchModelBindingSource == null)
            {
                iccEventSearchModelBindingSource = new BindingSource();
            }
            iccEventSearchModelBindingSource.DataSource = typeof(IccEventSearchModel);
            iccEventSearchModelBindingSource.Add(IccEventSearchModel);
            StartRefreshingRecords();
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

        private void StopApplication()
        {
            iccCore.Stop();
            stopStartApplicationButton.Image = Resources.start;
            stopStartApplicationButton.ToolTipText = "اجرای برنامه";
            settingsPropertyGrid.Enabled = true;
            Application.DoEvents();
        }
        private void RestartApplication()
        {
            StopApplication();
            StartApplication();
        }
        private void InitialIccCore()
        {
            iccCore = new IccCore(new InProcessTelegrams(), new LoggerInMemory(), new IccQueueInMemory());
            iccCore.TelegramDropped += new IccCore.IccCoreTelegramEventHandler(IccCore_TelegramDropped);
            iccCore.TelegramQueued += new IccCore.IccCoreTelegramEventHandler(IccCore_TelegramQueued);
            iccCore.TelegramSent += new IccCore.IccCoreTelegramEventHandler(IccCore_TelegramSent);
        }
        private void InitialUiSettings()
        {
            uiSettings = new UiSettings();
        }
        private void LoadTransfers()
        {
            LoadTransfers(uiSettings.RecordsLoadCount);
        }
        private void LoadStatusTypeComboBox()
        {
            try
            {
                var eventStatuses = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("", "همه")
                };
                foreach (int value in Enum.GetValues(typeof(IccEventStatus)))
                    eventStatuses.Add(new KeyValuePair<string, string>(GetEnumDescription((IccEventStatus)value), GetEnumDescription((IccEventStatus)value)));
                statusTypeComboBox.DataSource = eventStatuses;
                statusTypeComboBox.ValueMember = "Key";
                statusTypeComboBox.DisplayMember = "Value";

                statusTypeComboBox.SelectedIndex = -1;
            }
            catch (Exception exception)
            {
                iccCore.Logger.LogException(exception, "بروز خطا هنگام بارگذاری انواع رویداد");
            }
        }

        public string GetEnumDescription(Enum enumValue)
        {
            System.Reflection.FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes
                (typeof(DescriptionAttribute), false);

            return (attributes.Length > 0)
                ? attributes[0].Description
                : enumValue.ToString();
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
                }));

                base.Invoke(new Action(() =>
                {
                    resultsCountLabel.Text = GroupDigits(resultsCount);
                }));
                base.Invoke(new Action(() =>
                {
                    pageSizeLabel.Text = GroupDigits(pageSize);
                }));
            }
            catch (Exception exception)
            {
                iccCore.Logger.LogException(exception, "بروز خطا هنگام نمایش تلگرام ها");
            }
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
        private void LoadMoreTransfers()
        {
            LoadTransfers(transfersDataGrid.Rows.Count + uiSettings.RecordsIncrementCount);
        }
        private void LoadEvents()
        {
            LoadEvents(uiSettings.RecordsLoadCount);
        }
        private void LoadEvents(int recordCount)
        {
            try
            {
                if (base.Visible)
                {
                    if (GetSelectedTab() == eventsTabPage)
                    {
                        var query = iccCore.Logger.GetLogs();

                        if (!string.IsNullOrEmpty(IccEventSearchModel.EventDateFrom))
                        {
                            var date = IccEventSearchModel.EventDateFrom.ToEnglishDate();
                            query = query.Where(x => x.Time >= date);
                        }
                        if (IccEventSearchModel.HourFrom.HasValue)
                        {
                            if (!(IccEventSearchModel.HourFrom >= 0 && IccEventSearchModel.HourFrom <= 23))
                            {
                                string error = "ساعت باید بین 0 تا 23 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Hour >= IccEventSearchModel.HourFrom);
                        }
                        if (IccEventSearchModel.MinuteFrom.HasValue)
                        {
                            if (!(IccEventSearchModel.MinuteFrom >= 0 && IccEventSearchModel.MinuteFrom <= 59))
                            {
                                string error = "دقیقه باید بین 0 تا 59 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Minute >= IccEventSearchModel.MinuteFrom);
                        }
                        if (IccEventSearchModel.SecondFrom.HasValue)
                        {
                            if (!(IccEventSearchModel.SecondFrom >= 0 && IccEventSearchModel.SecondFrom <= 59))
                            {
                                string error = "ثانیه باید بین 0 تا 59 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Second >= IccEventSearchModel.SecondFrom);
                        }
                        if (!string.IsNullOrEmpty(IccEventSearchModel.EventDateTo))
                        {
                            var date = IccEventSearchModel.EventDateTo.ToEnglishDate();
                            query = query.Where(x => x.Time <= date);
                        }
                        if (IccEventSearchModel.HourTo.HasValue)
                        {
                            if (!(IccEventSearchModel.HourTo >= 0 && IccEventSearchModel.HourTo <= 23))
                            {
                                string error = "ساعت باید بین 0 تا 23 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Hour <= IccEventSearchModel.HourTo);
                        }
                        if (IccEventSearchModel.MinuteTo.HasValue)
                        {
                            if (!(IccEventSearchModel.MinuteTo >= 0 && IccEventSearchModel.MinuteTo <= 59))
                            {
                                string error = "دقیقه باید بین 0 تا 59 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Minute <= IccEventSearchModel.MinuteTo);
                        }
                        if (IccEventSearchModel.SecondTo.HasValue)
                        {
                            if (!(IccEventSearchModel.SecondTo >= 0 && IccEventSearchModel.SecondTo <= 59))
                            {
                                string error = "ثانیه باید بین 0 تا 59 باشد";
                                throw new Exception(error);
                            }
                            query = query.Where(x => x.Time.Second <= IccEventSearchModel.SecondTo);
                        }
                        if (!string.IsNullOrEmpty(IccEventSearchModel.Type))
                        {
                            query = query.Where(x => x.PersianType.Contains(IccEventSearchModel.Type));
                        }

                        query = query.OrderByDescending(x => x.Id).Take(recordCount);
                        SortableBindingList<LogEvent> source = new SortableBindingList<LogEvent>(query);
                        base.Invoke(new Action(() =>
                        {
                            eventsDataGrid.DataSource = source;
                        }));
                    }
                }
            }
            catch (Exception exception)
            {
                iccCore.Logger.LogException(exception, "بروز خطا هنگام نمایش رویداد ها");
            }
        }
        private void LoadMoreEvents()
        {
            LoadEvents(eventsDataGrid.Rows.Count + uiSettings.RecordsIncrementCount);
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
                    foreach (IIccAdapter current in iccCore.connectedAdapters)
                    {
                        settingControls.Add(new RadioButton
                        {
                            Text = current.Name + " - " + current.PersianDescription,
                            Tag = current
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
                iccCore.Logger.LogException(exception, "بروز خطا هنگام لود تنظیمات.");
            }
        }
        private void LoadAdapters()
        {
            try
            {
                adaptersPanel.Controls.Clear();
                if (iccCore.connectedAdapters != null)
                {
                    foreach (IIccAdapter current in iccCore.connectedAdapters)
                    {
                        AdapterUserControl value = new AdapterUserControl(current, iccCore.Logger);
                        adaptersPanel.Controls.Add(value);
                        current.ConnectionChanged += Adapter_ConnectionChanged;
                    }
                }
            }
            catch (Exception exception)
            {
                iccCore.Logger.LogException(exception, "بروز خطا هنگام بارگذاری آداپتور ها.");
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
            string a = ShowPasswordDialog(uiSettings.ProgramTitle);
            bool result;
            if (a != "iccAdmin")
            {
                MessageForm.ShowErrorMessage("کلمه عبور صحیح نمی باشد", new object[0]);
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
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
        }
        private void ClearControls(Control control)
        {
            if (control is CheckBox)
            {
                CheckBox checkBox = control as CheckBox;
                if (checkBox.ThreeState)
                {
                    checkBox.CheckState = CheckState.Indeterminate;
                }
                else
                {
                    checkBox.Checked = false;
                }
            }
            else
            {
                if (control is TextBox)
                {
                    (control as TextBox).Clear();
                }
                else
                {
                    if (control is MaskedTextBox)
                    {
                        (control as MaskedTextBox).Clear();
                    }
                    else
                    {
                        foreach (Control control2 in control.Controls)
                        {
                            ClearControls(control2);
                        }
                    }
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            StartApplication();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopApplication();
            iccCore.Logger.LogWarning("اجرای برنامه خاتمه یافت", new object[0]);
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
        private void IccCore_TelegramSent(IccCoreTelegramEventArgs e)
        {
            if (uiSettings.NotifyIconShowSent)
            {
                string tipText;
                if (uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("تلگرام از {0} به {1} موفقیت امیز ارسال شد.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                else
                {
                    tipText = string.Format("Telegram successfuly sent from {0} to {1} .", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                notifyIcon.ShowBalloonTip(uiSettings.NotifyIconShowTime, uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void IccCore_TelegramQueued(IccCoreTelegramEventArgs e)
        {
            if (uiSettings.NotifyIconShowQueued)
            {
                string tipText;
                if (uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("تلگرام ارسالی از {0} به {1} در صف انتظار قرار گرفت.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                else
                {
                    tipText = string.Format("Sending telegram from {0} to {1} Queued.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                notifyIcon.ShowBalloonTip(uiSettings.NotifyIconShowTime, uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void IccCore_TelegramDropped(IccCoreTelegramEventArgs e)
        {
            if (uiSettings.NotifyIconShowDrop)
            {
                string arg = " ";
                string tipText;
                if (uiSettings.NotifyIconPersianLanguage)
                {
                    if (e.IccTelegram.Destination.HasValue())
                    {
                        arg = string.Format(" به {0} ", e.IccTelegram.Destination);
                    }
                    tipText = string.Format("تلگرام ارسالی از {0} {1}حذف شد.", e.IccTelegram.Source, arg);
                }
                else
                {
                    if (e.IccTelegram.Destination.HasValue())
                    {
                        arg = string.Format(" to {0} ", e.IccTelegram.Destination);
                    }
                    tipText = string.Format("Sending Telegram from {0}{1}Dropped.", e.IccTelegram.Source, arg);
                }
                notifyIcon.ShowBalloonTip(uiSettings.NotifyIconShowTime, uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Error);
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
                MessageForm.ShowErrorMessage(ex.Message, new object[0]);
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
            iccEventSearchGroupBox.Visible = !iccEventSearchGroupBox.Visible;
            if (!iccEventSearchGroupBox.Visible)
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
    }
}
