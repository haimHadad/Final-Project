// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.







var interval; //time for the buyer
function setTimer(timeLeftInSeconds) //time for the buyer
{
    console.log("time left -- " + timeLeftInSeconds);
    clearInterval(interval)

    var timeSpan = convert(timeLeftInSeconds);
    console.log(timeSpan);
    //var countDownDate = new Date("Jan 5, 2021 15:37:25").getTime();
    var countDownDate = new Date(timeSpan).getTime();

    // Update the count down every 1 second
    interval = setInterval(function () {
        // Get today's date and time
        var now = new Date().getTime();

        // Find the distance between now and the count down date
        var distance = countDownDate - now;
        document.getElementById("openContractDialogTime").style.color = "lightgreen";
        document.getElementById("openContractDenyBtn").disabled = false;
        var checkBoxValue = document.getElementById("openContractCheckBox").checked;
        if (checkBoxValue == true) {
            document.getElementById("openContractSignBtn").disabled = true;
        }
        else {
            document.getElementById("openContractSignBtn").disabled = false;
        }
        // Time calculations for days, hours, minutes and seconds
        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Output the result in an element with id="demo"
        //document.getElementById("demo").innerHTML = days + "d " + hours + "h " + minutes + "m " + seconds + "s ";
        document.getElementById("openContractDialogTime").innerHTML = days + "d " + hours + "h " + minutes + "m " + seconds + "s ";


        // If the count down is over, write some text
        if (distance < 0) {
            clearInterval(interval);
            //document.getElementById("demo").innerHTML = "EXPIRED";
            document.getElementById("openContractDialogTime").innerHTML = "EXPIRED";
            document.getElementById("openContractDialogTime").style.color = "red";
            document.getElementById("openContractDenyBtn").disabled = true;
            document.getElementById("openContractSignBtn").disabled = true;
        }
    }, 1000);


}




function convert(timeLeft) //time for the buyer
{
    var now = new Date()
    var secondsSinceEpoch = Math.round(now.getTime() / 1000)

    // Unixtimestamp
    var unixtimestamp = timeLeft;
    unixtimestamp = parseInt(unixtimestamp);
    secondsSinceEpoch = parseInt(secondsSinceEpoch);
    unixtimestamp = unixtimestamp + secondsSinceEpoch;
    // Months array
    var months_arr = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    // Convert timestamp to milliseconds
    var date = new Date(unixtimestamp * 1000);

    // Year
    var year = date.getFullYear();

    // Month
    var month = months_arr[date.getMonth()];

    // Day
    var day = date.getDate();

    // Hours
    var hours = date.getHours();

    // Minutes
    var minutes = "0" + date.getMinutes();

    // Seconds
    var seconds = "0" + date.getSeconds();

    // Display date time in MM-dd-yyyy h:m:s format
    var convdataTime = month + ' ' + day + ', ' + year + ' ' + hours + ':' + minutes.substr(-2) + ':' + seconds.substr(-2);
    //"Jan 5, 2021 15:37:25"
    //document.getElementById('datetime').innerHTML = convdataTime;
    console.log("Time left date-->" + convdataTime);
    return convdataTime;
}






function AllowNotes() //allow notes on buyer deny
{
    checkBox = document.getElementById("openContractCheckBox").checked;
    if (checkBox == true) {

        $('#openContractSignBtn').disabled = true;
        document.getElementById("openContractdialogNotes").placeholder = "Add your notes";
        document.getElementById("openContractdialogNotes").value = "";
        document.getElementById("openContractdialogNotes").disabled = false;
        document.getElementById("openContractSignBtn").disabled = true;
    }

    else {
        document.getElementById("openContractdialogNotes").disabled = true;
        document.getElementById("openContractdialogNotes").placeholder = "";
        document.getElementById("openContractdialogNotes").value = "";
        document.getElementById("openContractSignBtn").disabled = false;
    }
    var checkTimeLeft = document.getElementById("openContractDialogTime").textContent;
    if (checkTimeLeft == "EXPIRED") {
        document.getElementById("openContractDenyBtn").disabled = true;
        document.getElementById("openContractSignBtn").disabled = true;
    }
}


