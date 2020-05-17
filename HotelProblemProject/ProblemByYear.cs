using System;
using System.Collections.Generic;
using System.Text;

namespace HotelProblemProject
{
    class ProblemByYear
    {
        public ProblemByYear(int year)
        {
            this.year = year;
            problemsByMonth = new List<ProblemByMonth>();
        }
        public int year;
        public List<ProblemByMonth> problemsByMonth;
    }
}
