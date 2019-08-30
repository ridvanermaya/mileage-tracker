$(`#navbar`).load(`Navbar.html`);
GetServicesHome();

let apiKey = "";
let path = new Array();
let intervalId;
let startDateTime;
let endDateTime;
let service;


function StartTracking() {
    let date = new Date();
    startDateTime = date.getMonth().toString() + "-" + date.getDate().toString() + "-" + date.getFullYear().toString() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
    service = $(`#select-input`).val();
    $(`#btn-content`).empty();
    
    let endTrackingButton = `<button class="btn btn-outline-danger" style="border-radius: 50%; height:100px; width: 100px; display: block; margin: auto;" type="button" onclick="EndTracking()">End Tracking</button>`;

    $(`#btn-content`).append(endTrackingButton);

    GetLocation();

    intervalId = setInterval(GetLocation, 20000);

}

function EndTracking() {
    clearInterval(intervalId);
    let distance = 0;

    let date = new Date();
    endDateTime = date.getMonth().toString() + "-" + date.getDate().toString() + "-" + date.getFullYear().toString() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();

    let apiPaths = new Array();
    let startTrackingButton = `<button class="btn btn-outline-primary" style="border-radius: 50%; height:100px; width: 100px; display: block; margin: auto;" onclick="StartTracking()" type="button">Start Tracking</button>`;
    let latLongs = new Array();
    let startLatLong = {
        latitude: 0,
        longitude: 0
    }

    latLongs.push(startLatLong);
    let snappedPointsCount = latLongs.length;
    
    $(`#btn-content`).empty();
    GetServicesHome();
    // $(`#btn-content`).append(startTrackingButton);

    // needs to be uncommented after testing
    // while(snappedPointsCount > 0) {
    //     apiPaths.push(latLongs.slice(0,100));
    //     latLongs.splice(0, 100);
    //     snappedPointsCount = latLongs.length;
    // }

    // for testing purposes
    path = ["43.034591,-87.911978", "43.034703,-87.912046", "43.034746,-87.911229", "43.036040,-87.911231", "43.036036,-87.912783", "43.035843,-87.913021", "43.035669,-87.913032", "43.035378,-87.912873", "43.035163,-87.912593", "43.035167,-87.912591", "43.035034,-87.912127", "43.035054,-87.911902", "43.035127,-87.911679", "43.035331,-87.911497", "43.035530, -87.911471", "43.035675,-87.911516", "43.035826,-87.911758", "43.035885,-87.912104", "43.035883,-87.916883", "43.035925, -87.919330", "43.035832,-87.920959", "43.035763,-87.921985", "43.035687,-87.922398", "43.035498,-87.922874", "43.035174,-87.923266", "43.034910,-87.923555", "43.034547,-87.923700", "43.034162,-87.923772", "43.034185,-87.923762", "43.033747, -87.923669", "43.030856,-87.923132", "43.030161,-87.923111", "43.029285,-87.923039", "43.028508,-87.923008", "43.027942,-87.922791", "43.025616,-87.921998", "43.023689,-87.921303", "43.022283,-87.920753", "43.021004,-87.919435", "43.019499,-87.917727", "43.018882,-87.917233", "43.017994,-87.916636", "43.017136,-87.916307", "43.015150,-87.916183", "43.010664,-87.916204", "43.001393,-87.916386", "43.000832,-87.916381", "42.999484,-87.915904", "42.997771,-87.915687", "42.995424,-87.915362", "42.991396,-87.914842", "42.988257,-87.914417", "42.987400,-87.914417", "42.986532,-87.914577", "42.984784,-87.915444", "42.983353,-87.916246", "42.982578,-87.916567", "42.982109,-87.916615", "42.980996,-87.916724", "42.979271,-87.916434", "42.975884,-87.915676", "42.973400,-87.915748", "42.970168,-87.915705", "42.968889,-87.916145", "42.967900,-87.917029", "42.967027,-87.918778", "42.966717,-87.921045", "42.965960,-87.929687", "42.965805,-87.930814", "42.965417,-87.932113", "42.964621,-87.933372", "42.963884,-87.934194", "42.963031,-87.934737", "42.961818,-87.934909", "42.960081,-87.935011", "42.958393,-87.935192", "42.955159,-87.935389", "42.951850,-87.935414", "42.949479,-87.935320", "42.942386,-87.935455", "42.939357,-87.935502", "42.936091,-87.935735", "42.934015,-87.935643", "42.932790,-87.935922", "42.930884,-87.936294", "42.930508,-87.936345", "42.930359,-87.937068", "42.930326,-87.939487", "42.929168,-87.939668", "42.928127,-87.940264", "42.925761,-87.940379", "42.923580,-87.940425", "42.922858,-87.940356", "42.920374,-87.940447", "42.919015,-87.940653", "42.918684,-87.940767", "42.918700,-87.940292", "42.918601,-87.939862"];

    apiPaths.push(path);

    let snappedLatLongs = new Array();

    for (let index = 0; index < apiPaths.length; index++) {
        let apiPath = apiPaths[index].join("|");
        $.ajax({
            type: "GET",
            async: false,
            url: `https://roads.googleapis.com/v1/snapToRoads?path=${apiPath}&interpolate=true&key=${apiKey}`,
            success: function(snappedPoints) {
                for (let index = 1; index < snappedPoints.snappedPoints.length; index++) {
                    let latLong = {
                        latitude: snappedPoints.snappedPoints[index].location.latitude,
                        longitude: snappedPoints.snappedPoints[index].location.longitude
                    }
                    snappedLatLongs.push(latLong);
                }
                for (let index = 1; index < snappedLatLongs.length - 1; index++) {
                    distance += GetDistanceFromLatLonInKm(snappedLatLongs[index].latitude, snappedLatLongs[index].longitude, snappedLatLongs[index + 1].latitude, snappedLatLongs[index + 1].longitude);
                }
                distance /= 1.609;
                distance = distance.toFixed(2);
            }
        })
    }

    let mileageRecord = {
        service: service,
        mileage: distance,
        startDateTime: startDateTime,
        endDateTime: endDateTime
    };

    // ajax call to post mileage record
    $.ajax({
        type: "POST",
        accepts: "application/json",
        url: "https://localhost:5001/api/MileageRecord",
        contentType: "application/json",
        data: JSON.stringify(mileageRecord),
        error: error => {
            console.log(error);
        },
        success: result => {
            console.log("Success");
        }
    })
}

