using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Alexa.Custom.Skills.API.Logging
{
    public sealed class AppInsightHelper
    {
        private static AppInsightHelper _instance;
        private AppInsightHelper() { }

        public static AppInsightHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppInsightHelper();
                }
                return _instance;
            }
        }
        private static TelemetryClient telemetryClient;
        public void AppInsightInit(AppInsightPayload _payload)
        {

            IServiceCollection services = new ServiceCollection();

            services.AddApplicationInsightsTelemetryWorkerService("{Application Insight Key}");

            IServiceProvider serviceProvider = services.BuildServiceProvider();            

            telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

            using (telemetryClient.StartOperation<RequestTelemetry>(_payload._operation))
            {               
                
                if (_payload._type.CompareTo(AppInsightLanguage.AppInsightEvent) == 0)
                {
                    EventTelemetry _evt = new EventTelemetry();                    
                    _evt.Context.Operation.Id = _payload._correlationId;
                    _evt.Name = _payload._payload;    
                    telemetryClient.TrackEvent(_payload._payload);
                }                    
                if (_payload._type.CompareTo(AppInsightLanguage.AppInsightTrace) == 0)
                {
                    TraceTelemetry _trct = new TraceTelemetry();
                    _trct.Message = _payload._payload;
                    _trct.Context.Operation.Id = _payload._correlationId;

                    telemetryClient.TrackTrace(_trct);
                }                    
                if (_payload._type.CompareTo(AppInsightLanguage.AppInsightException) == 0)
                {
                    ExceptionTelemetry _et = new ExceptionTelemetry();
                    _et.Exception = _payload._ex;
                    _et.Context.Operation.Id = _payload._correlationId;                    
                    telemetryClient.TrackException(_et);
                }
                    
            }
            telemetryClient.Flush();

        }
    }
    public enum AppInsightLanguage
    {
        AppInsightEvent,
        AppInsightTrace,
        AppInsightException
    }
}
