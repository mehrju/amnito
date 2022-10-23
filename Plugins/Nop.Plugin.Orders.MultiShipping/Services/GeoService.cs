using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Plugin.Orders.MultiShipping.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class GeoService : IGeoService
    {
        public string FindLocation(LocationPoint locationPoint)
        {
            string ErrorHint = "";

            try
            {
                foreach (var jsFile in Directory.GetFiles(Environment.CurrentDirectory + $@"\Plugins\Orders.MultiShipping\Content\MapResource\PrivatePost\GeoJsonData"))
                {
                    var jsContent = File.ReadAllText(jsFile);
                    jsContent = jsContent.Substring(jsContent.IndexOf('{') - 1).Trim();
                    try
                    {
                        JeoCollection jeoCollection = JsonConvert.DeserializeObject<JeoCollection>(jsContent);

                        foreach (var fea in jeoCollection.features)
                        {
                            List<LocationPoint> myPts = new List<LocationPoint>();
                            ErrorHint = jsFile + Environment.NewLine + fea.properties.Name;
                            //grab area points
                            foreach (var p1 in fea.geometry.coordinates[0])
                                myPts.Add(new LocationPoint() { X = p1[0], Y = p1[1] });
                            //Search edges
                            if (find(myPts, locationPoint)) { return fea.properties.Name; }

                            //File.WriteAllLines(@"d:\cdscd.txt", myPts.Select(x=>$"UNION ALL SELECT {x.X},{x.Y}"));
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " " + ErrorHint);

            }
            return "یافت نشد";

        }

        bool find(List<LocationPoint> myPts, LocationPoint locationPoint)
        {
            double X = locationPoint.X;
            double Y = locationPoint.Y;

            int sides = myPts.Count - 1;
            int j = sides - 1;
            bool pointStatus = false;
            for (int i = 0; i < sides; i++)
            {
                if (myPts[i].Y < Y && myPts[j].Y >= Y || myPts[j].Y < Y && myPts[i].Y >= Y)
                {
                    if (myPts[i].X + (Y - myPts[i].Y) / (myPts[j].Y - myPts[i].Y) * (myPts[j].X - myPts[i].X) < X)
                    {
                        pointStatus = !pointStatus;
                    }
                }
                j = i;
            }
            return pointStatus;
        }
    }
}
