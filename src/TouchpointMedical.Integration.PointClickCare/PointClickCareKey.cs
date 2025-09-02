using TouchpointMedical.Logging;

namespace TouchpointMedical.Integration.PointClickCare
{
    public class PointClickCareKey
    {
        private readonly PointClickCareKeyType _keyType;
        private readonly string _orgUuid;
        private readonly long? _facilityId;
        private readonly long? _residentId;
        private readonly long? _contactId;

        private PointClickCareKey(
            PointClickCareKeyType keyType,
            string orgUuid,
            long? facilityId,
            long? residentId = default,
            long? contactId = default) 
        {

            _keyType = Validate(keyType, facilityId, residentId, contactId);

            _orgUuid = orgUuid;
            _facilityId = facilityId;
            _residentId = residentId;   
            _contactId = contactId;
        }

        public PointClickCareKeyType Type { get { return _keyType; } }
        public string OrgId { get { return _orgUuid; } }
        public long? FacilityId { get { return Type != PointClickCareKeyType.Organization ? _facilityId : default; } }

        [TouchpointLogMasked]
        public long? ResidentId { get { return
                    (Type == PointClickCareKeyType.Resident 
                        || Type == PointClickCareKeyType.ResidentContact) ? 
                    _residentId : default; } }

        public override string ToString()
        {
            return ToString(_keyType);
        }

        public string ToString(PointClickCareKeyType keyType)
        {
            return keyType switch
            {
                PointClickCareKeyType.Organization => MakeFormattedKey(
                    PointClickCareKeyType.Organization,
                    [_orgUuid]),

                PointClickCareKeyType.Facility => MakeFormattedKey(
                    PointClickCareKeyType.Facility,
                    [_orgUuid, _facilityId!]),

                PointClickCareKeyType.Resident => MakeFormattedKey(
                    PointClickCareKeyType.Resident,
                    [_orgUuid, _facilityId!, _residentId!]),

                PointClickCareKeyType.ResidentContact => MakeFormattedKey(
                    PointClickCareKeyType.ResidentContact,
                    [_orgUuid, _facilityId!, _residentId!, _contactId!]),

                _ => MakeFormattedKey(
                    PointClickCareKeyType.NotSet,
                    [_orgUuid, _facilityId, _residentId, _contactId]),
            };
        }


        public static implicit operator PointClickCareKey(string pointClickCareKey)
        {
            return FromKey(pointClickCareKey);
        }

        public static implicit operator string(PointClickCareKey pointClickCareKey)
        {
            return pointClickCareKey.ToString();
        }

        #region Static Create Methods

        //Create the object from an existing key value
        public static PointClickCareKey FromKey(string key)
        {
            static PointClickCareKeyType getKeyTypeFromPrefix(string keyPrefix)
            {
                return keyPrefix switch
                {
                    "PCCO" => PointClickCareKeyType.Organization,
                    "PCCF" => PointClickCareKeyType.Facility,
                    "PCCR" => PointClickCareKeyType.Resident,
                    "PCCC" => PointClickCareKeyType.ResidentContact,
                    _ => PointClickCareKeyType.NotSet,
                };
            }

            var keyParts = key.Split(['-'], 2);

            PointClickCareKeyType keyType = 
                keyParts.Length > 0 
                ? getKeyTypeFromPrefix(keyParts[0]) 
                : throw new ArgumentException("Missing required PointClickCareKey Prefix value.");

            var remainingParts = keyParts.Length > 1 ? keyParts[1].Split([':']) : throw new ArgumentException("Missing required PointClickCareKey components.");

            string orgUuid =
                remainingParts.Length > 0
                ? remainingParts[0]
                : throw new ArgumentException("Missing required PointClickCareKey OrgUuid value");

            long? facilityId = 
                remainingParts.Length > 1 
                ? long.Parse( remainingParts[1]) 
                : null;

            long? residentId =
                remainingParts.Length > 2
                ? long.Parse(remainingParts[2])
                : null;

            long? contactId =
                remainingParts.Length > 3
                ? long.Parse(remainingParts[3])
                : null;

            return new PointClickCareKey(
                keyType, orgUuid, facilityId, residentId, contactId);
        }

