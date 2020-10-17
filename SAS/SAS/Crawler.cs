﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using RestSharp;

namespace SAS
{
    public class Crawler
    {
        public string departureAirport;
        public string arrivalAirport;
        public string connectionAirport;
        public string departureTime;
        public string arrivalTime;
        public string cheapestPrice;
        public string taxes;
        public Crawler()
        {

        }
        public Crawler(string dA, string aA, string cA, string dT, string aT, string cP, string tax)
        {
            this.departureAirport = dA;
            this.arrivalAirport = aA;
            this.connectionAirport = cA;
            this.departureTime = dT;
            this.arrivalTime = aT;
            this.cheapestPrice = cP;
            this.taxes = tax;
        }

        private List<Crawler> collectedDataFromSaS = new List<Crawler>();

        private IList<RestResponseCookie> cookieList = new List<RestResponseCookie>();
        public void addingCookiesToCookieList(IList<RestResponseCookie> cookiesToAdd)
        {
            foreach(RestResponseCookie cookie in cookiesToAdd)
            {
                cookieList.Add(cookie);
            }
        }
        public void crawling()
        {
            firstPageLoad();

            string enc = gettingENC();
            enc = enc.Trim('"');
         
            string javaScriptUrlNumber = returnJavaScriptUrlNumber(enc);
            
            string[] javaScriptData = returnJavaScriptData(javaScriptUrlNumber).Split(',');

            javaScriptData[0] = javaScriptData[0].Remove(javaScriptData[0].Length - 1);
            javaScriptData[1] = javaScriptData[1].Remove(javaScriptData[1].Length - 1);
            javaScriptData[1] = javaScriptData[1].Remove(0, 13);
           
            loadingFlights(javaScriptUrlNumber, javaScriptData[0], javaScriptData[1]);
            reloadingFlights(enc);
        }
        private void firstPageLoad()
        {
            // Url
            RestClient client = new RestClient("https://classic.flysas.com/en/de/");
            // Headers
            client.AddDefaultHeader("Host", "classic.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.FollowRedirects = false;
            // Request
            RestRequest request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);
            // Cookies
            addingCookiesToCookieList(response.Cookies);
        }
        private string gettingENC()
        {
            RestClient client = new RestClient("https://classic.flysas.com/en/de/");
            RestRequest request = new RestRequest("", Method.POST);
            // Headers
            client.AddDefaultHeader("Host", "classic.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            //client.AddDefaultHeader("Content-Type", "multipart/form-data; boundary=---------------------------33994240382562922193872950699");
            client.AddDefaultHeader("Origin", "https://classic.flysas.com");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Referer", "https://classic.flysas.com/en/de/");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            foreach (RestResponseCookie cookie in cookieList)
            {
                request.AddCookie(cookie.Name, cookie.Value);
            }
            request = buildingQuery(request);
            IRestResponse response = client.Execute(request);
            string query = "\"ENC\".value=\"[0-9A-Z]+\"";
            Regex regex = new Regex(query);
            MatchCollection match = regex.Matches(response.Content);
            string[] splittedData = match[0].Value.Split('=');
            return splittedData[1];
        }
        private RestRequest buildingQuery(RestRequest request)
        {
            request.AlwaysMultipartFormData = true;
            request.AddParameter("__EVENTTARGET", "ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$Searchbtn$ButtonLink");
            request.AddParameter("__EVENTARGUMENT", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$hdnProfilingConsent", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$hdnTermsConsent", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$_ssoLogin$MainFormBorderPanel$uid", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$_ssoLogin$MainFormBorderPanel$pwd", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$_ssoLogin$MainFormBorderPanel$hdnShowModal", "");
            request.AddParameter("ctl00$FullRegion$TopRegion$_siteHeader$_ssoLogin$MainFormBorderPanel$hdnIsEb0", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$ceptravelTypeSelector$TripTypeSelector", "roundtrip");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenIntercont", "False");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenDomestic","SE,GB");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenFareType", "A");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$txtFrom", "Stockholm, Sweden - Arlanda (ARN)");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenFrom", "ARN");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$txtTo", "London, United Kingdom - Heathrow (LHR)");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenTo", "LHR");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$txtFromTOJ", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$predictiveSearch$hiddenFromTOJ", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hiddenOutbound", "2020-11-05");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hiddenReturn", "2020-11-12");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hdnSelectedOutboundMonth", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hdnSelectedReturnMonth", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hiddenReturnCalVisible", "");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$hiddenStoreCalDates", "Sat Oct 17 2020 00:00:00 GMT+0300 (Eastern European Summer Time),Sat Oct 17 2020 00:00:00 GMT+0300 (Eastern European Summer Time),Mon Oct 11 2021 00:00:00 GMT+0300 (Eastern European Summer Time)");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$selectOutbound", "2020-10-01");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepCalendar$selectReturn", "2020-10-01");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$FlexDateSelector", "Show selected dates");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepPassengerTypes$passengerTypeAdult", "1");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepPassengerTypes$passengerTypeChild211", "0");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepPassengerTypes$passengerTypeInfant", "0");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$cepNdpFareTypeSelector$ddlFareTypeSelector", "A");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$hdnsetDefaultValue", "true");
            request.AddParameter("ctl00$FullRegion$MainRegion$ContentRegion$ContentFullRegion$ContentLeftRegion$CEPGroup1$CEPActive$cepNDPRevBookingArea$hdncalendarDropdown", "true");
            request.AddParameter("__PREVIOUSPAGE", "3aoIK5urOF6qLmjEUVWoe7Zlok_H7Ef8UkS2oCFR_Ccg24aQSIRhidbF3PGeuRmIFTuiGxx8ealPNKfgqBWh77mCC2k1");
            request.AddParameter("__VIEWSTATE", "");
            request.AddParameter("__VIEWSTATEGENERATOR", "CA0B0334");
            return request;
        }
        private string returnJavaScriptUrlNumber(string enc)
        {
            // Url and method
            RestClient client = new RestClient("https://book.flysas.com/pl/SASC/wds/Override.action?SO_SITE_EXT_PSPURL=https://classic.sas.dk/SASCredits/SASCreditsPaymentMaster.aspx&SO_SITE_TP_TPC_POST_EOT_WT=50000&SO_SITE_USE_ACK_URL_SERVICE=TRUE&WDS_URL_JSON_POINTS=ebwsprod.flysas.com%2FEAJI%2FEAJIService.aspx&SO_SITE_EBMS_API_SERVERURL=%20https%3A%2F%2F1aebwsprod.flysas.com%2FEBMSPointsInternal%2FEBMSPoints.asmx&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_MIN_REQ_MIL=500");
            RestRequest request = new RestRequest("", Method.POST);
            // Headers
            client.AddDefaultHeader("Host", "book.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Referer", "https://classic.flysas.com/");
            client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");
            client.AddDefaultHeader("Origin", "https://classic.flysas.com");
            client.AddDefaultHeader("DNT", "1");
            client.ConnectionGroupName = "keep-alive";
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            // Request Body
            string requestJson = "__EVENTTARGET=btnSubmitAmadeus&__EVENTARGUMENT=&LANGUAGE=GB&SITE=SKBKSKBK&EMBEDDED_TRANSACTION=FlexPricerAvailability&SIP_INTERNAL=44454641554C545F4E44505F4345505F49443D32313230383826504152414D455445525F434845434B53554D3D3633264345505F49443D3231353436312652454449524543545F55524C3D25326664656661756C742E617370782533666964253364383531372532366570736C616E6775616765253364656E265354415254504147455F49443D38353137264D41524B45543D4445265245565F5744535F4F42464545533D253363253366786D6C2B76657273696F6E25336427312E30272B656E636F64696E672533642769736F2D383835392D3127253366253365253363534F5F474C253365253363474C4F42414C5F4C4953542533652533634E414D45253365534954455F4C4953545F4F425F4645455F434F44455F544F5F4558454D50542533632532664E414D452533652533634C4953545F454C454D454E54253365253363434F4445253365543031253363253266434F44452533652533634C4953545F56414C55452533655430312533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E542533652533634C4953545F454C454D454E54253365253363434F4445253365543032253363253266434F44452533652533634C4953545F56414C55452533655430322533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E54253365253363253266474C4F42414C5F4C495354253365253363253266534F5F474C253365265245565F494E535552414E43453D253363253366786D6C2B76657273696F6E253364253232312E302532322B656E636F64696E6725336425323269736F2D383835392D31253232253366253365253363534F5F474C253365253363474C4F42414C5F4C4953542533652533634E414D45253365534954455F494E535552414E43455F50524F44554354532533632532664E414D452533652533634C4953545F454C454D454E54253365253363434F4445253365454154253363253266434F44452533652533634C4953545F56414C5545253365253363494E535552414E43455F434F44452533652533634555524F50455F4F57253365434F57452533632532664555524F50455F4F572533652533634555524F50455F5254253365435254452533632532664555524F50455F5254253365253363494E544552434F4E545F4F57253365434F5757253363253266494E544552434F4E545F4F57253365253363494E544552434F4E545F525425336543525457253363253266494E544552434F4E545F5254253365253363253266494E535552414E43455F434F44452533652533632532664C4953545F56414C55452533652533634C4953545F56414C55452533652533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C5545253365312533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E54253365253363253266474C4F42414C5F4C495354253365253363253266534F5F474C253365&WDS_FLOW=REVENUE&WDS_FACADE_CALLBACK=https%3A%2F%2Fclassic.flysas.com%2FAmadeusFacade%2Fdefault.aspx%3Fepslanguage%3Den&SO_SITE_ATC_ALLOW_LSA_INDIC=TRUE&SO_SITE_ADVANCED_CATEGORIES=TRUE&SO_SITE_TK_OFFICE_ID=FRASK08RV&SO_SITE_QUEUE_OFFICE_ID=FRASK08RV&SO_SITE_CSSR_TAXES=FALSE&SO_SITE_OFFICE_ID=FRASK08RV&SO_SITE_ETKT_Q_AND_CAT=32C0&SO_SITE_FP_CAL_DISP_NA_DATE=TRUE&SO_SITE_ETKT_Q_OFFICE_ID=FRASK08RV&SO_GL=%3CSO_GL%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_INSURANCE_PRODUCTS%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3EEAT%3C%2FCODE%3E%3CLIST_VALUE%3ECRTE%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E1%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_QUEUE_DEFINITION_LIST%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3E0%3C%2FCODE%3E%3CLIST_VALUE%3ESRV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E34%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E1%3C%2FCODE%3E%3CLIST_VALUE%3ECAN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E31%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E2%3C%2FCODE%3E%3CLIST_VALUE%3ERIR%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E3%3C%2FCODE%3E%3CLIST_VALUE%3EREI%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E4%3C%2FCODE%3E%3CLIST_VALUE%3EAWA%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E8%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E1%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E6%3C%2FCODE%3E%3CLIST_VALUE%3ERIP%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_LIST_OB_FEE_CODE_TO_EXEMPT%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3ET01%3C%2FCODE%3E%3CLIST_VALUE%3ET01%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3ET02%3C%2FCODE%3E%3CLIST_VALUE%3ET02%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3C%2FSO_GL%3E&SO_SITE_FD_SOLDOUT_FLIGHT=TRUE&SO_SITE_QUEUE_CATEGORY=8C50&SO_SITE_ALLOW_LSA_INDICATOR=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_AVD_SEL_FLIGHTS=TRUE&WDS_CAL_RANGE=15&WDS_SERVICING_FLOW_TE_FBAG=TRUE&WDS_SHOW_INVINFO=FALSE&WDS_BOOKING_FLOW_TE_MEAL=TRUE&WDS_ACTIVATE_APP_FOR_CC_MOP=TRUE&PRICING_TYPE=C&WDS_SHOW_TAXES=TRUE&B_LOCATION_1=ARN&WDS_FO_IATA=23494925&WDS_SHOW_ADDCAL=TRUE&WDS_INST_LIST=SAScDE%3Bklarna-SAScDE%3Bklarna_nt&WDS_USE_FQN=TRUE&WDS_ACTIVATE_APP_FOR_ALL_MOP=FALSE&COMMERCIAL_FARE_FAMILY_1=SKSTDA&WDS_CHECKIN_NOTIF=FALSE&TRIP_TYPE=R&WDS_HELPCONTACTURL=http%3A%2F%2Fclassic.sas.se%2Fen%2Fmisc%2FArkiv%2Fcontact-sia-%2F&WDS_SAS_CREDITS=TRUE&WDS_ANCILLARIES=FALSE&WDS_BOOKING_FLOW_TE_FBAG=TRUE&WDS_CC_LIST=AX-SAS%2FERETAIL_DE-true%3ACA-SAS%2FERETAIL_DE-true%3AVI-SAS%2FERETAIL_DE-true%3ADC-SAS%2FERETAIL_DE-false%3ADS-SAS%2FERETAIL_DE-true%3ATP-SAS%2FERETAIL_DE-false&WDS_SASCPCTRANGE=2-6&WDS_SHOW_AB=TRUE&WDS_FOID_EXCL_LIST=DK&DATE_RANGE_VALUE_1=1&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&DATE_RANGE_VALUE_2=1&WDS_BOOKING_FLOW_TE_XBAG=TRUE&WDS_POINTS_EARNED=FALSE&WDS_ORIGIN_SITE=DE&WDS_SHOW_CMP_CODE=TRUE&TRAVELLER_TYPE_1=ADT&WDS_NEWSLETTER_FCO=FALSE&B_LOCATION_2=LHR&WDS_BOOKING_FLOW_TE_SEATMAP=TRUE&WDS_TIME_OPTION=True&WDS_FRAS=TRUE&DISPLAY_TYPE=2&WDS_MOBILE_NEW_DESIGN=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SHOW_MINISEARCH=LINK&B_DATE_1=202011050000&B_DATE_2=202011120000&E_LOCATION_2=ARN&E_LOCATION_1=LHR&WDS_EBMS_CAMPAIGN=TRUE&DATE_RANGE_QUALIFIER_2=C&DATE_RANGE_QUALIFIER_1=C&WDS_INSTPAY=TRUE&ENCT=1&ENC=" + enc + "&__PREVIOUSPAGE=EOuVgEVGcPaooWlcQzY7uwfysikykaVpb-H5wZ3xp_fcVkbM_4Y3Yh3_OEwpzEWi5gOj_s80sjeP-1yYWe-Fp-6rsY8xAKiOA8--sL0aS3jICz0W0&__VIEWSTATE=%2FwEPDwUKMTE1MTc0MDk0N2RkuN2qfxyKJHLW%2BuU0D7%2BB8ZTdGMU%3D&__VIEWSTATEGENERATOR=BAA3076B";
            request.RequestFormat = DataFormat.Json;
            request.AddBody(requestJson);
            // Response
            IRestResponse response = client.Execute(request);
            string query = @"\/sk\d+.js";
            Regex regex = new Regex(query);
            MatchCollection match = regex.Matches(response.Content);
            return match[0].Value;
        }
        private string returnJavaScriptData(string urlNumber)
        {
            RestClient client = new RestClient("https://book.flysas.com" + urlNumber);
            RestRequest request = new RestRequest("", Method.GET);
            client.AddDefaultHeader("Host", "book.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "*/*");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Referer", "https://book.flysas.com/pl/SASC/wds/Override.action?SO_SITE_EXT_PSPURL=https://classic.sas.dk/SASCredits/SASCreditsPaymentMaster.aspx&SO_SITE_TP_TPC_POST_EOT_WT=50000&SO_SITE_USE_ACK_URL_SERVICE=TRUE&WDS_URL_JSON_POINTS=ebwsprod.flysas.com%2FEAJI%2FEAJIService.aspx&SO_SITE_EBMS_API_SERVERURL=%20https%3A%2F%2F1aebwsprod.flysas.com%2FEBMSPointsInternal%2FEBMSPoints.asmx&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_MIN_REQ_MIL=500");

            IRestResponse response = client.Execute(request);
            string query = @"PID=[A-Z0-9-]+.,ajax_header:.[a-z]+.";
            Regex regex = new Regex(query);
            MatchCollection match = regex.Matches(response.Content);
            return match[0].Value;
        }
        private void loadingFlights(string urlNumber, string urlJavaScriptData, string ajax)
        {
            RestClient client = new RestClient("https://book.flysas.com" + urlNumber + "?" + urlJavaScriptData);
            RestRequest request = new RestRequest("", Method.POST);
            // Headers
            client.AddDefaultHeader("Host", "book.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "*/*");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("X-Distil-Ajax", ajax);
            client.AddDefaultHeader("Content-Type", "text/plain;charset=UTF-8");
            client.AddDefaultHeader("Origin", "https://book.flysas.com");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Referer", "https://book.flysas.com/pl/SASC/wds/Override.action?SO_SITE_EXT_PSPURL=https://classic.sas.dk/SASCredits/SASCreditsPaymentMaster.aspx&SO_SITE_TP_TPC_POST_EOT_WT=50000&SO_SITE_USE_ACK_URL_SERVICE=TRUE&WDS_URL_JSON_POINTS=ebwsprod.flysas.com%2FEAJI%2FEAJIService.aspx&SO_SITE_EBMS_API_SERVERURL=%20https%3A%2F%2F1aebwsprod.flysas.com%2FEBMSPointsInternal%2FEBMSPoints.asmx&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_MIN_REQ_MIL=500");
            string requestQuery = "p=%7B%22proof%22%3A%221aa%3A1602835125594%3A75Y9pYL1WnR3HrYQynPi%22%2C%22cookies%22%3A1%2C%22setTimeout%22%3A0%2C%22setInterval%22%3A0%2C%22appName%22%3A%22Netscape%22%2C%22platform%22%3A%22Win32%22%2C%22syslang%22%3A%22en-GB%22%2C%22userlang%22%3A%22en-GB%22%2C%22cpu%22%3A%22WindowsNT10.0%3BWin64%3Bx64%22%2C%22productSub%22%3A%2220100101%22%2C%22plugins%22%3A%7B%220%22%3A%22ShockwaveFlash32.0.0.445%22%7D%2C%22mimeTypes%22%3A%7B%220%22%3A%22FutureSplashmovieapplication%2Fx-futuresplash%22%2C%221%22%3A%22AdobeFlashmovieapplication%2Fx-shockwave-flash%22%7D%2C%22screen%22%3A%7B%22width%22%3A1536%2C%22height%22%3A864%2C%22colorDepth%22%3A24%7D%2C%22fonts%22%3A%7B%220%22%3A%22Calibri%22%2C%221%22%3A%22Cambria%22%2C%222%22%3A%22Constantia%22%2C%223%22%3A%22LucidaBright%22%2C%224%22%3A%22Georgia%22%2C%225%22%3A%22SegoeUI%22%2C%226%22%3A%22Candara%22%2C%227%22%3A%22TrebuchetMS%22%2C%228%22%3A%22Verdana%22%2C%229%22%3A%22Consolas%22%2C%2210%22%3A%22LucidaConsole%22%2C%2211%22%3A%22LucidaSansTypewriter%22%2C%2212%22%3A%22CourierNew%22%2C%2213%22%3A%22Courier%22%7D%2C%22fp2%22%3A%7B%22userAgent%22%3A%22Mozilla%2F5.0(WindowsNT10.0%3BWin64%3Bx64%3Brv%3A81.0)Gecko%2F20100101Firefox%2F81.0%22%2C%22language%22%3A%22en-GB%22%2C%22screen%22%3A%7B%22width%22%3A1536%2C%22height%22%3A864%2C%22availHeight%22%3A834%2C%22availWidth%22%3A1536%2C%22pixelDepth%22%3A24%2C%22innerWidth%22%3A766%2C%22innerHeight%22%3A727%2C%22outerWidth%22%3A777%2C%22outerHeight%22%3A834%2C%22devicePixelRatio%22%3A1.25%7D%2C%22timezone%22%3A3%2C%22indexedDb%22%3Atrue%2C%22addBehavior%22%3Afalse%2C%22openDatabase%22%3Afalse%2C%22cpuClass%22%3A%22unknown%22%2C%22platform%22%3A%22Win32%22%2C%22doNotTrack%22%3A%221%22%2C%22plugins%22%3A%22ShockwaveFlash%3A%3AShockwaveFlash32.0r0%3A%3Aapplication%2Fx-shockwave-flash~swf%2Capplication%2Fx-futuresplash~spl%22%2C%22canvas%22%3A%7B%22winding%22%3A%22yes%22%2C%22towebp%22%3Afalse%2C%22blending%22%3Atrue%2C%22img%22%3A%22a6859e1f39a144ae9929f0b3a631c77a55da0a61%22%7D%2C%22webGL%22%3A%7B%22img%22%3A%228d37e15cc9363584537e76e4d202a7e8e811da59%22%2C%22extensions%22%3A%22ANGLE_instanced_arrays%3BEXT_blend_minmax%3BEXT_color_buffer_half_float%3BEXT_float_blend%3BEXT_frag_depth%3BEXT_shader_texture_lod%3BEXT_sRGB%3BEXT_texture_compression_bptc%3BEXT_texture_filter_anisotropic%3BOES_element_index_uint%3BOES_standard_derivatives%3BOES_texture_float%3BOES_texture_float_linear%3BOES_texture_half_float%3BOES_texture_half_float_linear%3BOES_vertex_array_object%3BWEBGL_color_buffer_float%3BWEBGL_compressed_texture_s3tc%3BWEBGL_compressed_texture_s3tc_srgb%3BWEBGL_debug_renderer_info%3BWEBGL_debug_shaders%3BWEBGL_depth_texture%3BWEBGL_draw_buffers%3BWEBGL_lose_context%22%2C%22aliasedlinewidthrange%22%3A%22%5B1%2C1%5D%22%2C%22aliasedpointsizerange%22%3A%22%5B1%2C1024%5D%22%2C%22alphabits%22%3A8%2C%22antialiasing%22%3A%22yes%22%2C%22bluebits%22%3A8%2C%22depthbits%22%3A24%2C%22greenbits%22%3A8%2C%22maxanisotropy%22%3A16%2C%22maxcombinedtextureimageunits%22%3A32%2C%22maxcubemaptexturesize%22%3A16384%2C%22maxfragmentuniformvectors%22%3A1024%2C%22maxrenderbuffersize%22%3A16384%2C%22maxtextureimageunits%22%3A16%2C%22maxtexturesize%22%3A16384%2C%22maxvaryingvectors%22%3A30%2C%22maxvertexattribs%22%3A16%2C%22maxvertextextureimageunits%22%3A16%2C%22maxvertexuniformvectors%22%3A4096%2C%22maxviewportdims%22%3A%22%5B32767%2C32767%5D%22%2C%22redbits%22%3A8%2C%22renderer%22%3A%22Mozilla%22%2C%22shadinglanguageversion%22%3A%22WebGLGLSLES1.0%22%2C%22stencilbits%22%3A0%2C%22vendor%22%3A%22Mozilla%22%2C%22version%22%3A%22WebGL1.0%22%2C%22vertexshaderhighfloatprecision%22%3A23%2C%22vertexshaderhighfloatprecisionrangeMin%22%3A127%2C%22vertexshaderhighfloatprecisionrangeMax%22%3A127%2C%22vertexshadermediumfloatprecision%22%3A23%2C%22vertexshadermediumfloatprecisionrangeMin%22%3A127%2C%22vertexshadermediumfloatprecisionrangeMax%22%3A127%2C%22vertexshaderlowfloatprecision%22%3A23%2C%22vertexshaderlowfloatprecisionrangeMin%22%3A127%2C%22vertexshaderlowfloatprecisionrangeMax%22%3A127%2C%22fragmentshaderhighfloatprecision%22%3A23%2C%22fragmentshaderhighfloatprecisionrangeMin%22%3A127%2C%22fragmentshaderhighfloatprecisionrangeMax%22%3A127%2C%22fragmentshadermediumfloatprecision%22%3A23%2C%22fragmentshadermediumfloatprecisionrangeMin%22%3A127%2C%22fragmentshadermediumfloatprecisionrangeMax%22%3A127%2C%22fragmentshaderlowfloatprecision%22%3A23%2C%22fragmentshaderlowfloatprecisionrangeMin%22%3A127%2C%22fragmentshaderlowfloatprecisionrangeMax%22%3A127%2C%22vertexshaderhighintprecision%22%3A0%2C%22vertexshaderhighintprecisionrangeMin%22%3A31%2C%22vertexshaderhighintprecisionrangeMax%22%3A30%2C%22vertexshadermediumintprecision%22%3A0%2C%22vertexshadermediumintprecisionrangeMin%22%3A31%2C%22vertexshadermediumintprecisionrangeMax%22%3A30%2C%22vertexshaderlowintprecision%22%3A0%2C%22vertexshaderlowintprecisionrangeMin%22%3A31%2C%22vertexshaderlowintprecisionrangeMax%22%3A30%2C%22fragmentshaderhighintprecision%22%3A0%2C%22fragmentshaderhighintprecisionrangeMin%22%3A31%2C%22fragmentshaderhighintprecisionrangeMax%22%3A30%2C%22fragmentshadermediumintprecision%22%3A0%2C%22fragmentshadermediumintprecisionrangeMin%22%3A31%2C%22fragmentshadermediumintprecisionrangeMax%22%3A30%2C%22fragmentshaderlowintprecision%22%3A0%2C%22fragmentshaderlowintprecisionrangeMin%22%3A31%2C%22fragmentshaderlowintprecisionrangeMax%22%3A30%2C%22unmaskedvendor%22%3A%22GoogleInc.%22%2C%22unmaskedrenderer%22%3A%22ANGLE(Intel(R)UHDGraphics620Direct3D11vs_5_0ps_5_0)%22%7D%2C%22touch%22%3A%7B%22maxTouchPoints%22%3A0%2C%22touchEvent%22%3Afalse%2C%22touchStart%22%3Afalse%7D%2C%22video%22%3A%7B%22ogg%22%3A%22probably%22%2C%22h264%22%3A%22probably%22%2C%22webm%22%3A%22probably%22%7D%2C%22audio%22%3A%7B%22ogg%22%3A%22probably%22%2C%22mp3%22%3A%22maybe%22%2C%22wav%22%3A%22probably%22%2C%22m4a%22%3A%22maybe%22%7D%2C%22vendor%22%3A%22%22%2C%22product%22%3A%22Gecko%22%2C%22productSub%22%3A%2220100101%22%2C%22browser%22%3A%7B%22ie%22%3Afalse%2C%22chrome%22%3Afalse%2C%22webdriver%22%3Afalse%7D%2C%22window%22%3A%7B%22historyLength%22%3A4%2C%22hardwareConcurrency%22%3A8%2C%22iframe%22%3Afalse%2C%22battery%22%3Afalse%7D%2C%22location%22%3A%7B%22protocol%22%3A%22https%3A%22%7D%2C%22fonts%22%3A%22Calibri%3BCentury%3BHaettenschweiler%3BMarlett%3BPristina%22%2C%22devices%22%3A%7B%22count%22%3A2%2C%22data%22%3A%7B%220%22%3A%7B%22deviceId%22%3A%22zIXKrFzeV1Fa1Qsd5EuoMfHDbf1lXWxcityZw%2B8JOLo%3D%22%2C%22kind%22%3A%22videoinput%22%2C%22label%22%3A%22%22%2C%22groupId%22%3A%2228t7FB1WFgUuKVXqrnkI8pWzTyTQLFSShXrxllLmAmI%3D%22%7D%2C%221%22%3A%7B%22deviceId%22%3A%220DO1Zw7RF5bTcWcosV%2FI4Kt6gTNi04dGZATVULZNhRM%3D%22%2C%22kind%22%3A%22audioinput%22%2C%22label%22%3A%22%22%2C%22groupId%22%3A%22gHsyhNMkW86mqoo1uOc%2B%2FlCfPrB7o9zYhow0pRm5Tyc%3D%22%7D%7D%7D%7D%7D";
            request.AddParameter("text/xml", requestQuery, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            addingCookiesToCookieList(response.Cookies);
        }
        private void reloadingFlights(string enc)
        {
            RestClient client = new RestClient("https://book.flysas.com/pl/SASC/wds/Override.action?SO_SITE_EXT_PSPURL=https://classic.sas.dk/SASCredits/SASCreditsPaymentMaster.aspx&SO_SITE_TP_TPC_POST_EOT_WT=50000&SO_SITE_USE_ACK_URL_SERVICE=TRUE&WDS_URL_JSON_POINTS=ebwsprod.flysas.com%2FEAJI%2FEAJIService.aspx&SO_SITE_EBMS_API_SERVERURL=%20https%3A%2F%2F1aebwsprod.flysas.com%2FEBMSPointsInternal%2FEBMSPoints.asmx&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_MIN_REQ_MIL=500");
            RestRequest request = new RestRequest("", Method.POST);
            client.AddDefaultHeader("Host", "book.flysas.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:81.0) Gecko/20100101 Firefox/81.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "en-GB,en;q=0.5");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Referer", "https://classic.flysas.com/");
            client.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");
            client.AddDefaultHeader("Origin", "https://classic.flysas.com");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            foreach(RestResponseCookie cookie in cookieList)
            {
                if(cookie.Name.Equals("D_IID") ||
                   cookie.Name.Equals("D_UID") ||
                   cookie.Name.Equals("D_ZID") ||
                   cookie.Name.Equals("D_ZUID") ||
                   cookie.Name.Equals("D_HID") ||
                   cookie.Name.Equals("D_SID"))
                {
                    request.AddCookie(cookie.Name, cookie.Value);
                }

            }
            string requestQuery = "__EVENTTARGET=btnSubmitAmadeus&__EVENTARGUMENT=&LANGUAGE=GB&SITE=SKBKSKBK&EMBEDDED_TRANSACTION=FlexPricerAvailability&SIP_INTERNAL=44454641554C545F4E44505F4345505F49443D32313230383826504152414D455445525F434845434B53554D3D3633264345505F49443D3231353436312652454449524543545F55524C3D25326664656661756C742E617370782533666964253364383531372532366570736C616E6775616765253364656E265354415254504147455F49443D38353137264D41524B45543D4445265245565F5744535F4F42464545533D253363253366786D6C2B76657273696F6E25336427312E30272B656E636F64696E672533642769736F2D383835392D3127253366253365253363534F5F474C253365253363474C4F42414C5F4C4953542533652533634E414D45253365534954455F4C4953545F4F425F4645455F434F44455F544F5F4558454D50542533632532664E414D452533652533634C4953545F454C454D454E54253365253363434F4445253365543031253363253266434F44452533652533634C4953545F56414C55452533655430312533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E542533652533634C4953545F454C454D454E54253365253363434F4445253365543032253363253266434F44452533652533634C4953545F56414C55452533655430322533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E54253365253363253266474C4F42414C5F4C495354253365253363253266534F5F474C253365265245565F494E535552414E43453D253363253366786D6C2B76657273696F6E253364253232312E302532322B656E636F64696E6725336425323269736F2D383835392D31253232253366253365253363534F5F474C253365253363474C4F42414C5F4C4953542533652533634E414D45253365534954455F494E535552414E43455F50524F44554354532533632532664E414D452533652533634C4953545F454C454D454E54253365253363434F4445253365454154253363253266434F44452533652533634C4953545F56414C5545253365253363494E535552414E43455F434F44452533652533634555524F50455F4F57253365434F57452533632532664555524F50455F4F572533652533634555524F50455F5254253365435254452533632532664555524F50455F5254253365253363494E544552434F4E545F4F57253365434F5757253363253266494E544552434F4E545F4F57253365253363494E544552434F4E545F525425336543525457253363253266494E544552434F4E545F5254253365253363253266494E535552414E43455F434F44452533652533632532664C4953545F56414C55452533652533634C4953545F56414C55452533652533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C55452533654E2533632532664C4953545F56414C55452533652533634C4953545F56414C5545253365312533632532664C4953545F56414C55452533652533632532664C4953545F454C454D454E54253365253363253266474C4F42414C5F4C495354253365253363253266534F5F474C253365&WDS_FLOW=REVENUE&WDS_FACADE_CALLBACK=https%3A%2F%2Fclassic.flysas.com%2FAmadeusFacade%2Fdefault.aspx%3Fepslanguage%3Den&SO_SITE_ATC_ALLOW_LSA_INDIC=TRUE&SO_SITE_ADVANCED_CATEGORIES=TRUE&SO_SITE_TK_OFFICE_ID=FRASK08RV&SO_SITE_QUEUE_OFFICE_ID=FRASK08RV&SO_SITE_CSSR_TAXES=FALSE&SO_SITE_OFFICE_ID=FRASK08RV&SO_SITE_ETKT_Q_AND_CAT=32C0&SO_SITE_FP_CAL_DISP_NA_DATE=TRUE&SO_SITE_ETKT_Q_OFFICE_ID=FRASK08RV&SO_GL=%3CSO_GL%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_INSURANCE_PRODUCTS%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3EEAT%3C%2FCODE%3E%3CLIST_VALUE%3ECRTE%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E1%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_QUEUE_DEFINITION_LIST%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3E0%3C%2FCODE%3E%3CLIST_VALUE%3ESRV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E34%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E1%3C%2FCODE%3E%3CLIST_VALUE%3ECAN%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E31%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E2%3C%2FCODE%3E%3CLIST_VALUE%3ERIR%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E3%3C%2FCODE%3E%3CLIST_VALUE%3EREI%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E4%3C%2FCODE%3E%3CLIST_VALUE%3EAWA%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E8%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E1%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3E6%3C%2FCODE%3E%3CLIST_VALUE%3ERIP%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3EFRASK08RV%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E30%3C%2FLIST_VALUE%3E%3CLIST_VALUE%3E0%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3CGLOBAL_LIST%3E%3CNAME%3ESITE_LIST_OB_FEE_CODE_TO_EXEMPT%3C%2FNAME%3E%3CLIST_ELEMENT%3E%3CCODE%3ET01%3C%2FCODE%3E%3CLIST_VALUE%3ET01%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3CLIST_ELEMENT%3E%3CCODE%3ET02%3C%2FCODE%3E%3CLIST_VALUE%3ET02%3C%2FLIST_VALUE%3E%3C%2FLIST_ELEMENT%3E%3C%2FGLOBAL_LIST%3E%3C%2FSO_GL%3E&SO_SITE_FD_SOLDOUT_FLIGHT=TRUE&SO_SITE_QUEUE_CATEGORY=8C50&SO_SITE_ALLOW_LSA_INDICATOR=TRUE&WDS_SERVICING_FLOW_TE_MEAL=TRUE&WDS_AVD_SEL_FLIGHTS=TRUE&WDS_CAL_RANGE=15&WDS_SERVICING_FLOW_TE_FBAG=TRUE&WDS_SHOW_INVINFO=FALSE&WDS_BOOKING_FLOW_TE_MEAL=TRUE&WDS_ACTIVATE_APP_FOR_CC_MOP=TRUE&PRICING_TYPE=C&WDS_SHOW_TAXES=TRUE&B_LOCATION_1=ARN&WDS_FO_IATA=23494925&WDS_SHOW_ADDCAL=TRUE&WDS_INST_LIST=SAScDE%3Bklarna-SAScDE%3Bklarna_nt&WDS_USE_FQN=TRUE&WDS_ACTIVATE_APP_FOR_ALL_MOP=FALSE&COMMERCIAL_FARE_FAMILY_1=SKSTDA&WDS_CHECKIN_NOTIF=FALSE&TRIP_TYPE=R&WDS_HELPCONTACTURL=http%3A%2F%2Fclassic.sas.se%2Fen%2Fmisc%2FArkiv%2Fcontact-sia-%2F&WDS_SAS_CREDITS=TRUE&WDS_ANCILLARIES=FALSE&WDS_BOOKING_FLOW_TE_FBAG=TRUE&WDS_CC_LIST=AX-SAS%2FERETAIL_DE-true%3ACA-SAS%2FERETAIL_DE-true%3AVI-SAS%2FERETAIL_DE-true%3ADC-SAS%2FERETAIL_DE-false%3ADS-SAS%2FERETAIL_DE-true%3ATP-SAS%2FERETAIL_DE-false&WDS_SASCPCTRANGE=2-6&WDS_SHOW_AB=TRUE&WDS_FOID_EXCL_LIST=DK&DATE_RANGE_VALUE_1=1&WDS_SERVICING_FLOW_TE_SEATMAP=TRUE&DATE_RANGE_VALUE_2=1&WDS_BOOKING_FLOW_TE_XBAG=TRUE&WDS_POINTS_EARNED=FALSE&WDS_ORIGIN_SITE=DE&WDS_SHOW_CMP_CODE=TRUE&TRAVELLER_TYPE_1=ADT&WDS_NEWSLETTER_FCO=FALSE&B_LOCATION_2=LHR&WDS_BOOKING_FLOW_TE_SEATMAP=TRUE&WDS_TIME_OPTION=True&WDS_FRAS=TRUE&DISPLAY_TYPE=2&WDS_MOBILE_NEW_DESIGN=TRUE&WDS_SERVICING_FLOW_TE_XBAG=TRUE&WDS_SHOW_MINISEARCH=LINK&B_DATE_1=202011050000&B_DATE_2=202011120000&E_LOCATION_2=ARN&E_LOCATION_1=LHR&WDS_EBMS_CAMPAIGN=TRUE&DATE_RANGE_QUALIFIER_2=C&DATE_RANGE_QUALIFIER_1=C&WDS_INSTPAY=TRUE&ENCT=1&ENC=" + enc + "&__PREVIOUSPAGE=EOuVgEVGcPaooWlcQzY7uwfysikykaVpb-H5wZ3xp_fcVkbM_4Y3Yh3_OEwpzEWi5gOj_s80sjeP-1yYWe-Fp-6rsY8xAKiOA8--sL0aS3jICz0W0&__VIEWSTATE=%2FwEPDwUKMTE1MTc0MDk0N2RkuN2qfxyKJHLW%2BuU0D7%2BB8ZTdGMU%3D&__VIEWSTATEGENERATOR=BAA3076B";
            request.AddParameter("text/xml", requestQuery, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //class="airport"> <span>[A-Z]+<\/span>
            string queryForAirports = "class=\"airport\"> <span>[A-Z]+<\\/span>";
            //class="time">\d+:\d+<
            string queryForTimes = "class=\"time\">\\d+:\\d+<";
            //price':'[0-9]+.[0-9]+'
            string queryForPrices = "price':'[0-9]+.[0-9]+'";
            //tax':'[0-9]+.[0-9]+'
            string queryForTaxes = "tax':'[0-9]+.[0-9]+'";

            Regex airports = new Regex(queryForAirports);
            Regex times = new Regex(queryForTimes);
            Regex prices = new Regex(queryForPrices);
            Regex taxes = new Regex(queryForTaxes);

            MatchCollection airportsmatch = airports.Matches(response.Content);
            MatchCollection timesmatch = times.Matches(response.Content);
            MatchCollection pricesmatch = prices.Matches(response.Content);
            MatchCollection taxessmatch = taxes.Matches(response.Content);

            creatingObjectAndAddingToList(airportsmatch, timesmatch, pricesmatch, taxessmatch);
        }

        private void creatingObjectAndAddingToList(MatchCollection airportM, MatchCollection timeM, MatchCollection priceM, MatchCollection taxM)
        {
            for (int i = 0; i < airportM.Count; i = i + 2)
            {

            }
        }
    }
}
