using System;
using System.Collections.Generic;
using System.IO;

namespace hotelproblem
{
    class Data
    {
        private string metrickey;
        private string providerkey;
        public string providerid { get; }
        private string calculationdatetime;
        private DateTime exactbookingmaxdate;
        private string exactbookingmaxdateadmin;
        //private bool allroomtypeshasimprovements;
        private string masquotaavailability;
        private string hasroomdependedoffers;
        private string hassmartrateoffers;
        //private bool hasofferswithoptions;
        private string allOffershasdescription;
        private string hasratemix;
        private string hasactivetransfers;
        private string translationpercentage;
        private string activepaymentsystemcount;
        private string haspaymentatarrival;
        private string hasbankcardguarantee;
        private string hasbankcard;
        private string guestUnfinishedemailsenabled;
        private string feedbackletterenabled;
        private string Surveyenabled;
        private string bookingformStatus;
        private string bookingformoperationmode;
        private string bookingformstatuslevel;
        private string minroomtypesphotoscount;
        private string maxbookingavailabilitydate;
        private string entrydatekey;
        private string specialofferavailability;
        private string hasofferswithaccesscodes;
        private string hasoptionswithoutoffers;
        private string providerunfinishedemailsenabled;
        private string yandexavailabilityapienabled;
        private string tripadvisorenabled;
        private string providerrateplan;
        private string bookingcount;
        private string haswarningcancellationruleperiod;
        private string welcomeletterisenabled;

        private byte pricesnotdetermined;
        private byte noquotas;
        private byte nopaymentsystem;
        private byte noform;
        private byte notenoughroominformation;
        private byte notenoughroomphoto;
        private byte fewtranslationsintoforeignlanguages;
        private byte notenoughpricinginformation;
        private byte nopaymentuponcheckin;
        private byte nofoodservice;
        private byte notariffratemix;
        private byte welcomefeedbacknotconfigured;

        public Data( string _line )
        {
            string[] splitstr = _line.Split( ';' );
            metrickey = splitstr[ 0 ];
            providerkey = splitstr[ 1 ];
            providerid = splitstr[ 2 ];
            calculationdatetime = splitstr[ 3 ];
            exactbookingmaxdate = DateTime.Parse( splitstr[ 4 ] );
            exactbookingmaxdateadmin = splitstr[ 5 ];
            //allroomtypeshasimprovements = bool.Parse( splitstr[ 6 ] );
            masquotaavailability = splitstr[ 7 ];
            hasroomdependedoffers = splitstr[ 8 ];
            hassmartrateoffers = splitstr[ 9 ];
            //hasofferswithoptions = bool.Parse( splitstr[ 10 ] );
            allOffershasdescription = splitstr[ 11 ];
            hasratemix = splitstr[ 12 ];
            hasactivetransfers = splitstr[ 13 ];
            translationpercentage = splitstr[ 14 ];
            activepaymentsystemcount = splitstr[ 15 ];
            haspaymentatarrival = splitstr[ 16 ];
            hasbankcardguarantee = splitstr[ 17 ];
            hasbankcard = splitstr[ 18 ];
            guestUnfinishedemailsenabled = splitstr[ 19 ];
            feedbackletterenabled = splitstr[ 20 ];
            Surveyenabled = splitstr[ 21 ];
            bookingformStatus = splitstr[ 22 ];
            bookingformoperationmode = splitstr[ 23 ];
            bookingformstatuslevel = splitstr[ 24 ];
            minroomtypesphotoscount = splitstr[ 25 ];
            maxbookingavailabilitydate = splitstr[ 26 ];
            entrydatekey = splitstr[ 27 ];
            specialofferavailability = splitstr[ 28 ];
            hasofferswithaccesscodes = splitstr[ 29 ];
            hasoptionswithoutoffers = splitstr[ 30 ];
            providerunfinishedemailsenabled = splitstr[ 31 ];
            yandexavailabilityapienabled = splitstr[ 32 ];
            tripadvisorenabled = splitstr[ 33 ];
            providerrateplan = splitstr[ 34 ];
            bookingcount = splitstr[ 34 ];
            haswarningcancellationruleperiod = splitstr[ 35 ];
            welcomeletterisenabled = splitstr[ 36 ];
        }
        public void problems( )
        {
            pricesnotdetermined = 1;
            if ( activepaymentsystemcount == "0" )
                nopaymentsystem = 1;
            noquotas = 0;
            /*if ( !allroomtypeshasimprovements )
                notenoughroominformation = 1;*/
            if ( minroomtypesphotoscount == "NULL" )
                minroomtypesphotoscount = "0";
            if ( Convert.ToInt32( minroomtypesphotoscount ) < 3 )
                notenoughroomphoto = 1;
            if ( Convert.ToDouble( translationpercentage ) < 50 )
                fewtranslationsintoforeignlanguages = 1;
            if ( allOffershasdescription == "0" )
                notenoughpricinginformation = 1;
            if ( haspaymentatarrival == "0" )
                nopaymentuponcheckin = 1;
            /*if ( !hasofferswithoptions )
                nofoodservice = 1;*/
            notariffratemix = 0;
            if ( feedbackletterenabled == "0" )
                welcomefeedbacknotconfigured = 1;
            Console.Write( metrickey + "\t" + pricesnotdetermined + "\t" + noquotas + "\t" + nopaymentsystem + "\t" + noform + "\t" + notenoughroominformation + "\t" + notenoughroomphoto + "\t" + fewtranslationsintoforeignlanguages + "\t" + notenoughpricinginformation + "\t" + nopaymentuponcheckin + "\t" + nofoodservice + "\t" + notariffratemix + "\t" + welcomefeedbacknotconfigured );
        }

    }

    class Program
    {
        static void Main( string[] args )
        {
            String[] lines = File.ReadAllLines( "input.txt" );
            var outputFile = "output.txt";
            string template = @"{0}    {1}";
            List<string> outputLines = new List<string>();
            for ( int i = 0; i < lines.Length; i++ )
            {
                Data line = new Data( lines[ i ] );
                line.problems();
                string outputLine = String.Format( template, line.providerid, DateTime.Now.ToString( "dd.MM.yyyy" ) );
                outputLines.Add( outputLine );
            }
            File.WriteAllLines(outputFile, outputLines);
        }
    }
}
