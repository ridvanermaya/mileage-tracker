$(`#navbar`).load(`Navbar.html`);

let path = new Array();
let intervalId;

function StartTracking() {
    path = new Array();
    $(`#body`).empty();
    
    let endTrackingButton = `<button class="btn btn-danger" type="button" onclick="EndTracking()">End Tracking</button>`;

    $(`#body`).append(endTrackingButton);

    GetLocation();

    intervalId = setInterval(GetLocation, 1000);
}

function EndTracking() {
    $(`#body`).empty();

    let startTrackingButton = `<button class="btn btn-primary" onclick="StartTracking()" type="button">Start Tracking</button>`;
    let longLats = [{latitude: 0, longitude: 0}];

    $(`#body`).append(startTrackingButton);

    clearInterval(intervalId);

    let apiPath = path.join("|");
    let placeIdsJoined;

    $.ajax({
        type: "GET",
        url: "https://roads.googleapis.com/v1/snapToRoads?path=-35.27801,149.12958|-35.28032,149.12907|-35.28099,149.12929|-35.28144,149.12984|-35.28194,149.13003|-35.28282,149.12956|-35.28302,149.12881|-35.28473,149.12836&interpolate=true&key=",
        success: function(snappedPoints) {
            for (let index = 0; index < snappedPoints.snappedPoints.length; index++) {
                if (snappedPoints.snappedPoints[index].location.latitude !== longLats[longLats.length - 1].latitude && snappedPoints.snappedPoints[index].location.longitude !== longLats[longLats.length - 1].longitude) {
                    let longLat = {
                        latitude: snappedPoints.snappedPoints[index].location.latitude,
                        longitude: snappedPoints.snappedPoints[index].location.longitude
                    }
                    longLats.push(longLat);
                }
            }

            let distance = 0;

            for (let index = 1; index < longLats.length - 1; index++) {
                distance += getDistanceFromLatLonInKm(longLats[index].latitude, longLats[index].longitude, longLats[index +1].latitude, longLats[index + 1].longitude);
            }

            console.log(distance / 1.609);
            // console.log(snappedPoints);
            // let origins = new Array();
            // let destinations = new Array();

            // for (let index = 0; index < placeIds.length - 1; index++) {
            //     origins.push(placeIds[index]);
            // }

            // for (let index = 1; index < placeIds.length; index++) {
            //     destinations.push(placeIds[index]);
            // }

            // let originsStr = origins.join("|place_id:");
            // let destinationsStr = destinations.join("|place_id:");

            // let distnace2 = getDistanceFromLatLonInKm(-35.2784195, 149.12946589999999, -35.2792731, 149.129338099999985);

            // console.log(distnace2 / 1.609);
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

function getDistanceFromLatLonInKm(lat1,lon1,lat2,lon2) {
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