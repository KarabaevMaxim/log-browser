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

            logManager = new LogManager();
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
            catch(FileNotFoundException)
            {
                MessageBox.Show(string.Format("Файл логов {0} не найден", Settings.SettingsInfo.LogFileName), "Ошибкапри загрузке логов.");
            }

            TypesCmb.Items.Add(LogTypes.ALL);
            TypesCmb.Items.Add(LogTypes.INFO);
            TypesCmb.Items.Add(LogTypes.WARN);
            TypesCmb.Items.Add(LogTypes.ERROR);
            TypesCmb.SelectedIndex = 0;
            FromDate.SelectedDate = DateTime.Now;
            UntilDate.SelectedDate = DateTime.Now;
        }

        private void UpdateLogTbl(List<Log> logs)
        {
            this.LogsTbl.Columns.Clear();
            this.LogsTbl.Items.Clear();

            foreach (var item in this.bindings)
                this.LogsTbl.Columns.Add(new DataGridTextColumn { Header = item.Key, Binding = new Binding(item.Value) });

            try
            {
                foreach (var item in logs)
                    this.LogsTbl.Items.Add(item);
            }
            catch(NullReferenceException)
            {

            }
            
        }

        private void LogsTbl_Loaded(object sender, RoutedEventArgs e)
        {
          //  this.UpdateLogTbl();
        }

        private void LogsTbl_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();

            if(e.Row.DataContext is Log log)
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
            {
                if((LogTypes)cmb.SelectedItem == LogTypes.ALL)
                    this.UpdateLogTbl(this.logManager.Logs);
                else
                    this.UpdateLogTbl(this.logManager.Logs.Where(l => l.LogType == (LogTypes)cmb.SelectedItem).ToList());
            }
        }

        private void FromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is DatePicker datePicker)
            {
                this.UpdateLogTbl(this.logManager.Logs.Where(l => l.DateTime >= datePicker.SelectedDate
                && l.DateTime <= UntilDate.SelectedDate).ToList());
            }
        }

        private void UntilDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker datePicker)
            {
                this.UpdateLogTbl(this.logManager.Logs.Where(l => l.DateTime >= FromDate.SelectedDate
                && l.DateTime <= datePicker.SelectedDate).ToList());
            }
        }

        private Dictionary<string, string> bindings;
        private LogManager logManager;


    }
}
