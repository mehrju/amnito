using Nop.Core.Data;
using Nop.Plugin.Orders.MultiShipping.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{

    public interface IGeoService
    {
        string FindLocation(LocationPoint locationPoint);
    }

    

    //***************************************
    public class LocationPoint
    {
        public double X;
        public double Y;
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Properties
    {
        public string name { get; set; }
    }

    public class Crs
    {
        public string type { get; set; }
        public Properties properties { get; set; }
    }

    public class Properties2
    {
        public string Name { get; set; }
        public object description { get; set; }
        public object timestamp { get; set; }
        public object begin { get; set; }
        public object end { get; set; }
        public object altitudeMode { get; set; }
        public double tessellate { get; set; }
        public double extrude { get; set; }
        public double visibility { get; set; }
        public object drawOrder { get; set; }
        public object icon { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
        //public List<List<List<List<double>>>> coordinates { get; set; }

    }

    public class Feature
    {
        public string type { get; set; }
        public Properties2 properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class JeoCollection
    {
        public string type { get; set; }
        public string name { get; set; }
        public Crs crs { get; set; }
        public List<Feature> features { get; set; }
    }



}
