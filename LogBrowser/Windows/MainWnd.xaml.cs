using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogBrowser.Windows
{
    using System.IO;
    using System.Threading.Tasks;
    using Logic;
    /// <summary>
    /// Логика взаимодействия для MainWnd.xaml
    /// </summary>
    public partial class MainWnd : Window
    {
        public MainWnd()
        {
            InitializeComponent();

            bindings = new Dictionary<string, string>
            {
                { "Дата/Время", "DateTime" },
                { "Тип", "LogType" },
                { "Источник", "Source" },
                { "Сообщение", "Message" }
            };

            this.logManager = new LogManager();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings.LoadSettings();
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("{0}: {1}. {2}.", ex.Message, ex.StackTrace, ex.Source), "Ошибка при загрузке настроек.");
            }

            try
            {
                logManager.ReadLogs();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(string.Format("Файл логов {0} не найден", Settings.SettingsInfo.LogFileName), "Ошибка при загрузке логов.");
            }

          //  this.UpdateLogs();

            TypesCmb.Items.Add(LogTypes.ALL);
            TypesCmb.Items.Add(LogTypes.INFO);
            TypesCmb.Items.Add(LogTypes.WARN);
            TypesCmb.Items.Add(LogTypes.ERROR);
            TypesCmb.SelectedIndex = 0;
            FromDate.SelectedDate = DateTime.Now;
            UntilDate.SelectedDate = DateTime.Now;
			RecordCountCmb.Items.Clear();
			RecordCountCmb.Items.Add(50);
			RecordCountCmb.Items.Add(200);
			RecordCountCmb.Items.Add(500);
			RecordCountCmb.Items.Add(1000);
			RecordCountCmb.Items.Add(logManager.Logs.Count);
			RecordCountCmb.SelectedIndex = 0;

			TypesCmb.SelectionChanged += TypesCmb_SelectionChanged;
            FromDate.SelectedDateChanged += FromDate_SelectedDateChanged;
            UntilDate.SelectedDateChanged += UntilDate_SelectedDateChanged;

            this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
        }

        private void UpdateLogs()
        {
            try
            {
                logManager.ReadLogs();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(string.Format("Файл логов {0} не найден", Settings.SettingsInfo.LogFileName), "Ошибка при загрузке логов.");
            }

            this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
        }

        private void UpdateLogTbl(List<Log> logs)
        {
            this.LogsTbl.Columns.Clear();
            this.LogsTbl.Items.Clear();

            
            foreach (var item in this.bindings)
            {
                Binding binding = new Binding(item.Value);

                if (item.Key == "Дата/Время")
                    binding.StringFormat = "dd.MM.yy HH.mm.ss";

                this.LogsTbl.Columns.Add(new DataGridTextColumn { Header = item.Key, Binding = binding });
            }
                

            try
            {
				var collection = logs.Skip(Math.Max(0, logs.Count - (int)RecordCountCmb.SelectedItem));

				foreach (var item in collection)
                    this.LogsTbl.Items.Add(item);
            }
            catch(NullReferenceException){}
        }

        private void LogsTbl_Loaded(object sender, RoutedEventArgs e)
        {}

        private void LogsTbl_LoadingRow(object sender, DataGridRowEventArgs e)
        {
			//e.Row.Header = (e.Row.GetIndex() + 1).ToString();
			e.Row.Header = (this.logManager.Logs.Count - e.Row.GetIndex()).ToString();


			if (e.Row.DataContext is Log log)
            {
                if (log.LogType == LogTypes.ERROR)
                    e.Row.Background = Brushes.Pink;
                else
                    if (log.LogType == LogTypes.WARN)
                        e.Row.Background = Brushes.Yellow;
                    else
                        e.Row.Background = Brushes.White;
            }
        }

        private void LogsTbl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.DataContext is Log log)
                {
                    MessageBox.Show(log.Message, string.Format("{0} : {1}", log.DateTime, log.LogType));
                }
            }
        }

        private void TypesCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox cmb)
				this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
        }

        private void FromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is DatePicker datePicker)
            {
                this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
            }
        }

        private void UntilDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker datePicker)
                this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
        }

        private Func<Log, bool> UpdateFilters()
        {
            Func<Log, bool> filters = null;

            if ((LogTypes)TypesCmb.SelectedItem == LogTypes.ALL)
            {
                filters = l => l.DateTime.Date >= FromDate.SelectedDate.Value.Date
                && l.DateTime.Date <= UntilDate.SelectedDate.Value.Date;
            }
            else
            {
                filters = l => l.DateTime.Date >= FromDate.SelectedDate.Value.Date
                && l.DateTime.Date <= UntilDate.SelectedDate.Value.Date && l.LogType == (LogTypes)TypesCmb.SelectedItem;
            }

            return filters;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateLogs();
        }

		private void RecordCountCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateLogTbl(this.logManager.Logs.Where(this.UpdateFilters()).ToList());
		}

		private Dictionary<string, string> bindings;
        private LogManager logManager;
	}
}
