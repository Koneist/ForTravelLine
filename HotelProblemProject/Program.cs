using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HotelProblemProject
{
    class Program
    {
        private static string _connectionString = @"Data Source=DESKTOP-TFCE212;Initial Catalog=hotelstats;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        static void Main(string[] args)
        {
            List<Сharacteristic> characteristics = ReadPosts();
            foreach (Сharacteristic characteristic in characteristics)
            {
                InsertProblem(characteristic.providerid, characteristic.allRoomTypesHasImprovements,
                              characteristic.hasOffersWithOptions, characteristic.allOffersHasDescription, characteristic.translationPercentage,
                              characteristic.activePaymentSystemCount, characteristic.hasPaymentAtArrival, characteristic.feedbackLetterEnabled,
                              characteristic.minRoomTypesPhotosCount, characteristic.maxBookingAvailabilityDate, characteristic.specialOfferAvailability);
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

        private static void InsertProblem(int providerid, int allRoomTypesHasImprovements, 
                                      int? hasOffersWithOptions, int? allOffersHasDescription, double translationPercentage, 
                                      int activePaymentSystemCount,int hasPaymentAtArrival,int feedbackLetterEnabled,
                                      int? minRoomTypesPhotosCount, DateTime maxBookingAvailabilityDate, int? specialOfferAvailability)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    Problem problem = new Problem();
                    if ((maxBookingAvailabilityDate < DateTime.Now.AddMonths(6)) || (specialOfferAvailability < 6))
                        problem.pricesNotDetermined = 1;
                    if (activePaymentSystemCount == 0)
                        problem.noPaymentSystem = 1;
                    problem.noQuotas = 0;
                    if (allRoomTypesHasImprovements == 0)
                        problem.notEnoughRoomInformation = 1;
                    if (minRoomTypesPhotosCount < 3)
                        problem.notEnoughRoomPhoto = 1;
                    if (translationPercentage < 50)
                        problem.fewTranslationsIntoForeignLanguages = 1;
                    if (allOffersHasDescription == 0)
                        problem.notEnoughPricingInformation = 1;
                    if (hasPaymentAtArrival == 0)
                        problem.noPaymentUponCheckIn = 1;
                    if (hasOffersWithOptions == 0)
                        problem.noFoodService = 1;
                    problem.noTariffRatemix = 0;
                    if (feedbackLetterEnabled == 0)
                        problem.welcomeFeedbackNotConfigured = 1;

                    if ((problem.pricesNotDetermined == 1) || (problem.noPaymentSystem == 1) || (problem.noQuotas == 1) ||
                        (problem.notEnoughRoomInformation == 1) || (problem.notEnoughRoomPhoto == 1) || (problem.fewTranslationsIntoForeignLanguages == 1) ||
                        (problem.notEnoughPricingInformation == 1) || (problem.noPaymentUponCheckIn == 1) || (problem.noFoodService == 1) || (problem.noTariffRatemix == 1) ||
                        (problem.welcomeFeedbackNotConfigured == 1))
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
                        cmd.Parameters.Add("@pricesNotDetermined", SqlDbType.Int).Value = problem.pricesNotDetermined;
                        cmd.Parameters.Add("@noQuotas", SqlDbType.Int).Value = problem.noQuotas;
                        cmd.Parameters.Add("@noPaymentSystem", SqlDbType.Int).Value = problem.noPaymentSystem;
                        cmd.Parameters.Add("@noForm", SqlDbType.Int).Value = problem.noForm;
                        cmd.Parameters.Add("@notEnoughRoomInformation", SqlDbType.Int).Value = problem.notEnoughRoomInformation;
                        cmd.Parameters.Add("@fewTranslationsIntoForeignLanguages", SqlDbType.Int).Value = problem.fewTranslationsIntoForeignLanguages;
                        cmd.Parameters.Add("@notEnoughPricingInformation", SqlDbType.Int).Value = problem.notEnoughPricingInformation;
                        cmd.Parameters.Add("@notEnoughRoomPhoto", SqlDbType.Int).Value = problem.notEnoughRoomPhoto;
                        cmd.Parameters.Add("@noPaymentUponCheckIn", SqlDbType.Int).Value = problem.noPaymentUponCheckIn;
                        cmd.Parameters.Add("@noFoodService", SqlDbType.Int).Value = problem.noFoodService;
                        cmd.Parameters.Add("@noTariffRatemix", SqlDbType.Int).Value = problem.noTariffRatemix;
                        cmd.Parameters.Add("@welcomeFeedbackNotConfigured", SqlDbType.Int).Value = problem.welcomeFeedbackNotConfigured;
                    }
                }
            }
        }
    }
}
