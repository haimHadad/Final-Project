// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function SendLoginData()
{
    var url = "/DappAccount/CheckAccount";
    $.post(url, { PublicKey: $("#public_key_input").val(), PrivateKey: $("#private_key_input").val() }, function (data) {
        if (data == false)
        {
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
        }
    });
}