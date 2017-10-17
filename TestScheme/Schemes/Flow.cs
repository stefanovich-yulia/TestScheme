using System;

namespace TestScheme.Schemes
{
    public class Flow
    {
        public double Vwater { get; set; }
        public double Voil { get; set; }
        public  double Tempreture { get; set; }

        public Flow(double vwater, double voil, double tempreture)
        {
            this.Vwater = vwater;
            this.Voil = voil;
            this.Tempreture = tempreture;
        }
    }
}
