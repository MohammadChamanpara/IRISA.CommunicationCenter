using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Library.Adapters;
using IRISA.CommunicationCenter.Library.Extensions;
using IRISA.CommunicationCenter.Library.Logging;
using IRISA.CommunicationCenter.Library.Models;
using IRISA.CommunicationCenter.Properties;
using IRISA.CommunicationCenter.Settings;
using IRISA.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace IRISA.CommunicationCenter
{
    public partial class MainForm : Form
    {
        #region Properties
        private IccCore iccCore;
        private UiSettings uiSettings;
        private bool refreshRecords = false;

        public IccEventSearchModel IccEventSearchModel { get; set; }
        #endregion

        public MainForm()
        {
            this.InitializeComponent();
        }
        private void StartApplication()
        {
            this.InitialIccCore();
            this.InitialUiSettings();
            this.iccCore.Start();
            this.LoadClients();
            this.LoadSettings();
            this.LoadRecords();
            this.LoadStatusTypeComboBox();
            this.stopStartApplicationButton.ToolTipText = "متوقف نمودن برنامه";
            this.stopStartApplicationButton.Image = Resources.stop;
            this.settingsPropertyGrid.Enabled = false;
            this.Text = this.uiSettings.ProgramTitle;
            this.notifyIcon.Text = this.uiSettings.ProgramTitle;
            Application.DoEvents();
            if (!this.iccCore.Started)
            {
                this.StopApplication();
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
            this.ChangeRefreshStatus(true);
            this.LoadRecords();
        }
        private void StopApplication()
        {
            this.iccCore.Stop();
            this.stopStartApplicationButton.Image = Resources.start;
            this.stopStartApplicationButton.ToolTipText = "اجرای برنامه";
            this.settingsPropertyGrid.Enabled = true;
            Application.DoEvents();
        }
        private void RestartApplication()
        {
            this.StopApplication();
            this.StartApplication();
        }
        private void InitialIccCore()
        {
            this.iccCore = new IccCore(new InProcessTelegrams(), new LoggerInMemory(), new IccQueueInMemory());
            this.iccCore.TelegramDropped += new IccCore.IccCoreTelegramEventHandler(this.IccCore_TelegramDropped);
            this.iccCore.TelegramQueued += new IccCore.IccCoreTelegramEventHandler(this.IccCore_TelegramQueued);
            this.iccCore.TelegramSent += new IccCore.IccCoreTelegramEventHandler(this.IccCore_TelegramSent);
            this.iccCore.Logger.EventLogged += EventLogger_EventLogged;
        }
        private void InitialUiSettings()
        {
            this.uiSettings = new UiSettings();
        }
        private void LoadTransfers()
        {
            this.LoadTransfers(this.uiSettings.RecordsLoadCount);
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
                MessageBox.Show(exception.Message);
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
                if (!this.Visible)
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
                    this.transfersDataGrid.DataSource = source;
                }));

                base.Invoke(new Action(() =>
                {
                    this.resultsCountLabel.Text = GroupDigits(resultsCount);
                }));
                base.Invoke(new Action(() =>
                {
                    this.pageSizeLabel.Text = GroupDigits(pageSize);
                }));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
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
            this.LoadTransfers(this.transfersDataGrid.Rows.Count + this.uiSettings.RecordsIncrementCount);
        }
        private void LoadEvents()
        {
            this.LoadEvents(this.uiSettings.RecordsLoadCount);
        }
        private void LoadEvents(int recordCount)
        {
            try
            {
                if (base.Visible)
                {
                    if (this.GetSelectedTab() == this.eventsTabPage)
                    {
                        var query = this.iccCore.Logger.GetLogs();

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
                            var date = this.IccEventSearchModel.EventDateTo.ToEnglishDate();
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
                            this.eventsDataGrid.DataSource = source;
                        }));
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void LoadMoreEvents()
        {
            this.LoadEvents(this.eventsDataGrid.Rows.Count + this.uiSettings.RecordsIncrementCount);
        }
        private void LoadRecords()
        {
            if (this.refreshRecords)
            {
                this.LoadEvents();
                this.LoadTransfers();
            }
        }
        private void LoadMoreRecords()
        {
            this.LoadMoreEvents();
            this.LoadMoreTransfers();
            this.ChangeRefreshStatus(false);
        }
        private void LoadSettings()
        {
            try
            {
                List<Control> list = new List<Control>
                {
                    new RadioButton
                    {
                        Text = this.iccCore.PersianDescription,
                        Tag = this.iccCore
                    },
                    new RadioButton
                    {
                        Text = this.uiSettings.UiInterfacePersianDescription,
                        Tag = this.uiSettings
                    },
                    new RadioButton
                    {
                        Text = "صف تلگرام ها",
                        Tag = this.iccCore.IccQueue
                    }
                };
                if (this.iccCore.connectedClients != null)
                {
                    foreach (IIccAdapter current in this.iccCore.connectedClients)
                    {
                        list.Add(new RadioButton
                        {
                            Text = current.Name + " - " + current.PersianDescription,
                            Tag = current
                        });
                    }
                }
                this.settingsPanel.Controls.Clear();
                foreach (Control current2 in list)
                {
                    current2.Cursor = Cursors.Hand;
                    current2.AutoSize = true;
                    current2.Click += new EventHandler(this.SettingControl_Click);
                    this.settingsPanel.Controls.Add(current2);
                }
                if (list.Count > 0)
                {
                    RadioButton radioButton = list.First<Control>() as RadioButton;
                    radioButton.Checked = true;
                    this.SettingControl_Click(radioButton, null);
                }
            }
            catch (Exception exception)
            {
                this.iccCore.Logger.LogException(exception);
            }
        }
        private void LoadClients()
        {
            try
            {
                this.clientsPanel.Controls.Clear();
                if (this.iccCore.connectedClients != null)
                {
                    foreach (IIccAdapter current in this.iccCore.connectedClients)
                    {
                        PluginUserControl value = new PluginUserControl(current, this.iccCore.Logger);
                        this.clientsPanel.Controls.Add(value);
                        current.ConnectionChanged += Client_ConnectionChanged;
                    }
                }
            }
            catch (Exception exception)
            {
                this.iccCore.Logger.LogException(exception);
            }
        }

        private TabPage GetSelectedTab()
        {
            TabPage selectedTab = null;
            base.Invoke(new Action(() =>
            {
                selectedTab = this.tabControl.SelectedTab;
            }));
            return selectedTab;
        }

        private bool CheckPassword()
        {
            string a = HelperMethods.ShowPasswordDialog(this.uiSettings.ProgramTitle);
            bool result;
            if (a != "iccAdmin")
            {
                HelperMethods.ShowErrorMessage("کلمه عبور صحیح نمی باشد", new object[0]);
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
        private void ChangeRefreshStatus(bool newStatus)
        {
            this.refreshRecords = newStatus;
            if (this.refreshRecords)
            {
                this.transfersRefreshButton.Image = Resources.refresh;
                this.eventsRefreshButton.Image = Resources.refresh;
            }
            else
            {
                this.transfersRefreshButton.Image = Resources.refresh_disable;
                this.eventsRefreshButton.Image = Resources.refresh_disable;
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
                            this.ClearControls(control2);
                        }
                    }
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.StartApplication();
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.StopApplication();
            this.iccCore.Logger.LogWarning("اجرای برنامه خاتمه یافت", new object[0]);
        }
        private void SettingControl_Click(object sender, EventArgs e)
        {
            this.settingsPropertyGrid.SelectedObject = (sender as Control).Tag;
        }
        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadRecords();
        }
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!base.Visible)
            {
                if (this.CheckPassword())
                {
                    base.Show();
                }
            }
        }
        private void RefreshRecordsButton_Click(object sender, EventArgs e)
        {
            this.ChangeRefreshStatus(!this.refreshRecords);
            if (this.refreshRecords)
            {
                this.LoadRecords();
            }
        }
        private void MoreRecordsButton_Click(object sender, EventArgs e)
        {
            this.LoadMoreRecords();
        }
        private void CloseButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }
        private void RestartButton_Click(object sender, EventArgs e)
        {
            this.RestartApplication();
        }
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            base.Hide();
        }
        private void StopStartButton_Click(object sender, EventArgs e)
        {
            if (this.iccCore.Started)
            {
                this.StopApplication();
            }
            else
            {
                this.StartApplication();
            }
        }
        private void IccCore_TelegramSent(IccCoreTelegramEventArgs e)
        {
            if (this.uiSettings.NotifyIconShowSent)
            {
                string tipText;
                if (this.uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("تلگرام از {0} به {1} موفقیت امیز ارسال شد.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                else
                {
                    tipText = string.Format("Telegram successfuly sent from {0} to {1} .", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                this.notifyIcon.ShowBalloonTip(this.uiSettings.NotifyIconShowTime, this.uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void IccCore_TelegramQueued(IccCoreTelegramEventArgs e)
        {
            if (this.uiSettings.NotifyIconShowQueued)
            {
                string tipText;
                if (this.uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("تلگرام ارسالی از {0} به {1} در صف انتظار قرار گرفت.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                else
                {
                    tipText = string.Format("Sending telegram from {0} to {1} Queued.", e.IccTelegram.Source, e.IccTelegram.Destination);
                }
                this.notifyIcon.ShowBalloonTip(this.uiSettings.NotifyIconShowTime, this.uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void IccCore_TelegramDropped(IccCoreTelegramEventArgs e)
        {
            if (this.uiSettings.NotifyIconShowDrop)
            {
                string arg = " ";
                string tipText;
                if (this.uiSettings.NotifyIconPersianLanguage)
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
                this.notifyIcon.ShowBalloonTip(this.uiSettings.NotifyIconShowTime, this.uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Error);
            }
        }
        private void Client_ConnectionChanged(object sender, AdapterConnectionChangedEventArgs e)
        {
            if (this.uiSettings.NotifyIconShowClientConnected)
            {
                string tipText;
                if (this.uiSettings.NotifyIconPersianLanguage)
                {
                    tipText = string.Format("کلاینت {0} {1} شد", e.Adapter.PersianDescription, e.Adapter.Connected ? "متصل" : "متوقف");
                }
                else
                {
                    tipText = string.Format("{0} Client {1}.", e.Adapter.Name, e.Adapter.Connected ? "Connected" : "Disconnected");
                }
                this.notifyIcon.ShowBalloonTip(this.uiSettings.NotifyIconShowTime, this.uiSettings.NotifyIconTitle, tipText, ToolTipIcon.Info);
            }
        }
        private void EventLogger_EventLogged()
        {
            this.LoadRecords();
        }
        private void TelegramDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.transfersDataGrid.RowCount != 0)
                {
                    IccTelegram iccTelegram = transfersDataGrid.Rows[this.transfersDataGrid.SelectedCells[0].RowIndex].DataBoundItem as IccTelegram;
                    new TelegramViewerForm(iccTelegram).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HelperMethods.ShowErrorMessage(ex.Message, new object[0]);
            }
        }

        private void ClearSearchButton_Click(object sender, EventArgs e)
        {
            this.ClearControls(this.searchFlowLayoutPanel);
            this.ChangeRefreshStatus(true);
            this.LoadTransfers();
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
            this.telegramSearchGroupbox.Visible = !this.telegramSearchGroupbox.Visible;
            if (!this.telegramSearchGroupbox.Visible)
            {
                this.ClearSearchButton_Click(null, null);
            }
        }

        private void DoTelegramSearch_Click(object sender, EventArgs e)
        {
            this.LoadTransfers();
        }

        private void IccEventSearchButton_Click(object sender, EventArgs e)
        {
            this.iccEventSearchGroupBox.Visible = !this.iccEventSearchGroupBox.Visible;
            if (!this.iccEventSearchGroupBox.Visible)
            {
                this.ClearSearchButton_Click(null, null);
            }
        }

        private void DoIccEventSearch_Click(object sender, EventArgs e)
        {
            this.LoadEvents();
        }

        private void ClearSearchEventsPanel_Click(object sender, EventArgs e)
        {
            this.ClearControls(this.eventsSearchflowLayout);
            this.ChangeRefreshStatus(true);
            this.LoadEvents();
        }

        private void StatusTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }













    }
}
