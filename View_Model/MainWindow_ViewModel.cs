using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using TaskMangerWPF.Controllers;

namespace TaskMangerWPF.View_Model
{
    public class MainWindow_ViewModel : INotifyPropertyChanged
    {
        
        
        public event PropertyChangedEventHandler PropertyChanged;

        public Process SelectedProcess
        {
            get { return _selectedProcess; }
            set { _selectedProcess = value;
                OnPropertyChanged("SelectedProcess");
            }
        }

        public string CpuUsage
        {
            get 
            {
                return _cpuUsage; 
            }
            set 
            {
                _cpuUsage = value;
                OnPropertyChanged("CpuUsage");
            }
        }

        public string RamUsage
        {
            get { return _ramUsage; }
            set
            {
                _ramUsage = value;
                OnPropertyChanged("RamUsage");
            }
        }

        private string _cpuUsage;
        private string _ramUsage;
        private void CalculateCpuUsage()
        {
            PerformanceCounter performance =
               new PerformanceCounter("Process", "% Processor Time");
            float res = 0;
            foreach (var item in _processes)
            {
                performance.InstanceName = item.ProcessName;
                try
                {
                    performance.NextValue();
                    res += performance.NextValue();
                }
                catch (Exception)
                {
                }
            }
            _cpuUsage = String.Format("{0:0.00}", res/100) + " %";
            OnPropertyChanged("CpuUsage");

            //DONT WORK
            /*_timer.Stop();
            PerformanceCounter performance = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            performance.NextValue();
            _cpuUsage = String.Format("{0:0.00}", performance.NextValue()) + " %";
            OnPropertyChanged("CpuUsage");
            _timer.Start();*/
        }

        private void CalculateRamUsage()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            float res = 0;
            res = ramCounter.NextValue();
            _ramUsage = String.Format("{0:0.00}", (100-(res*100)/16000)) + " %";
            OnPropertyChanged("RamUsage");
        }
        public ObservableCollection<Process> Processes
        {
            get => _processes;
            set
            {
                _processes = value;
                OnPropertyChanged("Processes");
            }
        }

        private ObservableCollection<Process> _processes;
        private Process _selectedProcess;
        private SortingController _sortingController;

        private bool _nameAcendingSort;
        private bool _ramAcendingSort;

        private event Action NameSortEvent;
        private event Action RamSortEvent;

        private System.Windows.Forms.Timer _timer;

        public MainWindow_ViewModel()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 15000;
            _timer.Tick += this.TimerTick;
            _timer.Start();


            _sortingController = new SortingController();
            _processes = new ObservableCollection<Process>(Process.GetProcesses());
        }

        private void TimerTick(object sender, EventArgs e)
        {
            RefreshProcesses();
            CalculateRamUsage();
            CalculateCpuUsage();
        }

        public RelayCommand KillCommand
        {
            get
            {
                return _killCommand ?? (_killCommand = new RelayCommand(() =>
                {

                    if(_selectedProcess != null)
                    {
                        string name = _selectedProcess.ProcessName;
                        try
                        {
                            foreach (var item in Process.GetProcessesByName(name))
                            {
                                item.Kill();
                            }
                        }
                        catch (Exception)
                        {
                        }
                        RefreshProcesses();
                    }           
                }));
            }
            set
            {
                _killCommand = value;
            }
        }
        public RelayCommand SortByNameCommand
        {
            get
            {
                return _sortByNameCommand ?? (_sortByNameCommand = new RelayCommand(() =>
                {
                    SortByName();
                }));
            }
            set
            {
                _sortByNameCommand = value;
            }
        }

        public RelayCommand SortByRamCommand
        {
            get
            {
                return _sortByRamCommand ?? (_sortByRamCommand = new RelayCommand(() =>
                {
                    SortByRam();
                }));
            }
            set
            {
                _sortByRamCommand = value;
            }
        }

        private RelayCommand _killCommand;
        private RelayCommand _sortByNameCommand;
        private RelayCommand _sortByRamCommand;

        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void RefreshProcesses()
        {
            _processes.Clear();
            try 
            {
                _processes = new ObservableCollection<Process>(Process.GetProcesses()); 
            }
            catch (Exception)
            {
            }
            OnPropertyChanged("Processes");

            if(NameSortEvent != null)
            {
                _nameAcendingSort = !_nameAcendingSort;
                SortByName();
            }
            if (RamSortEvent != null)
            {
                _ramAcendingSort = !_ramAcendingSort;
                SortByRam();
            }
        }

        private void SortByName()
        {
            NameSortEvent = null;
            if (_nameAcendingSort)
                NameSortEvent += SortByNameAscending;
            if (_nameAcendingSort == false)
                NameSortEvent += SortByNameDescending;
            NameSortEvent?.Invoke();
            _nameAcendingSort = !_nameAcendingSort;
        }

        private void SortByRam()
        {
            RamSortEvent = null;
            if (_ramAcendingSort)
                RamSortEvent += SortByRamAscending;
            if (_ramAcendingSort == false)
                RamSortEvent += SortByRamDescending;
            RamSortEvent?.Invoke();
            _ramAcendingSort = !_ramAcendingSort;
        }


        private void SortByNameAscending() => _sortingController.SortByNameAscending(ref _processes);
        private void SortByNameDescending() => _sortingController.SortByNameDescending(ref _processes);

        private void SortByRamAscending() => _sortingController.SortByRamAscending(ref _processes);
        private void SortByRamDescending() => _sortingController.SortByRamDescending(ref _processes);

    }
}