function GetLocation() {
    if ('geolocation' in navigator) {
        navigator.geolocation.getCurrentPosition(position => {
            let latLongStr = position.coords.latitude.toString() + "," + position.coords.longitude.toString();

            if (latLongStr === path[path.length - 1]) {
                return;
            } else {
                path.push(latLongStr);
            }
        })
    }
}

function GetDistanceFromLatLonInKm(lat1,lon1,lat2,lon2) {
    var R = 6371; // Radius of the earth in km
    var dLat = DegreeToRadius(lat2-lat1);  // deg2rad below
    var dLon = DegreeToRadius(lon2-lon1); 
    var a = 
      Math.sin(dLat/2) * Math.sin(dLat/2) +
      Math.cos(DegreeToRadius(lat1)) * Math.cos(DegreeToRadius(lat2)) * 
      Math.sin(dLon/2) * Math.sin(dLon/2)
      ; 
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a)); 
    var d = R * c; // Distance in km
    return d;
}
  
function DegreeToRadius(deg) {
    return deg * (Math.PI/180)
}

function GetServicesHome() {
    let selectService = `<div id="select-service" class="form-group" style="width: 100%; margin: auto; display: block;"></div>`;
    let startTrackingButton = `<button class="btn btn-outline-primary" style="border-radius: 50%; height:100px; width: 100px; display: block; margin: auto;" onclick="StartTracking()" type="button">Start Tracking</button>`;
    let selectInput = `<select id="select-input" class="form-control" style="width: auto; margin: auto; margin-bottom: 5px;"></select`;
    $(`#btn-content`).append(selectService);
    $(`#select-service`).append($(selectInput));

    $.ajax({
        type: "GET",
        async: false,
        url: "https://localhost:5001/api/Service",
        success: function(services) {
            $.each(services, function(index, service) {
                let option = new Option(`${service.name}`, `${service.name}`, false, false);
                $(option).html(`${service.name}`);
                $(`#select-input`).append($(option));
            })
        }
    })

    $(`#btn-content`).append(startTrackingButton);
}

