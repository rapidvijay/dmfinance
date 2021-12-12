using ModificationHousingFeesAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModificationHousingFeesAdmin.ViewModel
{
    public class RequestModifyHousingFeesViewModel
    {

        public enum Stauts { PendingReview, PendingDewa, Completed, Rejected };

        public int Id { get; set; }
        public string RequestorType { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<int> NumberOfRooms { get; set; }
        public string EmailAddress { get; set; }
        public string PropertyType { get; set; }
        public string PremiseNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Remarks { get; set; }
        public string MakaniNumber { get; set; }
        public string Xcoordinator { get; set; }
        public string Ycoordinator { get; set; }
        public string PlotNumber { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string PointOfSample { get; set; }
        public string Status { get; set; }
        public string TransCount { get; set; }


        public string CopyOfDewaBill_FileName { get; set; }
        public string LetterFromOwnerMentioningMeters_FileName { get; set; }
        public string CopyOfEjariCertificate_FileName { get; set; }
        public string TitleDeed_FileName { get; set; }
        public string LayoutAttachment_FileName { get; set; }


        public string CreatedBy { get; set; }
        public System.DateTime Creation_Date { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> Updated_Date { get; set; }
        public string RejectionReason { get; set; }


        public byte[] CopyOfDewaBill { get; set; }
        public byte[] LetterFromOwnerMentioningMeters { get; set; }
        public byte[] CopyOfEjariCertificate { get; set; }
        public byte[] LayoutAttachmentBinary { get; set; }
        public byte[] TitleDeed { get; set; }

        public string MimeTypeCopyOfDewaBill { get; set; }
        public string MimeTypeLetterFromOwnerMentioningMeters { get; set; }
        public string MimeTypeCopyOfEjariCertificate { get; set; }
        public string MimeTypeLayout { get; set; }
        public string MimeTypeTitleDeed { get; set; }

        public string approvedReviewerUser { get; set; }
        public string rejectedByUser { get; set; }
        public string lockedByUser { get; set; }
        public string approvedManualUpdateByUser { get; set; }

        public string Employee_Comment { get; set; }

        public Nullable<int> Locked_By_User_Id { get; set; }

        public string employeeRemarks { get; set; }

        public Nullable<int> PropertyId { get; set; }
        public Nullable<int> AreaId { get; set; }
        public Nullable<int> communityPlaceId { get; set; }
        public string OwnerName { get; set; }
        public string EjariNumber { get; set; }
        public string ContractAddress { get; set; }
        public string ContractEndDate { get; set; }
        public string EjariRegisteredBy { get; set; }
        public string HousingFees { get; set; }
        public virtual Area Area { get; set; }
        public virtual Community_Places Community_Places { get; set; }
        public virtual Property_Types Property_Types { get; set; }

        public bool isStatusNew()
        {
            if (Stauts.PendingReview.ToString() == Status)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isStatusReviewed()
        {
            if (Stauts.PendingDewa.ToString() == Status)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool isStatusCompleted()
        {
            if (Stauts.Completed.ToString() == Status)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool isStatusRejected()
        {
            if (Stauts.Rejected.ToString() == Status)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    
    }
   
    [Serializable()]
    public class UserDetails
    {
        public int id { get; set; }
        public string password { get; set; }
        public string USER_NAME { get; set; }
        public string NAME_ARABIC { get; set; }
        public string NAME_ENGLISH { get; set; }
        public string EMAIL { get; set; }
        public bool Has_Review_Role { get; set; }
        public bool Has_Manul_Update_Role_In_Dewa { get; set; }
        public string Created_By { get; set; }
        public Nullable<System.DateTime> Creation_Date { get; set; }
        public string Updated_By { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
        public bool Has_Admin_Role { get; set; }
        public Nullable<System.DateTime> Last_Login { get; set; }
    }

}