function CancelDealContract() //for the seller - if the buyer did not sign
{
    var offer = currentExpierdDealOffer;

    var loaderID = "DeployLoader" + offer.AssetID;
    var lblCancelID = "lblCancel" + offer.AssetID;
    var btnCancelID = "btnCancelID" + offer.AssetID;

    document.getElementById(btnCancelID).style.display = "none";
    document.getElementById(loaderID).style.display = "block";

    $.ajax({
        url: "/ContractsStatus/CancelDealAsSeller",
        type: 'POST',
        async: true,
        data: { ContractAddress: offer.ContractAddress, PublicKey: offer.SellerPublicKey },
        success: function (data) {
            document.getElementById(lblCancelID).style.display = "block";
            document.getElementById(lblCancelID).style.color = "red";
            document.getElementById(loaderID).style.display = "none";
            timeLeftInView = data;
            var result = JSON.parse(data);
            var feeILS = result.feeILS;
            document.getElementById("actionDealDialogTitle").innerHTML = "The contract canceled sucessfully";
            document.getElementById("actionDealDialogMessageContent1").style.display = "block";
            document.getElementById("actionDealDialogMessageContent2").style.display = "block";
            document.getElementById("actionDealDialogMessageContent3").style.display = "block";
            document.getElementById("actionDealDialogMessageContent4").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").href = "https://ropsten.etherscan.io/address/" + result.ContractAddress;
            document.getElementById("actionDealDialogMessageContent1").innerHTML = "The contract has canceled.";
            document.getElementById("actionDealDialogMessageContent2").innerHTML = "AssetID".bold() + " : " + offer.AssetID + "." + "</br>" + "Loaction".bold() + " : " + offer.Loaction + ".";
            document.getElementById("actionConfirmationImg").src = "/img/V-symbol.png";
            document.getElementById("actionDealDialogMessageContent4").innerHTML = "Fee : ₪" + result.feeILS;
            $('#actionDealDialog').modal('show');
            updateAccountBalanceAfterBlockchainOperation(offer.SellerPublicKey);

        }
    });



}


function DenyContract() //for buyer
{
    var offer = currentContract;

    var checkError = checkEthForFee();
    if (checkError != "") {
        $('#DialogBlockchainError').modal('show');
        console.log(offer);
        return;
    }
    var notes = document.getElementById("openContractdialogNotes").value;

    var loaderDenyID = "DeployLoader" + offer.AssetID;
    var lblDenyID = "resultOpenContract" + offer.AssetID;
    var btnOpenContractID = "openContract" + offer.AssetID;

    document.getElementById(btnOpenContractID).style.display = "none";
    document.getElementById(loaderDenyID).style.display = "block";

    $.ajax({
        url: "/ContractsStatus/CancelDealAsBuyer",
        type: 'POST',
        async: true,
        data: { ContractAddress: offer.ContractAddress, Notes: notes, PublicKey: offer.BuyerPublicKey },
        success: function (data) {

            document.getElementById(loaderDenyID).style.display = "none";
            document.getElementById(lblDenyID).style.display = "block";
            document.getElementById(lblDenyID).innerHTML = "Denied by the buyer (You)";
            document.getElementById(lblDenyID).style.color = "red";
            //timeLeftInView = data;
            var result = JSON.parse(data);
            var feeILS = result.feeILS;
            document.getElementById("actionDealDialogTitle").innerHTML = "The contract denied sucessfully";
            document.getElementById("actionDealDialogMessageContent1").style.display = "block";
            document.getElementById("actionDealDialogMessageContent2").style.display = "block";
            document.getElementById("actionDealDialogMessageContent3").style.display = "block";
            document.getElementById("actionDealDialogMessageContent4").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").href = "https://ropsten.etherscan.io/address/" + result.ContractAddress;
            document.getElementById("actionDealDialogMessageContent1").innerHTML = "The contract has denied.";
            document.getElementById("actionDealDialogMessageContent2").innerHTML = "AssetID".bold() + " : " + offer.AssetID + "." + "</br>" + "Loaction".bold() + " : " + offer.Loaction + ".";
            document.getElementById("actionConfirmationImg").src = "/img/V-symbol.png";
            document.getElementById("actionDealDialogMessageContent4").innerHTML = "Fee : ₪" + result.feeILS;
            $('#actionDealDialog').modal('show');
            updateAccountBalanceAfterBlockchainOperation(offer.BuyerPublicKey);


        }
    });


}


