using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using WeatherApp.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<WeatherJSON> WeatherEachHours;
        ObservableCollection<DailyForecast> WeatherEachDays;
        public MainPage()
        {
            this.InitializeComponent();
            WeatherEachHours = new ObservableCollection<WeatherJSON>();
            InitJSON();
            WeatherEachDays = new ObservableCollection<DailyForecast>();

            
                
        }
        private async void InitJSON()
        {
            var urls = "http://dataservice.accuweather.com/forecasts/v1/hourly/12hour/353412?" +
                "apikey=CxILqfbYMdKI30fs02iXyl2JZJdF2MeU&metric=true";
            var list = await WeatherJSON.GetJSON(urls) as List<WeatherJSON>;
            Debug.WriteLine("Count: " + list.Count);
            list.ForEach(it =>
            {
                var match = Regex.Matches(it.DateTime, "T(?<time>\\d+):")[0].Groups["time"].Value;
                if(int.Parse(match) > 12)
                {
                    match = (int.Parse(match) - 12) + "PM";
                }
                else
                {
                    match += "PM";
                }
                it.DateTime = match;
                it.Temperature.Value += "\u00B0";
                it.WeatherIcon = string.Format("https://vortex.accuweather.com/adc2010/images/slate/icons/{0}.svg",it.WeatherIcon);
                WeatherEachHours.Add(it);
            });
            WeatherDescriptionTextBlock.Text = list[0].IconPhrase;
            WeatherTemperatureTextBlock.Text = list[0].Temperature.Value;
        }
        private async void InitEachDaysJSON()
        {
            var url = "http://dataservice.accuweather.com/forecasts/v1/daily/5day/353412?" +
                "apikey=CxILqfbYMdKI30fs02iXyl2JZJdF2MeU&metric=true";
            var obj = await WeatherEachDay.GetWeatherEach(url) as WeatherEachDay;
            obj.DailyForecasts.ForEach(it =>
            {
                var match = Regex.Matches(it.Date, "\\d+");
                var date = new DateTime(int.Parse(match[0].Value), int.Parse(match[1].Value), int.Parse(match[2].Value));
                it.Date = date.DayOfWeek.ToString();        
                it.Day.Icon = string.Format("https://vortex.accuweather.com/adc2010/images/slate/icons/{0}.svg", it.Day.Icon);
                WeatherEachDays.Add(it);
            });
            Today.Text = WeatherEachDays[0].Date + "   Today";
            MaxTemperature.Text = WeatherEachDays[0].Temperature.Maximum.Value + "";
            MinTemperature.Text = WeatherEachDays[0].Temperature.Minimum.Value + "";
            WeatherEachDays.RemoveAt(0);
        }

    }
}