        //Create the Org Key object from the parts
        public static PointClickCareKey CreateOrganizationKey(string orgUuid)
        {
            return new PointClickCareKey(
                PointClickCareKeyType.Organization, orgUuid, null, null, null);
        }

        //Create the Facility Key object from the parts
        public static PointClickCareKey CreateFacilityKey(
            string orgUuid,
            long facilityId)
        {
            return new PointClickCareKey(
                PointClickCareKeyType.Facility, orgUuid, facilityId, null, null);
        }

        //Create the Resident Key object from the parts
        public static PointClickCareKey CreateResidentKey(
            string orgUuid,
            long facilityId,
            long residentId)
        {
            return new PointClickCareKey(
                PointClickCareKeyType.Resident, orgUuid, facilityId, residentId, null);
        }

        //Create the Resident Contact Key object from the parts
        public static PointClickCareKey CreateResidentContactKey(
            string orgUuid,
            long facilityId,
            long residentId,
            long contactId)
        {
            return new PointClickCareKey(
                PointClickCareKeyType.ResidentContact, orgUuid, facilityId, residentId, contactId);
        }

        //Create the Resident Contact Key object from the parts
        public static PointClickCareKey CreateKey(
            string orgUuid,
            long? facilityId,
            long? residentId,
            long? contactId)
        {
            PointClickCareKeyType keyType = PointClickCareKeyType.NotSet;

            return new PointClickCareKey(
                keyType, orgUuid, facilityId, residentId, contactId);
        }

        #endregion

        private static PointClickCareKeyType Validate(
            PointClickCareKeyType keyType,
            long? facilityId,
            long? residentId = default,
            long? contactId = default)
        {

            static bool validateNullArgument(
                bool isMissing,
                string argumentName,
                PointClickCareKeyType pointClickCareKeyType)
            {
                if (isMissing)
                {
                    throw new ArgumentNullException(argumentName,
                        $"{argumentName} is required for PointClickCare {pointClickCareKeyType.ToDisplayName()} key.");
                }

                return true;
            }

            //Find the type if it has not already been set.
            if (keyType == PointClickCareKeyType.NotSet)
            {
                if (contactId.HasValue)
                {
                    keyType = PointClickCareKeyType.ResidentContact;
                }
                else if (residentId.HasValue)
                {
                    keyType = PointClickCareKeyType.Resident;
                }
                else if (facilityId.HasValue)
                {
                    keyType = PointClickCareKeyType.Facility;
                }
                else
                {
                    keyType = PointClickCareKeyType.Organization;
                }

            }

            switch (keyType)
            {
                case PointClickCareKeyType.Facility:

                    validateNullArgument(
                        !facilityId.HasValue, nameof(facilityId), keyType);

                    break;
                case PointClickCareKeyType.Resident:
                    validateNullArgument(
                        !residentId.HasValue, nameof(residentId), keyType);

                    validateNullArgument(
                        !facilityId.HasValue, nameof(facilityId), keyType);

                    break;
                case PointClickCareKeyType.ResidentContact:
                    validateNullArgument(
                        !contactId.HasValue, nameof(contactId), keyType);

                    validateNullArgument(
                        !residentId.HasValue, nameof(residentId), keyType);

                    validateNullArgument(
                        !facilityId.HasValue, nameof(facilityId), keyType);

                    break;
            }

            return keyType;
        }

        private static string MakeFormattedKey(PointClickCareKeyType type, object?[] segments)
        {
            var prefix = "PCC";
            var format = "{0}-{1}:{2}:{3}:{4}";

            switch (type)
            {
                case PointClickCareKeyType.Organization:
                    format = "{0}-{1}";
                    prefix += "O";
                    break;
                case PointClickCareKeyType.Facility:
                    format = "{0}-{1}:{2}";
                    prefix += "F";
                    break;
                case PointClickCareKeyType.Resident:
                    format = "{0}-{1}:{2}:{3}";
                    prefix += "R";
                    break;
                case PointClickCareKeyType.ResidentContact:
                    prefix += "C";
                    break;
                default:
                    prefix = "PCCX";
                    break;
            }

            return string.Format(format, [prefix, ..segments]);
        }
    }
}