function ApproveDeal() //for the buyer
{
    var offer = currentContract;
    var ErrorMsg = "";
    ErrorMsg = checkEthForDeal(offer);

    if (ErrorMsg != "") {
        $('#DialogBlockchainError').modal('show');
        console.log(offer);
        return;
    }

    var btnIdApprove = "openContract" + offer.AssetID;
    var lblIdApprove = "resultOpenContract" + offer.AssetID;
    var loaderIdApprove = "DeployLoader" + offer.AssetID;
    document.getElementById(btnIdApprove).style.display = "none";
    document.getElementById(loaderIdApprove).style.display = "block";

    $.ajax({
        url: "/ContractsStatus/ApproveContract",
        type: 'POST',
        async: true,
        data: { ContractAddress: offer.ContractAddress, PublicKey: offer.BuyerPublicKey },
        success: function (data) {

            document.getElementById(lblIdApprove).innerHTML = "Awaiting for regulator signing";
            document.getElementById(lblIdApprove).style.color = "lightgreen";
            document.getElementById(lblIdApprove).style.display = "block";
            document.getElementById(loaderIdApprove).style.display = "none";
            updateAccountBalanceAfterBlockchainOperation(offer.BuyerPublicKey);
            var result = JSON.parse(data);
            var feeILS = result.feeILS;
            document.getElementById("actionDealDialogTitle").innerHTML = "The contract approved sucessfully";
            document.getElementById("actionDealDialogMessageContent1").style.display = "block";
            document.getElementById("actionDealDialogMessageContent2").style.display = "block";
            document.getElementById("actionDealDialogMessageContent3").style.display = "block";
            document.getElementById("actionDealDialogMessageContent4").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").style.display = "block";
            document.getElementById("actionDealDialogEtherscanURL").href = "https://ropsten.etherscan.io/address/" + result.ContractAddress;
            document.getElementById("actionDealDialogMessageContent1").innerHTML = "The contract been approved.";
            document.getElementById("actionDealDialogMessageContent2").innerHTML = "AssetID".bold() + " : " + offer.AssetID + "." + "</br>" + "Loaction".bold() + " : " + offer.Loaction + ".";
            document.getElementById("actionConfirmationImg").src = "/img/V-symbol.png";
            var dealPriceILS = offer.PriceILS.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            document.getElementById("actionDealDialogMessageContent4").innerHTML = "You paid : ".bold() + offer.PriceETH + " ETH / ₪" + dealPriceILS + "</br>" + "Fee : ₪" + result.feeILS;
            $('#actionDealDialog').modal('show');
        },
        error: function (xhr, text, error) {
            updateAccountBalanceAfterBlockchainOperation(offer.BuyerPublicKey)
            var ErrorMsg = "You running out of ETH, please deposit more ETH.<br>";
            $('#DialogBlockchainError').modal('show');
            console.log(offer);
            document.getElementById(btnIdApprove).style.display = "block";
            document.getElementById(loaderIdApprove).style.display = "none";
            return;
        }

    });



}


