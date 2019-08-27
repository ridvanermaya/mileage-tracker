var uri = "https://localhost:5001/api/Service";

function CreateServices() {
    let selectedServices = $(`#select-service`).val();
    $(`[name="addServices"]`).empty();
    
    if (selectedServices.length === 0) {
        $(`[name="addServices"]`).append(`* Select at least one service!`);
    } else {
        for (let index = 0; index < selectedServices.length; index++) {
            const service = {
                Name: selectedServices[index]
            }
            $.ajax({
                type: "POST",
                accepts: "application/json",
                url: uri,
                contentType: "application/json",
                data: JSON.stringify(service),
                error: error => {
                    console.log(error);
                },
                success: result => {
                    GetServices();
                }
            });
        }
    }
}

function GetServices() {
    $.ajax({
        type: "GET",
        url: uri,
        success: function(services) {
            const tBody = $(`#services`);
            tBody.empty();

            $.each(services, function(index, service)
            {
                let tr = `<tr></tr>`;
                let td = `<td></td>`;
                let tdLast = `<td></td>`;

                $(tBody).append(
                    $(tr).append(
                        $(td).text(service.name)
                    ).append(
                        $(tdLast).append(`<button id="btn-remove-service-${service.serviceId}" onclick="RemoveService(${service.serviceId})" type="button" class="btn btn-danger">Remove</button>`)
                    )
                );
            })
        }
    });
}

function RemoveService(serviceId) {
    $.ajax({
        url: uri + "/" + serviceId,
        type: "DELETE",
        success: function() {
            GetServices();
        }
    });
}

$(document).ready(function() {
    GetServices();
});