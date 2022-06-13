using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace TaskMangerWPF.Convertors
{
    public class CpuUsageConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PerformanceCounter performance = 
                new PerformanceCounter("Process", "% Processor Time", (value as string));
            try
            {
                performance.NextValue();
                return String.Format("{0:0.00}", performance.NextValue()/100) + " %";
            }
            catch (Exception)
            {
            }
            return "0.00 %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
