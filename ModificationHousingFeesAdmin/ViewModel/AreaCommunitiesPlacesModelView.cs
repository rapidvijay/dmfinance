using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModificationHousingFeesAdmin.ViewModel
{
    public class AreaCommunitiesPlacesModelView
    {
        public int Id { get; set; }

        public string areaArabicName { get; set; }

        public string areaEnglishName { get; set; }
    }

    public class HousingFeesViewModel
    {
        public MtoHousingFeeResp MTO_HousingFee_Resp { get; set; }
    }


    public class MtoHousingFeeResp
    {

        public Header Header { get; set; }


        public MtoHousingFeeRespItem Item { get; set; }


        public Message Message { get; set; }
    }

    public class Header
    {

        public string ContractAccount { get; set; }


        public string ContractAccountName { get; set; }


        public string PremiseNo { get; set; }


        public string PremiseType { get; set; }


        public string OwnerName { get; set; }


        public string LegacyAcccount { get; set; }


        public string ContractAccountAddress { get; set; }


        public DateTimeOffset? TenancyContractEDate { get; set; }


        public string TenancyContractValue { get; set; }


        public string MonthlyBillAmt { get; set; }


        public string ElectricMeter { get; set; }


        public string WaterMeter { get; set; }
    }

    public class MtoHousingFeeRespItem
    {
        [JsonProperty("record")]
        [JsonConverter(typeof(SingleValueArrayConverter<Record>))]
        public List<Record> Record { get; set; }
    }

    public class Record
    {
        [JsonProperty("Item")]
        public RecordItem Item { get; set; }
    }

    public class RecordItem
    {
        [JsonProperty("BillingMonth")]
        public string BillingMonth { get; set; }

        [JsonProperty("PostingDate")]
        public DateTime? PostingDate { get; set; }

        [JsonProperty("DueDate")]
        public DateTime? DueDate { get; set; }

        [JsonProperty("Division")]
        public string Division { get; set; }

        [JsonProperty("BillingType")]
        public string BillingType { get; set; }

        [JsonProperty("BilledAmount")]
        public string BilledAmount { get; set; }

        [JsonProperty("PaidAmount")]
        public string PaidAmount { get; set; }
    }

    public class Message
    {
        [JsonProperty("ContractAccount")]
        public string ContractAccount { get; set; }

        [JsonProperty("Desc")]
        public string Desc { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
    }





    public class EjariDetailsViewModel
    {
        public GetContractDetailsByNumberResponse GetContractDetailsByNumberResponse { get; set; }
    }

    public class GetContractDetailsByNumberResponse
    {

        public GetContractDetailsByNumberResult GetContractDetailsByNumberResult { get; set; }
    }

    public class GetContractDetailsByNumberResult
    {

        public AssociatedContractDetails AssociatedContractDetails { get; set; }


        public AssociatedLessorCompany AssociatedLessorCompany { get; set; }

        public object AssociatedLessorPerson { get; set; }


        public AssociatedOwnerCompany AssociatedOwnerCompany { get; set; }


        public AssociatedOwnerPerson AssociatedOwnerPerson { get; set; }


        public AssociatedPropertyDetails AssociatedPropertyDetails { get; set; }


        public AssociatedReceiptDetails AssociatedReceiptDetails { get; set; }

        public object AssociatedSubLessorCompany { get; set; }

        public object AssociatedSubLessorPerson { get; set; }


        public AssociatedTenantCompany AssociatedTenantCompany { get; set; }


        public AssociatedTenantPerson AssociatedTenantPerson { get; set; }


        public string ContractId { get; set; }


        public string ContractNumber { get; set; }

        public string ContractStatus { get; set; }

        public string ContractType { get; set; }

        public string ContractVersion { get; set; }

        public bool IsSubLease { get; set; }

        public string RegisteredBy { get; set; }

        public string TenantType { get; set; }
    }

    public class AssociatedContractDetails
    {
        public string ActualAnnualAmount { get; set; }

        public string ActualContractAmount { get; set; }

        public string AnnualRentAmount { get; set; }

        public string DiscountAmount { get; set; }

        public string DiscountTypeId { get; set; }

        public string EndDate { get; set; }

        public string GraceEndDate { get; set; }

        public string GraceStartDate { get; set; }

        public string IsShellCore { get; set; }

        public string RegistrationDate { get; set; }

        public string RentAmount { get; set; }

        public string SecurityDeposite { get; set; }

        public string StartDate { get; set; }
    }

    public class AssociatedOwnerCompany
    {
        public string Address { get; set; }

        public string ContactPersonCountryId { get; set; }

        public string ContactPersonNameAr { get; set; }

        public string ContactPersonNameEn { get; set; }

        public string Email { get; set; }

        public string Fax { get; set; }

        public string LicenseExpiryDate { get; set; }
        public string LicenseIssueDate { get; set; }

        public string LicenseIssuerId { get; set; }

        public string Mobile { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string OwnerNumber { get; set; }

        public string PoBox { get; set; }

        public string Phone { get; set; }

        public string TradeLicenseNumber { get; set; }
    }

    public class AssociatedPropertyDetails
    {
        public string ApprovalRefNumber { get; set; }

        public string BuildingCountOfFloors { get; set; }

        public string BuildingCountOfUnits { get; set; }

        public string BuildingName { get; set; }

        public string BuildingNameAr { get; set; }

        public string BuildingType { get; set; }

        public string BuildingUsage { get; set; }

        public string BuiltArea { get; set; }

        public string EjariPropertyId { get; set; }

        public GeoLocation GeoLocation { get; set; }

        public string LandArea { get; set; }

        public string LandAreaId { get; set; }

        public string LandDmNumber { get; set; }

        public string LandNumber { get; set; }

        public string LandSize { get; set; }

        public string LandUsage { get; set; }

        public string LandUsageId { get; set; }

        public string MainDewa { get; set; }

        public string MakaniNumber { get; set; }

        public PropertiesList PropertiesList { get; set; }

        public string PropertySubTypeId { get; set; }

        public string PropertyType { get; set; }

        public string PropertyTypeId { get; set; }

        public string PropertyUsageId { get; set; }

        public string UaengNumber { get; set; }
    }




    public class GeoLocation
    {
        public string Latitude { get; set; }

        public string Longitude { get; set; }
    }

    public class PropertiesList
    {
        [JsonConverter(typeof(SingleValueArrayConverter<PropertyListItem>))]
        public List<PropertyListItem> PropertyListItem { get; set; }
    }

    public class PropertyListItem
    {
        public string ApprovalRefNumber { get; set; }

        public string Dewa { get; set; }

        public string EjariPropertyId { get; set; }

        public string PropertyNameAr { get; set; }

        public string PropertyNameEn { get; set; }

        public string PropertyNumber { get; set; }

        public string PropertySubType { get; set; }

        public string PropertySubTypeId { get; set; }

        public string PropertyType { get; set; }

        public string PropertyTypeId { get; set; }

        public string PropertyUsageId { get; set; }

        public string Size { get; set; }

        public string Usage { get; set; }
    }

    public class AssociatedReceiptDetails
    {
        public string Amount { get; set; }

        public Fees Fees { get; set; }

        public string Number { get; set; }
    }

    public class Fees
    {
        [JsonConverter(typeof(SingleValueArrayConverter<FeesDetail>))]
        public List<FeesDetail> FeesDetails { get; set; }
    }

    public class FeesDetail
    {
        public string Amount { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }
    }

    public class AssociatedTenantCompany
    {
        public string Address { get; set; }

        public string ContactPersonCountryId { get; set; }

        public string ContactPersonNameAr { get; set; }

        public string ContactPersonNameEn { get; set; }

        public string DedReferenceType { get; set; }

        public string Email { get; set; }

        public string Fax { get; set; }

        public string InitialApprovalNumber { get; set; }

        public string LicenseExpiryDate { get; set; }

        public string LicenseIssueDate { get; set; }

        public string LicenseIssuer { get; set; }

        public string LicenseIssuerId { get; set; }

        public string LicenseNumber { get; set; }

        public string LicenseState { get; set; }

        public string Mobile { get; set; }

        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string PoBox { get; set; }

        public string Phone { get; set; }

        public string TenantNo { get; set; }
    }

    public class AssociatedTenantPerson
    {
        public string Address { get; set; }
        public string CountryId { get; set; }
        public string DEDReferenceType { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string EmiratesID { get; set; }
        public string Fax { get; set; }
        public string InitialApprovalNumber { get; set; }
        public string IsTradelicense { get; set; }
        public string LicenseIssuer { get; set; }
        public string LicenseNumber { get; set; }
        public int Mobile { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Nationality { get; set; }
        public string POBox { get; set; }
        public string PassportExpiryDate { get; set; }
        public string PassportIssueDate { get; set; }
        public string PassportIssuePlaceAr { get; set; }
        public string PassportIssuePlaceEn { get; set; }
        public string PassportNumber { get; set; }
        public string Phone { get; set; }
        public string PlaceOfBirthAr { get; set; }
        public string PlaceOfBirthEn { get; set; }
        public string TenantNumber { get; set; }
        public string UniqueNumber { get; set; }
        public string VisaExpiryDate { get; set; }
        public string VisaIssueDate { get; set; }
        public string VisaNumber { get; set; }
    }


    public class AssociatedLessorCompany
    {
        public string Address { get; set; }
        public string ContactPersonCountryId { get; set; }
        public string ContactPersonNameAr { get; set; }
        public string ContactPersonNameEn { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string LicenseIssueDate { get; set; }
        public string LicenseIssuer { get; set; }
        public string LicenseIssuerId { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseState { get; set; }
        public string Mobile { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string POBox { get; set; }
        public string Phone { get; set; }
    }

    public class AssociatedOwnerPerson
    {
        public string Address { get; set; }
        public string CountryId { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string EmiratesID { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Nationality { get; set; }
        public string OwnerNumber { get; set; }
        public string POBox { get; set; }
        public string PassportExpiryDate { get; set; }
        public string PassportIssueDate { get; set; }
        public string PassportIssuePlaceAr { get; set; }
        public string PassportIssuePlaceEn { get; set; }
        public string Phone { get; set; }
        public string PlaceOfBirthAr { get; set; }
        public string PlaceOfBirthEn { get; set; }
        public string UniqueNumber { get; set; }
        public string VisaExpiryDate { get; set; }
        public string VisaIssueDate { get; set; }
        public string VisaNumber { get; set; }
    }
}

public class SingleValueArrayConverter<T> : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        object retVal = new Object();
        if (reader.TokenType == JsonToken.StartObject)
        {
            T instance = (T)serializer.Deserialize(reader, typeof(T));
            retVal = new List<T>() { instance };
        }
        else if (reader.TokenType == JsonToken.StartArray)
        {
            retVal = serializer.Deserialize(reader, objectType);
        }
        return retVal;
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}