function Home() {
    $(`#navbar`).load(`Navbar.html`);
    $(`#btn-content`).empty();
    $(`#select-service`).empty();
    $(`#mileage-records`).empty();
    $(`#profile`).empty();
    GetServicesHome();
}

function MileageRecords() {
    $(`#navbar`).load(`Navbar.html`);
    $(`#btn-content`).empty();
    $(`#mileage-records`).empty();
    $(`#profile`).empty();
    let hr = `<hr/ class="mt-0">`
    let header = `<h5 class="text-center mt-2">Mileage Records</h5>`;
    let mileageRecords =    `<div class="card shadow" style="margin: auto; width: 100%; margin-top: 10px;">
                                <div class="row">
                                    <div class="col-12">
                                        <table class="col-12 table table-bordered table-hover mb-0">
                                            <thead class="thead-dark" style="text-align: center;">
                                                <tr>
                                                    <th>Service</th>
                                                    <th>Start Date</th>
                                                    <th>End Date</th>
                                                    <th>Mile</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody id="mileage-records-table" style="text-align: center;">

                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>`;
    
    $(`#mileage-records`).append(header);
    $(`#mileage-records`).append(mileageRecords);
    

    $.ajax({
        type: "GET",
        url: "https://localhost:5001/api/MileageRecord",
        success: function(mileageRecords) {
            const tBody = $(`#mileage-records-table`);

            tBody.empty();

            $.each(mileageRecords, function(index, mileageRecord) {
                let tr = `<tr class="align-middle"></tr>`;
                let td = `<td class="align-middle"></td>`;

                let startYear = mileageRecord.startDateTime.slice(0, 4);
                let startMonth = mileageRecord.startDateTime.slice(5, 7);
                let startDay = mileageRecord.startDateTime.slice(8, 10);
                let startHour = mileageRecord.startDateTime.slice(11, 13);
                let startMinute = mileageRecord.startDateTime.slice(14, 16);
                let startSecond = mileageRecord.startDateTime.slice(17, 19);
                let endYear = mileageRecord.endDateTime.slice(0, 4);
                let endMonth = mileageRecord.endDateTime.slice(5, 7);
                let endDay = mileageRecord.endDateTime.slice(8, 10);
                let endHour = mileageRecord.endDateTime.slice(11, 13);
                let endMinute = mileageRecord.endDateTime.slice(14, 16);
                let endSecond = mileageRecord.endDateTime.slice(17, 19);

                $(tBody).append(
                    $(tr).append(
                        $(td).text(mileageRecord.service)
                    ).append(
                        $(td).text(startMonth + "-" + startDay + "-" + startYear + " " + startHour + ":" + startMinute + ":" + startSecond)
                    ).append(
                        $(td).text(endMonth + "-" + endDay + "-" + endYear + " " + endHour + ":" + endMinute + ":" + endSecond)
                    ).append(
                        $(td).text(mileageRecord.mileage)
                    ).append(
                        $(td).append(`<button id="btn-remove-mileage-record-${mileageRecord.mileageRecordId}" onclick="RemoveMileageRecord(${mileageRecord.mileageRecordId})" type="button" class="btn btn-danger">Delete</button>`)
                    )
                );
            })
        }
    });
}

function RemoveMileageRecord(mileagerRecordId) {
    $.ajax({
        url: "https://localhost:5001/api/MileageRecord" + "/" + mileagerRecordId,
        type: "DELETE",
        success: function() {
            MileageRecords();
        }
    });
}

