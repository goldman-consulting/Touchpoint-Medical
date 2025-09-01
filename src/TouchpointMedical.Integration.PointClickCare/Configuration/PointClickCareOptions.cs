using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare.Configuration
{
    public class PointClickCareOptions
    {
        private readonly Dictionary<string, List<string>> DefaultEventMap = new()
        {
                { "ADT01",
                    ["patient.admit", "patient.readmit", "patient.cancelAdmit", "patient.discharge", "patient.cancelDischarge", "patient.transfer", "patient.cancelTransfer", "patient.updateResidentInfo"]
                },
                { "ADT02",
                    ["patient.leave", "patient.returnFromLeave", "patient.cancelLeave", "patient.cancelReturnFromLeave"]
                },
                { "ADT03",
                    ["patient.preAdmit"]
                },
                { "ADT04",
                    ["patient.outpatientToInpatient", "patient.inpatientToOutpatient"]
                },
                { "ADT05",
                    ["patient.updateContactInfo"]
                },
                { "ADT06",
                    ["patient.updateAccount", "patient.updateAccountOnReturnFromLeave"]
                },
                { "ADT07",
                    ["patient.updateHIEPrivacyConsent"]
                },
                { "ALL01",
                    ["allergy.add", "allergy.strikeout"]
                },
                { "ALL02",
                    ["allergy.update"]
                },
                { "MED01",
                    ["medication.add", "medication.discontinue", "medication.strikeout"]
                },
                { "MED02",
                    ["medication.cancelDiscontinue", "medication.update"]
                }
            };

        public HttpClientOptions HttpClientOptions { get; init; } = new HttpClientOptions();

        public required string AppName { get; set; }

        public required string AppKey { get; set; }

        public required string AppKeySecret { get; set; }

        public required string AuthUri { get; set; } = "https://connect.pointclickcare.com/auth/token";

        public required string BaseUri { get; set; } = "https://connect2.pointclickcare.com/api/public/preview1";

        public required string ClientCertificateDataPath { get; set; }

        public required string ClientCertificatePassword { get; set; }

        public required string WebhookDomain { get; set; } = "gateway.touchpointmed.io";

        public required string WebhookPort { get; set; } = "443";

        public required string WebhookUsername { get; set; }

        public required string WebhookPassword { get; set; }

        public List<string> WebhookEventTypeList { get; set; } =
            [
                //ADT01
                "patient.admit", "patient.readmit", "patient.cancelAdmit", "patient.discharge", "patient.cancelDischarge", "patient.transfer", "patient.cancelTransfer", "patient.updateResidentInfo", 
            
                //ADT02
                "patient.leave", "patient.returnFromLeave", "patient.cancelLeave", "patient.cancelReturnFromLeave",
        
                //ADT03
                "patient.preAdmit",

                //ADT04
                "patient.outpatientToInpatient", "patient.inpatientToOutpatient",

                //ALL01
                "allergy.add", "allergy.strikeout",

                //ALL02
                "allergy.update",

                //MED01
                "medication.add", "medication.discontinue", "medication.strikeout",

                //MED02
                "medication.cancelDiscontinue", "medication.update"
        ];

        public Dictionary<string, List<string>>? WebhookEventMap { get; set; }

        public string? GetEventGroup(string eventType)
        {
            var eventGroup = WebhookEventMap?.SingleOrDefault(kvp => kvp.Value.Contains(eventType)).Key;

            if (string.IsNullOrEmpty(eventGroup))
            {
                eventGroup = DefaultEventMap.SingleOrDefault(kvp => kvp.Value.Contains(eventType)).Key;
            }

            return eventGroup;
        }

    }

}