function offerContract()
{
    document.getElementById("CreateContractDialogMessage").innerHTML = "";
    ErrorMsg = checkErrors();
    console.log("Check after icon : " + ErrorMsg);
    if (ErrorMsg != "") {
        $('#CreateContractDialogTitle').text("Error"); //show error headline
        document.getElementById("CreateContractDialogMessage").innerHTML = ErrorMsg;
        document.getElementById("closeButton").style.display = "block";
        $('#dialogOfferContract').modal('show');
        return;
    }

    //AssetID OwnerID SellerPublicKey Loaction AreaIn Rooms BuyerPublicKey PriceETH TimeToBeOpen
    console.log("Here we continue after checking the errors! ! ! ! !");
    var AssetID = document.getElementById("AssetIdTxt").value;
    var OwnerID = document.getElementById("OwnerIdTxt").value;
    var SellerPublicKey = document.getElementById("OwnerPublicKeyTxt").value;
    var Loaction = document.getElementById("LoactionTxt").value;
    var AreaIn = document.getElementById("AreaInTxt").value;
    var Rooms = document.getElementById("RoomsTxt").value;
    var BuyerPublicKey = document.getElementById("BuyerAddressTxt").value;
    var PriceETH = document.getElementById("PriceEthTxt").value;
    var PriceILS = document.getElementById("PriceIlsTxt").value;;
    var TimeToBeOpen = document.getElementById("TimeSelector").value;
    var ImageURL = document.getElementById("imageAssetForm").src
    var BuyerID = document.getElementById("BuyerIdTxt").value;

    document.getElementById("AssetSelector").disabled = true;
    DisableForm();
    document.getElementById("DeployLoader").style.display = "block";
    $.ajax(
        {
            url: "/CreateContract/DeployContract",
            type: 'POST',
            async: true,
            data: { AssetID, OwnerID, SellerPublicKey, Loaction, AreaIn, Rooms, BuyerPublicKey, PriceETH, PriceILS, TimeToBeOpen, BuyerID, ImageURL },
            success: function (data)
            {
                    var result = JSON.parse(data);
                    var feeILS = result.feeILS;
                    document.getElementById("DialogDeployContractMessageTitle").innerHTML = "The contract sent to the buyer sucessfully";
                    document.getElementById("DialogDeployContractMessageContent1").style.display = "block";
                    document.getElementById("DialogDeployContractMessageContent2").style.display = "block";
                    document.getElementById("DialogDeployContractMessageContent3").style.display = "block";
                    document.getElementById("DialogDeployContractMessageContent4").style.display = "block";
                    document.getElementById("DialogDeployContractErrorMessageContent").style.display = "none";
                    document.getElementById("EtherscanURL").style.display = "block";
                    document.getElementById("EtherscanURL").href = "https://ropsten.etherscan.io/address/" + result.ContractAddress;
                    document.getElementById("DialogDeployContractMessageContent1").innerHTML = "The contract uploaded to the Blockchain.";
                    document.getElementById("DialogDeployContractMessageContent2").innerHTML = "AssetID".bold() + " : " + AssetID + "." + "</br>" + "Loaction".bold() + " : " + Loaction + ".";
                    document.getElementById("deployConfirmation").src = "/img/V-symbol.png";
                    document.getElementById("DialogDeployContractMessageContent4").innerHTML = "Fee : ₪" + result.feeILS;             
                    $('#DialogDeployContractMessage').modal('show');
                    updateAccountBalanceAfterBlockchainOperation(SellerPublicKey);
                    document.getElementById("DeployLoader").style.display = "none";
                    document.getElementById("AssetSelector").disabled = false;
                    DeleteFormContent();
                    var AvailableAssetsList = document.getElementById("AssetSelector");
                    AvailableAssetsList.remove(AvailableAssetsList.selectedIndex);
                    document.getElementById("AssetSelector").selectedIndex = "0";
                    

            },

            error: function (xhr, text, error) {
                document.getElementById("deployConfirmation").src = "/img/X-symbol.png";
                document.getElementById("DialogDeployContractMessageTitle").innerHTML = "Error";
                document.getElementById("DialogDeployContractMessageContent1").style.display = "none";
                document.getElementById("DialogDeployContractMessageContent2").style.display = "none";
                document.getElementById("DialogDeployContractMessageContent3").style.display = "none";
                document.getElementById("DialogDeployContractMessageContent4").style.display = "none";
                document.getElementById("DialogDeployContractErrorMessageContent").style.display = "block";
                document.getElementById("EtherscanURL").style.display = "none";
                document.getElementById("DialogDeployContractErrorMessageContent").innerHTML = "Failed to send to the buyer!";
                document.getElementById("DialogDeployContractErrorMessageContent").innerHTML = "Try again later";
                $('#DialogDeployContractMessage').modal('show');
                document.getElementById("DeployLoader").style.display = "none";
                EnableForm(assetReturnedFromFailedDeplyment);
                document.getElementById("AssetSelector").disabled = false;
            }
        });


}




function DeleteFormContent() { //for deployment
    $('#AssetIdTxt').val("");
    $('#OwnerIdTxt').val("")
    $('#OwnerPublicKeyTxt').val("")
    $('#LoactionTxt').val("")
    $('#AreaInTxt').val("")
    $('#RoomsTxt').val("")
    $('#PriceIlsTxt').val("")
    $('#PriceEthTxt').val("");
    $('#BuyerAddressTxt').val("");
    $('#BuyerIdTxt').val("");
    document.getElementById("lastPriceLabel").textContent = "";
    document.getElementById("TaxLabel").textContent = "";
    document.getElementById("lastPriceLabel").style.display = "none";
    document.getElementById("TaxLabel").style.display = "none";
    document.getElementById("TimeSelector").selectedIndex = "0";
    document.getElementById("imageAssetForm").setAttribute("src", "");
}

