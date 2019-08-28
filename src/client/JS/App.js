$(`#navbar`).load(`Navbar.html`);

let apiKey = "#";
let path = new Array();
let intervalId;

function StartTracking() {
    path = new Array();
    $(`#body`).empty();
    
    let endTrackingButton = `<button class="btn btn-danger" type="button" onclick="EndTracking()">End Tracking</button>`;

    $(`#body`).append(endTrackingButton);

    GetLocation();

    intervalId = setInterval(GetLocation, 20000);
}

function EndTracking() {
    clearInterval(intervalId);

    let apiPaths = new Array();
    let startTrackingButton = `<button class="btn btn-primary" onclick="StartTracking()" type="button">Start Tracking</button>`;
    let latLongs = [{latitude: 0, longitude: 0}];
    let snappedPointsCount = path.length;
    let distance = 0;
    
    $(`#body`).empty();
    $(`#body`).append(startTrackingButton);

    // needs to be uncommented after testing
    // while(snappedPointsCount > 0) {
    //     apiPaths.push(path.slice(0,100));
    //     path.splice(0, 100);
    //     snappedPointsCount = path.length;
    // }

    // for testing purposes
    path = ["43.034591,-87.911978", "43.034703,-87.912046", "43.034746,-87.911229", "43.036040,-87.911231", "43.036036,-87.912783", "43.035843,-87.913021", "43.035669,-87.913032", "43.035378,-87.912873", "43.035163,-87.912593", "43.035167,-87.912591", "43.035034,-87.912127", "43.035054,-87.911902", "43.035127,-87.911679", "43.035331,-87.911497", "43.035530, -87.911471", "43.035675,-87.911516", "43.035826,-87.911758", "43.035885,-87.912104", "43.035883,-87.916883", "43.035925, -87.919330", "43.035832,-87.920959", "43.035763,-87.921985", "43.035687,-87.922398", "43.035498,-87.922874", "43.035174,-87.923266", "43.034910,-87.923555", "43.034547,-87.923700", "43.034162,-87.923772", "43.034185,-87.923762", "43.033747, -87.923669", "43.030856,-87.923132", "43.030161,-87.923111", "43.029285,-87.923039", "43.028508,-87.923008", "43.027942,-87.922791", "43.025616,-87.921998", "43.023689,-87.921303", "43.022283,-87.920753", "43.021004,-87.919435", "43.019499,-87.917727", "43.018882,-87.917233", "43.017994,-87.916636", "43.017136,-87.916307", "43.015150,-87.916183", "43.010664,-87.916204", "43.001393,-87.916386", "43.000832,-87.916381", "42.999484,-87.915904", "42.997771,-87.915687", "42.995424,-87.915362", "42.991396,-87.914842", "42.988257,-87.914417", "42.987400,-87.914417", "42.986532,-87.914577", "42.984784,-87.915444", "42.983353,-87.916246", "42.982578,-87.916567", "42.982109,-87.916615", "42.980996,-87.916724", "42.979271,-87.916434", "42.975884,-87.915676", "42.973400,-87.915748", "42.970168,-87.915705", "42.968889,-87.916145", "42.967900,-87.917029", "42.967027,-87.918778", "42.966717,-87.921045", "42.965960,-87.929687", "42.965805,-87.930814", "42.965417,-87.932113", "42.964621,-87.933372", "42.963884,-87.934194", "42.963031,-87.934737", "42.961818,-87.934909", "42.960081,-87.935011", "42.958393,-87.935192", "42.955159,-87.935389", "42.951850,-87.935414", "42.949479,-87.935320", "42.942386,-87.935455", "42.939357,-87.935502", "42.936091,-87.935735", "42.934015,-87.935643", "42.932790,-87.935922", "42.930884,-87.936294", "42.930508,-87.936345", "42.930359,-87.937068", "42.930326,-87.939487", "42.929168,-87.939668", "42.928127,-87.940264", "42.925761,-87.940379", "42.923580,-87.940425", "42.922858,-87.940356", "42.920374,-87.940447", "42.919015,-87.940653", "42.918684,-87.940767", "42.918700,-87.940292", "42.918601,-87.939862"];

    apiPaths.push(path);

    for (let index = 0; index < apiPaths.length; index++) {
        let apiPath = apiPaths[index].join("|");
        $.ajax({
            type: "GET",
            url: `https://roads.googleapis.com/v1/snapToRoads?path=${apiPath}&interpolate=true&key=${apiKey}`,
            success: function(snappedPoints) {
                for (let index = 0; index < snappedPoints.snappedPoints.length; index++) {
                    if (snappedPoints.snappedPoints[index].location.latitude !== latLongs[latLongs.length - 1].latitude && snappedPoints.snappedPoints[index].location.longitude !== latLongs[latLongs.length - 1].longitude) {
                        let latLong = {
                            latitude: snappedPoints.snappedPoints[index].location.latitude,
                            longitude: snappedPoints.snappedPoints[index].location.longitude
                        };
                        latLongs.push(latLong);
                    }
                }
                for (let index = 1; index < latLongs.length - 1; index++) {
                    distance += GetDistanceFromLatLonInKm(latLongs[index].latitude, latLongs[index].longitude, latLongs[index + 1].latitude, latLongs[index + 1].longitude);
                }

                distance /= 1.609;
                console.log(distance);
            }
        })
    }
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