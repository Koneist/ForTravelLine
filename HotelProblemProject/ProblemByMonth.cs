using System;
using System.Collections.Generic;
using System.Text;

namespace HotelProblemProject
{
    class ProblemByMonth
    {
        public ProblemByMonth(Problem problem)
        {
            this.problem = problem;
            month = problem.calculationDateTime.Month;
        }
        public int month;
        public Problem problem;
    }
}