function DisableForm() { //for deployment

    document.getElementById("PriceEthTxt").disabled = true;
    document.getElementById("BuyerAddressTxt").disabled = true;
    document.getElementById("PriceEthTxt").placeholder = "";
    document.getElementById("BuyerAddressTxt").placeholder = "";
    document.getElementById("TimeSelector").disabled = true;
    document.getElementById("deployContractBtn").disabled = true;
    document.getElementById("AssetIdTxt").disabled = true;
    document.getElementById("AssetIdTxt").readOnly = false;
    document.getElementById("OwnerIdTxt").disabled = true;
    document.getElementById("OwnerIdTxt").readOnly = false;
    document.getElementById("OwnerPublicKeyTxt").disabled = true;
    document.getElementById("OwnerPublicKeyTxt").readOnly = false;
    document.getElementById("LoactionTxt").disabled = true;
    document.getElementById("LoactionTxt").readOnly = false;
    document.getElementById("AreaInTxt").disabled = true;
    document.getElementById("AreaInTxt").readOnly = false;
    document.getElementById("RoomsTxt").disabled = true;
    document.getElementById("RoomsTxt").readOnly = false;
    document.getElementById("PriceIlsTxt").disabled = true;
    document.getElementById("PriceIlsTxt").readOnly = false;
    document.getElementById("BuyerAddressTxt").disabled = true;
    document.getElementById("BuyerAddressTxt").readOnly = false;
    document.getElementById("BuyerIdTxt").disabled = true;
    document.getElementById("BuyerIdTxt").readOnly = false;

}

function EnableForm(getAssetJson) { //for deployemnt
    document.getElementById("AssetIdTxt").disabled = false;
    document.getElementById("AssetIdTxt").readOnly = true;
    document.getElementById("OwnerIdTxt").disabled = false;
    document.getElementById("OwnerIdTxt").readOnly = true;
    document.getElementById("OwnerPublicKeyTxt").disabled = false;
    document.getElementById("OwnerPublicKeyTxt").readOnly = true;
    document.getElementById("LoactionTxt").disabled = false;
    document.getElementById("LoactionTxt").readOnly = true;
    document.getElementById("AreaInTxt").disabled = false;
    document.getElementById("AreaInTxt").readOnly = true;
    document.getElementById("RoomsTxt").disabled = false;
    document.getElementById("RoomsTxt").readOnly = true;
    document.getElementById("PriceIlsTxt").disabled = false;
    document.getElementById("PriceIlsTxt").readOnly = true;
    document.getElementById("BuyerIdTxt").disabled = false;
    document.getElementById("BuyerIdTxt").readOnly = true;
    document.getElementById("BuyerIdTxt").disabled = false;
    document.getElementById("BuyerIdTxt").readOnly = true;
    var asset = JSON.parse(getAssetJson);
    $('#AssetIdTxt').val(asset.AssetID);
    $('#OwnerIdTxt').val(asset.OwnerID)
    $('#OwnerPublicKeyTxt').val(asset.OwnerPublicKey)
    $('#LoactionTxt').val(asset.Loaction)
    $('#AreaInTxt').val(asset.AreaIn)
    $('#RoomsTxt').val(asset.Rooms)
    $('#PriceIlsTxt').val("0")
    document.getElementById("lastPriceLabel").textContent = "Last purchase price: " + asset.Price + "ETH | ₪" + getPriceInILS(asset.Price);
    document.getElementById("lastPriceLabel").style.display = "block";
    document.getElementById("TaxLabel").textContent = "Tax amount: 0 ETH | ₪0";
    document.getElementById("TaxLabel").style.display = "block";
    document.getElementById("PriceEthTxt").disabled = false;
    document.getElementById("BuyerAddressTxt").disabled = false;
    document.getElementById("PriceEthTxt").placeholder = "Insert Price";
    document.getElementById("BuyerAddressTxt").placeholder = "Insert Buyer Address";
    document.getElementById("TimeSelector").disabled = false;
    document.getElementById("deployContractBtn").disabled = false;
    document.getElementById("imageAssetForm").setAttribute("src", asset.ImageURL);

}

