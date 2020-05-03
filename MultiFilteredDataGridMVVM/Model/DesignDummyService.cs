using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Common;

namespace MultiFilteredDataGridMVVM.Model
{
    public class DesignDummyService : IDataService
    {
        public List<Thing> GetThings()
        {
            List<Thing> results = new List<Thing>();

            var thing = new Thing();
            thing.Id = 22;
            thing.Year = 2011;
            thing.Author = "Rover Blue";
            thing.FileName = "Blah.txt";
            thing.LastUpdated = DateTime.Now;
            thing.Country = "Canada";
            thing.Title = "WooHoo #1";
            results.Add(thing);

            thing = new Thing();
            thing.Id = 34;
            thing.Year = 2011;
            thing.Author = "Jimmy Bobb";
            thing.FileName = "FOO.txt";
            thing.LastUpdated = DateTime.Now;
            thing.Country = "Canada";
            thing.Title = "WooHoo #2";
            results.Add(thing);

            thing = new Thing();
            thing.Id = 34;
            thing.Year = 2011;
            thing.Author = "Jimmy Bobb Blue";
            thing.FileName = "FOO2.txt";
            thing.LastUpdated = DateTime.Now;
            thing.Country = "Canada";
            thing.Title = "WooHoo #3";
            results.Add(thing);

            return results;
        }
    }
}
