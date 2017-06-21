using System;
using System.Threading;
using System.Threading.Tasks;
using BrightlightLib.SearchProviders;
using BrightlightLib.ViewModels;
using Brightlight.Models;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Brightlight.ConsoleDemo
{
    class MainClass
    {
        private static object resultsPrintLock = new object();

        private static MainViewModel vm;

        public static void SearchResultPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            PrintResults();
        }

        public static void SearchResultCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            PrintResults();
        }

        public static void Main(string[] args)
        {
            vm = new MainViewModel();
            vm.SearchResults.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (var x in e.OldItems)
                    {
                        var r = (SearchResultCollection)x;
                        r.PropertyChanged -= SearchResultPropertyChanged;
                        r.Results.CollectionChanged -= SearchResultCollectionChanged;
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (var x in e.NewItems)
                    {
                        var r = (SearchResultCollection)x;
                        r.PropertyChanged += SearchResultPropertyChanged;
                        r.Results.CollectionChanged += SearchResultCollectionChanged;
                    }
                }
                PrintResults();
            };

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (vm.Query.Length > 0)
                        vm.Query = vm.Query.Substring(0, vm.Query.Length - 1);
                }
                else
                {
                    vm.Query += key.KeyChar;
                }

                Console.Clear();
                Console.Out.WriteLine(vm.Query + "\r\n\r\n");
            }
        }

        public static void PrintResults()
        {
            lock (resultsPrintLock)
            {
                Console.Clear();
                foreach (var sr in vm.SearchResults)
                {
                    if (sr.Failed) continue;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(sr.Title);
                    Console.ForegroundColor = ConsoleColor.Black;
                    foreach (var r in sr.Results)
                    {
                        Console.Write(r.Title + " - ");
                        Console.WriteLine(r.Text.Substring(0, Math.Min(50, r.Text.Length - 1)));
                    }
                }
            }
        }
    }
}
