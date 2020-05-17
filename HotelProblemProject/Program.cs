using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HotelProblemProject
{
    class Program
    {
        private static string _connectionString = @"Data Source=DESKTOP-TFCE212;Initial Catalog=hotelstats;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        static void Main(string[] args)
        {
            List<Сharacteristic> characteristics = ReadPosts();
            List<Problem> problems = new List<Problem>();
            List<ProblemByYear> problemsByYear = new List<ProblemByYear>();
            foreach (Сharacteristic characteristic in characteristics)
            {
                Problem problem = CheckProblem(characteristic);
                //problems.Add(problem);
                problemsByYear = SortProblem(problemsByYear, problem);
                InsertProblem(characteristic.providerid, problem);
            }
            foreach(ProblemByYear problemByYear in problemsByYear)
            {
                foreach(ProblemByMonth problemByMonth in problemByYear.problemsByMonth)
                    Console.WriteLine("{0}, {1}", problemByYear.year, problemByMonth.month);
            }
        }

        private static List<Сharacteristic> ReadPosts()
        {
            List<Сharacteristic> characteristics = new List<Сharacteristic>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText =
                        @"SELECT
                            [metrickey],
                            [providerid],
                            [calculationDateTime],
                            [allRoomTypesHasImprovements],
                            [hasOffersWithOptions],
                            [allOffersHasDescription],
                            [translationPercentage],
                            [activePaymentSystemCount],
                            [hasPaymentAtArrival],
                            [feedbackLetterEnabled],
                            [minRoomTypesPhotosCount],
                            [maxBookingAvailabilityDate],
                            [specialOfferAvailability]
                        FROM Сharacteristic";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var characteristic = new Сharacteristic
                            {
                                metrickey = Convert.ToInt32(reader["metrickey"]),
                                providerid = Convert.ToInt32(reader["providerid"]),
                                calculationDateTime = Convert.ToDateTime(reader["calculationDateTime"]),
                                allRoomTypesHasImprovements = Convert.ToInt32(reader["allRoomTypesHasImprovements"]),
                                hasOffersWithOptions = reader["hasOffersWithOptions"] is DBNull ?  (int?)null : Convert.ToInt32(reader["hasOffersWithOptions"]),
                                allOffersHasDescription = reader["allOffersHasDescription"] is DBNull ? (int?)null : Convert.ToInt32(reader["allOffersHasDescription"]),
                                translationPercentage = Convert.ToDouble(reader["translationPercentage"]),
                                activePaymentSystemCount = Convert.ToInt32(reader["activePaymentSystemCount"]),
                                hasPaymentAtArrival = Convert.ToInt32(reader["hasPaymentAtArrival"]),
                                feedbackLetterEnabled = Convert.ToInt32(reader["feedbackLetterEnabled"]),
                                minRoomTypesPhotosCount = reader["minRoomTypesPhotosCount"] is DBNull ? (int ?)null : Convert.ToInt32(reader["minRoomTypesPhotosCount"]),
                                maxBookingAvailabilityDate = Convert.ToDateTime(reader["maxBookingAvailabilityDate"]),
                                specialOfferAvailability = reader["specialOfferAvailability"] is DBNull ? (int?)null : Convert.ToInt32(reader["specialOfferAvailability"]),
                            };
                            characteristics.Add(characteristic);
                        }
                    }
                }
            }
            return characteristics;
        }

        private static Problem CheckProblem(Сharacteristic characteristic)
        {
            var _problem = new Problem();
            if ((characteristic.maxBookingAvailabilityDate < DateTime.Now.AddMonths(6)) || (characteristic.specialOfferAvailability < 6))
                _problem.pricesNotDetermined = 1;
            if (characteristic.activePaymentSystemCount == 0)
                _problem.noPaymentSystem = 1;
            _problem.noQuotas = 0;
            if (characteristic.allRoomTypesHasImprovements == 0)
                _problem.notEnoughRoomInformation = 1;
            if (characteristic.minRoomTypesPhotosCount < 3)
                _problem.notEnoughRoomPhoto = 1;
            if (characteristic.translationPercentage < 50)
                _problem.fewTranslationsIntoForeignLanguages = 1;
            if (characteristic.allOffersHasDescription == 0)
                _problem.notEnoughPricingInformation = 1;
            if (characteristic.hasPaymentAtArrival == 0)
                _problem.noPaymentUponCheckIn = 1;
            if (characteristic.hasOffersWithOptions == 0)
                _problem.noFoodService = 1;
            _problem.noTariffRatemix = 0;
            if (characteristic.feedbackLetterEnabled == 0)
                _problem.welcomeFeedbackNotConfigured = 1;
            if ((_problem.pricesNotDetermined == 1) || (_problem.noPaymentSystem == 1) || (_problem.noQuotas == 1) ||
                        (_problem.notEnoughRoomInformation == 1) || (_problem.notEnoughRoomPhoto == 1) || (_problem.fewTranslationsIntoForeignLanguages == 1) ||
                        (_problem.notEnoughPricingInformation == 1) || (_problem.noPaymentUponCheckIn == 1) || (_problem.noFoodService == 1) || (_problem.noTariffRatemix == 1) ||
                        (_problem.welcomeFeedbackNotConfigured == 1))
            {
                _problem.isWrite = true;
            } else
            {
                _problem.isWrite = false;
            }
            _problem.calculationDateTime = characteristic.calculationDateTime;
                return _problem;
        }

        public static List<ProblemByYear> SortProblem(List<ProblemByYear> problemsByYears, Problem problem)
        {
            //ProblemByMonth problemByMonth = new ProblemByMonth(problem);
            ProblemByYear problemByYears = new ProblemByYear(problem.calculationDateTime.Year);
            problemByYears.problemsByMonth.Add(new ProblemByMonth(problem));
            if ((problemsByYears.Count == 0) || problemsByYears.Any(year => problemByYears.year != problem.calculationDateTime.Year)) 
            {
                problemsByYears.Add(problemByYears);
            } 
            else if (problemsByYears.Any(year => problemByYears.year == problem.calculationDateTime.Year))
            {
                problemsByYears.Where(year => problemByYears.year == problem.calculationDateTime.Year).FirstOrDefault().problemsByMonth.Add(new ProblemByMonth(problem));
            }

            return problemsByYears;
        }
        
        private static void InsertProblem(int providerid, Problem _problem)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    if (_problem.isWrite)
                    {
                        cmd.CommandText = @"
                        INSERT INTO [Problem]
                           ([providerId],
                            [date],
                            [pricesNotDetermined],
                            [noQuotas],
                            [noPaymentSystem],
                            [noForm],
                            [notEnoughRoomInformation],
                            [notEnoughRoomPhoto],
                            [fewTranslationsIntoForeignLanguages],
                            [notEnoughPricingInformation],
                            [noPaymentUponCheckIn],
                            [noFoodService],
                            [noTariffRatemix],
                            [welcomeFeedbackNotConfigured]) 
                        VALUES 
                           (@providerId,
                            @date,
                            @pricesNotDetermined,
                            @noQuotas,
                            @noPaymentSystem,
                            @noForm,
                            @notEnoughRoomInformation,
                            @fewTranslationsIntoForeignLanguages,
                            @notEnoughPricingInformation,
                            @notEnoughRoomPhoto,
                            @noPaymentUponCheckIn,
                            @noFoodService,
                            @noTariffRatemix,
                            @welcomeFeedbackNotConfigured)
                        SELECT SCOPE_IDENTITY()";

                        cmd.Parameters.Add("@providerId", SqlDbType.Int).Value = providerid;
                        cmd.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Now;
                        cmd.Parameters.Add("@pricesNotDetermined", SqlDbType.Int).Value = _problem.pricesNotDetermined;
                        cmd.Parameters.Add("@noQuotas", SqlDbType.Int).Value = _problem.noQuotas;
                        cmd.Parameters.Add("@noPaymentSystem", SqlDbType.Int).Value = _problem.noPaymentSystem;
                        cmd.Parameters.Add("@noForm", SqlDbType.Int).Value = _problem.noForm;
                        cmd.Parameters.Add("@notEnoughRoomInformation", SqlDbType.Int).Value = _problem.notEnoughRoomInformation;
                        cmd.Parameters.Add("@fewTranslationsIntoForeignLanguages", SqlDbType.Int).Value = _problem.fewTranslationsIntoForeignLanguages;
                        cmd.Parameters.Add("@notEnoughPricingInformation", SqlDbType.Int).Value = _problem.notEnoughPricingInformation;
                        cmd.Parameters.Add("@notEnoughRoomPhoto", SqlDbType.Int).Value = _problem.notEnoughRoomPhoto;
                        cmd.Parameters.Add("@noPaymentUponCheckIn", SqlDbType.Int).Value = _problem.noPaymentUponCheckIn;
                        cmd.Parameters.Add("@noFoodService", SqlDbType.Int).Value = _problem.noFoodService;
                        cmd.Parameters.Add("@noTariffRatemix", SqlDbType.Int).Value = _problem.noTariffRatemix;
                        cmd.Parameters.Add("@welcomeFeedbackNotConfigured", SqlDbType.Int).Value = _problem.welcomeFeedbackNotConfigured;
                    }
                }
            }
        }
    }
}
