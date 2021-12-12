

var app = angular.module('App', ['ui.bootstrap']);


app.controller('submitcontroller', function ($scope, $http, $uibModal) {


    //Dewa Integration
    $scope.fillHousingFees = function () {

        var accountnumber = $("#AccountNumber").val();
        if (accountnumber == null || accountnumber == "") {
            alert("Please enter accountnumber");
            return;
        }
        $('body').loadingModal({ text: 'loading...' });
        var postFillHousingFees = $http({
            method: "Post",
            url: "/Home/getHousingFeesDetails?lang=" + "en" + "&accountNumber=" + accountnumber,
            dataType: 'json',
            data: {},
            headers: { "Content-Type": "application/json" }
        }).then(function successCallback(response) {
            $('body').loadingModal('destroy');

            if (response.status == 200) { }
            //handling the response sent by server
            if (response.data != null) {

                if (response.data.MTO_HousingFee_Resp != null) {


                    if (response.data.MTO_HousingFee_Resp.Message.Status == "S") {
                        console.log(response.data);
                        $scope.items = response.data;
                        $scope.PremiseNumber = response.data.MTO_HousingFee_Resp.Header.LegacyAcccount;
                        //  $scope.EjariNumber = response.data.MTO_HousingFee_Resp.Header.LegacyAcccount;
                        var mydate = new Date(response.data.MTO_HousingFee_Resp.Header.TenancyContractEDate.match(/\d+/) * 1).toLocaleDateString();
                        $scope.InitContractEndDate = mydate.toString();
                        $scope.ContractEndDate = mydate;
                        $scope.ContractAddress = response.data.MTO_HousingFee_Resp.Header.ContractAccountAddress;
                        var fullstring = response.data.MTO_HousingFee_Resp.Header.OwnerName;
                        $scope.OwnerName = fullstring.split("/")[0];
                        if (response.data.MTO_HousingFee_Resp.Item != null) {
                            if (response.data.MTO_HousingFee_Resp.Item.Record != null) {
                                $scope.HousingFees = response.data.MTO_HousingFee_Resp.Item.Record[0].Item.BilledAmount;
                            }
                        }
                        console.log(response.data.MTO_HousingFee_Resp.Header.ContractAccount);
                        console.log('opening pop up');
                        var uibModalInstance = $uibModal.open({
                            templateUrl: '/Content/html/DewaPopUp.html',
                            controller: 'Viewcontroller',
                            resolve: {
                                items: function () {
                                    return $scope.items;
                                }
                            }
                        });


                        uibModalInstance.result.then(function (selectedItem) {
                            $scope.selected = selectedItem;
                        }, function () {
                           // $log.info('Modal dismissed at: ' + new Date());
                        });

                    }
                    else if (response.data.MTO_HousingFee_Resp.Message.Status == "E") {
                        $scope.PremiseNumber = "";
                        $scope.ContractEndDate = "";
                        $scope.ContractAddress = "";
                        $scope.OwnerName = "";
                        $scope.accountNumber = "";
                        $scope.HousingFees = "";
                        alert(response.data.MTO_HousingFee_Resp.Message.Desc);
                    }
                }
                else {
                    $('body').loadingModal('destroy'); alert('No Record Found for the given details');
                }

            }
        }, function errorCallback() { $('body').loadingModal('destroy'); alert('No Record Found for the given details'); });
        //postFillHousingFees.then(function successCallback(data, status) {
        //    $('body').loadingModal('destroy');
        

        //}, function errorCallback(response) {

        //});


    }

    $scope.fillEjariDetails = function () {
        var ejariNumber = $("#EjariNumber").val();
        if (ejariNumber == null || ejariNumber == "") {
            error('Please enter Ejari Number')
            return;
        }


        $('body').loadingModal({ text: 'loading...' });
        var postEjariDetails = $http({
            method: "Post",
            url: "/Home/getEjariDetails?lang=" + "en" + "&ejariNumber=" + ejariNumber,
            dataType: 'json',
            data: {},
            headers: { "Content-Type": "application/json" }
        }).then(function successCallback(response) {
            if (response.status == 200) { }
            $('body').loadingModal('destroy');
            if (response.data != null) {
                if (response.data.GetContractDetailsByNumberResponse != null) {
                    console.log(response.data);
                    $scope.EjariRegisteredBy = response.data.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.RegisteredBy;
                    $scope.items = response.data;
                    var uibModalInstance = $uibModal.open({
                        templateUrl: '/Content/html/EjariPopUp.html',
                        controller: 'ViewEjaricontroller',
                        resolve: {
                            items: function () {
                                return $scope.items;
                            }
                        }
                    });


                    uibModalInstance.result.then(function (selectedItem) {
                        $scope.selected = selectedItem;
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                }
                else if (response.data.GetContractDetailsByNumberResponse == null) {
                    $scope.EjariRegisteredBy = "";


                    alert('No Record Found for the given details');


                }

            } else { $('body').loadingModal('destroy'); alert('No Record Found for the given details'); }

            //handling the response sent by server
        }, function errorCallback() { $('body').loadingModal('destroy'); });
        //postEjariDetails.then(function successCallback(data, status){
        //    $('body').loadingModal('destroy');
        //    if (data != null) {
        //        if (data.data.GetContractDetailsByNumberResponse != null) {
        //            console.log(data);
        //            $scope.EjariRegisteredBy = data.data.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.RegisteredBy;
        //            $scope.items = data;
        //            var uibModalInstance = $uibModal.open({
        //                templateUrl: '/Content/html/EjariPopUp.html',
        //                controller: 'ViewEjaricontroller',
        //                resolve: {
        //                    items: function () {
        //                        return $scope.items;
        //                    }
        //                }
        //            });


        //            uibModalInstance.result.then(function (selectedItem) {
        //                $scope.selected = selectedItem;
        //            }, function () {
        //                $log.info('Modal dismissed at: ' + new Date());
        //            });
        //        }
        //        else if (data.dataGetContractDetailsByNumberResponse == null) {
        //            $scope.EjariRegisteredBy = "";


        //            alert('No Record Found for the given details');


        //        }

        //    } else { alert('No Record Found for the given details') }
        
          

            
        //}, function errorCallback(response) { } );


    }


});


    app.controller('Viewcontroller', function ($scope, $uibModalInstance, items) {
        $scope.PremiseNo = items.MTO_HousingFee_Resp.Header.LegacyAcccount;
        $scope.PremiseType = items.MTO_HousingFee_Resp.Header.PremiseType;
        $scope.AccountNo = items.MTO_HousingFee_Resp.Header.ContractAccount;
        $scope.LegacyAct = items.MTO_HousingFee_Resp.Header.LegacyAcccount;
        $scope.TenancyContractValue = items.MTO_HousingFee_Resp.Header.TenancyContractValue;

        $scope.ContractAccountName = items.MTO_HousingFee_Resp.Header.ContractAccountName;
        $scope.ContractAccAdd = items.MTO_HousingFee_Resp.Header.ContractAccountAddress;
        var fullstring = items.MTO_HousingFee_Resp.Header.OwnerName;
        $scope.OwnerName = fullstring.split("/")[0];
        $scope.PObox = fullstring.split("/")[1];
     
        var mydate = new Date(items.MTO_HousingFee_Resp.Header.TenancyContractEDate.match(/\d+/) * 1).toLocaleDateString();
        $scope.ContractEDate = mydate;
        if (items.MTO_HousingFee_Resp.Item != null) {
            if (items.MTO_HousingFee_Resp.Item.Record != null) {
                if (items.MTO_HousingFee_Resp.Item.Record.length == 1 || items.MTO_HousingFee_Resp.Item.Record.length > 1) {
                    $scope.BillingMonth = items.MTO_HousingFee_Resp.Item.Record[0].Item.BillingMonth;
                    if (items.MTO_HousingFee_Resp.Item.Record[0].Item.PostingDate != null) {
                        $scope.BilledDate = new Date(items.MTO_HousingFee_Resp.Item.Record[0].Item.PostingDate.match(/\d+/) * 1).toLocaleDateString();
                    }
                    if (items.MTO_HousingFee_Resp.Item.Record[0].Item.DueDate != null) {
                        $scope.BillDueDate = new Date(items.MTO_HousingFee_Resp.Item.Record[0].Item.DueDate.match(/\d+/) * 1).toLocaleDateString();
                    }
                    $scope.BillingMonth = items.MTO_HousingFee_Resp.Item.Record[0].Item.BillingMonth;
                    $scope.AccType = items.MTO_HousingFee_Resp.Item.Record[0].Item.Division;
                    $scope.BillintType = items.MTO_HousingFee_Resp.Item.Record[0].Item.BillingType;
                    $scope.BillAmt = items.MTO_HousingFee_Resp.Item.Record[0].Item.BilledAmount;
                    $scope.PaidAmt = items.MTO_HousingFee_Resp.Item.Record[0].Item.PaidAmount;
                }
            }
            if (items.MTO_HousingFee_Resp.Item.Record.length > 1) {
                $scope.BillingMonth1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.BillingMonth;
                if (items.MTO_HousingFee_Resp.Item.Record[1].Item.PostingDate != null) {
                    $scope.BilledDate1 = new Date(items.MTO_HousingFee_Resp.Item.Record[1].Item.PostingDate.match(/\d+/) * 1).toLocaleDateString();
                }
                if (items.MTO_HousingFee_Resp.Item.Record[1].Item.DueDate != null) {
                    $scope.BillDueDate1 = new Date(items.MTO_HousingFee_Resp.Item.Record[1].Item.DueDate.match(/\d+/) * 1).toLocaleDateString();
                }
                $scope.BillingMonth1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.BillingMonth;
                $scope.AccType1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.Division;
                $scope.BillintType1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.BillingType;
                $scope.BillAmt1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.BilledAmount;
                $scope.PaidAmt1 = items.MTO_HousingFee_Resp.Item.Record[1].Item.PaidAmount;
                if (items.MTO_HousingFee_Resp.Item.Record.length > 2) {
                    $scope.BillingMonth2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.BillingMonth;
                    if (items.MTO_HousingFee_Resp.Item.Record[2].Item.PostingDate != null) {
                        $scope.BilledDate2 = new Date(items.MTO_HousingFee_Resp.Item.Record[2].Item.PostingDate.match(/\d+/) * 1).toLocaleDateString();
                    }
                    if (items.MTO_HousingFee_Resp.Item.Record[2].Item.DueDate != null) {
                        $scope.BillDueDate2 = new Date(items.MTO_HousingFee_Resp.Item.Record[2].Item.DueDate.match(/\d+/) * 1).toLocaleDateString();
                    }
                    $scope.BillingMonth2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.BillingMonth;
                    $scope.AccType2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.Division;
                    $scope.BillintType2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.BillingType;
                    $scope.BillAmt2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.BilledAmount;
                    $scope.PaidAmt2 = items.MTO_HousingFee_Resp.Item.Record[2].Item.PaidAmount;
                }
            }
        }
        $scope.items = items;
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });

app.controller('ViewEjaricontroller', function ($scope, $uibModalInstance, items) {

    if (items != null) {

        if (items.GetContractDetailsByNumberResponse != null) {
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult != null) {
                $scope.EjariRegisteredBy = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.RegisteredBy;
                $scope.EjariContractId = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.ContractId;
                $scope.EjariContractNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.ContractNumber;
                $scope.EjariContractStatus = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.ContractStatus;
                $scope.EjariContractType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.ContractType;
                $scope.EjariContractVersion = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.ContractVersion;
                $scope.EjariIsSubLease = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.IsSubLease;
                $scope.EjariTenantType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.TenantType;
            }
            $scope.items = items;
            console.log(items);
            //owner company details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerCompany != null) {
                $scope.Ejariownercompen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerCompany.NameEn;
                $scope.Ejariownercompr = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerCompany.NameAr;
                $scope.Ejariownerlcno = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerCompany.TradeLicenseNumber;
            }

            //owner  details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson != null) {
                $scope.OwnerNameen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.NameEn;
                $scope.OwnerNamear = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.NameAr;
                $scope.OwnerNationlaity = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.Nationality;
                $scope.OwnerEmail = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.Email;
                $scope.OwnerEmiratesId = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.EmiratesID;
                $scope.OwnerMobile = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.Mobile;
                $scope.OwnerNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.OwnerNumber;
                $scope.OwnerPOBox = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.POBox;
                $scope.OwnerPlaceOfBirthEn = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.PlaceOfBirthEn;
                $scope.OwnerUniqNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.UniqueNumber;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.PassportExpiryDate != null)
                {
                    $scope.OwnerPassportExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.PassportExpiryDate).toLocaleDateString();
                }
             
                $scope.OwnerPassportIssuePlaceEn = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedOwnerPerson.PassportIssuePlaceEn;
               
            }
            //contract details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails != null) {

                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.StartDate != null) {
                    $scope.CntStartDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.StartDate).toLocaleDateString();
                }
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.EndDate != null) {
                    $scope.CntEndDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.EndDate).toLocaleDateString();
                }
                $scope.Contractamt = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.AnnualRentAmount;
                $scope.Secdpt = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.SecurityDeposite;
                $scope.ActualContAmt = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.ActualContractAmount;
                $scope.AnnualContAmt = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.ActualAnnualAmount;
                $scope.RentAmt = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.RentAmount;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.RegistrationDate != null)
                {
                    $scope.RegistrationDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedContractDetails.RegistrationDate).toLocaleDateString();
                }
                
            }
            //associated tenant company details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany != null) {
                $scope.EjariTenantCompen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.NameEn;
                $scope.EjariTenantCompar = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.NameAr;
                $scope.EjariTenantComplcno = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.LicenseNumber;
                $scope.TenantCompContactPersonen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.ContactPersonNameEn;
                $scope.TenantCompContactPersonar = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.ContactPersonNameAr;
                $scope.TenantCompDedReferenceType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.DedReferenceType;
                $scope.TenantCompEmail = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.Email;
                $scope.TenantCompFax = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.Fax;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.LicenseNumber != null) {
                    if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.LicenseExpiryDate != null) {
                        $scope.TenantCompLicenseExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.LicenseExpiryDate).toLocaleDateString();

                    }
                    $scope.TenantCompLicenseIssuer = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.LicenseIssuer;
                }
                $scope.TenantCompMobile = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.Mobile;
                $scope.TenantCompPhone = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.Phone;
                $scope.TenantCompPoBox = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.PoBox;
                $scope.TenantCompTenantNo = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantCompany.TenantNo;

            }


            //associated lessor company details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany != null) {
                $scope.LessorCompNameen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.NameEn;
                $scope.LessorCompNamear = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.NameAr;
                $scope.LessorComplcno = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseNumber;
                $scope.LessorContactPersonen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.ContactPersonNameEn;
                $scope.LessorContactPersonar = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.ContactPersonNameAr;
               
                $scope.LessorEmail = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.Email;
                $scope.LessorFax = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.Fax;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseNumber != null) {
                    if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseExpiryDate != null) {
                        $scope.LessorLicenseExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseExpiryDate).toLocaleDateString();
                    }
                    if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseIssueDate != null) {
                        $scope.LessorLicenseIssueDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseIssueDate).toLocaleDateString();
                    }
                        $scope.LessorLicenseIssuer = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.LicenseIssuer;
                }
                $scope.LessorMobile = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.Mobile;
                $scope.LessorPhone = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.Phone;
                $scope.LessorPoBox = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedLessorCompany.PoBox;
                

            }

            ////associated tenant person details
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson != null) {
                $scope.AssociatedTenantnameen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.NameEn;
                $scope.AssociatedTenantnamear = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.NameAr;
                $scope.AssociatedTenantnlicno = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseNumber;
                $scope.AssociatedTenantCtpersonen = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.ContactPersonNameEn;
                $scope.AssociatedTenantCtpersonar = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.ContactPersonNameAr;
       
                $scope.TenantEmail = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Email;
                $scope.TenantFax = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Fax;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseNumber != null) {
                    if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseExpiryDate != null) {
                        $scope.TenantLicenseExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseExpiryDate).toLocaleDateString();
                    }
                        $scope.TenantLicenseIssuer = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseIssuer;
                    $scope.TenantLicenseState = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.LicenseState;
                }
                $scope.TenantMobile = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Mobile;
                $scope.TenantPhone = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Phone;
                $scope.TenantPoBox = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.POBox;
                $scope.TenantAddress = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Address;
                $scope.TenantNationality = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.Nationality;
                $scope.TenantEmiratesId = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.EmiratesID;
                $scope.TenantPassportNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.PassportNumber;
                $scope.TenantVisaNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.VisaNumber;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.VisaExpiryDate != null)
                {
                    $scope.TenantVisaExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.VisaExpiryDate).toLocaleDateString();
                }
                $scope.TenantUniqNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.UniqueNumber;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.PassportExpiryDate != null)
                {
                    $scope.TenantPassportExpiryDate = new Date(items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.PassportExpiryDate).toLocaleDateString();
                }
                $scope.TenantPassportIssuePlaceEn = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.PassportIssuePlaceEn;
                $scope.TenantNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedTenantPerson.TenantNumber;
            }

            //associated propertydetails
            if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails != null) {
                $scope.AssPropBldname = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.BuildingName;
                $scope.AssPropLandArea = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.LandArea;
                $scope.AssPropLandDMNo = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.LandDmNumber;
                $scope.AssPropLandNo = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.LandNumber;
                $scope.AssProptype = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertyType;
                $scope.AssPropuaengno = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.UaengNumber;
                if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList != null)
                {
                    if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem != null) {
                        if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem.length == 1 ) {
                            $scope.AppRefNo = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].ApprovalRefNumber;
                            $scope.Dewa = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Dewa;
                            $scope.EjariPropertyId = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].EjariPropertyId;
                            $scope.PropertyNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertyNumber;
                            $scope.PropertySubType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertySubType;
                            $scope.PropertyType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertyType;
                            $scope.Size = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Size;
                            $scope.Usage = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Usage;
                        }
                        if (items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem.length > 1) {
                            $scope.AppRefNo = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].ApprovalRefNumber;
                            $scope.Dewa = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Dewa;
                            $scope.EjariPropertyId = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].EjariPropertyId;
                            $scope.PropertyNumber = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertyNumber;
                            $scope.PropertySubType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertySubType;
                            $scope.PropertyType = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].PropertyType;
                            $scope.Size = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Size;
                            $scope.Usage = items.GetContractDetailsByNumberResponse.GetContractDetailsByNumberResult.AssociatedPropertyDetails.PropertiesList.PropertyListItem[0].Usage;
                        }
                    }
                }
            }
        }
    }
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });

