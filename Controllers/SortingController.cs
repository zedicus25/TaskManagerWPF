using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace TaskMangerWPF.Controllers
{
    public class SortingController
    {
        public void SortByNameDescending(ref ObservableCollection<Process> processes)
        {
            List<Process> list = new List<Process>(processes.OrderByDescending(p=> p.ProcessName));
            processes.Clear();
            foreach (var item in list)
                processes.Add(item);
            GC.Collect(GC.GetGeneration(list));
        }

        public void SortByNameAscending(ref ObservableCollection<Process> processes)
        {
            List<Process> list = new List<Process>(processes.OrderBy(p => p.ProcessName));
            processes.Clear();
            foreach (var item in list)
                processes.Add(item);
            GC.Collect(GC.GetGeneration(list));
        }

        public void SortByRamAscending(ref ObservableCollection<Process> processes)
        {
            List<Process> list = new List<Process>(processes.OrderBy(p => p.PagedMemorySize64));
            processes.Clear();
            foreach (var item in list)
                processes.Add(item);
            GC.Collect(GC.GetGeneration(list));
        }

        public void SortByRamDescending(ref ObservableCollection<Process> processes)
        {
            List<Process> list = new List<Process>(processes.OrderByDescending(p => p.PagedMemorySize64));
            processes.Clear();
            foreach (var item in list)
                processes.Add(item);
            GC.Collect(GC.GetGeneration(list));
        }

    }
}
