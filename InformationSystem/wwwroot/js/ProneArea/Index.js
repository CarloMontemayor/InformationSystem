
// This example uses SVG path notation to add a vector-based symbol
// as the icon for a marker. The resulting icon is a marker-shaped
// symbol with a blue fill and no border.
//let map;
let locations;
var icon = "https://www.iconpacks.net/icons/2/free-location-icon-2955-thumb.png";
function initMap() {

    //var locations = [
    //    //['loan 1', 33.890542, 151.274856, 'address 1'],
    //    //['loan 2', 33.923036, 151.259052, 'address 2'],
    //    //['loan 3', 34.028249, 151.157507, 'address 3'],
    //    //['loan 4', 33.80010128657071, 151.28747820854187, 'address 4'],
    //    //['loan 5', 33.950198, 151.259302, 'address 5']
    //];

    $.ajax({
        url: "/AccidentProne/GetAccidentProneMap",
        type: "POST",
        success: function (result) {
            locations = result;
            initialize();
        }
    });



}

function initialize() {

    var myOptions = {
        center: new google.maps.LatLng(15.32254044732912, 120.65272384788983),
        zoom: 15,
        mapTypeId: google.maps.MapTypeId.ROADMAP

    };
    var map = new google.maps.Map(document.getElementById("map"),
        myOptions);

    setMarkers(map, locations)

}

function setMarkers(map, locations) {
    console.log(locations);
    var marker, i;

    for (i = 0; i < locations.length; i++) {

        var title = locations[i].name;
        var lat = locations[i].latitude;
        var long = locations[i].longitude;

        latlngset = new google.maps.LatLng(lat, long);

        var marker = new google.maps.Marker({
            map: map, title: title, position: latlngset,
            icon: {
                url: icon,
                scaledSize: new google.maps.Size(28, 31)
            }
        });
        map.setCenter(marker.getPosition())


        var content = "Accident Prone Area: <a target='_blank' href='/AccidentProne'><h5>" + title + '</h5></a>';

        var infowindow = new google.maps.InfoWindow()

        google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
            return function () {
                infowindow.setContent(content);
                infowindow.open(map, marker);
            };
        })(marker, content, infowindow));

    }
}
    //map = new google.maps.Map(document.getElementById("map"), {
    //    center: new google.maps.LatLng(15.32254044732912, 120.65272384788983),
    //    zoom: 17,
    //});
    //const iconBase =
    //    "https://developers.google.com/maps/documentation/javascript/examples/full/images/";
    //const icons = {
    //    parking: {
    //        icon: iconBase + "parking_lot_maps.png",
    //    },
    //    library: {
    //        icon: iconBase + "library_maps.png",
    //    },
    //    info: {
    //        icon: iconBase + "info-i_maps.png",
    //    },
    //};
    //var icon = "https://www.pngrepo.com/download/133496/hospital-location.png";
    //const features = [
    //    {
    //        position: new google.maps.LatLng(15.32254044732912, 120.65272384788983),
    //        type: "info",
    //    },
    //];

    //// Create markers.
    //for (let i = 0; i < features.length; i++) {
    //    var infowindow = new google.maps.InfoWindow({
    //        content: 'Welcome to stackoverflow!'
    //    });
    //    var marker = new google.maps.Marker({
    //        position: features[i].position,
    //        icon: {
    //            url: icon,
    //            scaledSize: new google.maps.Size(28, 31)
    //        },
    //        map: map,
    //    });
    //    infowindow.open(map, marker);
    //}
/*}*/