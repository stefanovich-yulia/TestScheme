using System;

namespace TestScheme.Scheme
{
    public class Flow
    {
        double _vwater;
        double _voil;
        double _tempreture;

        public Flow(double vwater, double voil, double tempreture)
        {
            this._vwater = vwater;
            this._voil = voil;
            this._tempreture = tempreture;
        }
    }
}
