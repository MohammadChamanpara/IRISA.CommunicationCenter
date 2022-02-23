using IRISA.CommunicationCenter.Adapters;
using IRISA.CommunicationCenter.Core;
using IRISA.CommunicationCenter.Properties;
using IRISA.CommunicationCenter.SearchModels;
using IRISA.Loggers;
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
        private const string ApplicationPassword = "iccAdmin";
        private IccCore iccCore;
        private UiSettings uiSettings;
        private bool refreshRecords = false;

        public ICCTransferSearchModel IccTransferSearchModel { get; set; }

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
            this.iccCore.TelegramDropped += new IccCore.IccCoreTelegramEventHandler(this.iccCore_TelegramDropped);
            this.iccCore.TelegramQueued += new IccCore.IccCoreTelegramEventHandler(this.iccCore_TelegramQueued);
            this.iccCore.TelegramSent += new IccCore.IccCoreTelegramEventHandler(this.iccCore_TelegramSent);
            this.iccCore.Logger.EventLogged += eventLogger_EventLogged;
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
                var eventStatuses = new List<KeyValuePair<string, string>>();
                eventStatuses.Add(new KeyValuePair<string, string>("", "همه"));
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


        private DateTime GetLatinDate(string pDate)
        {
            int year = 0;
            int month = 0;
            int day = 0;
            PersianCalendar persianCalendar = new PersianCalendar();
            try
            {
                year = int.Parse(pDate.Substring(0, 4));
                month = int.Parse(pDate.Substring(5, 2));
                day = int.Parse(pDate.Substring(8, 2));
            }
            catch (Exception)
            {
                throw new Exception("تاریخ به اشتباه وارد شده است");
            }
            if (!(year < 1404 && year > 1385))
            {
                string error = "سال باید بین 1385 تا 1404 باشد";
                throw new Exception(error);
            }

            if (!(month >= 1 && month <= 12))
            {
                string error = "ماه باید بین 1 تا 12 باشد";
                throw new Exception(error);
            }

            if (!(day >= 1 && day <= 31))
            {
                string error = "روز باید بین 1 تا 31 باشد";
                throw new Exception(error);
            }

            DateTime searchDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            return searchDate;

        }

        private void LoadTransfers(int showRecordsCount)
        {
            try
            {
                if (base.Visible)
                {
                    if (this.GetSelectedTab() == this.TransfersTabPage)
                    {
                        if (this.iccCore.IccQueue.Connected)
                        {
                            var telegrams = this.iccCore.IccQueue.GetTelegrams(showRecordsCount);
                            //if (this.idTextBox.Text.HasValue())
                            //{
                            //    int id = int.Parse(this.idTextBox.Text);
                            //    queryable =
                            //        from x in queryable
                            //        where x.ID == (long)id
                            //        select x;
                            //}
                            //if (this.telegramIdTextBox.Text.HasValue())
                            //{
                            //    int telegramId = int.Parse(this.telegramIdTextBox.Text);
                            //    queryable =
                            //        from x in queryable
                            //        where x.TELEGRAM_ID == (int?)telegramId
                            //        select x;
                            //}
                            //if (this.sourceTextBox.Text.HasValue())
                            //{
                            //    queryable =
                            //        from x in queryable
                            //        where x.SOURCE.ToLower().Contains(this.sourceTextBox.Text.ToLower())
                            //        select x;
                            //}
                            //if (this.destinationTextbox.Text.HasValue())
                            //{
                            //    queryable =
                            //        from x in queryable
                            //        where x.DESTINATION.ToLower().Contains(this.destinationTextbox.Text.ToLower())
                            //        select x;
                            //}
                            //if (this.sentCheckBox.CheckState == CheckState.Checked)
                            //{
                            //    var sent = this.sentCheckBox.Checked ? true : false;
                            //    queryable =
                            //        from x in queryable
                            //        where x.SENT == sent
                            //        select x;
                            //}
                            //if (this.droppedCheckBox.CheckState == CheckState.Checked)
                            //{
                            //    var dropped = this.droppedCheckBox.Checked ? true : false;
                            //    queryable =
                            //        from x in queryable
                            //        where x.DROPPED == dropped
                            //        select x;
                            //}
                            //if (this.sendHourTextBox.Text.HasValue())
                            //{
                            //    int hour = int.Parse(this.sendHourTextBox.Text);
                            //    if (!(hour >= 0 && hour <= 23))
                            //    {
                            //        string error = "ساعت باید بین 0 تا 23 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.SEND_TIME.Hour == hour
                            //        select x;
                            //}
                            //if (this.sendMinuteTextBox.Text.HasValue())
                            //{
                            //    int minute = int.Parse(this.sendMinuteTextBox.Text);
                            //    if (!(minute >= 0 && minute <= 59))
                            //    {
                            //        string error = "دقیقه باید بین 0 تا 59 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.SEND_TIME.Minute == minute
                            //        select x;
                            //}
                            //if (this.sendSecondTextBox.Text.HasValue())
                            //{
                            //    int second = int.Parse(this.sendSecondTextBox.Text);
                            //    if (!(second >= 0 && second <= 59))
                            //    {
                            //        string error = "ثانیه باید بین 0 تا 59 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.SEND_TIME.Second == second
                            //        select x;
                            //}
                            //if (!this.sendDateTextBox.Text.Contains(' ') && this.sendDateTextBox.Text.Length == 10)
                            //{
                            //    DateTime searchDate = GetLatinDate(this.sendDateTextBox.Text);
                            //    queryable = queryable.Where(x => x.SEND_TIME.Year == searchDate.Year && x.SEND_TIME.Month == searchDate.Month && x.SEND_TIME.Day == searchDate.Day);
                            //}
                            //if (!this.receiveDateTextBox.Text.Contains(' ') && this.receiveDateTextBox.Text.Length == 10)
                            //{
                            //    DateTime searchDate = GetLatinDate(this.receiveDateTextBox.Text);
                            //    queryable = queryable.Where(x => x.RECEIVE_TIME.Value.Year == searchDate.Year && x.RECEIVE_TIME.Value.Month == searchDate.Month &&x.RECEIVE_TIME.Value.Day == searchDate.Day);
                            //}
                            //if (this.receiveHourTextBox.Text.HasValue())
                            //{
                            //    int hour = int.Parse(this.receiveHourTextBox.Text);
                            //    if (!(hour >= 0 && hour <= 23))
                            //    {
                            //        string error = "ساعت باید بین 0 تا 23 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.RECEIVE_TIME.HasValue && x.RECEIVE_TIME.Value.Hour == hour
                            //        select x;
                            //}
                            //if (this.receiveMinuteTextBox.Text.HasValue())
                            //{
                            //    int minute = int.Parse(this.receiveMinuteTextBox.Text);
                            //    if (!(minute >= 0 && minute <= 59))
                            //    {
                            //        string error = "دقیقه باید بین 0 تا 59 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.RECEIVE_TIME.HasValue && x.RECEIVE_TIME.Value.Minute == minute
                            //        select x;
                            //}
                            //if (this.receiveSecondTextBox.Text.HasValue())
                            //{
                            //    int second = int.Parse(this.receiveSecondTextBox.Text);
                            //    if (!(second >= 0 && second <= 59))
                            //    {
                            //        string error = "ثانیه باید بین 0 تا 59 باشد";
                            //        throw new Exception(error);
                            //    }
                            //    queryable =
                            //        from x in queryable
                            //        where x.RECEIVE_TIME.HasValue && x.RECEIVE_TIME.Value.Second == second
                            //        select x;
                            //}
                            //if (this.dropReasonTextBox.Text.HasValue())
                            //{
                            //    queryable =
                            //        from x in queryable
                            //        where x.DROP_REASON.Contains(this.dropReasonTextBox.Text)
                            //        select x;
                            //}

                            int allRecordsCount = iccCore.IccQueue.Count;

                            showRecordsCount = Math.Min(showRecordsCount, allRecordsCount);
                            
                            SortableBindingList<IccTelegram> source = new SortableBindingList<IccTelegram>(telegrams);
                            base.Invoke(new Action(() =>
                            {
                                this.transfersDataGrid.DataSource = source;
                            }));
                            base.Invoke(new Action(() =>
                            {
                                this.allRecordsCountLabel.Text = allRecordsCount.DigitGrouping();
                            }));
                            base.Invoke(new Action(() =>
                            {
                                this.showRecordsCountLabel.Text = showRecordsCount.DigitGrouping();
                            }));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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
                            var date = this.GetLatinDate(IccEventSearchModel.EventDateFrom);
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
                            var date = this.GetLatinDate(IccEventSearchModel.EventDateTo);
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
                List<Control> list = new List<Control>();
                list.Add(new RadioButton
                {
                    Text = this.iccCore.PersianDescription,
                    Tag = this.iccCore
                });
                list.Add(new RadioButton
                {
                    Text = this.uiSettings.UiInterfacePersianDescription,
                    Tag = this.uiSettings
                });
                list.Add(new RadioButton
                {
                    Text = "صف تلگرام ها",
                    Tag = this.iccCore.IccQueue
                });
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
                    current2.Click += new EventHandler(this.settingControl_Click);
                    this.settingsPanel.Controls.Add(current2);
                }
                if (list.Count > 0)
                {
                    RadioButton radioButton = list.First<Control>() as RadioButton;
                    radioButton.Checked = true;
                    this.settingControl_Click(radioButton, null);
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
                        current.ConnectionChanged += client_ConnectionChanged;
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
        private void settingControl_Click(object sender, EventArgs e)
        {
            this.settingsPropertyGrid.SelectedObject = (sender as Control).Tag;
        }
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadRecords();
        }
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!base.Visible)
            {
                if (this.CheckPassword())
                {
                    base.Show();
                }
            }
        }
        private void refreshRecordsButton_Click(object sender, EventArgs e)
        {
            this.ChangeRefreshStatus(!this.refreshRecords);
            if (this.refreshRecords)
            {
                this.LoadRecords();
            }
        }
        private void moreRecordsButton_Click(object sender, EventArgs e)
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
        private void minimizeButton_Click(object sender, EventArgs e)
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
        private void iccCore_TelegramSent(IccCoreTelegramEventArgs e)
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
        private void iccCore_TelegramQueued(IccCoreTelegramEventArgs e)
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
        private void iccCore_TelegramDropped(IccCoreTelegramEventArgs e)
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
        private void client_ConnectionChanged(object sender, AdapterConnectionChangedEventArgs e)
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
        private void eventLogger_EventLogged()
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

        private void clearSearchButton_Click(object sender, EventArgs e)
        {
            this.ClearControls(this.searchFlowLayoutPanel);
            this.ChangeRefreshStatus(true);
            this.LoadTransfers();
        }
        private void maskedTextBox_Click(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox)
            {
                (sender as MaskedTextBox).SelectAll();
            }
        }
        private void searchTelegramButton_Click(object sender, EventArgs e)
        {
            this.telegramSearchGroupbox.Visible = !this.telegramSearchGroupbox.Visible;
            if (!this.telegramSearchGroupbox.Visible)
            {
                this.clearSearchButton_Click(null, null);
            }
        }

        private void DoTelegramSearch_Click(object sender, EventArgs e)
        {
            this.LoadTransfers();
        }

        private void iccEventSearchButton_Click(object sender, EventArgs e)
        {
            this.iccEventSearchGroupBox.Visible = !this.iccEventSearchGroupBox.Visible;
            if (!this.iccEventSearchGroupBox.Visible)
            {
                this.clearSearchButton_Click(null, null);
            }
        }

        private void DoIccEventSearch_Click(object sender, EventArgs e)
        {
            this.LoadEvents();
        }

        private void clearSearchEventsPanel_Click(object sender, EventArgs e)
        {
            this.ClearControls(this.eventsSearchflowLayout);
            this.ChangeRefreshStatus(true);
            this.LoadEvents();
        }

        private void statusTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }













    }
}
