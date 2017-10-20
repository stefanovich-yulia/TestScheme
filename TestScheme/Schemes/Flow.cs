using System;

namespace TestScheme.Schemes
{
    public class Flow
    {
        public double Gwater { get; set; }
        public double Goil { get; set; }
        public  double Tempreture { get; set; }
        public double Pressure { get; set; }

        public Flow()
        {
            this.Gwater = 0;
            this.Goil = 0;
            this.Tempreture = 0;
            this.Pressure = 0;
        }
        public Flow(double gwater, double goil, double tempreture, double pressure)
        {
            this.Gwater = gwater;
            this.Goil = goil;
            this.Tempreture = tempreture;
            this.Pressure = pressure;
        }
    }
}