function Profile() {
    $(`#navbar`).load(`Navbar.html`);
    $(`#profile`).empty();
    $(`#btn-content`).empty();
    $(`#mileage-records`).empty();

    let header = `<h5 class="text-center mt-2">Profile</h5>`;
    let userProfile =   `<div class="card mt-0" id="user-information">
                            <div class="card-body">
                                <form>
                                    <div class="form-group">
                                        <label for="add-first-name" class="col-form-label">First Name</label>
                                        <input type="text" class="form-control" id="add-first-name" required>
                                        <span for="add-first-name" class="text-danger" name="addFirstName"></span>
                                    </div>
                                    <div class="form-group">
                                        <label for="add-last-name" class="col-form-label">Last Name</label>
                                        <input type="text" class="form-control" id="add-last-name" required>
                                        <span for="add-last-name" class="text-danger" name="addLastName"></span>
                                    </div>
                                    <div class="form-group">
                                        <input type="button" class="btn btn-primary" onclick="UpdateUserProfile()" value="Update Profile" />
                                    </div>
                                </form>
                            </div>
                        </div>`;

    let userAddress =   `<div class="card" id="user-address">
                            <div class="card-body">
                                <form>
                                    <div class="form-group">
                                        <label for="add-address-line-one" class="col-form-label">Address Line One</label>
                                        <input type="text" class="form-control" id="add-address-line-one" required>
                                        <span for="add-address-line-one" class="text-danger" name="addAddressLineOne"></span>
                                    </div>
                                    <div class="form-group">
                                        <label for="add-address-line-two" class="col-form-label">Address Line Two</label>
                                        <input type="text" class="form-control" id="add-address-line-two" required>
                                        <span for="add-address-line-two" class="text-danger" name="addAddressLineTwo"></span>
                                    </div>
                                    <div class="form-group">
                                        <label for="add-city" class="col-form-label">City</label>
                                        <input type="text" class="form-control" id="add-city" required>
                                        <span for="add-city" class="text-danger" name="addCity"></span>
                                    </div>
                                    <div class="form-group">
                                        <label for="add-state-abbreviation" class="col-form-label">State Abbreviation</label>
                                        <input type="text" class="form-control" id="add-state" required>
                                        <span for="add-state" class="text-danger" name="addState"></span>
                                    </div>
                                    <div class="form-group">
                                        <label for="add-zip-code" class="col-form-label">Zip Code</label>
                                        <input type="number" class="form-control" id="add-zip-code" required>
                                        <span for="add-zip-code" class="text-danger" name="addZipCode"></span>
                                    </div>
                                    <div class="form-group">
                                        <input type="button" class="btn btn-primary" onclick="UpdateAddress()" value="Update Address" />
                                    </div>
                                </form>
                            </div>
                        </div>`

    let userServices = ` <div class="card" id="user-services">
                            <div class="row">
                                <div class="col-12">
                                    <table class="col-12 table table-bordered table-hover">
                                        <thead class="thead-dark">
                                            <tr>
                                                <th>Name</th>
                                                <th>Action</th>
                                            </tr>
                                        </thead>
                                        <tbody id="services">
                                            
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>`;

    let createServices =    `<div class="card">
                                <div class="card-header">
                                    <h5>Select Service</h5>
                                </div>
                                <div class="card-body">
                                    <form>
                                        <div class="form-group">
                                            <select id="select-service" multiple>
                                                <option value="GrubHub">Grubhub</option>
                                                <option value="DoorDash">DoorDash</option>
                                                <option value="Uber">Uber</option>
                                                <option value="PostMates">PostMates</option>
                                                <option value="Lyft">Lyft</option>
                                                <option value="Amazon Restaurants">Amazon R</option>
                                                <option value="Zomato">Zomato</option>
                                                <option value="Swiggy">Swiggy</option>
                                                <option value="Personal">Personal</option>
                                            </select>
                                            <span name="addServices" class="text-danger"></span>    
                                        </div>              
                                        <div class="form-group">
                                            <input type="button" class="btn btn-primary" onclick="AddServices()" value="Add Services" />
                                        </div>
                                    </form>
                                </div>
                            </div>`;

    $(`#profile`).append(header);
    $(`#profile`).append(userProfile);
    $(`#profile`).append(userAddress);
    $(`#profile`).append(createServices);
    $(`#profile`).append(userServices);
    $('select').selectpicker();
    
    GetServices();
}

function GetServices() {
    $.ajax({
        type: "GET",
        url: "https://localhost:5001/api/Service",
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
                        $(tdLast)
                            .append(`<button id="btn-remove-service-${service.serviceId}" onclick="RemoveService(${service.serviceId})" type="button" class="btn btn-danger">Remove</button>`)
                    )
                );
            })
        }
    });
}

function RemoveService(serviceId) {
    $.ajax({
        url: "https://localhost:5001/api/Service" + "/" + serviceId,
        type: "DELETE",
        success: function() {
            GetServices();
        }
    });
}

function AddServices() {
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
                url: "https://localhost:5001/api/Service",
                contentType: "application/json",
                data: JSON.stringify(service),
                error: error => {
                    console.log(error);
                },
                success: result => {
                    GetServices();
                    Profile();
                }
            });
        }
    }
}