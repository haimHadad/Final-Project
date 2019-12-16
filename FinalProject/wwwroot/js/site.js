// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function offerContract() {
    document.getElementById("CreateContractDialogMessage").innerHTML = "";
    ErrorMsg = checkErrors();

    if (ErrorMsg != "") {
        $('#CreateContractDialogTitle').text("Error"); //show error headline
        document.getElementById("CreateContractDialogMessage").innerHTML = ErrorMsg;
        document.getElementById("closeButton").style.display = "block";
        $('#dialogOfferContract').modal('show');
        return;
    }

    //AssetID OwnertID SellerPublicKey Loaction AreaIn Rooms BuyerPublicKey PriceETH TimeToBeOpen

    var AssetID = document.getElementById("AssetIdTxt").value;
    var OwnertID = document.getElementById("OwnerIdTxt").value;
    var SellerPublicKey = document.getElementById("OwnerPublicKeyTxt").value;
    var Loaction = document.getElementById("LoactionTxt").value;
    var AreaIn = document.getElementById("AreaInTxt").value;
    var Rooms = document.getElementById("RoomsTxt").value;
    var BuyerPublicKey = document.getElementById("BuyerAddressTxt").value;
    var PriceETH = document.getElementById("PriceEthTxt").value;
    var PriceILS = document.getElementById("PriceIlsTxt").value;;
    var TimeToBeOpen = document.getElementById("TimeSelector").value;
    var ImageURL = document.getElementById("imageAssetForm").src

    document.getElementById("AssetSelector").disabled = true;
    DisableForm();
    document.getElementById("DeployLoader").style.display = "block";
    $.ajax(
        {
            url: "/CreateContract/DeployContract",
            type: 'POST',
            async: true,
            data: { AssetID, OwnertID, SellerPublicKey, Loaction, AreaIn, Rooms, BuyerPublicKey, PriceETH, PriceILS, TimeToBeOpen, ImageURL },
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
                    updateAccountBalanceAfterBlockchainOperation();
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




function DeleteFormContent() {
    $('#AssetIdTxt').val("");
    $('#OwnerIdTxt').val("")
    $('#OwnerPublicKeyTxt').val("")
    $('#LoactionTxt').val("")
    $('#AreaInTxt').val("")
    $('#RoomsTxt').val("")
    $('#PriceIlsTxt').val("")
    $('#PriceEthTxt').val("");
    $('#BuyerAddressTxt').val("");
    document.getElementById("lastPriceLabel").textContent = "";
    document.getElementById("TaxLabel").textContent = "";
    document.getElementById("lastPriceLabel").style.display = "none";
    document.getElementById("TaxLabel").style.display = "none";
    document.getElementById("TimeSelector").selectedIndex = "0";
    document.getElementById("imageAssetForm").setAttribute("src", "");
}

function DisableForm() {

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

}

function EnableForm(getAssetJson) {
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
