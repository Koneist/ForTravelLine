using System;
using System.Collections.Generic;
using System.IO;

namespace hotelproblem
{
    class Data
    {
        private string metrickey;
        private string providerkey;
        public string providerid;   
        private DateTime calculationDateTime;
        private DateTime exactBookingMaxDate;
        private DateTime exactBookingMaxDateAdmin;
        private string allRoomTypesHasImprovements;
        private string hasQuotaAvailability;
        private string hasRoomDependedOffers;
        private string hasSmartRateOfferss;
        private string hasOffersWithOptions;
        private string allOffersHasDescription;
        private string hasRateMix;
        private string hasActiveTransfers;
        private string translationPercentage;
        private string activePaymentSystemCount;
        private string hasPaymentAtArrival;
        private string hasBankCardGuarantee;
        private string hasBankCard;
        private string guestUnfinishedEmailsEnabled;
        private string feedbackLetterEnabled;
        private string surveyEnabled;
        private string bookingFormStatus;
        private string bookingFormOperationMode;
        private string bookingFormStatusLevel;
        private string minRoomTypesPhotosCount;
        private DateTime maxBookingAvailabilityDate;
        private string entryDateKey;
        private string specialOfferAvailability;
        private string hasOffersWithAccessCodes;
        private string hasOptionsWithoutOffers;
        private string providerUnfinishedEmailsEnabled;
        private string yandexAvailabilityApiEnabled;
        private string tripAdvisorEnabled;
        private string providerRatePlan;
        private string bookingCount;
        private string hasWarningCancellationRulePeriod;
        private string welcomeLetterIsEnabled;

        private byte pricesNotDetermined;
        private byte noQuotas;
        private byte noPaymentSystem;
        private byte noForm;
        private byte notEnoughRoomInformation;
        private byte notEnoughRoomPhoto;
        private byte fewTranslationsIntoForeignLanguages;
        private byte notEnoughPricingInformation;
        private byte noPaymentUponCheckIn;
        private byte noFoodService;
        private byte noTariffRatemix;
        private byte welcomeFeedbackNotConfigured;
        private string template;
        public string outputLine { get; }

        public Data( string _line )
        {
            string[] splitstr = _line.Split( ';' );
            metrickey = splitstr[ 0 ];
            providerkey = splitstr[ 1 ];
            providerid = splitstr[ 2 ];
            calculationDateTime = DateTime.Parse(splitstr[ 3 ]);
            exactBookingMaxDate = DateTime.Parse( splitstr[ 4 ] );
            exactBookingMaxDateAdmin = DateTime.Parse(splitstr[ 5 ]);
            allRoomTypesHasImprovements =  splitstr[ 6 ];
            hasQuotaAvailability = splitstr[ 7 ];
            hasRoomDependedOffers = splitstr[ 8 ];
            hasSmartRateOfferss = splitstr[ 9 ];
            hasOffersWithOptions =  splitstr[ 10 ];
            allOffersHasDescription = splitstr[ 11 ];
            hasRateMix = splitstr[ 12 ];
            hasActiveTransfers = splitstr[ 13 ];
            translationPercentage = splitstr[ 14 ];
            activePaymentSystemCount = splitstr[ 15 ];
            hasPaymentAtArrival = splitstr[ 16 ];
            hasBankCardGuarantee = splitstr[ 17 ];
            hasBankCard = splitstr[ 18 ];
            guestUnfinishedEmailsEnabled = splitstr[ 19 ];
            feedbackLetterEnabled = splitstr[ 20 ];
            surveyEnabled = splitstr[ 21 ];
            bookingFormStatus = splitstr[ 22 ];
            bookingFormOperationMode = splitstr[ 23 ];
            bookingFormStatusLevel = splitstr[ 24 ];
            minRoomTypesPhotosCount = splitstr[ 25 ];
            maxBookingAvailabilityDate = DateTime.Parse(splitstr[ 26 ]);
            entryDateKey = splitstr[ 27 ];
            specialOfferAvailability = splitstr[ 28 ];
            hasOffersWithAccessCodes = splitstr[ 29 ];
            hasOptionsWithoutOffers = splitstr[ 30 ];
            providerUnfinishedEmailsEnabled = splitstr[ 31 ];
            yandexAvailabilityApiEnabled = splitstr[ 32 ];
            tripAdvisorEnabled = splitstr[ 33 ];
            providerRatePlan = splitstr[ 34 ];
            bookingCount = splitstr[ 34 ];
            hasWarningCancellationRulePeriod = splitstr[ 35 ];
            welcomeLetterIsEnabled = splitstr[ 36 ];
        }
        public string problems()
        {
            if (specialOfferAvailability == "NULL")
                specialOfferAvailability = "0";
            if ((maxBookingAvailabilityDate < DateTime.Now.AddMonths(6)) || (Convert.ToInt32(specialOfferAvailability) < 6))
                pricesNotDetermined = 1;
            if ( activePaymentSystemCount == "0" )
                noPaymentSystem = 1;
            noQuotas = 0;
            if ( allRoomTypesHasImprovements == "0" )
                notEnoughRoomInformation = 1;
            if ( minRoomTypesPhotosCount == "NULL" )
                minRoomTypesPhotosCount = "0";
            if ( Convert.ToInt32( minRoomTypesPhotosCount ) < 3 )
                notEnoughRoomPhoto = 1;
            if ( Convert.ToDouble( translationPercentage ) < 50 )
                fewTranslationsIntoForeignLanguages = 1;
            if ( allOffersHasDescription == "0" )
                notEnoughPricingInformation = 1;
            if ( hasPaymentAtArrival == "0" )
                noPaymentUponCheckIn = 1;
            if ( hasOffersWithOptions == "0" )
                noFoodService = 1;
            noTariffRatemix = 0;
            if ( feedbackLetterEnabled == "0" )
                welcomeFeedbackNotConfigured = 1;
            string template = @"{0}   {1}   {2}   {3}   {4}   {5}   {6}   {7}   {8}   {9}   {10}   {11}   {12}   {13}";
            string outputLine = String.Format(template, metrickey, DateTime.Now.ToString("dd.MM.yyyy"), pricesNotDetermined, noQuotas, noPaymentSystem, noForm, notEnoughRoomInformation, notEnoughRoomPhoto, fewTranslationsIntoForeignLanguages, notEnoughPricingInformation, noPaymentUponCheckIn, noFoodService, noTariffRatemix, welcomeFeedbackNotConfigured);
            return outputLine;
        }
        public void Print() 
        {
            Console.WriteLine(outputLine);
        }

    }

    class Program
    {
        static void Main( string[] args )
        {
            var inputFile = "input.txt";
            var outputFile = "output.txt";
            String[] lines = File.ReadAllLines(inputFile);
            //string template = @"{0}    {1}";
            List<string> outputLines = new List<string>();
            for ( int i = 0; i < lines.Length; i++ )
            {
                Data line = new Data( lines[ i ] );
                line.problems();
                //string outputLine = String.Format( template, line.providerid, DateTime.Now.ToString( "dd.MM.yyyy" ) );
                outputLines.Add(line.outputLine);
                line.Print();
            }
            
            File.WriteAllLines(outputFile, outputLines);
        }
    }
}
