var uri = "https://localhost:5001/api/Address";

function CreateAddress() {
    $(`[name="addAddressLineOne"]`).empty();
    $(`[name="addCity"]`).empty();
    $(`[name="addState"]`).empty();
    $(`[name="addZipCode"]`).empty();

    const address = {
        AddressLineOne: $(`#add-address-line-one`).val(),
        AddressLineTwo: $(`#add-address-line-two`).val(),
        City: $(`#add-city`).val(),
        StateAbbreviation: $(`#add-state`).val(),
        ZipCode: $(`#add-zip-code`).val()
    };

    if (address.AddressLineOne === "") {
        $(`[name="addAddressLineOne"]`).append("* Address Line One is required.");
        return;
    } else if (address.City === "") {
        $(`[name="addCity"]`).append("* City is required.");
    } else if (address.StateAbbreviation === "") {
        $(`[name="addState"]`).append("* State is required.");
    } else if (address.ZipCode === "") {
        $(`[name="addZipCode"]`).append("* Zip code is required.");
    } else {
        $.ajax({
            type: "POST",
            accepts: "application/json",
            url: uri,
            contentType: "application/json",
            data: JSON.stringify(address),
            error: error => {
                console.log(error);
            },
            success: result => {
                $(`#add-address-line-one`).val("");
                $(`#add-address-line-two`).val("");
                $(`#add-city`).val("");
                $(`#add-state`).val("");
                $(`#add-zip-code`).val("");               
            }
        });
    }
}