function getBuyerID() {
    var buyerPublicKey = document.getElementById("BuyerAddressTxt").value;
    var sellerPublicKey = document.getElementById("OwnerPublicKeyTxt").value;

    if (buyerPublicKey == sellerPublicKey) {
        var OwnerID = document.getElementById("OwnerIdTxt").value;
        $('#BuyerIdTxt').val("Error: buyer is owner!");
        document.getElementById("BuyerIdTxt").style.color = "red";
        return;
    }

    if (buyerPublicKey == "") {
        $('#BuyerIdTxt').val("");
        document.getElementById("BuyerIdTxt").style.color = "black";
        return;
    }

    if (buyerPublicKey != "") {
        $.ajax({
            url: "/CreateContract/GetAddressID",
            type: 'POST',
            async: false,
            data: { PublicKey: buyerPublicKey },
            success: function (data) {
                if (data == 0) {
                    $('#BuyerIdTxt').val("Error- buyer is not recognized");
                    document.getElementById("BuyerIdTxt").style.color = "red";
                }

                else {
                    $('#BuyerIdTxt').val(data);
                    document.getElementById("BuyerIdTxt").style.color = "black";
                }

            }
        });
    }
}

function ViewAssetDeteils(assetJson) {
    var fullPublicKey = assetJson.OwnerPublicKey;
    var partOne = fullPublicKey.substring(0, 12);
    var partTwo = " . . . ";
    var partThree = fullPublicKey.substring(32, fullPublicKey.length);
    var publicKeyToShow = "" + partOne + partTwo + partThree;
    document.getElementById("dialogAssetID").innerHTML = "Asset No : ".bold() + assetJson.AssetID;
    document.getElementById("dialogOwnerID").innerHTML = "Your ID : ".bold() + assetJson.OwnerID;
    document.getElementById("dialogOwnerPublicKey").innerHTML = "Your PublicKey :\n".bold() + publicKeyToShow;//param.OwnerPublicKey ;
    document.getElementById("dialogLoaction").innerHTML = "Loaction : ".bold() + assetJson.Loaction;
    document.getElementById("dialogAreaIn").innerHTML = "AreaIn : ".bold() + assetJson.AreaIn;
    document.getElementById("dialogRooms").innerHTML = "Rooms : ".bold() + assetJson.Rooms;
    document.getElementById("dialogImageURL").setAttribute("src", assetJson.ImageURL);
    document.getElementById("dialogPrice").innerHTML = "Purchace Price : ".bold() + assetJson.Price + " ETH / ₪" + getPriceInILS(assetJson.Price);
    $('.table tbody').on('click', '.btn', function () {
        var row = $(this).closest('tr');
        var col = row.find('td:eq(1)').text();
        //document.getElementById("asset_dialog_body").innerHTML("vddv");
    })
}
$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
})


function copyToClipBoard()
{
    var copyText = document.getElementById("PublicKeyInput");
    copyText.select();
    copyText.setSelectionRange(0, 99999);
    document.execCommand("copy");
    $('#dialogCopy').modal('show');
}



function SendLoginData() {
    var url = "/DappAccount/CheckAccount";
    $.post(url, { PublicKey: $("#public_key_input").val(), PrivateKey: $("#private_key_input").val() }, function (data) {
        if (data == false) {
            $("#public_key_input").val("Wrong Credentials");
            document.getElementById("public_key_input").style.color = "red";
            $("#private_key_input").val("");
            document.getElementById("public_key_input").addEventListener("click", ResetInputPublicKey);

            return;
        }
        else {
            document.getElementById("loader").style.display = "block";
            if ($("#public_key_input").val() == "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80")
                document.getElementById("israelLogo").style.display = "block";
            $("#myform").submit()
        }

        function ResetInputPublicKey() {
            document.getElementById("public_key_input").style.color = "black";
            $("#public_key_input").val("");
        }
    });
}

function wait(ms) {
    var d = new Date();
    var d2 = null;
    do {
        d2 = new Date();
    }
    while (d2 - d < ms);
}
